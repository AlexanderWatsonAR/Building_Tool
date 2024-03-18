using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.VisualScripting;

[CustomPropertyDrawer(typeof(DoorwayData))]
public class DoorwayDataDrawer : PropertyDrawer, IFieldInitializer
{
    DoorwayDataSerializedProperties m_Props;

    VisualElement m_Root;

    Foldout m_GridFoldout, m_SizePositionFoldout;
    PropertyField m_Columns, m_Rows;
    PropertyField m_PositionOffset;
    PropertyField m_Height, m_Width;

    [SerializeField] DoorwayData m_PreviousData;

    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        Initialize(data);
        DefineFields();
        BindFields();
        //RegisterValueChangeCallbacks();
        AddFieldsToRoot();

        return m_Root;
    }

    public void AddFieldsToRoot()
    {
        m_GridFoldout.Add(m_Columns);
        m_GridFoldout.Add(m_Rows);
        m_SizePositionFoldout.Add(m_PositionOffset);
        m_SizePositionFoldout.Add(m_Height);
        m_SizePositionFoldout.Add(m_Width);
        m_Root.Add(m_GridFoldout);
        m_Root.Add(m_SizePositionFoldout);
    }

    public void BindFields()
    {
        m_Columns.BindProperty(m_Props.Columns);
        m_Rows.BindProperty(m_Props.Rows);
        m_PositionOffset.BindProperty(m_Props.PositionOffset);
        m_Height.BindProperty(m_Props.Height);
        m_Width.BindProperty(m_Props.Width);
    }

    public void DefineFields()
    {
        m_GridFoldout = new Foldout() { text = "Grid" };
        m_Columns = new PropertyField(m_Props.Columns, "Columns") { label = "Columns" };
        m_Rows = new PropertyField(m_Props.Rows, "Rows") { label = "Rows" };
        m_SizePositionFoldout = new Foldout() { text = "Size & Position" };
        m_PositionOffset = new PropertyField(m_Props.PositionOffset, "Offset") { label = "Offset" };
        m_Height = new PropertyField(m_Props.Height, "Height") { label = "Height" };
        m_Width = new PropertyField(m_Props.Width, "Width") { label = "Width" };
    }

    public void Initialize(SerializedProperty data)
    {
        m_Props = new DoorwayDataSerializedProperties(data);
        m_Root = new VisualElement() { name = nameof(DoorwayData) + "_Root" };
        DoorwayData doorway = data.GetUnderlyingValue() as DoorwayData;
        m_PreviousData = doorway.Clone() as DoorwayData;
    }

    public void RegisterValueChangeCallbacks()
    {
        throw new System.NotImplementedException();
    }
}
