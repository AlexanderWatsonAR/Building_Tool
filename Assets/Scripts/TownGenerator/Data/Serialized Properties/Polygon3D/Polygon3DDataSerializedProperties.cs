using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Polygon3DDataSerializedProperties : SerializedPropertyGroup
{
    #region Constants
    const string k_Depth = "m_Depth";
    #endregion

    #region Accessors
    public SerializedProperty Depth => m_Data.FindPropertyRelative(k_Depth);
    #endregion

    public Polygon3DDataSerializedProperties(SerializedProperty data) : base(data)
    {

    }

}
