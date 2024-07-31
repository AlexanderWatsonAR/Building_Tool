using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SectionDataSerializedProperties : SerializedPropertyGroup
{
    const string k_Openings = "m_Openings";

    public SerializedProperty Openings => m_Data.FindPropertyRelative(k_Openings);

    public SectionDataSerializedProperties(SerializedProperty data) : base(data)
    {
    }
}
