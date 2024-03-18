using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SerializedPropertyGroup
{
    [SerializeField] protected SerializedProperty m_Data;

    const string k_ID = "m_ID";

    public SerializedProperty Data => m_Data;
    public SerializedObject SerializedObject => m_Data.serializedObject;
    public SerializedProperty ID => m_Data.FindPropertyRelative(k_ID);

    public SerializedPropertyGroup(SerializedProperty data)
    {
        m_Data = data;
    }

}
