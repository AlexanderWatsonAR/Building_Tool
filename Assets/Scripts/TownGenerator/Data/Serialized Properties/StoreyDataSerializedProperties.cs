using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

public class StoreyDataSerializedProperties
{
    [SerializeField] private SerializedProperty m_StoreyData;
    [SerializeField] private SerializedObject m_SerializedObject;
    [SerializeField] private WallDataSerializedProperties m_WallSerializedProperties;
    [SerializeField] private PillarDataSerializedProperties m_PillarSerializedProperties;
    [SerializeField] private CornerDataSerializedProperties m_CornerSerializedProperties;
    [SerializeField] private FloorDataSerializedProperties m_FloorSerializedProperties;

    #region Accessors
    public SerializedObject SerializedObject => m_SerializedObject;
    public SerializedProperty ActiveElements => m_StoreyData.FindPropertyRelative("m_ActiveElements");
    public WallDataSerializedProperties Wall => m_WallSerializedProperties;
    public PillarDataSerializedProperties Pillar => m_PillarSerializedProperties;
    public CornerDataSerializedProperties Corner => m_CornerSerializedProperties;
    public FloorDataSerializedProperties Floor => m_FloorSerializedProperties;
    #endregion

    public StoreyDataSerializedProperties(SerializedProperty storeyData)
    {
        m_StoreyData = storeyData;
        m_SerializedObject = m_StoreyData.serializedObject;
        m_WallSerializedProperties = new WallDataSerializedProperties(m_StoreyData.FindPropertyRelative("m_Wall"));
        m_PillarSerializedProperties = new PillarDataSerializedProperties(m_StoreyData.FindPropertyRelative("m_Pillar"));
        m_CornerSerializedProperties = new CornerDataSerializedProperties(m_StoreyData.FindPropertyRelative("m_Corner"));
        m_FloorSerializedProperties = new FloorDataSerializedProperties(m_StoreyData.FindPropertyRelative("m_Floor"));
    }

}
