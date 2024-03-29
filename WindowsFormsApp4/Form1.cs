﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Awesomium.Windows.Forms;
using Microsoft.Win32;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        ProgrammMethods pm;
        public bool PageLoad = false;
        private string registrPath = @"Software\MyProgrammDeff83";
        private bool Hide = false;
        private string cmdargs;
        public string typeCloseTab = null;
        public bool isClosing = false;
        public string patternForFind;
        public string pathbalanceFile;

        public RegistryKey currentUser { get; private set; }
        public RegistryKey myProgramm { get; private set; }

        public Form1(string[] args)
        {
            InitializeComponent();
            if (args.Length > 0)
                if (args[0] != null)
                {
                    cmdargs = args[0];
                };
            

        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
            pm = new ProgrammMethods(tabControl1, webSessionProvider1, this, notifyIcon1, addressBox1, panelosn, timer1);
            pm.AddPages(tabControl1);
            

            //меню контекстное значка в трее
            настройкиToolStripMenuItem.Click += НастройкиToolStripMenuItem_Click;
            выходToolStripMenuItem.Click += ВыходToolStripMenuItem_Click;
            this.Resize += Form1_Resize;
            initial();
            //for TEST deleted
            // this.Visible = false;
            //  this.ShowInTaskbar = false;
            // autostart();

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void ВыходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //выход из программы
            Application.Exit();
        }

        private void НастройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //new FormShowWeb(webControl()).Show();
        }

        private void initial()
        {
            //инициализация программы
            currentUser = Registry.CurrentUser;
            string path, balance;
            if (cmdargs != null) { path = cmdargs; }//если была комманда из cmd с параметром
            else
            {
                using (myProgramm = currentUser.CreateSubKey(registrPath))
                {
                    path = (string)myProgramm.GetValue("pathConfig");
                };
            }
            if (path != null)
            {
                textBoxFolderPath.Text = path;

                using (StreamReader filereaderStream = new StreamReader(textBoxFolderPath.Text))
                {
                    string lineCmd = filereaderStream.ReadLine();
                    if (lineCmd != null)
                    {
                        switch (lineCmd)
                        {
                            case "[AUTORUNHIDE]":
                                {
                                    /// автозапуск с скрытием окна
                                    Hide = true;
                                    autostart();
                                }
                                break;
                            case "[AUTORUN]":
                                {
                                    /// автозапуск
                                    autostart();
                                }
                                break;
                            case "[DEBUGAUTORUN]":
                                {
                                    /// автозапуск
                                    pm.isDebug = true;
                                    FormLogs formlogs = new FormLogs();
                                    pm.formlog = formlogs;
                                    formlogs.Show();
                                    formlogs.DownLog(pm.logystring);
                                    autostart();
                                }
                                break;

                        }

                    }
                };
            }
            using (myProgramm = currentUser.CreateSubKey(registrPath))
            {
                balance = (string)myProgramm.GetValue("pathBalance");
                pathbalanceFile = (string)myProgramm.GetValue("pathBalanceFile");
            };
            if (balance != null)
            {
                textBoxBalance.Text = balance;
                pm.isBalanceUse = true;
                pm.balancefailPath = balance;
            }
            if (pathbalanceFile != null)
            {
                textBoxBalanceFile.Text = pathbalanceFile;
            }
            notifyIcon1.ContextMenuStrip = contextMenuStrip2;
            notifyIcon1.Visible = true;
        }


        private void autostart()
        {
            ///здесь действия для авто запуска
            goautofiled();
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

        private void button6_Click(object sender, EventArgs e)
        {
            new FormCaptchaImg("<center><img src='https://adbtc.top/1captcha/1561383462.5671.jpg' style='width: 150; height: 87; border: 0;' alt=' '></center> ", pm).Show();
        }
        private async void goautofiled()
        {
            await Task.Run(() =>
            {
                Invoke((Action)(() => { if(Hide) Hide(); }));
                pm.isworking = true;
                pm.bot_writer_fileConfig(textBoxFolderPath.Text, webControl(), labelInfo);
                pm.showNotify("Deff83-botADB", "bot завершил работу", ToolTipIcon.Warning);
                pm.isworking = false;
            });
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            textBoxFolderPath.Text = ((OpenFileDialog)sender).FileName;
            using (myProgramm = currentUser.OpenSubKey(registrPath, true))
                myProgramm.SetValue("pathConfig", textBoxFolderPath.Text);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        
        private void button9_Click(object sender, EventArgs e)
        {
            goautofiled();
        }

        private void logsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormLogs formlogs = new FormLogs();
            pm.formlog = formlogs;
            formlogs.Show();
            formlogs.DownLog(pm.logystring);
        }

        private void показатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void стопToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pm.isworking = false;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            textBoxBalance.Text = ((OpenFileDialog)sender).FileName;
            pm.balancefailPath = textBoxBalance.Text;
            using (myProgramm = currentUser.OpenSubKey(registrPath, true))
                myProgramm.SetValue("pathBalance", textBoxBalance.Text);
            pm.isBalanceUse = true;
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(isClosing)
            e.Cancel = true;
        }
        //если окно открыто как formcaptcha то показывать его даже при скрытии
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (typeCloseTab != null)
            {
                switch (typeCloseTab)
                {
                    case "notClosing":
                        int i = 0;
                        bool isCloseTabsAny = false;
                        while(tabControl1.TabCount > 2)
                        {
                            string adres = addressBox1.AccessibilityObject.Value;
                            if (Regex.Matches(adres, patternForFind).Count  > 0)
                            {
                                //пропустить
                                i++;
                            }
                            else
                            {
                                //закрыть вкладку
                            }
                            {
                                if (tabControl1.TabCount > 2)
                                {
                                    tabControl1.TabPages[tabControl1.TabCount - 2 - i].Controls[0].Dispose();
                                    tabControl1.TabPages[tabControl1.TabCount - 2 - i].Dispose();
                                    isCloseTabsAny = true;
                                    
                                }
                            }
                        }
                        pm.newWebcontroll = tabControl1.TabPages[0].Controls[0] as WebControl;
                        if (isCloseTabsAny)
                        {
                            pm.UserAnswer = true;
                        }
                        break;
                }
            }
            else
            {
                tabControl1.SelectedIndex = 0;

                {

                    if (tabControl1.TabCount > 2)
                    {
                        tabControl1.TabPages[tabControl1.TabCount - 2].Controls[0].Dispose();
                        tabControl1.TabPages[tabControl1.TabCount - 2].Dispose();
                    }
                }
            }
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void openFileDialogBalanseFile_FileOk(object sender, CancelEventArgs e)
        {
            textBoxBalanceFile.Text = ((OpenFileDialog)sender).FileName;
            using (myProgramm = currentUser.OpenSubKey(registrPath, true))
                myProgramm.SetValue("pathBalanceFile", textBoxBalanceFile.Text);
            pathbalanceFile = textBoxBalanceFile.Text;
        }

        private void buttonBalanceFile_Click(object sender, EventArgs e)
        {
            openFileDialogBalanseFile.ShowDialog();
        }
    }
}
