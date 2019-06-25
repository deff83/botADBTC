using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Awesomium.Windows.Forms;

namespace WindowsFormsApp4
{
    public class ProgrammMethods
    {
        NavigationOnWebControl NOW;
        public TabControlAwesomium TCA;
        private Form formGUI;
        /// поле если дебаг режим то выводить информацию о работе бота
        public bool isDebug = false;
        /// <summary>
        /// PageLoad - страница загрузилась с событием Loaded
        /// UserAnswer - ответ от User получен
        /// </summary>
        public bool PageLoad = false;
        public bool UserAnswer = false;
        public int startPosition = 0;
        public int nextPosition = 0;
        ///     {atribstring} - аттрибут каконебуть элемента
        ///     {userword} - возвращенное значение от user
        public string userword;
        private string atribstring;
        private int countProgramm;

        public ProgrammMethods(TabControl tabControl, WebSessionProvider webSessionProvider, Form1 form1)
        {
            NOW = new NavigationOnWebControl();
            TCA = new TabControlAwesomium(tabControl, webSessionProvider, this);
            this.formGUI = form1;
        }

        public void AddPages(TabControl tabControl) // При загрузки формы добавляет 2 вкладки на TabControl
        {
            tabControl.TabPages.Insert(1, "   +");
            TCA.AddPage();
        }

        private void waitPage(int sec)
        {
            PageLoad = false;
            int secstart = sec;
            while (!PageLoad&& secstart>1)
            {
                secstart--;
                Thread.Sleep(1000);
            }

        }
        private void waitUser()
        {
            UserAnswer = false;
            while (!UserAnswer)
            {
                Thread.Sleep(1000);
            }

        }


        public void TabControlSelecting(TabControlCancelEventArgs e, AddressBox addressBox) //Добавляет страницу -> выбираем или выбирает эту страницу -> задает этой странице addressBox и задает текст addressBox(а) этой страницы
        {

            if (e.TabPage.Text == "   +")
            {
                TCA.AddPage();
            }
            else
            {
                TCA.SetWebControlInAddressBox(addressBox);
                TCA.SetTextAddressBox(addressBox);
            }

        }

        public void bot_writer_fileConfig(string pathFileconfig, WebControl webControl, Label labelinfor)
        {
            using (StreamReader filereaderStream = new StreamReader(pathFileconfig))
            {
                string lineCmd;
                //переменные для поиска и сохранения результата
                //пропустить первую строчку
                lineCmd = filereaderStream.ReadLine();
                countProgramm = 1;

                while ((lineCmd = filereaderStream.ReadLine()) != null)
                {
                    countProgramm++;
                    if (countProgramm == nextPosition) continue;
                    string[] commandsSplit = lineCmd.Split('|');
                    string command = commandsSplit[0];
                    
                    formGUI.BeginInvoke((Action)(()=> {
                        labelinfor.Text = "line " + countProgramm + ": " + lineCmd;
                    }));
                    //Thread.Sleep(2000);
                    switch (command)
                    {
                        case "webGo":
                            {
                                /// загрузка страници
                                /// commandsSplit[1] - URI страницы для загрузки
                               formGUI.Invoke((Action)(() => {
                                   webControl.Source = new Uri(commandsSplit[1]);
                               }));
                            }
                            break;
                        case "waitPage":
                            {
                                /// ждать страницу - событи Loaded
                                /// commandsSplit[1] - время в секундах скока прождать
                                waitPage(Int32.Parse(commandsSplit[1]));
                               
                            }
                            break;
                        case "WriteInField":
                            {
                                formGUI.Invoke((Action)(() => {
                                    /// ищется перое поле по поиску и заполняется
                                    /// commandsSplit[1] - по какому типу поиск поля
                                    /// commandsSplit[2] - имя типа для поиска
                                    /// commandsSplit[3] - что вставит в найденное поле
                                    NOW.WriteInField(webControl, getNavigOnWebControl(commandsSplit[1]), commandsSplit[2], DeformateStringForWords(commandsSplit[3])); 
                                }));
                            }
                            break;
                        case "GetAtribInDivInDiv":
                            {
                                formGUI.Invoke((Action)(() => {
                                    /// ищется атрибут у элемента в элементе - найденное записывается в переменную atribstring
                                    /// commandsSplit[1] - по какому типу поиск поля первого элемента
                                    /// commandsSplit[2] - имя типа для поиска первого элемента
                                    /// commandsSplit[3] - какой из найденный элемент из списка использовать
                                    /// commandsSplit[4] - по какому типу поиск поля внутреннего элемента
                                    /// commandsSplit[5] - имя типа для поиска внутреннего элемента
                                    /// commandsSplit[6] - какой из найденный элемент из списка использовать
                                    /// commandsSplit[7] - какой атрибут нужен
                                    atribstring = NOW.GetAtribInDivInDiv(webControl, getNavigOnWebControl(commandsSplit[1]), commandsSplit[2], Int32.Parse(commandsSplit[3]), getNavigOnWebControl(commandsSplit[4]), commandsSplit[5], Int32.Parse(commandsSplit[6]), commandsSplit[7]);
                                }));
                            }
                            break;
                        case "FormHTML":
                            {
                                formGUI.Invoke((Action)(() => {
                                    /// открытие нового окна с html кодом
                                    /// commandsSplit[1] - HTML код с параметрами 
                                    new FormCaptchaImg(DeformateStringForWords(commandsSplit[1]), this).Show();
                                }));
                            }
                            break;
                        case "waitUser":
                                /// ждать ответа от формы Юзера
                                waitUser();
                            break;
                        case "PressButton":
                            {
                                formGUI.Invoke((Action)(() => {
                                    /// ищется атрибут у элемента в элементе - найденное записывается в переменную atribstring
                                    /// commandsSplit[1] - по какому типу поиск кнопки input
                                    /// commandsSplit[2] - имя типа для поиска кнопки
                                    /// commandsSplit[3] - номер элемента
                                    /// commandsSplit[4] - действие с кнопкой (Например click)
                                    NOW.PressButton(webControl, getNavigOnWebControl(commandsSplit[1]), commandsSplit[2], Int32.Parse(commandsSplit[3]), commandsSplit[4]);
                                }));
                            }
                            break;
                        case "PressButtonInDIV":
                            {
                                formGUI.Invoke((Action)(() => {
                                    /// ищется атрибут у элемента в элементе - найденное записывается в переменную atribstring
                                    /// commandsSplit[1] - по какому типу поиск элемента в котором кнопка 
                                    /// commandsSplit[2] - имя типа для поиска элемента
                                    /// commandsSplit[3] - номер элемента
                                    /// commandsSplit[4] - по какому типу поиск кнопки input
                                    /// commandsSplit[5] - имя типа для поиска кнопки
                                    /// commandsSplit[6] - номер элемента
                                    /// commandsSplit[7] - действие с кнопкой (Например click)
                                    NOW.PressButtonInDIV(webControl, getNavigOnWebControl(commandsSplit[1]), commandsSplit[2], Int32.Parse(commandsSplit[3]), getNavigOnWebControl(commandsSplit[4]), commandsSplit[5], Int32.Parse(commandsSplit[6]), commandsSplit[7]);
                                }));
                            }
                            break;
                        case "[SUB_PROGRAMM]":
                            {
                                ///меткак куда перейти
                                startPosition = countProgramm;
                            }
                            break;
                        case "[IF]":
                            {
                                ///оператор сравнения
                                ///commandsSplit[1] - первое поле для сравнения
                                ///commandsSplit[2] - второе поле для сравнения
                                ///commandsSplit[3] - переход на стр если true
                                ///commandsSplit[4] - переход если false
                                if (DeformateStringForWords(commandsSplit[1]) == DeformateStringForWords(commandsSplit[2]))
                                    gotoline(Int32.Parse(commandsSplit[3]), filereaderStream); 
                                else
                                    gotoline(Int32.Parse(commandsSplit[4]), filereaderStream); ;

                            }
                            break;
                        case "[GOTO_SUB_PROGRAMM]":
                            {
                                ///меткак куда перейти
                                nextPosition = countProgramm;
                                filereaderStream.DiscardBufferedData();
                                filereaderStream.BaseStream.Seek(0, SeekOrigin.Begin);
                                countProgramm = startPosition;
                                for (int pos = 0; pos < startPosition; pos++) filereaderStream.ReadLine();
                            }
                            break;
                        case "[GOTO]":
                            {
                                ///меткак куда перейти
                                ///commandsSplit[1] - строка куда перейти
                                int stroka = Int32.Parse(commandsSplit[1]);
                                gotoline(stroka, filereaderStream);
                            }
                            break;

                    }
                }

            }
        }
        private void gotoline(int line, StreamReader stream)
        {
            stream.DiscardBufferedData();
            stream.BaseStream.Seek(0, SeekOrigin.Begin);
            countProgramm = line - 1;
            for (int pos = 0; pos < line - 1; pos++) stream.ReadLine();
        }
        private NavigationOnWebControl.GetElementBy getNavigOnWebControl(string type) {
            switch (type)
            {
                case "Name":
                    return NavigationOnWebControl.GetElementBy.Name;
                    break;
                case "ClassName":
                    return NavigationOnWebControl.GetElementBy.ClassName;
                    break;
                case "Id":
                    return NavigationOnWebControl.GetElementBy.Id;
                    break;
                case "TagName":
                    return NavigationOnWebControl.GetElementBy.TagName;
                    break;
            }
            //дефолтно
            return NavigationOnWebControl.GetElementBy.Name;
        }
        public string DeformateStringForWords(string instring)
        {
            return instring
                .Replace("{userword}", userword)
                .Replace("{atribstring}", atribstring);
        }
        
    }
}