using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class ExtensionDataSerializedProperties : OpeningDataSerializedProperties
{
    const string k_Distance = "m_Distance";

    public SerializedProperty Distance => m_Data.FindPropertyRelative(k_Distance);

    public ExtensionDataSerializedProperties(SerializedProperty extensionData) : base(extensionData)
    {

    }
}
