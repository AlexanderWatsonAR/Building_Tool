using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.VisualScripting;

namespace OnlyInvalid.ProcGenBuilding.Window
{
    [CustomPropertyDrawer(typeof(WindowOpeningData))]
    public class WindowOpeningDataDrawer : OpeningDataDrawer
    {
        WindowOpeningData m_CurrentData, m_PreviousData;

        WindowOpeningDataSerializedProperties m_Props;

        Foldout m_GridFoldout, m_ShapeFoldout;
        PropertyField m_Sides, m_Angle;

        protected override void AddFieldsToRoot()
        {
            m_GridFoldout.Add(m_Columns);
            m_GridFoldout.Add(m_Rows);
            m_ShapeFoldout.Add(m_Sides);
            m_ShapeFoldout.Add(m_Height);
            m_ShapeFoldout.Add(m_Width);
            m_ShapeFoldout.Add(m_Angle);
            m_Root.Add(m_GridFoldout);
            m_Root.Add(m_ShapeFoldout);
        }
        protected override void BindFields()
        {
            base.BindFields();

            m_Sides.BindProperty(m_Props.Sides);
            m_Angle.BindProperty(m_Props.Angle);
        }
        protected override void DefineFields()
        {
            base.DefineFields();

            m_GridFoldout = new Foldout() { text = "Grid" };
            m_ShapeFoldout = new Foldout() { text = "Shape" };
            m_Sides = new PropertyField(m_Props.Sides) { label = "Sides" };
            m_Angle = new PropertyField(m_Props.Angle) { label = "Angle" };
        }
        protected override void Initialize(SerializedProperty data)
        {
            base.Initialize(data);
            m_Props = new WindowOpeningDataSerializedProperties(data);
            m_CurrentData = data.GetUnderlyingValue() as WindowOpeningData;
            m_PreviousData = m_CurrentData.Clone() as WindowOpeningData;
        }
        protected override void RegisterValueChangeCallbacks()
        {
            base.RegisterValueChangeCallbacks();

            m_Sides.RegisterValueChangeCallback(evt =>
            {
                int sides = evt.changedProperty.intValue;
                if (sides == m_PreviousData.Sides)
                    return;

                m_PreviousData.Sides = sides;
            });
            m_Angle.RegisterValueChangeCallback(evt =>
            {
                float angle = evt.changedProperty.floatValue;

                if (angle == m_PreviousData.Angle)
                    return;

                m_PreviousData.Angle = angle;
            });
        }
    }
}
