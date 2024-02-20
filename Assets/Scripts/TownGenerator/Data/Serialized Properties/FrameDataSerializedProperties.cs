using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// you may want to create a serialized prop class for polygon3d, then this could inherit from that.
public class FrameDataSerializedProperties : SerializedPropertyGroup
{
    #region Constants
    const string k_Depth = "m_Depth";
    const string k_Scale = "m_Scale";
    #endregion

    #region Accessors
    public SerializedProperty Depth => m_Data.FindPropertyRelative(k_Depth);
    public SerializedProperty Scale => m_Data.FindPropertyRelative(k_Scale);
    #endregion

    public FrameDataSerializedProperties(SerializedProperty data) : base (data)
    {

    }
}
