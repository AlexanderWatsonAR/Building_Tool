using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Unity.VisualScripting;

[CustomPropertyDrawer(typeof(GridFrameData))]
public class GridFrameDataDrawer : PropertyDrawer, IFieldInitializer
{
    IBuildable m_Buildable;

    [SerializeField] GridFrameData m_PreviousData;
    [SerializeField] GridFrameData m_CurrentData;
    [SerializeField] GridFrameDataSerializedProperties m_Props;

    VisualElement m_Root;
    Foldout m_Grid;
    PropertyField m_Depth, m_Scale, m_Columns, m_Rows;

    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        Initialize(data);
        m_Root.name = nameof(GridFrameData) + "_Root";
        m_CurrentData = data.GetUnderlyingValue() as GridFrameData;
        m_PreviousData = new GridFrameData(m_CurrentData);

        DefineFields();
        BindFields();
        RegisterValueChangeCallbacks();
        AddFieldsToRoot();

        return m_Root;
    }
    public void Initialize(SerializedProperty data)
    {
        m_Root = new VisualElement();
        m_Props = new GridFrameDataSerializedProperties(data);
        m_Buildable = data.serializedObject.targetObject as IBuildable;
    }
    public void DefineFields()
    {
        m_Grid = new Foldout() { text = "Grid"};
        m_Scale = new PropertyField(m_Props.Scale, "Scale");
        m_Depth = new PropertyField(m_Props.Depth, "Depth");
        m_Columns = new PropertyField(m_Props.Columns, "Columns");
        m_Rows = new PropertyField(m_Props.Rows, "Rows");
    }
    public void BindFields()
    {
        m_Scale.BindProperty(m_Props.Scale);
        m_Depth.BindProperty(m_Props.Depth);
        m_Columns.BindProperty(m_Props.Columns);
        m_Rows.BindProperty(m_Props.Rows);
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
        m_Columns.RegisterValueChangeCallback(evt => 
        {
            if (m_CurrentData.Columns == m_PreviousData.Columns)
                return;

            m_PreviousData.Columns = m_CurrentData.Columns;

            Build();

        });
        m_Rows.RegisterValueChangeCallback(evt =>
        {
            if (m_CurrentData.Rows == m_PreviousData.Rows)
                return;

            m_PreviousData.Rows = m_CurrentData.Rows;

            Build();

        });
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
        m_Grid.Add(m_Columns);
        m_Grid.Add(m_Rows);
        m_Root.Add(m_Grid);
        m_Root.Add(m_Scale);
        m_Root.Add(m_Depth);
    }

    private void Build()
    {
        switch (m_Buildable)
        {
            case GridFrame:
                m_Buildable.Build();
                break;
            case Window:
                Window window = m_Buildable as Window;
                window.BuildOuterFrame();
                break;
        }
    }

}
