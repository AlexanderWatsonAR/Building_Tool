using OnlyInvalid.CustomVisualElements;
using OnlyInvalid.ProcGenBuilding.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using Microsoft.Win32.SafeHandles;

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
        Foldout m_TransformFoldout, m_ShapeFoldout, m_ContentFoldout;
        DropdownField m_DropdownField;
        InspectorElement m_Content;

        OpeningDataSerializedProperties m_Props;
        float m_PreviousAngle;
        Vector2 m_PreviousPosition, m_PreviousScale;
        Vector3[] m_PreviousShapeControlPoints;
        #endregion

        #region Accessors
        public HeaderFoldout HeaderFoldout => m_HeaderFoldout;
        public Foldout ContentFoldout => m_ContentFoldout;
        public VisualElement Container => m_Container;
        public ShapeField Shape => m_Shape;
        public Vector2Field Position => m_Position;
        public FloatField Angle => m_Angle;
        public Vector2Field Scale => m_Scale;
        public DropdownField DropdownField => m_DropdownField;
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

            m_PreviousAngle = m_Props.Angle.floatValue;
            m_PreviousPosition = m_Props.Position.vector2Value;
            m_PreviousShapeControlPoints = Opening.Shape.ControlPoints();

            m_HeaderFoldout = new HeaderFoldout(Opening.Name);
            m_HeaderFoldout.toggle.BindProperty(m_Props.IsActive);

            m_ShapeFoldout = new Foldout() { text = "Shape Properties" };
            m_TransformFoldout = new Foldout() { text = "Transform" };
            m_ContentFoldout = new Foldout() { text = "Content" };
            m_Container = new VisualElement();

            m_DropdownField = new DropdownField("Shape");

            List<string> choices = new List<string>();
            choices.Add(nameof(NPolygon));
            choices.Add(nameof(Arch));
            choices.Add(nameof(PathShape));
            choices.Add(nameof(MeshShape));

            m_DropdownField.choices = choices;
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
                }
            });

            m_Shape = new ShapeField(Opening.Shape);

            m_Position = new Vector2Field(m_Props.Position.displayName);
            m_Angle = new FloatField(m_Props.Angle.displayName);
            m_Scale = new Vector2Field(m_Props.Scale.displayName);
            m_Content = new InspectorElement(m_Props.Polygon3D);

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
                m_Container.Add(m_ContentFoldout);
                m_ContentFoldout.Add(m_Content);
            }
            else
            {
                Button contentButton = new Button()
                {
                    text = "Add Content"
                };
                m_Container.Add(contentButton);
            }

            m_HeaderFoldout.AddItem(m_Container);
            this.Add(m_HeaderFoldout);
        }


    }
}