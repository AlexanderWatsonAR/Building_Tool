using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace OnlyInvalid.ProcGenBuilding.Corner
{
    public class CornersDataSerializedProperties : SerializedPropertyGroup
    {
        CornerDataSerializedProperties[] m_Corners;
        CornerDataSerializedProperties m_Corner;

        #region Constants
        const string k_Corner = "m_Corner";
        const string k_Corners = "m_Corners";
        #endregion

        #region Accessors
        public CornerDataSerializedProperties[] Corners => m_Corners;
        public CornerDataSerializedProperties Corner => m_Corner;
        #endregion

        public CornersDataSerializedProperties(SerializedProperty data) : base(data)
        {
            m_Corner = new CornerDataSerializedProperties(data.FindPropertyRelative(k_Corner));

            SerializedProperty corners = data.FindPropertyRelative(k_Corners);

            m_Corners = new CornerDataSerializedProperties[corners.arraySize];

            for (int i = 0; i < m_Corners.Length; i++)
            {
                m_Corners[i] = new CornerDataSerializedProperties(corners.GetArrayElementAtIndex(i));
            }
        }
    }
}
