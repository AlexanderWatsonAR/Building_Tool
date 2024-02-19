using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SerializedPropertyGroup
{
    [SerializeField] protected SerializedProperty m_Data;

    public SerializedProperty Data => m_Data;
    public SerializedObject SerializedObject => m_Data.serializedObject;

    public SerializedPropertyGroup(SerializedProperty data)
    {
        m_Data = data;
    }

}
