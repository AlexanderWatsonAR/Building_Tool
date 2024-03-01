using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

public class StoreyDataSerializedProperties : SerializedPropertyGroup
{
    [SerializeField] WallDataSerializedProperties m_WallSerializedProperties;
    [SerializeField] PillarDataSerializedProperties m_PillarSerializedProperties;
    [SerializeField] CornerDataSerializedProperties m_CornerSerializedProperties;
    [SerializeField] FloorDataSerializedProperties m_FloorSerializedProperties;

    #region Constants
    const string k_ActiveElements = "m_ActiveElements";
    const string k_Wall = "m_Wall";
    const string k_Pillar = "m_Pillar";
    const string k_Corner = "m_Corner";
    const string k_Floor = "m_Floor";
    const string k_ID = "m_ID";
    const string k_Name = "m_Name";
    #endregion

    #region Accessors
    public SerializedProperty ID => m_Data.FindPropertyRelative(k_ID);
    public SerializedProperty ActiveElements => m_Data.FindPropertyRelative(k_ActiveElements);
    public SerializedProperty Name => m_Data.FindPropertyRelative(k_Name);
    public WallDataSerializedProperties Wall => m_WallSerializedProperties;
    public PillarDataSerializedProperties Pillar => m_PillarSerializedProperties;
    public CornerDataSerializedProperties Corner => m_CornerSerializedProperties;
    public FloorDataSerializedProperties Floor => m_FloorSerializedProperties;
    #endregion

    public StoreyDataSerializedProperties(SerializedProperty storeyData) : base(storeyData)
    {
        m_WallSerializedProperties = new WallDataSerializedProperties(m_Data.FindPropertyRelative(k_Wall));
        m_PillarSerializedProperties = new PillarDataSerializedProperties(m_Data.FindPropertyRelative(k_Pillar));
        m_CornerSerializedProperties = new CornerDataSerializedProperties(m_Data.FindPropertyRelative(k_Corner));
        m_FloorSerializedProperties = new FloorDataSerializedProperties(m_Data.FindPropertyRelative(k_Floor));
    }

}
