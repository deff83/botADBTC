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
    public partial class FormLogs : Form
    {
        public FormLogs()
        {
            InitializeComponent();
            textBox1.Text = "";
        }
        public void RefreshLog(string logi)
        {
            textBox1.AppendText(logi);
        }
        public void DownLog(string logs)
        {
            textBox1.Text = logs;
        }
    }
}
