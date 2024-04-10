using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Polygon3D;

public class DoorDataSerializedProperties : Polygon3DDataSerializedProperties
{
    readonly TransformDataSerializedProperties m_HingeData;

    #region Constants
    const string k_ActiveElements = "m_ActiveElements";
    const string k_HingeData = "m_HingeData";
    const string k_HandlePoint = "m_HandlePoint";
    const string k_HandlePosition = "m_HandlePosition";
    const string k_HandleScale = "m_HandleScale";
    #endregion

    #region Accessors
    public SerializedProperty ActiveElements => m_Data.FindPropertyRelative(k_ActiveElements);
    public TransformDataSerializedProperties Hinge => m_HingeData;
    public SerializedProperty HandlePoint => m_Data.FindPropertyRelative(k_HandlePoint);
    public SerializedProperty HandlePosition => m_Data.FindPropertyRelative(k_HandlePosition);
    public SerializedProperty HandleScale => m_Data.FindPropertyRelative(k_HandleScale);
    #endregion

    public DoorDataSerializedProperties(SerializedProperty doorData) : base(doorData)
    {
        m_HingeData = new TransformDataSerializedProperties(doorData.FindPropertyRelative(k_HingeData));

    }

}
