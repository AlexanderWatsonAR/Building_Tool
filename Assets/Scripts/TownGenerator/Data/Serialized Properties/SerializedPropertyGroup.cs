using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SerializedPropertyGroup
{
    [SerializeField] protected SerializedProperty m_Data;

    const string k_ID = "m_ID";
    const string k_IsDirty = "m_IsDirty";

    public SerializedProperty Data => m_Data;
    public SerializedObject SerializedObject => m_Data.serializedObject;
    public SerializedProperty ID => m_Data.FindPropertyRelative(k_ID);
    public SerializedProperty IsDirty => m_Data.FindPropertyRelative(k_IsDirty);

    public SerializedPropertyGroup(SerializedProperty data)
    {
        m_Data = data;
    }

}
