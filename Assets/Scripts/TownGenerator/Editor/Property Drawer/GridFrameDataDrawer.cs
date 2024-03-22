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
    [SerializeField] GridFrameData m_PreviousData;

    GridFrameDataSerializedProperties m_Props;
    VisualElement m_Root;
    Foldout m_Grid;
    PropertyField m_Depth, m_Scale, m_Columns, m_Rows;

    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        Initialize(data);
        m_Root.name = nameof(GridFrameData) + "_Root";
        GridFrameData current = data.GetUnderlyingValue() as GridFrameData;
        m_PreviousData = current.Clone() as GridFrameData;

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
            float scale = evt.changedProperty.floatValue;

            if (scale == m_PreviousData.Scale)
                return;

            m_PreviousData.Scale = scale;
        });
        m_Depth.RegisterValueChangeCallback(evt =>
        {
            if (evt.changedProperty.floatValue == m_PreviousData.Depth)
                return;

            m_PreviousData.Depth = evt.changedProperty.floatValue;
        });
        m_Columns.RegisterValueChangeCallback(evt => 
        {
            if (evt.changedProperty.intValue == m_PreviousData.Columns)
                return;

            m_PreviousData.Columns = evt.changedProperty.intValue;
        });
        m_Rows.RegisterValueChangeCallback(evt =>
        {
            if (evt.changedProperty.intValue == m_PreviousData.Rows)
                return;

            m_PreviousData.Rows = evt.changedProperty.intValue;
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

}
