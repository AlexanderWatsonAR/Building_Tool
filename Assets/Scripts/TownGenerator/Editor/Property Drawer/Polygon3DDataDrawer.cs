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
    [SerializeField] Polygon3DData m_CurrentData;

    Polygon3DDataSerializedProperties m_Polygon3DProps;
    VisualElement m_Root;
    PropertyField m_Depth;

    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        Initialize(data);
        m_Root.name = nameof(FrameData) + "_Root";
        m_CurrentData = data.GetUnderlyingValue() as Polygon3DData;
        m_PreviousData = new Polygon3DData(m_CurrentData);

        DefineFields();
        BindFields();
        RegisterValueChangeCallbacks();
        AddFieldsToRoot();

        return m_Root;
    }

    public void Initialize(SerializedProperty data)
    {
        m_Root = new VisualElement();
        m_Polygon3DProps = new Polygon3DDataSerializedProperties(data);
        m_Buildable = data.serializedObject.targetObject as IBuildable;
    }
    public void DefineFields()
    {
        m_Depth = new PropertyField(m_Polygon3DProps.Depth, "Depth");
    }
    public void BindFields()
    {
        m_Depth.BindProperty(m_Polygon3DProps.Depth);
    }
    public void RegisterValueChangeCallbacks()
    {
        m_Depth.RegisterValueChangeCallback(evt =>
        {
            if (m_CurrentData.Depth == m_PreviousData.Depth)
                return;

            m_PreviousData.Depth = m_CurrentData.Depth;

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
                window.BuildPane();
                break;
        }
    }

}
