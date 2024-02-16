using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class ExtensionDataSerializedProperties : OpeningDataSerializedProperties
{

    public SerializedProperty Distance => m_OpeningData.FindPropertyRelative("m_Distance");

    public ExtensionDataSerializedProperties(SerializedProperty data) : base(data)
    {

    }
}
