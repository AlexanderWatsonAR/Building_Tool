using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ArchwayDataSerializedProperties : DoorwayDataSerializedProperties
{
    #region Constants
    const string k_ArchHeight = "m_ArchHeight";
    const string k_ArchSides = "m_ArchSides";
    #endregion

    #region Accessors
    public SerializedProperty ArchHeight => m_Data.FindPropertyRelative(k_ArchHeight);
    public SerializedProperty ArchSides => m_Data.FindPropertyRelative(k_ArchSides);
    #endregion

    public ArchwayDataSerializedProperties(SerializedProperty archwayData) : base(archwayData)
    {

    }
}
