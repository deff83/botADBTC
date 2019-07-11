using Microsoft.Win32;
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
    public partial class FormSlot : Form
    {
        public RegistryKey myProgramm { get; set; }
        public RegistryKey currentUser { get; set; }
        public int locationSlot { get; private set; }
        public int value { get; private set; }

        private string registrPath = @"Software\MyProgrammDeff83"; private ProgrammMethods pm;
        public FormSlot(string text, ProgrammMethods pm)
        {
            InitializeComponent();
            label1.Text = text;
            this.pm = pm;
            locationWind();
        }

        private void locationWind()
        {
            bool isShow = false;
            currentUser = Registry.CurrentUser;
            while (!isShow)
            {
                try
                {
                    string locationSlotStr;
                    char[] c;
                    using (myProgramm = currentUser.CreateSubKey(registrPath))
                    {
                        locationSlotStr = (string) myProgramm.GetValue("locationSlot");
                    };
                    if (locationSlotStr == null)
                    {
                        c = new char[0];
                    }
                    else
                    {
                        c = Encoding.Default.GetChars(Encoding.Default.GetBytes(locationSlotStr));
                    }
                    
                    value = 40;
                    bool ismesto = true;
                    while (ismesto)
                    {
                        ismesto = false;
                        //Создание объекта для генерации чисел
                        Random rnd = new Random();

                        //Получить случайное число (в диапазоне от 0 до 10)
                        value = rnd.Next(0, 64) + 40;//5*13
                        for (int i = 0; i < c.Length; i++)
                        {
                            if (value == c[i]) ismesto = true;
                        }
                    }
                    
                    char bukv =  (char) value;
                    string locatestriSave = new string(c) + bukv;
                    pm.setLog("[warning]", locatestriSave + "_" + value + "_" + new string(c));
                    using (myProgramm = currentUser.OpenSubKey(registrPath, true))
                        myProgramm.SetValue("locationSlot", locatestriSave);
                    
                    isShow = true;
                } catch (Exception e)
                {
                    
                }
            }
        }

        private void FormSlot_Click(object sender, EventArgs e)
        {
            Hide();
            bool isShow = false;
            while (!isShow)
            {
                string locationSlotStr;
                using (myProgramm = currentUser.CreateSubKey(registrPath))
                {
                    locationSlotStr = (string)myProgramm.GetValue("locationSlot");
                };
                string locatestriSave = locationSlotStr.Replace(new string(new char[]{ (char) value}),"");
                using (myProgramm = currentUser.OpenSubKey(registrPath, true))
                    myProgramm.SetValue("locationSlot", locatestriSave);
                pm.UserAnswer = true;
                Close();
                isShow = true;
            }
        }

        private void FormSlot_Load(object sender, EventArgs e)
        {
            this.Location = new Point(((int)((value - 40) % 5)) * this.Width, ((int)((value - 40) / 5)) * this.Height);
            /*for(int i=30; i<170; i++)
                {
                pm.setLog(i +"|", (char)i + "");
                }*/
        }

    }
}
