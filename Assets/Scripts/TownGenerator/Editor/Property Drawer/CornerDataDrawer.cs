using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.VisualScripting;
using UnityEditor.Rendering;

[CustomPropertyDrawer(typeof(CornerData))]
public class CornerDataDrawer : PropertyDrawer, IFieldInitializer
{
    CornerDataSerializedProperties m_Props;

    [SerializeField] CornerData m_PreviousData;
    [SerializeField] IBuildable m_Buildable;

    VisualElement m_Root;
    PropertyField m_Type, m_Sides;

    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        Initialize(data);
        DefineFields();
        BindFields();
        RegisterValueChangeCallbacks();
        AddFieldsToRoot();

        return m_Root;
    }
    public void Initialize(SerializedProperty data)
    {
        m_Props = new CornerDataSerializedProperties(data);
        m_Buildable = data.serializedObject.targetObject as IBuildable;
        m_PreviousData = new CornerData(data.GetUnderlyingValue() as CornerData);
    }
    public void DefineFields()
    {
        m_Root = new VisualElement() { name = nameof(CornerData) + "_Root" };
        m_Type = new PropertyField(m_Props.Type);
        m_Sides = new PropertyField(m_Props.Sides);
    }
    public void BindFields()
    {
        m_Type.BindProperty(m_Props.Type);
        m_Sides.BindProperty(m_Props.Sides);
    }
    public void RegisterValueChangeCallbacks()
    {
        m_Type.RegisterValueChangeCallback(evt =>
        {
            CornerType type = evt.changedProperty.GetEnumValue<CornerType>();

            if (type == m_PreviousData.Type)
                return;

            m_PreviousData.Type = type;

            CornerData[] corners = GetDataFromBuildable();

            foreach(CornerData corner in corners)
            {
                corner.Type = type;
            }
        });
        m_Sides.RegisterValueChangeCallback(evt =>
        {
            int sides = evt.changedProperty.intValue;

            if (sides == m_PreviousData.Sides)
                return;

            m_PreviousData.Sides = sides;

            CornerData[] corners = GetDataFromBuildable();

            foreach (CornerData corner in corners)
            {
                corner.Sides = sides;
            }
        });

    }
    public void AddFieldsToRoot()
    {
        m_Root.Add(m_Type);
        m_Root.Add(m_Sides);
    }

    private CornerData[] GetDataFromBuildable()
    {
        CornerData[] cornerData = null;

        switch (m_Buildable)
        {
            case Building:
                cornerData = new CornerData[0];
                // How can I get the corners?
                // we have a list of storey data
                // how can I know which of them to get the corners data from.
                break;
            case Storey:
                Storey storey = m_Buildable as Storey;
                cornerData = storey.Data.Corners;
                break;
            case Corner:
                cornerData = new CornerData[1];
                cornerData[0] = m_Props.Data.GetUnderlyingValue() as CornerData;
                break;
        }

        return cornerData;
    }

    private void Build()
    {
        switch (m_Buildable)
        {
            case Building:
                Building building = m_Buildable as Building;
                building.BuildStoreys();
                break;
            case Storey:
                Storey storey = m_Buildable as Storey;
                storey.BuildCorners();
            break;
            case Corner:
                m_Buildable.Build();
            break;
        }
    }
}
