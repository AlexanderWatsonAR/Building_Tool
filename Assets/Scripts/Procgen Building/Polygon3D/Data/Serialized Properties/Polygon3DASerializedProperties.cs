using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JetBrains.Annotations;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    public class Polygon3DASerializedProperties : SerializedPropertyGroup
    {
        #region Constants
        const string k_Position = "m_Position";
        const string k_EulerAngle = "m_EulerAngle";
        const string k_Scale = "m_Scale";
        const string k_ExteriorShape = "m_ExteriorShape";
        const string k_InteriorShapes = "m_InteriorShape";
        const string k_Depth = "m_Depth";
        #endregion

        #region Accessors
        public SerializedProperty Position => m_Data.FindPropertyRelative(k_Position);
        public SerializedProperty EulerAngle => m_Data.FindPropertyRelative(k_EulerAngle);
        public SerializedProperty Scale => m_Data.FindPropertyRelative(k_Scale);
        public SerializedProperty ExteriorShape => m_Data.FindPropertyRelative(k_ExteriorShape);
        public SerializedProperty InteriorShapes => m_Data.FindPropertyRelative(k_InteriorShapes);
        public SerializedProperty Depth => m_Data.FindPropertyRelative(k_Depth);
        #endregion

        public Polygon3DASerializedProperties(SerializedProperty data) : base(data)
        {
        }
    }

}
