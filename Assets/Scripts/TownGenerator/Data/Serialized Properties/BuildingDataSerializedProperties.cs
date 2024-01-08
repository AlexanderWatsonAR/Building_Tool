using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildingDataSerializedProperties
{
    [SerializeField] private SerializedProperty m_Data;
    [SerializeField] private List<StoreyDataSerializedProperties> m_Storeys;
    [SerializeField] private RoofDataSerializedProperties m_Roof;

    public SerializedObject SerializedObject => m_Data.serializedObject;

    public SerializedProperty Data => m_Data;

    public List<StoreyDataSerializedProperties> Storeys => m_Storeys;
    public RoofDataSerializedProperties Roof => m_Roof;

    public BuildingDataSerializedProperties(SerializedProperty buildingData)
    {
        m_Data = buildingData;
        m_Roof = new RoofDataSerializedProperties(m_Data.FindPropertyRelative("m_Roof"));

        SerializedProperty storeys = m_Data.FindPropertyRelative("m_Storeys");

        m_Storeys = new List<StoreyDataSerializedProperties>(storeys.arraySize);

        for(int i = 0; i < storeys.arraySize; i++)
        {
            m_Storeys[i] = new StoreyDataSerializedProperties(storeys.GetArrayElementAtIndex(i));
        }
    }
}
