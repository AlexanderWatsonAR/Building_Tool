using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OnlyInvalid.ProcGenBuilding.Common
{

    public class OpeningDataSerializedProperties : SerializedPropertyGroup
    {
        #region Constants
        //const string k_Columns = "m_Columns";
        //const string k_Rows = "m_Rows";
        const string k_Height = "m_Height";
        const string k_Width = "m_Width";
        const string k_Angle = "m_Angle";
        const string k_Position = "m_Position";
        const string k_Shape = "m_Shape";
        #endregion

        //public SerializedProperty Columns => m_Data.FindPropertyRelative(k_Columns);
        //public SerializedProperty Rows => m_Data.FindPropertyRelative(k_Rows);
        public SerializedProperty Height => m_Data.FindPropertyRelative(k_Height);
        public SerializedProperty Width => m_Data.FindPropertyRelative(k_Width);
        public SerializedProperty Angle => m_Data.FindPropertyRelative(k_Angle);
        public SerializedProperty Position => m_Data.FindPropertyRelative(k_Position);
        public SerializedProperty Shape => m_Data.FindPropertyRelative(k_Shape);


        public OpeningDataSerializedProperties(SerializedProperty openingData) : base(openingData)
        {
        }
    }
}
