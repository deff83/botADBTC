using System;
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
        string LOGIN = "masleey@mail.ru";
        string PASSWORD = "07041962waiss8921305075611";
        public bool PageLoad = false;
        private Form formGUI;

        public bool UserAnswer = false;
        public string userword;

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

        public void authorizationSign(WebControl webControl)
        {
            //вход на сайт abtc.top
            Action action = () => {webControl.Source = new Uri(@"https://adbtc.top");};
            formGUI.Invoke(action);
            waitPage();

            //кнопка sign in
            action = () => { NOW.PressButton(webControl, NavigationOnWebControl.GetElementBy.ClassName, "btn-large accent-2 black-text","click"); };
            formGUI.Invoke(action);

            waitPage();
            waitPage();
            //ввод адреса и пароля с показом капчи картинкой
            action = () => {
                
                NOW.WriteInField(webControl, NavigationOnWebControl.GetElementBy.Name, "addr", LOGIN);
                NOW.WriteInField(webControl, NavigationOnWebControl.GetElementBy.Name, "secret", PASSWORD);
                string imgsrc = NOW.GetAtribInDivInDiv(webControl, NavigationOnWebControl.GetElementBy.ClassName, "col s12 m6 offset-m3",0,NavigationOnWebControl.GetElementBy.TagName, "img",0,"src");
                string HTMLForWin = "<center><img src='"+ imgsrc + "' style='width: 150; height: 87; border: 0;' alt=' '></center> ";
                new FormCaptchaImg(HTMLForWin, this).Show();
            };
            formGUI.Invoke(action);
            waitUser();
            //нажатие на кнопку вход
            action = () => {
                NOW.WriteInField(webControl, NavigationOnWebControl.GetElementBy.Name, "number", userword);
                NOW.PressButton(webControl, NavigationOnWebControl.GetElementBy.Id, "submit_btn", "click");
            };
            formGUI.Invoke(action);
        }
        private void waitPage()
        {
            PageLoad = false;
            while (!PageLoad)
            {
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


        
    }
}