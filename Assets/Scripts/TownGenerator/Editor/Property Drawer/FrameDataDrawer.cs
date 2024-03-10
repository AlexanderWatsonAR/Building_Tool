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
    IBuildable m_Buildable;
    [SerializeField] FrameData m_PreviousData;

    FrameDataSerializedProperties m_FrameDataProps;
    VisualElement m_Root;
    PropertyField m_Depth, m_Scale;

    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        Initialize(data);
        m_Root.name = nameof(FrameData) + "_Root";
        m_PreviousData = new FrameData(data.GetUnderlyingValue() as FrameData);

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

        m_Scale.SetEnabled(m_Buildable is not Frame);
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
            float scale = evt.changedProperty.floatValue;

            if (scale == m_PreviousData.Scale)
                return;

            m_PreviousData.Scale = scale;

            Build();
        });
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
                Window window = m_Buildable as Window;
                window.Rebuild();
            break;
            case WallSection:
                WallSection wallSection = m_Buildable as WallSection;

                switch(wallSection.Data.WallElement)
                {
                    case WallElement.Window:
                        wallSection.BuildWindows(true, true, true, true);
                    break;
                }
            break;
        }
    }

}
