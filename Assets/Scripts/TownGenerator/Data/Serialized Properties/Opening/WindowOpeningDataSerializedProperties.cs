using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WindowOpeningDataSerializedProperties : OpeningDataSerializedProperties
{
    readonly WindowDataSerializedProperties[] m_Windows;

    #region Constants
    const string k_Windows = "m_Windows";
    const string k_Sides = "m_Sides";
    const string k_Angle = "m_Angle";
    #endregion

    public WindowDataSerializedProperties[] Windows => m_Windows;
    public SerializedProperty Sides => m_Data.FindPropertyRelative(k_Sides);
    public SerializedProperty Angle => m_Data.FindPropertyRelative(k_Angle);

    public WindowOpeningDataSerializedProperties(SerializedProperty windowOpeningData) : base(windowOpeningData)
    {
        SerializedProperty windows = m_Data.FindPropertyRelative(k_Windows);
        m_Windows = new WindowDataSerializedProperties[windows.arraySize];

        for(int i = 0; i < m_Windows.Length; i++)
        {
            m_Windows[i] = new WindowDataSerializedProperties(windows.GetArrayElementAtIndex(i));
        }
    }

}
