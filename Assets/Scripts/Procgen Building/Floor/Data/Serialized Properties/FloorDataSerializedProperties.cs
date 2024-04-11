using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OnlyInvalid.ProcGenBuilding.Floor
{
    public class FloorDataSerializedProperties : SerializedPropertyGroup
    {
        #region Constants
        const string k_Height = "m_Height";
        #endregion

        #region Accessors
        public SerializedProperty Height => m_Data.FindPropertyRelative(k_Height);
        #endregion

        public FloorDataSerializedProperties(SerializedProperty floorData) : base(floorData)
        {
        }
    }
}
