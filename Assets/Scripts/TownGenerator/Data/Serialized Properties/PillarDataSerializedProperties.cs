using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PillarDataSerializedProperties
{
    [SerializeField] private SerializedProperty m_PillarData;
    [SerializeField] private SerializedObject m_SerializedObject;

    #region Accessors
    public SerializedObject SerializedObject => m_SerializedObject;
    public SerializedProperty Height => m_PillarData.FindPropertyRelative("m_Height");
    public SerializedProperty Width => m_PillarData.FindPropertyRelative("m_Width");
    public SerializedProperty Depth => m_PillarData.FindPropertyRelative("m_Depth");
    public SerializedProperty Sides => m_PillarData.FindPropertyRelative("m_Sides");
    public SerializedProperty IsSmooth => m_PillarData.FindPropertyRelative("m_IsSmooth");
    #endregion

    public PillarDataSerializedProperties(SerializedProperty pillarData)
    {
        m_PillarData = pillarData;
        m_SerializedObject = pillarData.serializedObject;
    }
}
