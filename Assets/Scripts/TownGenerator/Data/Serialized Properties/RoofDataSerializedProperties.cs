using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoofDataSerializedProperties
{
    [SerializeField] private SerializedProperty m_RoofData;
    [SerializeField] private SerializedObject m_SerializedObject;
    [SerializeField] private RoofTileDataSerializedProperties m_RoofTileSerializedProperties;

    #region Accessors
    public SerializedProperty RoofData => m_RoofData;
    public SerializedObject SerializedObject => m_SerializedObject;
    public RoofTileDataSerializedProperties RoofTile => m_RoofTileSerializedProperties;
    public SerializedProperty Type => m_RoofData.FindPropertyRelative("m_RoofType");
    public SerializedProperty MansardHeight => m_RoofData.FindPropertyRelative("m_MansardHeight");
    public SerializedProperty MansardScale => m_RoofData.FindPropertyRelative("m_MansardScale");
    public SerializedProperty PyramidHeight => m_RoofData.FindPropertyRelative("m_PyramidHeight");
    public SerializedProperty GableHeight => m_RoofData.FindPropertyRelative("m_GableHeight");
    public SerializedProperty GableScale => m_RoofData.FindPropertyRelative("m_GableScale");
    public SerializedProperty IsOpen => m_RoofData.FindPropertyRelative("m_IsOpen");
    public SerializedProperty IsFlipped => m_RoofData.FindPropertyRelative("m_IsFlipped");
    #endregion

    public RoofDataSerializedProperties(SerializedProperty roofData)
    {
        m_RoofData = roofData;
        m_RoofTileSerializedProperties = new RoofTileDataSerializedProperties(m_RoofData.FindPropertyRelative("m_RoofTileData"));
        m_SerializedObject = m_RoofData.serializedObject;
    }

}
