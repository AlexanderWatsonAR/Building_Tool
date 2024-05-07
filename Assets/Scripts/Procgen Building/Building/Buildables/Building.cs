using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using OnlyInvalid.ProcGenBuilding.Common;
using OnlyInvalid.ProcGenBuilding.Storey;
using OnlyInvalid.ProcGenBuilding.Roof;
using OnlyInvalid.ProcGenBuilding.Wall;

namespace OnlyInvalid.ProcGenBuilding.Building
{
    [DisallowMultipleComponent]
    public class Building : Buildable, IPolygon
    {
        [SerializeField] BuildingScriptableObject m_DataAccessor;
        [SerializeField] BuildingData m_BuildingData;

        [SerializeField] List<Storey.Storey> m_Storeys;
        [SerializeField] Roof.Roof m_Roof;

        public BuildingScriptableObject DataAccessor => m_DataAccessor;
        public BuildingData BuildingData { get { return m_DataAccessor.Data; } set { m_BuildingData = value; m_DataAccessor.Data = value; } }
        public PlanarPath Path => BuildingData.Path;

        public override Buildable Initialize(DirtyData data)
        {
            base.Initialize(data);
            m_BuildingData = data as BuildingData;
            m_DataAccessor = BuildingScriptableObject.Create(m_BuildingData);
            return this;
        }

        public override void Build()
        {
            Demolish();

            if (!m_DataAccessor.Data.Path.IsPathValid)
                return;

            Vector3 pos = Vector3.zero;

            for (int i = 0; i < m_DataAccessor.Data.Storeys.Count; i++)
            {
                Storey.Storey storey = CreateStorey(m_DataAccessor.Data.Storeys[i]);
                storey.transform.SetParent(transform, false);
                storey.transform.localPosition = pos;
                storey.Data.IsDirty = true;
                storey.Build();

                StoreyData storeyData = storey.Data as StoreyData;
                WallData wallData = storeyData.WallData;
                pos += Vector3.up * wallData.Height;
            }

            GameObject roofGO = new GameObject("Roof");
            roofGO.transform.SetParent(transform, false);
            roofGO.transform.localPosition = pos;
            roofGO.AddComponent<Roof.Roof>().Initialize(m_DataAccessor.Data.Roof).Build();
        }

        private Storey.Storey CreateStorey(StoreyData data)
        {
            ProBuilderMesh proBuilderMesh = ProBuilderMesh.Create();
            proBuilderMesh.name = "Storey " + data.ID;
            Storey.Storey storey = proBuilderMesh.AddComponent<Storey.Storey>();
            storey.Initialize(data);
            return storey;
        }

        public void AddStorey(string name)
        {
            m_DataAccessor.Data.Storeys.Add(new StoreyData() { Name = name, ControlPoints = m_DataAccessor.Data.Path.ControlPoints });
        }

        public void InitializeRoof()
        {
            m_DataAccessor.Data.Roof.ControlPoints = m_DataAccessor.Data.Path.ControlPoints;
        }

        public void BuildStorey(int index)
        {
            m_Storeys[index].Build();
        }

        public void BuildStoreys()
        {
            m_Storeys.BuildCollection();
        }

        public override void Demolish()
        {
            transform.DeleteChildren();
        }

    }
}
