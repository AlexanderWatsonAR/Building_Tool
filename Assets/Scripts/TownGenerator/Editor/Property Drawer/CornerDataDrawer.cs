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
        });
        m_Sides.RegisterValueChangeCallback(evt =>
        {
            int sides = evt.changedProperty.intValue;

            if (sides == m_PreviousData.Sides)
                return;

            m_PreviousData.Sides = sides;
        });

    }
    public void AddFieldsToRoot()
    {
        m_Root.Add(m_Type);
        m_Root.Add(m_Sides);
    }
}
