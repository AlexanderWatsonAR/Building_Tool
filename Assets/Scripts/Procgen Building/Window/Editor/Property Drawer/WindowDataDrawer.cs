using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Rendering;
using System.Linq;
using Unity.VisualScripting;
using static PlasticPipe.Server.MonitorStats;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using System;
using OnlyInvalid.CustomVisualElements;

namespace OnlyInvalid.ProcGenBuilding.Window
{
    [CustomPropertyDrawer(typeof(WindowData))]
    public class WindowDataDrawer : DataDrawer
    {
        WindowDataSerializedProperties m_Props;
        WindowData m_CurrentData;
        WindowData m_PreviousData;

        CustomVisualElements.HeaderFoldout m_OuterFrameFoldout, m_InnerFrameFoldout, m_PaneFoldout, m_ShuttersFoldout;
        PropertyField m_ActiveElements, m_OuterFrame, m_InnerFrame, m_Pane, m_LeftShutter, m_RightShutter;

        protected override void Initialize(SerializedProperty data)
        {
            m_Props = new WindowDataSerializedProperties(data);
            m_CurrentData = data.GetUnderlyingValue() as WindowData;
            m_PreviousData = m_CurrentData.Clone() as WindowData;
        }
        protected override void DefineFields()
        {
            m_ActiveElements = new PropertyField(m_Props.ActiveElements);
            m_OuterFrameFoldout = new HeaderFoldout("Outer Frame");
            m_OuterFrame = new PropertyField(m_Props.OuterFrame.Data);
            m_InnerFrameFoldout = new HeaderFoldout("Inner Frame");
            m_InnerFrame = new PropertyField(m_Props.InnerFrame.Data);
            m_PaneFoldout = new HeaderFoldout("Pane");
            m_Pane = new PropertyField(m_Props.Data);
            m_ShuttersFoldout = new HeaderFoldout("Shutters");
            // How did this look before?
            m_LeftShutter = new PropertyField(m_Props.LeftShutter.Data);
            m_RightShutter = new PropertyField(m_Props.RightShutter.Data);
        }
        protected override void BindFields()
        {
            m_ActiveElements.BindProperty(m_Props.ActiveElements);
            m_OuterFrame.BindProperty(m_Props.OuterFrame.Data);
            m_InnerFrame.BindProperty(m_Props.InnerFrame.Data);
            m_Pane.BindProperty(m_Props.Pane.Data);
            m_LeftShutter.BindProperty(m_Props.LeftShutter.Data);
            m_RightShutter.BindProperty(m_Props.RightShutter.Data);
        }
        protected override void RegisterValueChangeCallbacks()
        {
            m_ActiveElements.RegisterValueChangeCallback(evt =>
            {
                WindowElement currentlyActive = evt.changedProperty.GetEnumValue<WindowElement>();

                bool isOuterFrameActive = currentlyActive.IsElementActive(WindowElement.OuterFrame);
                bool isInnerFrameActive = currentlyActive.IsElementActive(WindowElement.InnerFrame);
                bool isPaneActive = currentlyActive.IsElementActive(WindowElement.Pane);
                bool areShuttersActive = currentlyActive.IsElementActive(WindowElement.Shutters);

                m_OuterFrameFoldout.SetEnabled(isOuterFrameActive);
                m_InnerFrameFoldout.SetEnabled(isInnerFrameActive);
                m_PaneFoldout.SetEnabled(isPaneActive);
                m_ShuttersFoldout.SetEnabled(areShuttersActive);

                if(m_PreviousData.ActiveElements != currentlyActive)
                    m_CurrentData.IsDirty = true;

            });

            m_OuterFrame.RegisterValueChangeCallback(evt =>
            {
                OuterFrameData frameData = evt.changedProperty.GetUnderlyingValue() as OuterFrameData;

                if (frameData.Equals(m_PreviousData.OuterFrame))
                    return;

                if (frameData.FrameScale != m_PreviousData.OuterFrame.FrameScale)
                {
                    m_CurrentData.IsDirty = true;
                }
                    
                m_PreviousData.OuterFrame = frameData.Clone() as OuterFrameData;
            });
            m_InnerFrame.RegisterValueChangeCallback(evt => 
            {
                InnerFrameData frameData = evt.changedProperty.GetUnderlyingValue() as InnerFrameData;

                if (frameData.Equals(m_PreviousData.InnerFrame))
                    return;

                m_CurrentData.InnerFrame.IsDirty = true;
                m_PreviousData.InnerFrame = frameData.Clone() as InnerFrameData;
            });
            m_Pane.RegisterValueChangeCallback(evt => 
            {
                PaneData paneData = evt.changedProperty.GetUnderlyingValue() as PaneData;

                if (paneData.Equals(m_PreviousData.Pane))
                    return;

                m_CurrentData.Pane.IsDirty = true;
                m_PreviousData.Pane = paneData.Clone() as PaneData;
            });

        }
        protected override void AddFieldsToRoot()
        {
            m_OuterFrameFoldout.AddItem(m_OuterFrame);
            m_InnerFrameFoldout.AddItem(m_InnerFrame);
            m_PaneFoldout.AddItem(m_Pane);
            m_ShuttersFoldout.AddItem(m_LeftShutter);
            m_ShuttersFoldout.AddItem(m_RightShutter);
            m_Root.Add(m_ActiveElements);
            m_Root.Add(m_OuterFrameFoldout);
            m_Root.Add(m_InnerFrameFoldout);
            m_Root.Add(m_PaneFoldout);
            m_Root.Add(m_ShuttersFoldout);
        }
    }
}
