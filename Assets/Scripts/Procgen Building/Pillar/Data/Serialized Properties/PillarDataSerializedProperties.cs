using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OnlyInvalid.ProcGenBuilding.Pillar
{
    public class PillarDataSerializedProperties : Polygon3D.Polygon3DDataSerializedProperties
    {
        #region Constants
        const string k_Sides = "m_Sides";
        const string k_IsSmooth = "m_IsSmooth";
        const string k_Scale = "m_Scale";
        #endregion

        #region Accessors
        public SerializedProperty Sides => m_Data.FindPropertyRelative(k_Sides);
        public SerializedProperty IsSmooth => m_Data.FindPropertyRelative(k_IsSmooth);
        public SerializedProperty Scale => m_Data.FindPropertyRelative(k_Scale);
        #endregion

        public PillarDataSerializedProperties(SerializedProperty pillarData) : base(pillarData)
        {

        }
    }
}
