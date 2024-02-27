using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// This is for custom Property drawers & Inspectors
/// </summary>
public interface IFieldInitializer
{
    void Initialize(SerializedProperty data);
    void DefineFields();
    void BindFields();
    void RegisterValueChangeCallbacks();
    void AddFieldsToRoot();
}
