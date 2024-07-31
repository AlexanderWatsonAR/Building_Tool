using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    public class PolygonDataSerializedProperties : SerializedPropertyGroup
    {
        #region Constants
        const string k_ControlPoints = "m_ControlPoints";
        const string k_Scale = "m_Scale";
        const string k_Normal = "m_Normal";
        #endregion

        #region Accessors
        public SerializedProperty ControlPoints => m_Data.FindPropertyRelative(k_ControlPoints);
        public SerializedProperty Scale => m_Data.FindPropertyRelative(k_Scale);
        public SerializedProperty Normal => m_Data.FindPropertyRelative(k_Normal);
        #endregion

        public PolygonDataSerializedProperties(SerializedProperty data) : base(data)
        {

        }

    }
}
