using OnlyInvalid.CustomVisualElements;
using OnlyInvalid.ProcGenBuilding.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    public class OpeningDataField : VisualElement
    {
        #region Const
        const string k_UssAligned = "unity-base-field__aligned";
        #endregion

        #region Members
        HeaderFoldout m_HeaderFoldout;
        VisualElement m_Container;
        ShapeField m_Shape;
        Vector2Field m_Position;
        FloatField m_Angle;
        Vector2Field m_Scale;
        Foldout m_TransformFoldout, m_ShapeFoldout /*m_ContentFoldout*/;
        DropdownField m_DropdownField;
        InspectorElement m_Content;
        Button m_ContentButton;

        OpeningDataSerializedProperties m_Props;
        float m_PreviousAngle;
        Vector2 m_PreviousPosition, m_PreviousScale;
        Vector3[] m_PreviousShapeControlPoints;
        #endregion

        #region Accessors
        public HeaderFoldout HeaderFoldout => m_HeaderFoldout;
       // public Foldout ContentFoldout => m_ContentFoldout;
        public VisualElement Container => m_Container;
        public ShapeField Shape => m_Shape;
        public Vector2Field Position => m_Position;
        public FloatField Angle => m_Angle;
        public Vector2Field Scale => m_Scale;
        public DropdownField DropdownField => m_DropdownField;
        public Button ContentButton => m_ContentButton;
        public OpeningDataSerializedProperties Props => m_Props;
        public OpeningData Opening => m_Props.Data.GetUnderlyingValue() as OpeningData;
        public Vector2 PreviousPosition { get => m_PreviousPosition; set => m_PreviousPosition = value; }
        public float PreviousAngle { get => m_PreviousAngle; set => m_PreviousAngle = value; }
        public Vector2 PreviousScale { get => m_PreviousScale; set => m_PreviousScale = value; }
        public Vector3[] PreviousShapeControlPoints { get => m_PreviousShapeControlPoints; set => m_PreviousShapeControlPoints = value; }


        #endregion

        public OpeningDataField(OpeningDataSerializedProperties props) : base()
        {
            m_Props = props;

            this.name = Opening.Name;

            m_PreviousAngle = m_Props.Angle.floatValue;
            m_PreviousPosition = m_Props.Position.vector2Value;
            m_PreviousShapeControlPoints = Opening.Shape.ControlPoints();

            m_HeaderFoldout = new HeaderFoldout(Opening.Name);
            m_HeaderFoldout.toggle.BindProperty(m_Props.IsActive);

            m_ShapeFoldout = new Foldout()
            {
                text = "Shape Properties",
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                },
                
            };
            m_TransformFoldout = new Foldout()
            {
                text = "Transform",
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            };
            Label contentLabel = new Label("Content")
            {
                style =
                {
                    marginTop = 5,
                    marginBottom = 5,
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            };

            m_ShapeFoldout.RegisterCallback<PointerEnterEvent>(evt =>
            {
                m_ShapeFoldout.style.backgroundColor = new StyleColor(new Color(1, 1, 1, 0.05f));
            });
            m_ShapeFoldout.RegisterCallback<PointerLeaveEvent>(evt =>
            {
                m_ShapeFoldout.style.backgroundColor = new StyleColor(new Color(1, 1, 1, 0));
            });

            m_Container = new VisualElement();

            m_DropdownField = new DropdownField("Shape")
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Normal,
                }
            }; 

            m_DropdownField.choices = new List<string>()
            {
                nameof(NPolygon),
                nameof(Arch),
                nameof(PathShape),
                nameof(MeshShape),
                nameof(RoundedSquare)
            };
            m_DropdownField.value = Opening.Shape.GetType().Name;

            m_DropdownField.RegisterValueChangedCallback(evt =>
            {
                switch(evt.newValue)
                {
                    case nameof(NPolygon):
                        {
                            if (Opening.Shape is NPolygon)
                                break;

                            m_Props.Shape.SetUnderlyingValue(new NPolygon(4));
                            m_Shape.ChangeShape(Opening.Shape);

                        }
                        break;
                    case nameof(Arch):
                        {
                            if (Opening.Shape is Arch)
                                break;

                            m_Props.Shape.SetUnderlyingValue(new Arch(1, 1, 5));
                            m_Shape.ChangeShape(Opening.Shape);
                        }
                        break;
                    case nameof(PathShape):
                        {
                            if (Opening.Shape is PathShape)
                                break;
                            Opening.Shape = new PathShape();
                        }
                        break;
                    case nameof(MeshShape):
                        {
                            if (Opening.Shape is MeshShape)
                                break;

                            Mesh squareMesh = new Mesh();
                            squareMesh.vertices = MeshMaker.Square();
                            squareMesh.name = "Square";

                            m_Props.Shape.SetUnderlyingValue(new MeshShape(squareMesh));
                            m_Shape.ChangeShape(Opening.Shape);
                        }
                        break;
                    case nameof(RoundedSquare):
                        {
                            if (Opening.Shape is RoundedSquare)
                                break;

                            m_Props.Shape.SetUnderlyingValue(new RoundedSquare());
                            m_Shape.ChangeShape(Opening.Shape);
                        }
                        break;
                }
            });

            m_Shape = new ShapeField(Opening.Shape)
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Normal
                }
            };
            m_Position = new Vector2Field(m_Props.Position.displayName)
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Normal
                }
            };
            m_Angle = new FloatField(m_Props.Angle.displayName)
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Normal
                }
            };
            m_Scale = new Vector2Field(m_Props.Scale.displayName)
            { 
                style = 
                { 
                    unityFontStyleAndWeight = FontStyle.Normal 
                }
            };
            m_Content = new InspectorElement(m_Props.Polygon3D)
            {
                style = 
                {
                    unityFontStyleAndWeight = FontStyle.Normal
                }
            };

            m_Container.RegisterCallback<AddContentEvent>(evt => 
            {
                Opening.Polygon3D = evt.Content;

                Extensions.Reselect();
            });

            m_Position.AddToClassList(k_UssAligned);
            m_Angle.AddToClassList(k_UssAligned);
            m_Scale.AddToClassList(k_UssAligned);

            m_Position.BindProperty(m_Props.Position);
            m_Angle.BindProperty(m_Props.Angle);
            m_Scale.BindProperty(m_Props.Scale);

            m_ShapeFoldout.Add(m_DropdownField);
            m_ShapeFoldout.Add(m_Shape);

            m_TransformFoldout.Add(m_Position);
            m_TransformFoldout.Add(m_Angle);
            m_TransformFoldout.Add(m_Scale);

            m_Container.Add(m_ShapeFoldout);
            m_Container.Add(m_TransformFoldout);

            if (m_Props.Polygon3D != m_Props.Data.exposedReferenceValue)
            {
                m_Container.Add(contentLabel);

                BorderBox box = new BorderBox(1, 5);
                WindowHeader header = new WindowHeader(m_Props.Polygon3D.GetType().Name);
                header.RemoveButton.clicked += () =>
                {
                    Opening.RemoveContent();
                    Extensions.Reselect();
                };
                box.Add(header);
                box.Add(m_Content);

                m_Container.Add(box);
            }
            else
            {
                m_ContentButton = new Button()
                {
                    text = "Add Content"
                };

                AddContentMenu menu = AssetDatabase.LoadAssetAtPath("Assets/Scripts/Menu/SO/Add Content Menu.asset", typeof(AddContentMenu)) as AddContentMenu;
                menu.OnCreatedContent.AddListener(content =>
                {
                    using (AddContentEvent evt = AddContentEvent.GetPooled(content))
                    {
                        evt.Content = content;
                        evt.target = m_Container;

                        m_Container.SendEvent(evt);
                    }
                });
                menu.Initialize(m_ContentButton);

                m_ContentButton.clicked += () =>
                {
                    menu.CreateMenu();
                    menu.Menu.DropDown(m_ContentButton.worldBound, m_ContentButton);
                };

                m_Container.Add(m_ContentButton);
            }

            m_HeaderFoldout.AddItem(m_Container);
            this.Add(m_HeaderFoldout);
        }


    }
}