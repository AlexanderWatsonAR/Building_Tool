using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WindowOpeningDataSerializedProperties : OpeningDataSerializedProperties
{
    public SerializedProperty Windows => m_OpeningData.FindPropertyRelative("m_Windows");
    public SerializedProperty Sides => m_OpeningData.FindPropertyRelative("m_Sides");
    public SerializedProperty Angle => m_OpeningData.FindPropertyRelative("m_Angle");

    public WindowOpeningDataSerializedProperties(SerializedProperty data) : base(data)
    {

    }

}
