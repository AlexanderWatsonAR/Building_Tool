using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Polygon3DDataSerializedProperties : SerializedPropertyGroup
{
    readonly PolygonDataSerializedProperties m_Polygon;
    readonly PolygonDataSerializedProperties[] m_Holes;

    #region Constants
    const string k_Polygon = "m_Polygon";
    const string k_Holes = "m_Holes";
    const string k_Normal = "m_Normal";
    const string k_Up = "m_Up";
    const string k_Height = "m_Height";
    const string k_Width = "m_Width";
    const string k_Depth = "m_Depth";
    const string k_Position = "m_Position";
    #endregion

    #region Accessors
    public PolygonDataSerializedProperties Polygon => m_Polygon;
    public PolygonDataSerializedProperties[] Holes => m_Holes;
    public SerializedProperty Normal => m_Data.FindPropertyRelative(k_Normal);
    public SerializedProperty Up => m_Data.FindPropertyRelative(k_Up);
    public SerializedProperty Height => m_Data.FindPropertyRelative(k_Height);
    public SerializedProperty Width => m_Data.FindPropertyRelative(k_Width);
    public SerializedProperty Depth => m_Data.FindPropertyRelative(k_Depth);
    public SerializedProperty Position => m_Data.FindPropertyRelative(k_Position);
    #endregion

    public Polygon3DDataSerializedProperties(SerializedProperty data) : base(data)
    {
        m_Polygon = new PolygonDataSerializedProperties(m_Data.FindPropertyRelative(k_Polygon));

        SerializedProperty holes = data.FindPropertyRelative(k_Holes);

        m_Holes = new PolygonDataSerializedProperties[holes.arraySize];

        for(int i = 0; i < m_Holes.Length; i++)
        {
            m_Holes[i] = new PolygonDataSerializedProperties(holes.GetArrayElementAtIndex(i));
        }
    }

}
