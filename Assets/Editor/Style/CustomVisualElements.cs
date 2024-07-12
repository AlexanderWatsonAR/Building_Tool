using System.Web;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UIElements;

namespace OnlyInvalid.CustomVisualElements
{
    public class HorizontalContainer : VisualElement
    {
        public HorizontalContainer(bool reverse = false) : base()
        {
            this.style.flexDirection = reverse ? FlexDirection.RowReverse : FlexDirection.Row;
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

    public class ContextElement : VisualElement
    {
        GenericDropdownMenu m_ContextMenu;

        public GenericDropdownMenu menu => m_ContextMenu;

        public ContextElement() : base()
        {
            this.style.backgroundImage = new StyleBackground(EditorGUIUtility.IconContent("d_more").image as Texture2D);

            this.m_ContextMenu = new GenericDropdownMenu();
            this.AddManipulator(new Clickable(() => DisplayContextMenu()));
        }
        public ContextElement(GenericDropdownMenu contextMenu)
        {
            this.m_ContextMenu = contextMenu;
            this.AddManipulator(new Clickable(() => DisplayContextMenu()));
        }

        private void DisplayContextMenu()
        {
            m_ContextMenu.DropDown(this.worldBound, this);
        }
    }

    public class HeaderFoldout : BindableElement
    {
        #region UXML
        public new class UxmlFactory : UxmlFactory<HeaderFoldout, UxmlTraits>
        {
        }
        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription { name = "text" };
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
                    foldout.SetFoldoutOrientation(m_Value.GetValueFromBag(bag, cc));
                }
            }
        }
        #endregion

        #region USS
        public static readonly string ussClassName = "only-invalid-header-foldout";
        public static readonly string arrowUssClassName = ussClassName + "__arrow";
        public static readonly string toggleUssClassName = ussClassName + "__toggle";
        public static readonly string labelUssClassName = ussClassName + "__label";
        public static readonly string headerUssClassName = ussClassName + "__header";
        public static readonly string leftHeaderUssClassName = ussClassName + "__left-header";
        public static readonly string rightHeaderUssClassName = ussClassName + "__right-header";
        public static readonly string contentUssClassName = ussClassName + "__content";
        public static readonly string contextMenuUssClassName = ussClassName + "__context-menu";
        #endregion

        #region Members
        VisualElement m_Arrow;
        Toggle m_Toggle;
        Label m_Label;
        TextField m_LabelEditField;
        ContextElement m_ContextElement;
        HorizontalContainer m_HeaderContainer, m_LeftHeader, m_RightHeader;
        VerticalContainer m_ContentContainer;
        [SerializeField] bool m_Value;
        [SerializeField] string m_ViewDataKey;
        #endregion

        #region Accessors
        public string text { get { return m_Label.text; } set { m_Label.text = value; } }
        public bool active
        {
            get 
            {
                return m_Toggle.value;
            }
            set
            {
                m_Toggle.value = value;
                
            }
        }
        public bool value
        {
            get
            {
                return m_Value;
            }
            set
            {
                if (m_Value != value)
                {
                    SetFoldoutOrientation(value);

                    EditorPrefs.SetBool(m_ViewDataKey, m_Value);
                }
            }
        }
        public GenericDropdownMenu contextMenu => m_ContextElement.menu;
        public Toggle toggle => m_Toggle;
        public Label label => m_Label;
        public TextField textField => m_LabelEditField;
        public new string viewDataKey
        {
            get
            {
                return m_ViewDataKey;
            }
            set
            {
                m_ViewDataKey = value;
                this.value = EditorPrefs.GetBool(m_ViewDataKey, true);
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
            m_LabelEditField.value = text;
        }
        private void DefineFields()
        {
            m_HeaderContainer = new HorizontalContainer();
            m_LeftHeader = new HorizontalContainer();
            m_RightHeader = new HorizontalContainer(true);
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
            m_ContextElement = new ContextElement();
            m_ContentContainer = new VerticalContainer() { name = "__content" };
            m_LabelEditField = new TextField()
            {
                value = m_Label.text,
            };
            m_LabelEditField.style.display = DisplayStyle.None;
            m_LabelEditField.style.width = 125;
        }
        private void SetDefaults()
        {
            value = true;
            active = true;
            m_HeaderContainer.name = "__header";
            m_LeftHeader.name = "__left-header";
            m_LeftHeader.style.flexGrow = 1;
            m_RightHeader.name = "__right-header";
            m_RightHeader.style.flexGrow = 1;
        }
        private void AddToClassList()
        {
            this.AddToClassList(ussClassName);
            m_Arrow.AddToClassList(arrowUssClassName);
            m_Toggle.AddToClassList(toggleUssClassName);
            m_Label.AddToClassList(labelUssClassName);
            m_ContextElement.AddToClassList(contextMenuUssClassName);
            m_ContentContainer.AddToClassList(contentUssClassName);
            m_HeaderContainer.AddToClassList(headerUssClassName);
            m_LeftHeader.AddToClassList(leftHeaderUssClassName);
            m_RightHeader.AddToClassList(rightHeaderUssClassName);
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
            m_HeaderContainer.Add(m_LeftHeader);
            m_HeaderContainer.Add(m_RightHeader);
            m_LeftHeader.Add(m_Arrow);
            m_LeftHeader.Add(m_Toggle);
            m_LeftHeader.Add(m_Label);
            m_LeftHeader.Add(m_LabelEditField);
            m_RightHeader.Add(m_ContextElement);
            this.Add(m_ContentContainer);
        }
        public void AddItem(VisualElement element)
        {
            m_ContentContainer.Add(element);
        }
        private void SetFoldoutOrientation(bool newValue)
        {
            m_Value = newValue;

            if (m_Value)
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

    public class ShapeField : VisualElement
    {
        protected Shape m_Shape;

        public ShapeField(Shape shape)
        {
            ChangeShape(shape);
        }

        public void ChangeShape(Shape shape)
        {
            m_Shape = shape;
            this.Clear();

            switch (shape)
            {
                case NPolygon:
                    {
                        NPolygon nPolygon = shape as NPolygon;
                        SliderInt sides = new SliderInt()
                        {
                            label = "Sides",
                            value = nPolygon.Sides,
                            lowValue = 3,
                            highValue = 18,
                            showInputField = true,
                        };

                        sides.RegisterValueChangedCallback(evt => 
                        {
                            nPolygon.Sides = evt.newValue;

                            using (ChangeEvent<Shape> changeEvent = ChangeEvent<Shape>.GetPooled(new NPolygon(evt.previousValue), nPolygon))
                            {
                                changeEvent.target = this;
                                this.SendEvent(changeEvent);
                            }
                            
                        });

                        this.Add(sides);
                    }
                    break;
                case Arch:
                    {
                        Arch arch = shape as Arch;
                        FloatField archHeight = new FloatField("Arch Height") { value = arch.ArchHeight };
                        FloatField baseHeight = new FloatField("Base Height") { value = arch.BaseHeight };
                        SliderInt sides = new SliderInt()
                        {
                            label = "Sides",
                            value = arch.Sides,
                            lowValue = 3,
                            highValue = 18,
                            showInputField = true
                        };

                        sides.RegisterValueChangedCallback(evt =>
                        {
                            arch.Sides = evt.newValue;

                            using (ChangeEvent<Shape> changeEvent = ChangeEvent<Shape>.GetPooled(new Arch(arch.ArchHeight, arch.BaseHeight, evt.previousValue), arch))
                            {
                                changeEvent.target = this;
                                this.SendEvent(changeEvent);
                            }
                        });

                        archHeight.RegisterValueChangedCallback(evt =>
                        {
                            arch.ArchHeight = evt.newValue;

                            using (ChangeEvent<Shape> changeEvent = ChangeEvent<Shape>.GetPooled(new Arch(evt.previousValue, arch.BaseHeight, arch.Sides), arch))
                            {
                                changeEvent.target = this;
                                this.SendEvent(changeEvent);
                            }
                        });

                        baseHeight.RegisterValueChangedCallback(evt =>
                        {
                            arch.BaseHeight = evt.newValue;

                            using (ChangeEvent<Shape> changeEvent = ChangeEvent<Shape>.GetPooled(new Arch(arch.ArchHeight, evt.previousValue, arch.Sides), arch))
                            {
                                changeEvent.target = this;
                                this.SendEvent(changeEvent);
                            }
                        });


                        this.Add(archHeight);
                        this.Add(baseHeight);
                        this.Add(sides);
                    }
                    break;
                case MeshShape:

                    MeshShape meshShape = shape as MeshShape;

                    ObjectField meshPicker = new ObjectField()
                    {
                        label = "Mesh",
                        value = meshShape.Mesh,
                        objectType = typeof(Mesh),
                    };

                    meshPicker.RegisterValueChangedCallback(evt => 
                    {
                        meshShape.Mesh = evt.newValue as Mesh;

                        using (ChangeEvent<Shape> changeEvent = ChangeEvent<Shape>.GetPooled(new MeshShape(evt.previousValue as Mesh), meshShape))
                        {
                            changeEvent.target = this;
                            this.SendEvent(changeEvent);
                        }
                    });

                    this.Add(meshPicker);

                    break;
            }
        }
    }

    //public class ArchField : ShapeField
    //{
    //    public ArchField(Arch arch) : base(arch)
    //    {
    //        FloatField archHeightField = new FloatField("Arch Height") { value = arch.TopHeight};
    //        FloatField baseHeightField = new FloatField("Base Height") { value = arch.BottomHeight};
    //        SliderInt sidesField = new SliderInt()
    //        {
    //            label = "Sides",
    //            value = arch.Sides,
    //            lowValue = 3,
    //            highValue = 18,
    //            showInputField = true
    //        };

    //        this.Add(archHeightField);
    //        this.Add(baseHeightField);
    //        this.Add(sidesField);
    //    }
    //}

    //public class NPolygonField : ShapeField
    //{
    //    public NPolygonField(NPolygon nPolygon) : base(nPolygon)
    //    {
    //        SliderInt sides = new SliderInt()
    //        {
    //            label = "Sides",
    //            value = nPolygon.Sides,
    //            lowValue = 3,
    //            highValue = 18,
    //            showInputField = true,
    //        };

    //        this.Add(sides);
    //    }
    //}
    
}