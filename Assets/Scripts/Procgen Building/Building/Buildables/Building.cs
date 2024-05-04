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
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class Building : Buildable, IDrawable
    {
        [SerializeField] BuildingScriptableObject m_DataContainer;

        [SerializeField] bool m_IsPolyPathHandleSelected;

        [SerializeField] List<Storey.Storey> m_Storeys;
        [SerializeField] Roof.Roof m_Roof;

        //public bool IsPolyPathHandleSelected => m_IsPolyPathHandleSelected;
        public BuildingScriptableObject Container { get { return m_DataContainer; } set { m_DataContainer = value; } }
        public BuildingData BuildingData => m_DataContainer.Data;
        public PlanarPath Path => BuildingData.Path;

        public override Buildable Initialize(DirtyData data)
        {
            base.Initialize(data);
            m_DataContainer.Data = data as BuildingData;
            return this;
        }

        public override void Build()
        {
            Demolish();

            if (!m_DataContainer.Data.Path.IsPathValid)
                return;

            Vector3 pos = Vector3.zero;

            for (int i = 0; i < m_DataContainer.Data.Storeys.Count; i++)
            {
                Storey.Storey storey = CreateStorey(m_DataContainer.Data.Storeys[i]);
                storey.transform.SetParent(transform, false);
                storey.transform.localPosition = pos;
                storey.Data.IsDirty = true;
                storey.Build();

                StoreyData storeyData = storey.Data as StoreyData;
                WallData wallData = storeyData.WallData;
                pos += (Vector3.up * wallData.Height);
            }

            GameObject roofGO = new GameObject("Roof");
            roofGO.transform.SetParent(transform, false);
            roofGO.transform.localPosition = pos;
            roofGO.AddComponent<Roof.Roof>().Initialize(m_DataContainer.Data.Roof).Build();
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
            m_DataContainer.Data.Storeys.Add(new StoreyData() { Name = name, ControlPoints = m_DataContainer.Data.Path.ControlPoints });
        }

        public void InitializeRoof()
        {
            m_DataContainer.Data.Roof.ControlPoints = m_DataContainer.Data.Path.ControlPoints;
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
