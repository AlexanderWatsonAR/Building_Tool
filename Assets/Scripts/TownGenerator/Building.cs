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
    protected Storey[] m_Storeys;
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

    public void Construct()
    {
        Deconstruct();

        m_Storeys = GetComponents<Storey>();
        m_Roof = GetComponent<Roof>();
        m_BuildingPolytool = GetComponent<Polytool>();

        if (!m_BuildingPolytool.IsClockwise())
        {
            IEnumerable<Vector3> reverseControlPoints = m_BuildingPolytool.ControlPoints;
            reverseControlPoints.Reverse();
            m_BuildingPolytool.SetControlPoints(reverseControlPoints);
        }

        Vector3 pos = Vector3.zero;

        for (int i = 0; i < m_Storeys.Length; i++)
        {
            GameObject next = new GameObject("Storey " + i.ToString());
            next.transform.SetParent(transform, false);
            next.transform.localPosition = pos;
            Storey storey = next.AddComponent<Storey>().Initialize(m_Storeys[i]).Contruct(m_BuildingPolytool.ControlPoints);
            pos += storey.TopCentre;
        }

        Roof roof = GetComponent<Roof>();

        GameObject roofGO = new GameObject("Roof");
        roofGO.transform.SetParent(transform, false);
        roofGO.transform.localPosition = pos;
        roofGO.AddComponent<Roof>().Initialize(roof, m_BuildingPolytool.ControlPoints).ConstructFrame();
        m_HasConstructed = true;
    }

    public void RevertToPolyshape()
    {
        Deconstruct();
        m_HasConstructed = false;
    }

    private void Deconstruct()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (Application.isEditor)
            {
                DestroyImmediate(child);
            }
            else
            {
                Destroy(child);
            }

        }

        if (transform.childCount > 0)
        {
            Deconstruct();
        }
    }
    

}
