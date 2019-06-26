using Awesomium.Core;
using Awesomium.Windows.Forms;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public class TabControlAwesomium
    {
        private Point lastClickPos;
        private TabControl tabControl;
        private WebSessionProvider webSessionProvider;
        private ProgrammMethods pm;

        /// <summary>
        /// Инициализирует новый экземпляр класса TabControlAwesomium
        /// </summary>
        /// <param name="tabControl">Объект tabControl, с которым будет работать класс</param>
        /// <param name="webSessionProvider">Объект webSessionProvider,  с которым будет работать класс</param>
        public TabControlAwesomium(TabControl tabControl, WebSessionProvider webSessionProvider, ProgrammMethods pm)
        {
            this.tabControl = tabControl;
            this.webSessionProvider = webSessionProvider;
            this.pm = pm;
        }


        /// <summary>
        /// Добавит новую вкладку с WebControl(ом)
        /// </summary>
        public void AddPage()
        {
            WebControl webControl = new_WebControl();

            CreateTabPage(webControl);

        }

        /// <summary>
        /// Добавит новую вкладку с WebControl(ом) и с Uri на который кликнули 
        /// </summary>
        /// <param name="e">e, объект класса ShowCreatedWebViewEventArgs</param>
        public void AddPage_ClickReference(ShowCreatedWebViewEventArgs e)
        {
            WebControl webControl = new_WebControl();

            webControl.Source = e.TargetURL;

            CreateTabPage(webControl);
        }

        /// <summary>
        /// (Внимание! Метод зависит от метода TabControlMouseDown) Закрывает вкладку. Или сделать как здесь [url]http://www.youtube.com/watch?v=DJu2ivQFooc[/url]
        /// </summary>
        public void CloseTab()
        {

            for (int i = 0; i < tabControl.TabCount - 1; i++)
            {
                if (tabControl.GetTabRect(i).Contains(tabControl.PointToClient(lastClickPos)))
                {
                    tabControl.TabPages[i].Dispose();
                }
            }

        }

        /// <summary>
        /// Создает на tabControl(е), TabPage с webControl(ом)
        /// </summary>
        /// <param name="webControl">WebControl, с которым будет создавать страницу</param>
        public void CreateTabPage(WebControl webControl)
        {
            tabControl.TabPages.Insert(tabControl.TabCount - 1, "Новая вкладка");
            tabControl.TabPages[tabControl.TabCount - 2].Controls.Add(webControl);
            tabControl.SelectedIndex = tabControl.TabCount - 2;
        }

        /// <summary>
        /// Присвоит текст adressBox(у)
        /// </summary>
        /// <param name="addressBox">объект addressBox, с которым будет работать метод</param>
        public void SetTextAddressBox(AddressBox addressBox)
        {
            if (addressBox.WebControl.Source == null)
            {
                addressBox.AccessibilityObject.Value = "about:blank";
            }
            else
            {
                addressBox.AccessibilityObject.Value = addressBox.WebControl.Source.OriginalString;
            }
        }

        /// <summary>
        /// Задает adressBox(y) webControl, принадлежащий вкладки
        /// </summary>
        /// <param name="addressBox">addressBox с которым будет работать</param>
        /// <param name="Tab">TabPage с которого будет считываться WebControl</param>
        public void SetWebControlInAddressBox(AddressBox addressBox)
        {
            addressBox.WebControl = (WebControl)tabControl.TabPages[tabControl.SelectedIndex].Controls[0];
        }

        /// <summary>
        /// (Определить TabControlMouseDown в событие TabControl.MouseDown) Задает координаты, кликом правой кнопки мыши по вкладке, объекту, для метода CloseTab
        /// </summary>
        /// <param name="e">e - объект события TabControl.MouseDown</param>
        public void TabControlMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                lastClickPos = Cursor.Position;
            }
        }

        /// <summary>
        /// WebControl с некоторыми параметрами
        /// </summary>
        /// <returns>WebControl</returns>
        public WebControl new_WebControl()
        {
            WebControl webControl = new WebControl();
            webControl.Dock = DockStyle.Fill;
            webControl.DocumentReady += webControl_DocumentReady;
            webControl.ShowCreatedWebView += webControl_ShowCreatedWebView;
            

            webSessionProvider.Views.Add(webControl);

            return webControl;
        }

        /// <summary>
        /// Метод для события webControl.DocumentReady. Меняет текст вкладки, когда страница грузится и загружена
        /// </summary>
        private void webControl_DocumentReady(object sender, DocumentReadyEventArgs e)
        {
            string url = ((WebControl)sender).Title;
            ((WebControl)sender).Parent.Text = url;
            if (e.ReadyState == DocumentReadyState.Loaded) {  pm.PageLoad = true;
                pm.setLog("info", "страница " + url + " загрузилась");
                /*флаг загруженной страници*/ };
        }

        /// <summary>
        /// Метод для события webControl.ShowCreatedWebView. При клике по ссылке, создает новую вкладку с WebControl(ом) и с Uri, на который кликнули
        /// </summary>
        private void webControl_ShowCreatedWebView(object sender, ShowCreatedWebViewEventArgs e)
        {
            AddPage_ClickReference(e);
        }

        /// <summary>
        /// Освободить память webControl(а), на выбранной вкладке
        /// </summary>
        /// <param name="addressBox">addressBox</param>
        public void ClearMemory(AddressBox addressBox)
        {
            Uri uri = (tabControl.TabPages[tabControl.SelectedIndex].Controls[0] as WebControl).Source;

            webSessionProvider.Views.Remove(tabControl.TabPages[tabControl.SelectedIndex].Controls[0] as WebControl); // Не обязательная строка
            tabControl.TabPages[tabControl.SelectedIndex].Controls[0].Dispose();

            WebControl webControl = new_WebControl();
            addressBox.WebControl = webControl;

            webSessionProvider.Views.Add(webControl);

            webControl.Source = uri;

            tabControl.TabPages[tabControl.SelectedIndex].Controls.Add(webControl);
        }
        //асинхронный вывод сообщения
        private async void messagInfo(string message)
        {
            await Task.Run(() =>
            {
                if (pm.isDebug) MessageBox.Show(message);
            });

        }
    }
}

