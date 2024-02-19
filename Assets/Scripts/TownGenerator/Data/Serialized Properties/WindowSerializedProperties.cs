using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WindowDataSerializedProperties : SerializedPropertyGroup
{
    #region Constants
    const string k_ActiveElements = "m_ActiveElements";
    const string k_InnerFrameColumns = "m_InnerFrameColumns";
    const string k_InnerFrameRows = "m_InnerFrameRows";
    const string k_InnerFrameScale = "m_InnerFrameScale";
    const string k_InnerFrameDepth = "m_InnerFrameDepth";
    const string k_OuterFrameScale = "m_OuterFrameScale";
    const string k_OuterFrameDepth = "m_OuterFrameDepth";
    const string k_PaneDepth = "m_PaneDepth";
    const string k_ShuttersDepth = "m_ShuttersDepth";
    const string k_ShuttersAngle = "m_ShuttersAngle";
    #endregion

    #region Accessors
    public SerializedProperty ActiveElements => m_Data.FindPropertyRelative(k_ActiveElements);
    public SerializedProperty InnerFrameColumns => m_Data.FindPropertyRelative(k_InnerFrameColumns);
    public SerializedProperty InnerFrameRows => m_Data.FindPropertyRelative(k_InnerFrameRows);
    public SerializedProperty InnerFrameScale => m_Data.FindPropertyRelative(k_InnerFrameScale);
    public SerializedProperty InnerFrameDepth => m_Data.FindPropertyRelative(k_InnerFrameDepth);
    public SerializedProperty OuterFrameScale => m_Data.FindPropertyRelative(k_OuterFrameScale); 
    public SerializedProperty OuterFrameDepth => m_Data.FindPropertyRelative(k_OuterFrameDepth);
    public SerializedProperty PaneDepth => m_Data.FindPropertyRelative(k_PaneDepth);
    public SerializedProperty ShuttersDepth => m_Data.FindPropertyRelative(k_ShuttersDepth);
    public SerializedProperty ShuttersAngle => m_Data.FindPropertyRelative(k_ShuttersAngle);
    #endregion

    public WindowDataSerializedProperties(SerializedProperty windowData) : base(windowData)
    {
    }
}
