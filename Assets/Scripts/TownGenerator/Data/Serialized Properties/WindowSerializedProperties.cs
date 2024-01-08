using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WindowDataSerializedProperties
{
    [SerializeField] SerializedObject m_SerializedObject;
    [SerializeField] SerializedProperty m_Data;

    public SerializedProperty Data => m_Data;
    public SerializedObject SerializedObject => m_SerializedObject;
    public SerializedProperty ActiveElements => m_Data.FindPropertyRelative("m_ActiveElements");
    public SerializedProperty InnerFrameColumns => m_Data.FindPropertyRelative("m_InnerFrameColumns");
    public SerializedProperty InnerFrameRows => m_Data.FindPropertyRelative("m_InnerFrameRows");
    public SerializedProperty InnerFrameScale => m_Data.FindPropertyRelative("m_InnerFrameScale");
    public SerializedProperty InnerFrameDepth => m_Data.FindPropertyRelative("m_InnerFrameDepth");
    public SerializedProperty OuterFrameScale => m_Data.FindPropertyRelative("m_OuterFrameScale"); 
    public SerializedProperty OuterFrameDepth => m_Data.FindPropertyRelative("m_OuterFrameDepth");
    public SerializedProperty PaneDepth => m_Data.FindPropertyRelative("m_PaneDepth");
    public SerializedProperty ShuttersDepth => m_Data.FindPropertyRelative("m_ShuttersDepth");
    public SerializedProperty ShuttersAngle => m_Data.FindPropertyRelative("m_ShuttersAngle");

    public WindowDataSerializedProperties(SerializedProperty windowData)
    {
        m_Data = windowData;
        m_SerializedObject = m_Data.serializedObject;
    }
}
