using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OnlyInvalid.ProcGenBuilding.Wall
{
    public class WallsDataSerializedProperties : SerializedPropertyGroup
    {
        WallDataSerializedProperties m_Wall;
        WallDataSerializedProperties[] m_Walls;

        #region Constants
        const string k_Wall = "m_Wall";
        const string k_Walls = "m_Walls";
        #endregion

        #region Accessors
        public WallDataSerializedProperties Wall => m_Wall;
        public WallDataSerializedProperties[] Walls => m_Walls;

        #endregion

        public WallsDataSerializedProperties(SerializedProperty data) : base(data)
        {
            SerializedProperty walls = m_Data.FindPropertyRelative(k_Walls);

            m_Wall = new WallDataSerializedProperties(m_Data.FindPropertyRelative(k_Wall));
            m_Walls = new WallDataSerializedProperties[walls.arraySize];

            for (int i = 0; i < m_Walls.Length; i++)
            {
                m_Walls[i] = new WallDataSerializedProperties(walls.GetArrayElementAtIndex(i));
            }

        }
    }
}
