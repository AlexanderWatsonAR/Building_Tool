using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WindowSerializedProperties
{
    [SerializeField] SerializedObject m_SerializedObject;
    [SerializeField] SerializedProperty m_Data;

    public SerializedProperty ActiveElements { get { return m_Data.FindPropertyRelative("m_ActiveElements"); } }
    public SerializedProperty InnerFrameColumns { get { return m_Data.FindPropertyRelative("m_InnerFrameColumns"); } }
    public SerializedProperty InnerFrameRows { get { return m_Data.FindPropertyRelative("m_InnerFrameRows"); } }
    public SerializedProperty InnerFrameScale { get { return m_Data.FindPropertyRelative("m_InnerFrameScale"); } }
    public SerializedProperty InnerFrameDepth { get { return m_Data.FindPropertyRelative("m_InnerFrameDepth"); } }
    public SerializedProperty OuterFrameScale { get { return m_Data.FindPropertyRelative("m_OuterFrameScale"); } }
    public SerializedProperty OuterFrameDepth { get { return m_Data.FindPropertyRelative("m_OuterFrameDepth"); } }
    public SerializedProperty PaneDepth { get { return m_Data.FindPropertyRelative("m_PaneDepth"); } }
    public SerializedProperty ShuttersDepth { get { return m_Data.FindPropertyRelative("m_ShuttersDepth"); } }
    public SerializedProperty ShuttersAngle { get { return m_Data.FindPropertyRelative("m_ShuttersAngle"); } }

    public WindowSerializedProperties(Window window)
    {
        m_SerializedObject = new SerializedObject(window);
        m_Data = m_SerializedObject.FindProperty("m_Data");
    }

    public WindowSerializedProperties(SerializedObject serializedObject, SerializedProperty windowData)
    {
        m_SerializedObject = serializedObject;
        m_Data = windowData;
    }
}
