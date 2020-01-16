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
    public partial class FormFunc : Form
    {
        KeyValuePair<FunctionsLib.basic.FunctionWithParameters<double>, string> func;
        FunctionsLib.basic.FunctionWithParameters<double> lastKey;
        FunctionsLib.FunctionBuilder<double> builder;
        FormMain form;

        public FormFunc(KeyValuePair<FunctionsLib.basic.FunctionWithParameters<double>, string> func, FunctionsLib.FunctionBuilder<double> build, FormMain form)
        {
            InitializeComponent();
            textBox1.Text = func.Value;
            this.func = func;
            builder = build;
            this.form = form;
            lastKey = func.Key;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            string text = "";
            for (int i = 0; i < textBox1.Text.Length; i++)
                if (textBox1.Text[i] != ' ')
                    text += textBox1.Text[i];
            if (text == "") buttonDelete_Click(sender, e);
            try
            {
                func = new KeyValuePair<FunctionsLib.basic.FunctionWithParameters<double>, string>(builder.Create(text), textBox1.Text);
                form.ChangetFunc(lastKey, func);
                lastKey = func.Key;
            }
            catch
            {
                MessageBox.Show("Не верно введен график.\nКлючевые слова: sin cos tg ln arcsin arccos arctg sgn e pi");
            }
        }
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            form.DeleteFunc(lastKey);
            Close();
        }
    }
}
