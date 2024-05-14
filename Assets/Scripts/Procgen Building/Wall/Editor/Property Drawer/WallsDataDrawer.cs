using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.VisualScripting;

namespace OnlyInvalid.ProcGenBuilding.Wall
{
    public class WallsDataDrawer : DataDrawer
    {
        WallsDataSerializedProperties m_Props;
        WallsData m_PreviousData;

        PropertyField m_Height, m_Depth;

        protected override void AddFieldsToRoot()
        {
            m_Root.Add(m_Height);
            m_Root.Add(m_Depth);
        }

        protected override void BindFields()
        {
            m_Height.BindProperty(m_Props.Wall.Height);
            m_Depth.BindProperty(m_Props.Wall.Depth);
        }

        protected override void DefineFields()
        {
            m_Root = new VisualElement() { name = nameof(WallsData) + "_Root" };
            m_Height = new PropertyField(m_Props.Wall.Height, "Height");
            m_Depth = new PropertyField(m_Props.Wall.Depth, "Depth");
        }

        protected override void Initialize(SerializedProperty data)
        {
            m_Root = new VisualElement();
            m_Props = new WallsDataSerializedProperties(data);
            WallsData current = data.GetUnderlyingValue() as WallsData;
            m_PreviousData = current.Clone() as WallsData;
        }

        protected override void RegisterValueChangeCallbacks()
        {
            m_Height.RegisterValueChangeCallback(evt =>
            {
            });
        }
    }
}