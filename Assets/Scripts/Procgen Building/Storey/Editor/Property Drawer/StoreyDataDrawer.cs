using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using UnityEditor.Build.Reporting;
using OnlyInvalid.ProcGenBuilding.Corner;
using OnlyInvalid.ProcGenBuilding.Pillar;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Storey
{
    [CustomPropertyDrawer(typeof(StoreyData))]
    public class StoreyDataDrawer : DataDrawer
    {
        StoreyDataSerializedProperties m_Props;

        [SerializeField] StoreyData m_PreviousData;
        [SerializeField] StoreyData m_CurrentData;
        [SerializeField] Buildable m_Buildable;

        Foldout m_StoreyFoldout, m_WallFoldout, m_CornerFoldout, m_PillarFoldout, m_FloorFoldout;
        PropertyField m_ActiveElements;
        PropertyField m_Corner;
        PropertyField m_CornerType, m_CornerSides;
        PropertyField m_WallHeight, m_WallDepth;
        PropertyField m_Pillar;
        PropertyField m_FloorHeight;

        protected override void Initialize(SerializedProperty data)
        {
            m_Props = new StoreyDataSerializedProperties(data);
            m_CurrentData = data.GetUnderlyingValue() as StoreyData;
            m_PreviousData = new StoreyData(m_CurrentData);
            m_Buildable = data.serializedObject.targetObject as Buildable;
        }
        protected override void DefineFields()
        {
            m_Root = new VisualElement() { name = nameof(StoreyData) + "_" + m_Props.Name.stringValue + "_Root" };
            m_StoreyFoldout = new Foldout() { text = m_Props.Name.stringValue };
            m_ActiveElements = new PropertyField(m_Props.ActiveElements);

            m_WallFoldout = new Foldout() { text = "Walls" };
            m_WallHeight = new PropertyField(m_Props.Wall.Height);
            m_WallHeight.SetEnabled(m_Buildable is OnlyInvalid.ProcGenBuilding.Building.Building);
            m_WallDepth = new PropertyField(m_Props.Wall.Depth);

            m_CornerFoldout = new Foldout() { text = "Corners" };
            m_Corner = new PropertyField(m_Props.Corner.Data);

            m_PillarFoldout = new Foldout() { text = "Pillars" };
            m_Pillar = new PropertyField(m_Props.Pillar.Data);

            m_FloorFoldout = new Foldout() { text = "Floor" };
            m_FloorHeight = new PropertyField(m_Props.Floor.Height);
        }
        protected override void BindFields()
        {
            m_ActiveElements.BindProperty(m_Props.ActiveElements);
            m_WallHeight.BindProperty(m_Props.Wall.Height);
            m_WallDepth.BindProperty(m_Props.Wall.Depth);
            m_Corner.BindProperty(m_Props.Corner.Data);
            m_Pillar.BindProperty(m_Props.Pillar.Data);
            m_FloorHeight.BindProperty(m_Props.Floor.Height);
        }
        protected override void RegisterValueChangeCallbacks()
        {
            m_ActiveElements.RegisterValueChangeCallback(evt =>
            {
                StoreyElement activeElement = evt.changedProperty.GetEnumValue<StoreyElement>();

                if (activeElement == m_PreviousData.ActiveElements)
                    return;

                m_PreviousData.ActiveElements = activeElement;

                bool isWallActive = activeElement.IsElementActive(StoreyElement.Walls);
                bool isPillarActive = activeElement.IsElementActive(StoreyElement.Pillars);
                bool isFloorActive = activeElement.IsElementActive(StoreyElement.Floor);

                m_WallFoldout.SetEnabled(isWallActive);
                m_PillarFoldout.SetEnabled(isPillarActive);
                m_FloorFoldout.SetEnabled(isFloorActive);

            });
            m_WallHeight.RegisterValueChangeCallback(evt =>
            {
                if (m_CurrentData.Walls == null)
                    return;

                float height = evt.changedProperty.floatValue;

                if (height == m_PreviousData.WallData.Height)
                    return;

                m_PreviousData.WallData.Height = height;

                for (int i = 0; i < m_CurrentData.Walls.Length; i++)
                {
                    m_CurrentData.Walls[i].Height = height;
                }
            });
            m_WallDepth.RegisterValueChangeCallback(evt =>
            {
                if (m_CurrentData.Walls == null)
                    return;

                float depth = evt.changedProperty.floatValue;

                if (depth == m_PreviousData.WallData.Depth)
                    return;

                m_PreviousData.WallData.Depth = depth;

                for (int i = 0; i < m_CurrentData.Walls.Length; i++)
                {
                    m_CurrentData.Walls[i].Depth = depth;
                }
            });
            m_Corner.RegisterValueChangeCallback(evt =>
            {
                CornerData corner = evt.changedProperty.GetUnderlyingValue() as CornerData;

                if (m_PreviousData.CornerData.Equals(corner))
                {
                    return;
                }

                m_PreviousData.CornerData.Sides = corner.Sides;
                m_PreviousData.CornerData.Type = corner.Type;

                for (int i = 0; i < m_CurrentData.Pillars.Length; i++)
                {
                    m_CurrentData.Corners[i] = new CornerData(corner);
                }

            });
            m_Pillar.RegisterValueChangeCallback(evt =>
            {
                PillarData pillar = evt.changedProperty.GetUnderlyingValue() as PillarData;

                if (m_PreviousData.Pillar.Equals(pillar))
                {
                    return;
                }

                m_PreviousData.Pillar.Height = pillar.Height;
                m_PreviousData.Pillar.Width = pillar.Width;
                m_PreviousData.Pillar.Depth = pillar.Depth;
                m_PreviousData.Pillar.Sides = pillar.Sides;
                m_PreviousData.Pillar.IsSmooth = pillar.IsSmooth;

                for (int i = 0; i < m_CurrentData.Pillars.Length; i++)
                {
                    m_CurrentData.Pillars[i] = new PillarData(pillar);
                }
            });
        }
        protected override void AddFieldsToRoot()
        {
            m_Root.Add(m_StoreyFoldout);
            m_WallFoldout.Add(m_WallHeight);
            m_WallFoldout.Add(m_WallDepth);
            m_WallFoldout.Add(m_CornerFoldout);
            m_CornerFoldout.Add(m_Corner);
            m_PillarFoldout.Add(m_Pillar);
            m_FloorFoldout.Add(m_FloorHeight);
            m_StoreyFoldout.Add(m_ActiveElements);
            m_StoreyFoldout.Add(m_WallFoldout);
            m_StoreyFoldout.Add(m_PillarFoldout);
            m_StoreyFoldout.Add(m_FloorFoldout);
        }
    }
}
