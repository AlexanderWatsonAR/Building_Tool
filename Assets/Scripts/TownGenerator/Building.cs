using AutoLayout3D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
[RequireComponent(typeof(Storey), typeof(Roof))]
public class Building : MonoBehaviour
{
    [SerializeField, HideInInspector] private bool m_HasConstructed;

    [SerializeField] private PolyPath m_BuildingPolyPath;
    [SerializeField] private List<Storey> m_Storeys;
    [SerializeField] private Roof m_Roof;
    [SerializeField] private bool m_HasInitialized;
    [SerializeField] private MaterialPalette m_Palette;

    public bool HasConstructed => m_HasConstructed;
    public ControlPoint[] ControlPoints => m_BuildingPolyPath.ControlPoints.ToArray();
    public PolyPath PolyPath => m_BuildingPolyPath ?? new PolyPath();

    private void Reset()
    {
        m_BuildingPolyPath = new PolyPath();
        m_Storeys = GetComponents<Storey>().ToList();
        m_Roof = GetComponent<Roof>();
        m_Palette = ScriptableObject.CreateInstance<MaterialPalette>().Initialize();
    }

    public Building Initialize()
    {
        if (m_HasInitialized)
            return this;

        m_BuildingPolyPath.CalculateForwards();
        m_BuildingPolyPath.OnControlPointsChanged += Building_OnControlPointsChanged;

        if(m_Storeys == null)
        {
            m_Storeys.Add(gameObject.AddComponent<Storey>());
        }

        if(m_Roof == null)
        {
            m_Roof = gameObject.AddComponent<Roof>();
        }

        int count = 0;
        foreach(Storey storey in m_Storeys)
        {
            storey.SetControlPoints(ControlPoints);
            storey.SetID(count);
            count++;
        }

        m_Roof.SetControlPoints(ControlPoints).SetRoofActive(true);

        m_HasInitialized = true;

        return this;
    }

    public Building Build()
    {
        transform.DeleteChildren();

        Vector3 pos = Vector3.zero;

        for (int i = 0; i < m_Storeys.Count; i++)
        {
            GameObject next = new GameObject("Storey " + i.ToString());
            next.transform.SetParent(transform, false);
            next.transform.localPosition = pos;
            Storey storey = next.AddComponent<Storey>().Initialize(m_Storeys[i]).Build();
            pos += (Vector3.up * storey.WallData.Height);
        }

        GameObject roofGO = new GameObject("Roof");
        roofGO.transform.SetParent(transform, false);
        roofGO.transform.localPosition = pos;
        roofGO.AddComponent<Roof>().Initialize(m_Roof.Data).SetControlPoints(ControlPoints);
        roofGO.GetComponent<Roof>().BuildFrame();
        roofGO.GetComponent<Roof>().OnAnyRoofChange += Building_OnAnyRoofChange;
        m_HasConstructed = true;
        return this;
    }

    private void Building_OnControlPointsChanged(List<ControlPoint> controlPoints)
    {
        m_HasInitialized = false;
        m_BuildingPolyPath.OnControlPointsChanged -= Building_OnControlPointsChanged;
        Build();
    }

    private void Building_OnAnyRoofChange(RoofData data)
    {
        if (m_Roof == null && m_BuildingPolyPath == null)
            return;
        
        m_Roof.Initialize(data).BuildFrame();
    }

    public void RevertBuilding()
    {
        transform.DeleteChildren();
        m_HasConstructed = false;
    }

}
