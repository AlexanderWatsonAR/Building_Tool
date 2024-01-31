using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
[DisallowMultipleComponent]
public class Building : MonoBehaviour, IBuildable
{
    [SerializeField] private BuildingData m_Data;

    [SerializeField] private bool m_IsPolyPathHandleSelected;
    public bool IsPolyPathHandleSelected => m_IsPolyPathHandleSelected;

    public BuildingData Data => m_Data;

    private void Reset()
    {
        m_Data = new BuildingData();
    }
    private void OnEnable()
    {
        // Calling build is a quick way of maintaining the object reference after serialization.
        if(m_Data != null)
            Build();
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

        int count = m_Data.StoreysData.Count;

        m_Data.StoreysData = new List<StoreyData>(count);

        for(int i = 0; i < count; i++)
        {
            StoreyData storey = new StoreyData() { ControlPoints = m_Data.Path.ControlPoints.ToArray(), Name = "Storey " + i.ToString()};
            m_Data.StoreysData.Add(storey);
        }

        Build();
    }

    public void Build()
    {
        transform.DeleteChildren();
        if (!m_Data.Path.IsPathValid/* || !m_HasInitialized*/)
            return;

        Vector3 pos = Vector3.zero;

        for (int i = 0; i < m_Data.StoreysData.Count; i++)
        {
            GameObject next = new GameObject("Storey " + i.ToString());
            next.transform.SetParent(transform, false);
            next.transform.localPosition = pos;
            Storey storey = next.AddComponent<Storey>().Initialize(m_Data.StoreysData[i]) as Storey;
            storey.Build();
            pos += (Vector3.up * storey.Data.WallData.Height);
        }

        GameObject roofGO = new GameObject("Roof");
        roofGO.transform.SetParent(transform, false);
        roofGO.transform.localPosition = pos;
        roofGO.AddComponent<Roof>().Initialize(m_Data.RoofData).Build();
    }

    public void Demolish()
    {

    }

    public void RevertBuilding()
    {
        transform.DeleteChildren();
    }

}
