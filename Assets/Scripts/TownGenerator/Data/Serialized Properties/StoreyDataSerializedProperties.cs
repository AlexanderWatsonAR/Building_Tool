using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

public class StoreyDataSerializedProperties : SerializedPropertyGroup
{
    [SerializeField] private WallDataSerializedProperties m_WallSerializedProperties;
    [SerializeField] private PillarDataSerializedProperties m_PillarSerializedProperties;
    [SerializeField] private CornerDataSerializedProperties m_CornerSerializedProperties;
    [SerializeField] private FloorDataSerializedProperties m_FloorSerializedProperties;

    #region Constants
    const string k_ActiveElements = "m_ActiveElements";
    const string k_Wall = "m_Wall";
    const string k_Pillar = "m_Pillar";
    const string k_Corner = "m_Corner";
    const string k_Floor = "m_Floor";
    #endregion

    #region Accessors
    public SerializedProperty ActiveElements => m_Data.FindPropertyRelative(k_ActiveElements);
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
