using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Awesomium.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace WindowsFormsApp4
{
    public class ProgrammMethods
    {
        NavigationOnWebControl NOW;
        public TabControlAwesomium TCA;
        private Form1 formGUI;
        private NotifyIcon notifyIcon1;
        private TabControl tabControl;
        private AddressBox addressBox1;
        private Panel splitContainerUp;
        private Timer timer;
        private WebSessionProvider webSessionProvider;
        ///webControl для удаления
        private WebControl webControlDel;
        /// поле если дебаг режим то выводить информацию о работе бота
        /// isworking - работать ли
        /// isBalanceUse - использовать ли баланс файл
        public bool isDebug = false;
        public bool isworking = false;
        public bool isBalanceUse = false;
        public string balancefailPath;
        /// <summary>
        /// PageLoad - страница загрузилась с событием Loaded
        /// UserAnswer - ответ от User получен
        /// </summary>
        public string logystring { get; set; }
        public string balancestring { get; set; }
        public FormLogs formlog { get; set; }
        public bool PageLoad = false;
        public bool UserAnswer = false;
        private bool TimerLoad = false;

        public int startPosition = 0;
        public int nextPosition = 0;
        ///     {atribstring} - аттрибут каконебуть элемента
        ///     {userword} - возвращенное значение от user
        ///     {saved} - возвращенное значение от user
        public string userword;
        private string atribstring;
        private string saved;
        private int countProgramm;
        private Dictionary<string,string> saveString = new Dictionary<string, string>();
        private Button button_ok;

        public ProgrammMethods(TabControl tabControl, WebSessionProvider webSessionProvider, Form1 form1, NotifyIcon notifyIcon1, AddressBox addressBox1, Panel splitContainerUp, Timer timer)
        {
            NOW = new NavigationOnWebControl();
            TCA = new TabControlAwesomium(tabControl, webSessionProvider, this);
            this.formGUI = form1;
            this.notifyIcon1 = notifyIcon1;
            this.tabControl = tabControl;
            this.webSessionProvider = webSessionProvider;
            this.addressBox1 = addressBox1;
            this.splitContainerUp = splitContainerUp;
            this.timer = timer;
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
            bool exit = true;
            while (exit)
            {
                if (PageLoad) exit = false;
                if (secstart<0) exit = false;
                secstart--;
                Thread.Sleep(1000);
            }

        }
        private void waitTimer(int sec)
        {
            TimerLoad = false;
            int secstart = (int) (sec / 5);
            bool exit = true;
            while (exit)
            {
                if (TimerLoad) exit = false;
                if (secstart < 0) exit = false;
                secstart--;
                Thread.Sleep(5000);
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

                while ((lineCmd = filereaderStream.ReadLine()) != null && isworking)
                {
                    countProgramm++;
                    if (countProgramm == nextPosition) continue;
                    string[] commandsSplit = lineCmd.Split('|');
                    string command = commandsSplit[0];
                    
                    formGUI.BeginInvoke((Action)(()=> {
                        labelinfor.Text = "line " + countProgramm + ": " + lineCmd;
                        
                    }));
                    setLog("line", countProgramm + ": " + lineCmd);
                    //Thread.Sleep(2000);
                    try
                    {
                        switch (command)
                        {
                            case "webGo":
                                {
                                    /// загрузка страници
                                    /// commandsSplit[1] - URI страницы для загрузки
                                    formGUI.Invoke((Action)(() =>
                                    {
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
                                    formGUI.Invoke((Action)(() =>
                                    {
                                        /// ищется перое поле по поиску и заполняется
                                        /// commandsSplit[1] - по какому типу поиск поля
                                        /// commandsSplit[2] - имя типа для поиска
                                        /// commandsSplit[3] - что вставит в найденное поле
                                        NOW.WriteInField(webControl, getNavigOnWebControl(commandsSplit[1]), commandsSplit[2], DeformateStringForWords(commandsSplit[3]));
                                    }));
                                }
                                break;
                            case "WriteInFieldInDiv":
                                {
                                    formGUI.Invoke((Action)(() =>
                                    {
                                        /// ищется поле по поиску и заполняется из элемента
                                        /// commandsSplit[1] - по какому типу поиск элемента
                                        /// commandsSplit[2] - имя типа для поиска
                                        /// commandsSplit[3] - индекс найденного элемента
                                        /// commandsSplit[4] - по какому типу поиск поля
                                        /// commandsSplit[5] - имя типа для поиска
                                        /// commandsSplit[6] - индекс поля
                                        /// commandsSplit[7] - что вставить в найденное поле
                                        NOW.WriteInFieldInDiv(webControl, getNavigOnWebControl(commandsSplit[1]), commandsSplit[2], Int32.Parse(commandsSplit[3]), getNavigOnWebControl(commandsSplit[4]), commandsSplit[5], Int32.Parse(commandsSplit[6]), DeformateStringForWords(commandsSplit[7]));
                                    }));
                                }
                                break;
                            case "GetAtribInDivInDiv":
                                {
                                    formGUI.Invoke((Action)(() =>
                                    {
                                        /// ищется атрибут у элемента в элементе - найденное записывается в переменную atribstring
                                        /// commandsSplit[1] - по какому типу поиск поля первого элемента
                                        /// commandsSplit[2] - имя типа для поиска первого элемента
                                        /// commandsSplit[3] - какой из найденный элемент из списка использовать
                                        /// commandsSplit[4] - по какому типу поиск поля внутреннего элемента
                                        /// commandsSplit[5] - имя типа для поиска внутреннего элемента
                                        /// commandsSplit[6] - какой из найденный элемент из списка использовать
                                        /// commandsSplit[7] - какой атрибут нужен
                                        atribstring = NOW.GetAtribInDivInDiv(webControl, getNavigOnWebControl(commandsSplit[1]), commandsSplit[2], Int32.Parse(commandsSplit[3]), getNavigOnWebControl(commandsSplit[4]), commandsSplit[5], Int32.Parse(commandsSplit[6]), commandsSplit[7]);
                                        setLog("Warning", "{atribstring} = " + atribstring);
                                    }));
                                }
                                break;
                            case "GetAtribInDiv":
                                {
                                    formGUI.Invoke((Action)(() =>
                                    {
                                        /// ищется атрибут у элемента в элементе - найденное записывается в переменную atribstring
                                        /// commandsSplit[1] - по какому типу поиск поля первого элемента
                                        /// commandsSplit[2] - имя типа для поиска первого элемента
                                        /// commandsSplit[3] - какой из найденный элемент из списка использовать
                                        /// commandsSplit[4] - какой атрибут нужен
                                        atribstring = NOW.GetAtribInDiv(webControl, getNavigOnWebControl(commandsSplit[1]), commandsSplit[2], Int32.Parse(commandsSplit[3]), commandsSplit[4]);
                                        setLog("Warning", "{atribstring} = " + atribstring);
                                    }));
                                }
                                break;
                            case "FormHTML":
                                {
                                    formGUI.Invoke((Action)(() =>
                                    {
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
                            case "waitTimer":
                                /// ждать таймер
                                /// commandsSplit[1] - время в секундах скока прождать
                                waitTimer(Int32.Parse(commandsSplit[1]));
                                break;
                            case "PressButton":
                                {
                                    formGUI.Invoke((Action)(() =>
                                    {
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
                                    formGUI.Invoke((Action)(() =>
                                    {
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
                            case "[MASK]":
                                {
                                    ///определить atribstring по маске
                                    ///commandsSplit[1] - маска
                                    ///commandsSplit[2] - входящий string
                                    atribstring = Regex.Match(DeformateStringForWords(commandsSplit[2]), commandsSplit[1]).ToString();
                                    setLog("Warning", "{atribstring} = " + atribstring);
                                }
                                break;
                            case "[RELOAD]":
                                ///перезагрузить страницу
                                formGUI.Invoke((Action)(() =>
                                {
                                    webControl.Reload(false);
                                }));
                                break;
                            case "[SUBSTRING]":
                                {
                                    ///выделение слова из строки
                                    ///commandsSplit[1] - строка
                                    ///commandsSplit[2] - номер слова
                                    atribstring = DeformateStringForWords(commandsSplit[1]).Split(' ')[Int32.Parse(commandsSplit[2])];
                                    setLog("Warning", "{atribstring} = " + atribstring);
                                }
                                break;
                            case "[MATH]":
                                {
                                    ///простейшие математические операции
                                    ///commandsSplit[1] - первое слагаемое
                                    ///commandsSplit[2] - математичесий оператор
                                    ///commandsSplit[3] - второе слагаемое
                                    switch (commandsSplit[2])
                                    {
                                        case "+":
                                            atribstring = Int32.Parse(DeformateStringForWords(commandsSplit[1])) + Int32.Parse(DeformateStringForWords(commandsSplit[3])) + "";
                                            break;
                                        case "-":
                                            atribstring = Int32.Parse(DeformateStringForWords(commandsSplit[1])) - Int32.Parse(DeformateStringForWords(commandsSplit[3])) + "";
                                            break;
                                        case "*":
                                            atribstring = Int32.Parse(DeformateStringForWords(commandsSplit[1])) * Int32.Parse(DeformateStringForWords(commandsSplit[3])) + "";
                                            break;
                                        case "/":
                                            atribstring = Int32.Parse(DeformateStringForWords(commandsSplit[1])) / Int32.Parse(DeformateStringForWords(commandsSplit[3])) + "";
                                            break;

                                    }

                                    setLog("Warning", "{atribstring} = " + atribstring);
                                }
                                break;
                            case "[MATHD]":
                                {
                                    ///простейшие математические операции
                                    ///commandsSplit[1] - первое слагаемое
                                    ///commandsSplit[2] - математичесий оператор
                                    ///commandsSplit[3] - второе слагаемое


                                    switch (commandsSplit[2])
                                    {
                                        case "+":
                                            atribstring = (Double.Parse(DeformateStringForWords(commandsSplit[1]), CultureInfo.InvariantCulture) + Double.Parse(DeformateStringForWords(commandsSplit[3]), CultureInfo.InvariantCulture)).ToString("0.00000000");
                                            break;
                                        case "-":
                                            atribstring = (Double.Parse(DeformateStringForWords(commandsSplit[1]), CultureInfo.InvariantCulture) - Double.Parse(DeformateStringForWords(commandsSplit[3]), CultureInfo.InvariantCulture)).ToString("0.00000000");
                                            break;
                                        case "*":
                                            atribstring = (Double.Parse(DeformateStringForWords(commandsSplit[1]), CultureInfo.InvariantCulture) * Double.Parse(DeformateStringForWords(commandsSplit[3]), CultureInfo.InvariantCulture)).ToString("0.00000000");
                                            break;
                                        case "/":
                                            atribstring = (Double.Parse(DeformateStringForWords(commandsSplit[1]), CultureInfo.InvariantCulture) / Double.Parse(DeformateStringForWords(commandsSplit[3]), CultureInfo.InvariantCulture)).ToString("0.00000000");
                                            break;

                                    }
                                    setLog("Warning", "{atribstring} = " + atribstring);

                                }
                                break;
                            case "[EMUL]"://testForMoon
                            formGUI.Invoke((Action)(() =>
                            {
                               //NOW.emulBut(webControl);
                            }));
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
                            case "[INFORMER]":
                                {
                                    ///вывести сообщение
                                    ///commandsSplit[1] - заголовок
                                    ///commandsSplit[2] - текст
                                    ///commandsSplit[3] - иконка
                                    showNotify(DeformateStringForWords(commandsSplit[1]), DeformateStringForWords(commandsSplit[2]), getToolTipIcon(commandsSplit[3]));


                                }
                                break;
                            case "[SAVE]":
                                {
                                    ///сохранение в словарь saveString
                                    ///commandsSplit[1] - ключ
                                    ///commandsSplit[2] - значение
                                    if (saveString.ContainsKey(commandsSplit[1]))
                                        saveString[commandsSplit[1]] = DeformateStringForWords(commandsSplit[2]);
                                    else
                                        saveString.Add(commandsSplit[1], DeformateStringForWords(commandsSplit[2]));
                                }
                                break;
                            case "[STOP]":
                                {
                                    ///остановка скрипта
                                    isworking = false;
                                }
                                break;
                            case "[CLOSE_TAB]":
                                {
                                    ///закрытие активной вкладки
                                    ///commandsSplit[1] - ключ
                                    formGUI.Invoke((Action)(() =>
                                    {
                                        string textaddres = addressBox1.AccessibilityObject.Value;
                                        tabControl.SelectedIndex = 0;

                                        {

                                            if (tabControl.TabCount > 2)
                                            {
                                                tabControl.TabPages[tabControl.TabCount - 2].Controls[0].Dispose();
                                                tabControl.TabPages[tabControl.TabCount - 2].Dispose();
                                                setLog("Warning", "CLOSE_TAB");
                                            }
                                        }
                                    }));

                                }
                                break;
                            case "[CLOSE_TABN]":
                                {
                                    ///закрытие активной вкладки
                                    ///
                                    formGUI.Invoke((Action)(() =>
                                    {
                                        tabControl.TabPages[tabControl.SelectedIndex].Controls[0].Dispose();
                                        tabControl.TabPages[tabControl.SelectedIndex].Dispose();
                                        setLog("Warning", "CLOSE_TAB");
                                    }));

                                }
                                break;
                            case "[READ_SAVED]":
                                {
                                    ///чтение записи из словаря saveString в {saved}
                                    ///commandsSplit[1] - ключ
                                    if (saveString.ContainsKey(commandsSplit[1]))
                                        saved = saveString[commandsSplit[1]];
                                    else saved = "NotFoundInSaved";
                                }
                                break;
                            case "[LOG]":
                                {
                                    ///запись лога в событие
                                    ///commandsSplit[1] - текст лога
                                    setLog("PROGRAMMS", DeformateStringForWords(commandsSplit[1]));
                                }
                                break;
                            case "[GOTO_SUB_PROGRAMM]":
                                {
                                    ///меткак куда перейти
                                    //nextPosition = countProgramm;
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
                            case "[CLEAR]":
                                {
                                    ///очистить память 
                                    ///новый webView() во вкладке

                                    GC.Collect();
                                    GC.WaitForFullGCComplete();
                                }
                                break;
                            case "[EXIT]":
                                {
                                    ///очистить память 
                                    ///новый webView() во вкладке
                                    //waitTimer(10);

                                    Application.Exit();
                                }
                                break;

                            case "[BALANCE]":
                                {
                                    ///записать в балансе
                                    ///commandsSplit[1] - что записать
                                    if (isBalanceUse)
                                    {
                                        bool uyt = true;
                                        while (uyt)
                                        {
                                            try
                                            {
                                                using (StreamWriter filebalanceStream = new StreamWriter(balancefailPath, true))
                                                {
                                                    filebalanceStream.WriteLine(DeformateStringForWords(commandsSplit[1]), true);
                                                    filebalanceStream.Flush();
                                                    uyt = false;
                                                }
                                            }
                                            catch (Exception e) { }
                                        }
                                    }
                                }
                                break;
                            case "[HIDEDIV]":
                                {
                                    ///скрыть все DIV
                                    ///commandsSplit[1] - что записать
                                    formGUI.Invoke((Action)(() =>
                                    {
                                        /// ищется поле по поиску и заполняется из элемента
                                        /// commandsSplit[1] - по какому типу поиск элемента
                                        /// commandsSplit[2] - имя типа для поиска
                                        /// commandsSplit[3] - индекс найденного элемента
                                        /// commandsSplit[4] - тип стиля display - none или block
                                        NOW.HideDiv(webControl, getNavigOnWebControl(commandsSplit[1]), commandsSplit[2], Int32.Parse(commandsSplit[3]), commandsSplit[4]);
                                    }));
                                }
                                break;
                            case "formNorm":
                                userEnt(button_ok);
                                break;
                            case "[IFwebGo]":
                                ///если страница не загружена
                                ///commandsSplit[1] - куда перейти
                                waitPage(2);
                                if(addressBox1.AccessibilityObject.Value == "" || addressBox1.AccessibilityObject.Value == "about:blank")
                                {
                                    int stroka = Int32.Parse(commandsSplit[1]);
                                    gotoline(stroka, filereaderStream);
                                }
                                break;
                            case "FormCaptchaSlot":
                                ///показать слот для качи
                                
                                break;
                            case "FormForCaptcha":
                                {
                                    ///показать основную форму для капчи
                                    ///commandsSplit[1] - длина окна
                                    ///commandsSplit[2] - ширина окна
                                    ///commandsSplit[3] - положение по x окна
                                    ///commandsSplit[4] - положение по y окна
                                    /// commandsSplit[5] - тип окна
                                    ///     notClosing - закрыть вкладки все до одной содержащей паттерн
                                    ///     clickone - не ждать юзера
                                    /// commandsSplit[6] - паттерн
                                    setLog("ERROR", commandsSplit.Length+"");
                                    switch (commandsSplit.Length)
                                    {
                                        case 3:
                                            showCaptcha(Int32.Parse(commandsSplit[1]), Int32.Parse(commandsSplit[2]), -1,-1, null, null);
                                            break;
                                        case 5:
                                            showCaptcha(Int32.Parse(commandsSplit[1]), Int32.Parse(commandsSplit[2]), Int32.Parse(commandsSplit[3]), Int32.Parse(commandsSplit[4]), null, null);
                                            break;
                                        case 7:
                                            showCaptcha(Int32.Parse(commandsSplit[1]), Int32.Parse(commandsSplit[2]), Int32.Parse(commandsSplit[3]), Int32.Parse(commandsSplit[4]), commandsSplit[5], commandsSplit[6]);
                                            break;
                                    }
                                    
                                }
                                break;

                        }
                    }catch(Exception e)
                    {
                        bool uyt = true;
                        while (uyt)
                        {
                            try
                            {
                                using (StreamWriter filebalanceStream = new StreamWriter(balancefailPath, true))
                                {
                                    filebalanceStream.WriteLine(DateTime.Now.ToString() + " [ERROR] " + e.Message + " //// " + e.ToString(), true);
                                    filebalanceStream.Flush();
                                    uyt = false;
                                }
                            }
                            catch (Exception ex) { }
                        }
                        Application.Exit();
                    }
                }

            }
        }

        private void showCaptcha(int width, int height, int x, int y, string type, string pattern)
        {
            formGUI.Invoke((Action)(() =>
            {

                splitContainerUp.Hide();
                button_ok = new Button();
                button_ok.Dock = DockStyle.Bottom;
                button_ok.Text = "Отправить";
                button_ok.Font = new Font("Times New Roman", 13, FontStyle.Regular);
                button_ok.Height = 40;
                button_ok.Click += (s, e) =>
                {
                    userEnt(button_ok);
                };
                formGUI.Controls.AddRange(new Control[] { button_ok });
                formGUI.Size = new Size(width, height);
                formGUI.Location = new Point(x, y);
                formGUI.Show();
                formGUI.WindowState = FormWindowState.Normal;
                ((Form1)formGUI).isClosing = true;
                timer.Start();

            }));
            //тип окна
            switch (type)
            {
                case "notClosing":
                    formGUI.typeCloseTab = "notClosing";
                    formGUI.patternForFind = pattern;
                    waitUser();

                    break;
                case "clickone":
                    formGUI.Controls.Remove(button_ok);
                    break;
                default:
                    waitUser();
                    break;
            }
        }

        private ToolTipIcon getToolTipIcon(string v)
        {
            ToolTipIcon typeIcon = ToolTipIcon.None;
            switch (v)
            {
                case "Error":
                    typeIcon = ToolTipIcon.Error;
                    break;
                case "Info":
                    typeIcon = ToolTipIcon.Info;
                    break;
                case "Warning":
                    typeIcon = ToolTipIcon.Warning;
                    break;
               

            }
            return typeIcon;
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
                .Replace("{atribstring}", atribstring)
                .Replace("{time}",  DateTime.Now.ToString())
                .Replace("{saved}", saved);
        }
        public void showNotify(string title, string text, ToolTipIcon tooltip)
        {
            formGUI.Invoke((Action)(() =>
            {
                if (text != null && title != null && tooltip != null)
                {
                    notifyIcon1.BalloonTipIcon = tooltip;
                    notifyIcon1.BalloonTipText = text;
                    notifyIcon1.BalloonTipTitle = title;
                    notifyIcon1.ShowBalloonTip(10);
                }
            }));
        }
        public void setLog(string type, string text)
        {
            
                string newlogs = Environment.NewLine + DateTime.Now + " [" + type + "]: " + text;
                logystring += newlogs;
            try
            {
                formGUI.Invoke((Action)(() =>
                {
                    if (formlog != null)
                        formlog.RefreshLog(newlogs);
                }));
            }catch(Exception e) { }
           // Thread.Sleep(5000);
        }
        private void userEnt( Button button_ok)
        {

            timer.Stop();
             UserAnswer = true;
            if (formGUI.typeCloseTab != null) formGUI.typeCloseTab = null;
                formGUI.Hide();
            ((Form1)formGUI).isClosing = false;
            splitContainerUp.Show();
                formGUI.Controls.Remove(button_ok);
                formGUI.Size = new Size(1000, 600);
            
            
        }
    }
}