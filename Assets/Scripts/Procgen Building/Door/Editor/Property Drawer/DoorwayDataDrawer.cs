using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.VisualScripting;
using static UnityEngine.Rendering.DebugUI.Table;

namespace OnlyInvalid.ProcGenBuilding.Door
{
    [CustomPropertyDrawer(typeof(DoorwayData))]
    public class DoorwayDataDrawer : OpeningDataDrawer
    {
        DoorwayDataSerializedProperties m_Props;

        Foldout m_GridFoldout, m_SizePositionFoldout;
        PropertyField m_PositionOffset;

        DoorwayData m_PreviousData;

        protected override void AddFieldsToRoot()
        {
            m_GridFoldout.Add(m_Columns);
            m_GridFoldout.Add(m_Rows);
            m_SizePositionFoldout.Add(m_PositionOffset);
            m_SizePositionFoldout.Add(m_Height);
            m_SizePositionFoldout.Add(m_Width);
            m_Root.Add(m_GridFoldout);
            m_Root.Add(m_SizePositionFoldout);
        }
        protected override void BindFields()
        {
            base.BindFields();
            m_PositionOffset.BindProperty(m_Props.PositionOffset);
        }
        protected override void DefineFields()
        {
            base.DefineFields();
            m_GridFoldout = new Foldout() { text = "Grid" };
            m_SizePositionFoldout = new Foldout() { text = "Size & Position" };
            m_PositionOffset = new PropertyField(m_Props.PositionOffset, "Offset") { label = "Offset" };
        }
        protected override void Initialize(SerializedProperty data)
        {
            base.Initialize(data);
            m_Props = new DoorwayDataSerializedProperties(data);
            m_Root = new VisualElement() { name = nameof(DoorwayData) + "_Root" };
            DoorwayData doorway = data.GetUnderlyingValue() as DoorwayData;
            m_PreviousData = doorway.Clone() as DoorwayData;
        }
        protected override void RegisterValueChangeCallbacks()
        {
            base.RegisterValueChangeCallbacks();
            m_PositionOffset.RegisterValueChangeCallback(evt =>
            {
                float offset = evt.changedProperty.floatValue;

                if (offset == m_PreviousData.PositionOffset)
                    return;

                m_PreviousData.PositionOffset = offset;
            });
        }
    }
}
