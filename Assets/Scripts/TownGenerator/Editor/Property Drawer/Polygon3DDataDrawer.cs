using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.VisualScripting;
using UnityEditor.Build.Reporting;

[CustomPropertyDrawer(typeof(Polygon3DData))]
public class Polygon3DDataDrawer : PropertyDrawer, IFieldInitializer
{
    IBuildable m_Buildable;

    [SerializeField] Polygon3DData m_PreviousData;

    Polygon3DDataSerializedProperties m_Props;
    VisualElement m_Root;
    PropertyField m_Depth;

    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        Initialize(data);
        m_Root.name = nameof(FrameData) + "_Root";
        Polygon3DData currentData = data.GetUnderlyingValue() as Polygon3DData;
        m_PreviousData = currentData.Clone() as Polygon3DData;

        DefineFields();
        BindFields();
        RegisterValueChangeCallbacks();
        AddFieldsToRoot();

        return m_Root;
    }

    public void Initialize(SerializedProperty data)
    {
        m_Root = new VisualElement();
        m_Props = new Polygon3DDataSerializedProperties(data);
        m_Buildable = data.serializedObject.targetObject as IBuildable;
    }
    public void DefineFields()
    {
        m_Depth = new PropertyField(m_Props.Depth, "Depth");
    }
    public void BindFields()
    {
        m_Depth.BindProperty(m_Props.Depth);
    }
    public void RegisterValueChangeCallbacks()
    {
        m_Depth.RegisterValueChangeCallback(evt =>
        {
            float depth = evt.changedProperty.floatValue;

            if (depth == m_PreviousData.Depth)
                return;

            m_PreviousData.Depth = depth;

            Build();
        });
    }
    public void AddFieldsToRoot()
    {
        m_Root.Add(m_Depth);
    }
    public void Build()
    {
        switch(m_Buildable)
        {
            case Pane:
                m_Buildable.Build();
            break;
            case Window:
                Window window = m_Buildable as Window;
                window.Data.DoesPaneNeedRebuild = true;
                window.BuildPane();
            break;
            case WallSection:
                WallSection section = m_Buildable as WallSection;
                switch(section.Data.WallElement)
                {
                    case WallElement.Window:
                        section.BuildWindows(false, false, true);
                    break;
                }
            break;
        }
    }

}
