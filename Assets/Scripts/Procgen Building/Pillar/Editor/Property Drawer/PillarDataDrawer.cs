using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using Unity.VisualScripting;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Pillar
{
    [CustomPropertyDrawer(typeof(PillarData))]
    public class PillarDataDrawer : DataDrawer
    {
        PillarDataSerializedProperties m_Props;

        FloatField m_Depth;
        Vector3Field m_Scale;
        SliderInt m_Sides;
        //Toggle m_IsSmooth;

        protected override void Initialize(SerializedProperty data)
        {
            m_Root.name = nameof(PillarData) + "_Root" ;
            m_Props = new PillarDataSerializedProperties(data);
        }

        protected override void DefineFields()
        {
            m_Scale = new Vector3Field("Scale");

            m_Depth = new FloatField("Height");
            m_Sides = new SliderInt()
            {
                label = DisplayDataSettings.Data.Pillar.Sides.label,
                lowValue = DisplayDataSettings.Data.Pillar.Sides.range.lower,
                highValue = DisplayDataSettings.Data.Pillar.Sides.range.upper,
                inverted = DisplayDataSettings.Data.Pillar.Sides.inverted,
                showInputField = DisplayDataSettings.Data.Pillar.Sides.showInputField,
                direction = DisplayDataSettings.Data.Pillar.Sides.direction
            };
            //m_IsSmooth = new Toggle("Smooth");
        }
        protected override void BindFields()
        {
            m_Scale.BindProperty(m_Props.Scale);
            m_Depth.BindProperty(m_Props.Depth);
            m_Sides.BindProperty(m_Props.Sides);
            //m_IsSmooth.BindProperty(m_Props.IsSmooth);
        }

        protected override void RegisterValueChangeCallbacks()
        {

        }

        protected override void AddFieldsToRoot()
        {
            m_Root.Add(m_Scale);
            m_Root.Add(m_Depth);
            m_Root.Add(m_Sides);
//            m_Root.Add(m_IsSmooth);
        }

    }
}
