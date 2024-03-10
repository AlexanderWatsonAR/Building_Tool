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
    //[SerializeField] GridFrameData m_CurrentData;

    GridFrameDataSerializedProperties m_Props;
    VisualElement m_Root;
    Foldout m_Grid;
    PropertyField m_Depth, m_Scale, m_Columns, m_Rows;

    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        Initialize(data);
        m_Root.name = nameof(GridFrameData) + "_Root";
        GridFrameData current = data.GetUnderlyingValue() as GridFrameData;
        m_PreviousData = new GridFrameData(current);

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
            float scale = evt.changedProperty.floatValue;

            if (scale == m_PreviousData.Scale)
                return;

            m_PreviousData.Scale = scale;

            GridFrameData[] frames = GetDataFromBuildable();

            foreach(GridFrameData frame in frames)
            {
                frame.Scale = scale;
            }

            Build();
        });
        m_Depth.RegisterValueChangeCallback(evt =>
        {
            if (evt.changedProperty.floatValue == m_PreviousData.Depth)
                return;

            m_PreviousData.Depth = evt.changedProperty.floatValue;

            Build();
        });
        m_Columns.RegisterValueChangeCallback(evt => 
        {
            if (evt.changedProperty.intValue == m_PreviousData.Columns)
                return;

            m_PreviousData.Columns = evt.changedProperty.intValue;

            Build();

        });
        m_Rows.RegisterValueChangeCallback(evt =>
        {
            if (evt.changedProperty.intValue == m_PreviousData.Rows)
                return;

            m_PreviousData.Rows = evt.changedProperty.intValue;

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
    /// <summary>
    /// A bit complicated, but this alllows us to modify the grid frame data from multiple objects.
    /// </summary>
    /// <returns></returns>
    private GridFrameData[] GetDataFromBuildable()
    {
        GridFrameData[] frameData = null;

        switch(m_Buildable)
        {
            case WallSection:
                WallSection section = m_Buildable as WallSection;
                switch (section.Data.WallElement)
                {
                    case WallElement.Window:
                        frameData = new GridFrameData[section.Data.WindowOpening.Windows.Length];
                        for(int i = 0; i < frameData.Length; i++)
                        {
                            frameData[i] = section.Data.WindowOpening.Windows[i].InnerFrame;
                        }
                        break;
                }
           break;
        }

        return frameData;
    }


    private void Build()
    {
        switch (m_Buildable)
        {
            case GridFrame:
                m_Buildable.Build();
                break;
            case Window:
                {
                    // window innerframe data & gridframedata are the same object.
                    Window window = m_Buildable as Window;
                    window.Data.DoesInnerFrameNeedRebuild = true;
                    window.BuildInnerFrame();
                }
                break;
            case WallSection:
                // TODO: We want to pass the grid frame data down to the windows collection.
                WallSection section = m_Buildable as WallSection;
                switch(section.Data.WallElement)
                {
                    case WallElement.Window:
                        section.BuildWindows(false, true);
                        break;
                }
                break;
        }
    }

}