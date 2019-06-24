using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Awesomium.Windows.Forms;


namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        ProgrammMethods pm;
        public bool PageLoad = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pm = new ProgrammMethods(tabControl1, webSessionProvider1, this);
            pm.AddPages(tabControl1);
            splitContainer1.IsSplitterFixed = true;
            //for TEST deleted
            this.Visible = false;
            this.ShowInTaskbar = false;
            autostart();

        }

        private void autostart()
        {
            
            goautorized();
        }

        private void button1_Click(object sender, EventArgs e) // Назад
        {
            webControl().GoBack();
        }

        private void button2_Click(object sender, EventArgs e) // Вперед
        {
            webControl().GoForward();
        }

        private void button3_Click(object sender, EventArgs e) // Обновить
        {
            webControl().Reload(false);
        }

        

        private void button5_Click(object sender, EventArgs e) // "Освободить память"
        {
           pm.TCA.ClearMemory(addressBox1);
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e) //Добавить страницу(если вкладка с текстом  + ) -> выбирает эту страницу -> задает этой странице addressBox и задает текст addressBox(а) этой страницы
        {
           pm.TabControlSelecting(e, addressBox1);
        }

        // ---------------------------> Данными 2-мя методами можно обойтись, если создать свой контрол, как тут [url]http://www.youtube.com/watch?v=DJu2ivQFooc[/url]
        private void tabControl1_MouseDown(object sender, MouseEventArgs e) //При клике по вкладке, определяет эту вкладку
        {
            pm.TCA.TabControlMouseDown(e);
        }

       
        //  <---------------------------

        WebControl webControl() // Возвращает WebControl выбранной вкладки
        {
            return tabControl1.TabPages[tabControl1.SelectedIndex].Controls[0] as WebControl;
        }

        private void з0акрытьВклпToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pm.TCA.CloseTab();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            webControl().Source = new Uri(@"https://adbtc.top");
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string textHTML = webControl().HTML;
            FormXMLView form_xml = new FormXMLView(textHTML);
            form_xml.Show();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            goautorized();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            new FormCaptchaImg("<center><img src='https://adbtc.top/1captcha/1561383462.5671.jpg' style='width: 150; height: 87; border: 0;' alt=' '></center> ", pm).Show();
        }
        private async void goautorized()
        {
            await Task.Run(() =>
            {
                //авторизация на adb.top
                pm.authorizationSign(webControl());
            });
        }
    }
}
