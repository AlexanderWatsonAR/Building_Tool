using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;

[ExecuteAlways]
[DisallowMultipleComponent]
public class Building : MonoBehaviour, IBuildable
{
    [SerializeField] BuildingData m_Data;

    [SerializeField] bool m_IsPolyPathHandleSelected;

    [SerializeField] List<Storey> m_Storeys;
    [SerializeField] Roof m_Roof;

    public bool IsPolyPathHandleSelected => m_IsPolyPathHandleSelected;
    public BuildingData Data => m_Data;

    private void Reset()
    {
        m_Data = new BuildingData();
    }
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
        if (m_Data == null)
            return;

        if (m_Data.Path == null)
            return;

        if (m_Data.Path.ControlPointCount == 0)
            return;

        if (m_Data.Path.PolyMode == PolyMode.Hide)
            return;

        if (GUIUtility.hotControl == 0 && m_IsPolyPathHandleSelected)
        {
            Rebuild();
        }

        m_IsPolyPathHandleSelected = GUIUtility.hotControl > 0 && GUIUtility.hotControl < m_Data.Path.ControlPointCount + 1 ? true : false;
    }

    public IBuildable Initialize(IData data)
    {
        m_Data = data as BuildingData;
        return this;
    }

    private void Rebuild()
    {
        m_Data.RoofData = new RoofData() { ControlPoints = m_Data.Path.ControlPoints.ToArray() };

        int count = m_Data.Storeys.Count;

        m_Data.Storeys = new List<StoreyData>(count);

        for(int i = 0; i < count; i++)
        {
            StoreyData storey = new StoreyData() { ControlPoints = m_Data.Path.ControlPoints.ToArray(), Name = "Storey " + i.ToString()};
            m_Data.Storeys.Add(storey);
        }

        Build();
    }

    public void Build()
    {
        Demolish();

        if (!m_Data.Path.IsPathValid)
            return;

        Vector3 pos = Vector3.zero;

        for (int i = 0; i < m_Data.Storeys.Count; i++)
        {
            Storey storey = CreateStorey(m_Data.Storeys[i]);
            storey.transform.SetParent(transform, false);
            storey.transform.localPosition = pos;
            storey.Build();
            pos += (Vector3.up * storey.Data.WallData.Height);
        }

        GameObject roofGO = new GameObject("Roof");
        roofGO.transform.SetParent(transform, false);
        roofGO.transform.localPosition = pos;
        roofGO.AddComponent<Roof>().Initialize(m_Data.RoofData).Build();
    }

    private Storey CreateStorey(StoreyData data)
    {
        ProBuilderMesh proBuilderMesh = ProBuilderMesh.Create();
        proBuilderMesh.name = "Storey " + data.ID;
        Storey storey = proBuilderMesh.AddComponent<Storey>();
        storey.Initialize(data);
        return storey;
    }

    public void BuildStorey(int index)
    {
        m_Storeys[index].Build();
    }

    public void BuildStoreys()
    {
        m_Storeys.BuildCollection();
    }

    public void Demolish()
    {
        transform.DeleteChildren();
    }

}
