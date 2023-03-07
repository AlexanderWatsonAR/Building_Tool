using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Serialization;

[System.Serializable]
public class Storey : MonoBehaviour
{
    [SerializeField] private GameObject m_PillarPrefab;
    [SerializeField] private GameObject m_WallOutlinePrefab;
    [SerializeField] private Material m_FloorMaterial;
    [SerializeField, HideInInspector] private float m_TopY;

    private IEnumerable<Vector3> m_ControlPoints;
    private IEnumerable<Vector3> m_NextStoreyPoints;

    public IEnumerable<Vector3> NextStoreyPoints => m_NextStoreyPoints;
    public GameObject PillarPrefab => m_PillarPrefab;
    public GameObject WallOutlinePrefab => m_WallOutlinePrefab;
    public Material FloorMaterial => m_FloorMaterial;

    public Vector3 TopCentre => m_TopY * Vector3.up;

    public Storey Initialize(Storey storey)
    {
        return Initialize(storey.WallOutlinePrefab, storey.PillarPrefab, storey.FloorMaterial);
    }

    public Storey Initialize(GameObject wallOutlinePrefab, GameObject pillarPrefab, Material floorMaterial)
    {
        m_PillarPrefab = pillarPrefab;
        m_WallOutlinePrefab = wallOutlinePrefab;
        m_FloorMaterial = floorMaterial;
        return this;
    }

    public Storey Contruct(IEnumerable<Vector3> controlPoints)
    {
        m_ControlPoints = controlPoints;

        if (m_PillarPrefab != null)
            PlacePillars();

        ConstructExternalWalls();
        ConstructFloor();

        return this;
    }

    private void PlacePillars()
    {
        Vector3[] controlPointsArray = m_ControlPoints.ToArray();

        List<Vector3> topPillarPoints = new List<Vector3>();

        GameObject pillars = new GameObject("Pillars");
        pillars.transform.SetParent(transform, false);

        float topY = 0; 

        for (int i = 0; i < controlPointsArray.Length; i++)
        {
            GameObject pillar = Instantiate(m_PillarPrefab);
            pillar.name = "Pillar " + i.ToString();
            pillar.transform.SetParent(transform, true);
            pillar.transform.localPosition = controlPointsArray[i];

            int index = controlPointsArray.GetNextControlPoint(i);

            pillar.transform.forward = pillar.transform.localPosition.GetDirectionToTarget(controlPointsArray[index]);

            topY += pillar.GetComponent<Renderer>().localBounds.max.y * pillar.transform.localScale.y;

            pillar.transform.SetParent(pillars.transform, true);
        }

        m_TopY = (topY /= controlPointsArray.Length);
        m_NextStoreyPoints = topPillarPoints;
    }

    private void ConstructExternalWalls()
    {
        Vector3[] controlPointsArray = m_ControlPoints.ToArray();

        GameObject walls = new ("Walls");
        walls.transform.SetParent(transform, false);

        for (int i = 0; i < controlPointsArray.Length; i++)
        {
            int next = m_ControlPoints.GetNextControlPoint(i);

            Vector3 a = transform.TransformPoint(controlPointsArray[i]);
            Vector3 b = transform.TransformPoint(controlPointsArray[next]);

            float distance = Vector3.Distance(a, b);

            GameObject wall = new ("Wall");
            wall.transform.SetParent(transform, true);
            wall.transform.localPosition = controlPointsArray[i];
            wall.transform.forward = wall.transform.localPosition.GetDirectionToTarget(controlPointsArray[next]);
            wall.AddComponent<Wall>().Initialize(m_WallOutlinePrefab, distance).Build();
            wall.transform.SetParent(walls.transform, true);
        }
    }

    private void ConstructFloor()
    {
        ProBuilderMesh floor = ProBuilderMesh.Create();
        floor.name = "Floor";
        floor.transform.SetParent(transform, false);
        floor.CreateShapeFromPolygon(m_ControlPoints.ToList(), 0.1f, false);
        floor.GetComponent<Renderer>().sharedMaterial = m_FloorMaterial;
        floor.ToMesh();
        floor.Refresh();
    }
}
