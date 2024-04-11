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
    public class Building : Buildable
    {
        [SerializeField] BuildingScriptableObject m_Container;
        [SerializeField] BuildingData m_BuildingData;

        [SerializeField] bool m_IsPolyPathHandleSelected;

        [SerializeField] List<Storey.Storey> m_Storeys;
        [SerializeField] Roof.Roof m_Roof;

        public bool IsPolyPathHandleSelected => m_IsPolyPathHandleSelected;
        public BuildingScriptableObject Container { get { return m_Container; } set { m_Container = value; } }

        private void OnEnable()
        {
            UnityEditor.EditorApplication.update = Update;
        }
        private void OnDisable()
        {
            UnityEditor.EditorApplication.update = null;
        }
        private void Update()
        {
            if (m_BuildingData == null)
                return;

            if (m_BuildingData.Path == null)
                return;

            if (m_BuildingData.Path.ControlPointCount == 0)
                return;

            if (m_BuildingData.Path.PolyMode == PolyMode.Hide)
                return;

            if (GUIUtility.hotControl == 0 && m_IsPolyPathHandleSelected)
            {
                Rebuild();
            }

            m_IsPolyPathHandleSelected = GUIUtility.hotControl > 0 && GUIUtility.hotControl < m_BuildingData.Path.ControlPointCount + 1 ? true : false;
        }

        public override Buildable Initialize(DirtyData data)
        {
            base.Initialize(data);
            m_BuildingData = data as BuildingData;

            return this;
        }

        private void Rebuild()
        {
            m_BuildingData.Roof = new RoofData() { ControlPoints = m_BuildingData.Path.ControlPoints.ToArray() };

            int count = m_BuildingData.Storeys.Count;

            m_BuildingData.Storeys = new List<StoreyData>(count);

            for (int i = 0; i < count; i++)
            {
                StoreyData storey = new StoreyData() { ControlPoints = m_BuildingData.Path.ControlPoints.ToArray(), Name = "Storey " + i.ToString() };
                m_BuildingData.Storeys.Add(storey);
            }

            Build();
        }

        public override void Build()
        {
            Demolish();

            if (!m_BuildingData.Path.IsPathValid)
                return;

            Vector3 pos = Vector3.zero;

            for (int i = 0; i < m_BuildingData.Storeys.Count; i++)
            {
                Storey.Storey storey = CreateStorey(m_BuildingData.Storeys[i]);
                storey.transform.SetParent(transform, false);
                storey.transform.localPosition = pos;
                storey.Build();

                StoreyData storeyData = storey.Data as StoreyData;
                WallData wallData = storeyData.WallData;
                pos += (Vector3.up * wallData.Height);
            }

            GameObject roofGO = new GameObject("Roof");
            roofGO.transform.SetParent(transform, false);
            roofGO.transform.localPosition = pos;
            roofGO.AddComponent<Roof.Roof>().Initialize(m_BuildingData.Roof).Build();
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
            m_BuildingData.Storeys.Add(new StoreyData() { Name = name, ControlPoints = m_BuildingData.Path.ControlPoints.ToArray() });
        }

        public void InitializeRoof()
        {
            m_BuildingData.Roof.ControlPoints = m_BuildingData.Path.ControlPoints.ToArray();
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
