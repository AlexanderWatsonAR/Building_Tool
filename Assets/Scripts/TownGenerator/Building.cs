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

// Generic Building Base Class
[DisallowMultipleComponent]
[RequireComponent(typeof(Storey), typeof(Roof))]
public class Building : MonoBehaviour
{
    [SerializeField, HideInInspector] private bool m_HasConstructed;

    protected Polytool m_BuildingPolytool;
    protected List<Storey> m_Storeys;
    protected Roof m_Roof;

    public bool HasConstructed => m_HasConstructed;

    public void SetBuildingMaterials(GameObject pillarPrefab, GameObject wallOutlinePrefab, Material colourSwatchMaterial)
    {
        //m_Storeys = GetComponents<Storey>();

        //foreach (Storey storey in m_Storeys)
        //{
        //    storey.Initialize(wallOutlinePrefab, pillarPrefab);
        //}

        //m_ColourSwatchMaterial = colourSwatchMaterial;
    }

    private void Reset()
    {
        Initialize();
    }

    public Building Initialize()
    {
        m_Storeys = GetComponents<Storey>().ToList();
        m_Roof = GetComponent<Roof>();
        m_BuildingPolytool = GetComponent<Polytool>();

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
            storey.SetControlPoints(m_BuildingPolytool.ControlPoints);
            storey.SetID(count);
            count++;
        }

        m_Roof.SetControlPoints(m_BuildingPolytool.ControlPoints);

        return this;
    }

    public Building Build()
    {
        transform.DeleteChildren();

        Initialize();

        if (!m_BuildingPolytool.IsClockwise())
        {
            IEnumerable<Vector3> reverseControlPoints = m_BuildingPolytool.ControlPoints;
            reverseControlPoints.Reverse();
            m_BuildingPolytool.SetControlPoints(reverseControlPoints);
        }

        Vector3 pos = Vector3.zero;

        for (int i = 0; i < m_Storeys.Count; i++)
        {
            GameObject next = new GameObject("Storey " + i.ToString());
            next.transform.SetParent(transform, false);
            next.transform.localPosition = pos;
            Storey storey = next.AddComponent<Storey>().Initialize(m_Storeys[i]).Build();
            pos += (Vector3.up * storey.WallHeight);
        }

        Roof roof = GetComponent<Roof>();

        GameObject roofGO = new GameObject("Roof");
        roofGO.transform.SetParent(transform, false);
        roofGO.transform.localPosition = pos;
        roofGO.AddComponent<Roof>().Initialize(roof, m_BuildingPolytool.ControlPoints).ConstructFrame();
        roofGO.GetComponent<Roof>().OnAnyRoofChange += Building_OnAnyRoofChange;
        m_HasConstructed = true;
        return this;
    }

    private void Building_OnAnyRoofChange(Roof obj)
    {
        if (m_Roof == null && m_BuildingPolytool == null)
            return;

        m_Roof.Initialize(obj, m_BuildingPolytool.ControlPoints);
    }

    public void RevertToPolyshape()
    {
        transform.DeleteChildren();
        m_HasConstructed = false;
    }

}
