using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using OnlyInvalid.ProcGenBuilding.Common;

[CustomPropertyDrawer(typeof(DirtyData), useForChildren:true)]
public abstract class DataDrawer : PropertyDrawer
{
    protected VisualElement m_Root;

    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        m_Root = new VisualElement() { name = data.boxedValue.GetType().Name + "_Root" };
        Initialize(data);
        DefineFields();
        BindFields();
        RegisterValueChangeCallbacks();
        AddFieldsToRoot();
        return m_Root;
    }

    protected abstract void Initialize(SerializedProperty data);
    protected abstract void DefineFields();
    protected abstract void BindFields();
    protected abstract void RegisterValueChangeCallbacks();
    protected abstract void AddFieldsToRoot();

}

[CustomPropertyDrawer(typeof(OpeningData), useForChildren:true)]
public abstract class OpeningDataDrawer : DataDrawer
{
    protected PropertyField m_Columns, m_Rows, m_Height, m_Width;

    protected override void AddFieldsToRoot()
    {
        throw new System.NotImplementedException();
    }

    protected override void BindFields()
    {
        throw new System.NotImplementedException();
    }

    protected override void DefineFields()
    {
        m_Columns = new PropertyField();
        m_Rows = new PropertyField();
        m_Height = new PropertyField();
        m_Width = new PropertyField();
    }

    protected override void Initialize(SerializedProperty data)
    {
        throw new System.NotImplementedException();
    }

    protected override void RegisterValueChangeCallbacks()
    {
        throw new System.NotImplementedException();
    }
}