using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildingSerializedProperties
{
    [SerializeField, HideInInspector] private SerializedObject m_SerializedObject;

    public SerializedObject SerializedObject => m_SerializedObject;

    public SerializedProperty Data => m_SerializedObject.FindProperty("m_Data");

    public SerializedProperty Storeys => Data.FindPropertyRelative("m_Storeys");

    public BuildingSerializedProperties(Building building)
    {
        m_SerializedObject = new SerializedObject(building);
    }

    public BuildingSerializedProperties(Building context, Storey obj)
    {
        m_SerializedObject = new SerializedObject(context, obj);
    }
}
