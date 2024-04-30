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
        [SerializeField] List<StoreyDataSerializedProperties> m_Storeys;
        [SerializeField] RoofDataSerializedProperties m_Roof;

        #region Constants
        const string k_Roof = "m_Roof";
        const string k_Storeys = "m_Storeys";
        #endregion

        #region Accessor
        public List<StoreyDataSerializedProperties> Storeys => m_Storeys;
        public RoofDataSerializedProperties Roof => m_Roof;
        #endregion

        public BuildingDataSerializedProperties(SerializedProperty buildingData) : base(buildingData)
        {

            m_Roof = new RoofDataSerializedProperties(m_Data.FindPropertyRelative(k_Roof));

            SerializedProperty storeys = m_Data.FindPropertyRelative(k_Storeys);

            m_Storeys = new List<StoreyDataSerializedProperties>(storeys.arraySize);

            for (int i = 0; i < storeys.arraySize; i++)
            {
                m_Storeys.Add(new StoreyDataSerializedProperties(storeys.GetArrayElementAtIndex(i)));
            }
        }
    }
}
