using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using FunctionsLib;

namespace гравики_и_производные
{
    public partial class FormMain : Form
    {
        IFunction function;
        Camera camera;
        Thread drawing;
        bool isClickMouse = false;
        Point lastMouseCoordinate;

        public FormMain()
        {
            InitializeComponent();
            pictureBox1.MouseWheel += PictureBox1_MouseWheel;
            camera = new Camera(pictureBox1);
            function = new FunctionsLib.basic.NaN();
            drawing = new Thread(DrawLoop);
            drawing.Start();
        }

        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            camera.Zoom(e.Delta > 0);
        }
        private void BuildButton_Click(object sender, EventArgs e)
        {
            string text = "";
            for (int i = 0; i < textBox1.Text.Length; i++)
                if (textBox1.Text[i] != ' ')
                    text += textBox1.Text[i];
            try
            {
                function = FunctionBuilder.Create(text);
            }
            catch
            {
                MessageBox.Show("Не верно введен график.\nКлючевые слова: sin cos tg ln arcsin arccos arctg sgn e pi");
                function = new FunctionsLib.basic.NaN();
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string ValidCharacters = "Eqwertyuiopasdfghjklzxcvbnm0123456789+-()*/^%, ";
            string newText = "";
            string oldText = textBox1.Text;
            for (int i = 0; i < oldText.Length; i++)
                for (int j = 0; j < ValidCharacters.Length; j++)
                    if (oldText[i] == ValidCharacters[j]) newText += oldText[i];
            textBox1.Text = newText;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (drawing != null) drawing.Abort();
        }

        private void Draw()
        {
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
            PointF prev = new PointF(float.NaN, 0);
            PointF curr = new PointF(float.NaN, 0);

            Pen line = new Pen(Color.Red);
            PointF center = camera.inCamera(PointF.Empty);
            g.DrawLine(new Pen(Color.Black), 0, center.Y, pictureBox1.Width, center.Y);
            g.DrawLine(new Pen(Color.Black), center.X, 0, center.X, pictureBox1.Height);

            for (int i = 0; i < pictureBox1.Width; i++)
            {
                prev = curr;
                curr.X = i;
                curr.Y = camera.inCamera(new PointF(0, (float)function.get(camera.fromCamera(curr).X))).Y;
                if(!Double.IsNaN(curr.X) && !Double.IsNaN(curr.Y) && !Double.IsNaN(prev.X) && !Double.IsNaN(prev.Y) &&
                    curr.Y > -pictureBox1.Height && prev.Y > -pictureBox1.Height &&
                    curr.Y < 2 * pictureBox1.Height && prev.Y < 2 * pictureBox1.Height)
                    g.DrawLine(line, curr, prev);
            }
            Invoke((Action)(() => { pictureBox1.Image = bmp; }));
        }
        private void DrawLoop()
        {
            while(true)
                Draw();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            isClickMouse = true;
            lastMouseCoordinate = e.Location;
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isClickMouse = false;
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isClickMouse)
            {
                Point t = new Point(lastMouseCoordinate.X - e.X, lastMouseCoordinate.Y - e.Y);
                lastMouseCoordinate = e.Location;
                camera.Move(t);
            }
            var coord = camera.fromCamera(e.Location);
            toolTip1.SetToolTip(this.pictureBox1, "X = " + coord.X + "\nY = " + coord.Y);
        }
        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.ShowAlways = true;
        }
        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.ShowAlways = false;
        }
    }

    public class Camera
    {
        private PictureBox pb;
        private PointF Coordinate = new PointF(0, 0);
        private float zoom = 50;

        public Camera(PictureBox pictureBox)
        {
            pb = pictureBox;
        }

        public PointF inCamera(PointF point)
        {
            point.X *= zoom;
            point.Y *= zoom;
            point.X += pb.Width / 2.0f - Coordinate.X;
            point.Y += pb.Height / 2.0f - Coordinate.Y;
            point.Y = pb.Height - point.Y;
            return point;
        }
        public PointF fromCamera(PointF pixel)
        {
            pixel.Y = pb.Height - pixel.Y;
            pixel.X -= pb.Width / 2.0f - Coordinate.X;
            pixel.Y -= pb.Height / 2.0f - Coordinate.Y;
            pixel.X /= zoom;
            pixel.Y /= zoom;
            return pixel;
        }
        public void Move(PointF step)
        {
            Coordinate.X += step.X;
            Coordinate.Y -= step.Y;
        }
        public void Zoom(bool increase)
        {
            if (increase) { zoom *= 1.1f; Coordinate.X *= 1.1f; Coordinate.Y *= 1.1f; }
            else { zoom /= 1.1f; Coordinate.X /= 1.1f; Coordinate.Y /= 1.1f; }
        }
        public RectangleF getField()
        {
            PointF one = fromCamera(new PointF(0, 0));
            PointF two = fromCamera(new PointF(pb.Width, pb.Height));
            return new RectangleF(one.X, one.Y, two.X - one.X, two.Y - one.Y);
        }
    }
}
