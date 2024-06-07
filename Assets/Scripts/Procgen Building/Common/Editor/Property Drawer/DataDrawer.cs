using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using OnlyInvalid.ProcGenBuilding.Common;

[CustomPropertyDrawer(typeof(DirtyData), false)]
public abstract class DataDrawer : PropertyDrawer
{
    protected VisualElement m_Root;
    protected SerializedProperty m_Data;

    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        m_Root = new VisualElement();
        m_Data = data;
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