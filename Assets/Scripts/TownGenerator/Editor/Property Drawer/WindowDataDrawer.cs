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

[CustomPropertyDrawer(typeof(WindowData))]
public class WindowDataDrawer : PropertyDrawer, IFieldInitializer
{
    VisualElement m_Root;
    WindowDataSerializedProperties m_Props;
    WindowData m_CurrentData;

    Foldout m_OuterFrameFoldout, m_InnerFrameFoldout, m_PaneFoldout, m_ShuttersFoldout;
    PropertyField m_ActiveElements, m_OuterFrame, m_InnerFrame, m_Pane, m_LeftShutter, m_RightShutter;

    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        Initialize(data);
        m_Root.name = nameof(WindowData) + "_Root";

        DefineFields();
        BindFields();
        RegisterValueChangeCallbacks();
        AddFieldsToRoot();

        return m_Root;
    }

    public void Initialize(SerializedProperty data)
    {
        m_Root = new VisualElement();
        m_Props = new WindowDataSerializedProperties(data);
        m_CurrentData = data.GetUnderlyingValue() as WindowData;
    }
    public void DefineFields()
    {
        m_ActiveElements = new PropertyField(m_Props.ActiveElements);
        m_OuterFrameFoldout = new Foldout() { text = "Outer Frame" };
        m_OuterFrame = new PropertyField(m_Props.OuterFrame.Data);
        m_InnerFrameFoldout = new Foldout() { text = "Inner Frame" };
        m_InnerFrame = new PropertyField(m_Props.InnerFrame.Data);
        m_PaneFoldout = new Foldout() { text = "Pane" };
        m_Pane = new PropertyField(m_Props.Data);
        m_ShuttersFoldout = new Foldout() { text = "Shutters" };
        // How did this look before?
        m_LeftShutter = new PropertyField(m_Props.LeftShutter.Data);
        m_RightShutter = new PropertyField(m_Props.RightShutter.Data);
    }

    public void BindFields()
    {
        m_ActiveElements.BindProperty(m_Props.ActiveElements);
        m_OuterFrame.BindProperty(m_Props.OuterFrame.Data);
        m_InnerFrame.BindProperty(m_Props.InnerFrame.Data);
        m_Pane.BindProperty(m_Props.Pane.Data);
        m_LeftShutter.BindProperty(m_Props.LeftShutter.Data);
        m_RightShutter.BindProperty(m_Props.RightShutter.Data);
    }

    public void RegisterValueChangeCallbacks()
    {
        #region Register Value Change Callback
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
        });
        #endregion
    }

    public void AddFieldsToRoot()
    {
        #region Add Fields to Container
        m_OuterFrameFoldout.Add(m_OuterFrame);
        m_InnerFrameFoldout.Add(m_InnerFrame);
        m_PaneFoldout.Add(m_Pane);
        m_ShuttersFoldout.Add(m_LeftShutter);
        m_ShuttersFoldout.Add(m_RightShutter);
        m_Root.Add(m_ActiveElements);
        m_Root.Add(m_OuterFrameFoldout);
        m_Root.Add(m_InnerFrameFoldout);
        m_Root.Add(m_PaneFoldout);
        m_Root.Add(m_ShuttersFoldout);
        #endregion
    }
}
