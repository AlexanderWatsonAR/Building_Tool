using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(FrameData))]
public class FrameDataDrawer : PropertyDrawer, IFieldInitializer
{
    [SerializeField] FrameData m_PreviousData;
    [SerializeField] FrameData m_CurrentData;
    [SerializeField] FrameDataSerializedProperties m_FrameDataProps;

    private VisualElement m_Root;
    private PropertyField m_Depth;
    private IBuildable m_Buildable;
    private PropertyField m_Scale;

    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        Initialize(data);
        m_Root.name = nameof(FrameData) + "_Root";
        m_CurrentData = data.GetUnderlyingValue() as FrameData;
        m_PreviousData = new FrameData(m_CurrentData);

        DefineFields();
        BindFields();
        RegisterValueChangeCallbacks();
        AddFieldsToRoot();

        return m_Root;
    }

    public void Initialize(SerializedProperty data)
    {
        m_Root = new VisualElement();
        m_FrameDataProps = new FrameDataSerializedProperties(data);
        m_Buildable = data.serializedObject.targetObject as IBuildable;
    }

    public void DefineFields()
    {
        m_Scale = new PropertyField(m_FrameDataProps.Scale);
        m_Depth = new PropertyField(m_FrameDataProps.Depth);
    }

    public void BindFields()
    {
        m_Scale.BindProperty(m_FrameDataProps.Scale);
        m_Depth.BindProperty(m_FrameDataProps.Depth);
    }

    public void RegisterValueChangeCallbacks()
    {
        m_Scale.RegisterValueChangeCallback(evt =>
        {
            if (m_CurrentData.Scale == m_PreviousData.Scale)
                return;

            m_PreviousData.Scale = m_CurrentData.Scale;

            Build();
        });
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
        m_Root.Add(m_Scale);
        m_Root.Add(m_Depth);
    }

    private void Build()
    {
        switch(m_Buildable)
        {
            case Frame:
                m_Buildable.Build();
            break;
            case Window:
                // TODO add build case.
            break;
        }
    }

}
