using AutoLayout3D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using ProMaths = UnityEngine.ProBuilder.Math;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(Storey), typeof(Roof))]
public class Building : MonoBehaviour, IBuildable
{
    [SerializeField] private BuildingData m_Data;

    [SerializeField, HideInInspector] ProBuilderMesh m_ProBuilderMesh;
    [SerializeField] private PolyPath m_BuildingPolyPath;
    [SerializeField] private Storey m_Storey;
    [SerializeField] private Roof m_Roof;
    private bool m_HasInitialized;
    [SerializeField] private MaterialPalette m_Palette;

    [SerializeField] private bool m_IsPolyPathHandleSelected;
    [SerializeField] private Vector3 m_PivotPosition;
    public bool IsPolyPathHandleSelected => m_IsPolyPathHandleSelected;

    public BuildingData Data => m_Data;

    public ControlPoint[] ControlPoints => m_BuildingPolyPath.ControlPoints.ToArray();

    public ControlPoint[] BuildingControlPoints
    {
        get
        {
            ControlPoint[] controlPoints = new ControlPoint[ControlPoints.Length];
            Array.Copy(ControlPoints, controlPoints, controlPoints.Length);

            Vector3 centre = ProMaths.Average(ControlPoints.GetPositions());

            for (int i = 0; i < controlPoints.Length; i++)
            {
                controlPoints[i] = controlPoints[i] - centre;
            }

            return controlPoints;
        }
    }

    public PolyPath PolyPath => m_BuildingPolyPath ?? new PolyPath();

    public bool HasInitialized => m_HasInitialized;

    private void Reset()
    {
        m_BuildingPolyPath = new PolyPath();
        m_Storey = GetComponent<Storey>();
        m_Roof = GetComponent<Roof>();
        m_Palette = ScriptableObject.CreateInstance<MaterialPalette>().Initialize();
        m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
        m_PivotPosition = Vector3.zero;
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
        if (m_BuildingPolyPath == null)
            return;

        if (m_BuildingPolyPath.ControlPointCount == 0)
            return;

        if (m_BuildingPolyPath.PolyMode == PolyMode.Hide)
            return;

        if (GUIUtility.hotControl == 0 && m_IsPolyPathHandleSelected)
        {
            Rebuild();
        }

        m_IsPolyPathHandleSelected = GUIUtility.hotControl > 0 && GUIUtility.hotControl < m_BuildingPolyPath.ControlPointCount + 1 ? true : false;
    }

    public void Rebuild()
    {
        // Issue: When try to centre the pivot point the building moves position & doesn't align with the control points.
        m_HasInitialized = false;
        //Initialize();
        Build();
    }

    public IBuildable Initialize(IData data)
    {
        if (m_HasInitialized)
            return this;

        m_Data = data as BuildingData;
        m_HasInitialized = true;

        return this;
    }

    public void Build()
    {
        transform.DeleteChildren();

        if (!m_BuildingPolyPath.IsPathValid || !m_HasInitialized)
            return;

        Vector3 pos = Vector3.zero;

        for (int i = 0; i < m_Data.StoreysData.Count; i++)
        {
            GameObject next = new GameObject("Storey " + i.ToString());
            next.transform.SetParent(transform, false);
            next.transform.localPosition = pos;
            Storey storey = next.AddComponent<Storey>().Initialize(m_Data.StoreysData[i]) as Storey;
            storey.Build();
            storey.OnDataChange += (StoreyData data) => { m_Data.StoreysData[data.ID] = data; };
            pos += (Vector3.up * storey.Data.WallData.Height);
        }

        GameObject roofGO = new GameObject("Roof");
        roofGO.transform.SetParent(transform, false);
        roofGO.transform.localPosition = pos;
        roofGO.AddComponent<Roof>().Initialize(m_Roof.Data).Build();
        roofGO.GetComponent<Roof>().OnDataChange += (RoofData data) => { m_Data.RoofData = data; };
        return;
    }

    //private void Building_OnControlPointsChanged(List<ControlPoint> controlPoints)
    //{
    //    m_HasInitialized = false;
    //    m_BuildingPolyPath.OnControlPointsChanged -= Building_OnControlPointsChanged;
    //    Build();
    //}

    //private void Building_OnAnyRoofChange(RoofData data)
    //{
    //    if (m_Roof == null && m_BuildingPolyPath == null)
    //        return;
        
    //    m_Roof.Initialize(data);
    //}

    public void RevertBuilding()
    {
        transform.DeleteChildren();
    }

    private void OnDrawGizmos()
    {
        //for (int i = 0; i < PolyPath.ControlPointCount; i++)
        //{
        //    Handles.Label(transform.TransformPoint(PolyPath.GetPositionAt(i)), i.ToString(), EditorStyles.largeLabel);
        //}
    }

}
