using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    public class FrameDataSerializedProperties : Polygon3DDataSerializedProperties
    {
        #region Constants
        const string k_Scale = "m_InsideScale";
        #endregion

        #region Accessors
        public SerializedProperty InsideScale => m_Data.FindPropertyRelative(k_Scale);
        #endregion

        public FrameDataSerializedProperties(SerializedProperty data) : base(data)
        {

        }
    }
}
