using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

public class StoreyDataSerializedProperties : SerializedPropertyGroup
{
    readonly WallDataSerializedProperties m_Wall;
    readonly PillarDataSerializedProperties m_Pillar;
    readonly CornerDataSerializedProperties m_Corner;
    readonly FloorDataSerializedProperties m_Floor;

    readonly WallDataSerializedProperties[] m_Walls;
    readonly PillarDataSerializedProperties[] m_Pillars;
    readonly CornerDataSerializedProperties[] m_Corners;

    #region Constants
    const string k_ActiveElements = "m_ActiveElements";
    const string k_Wall = "m_Wall";
    const string k_Pillar = "m_Pillar";
    const string k_Corner = "m_Corner";
    const string k_Floor = "m_Floor";
    const string k_Walls = "m_Walls";
    const string k_Pillars = "m_Pillars";
    const string k_Corners = "m_Corners";
    const string k_Name = "m_Name";
    #endregion

    #region Accessors
    public SerializedProperty ActiveElements => m_Data.FindPropertyRelative(k_ActiveElements);
    public SerializedProperty Name => m_Data.FindPropertyRelative(k_Name);
    public WallDataSerializedProperties Wall => m_Wall;
    public PillarDataSerializedProperties Pillar => m_Pillar;
    public CornerDataSerializedProperties Corner => m_Corner;
    public FloorDataSerializedProperties Floor => m_Floor;
    public WallDataSerializedProperties[] Walls => m_Walls;
    public PillarDataSerializedProperties[] Pillars => m_Pillars;
    public CornerDataSerializedProperties[] Corners => m_Corners;
    #endregion

    public StoreyDataSerializedProperties(SerializedProperty storeyData) : base(storeyData)
    {
        m_Wall = new WallDataSerializedProperties(m_Data.FindPropertyRelative(k_Wall));
        m_Pillar = new PillarDataSerializedProperties(m_Data.FindPropertyRelative(k_Pillar));
        m_Corner = new CornerDataSerializedProperties(m_Data.FindPropertyRelative(k_Corner));
        m_Floor = new FloorDataSerializedProperties(m_Data.FindPropertyRelative(k_Floor));

        SerializedProperty walls = m_Data.FindPropertyRelative(k_Walls);
        m_Walls = new WallDataSerializedProperties[walls.arraySize];

        for(int i = 0; i < m_Walls.Length; i++)
        {
            m_Walls[i] =  new WallDataSerializedProperties(walls.GetArrayElementAtIndex(i));
        }

        SerializedProperty pillars = m_Data.FindPropertyRelative(k_Pillars);
        m_Pillars = new PillarDataSerializedProperties[pillars.arraySize];

        for (int i = 0; i < m_Pillars.Length; i++)
        {
            m_Pillars[i] = new PillarDataSerializedProperties(pillars.GetArrayElementAtIndex(i));
        }

        SerializedProperty corners = m_Data.FindPropertyRelative(k_Corners);
        m_Corners = new CornerDataSerializedProperties[corners.arraySize];

        for (int i = 0; i < m_Corners.Length; i++)
        {
            m_Corners[i] = new CornerDataSerializedProperties(corners.GetArrayElementAtIndex(i));
        }
    }

}
