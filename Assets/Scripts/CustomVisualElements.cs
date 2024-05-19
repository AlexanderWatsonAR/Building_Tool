using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace OnlyInvalid.CustomVisualElements
{
    public class HorizontalContainer : VisualElement
    {
        public HorizontalContainer() : base()
        {
            this.style.flexDirection = FlexDirection.Row;
        }
    }

    public class HeaderFoldout : HorizontalContainer
    {
        Label m_Arrow;
        Toggle m_Toggle;
        Label m_Label;
        VisualElement m_ContentContainer;

        public string text { get { return m_Label.text; } set { m_Label.text = value; } }
        public bool active { get { return m_Toggle.value; } set { m_Toggle.value = value; } }
        public bool open
        {
            get
            {
                return viewDataKey == "open";
            }
            set
            {
                if (value)
                {
                    viewDataKey = "open";
                    m_ContentContainer.style.display = DisplayStyle.Flex;
                    m_Arrow.text = '\u25BC'.ToString();
                }
                else
                {
                    viewDataKey = "closed";
                    m_ContentContainer.style.display = DisplayStyle.None;
                    m_Arrow.text = '\u25B6'.ToString();
                }
            }
        }

        public HeaderFoldout()
        {
            this.style.backgroundColor = new StyleColor(new Color(1, 1, 1, 0.1f));

            DefineFields();
            AddToClassList();
            AddFieldsToRoot();

            open = true;

            this.AddManipulator(new Clickable(() =>
            {
                open = !open;
            }));

            //this.StretchToParentWidth();
        }
        public HeaderFoldout(string text) : this()
        {
            m_Label.text = text;
        }
        private void DefineFields()
        {
            m_Arrow = new Label();
            m_Toggle = new Toggle()
            {
                style =
                {
                    marginRight = 10
                }
            };
            m_Label = new Label();
            m_ContentContainer = new VisualElement();
        }
        private void AddToClassList()
        {
            m_Arrow.AddToClassList("header-foldout-arrow");
            m_Toggle.AddToClassList("header-foldout-toggle");
            m_Label.AddToClassList("header-foldout-label");
            m_ContentContainer.AddToClassList("header-foldout-content");
        }
        private void AddFieldsToRoot()
        {
            this.Add(m_Arrow);
            this.Add(m_Toggle);
            this.Add(m_Label);
            this.Add(m_ContentContainer);
        }
        public void AddItem(VisualElement element)
        {
            m_ContentContainer.Add(element);
        }
    }
}