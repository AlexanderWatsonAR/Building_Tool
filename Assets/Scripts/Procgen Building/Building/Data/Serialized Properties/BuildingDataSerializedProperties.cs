using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Storey;
using OnlyInvalid.ProcGenBuilding.Roof;

namespace OnlyInvalid.ProcGenBuilding.Building
{
    public class BuildingDataSerializedProperties : SerializedPropertyGroup
    {
        readonly StoreysDataSerializedProperties m_Storeys;
        readonly RoofDataSerializedProperties m_Roof;

        #region Constants
        const string k_Roof = "m_Roof";
        const string k_Storeys = "m_Storeys";
        #endregion

        #region Accessor
        public StoreysDataSerializedProperties Storeys => m_Storeys;
        public RoofDataSerializedProperties Roof => m_Roof;
        #endregion

        public BuildingDataSerializedProperties(SerializedProperty buildingData) : base(buildingData)
        {
            m_Roof = new RoofDataSerializedProperties(m_Data.FindPropertyRelative(k_Roof));
            m_Storeys = new StoreysDataSerializedProperties(m_Data.FindPropertyRelative(k_Storeys));
        }
    }
}
