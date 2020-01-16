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
using FunctionsLib.basic;
using Graphics;

namespace гравики_и_производные
{
    public partial class FormMain : Form
    {
        FunctionBuilder<double> builder;
        Dictionary<FunctionWithParameters<double>, string> functions;
        SortedSet<string> parametrs;
        Camera camera;
        Thread drawing;
        bool isClickMouse = false;
        Point lastMouseCoordinate;
        object drawinglock = new object();

        public FormMain()
        {
            InitializeComponent();
            pictureBox1.MouseWheel += PictureBox1_MouseWheel;
            string[] variables = { "x" };
            builder = new FunctionBuilder<double>(variables);
            camera = new Camera(pictureBox1);
            functions = new Dictionary<FunctionWithParameters<double>, string>();
            parametrs = new SortedSet<string>();
            drawing = new Thread(DrawLoop);
            drawing.Start();
            UpdateComboBox();
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

            lock (drawinglock)
                foreach (var function in functions)
                {
                    for (int i = 0; i < pictureBox1.Width; i++)
                    {
                        prev = curr;
                        curr.X = i;
                        curr.Y = camera.inCamera(new PointF(0, (float)function.Key.get(camera.fromCamera(curr).X))).Y;
                        if (!Double.IsNaN(curr.X) && !Double.IsNaN(curr.Y) && !Double.IsNaN(prev.X) && !Double.IsNaN(prev.Y) &&
                            curr.Y > -pictureBox1.Height && prev.Y > -pictureBox1.Height &&
                            curr.Y < 2 * pictureBox1.Height && prev.Y < 2 * pictureBox1.Height)
                            g.DrawLine(line, curr, prev);
                    }
                    prev = new PointF(float.NaN, 0);
                    curr = new PointF(float.NaN, 0);
                }
            Invoke((Action)(() => { pictureBox1.Image = bmp; }));
        }
        private void DrawLoop()
        {
            while (true)
                Draw();
        }
        public void ChangetFunc(FunctionWithParameters<double> key, KeyValuePair<FunctionWithParameters<double>, string> newFunc)
        {
            lock (drawinglock)
            {
                functions.Remove(key);
                functions.Add(newFunc.Key, newFunc.Value);
            }
            UpdateParametrs();
            UpdateComboBox();
        }
        public void DeleteFunc(FunctionWithParameters<double> key)
        {
            lock (drawinglock)
                functions.Remove(key);
            UpdateParametrs();
            UpdateComboBox();
        }
        public void ChangetParametr(string name, double value)
        {
            foreach (var x in functions)
                foreach (var p in x.Key.parameters)
                    if (name == p.Key) p.Value.a = value;
        }
        private void UpdateComboBox()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Add");
            foreach (var x in functions)
                comboBox1.Items.Add(x.Value);
            foreach (var x in parametrs)
                comboBox1.Items.Add(x);
            comboBox1.SelectedIndex = 0;
        }
        private void UpdateParametrs()
        {
            parametrs.Clear();
            foreach (var x in functions)
                foreach (var p in x.Key.parameters)
                    parametrs.Add(p.Key);
        }

        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            camera.Zoom(e.Delta > 0);
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (drawing != null) drawing.Abort();
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
        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                FunctionWithParameters<double> f = new FunctionWithParameters<double>(new NaN<double>());
                lock (drawinglock)
                    functions.Add(f, "");
                FormFunc form = new FormFunc(new KeyValuePair<FunctionWithParameters<double>, string>(f, ""), builder, this);
                form.Show();
            }
            else if (comboBox1.SelectedIndex < functions.Count + 1)
            {
                var enumer = functions.GetEnumerator();
                for (int i = 0; i < comboBox1.SelectedIndex; i++) enumer.MoveNext();
                FormFunc form = new FormFunc(enumer.Current, builder, this);
                form.Show();
            }
            else
            {
                int i = comboBox1.SelectedIndex - functions.Count - 1;
                FormParam form = new FormParam(this, parametrs.ElementAt(i));
                form.Show();
            }
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
