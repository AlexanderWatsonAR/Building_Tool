using System;
using System.Runtime.CompilerServices;
using System.Web;
using Unity.Plastic.Newtonsoft.Json.Bson;
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
                            label = DisplayDataSettings.Data.NPolygon.Sides.label,
                            value = nPolygon.Sides,
                            lowValue = DisplayDataSettings.Data.NPolygon.Sides.range.lower,
                            highValue = DisplayDataSettings.Data.NPolygon.Sides.range.upper,
                            showInputField = DisplayDataSettings.Data.NPolygon.Sides.showInputField,
                            direction = DisplayDataSettings.Data.NPolygon.Sides.direction,
                            inverted = DisplayDataSettings.Data.NPolygon.Sides.inverted
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
                            label = DisplayDataSettings.Data.Arch.Sides.label,
                            value = arch.Sides,
                            lowValue = DisplayDataSettings.Data.Arch.Sides.range.lower,
                            highValue = DisplayDataSettings.Data.Arch.Sides.range.upper,
                            showInputField = DisplayDataSettings.Data.Arch.Sides.showInputField,
                            direction = DisplayDataSettings.Data.Arch.Sides.direction,
                            inverted = DisplayDataSettings.Data.Arch.Sides.inverted,
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
                    {
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
                    }
                    break;
            }
        }
    }

    public class SliderDisplayDataField : BaseSliderDisplayDataField<float>
    {
        public SliderDisplayDataField(SliderDisplayData<float> data) : base(data)
        {
            Initalize();
            RegisterValueChangedCallbacks();
            AddToRoot();
        }
        protected override void Initalize()
        {
            base.Initalize();

            m_Lower = new FloatField("Lower")
            {
                value = m_Data.range.lower
            };
            m_Upper = new FloatField("Upper")
            {
                value = m_Data.range.upper
            };
        }
        protected override void RegisterValueChangedCallbacks()
        {
            base.RegisterValueChangedCallbacks();

            m_Lower.RegisterValueChangedCallback(evt =>
            {
                value = new SliderDisplayData<float>(m_Data)
                {
                    range = new RangeValues<float>(evt.newValue, m_Data.range.upper),
                };
            });
            m_Upper.RegisterValueChangedCallback(evt =>
            {
                value = new SliderDisplayData<float>(m_Data)
                {
                    range = new RangeValues<float>(m_Data.range.lower, evt.newValue),
                };
            });
        }
    }

    public class SliderIntDisplayDataField : BaseSliderDisplayDataField<int>
    {
        public SliderIntDisplayDataField(SliderDisplayData<int> data) : base(data)
        {
            Initalize();
            RegisterValueChangedCallbacks();
            AddToRoot();
        }
        protected override void Initalize()
        {
            base.Initalize();

            m_Lower = new IntegerField("Lower")
            {
                value = m_Data.range.lower
            };
            m_Upper = new IntegerField("Upper")
            {
                value = m_Data.range.upper
            };
        }
        protected override void RegisterValueChangedCallbacks()
        {
            base.RegisterValueChangedCallbacks();

            m_Lower.RegisterValueChangedCallback(evt =>
            {
                value = new SliderDisplayData<int>(m_Data)
                {
                    range = new RangeValues<int>(evt.newValue, m_Data.range.upper),
                };
            });
            m_Upper.RegisterValueChangedCallback(evt =>
            {
                value = new SliderDisplayData<int>(m_Data)
                {
                    range = new RangeValues<int>(m_Data.range.lower, evt.newValue),
                };
            });
        }
    }

    public abstract class BaseSliderDisplayDataField<T> : VisualElement
    {
        protected SliderDisplayData<T> m_Data;

        protected Label m_Name;
        protected TextField m_Label;
        protected TextValueField<T> m_Lower, m_Upper;
        protected EnumField m_Direction;
        protected Toggle m_ShowInputField, m_Inverted;

        public SliderDisplayData<T> value
        {
            get { return m_Data; }

            set
            {
                SliderDisplayData<T> previous = new SliderDisplayData<T>(m_Data);

                using (ChangeEvent<SliderDisplayData<T>> evt = ChangeEvent<SliderDisplayData<T>>.GetPooled(previous, value))
                {
                    m_Data = value;
                    evt.target = this;
                    this.SendEvent(evt);
                }
            }
        }

        public BaseSliderDisplayDataField(SliderDisplayData<T> data) : base()
        {
            m_Data = data;
        }

        protected virtual void Initalize()
        {
            m_Name = new Label(name);
            m_Label = new TextField("Label")
            {
                value = m_Data.label
            };
            m_Direction = new EnumField("Direction", m_Data.direction)
            {
                value = m_Data.direction
            };
            m_ShowInputField = new Toggle("Show Input Field")
            {
                value = m_Data.showInputField
            };
            m_Inverted = new Toggle("Inverted")
            {
                value = m_Data.inverted
            };
        }

        protected virtual void RegisterValueChangedCallbacks()
        {
            m_Label.RegisterCallback<KeyDownEvent>(evt =>
            {
                value = new SliderDisplayData<T>(m_Data)
                {
                    label = m_Label.text
                };
            });
            m_Direction.RegisterValueChangedCallback(evt => 
            {
                value = new SliderDisplayData<T>(m_Data)
                {
                    direction = (SliderDirection)m_Direction.value
                };
            });
            m_ShowInputField.RegisterValueChangedCallback(evt => 
            {
                value = new SliderDisplayData<T>(m_Data)
                {
                    showInputField = m_ShowInputField.value
                };
            });
            m_Inverted.RegisterValueChangedCallback(evt =>
            {
                value = new SliderDisplayData<T>(m_Data)
                {
                    inverted = m_Inverted.value
                };
            });
        }

        protected virtual void AddToRoot()
        {
            this.Add(m_Name);
            this.Add(m_Label);
            this.Add(m_Lower);
            this.Add(m_Upper);
            this.Add(m_Direction);
            this.Add(m_ShowInputField);
            this.Add(m_Inverted);
        }
    }
}