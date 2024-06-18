using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using OnlyInvalid.CustomVisualElements;
using UnityEditor.Rendering;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.ProBuilder.Shapes;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [CustomPropertyDrawer(typeof(OpeningDataList), false)]
    public class OpeningDataListDrawer : PropertyDrawer
    {
        SerializedProperty m_Data, m_InnerList;
        VisualElement m_Root;
        List<OpeningDataFields> m_OpeningDataFields;

        OpeningDataList m_Openings;

        public override VisualElement CreatePropertyGUI(SerializedProperty data)
        {
            m_Root = new VisualElement();
            m_Data = data;
            m_InnerList = m_Data.FindPropertyRelative("m_Openings");
            m_Openings = m_Data.GetUnderlyingValue() as OpeningDataList;
            m_OpeningDataFields = new List<OpeningDataFields>();

            for (int i = 0; i < m_InnerList.arraySize; i++)
            {
                m_OpeningDataFields.Add(new OpeningDataFields(new OpeningDataSerializedProperties(m_InnerList.GetArrayElementAtIndex(i))));
            }

            foreach (var field in m_OpeningDataFields)
            {
                field.HeaderFoldout.contextMenu.AddItem("Remove", false, () => { m_Openings.Remove(field.Opening); m_Openings.IsDirty = true; });
                //field.Shape.RegisterCallback<GeometryChangedEvent> (evt =>
                //{
                //    m_Openings.IsDirty = true;
                //});
                field.Shape.RegisterValueChangeCallback(evt => { Debug.Log("Shape changed"); });
                field.Height.RegisterValueChangedCallback(evt =>
                {
                    if (evt.previousValue == evt.newValue)
                        return;

                    if (field.PreviousHeight == evt.newValue)
                        return;

                    field.PreviousHeight = evt.newValue;

                    m_Openings.IsDirty = true;
                });
                field.Width.RegisterValueChangedCallback(evt =>
                {
                    if (evt.previousValue == evt.newValue)
                        return;

                    if (field.PreviousWidth == evt.newValue)
                        return;

                    field.PreviousWidth = evt.newValue;

                    m_Openings.IsDirty = true;
                });
                field.Angle.RegisterValueChangedCallback(evt =>
                {
                    if (evt.previousValue == evt.newValue)
                        return;

                    if (field.PreviousAngle == evt.newValue)
                        return;

                    field.PreviousAngle = evt.newValue;

                    m_Openings.IsDirty = true;
                });
                field.Position.RegisterValueChangedCallback(evt =>
                {
                    if (evt.previousValue == evt.newValue)
                        return;

                    if (field.PreviousPosition == evt.newValue)
                        return;

                    field.PreviousPosition = evt.newValue;

                    m_Openings.IsDirty = true;
                });

                m_Root.Add(field.HeaderFoldout);
            }

            return m_Root;
        }
    }

    class OpeningDataFields
    {
        #region Const
        const string k_UssAligned = "unity-base-field__aligned";
        #endregion

        #region Members
        HeaderFoldout m_HeaderFoldout;
        VisualElement m_Container;
        PropertyField m_Shape;
        Slider m_Height, m_Width, m_Angle;
        Vector2Field m_Position;
        Foldout m_ContentFoldout;
        InspectorElement m_Content;

        OpeningDataSerializedProperties m_Props;
        float m_PreviousHeight, m_PreviousWidth, m_PreviousAngle;
        Vector2 m_PreviousPosition;
        Vector3[] m_PreviousShapeControlPoints;
        #endregion

        #region Accessors
        public HeaderFoldout HeaderFoldout => m_HeaderFoldout;
        public Foldout ContentFoldout => m_ContentFoldout;
        public VisualElement Container => m_Container;
        public PropertyField Shape => m_Shape;
        public Slider Height => m_Height;
        public Slider Width => m_Width;
        public Slider Angle => m_Angle;
        public Vector2Field Position => m_Position;
        public OpeningDataSerializedProperties Props => m_Props;
        public OpeningData Opening => m_Props.Data.GetUnderlyingValue() as OpeningData;
        public float PreviousHeight { get => m_PreviousHeight; set => m_PreviousHeight = value; }
        public float PreviousWidth { get => m_PreviousWidth; set => m_PreviousWidth = value; }
        public float PreviousAngle { get => m_PreviousAngle; set => m_PreviousAngle = value; }
        public Vector2 PreviousPosition { get => m_PreviousPosition; set => m_PreviousPosition = value; }
        public Vector3[] PreviousShapeControlPoints { get => m_PreviousShapeControlPoints; set => m_PreviousShapeControlPoints = value; }

        #endregion

        public OpeningDataFields(OpeningDataSerializedProperties props)
        {
            m_Props = props;

            m_PreviousHeight = m_Props.Height.floatValue;
            m_PreviousWidth = m_Props.Width.floatValue;
            m_PreviousAngle = m_Props.Angle.floatValue;
            m_PreviousPosition = m_Props.Position.vector2Value;
            m_PreviousShapeControlPoints = Opening.Shape.ControlPoints();

            m_HeaderFoldout = new HeaderFoldout(Opening.Name);
            m_ContentFoldout = new Foldout() { text = "Content" };
            m_Container = new VisualElement();
            
            m_Shape = new PropertyField(m_Props.Shape);
            m_Height = new Slider()
            {
                label = m_Props.Height.displayName,
                lowValue = 0,
                highValue = 1,
                showInputField = true
            };
            m_Width = new Slider()
            {
                label = m_Props.Width.displayName,
                lowValue = 0,
                highValue = 1,
                showInputField = true
            };
            m_Angle = new Slider()
            {
                label = m_Props.Angle.displayName,
                lowValue = 0,
                highValue = 180,
                showInputField = true
            };
            m_Position = new Vector2Field()
            {
                label = m_Props.Position.displayName
            };
            m_Content = new InspectorElement(m_Props.Polygon3D);

            m_Height.AddToClassList(k_UssAligned);
            m_Width.AddToClassList(k_UssAligned);
            m_Angle.AddToClassList(k_UssAligned);
            m_Position.AddToClassList(k_UssAligned);

            m_Shape.BindProperty(m_Props.Shape);
            m_Height.BindProperty(m_Props.Height);
            m_Width.BindProperty(m_Props.Width);
            m_Angle.BindProperty(m_Props.Angle);
            m_Position.BindProperty(m_Props.Position);

            m_Container.Add(m_Shape);
            m_Container.Add(m_Height);
            m_Container.Add(m_Width);
            m_Container.Add(m_Angle);
            m_Container.Add(m_Position);
            m_Container.Add(m_ContentFoldout);

            if(m_Props.Polygon3D != m_Props.Data.exposedReferenceValue)
            {
                m_ContentFoldout.Add(m_Content);
            }
            
            m_HeaderFoldout.AddItem(m_Container);
        }

    }

    [System.Serializable]
    class ShapeContainer
    {
        public Shape m_Shape;
    }

    //[CustomPropertyDrawer(typeof(ShapeContainer), false)]
    //class ShapeContainerDrawer : PropertyDrawer
    //{
    //    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    //    {
    //        VisualElement container = new VisualElement();

    //        PropertyField shape = new PropertyField(property.FindPropertyRelative("m_Shape"));

    //        container.Add(shape);

    //        return container;
    //    }
    //}

}
