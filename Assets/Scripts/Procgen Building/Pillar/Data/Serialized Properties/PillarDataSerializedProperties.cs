using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OnlyInvalid.ProcGenBuilding.Pillar
{
    public class PillarDataSerializedProperties : Polygon3D.Polygon3DASerializedProperties
    {
        #region Constants
        const string k_Sides = "m_Sides";
        const string k_IsSmooth = "m_IsSmooth";
        #endregion

        #region Accessors
        public SerializedProperty Sides => ExteriorShape.FindPropertyRelative(k_Sides);
        public SerializedProperty IsSmooth => m_Data.FindPropertyRelative(k_IsSmooth);
        #endregion

        public PillarDataSerializedProperties(SerializedProperty pillarData) : base(pillarData)
        {

        }
    }
}
