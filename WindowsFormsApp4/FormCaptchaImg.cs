using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public partial class FormCaptchaImg : Form
    {
        private Color textBoxColor;
        private string HTMLCode;
        private ProgrammMethods pm;
        private bool isClosing = true;
        public FormCaptchaImg(string HTMLCode, ProgrammMethods pm)
        {
            InitializeComponent();
            textBoxColor = textBox1.ForeColor;
            this.HTMLCode = HTMLCode;
            webBrowser1.DocumentText = HTMLCode;
            this.pm = pm;
            
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Введите результат сюда")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "Введите результат сюда";
                textBox1.ForeColor = textBoxColor;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            userEnt();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(Convert.ToInt32(e.KeyChar) == 13)
            {
                userEnt();
            }
        }
        private void userEnt()
        {
            if (textBox1.Text != "")
            {
                this.Hide();

                pm.userword = textBox1.Text;
                pm.UserAnswer = true;
                isClosing = false;
                this.Close();
            }

        }

        private void FormCaptchaImg_Load(object sender, EventArgs e)
        {
            if (textBox1.CanSelect)
            {
                textBox1.Select();
            }
        }

        private void FormCaptchaImg_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isClosing)
            {
                this.Hide();
                pm.userword = "0";
                pm.UserAnswer = true;
            }
        }
    }
}
