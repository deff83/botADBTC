using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Awesomium.Windows.Forms;

namespace WindowsFormsApp4
{
    
    class NavigationOnWebControl
    {

        Dictionary<GetElementBy, string> Elements;
        
        /// <summary>
        /// Инициализирует новый экземпляр класса NavigationOnWebControl
        /// </summary>
        public NavigationOnWebControl()
        {
            Elements = new Dictionary<GetElementBy, string>();
            AddElements();
        }

        /// <summary>
        /// Атрибут поиска в теге
        /// </summary>
        public enum GetElementBy
        {
            /// <summary>
            /// Поиск по ClassName
            /// </summary>
            ClassName = 0,
            /// <summary>
            /// Поиск по Id
            /// </summary>
            Id = 1,
            /// <summary>
            /// Поиск по Name
            /// </summary>
            Name = 2,
            /// <summary>
            /// Поиск по Tag
            /// </summary>
            TagName = 3
        }

        internal void WriteInField(WebControl webControl, GetElementBy id, string v)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Добавляет элементы в коллекцию
        /// </summary>
        private void AddElements()
        {
            Elements.Add(GetElementBy.ClassName, "getElementsByClassName");
            Elements.Add(GetElementBy.Id, "getElementById");
            Elements.Add(GetElementBy.Name, "getElementsByName");
            Elements.Add(GetElementBy.TagName, "getElementsByTagName");
        }

        /// <summary>
        /// Нажимает кнопку на сайте
        /// </summary>
        /// <param name="webControl">webControl на котором будет выполнять метод</param>
        /// <param name="getElementBy">Атрибут поиска в теге</param>
        /// <param name="ElementValue">Значение атрибута, по которому будет происходить поиск</param>
        public void PressButton(WebControl webControl, GetElementBy getElementBy, string ElementValue, int count, string type)
        {
            if (getElementBy == GetElementBy.Id)
            {
                webControl.ExecuteJavascript("document." + Elements[getElementBy] + "('" + ElementValue + "')." + type + "()");
            }
            else
            {
                webControl.ExecuteJavascript("document." + Elements[getElementBy] + "('" + ElementValue + "')[" + count + "]." + type + "()");
            }

        }
        /// <summary>
        /// Нажимает кнопку на сайте в элементе
        /// </summary>
        /// <param name="webControl">webControl на котором будет выполнять метод</param>
        /// <param name="getElementBy">Атрибут поиска в теге</param>
        /// <param name="ElementValue">Значение атрибута, по которому будет происходить поиск</param>
        public void PressButtonInDIV(WebControl webControl, GetElementBy getElementBy, string ElementValue, int count, GetElementBy getElementBy2, string ElementValue2, int count2, string type)
        {
            if (getElementBy == GetElementBy.Id)
            {
                if (getElementBy2 == GetElementBy.Id)
                {
                    webControl.ExecuteJavascript("document." + Elements[getElementBy] + "('" + ElementValue + "')." + Elements[getElementBy2] + "('" + ElementValue2 + "')." + type + "()");
                }
                else
                {
                    webControl.ExecuteJavascript("document." + Elements[getElementBy] + "('" + ElementValue + "')." + Elements[getElementBy2] + "('" + ElementValue2 + "')[" + count2 + "]." + type + "()");
                }
            }
            else
            {
                if (getElementBy2 == GetElementBy.Id)
                {
                    webControl.ExecuteJavascript("document." + Elements[getElementBy] + "('" + ElementValue + "')[" + count + "]." + Elements[getElementBy2] + "('" + ElementValue2 + "')." + type + "()");
                }
                else
                {
                    webControl.ExecuteJavascript("document." + Elements[getElementBy] + "('" + ElementValue + "')[" + count + "]." + Elements[getElementBy2] + "('" + ElementValue2 + "')[" + count2 + "]." + type + "()");
                }
            }

        }

        /// <summary>
        /// Обновит страницу на webControl
        /// </summary>
        /// <param name="webControl">webControl на котором будет выполнять метод</param>
        public void RefreshPage(WebControl webControl)
        {
            webControl.Reload(false);
        }

        /// <summary>
        /// Вернет индекс nameValueComboBox
        /// </summary>
        /// <param name="namesValueComboBox">Имена значений comboBox(по порядку, как на сайте)</param> // Так же можно найти скрипт на считывание этих значений
        /// <param name="nameValueComboBox">Имя значения, по которому будет сравнивать с массивом имен значений comboBox, для получения индекса значения в comboBox</param>
        /// <returns>Индекс, соответствующий значению nameValueComboBox из массива значений namesValueComboBox</returns>
        public int RetIndex(string[] namesValueComboBox, string nameValueComboBox)
        {

            for (int i = 0; i < namesValueComboBox.Length; i++)
            {
                if (nameValueComboBox == namesValueComboBox[i])
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Выбирает значение из comboBox на сайте
        /// </summary>
        /// <param name="webControl">webControl на котором будет выполнять метод</param>
        /// <param name="getElementBy">Атрибут поиска в теге</param>
        /// <param name="nuberComboBoxValue">Индекс, по которому выберет значение из comboBox</param>
        public void SelectComboBoxValue(WebControl webControl, GetElementBy getElementBy, string ElementValue, int nuberComboBoxValue)
        {
            webControl.ExecuteJavascript("document." + Elements[getElementBy] + "('" + ElementValue + "').selectedIndex = " + nuberComboBoxValue);
        }

        /// <summary>
        /// Записывает в поле на сайта
        /// </summary>
        /// <param name="webControl">webControl на котором будет выполнять метод</param>
        /// <param name="getElementBy">Атрибут поиска в теге</param>
        /// <param name="AttributeValue">Значение атрибута, по которому происходит поиск элемента</param>
        /// <param name="value">Значение, которое запишет в поле на сайте</param>
        public void WriteInField(WebControl webControl, GetElementBy getElementBy, string AttributeValue, string value)
        {
            if (getElementBy == GetElementBy.Id)
            {
                webControl.ExecuteJavascript("document." + Elements[getElementBy] + "('" + AttributeValue + "').value = " + "'" + value + "'");
            }
            else
            {
                webControl.ExecuteJavascript("document." + Elements[getElementBy] + "('" + AttributeValue + "')[0].value = " + "'" + value + "'");
            }
        }

        public string GetAtribInDivInDiv(WebControl webControl, GetElementBy getElementBy, string Atrribute, int count, GetElementBy getElementBy2, string Atrribute2, int count2,string Atrrib)
        {
            string js_code;
            if (getElementBy == GetElementBy.Id)
            {
                if (getElementBy2 == GetElementBy.Id)
                {
                    js_code = "document." + Elements[getElementBy] + "('" + Atrribute + "')." + Elements[getElementBy2] + "('" + Atrribute2 + "')." + Atrrib;
                }
                else
                {
                    js_code = "document." + Elements[getElementBy] + "('" + Atrribute + "')." + Elements[getElementBy2] + "('" + Atrribute2 + "')[" + count2 + "]." + Atrrib;
                }
            }
            else
            {
                if (getElementBy2 == GetElementBy.Id)
                {
                    js_code = "document." + Elements[getElementBy] + "('" + Atrribute + "')[" + count + "]." + Elements[getElementBy2] + "('" + Atrribute2 + "')." + Atrrib;
                }
                else
                {
                    js_code = "document." + Elements[getElementBy] + "('" + Atrribute + "')[" + count + "]." + Elements[getElementBy2] + "('" + Atrribute2 + "')[" + count2 + "]." + Atrrib;
                }
            }
            return webControl.ExecuteJavascriptWithResult(js_code);
           // return js_code;
        }
    }
}
