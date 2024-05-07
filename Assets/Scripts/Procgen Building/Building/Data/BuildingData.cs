using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Storey;
using OnlyInvalid.ProcGenBuilding.Common;
using OnlyInvalid.ProcGenBuilding.Roof;

namespace OnlyInvalid.ProcGenBuilding.Building
{
    [System.Serializable]
    public class BuildingData : DirtyData
    {
        [SerializeField] XZPolygonPath m_Path;
        [SerializeField] List<StoreyData> m_Storeys;
        [SerializeField] RoofData m_Roof;

        public XZPolygonPath Path => m_Path;
        public List<StoreyData> Storeys { get { return m_Storeys; } set { m_Storeys = value; } }
        public RoofData Roof { get { return m_Roof; } set { m_Roof = value; } }

        public BuildingData() : this(new XZPolygonPath(), new List<StoreyData>(), new RoofData())
        {

        }
        public BuildingData(BuildingData data) : this(data.Path, data.Storeys, data.Roof)
        {

        }
        public BuildingData(XZPolygonPath path, List<StoreyData> storeysData, RoofData roofData)
        {
            m_Path = path;
            m_Storeys = storeysData;
            m_Roof = roofData;
        }
    }
}
