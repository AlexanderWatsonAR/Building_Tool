using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using OnlyInvalid.ProcGenBuilding.Common;
using UnityEngine.ProBuilder.Shapes;

namespace OnlyInvalid.ProcGenBuilding.Corner
{
    [CustomPropertyDrawer(typeof(CornerData))]
    public class CornerDataDrawer : DataDrawer
    {
        CornerDataSerializedProperties m_Props;

        PropertyField m_Type;
        SliderInt m_Sides;

        protected override void Initialize(SerializedProperty data)
        {
            m_Props = new CornerDataSerializedProperties(data);
        }
        protected override void DefineFields()
        {
            var settings = DisplayDataSettings.Data.Corner.Sides;

            m_Type = new PropertyField(m_Props.Type);
            m_Sides = new SliderInt()
            {
                label = settings.label,
                value = m_Props.Sides.intValue,
                lowValue = settings.range.lower,
                highValue = settings.range.upper,
                showInputField = settings.showInputField,
                direction = settings.direction,
                inverted = settings.inverted,
            };
        }
        protected override void BindFields()
        {
            m_Type.BindProperty(m_Props.Type);
            m_Sides.BindProperty(m_Props.Sides);
        }
        protected override void AddFieldsToRoot()
        {
            m_Root.Clear();
            m_Root.Add(m_Type);

            if(m_Props.Type.GetEnumValue<CornerType>() == CornerType.Round)
                m_Root.Add(m_Sides);
        }

        protected override void RegisterValueChangeCallbacks()
        {
            m_Type.RegisterValueChangeCallback(evt =>
            {
                AddFieldsToRoot();
            });
        }
    }
}
