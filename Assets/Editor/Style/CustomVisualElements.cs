using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor;
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

    public class VerticalContainer : VisualElement
    {
        public VerticalContainer() : base()
        {
            this.style.flexDirection = FlexDirection.Column;
        }
    }

    public class RemoveButton : Button
    {
        public RemoveButton() : base()
        {

        }

        public RemoveButton(System.Action clickEvent) : base(clickEvent)
        {
        }
    }

    public class HeaderFoldout : BindableElement, INotifyValueChanged<bool>
    {
        #region UXML
        public new class UxmlFactory : UxmlFactory<HeaderFoldout, UxmlTraits>
        {
        }
        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription { name = "text"};
            UxmlBoolAttributeDescription m_Value = new UxmlBoolAttributeDescription { name = "value", defaultValue = true };
            UxmlBoolAttributeDescription m_Toggle = new UxmlBoolAttributeDescription { name = "active", defaultValue = true };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                HeaderFoldout foldout = ve as HeaderFoldout;
                if (foldout != null)
                {
                    foldout.text = m_Text.GetValueFromBag(bag, cc);
                    foldout.active = m_Toggle.GetValueFromBag(bag, cc);
                    foldout.SetValueWithoutNotify(m_Value.GetValueFromBag(bag, cc));
                }
            }
        }
        #endregion

        #region USS
        public static readonly string ussClassName = "only-invalid-header-foldout";
        public static readonly string arrowUssClassName = ussClassName + "__arrow";
        public static readonly string toggleUssClassName = ussClassName + "__toggle";
        public static readonly string labelUssClassName = ussClassName + "__label";
        public static string headerUssClassName = ussClassName + "__header";
        public static readonly string contentUssClassName = ussClassName + "__content";
        #endregion

        #region Members
        VisualElement m_Arrow;
        Toggle m_Toggle;
        Label m_Label;
        HorizontalContainer m_HeaderContainer;
        VerticalContainer m_ContentContainer;
        [SerializeField] bool m_Value;
        #endregion

        #region Accessors
        public string text { get { return m_Label.text; } set { m_Label.text = value; } }
        public bool active { get { return m_Toggle.value; } set { m_Toggle.value = value; } }
        public bool value
        {
            get
            {
                return m_Value;
            }
            set
            {
                if(m_Value != value)
                {
                    using ChangeEvent<bool> changeEvent = ChangeEvent<bool>.GetPooled(m_Value, value);
                    changeEvent.target = this;
                    SetValueWithoutNotify(value);
                    SendEvent(changeEvent);
                }
            }
        }
        #endregion

        public HeaderFoldout()
        {
            this.style.flexDirection = FlexDirection.Column;

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/Style/CustomVisualElements.uss");
            if (styleSheet != null)
            {
                styleSheets.Add(styleSheet);
            }

            DefineFields();
            SetDefaults();
            AddToClassList();
            RegisterValueChangedCallbacks();
            AddFieldsToRoot();

            m_HeaderContainer.AddManipulator(new Clickable(() =>
            {
                value = !value;
            }));
        }
        public HeaderFoldout(string text) : this()
        {
            m_Label.text = text;
        }
        private void DefineFields()
        {
            m_HeaderContainer = new HorizontalContainer();
            m_Arrow = new VisualElement()
            {
                name = "__arrow",
            };
            m_Toggle = new Toggle()
            {
                style =
                {
                    marginRight = 10
                }
            };
            m_Label = new Label()
            {
                name = "__label",
            };
            m_ContentContainer = new VerticalContainer() { name = "__content"};
        }
        private void SetDefaults()
        {
            value = true;
            active = true;
        }
        private void AddToClassList()
        {
            this.AddToClassList(ussClassName);
            m_Arrow.AddToClassList(arrowUssClassName);
            m_Toggle.AddToClassList(toggleUssClassName);
            m_Label.AddToClassList(labelUssClassName);
            m_ContentContainer.AddToClassList(contentUssClassName);
            m_HeaderContainer.AddToClassList(headerUssClassName);
        }
        private void RegisterValueChangedCallbacks()
        {
            m_Toggle.RegisterValueChangedCallback(evt => 
            {
                bool value = evt.newValue;

                m_ContentContainer.SetEnabled(value);
            });
        }
        private void AddFieldsToRoot()
        {
            this.Add(m_HeaderContainer);
            m_HeaderContainer.Add(m_Arrow);
            m_HeaderContainer.Add(m_Toggle);
            m_HeaderContainer.Add(m_Label);
            this.Add(m_ContentContainer);
        }
        public void AddItem(VisualElement element)
        {
            m_ContentContainer.Add(element);
        }
        public void SetValueWithoutNotify(bool newValue)
        {
            m_Value = newValue;

            if(m_Value)
            {
                m_ContentContainer.style.display = DisplayStyle.Flex;
                m_Arrow.style.rotate = new StyleRotate(new Rotate(90));
            }
            else
            {
                m_ContentContainer.style.display = DisplayStyle.None;
                m_Arrow.style.rotate = new StyleRotate(new Rotate(0));
            }
        }
    }
}