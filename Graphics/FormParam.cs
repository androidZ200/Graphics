using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using гравики_и_производные;

namespace Graphics
{
    public partial class FormParam : Form
    {
        string paramName;
        FormMain form;
        double max = 10;
        double min = 0;

        public FormParam(FormMain form, string name)
        {
            InitializeComponent();
            paramName = name;
            this.form = form;
            Text = paramName;
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            try
            {
                max = Convert.ToDouble(textBox1.Text);
            }
            catch
            {
                textBox1.Text = max.ToString();
            }
        }
        private void textBox2_Leave(object sender, EventArgs e)
        {
            try
            {
                min = Convert.ToDouble(textBox2.Text);
            }
            catch
            {
                textBox2.Text = min.ToString();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if(min > max)
            {
                double t = min;
                min = max;
                max = t;
                textBox1.Text = max.ToString();
                textBox2.Text = min.ToString();
            }
            double val = min + (max - min) * trackBar1.Value / 100d;
            label3.Text = val.ToString();
            form.ChangetParametr(paramName, val);
        }
    }
}
