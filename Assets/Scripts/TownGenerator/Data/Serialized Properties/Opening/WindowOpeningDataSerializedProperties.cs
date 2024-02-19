using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WindowOpeningDataSerializedProperties : OpeningDataSerializedProperties
{
    #region Constants
    const string k_Windows = "m_Windows";
    const string k_Sides = "m_Sides";
    const string k_Angle = "m_Angle";
    #endregion

    public SerializedProperty Windows => m_Data.FindPropertyRelative(k_Windows);
    public SerializedProperty Sides => m_Data.FindPropertyRelative(k_Sides);
    public SerializedProperty Angle => m_Data.FindPropertyRelative(k_Angle);

    public WindowOpeningDataSerializedProperties(SerializedProperty windowOpeningData) : base(windowOpeningData)
    {

    }

}
