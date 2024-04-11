using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OnlyInvalid.ProcGenBuilding.Roof
{
    public class RoofDataSerializedProperties : SerializedPropertyGroup
    {
        [SerializeField] RoofTileDataSerializedProperties m_RoofTileSerializedProperties;

        #region Constants
        const string k_RoofTileData = "m_RoofTileData";
        const string k_RoofType = "m_RoofType";
        const string k_MansardHeight = "m_MansardHeight";
        const string k_MansardScale = "m_MansardScale";
        const string k_PyramidHeight = "m_PyramidHeight";
        const string k_GableHeight = "m_GableHeight";
        const string k_GableScale = "m_GableScale";
        const string k_IsOpen = "m_IsOpen";
        const string k_IsFlipped = "m_RoofType";
        #endregion

        #region Accessors
        public RoofTileDataSerializedProperties RoofTile => m_RoofTileSerializedProperties;
        public SerializedProperty Type => m_Data.FindPropertyRelative(k_RoofType);
        public SerializedProperty MansardHeight => m_Data.FindPropertyRelative(k_MansardHeight);
        public SerializedProperty MansardScale => m_Data.FindPropertyRelative(k_MansardScale);
        public SerializedProperty PyramidHeight => m_Data.FindPropertyRelative(k_PyramidHeight);
        public SerializedProperty GableHeight => m_Data.FindPropertyRelative(k_GableHeight);
        public SerializedProperty GableScale => m_Data.FindPropertyRelative(k_GableScale);
        public SerializedProperty IsOpen => m_Data.FindPropertyRelative(k_IsOpen);
        public SerializedProperty IsFlipped => m_Data.FindPropertyRelative(k_IsFlipped);
        #endregion

        public RoofDataSerializedProperties(SerializedProperty roofData) : base(roofData)
        {
            m_RoofTileSerializedProperties = new RoofTileDataSerializedProperties(m_Data.FindPropertyRelative(k_RoofTileData));
        }
    }
}
