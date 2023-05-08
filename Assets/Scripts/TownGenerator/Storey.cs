using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Serialization;

[System.Serializable]
public class Storey : MonoBehaviour
{
    [SerializeField, HideInInspector] private int m_StoreyID;
    [SerializeField] private ControlPoint[] m_ControlPoints;
    private IEnumerable<Vector3> m_NextStoreyPoints;

    // Wall
    [SerializeField, Range(1, 100)] private float m_WallHeight;
    [SerializeField, Range(0, 1)] private float m_WallDepth;
    [SerializeField] private Material m_WallMaterial;
    [SerializeField] private Vector3[] m_InsidePoints;
    public float WallHeight => m_WallHeight;
    public float WallDepth => m_WallDepth;
    public Material WallMaterial => m_WallMaterial;
    // End Wall

    // Floor
    [SerializeField] private Material m_FloorMaterial;
    [SerializeField, Range(0.00001f, 1)] private float m_FloorHeight;
    public float FloorHeight => m_FloorHeight;
    public Material FloorMaterial => m_FloorMaterial;
    // End Floor

    // Pillar
    [SerializeField] private float m_PillarWidth, m_PillarDepth;
    [SerializeField] private Material m_PillarMaterial;
    public Material PillarMaterial => m_PillarMaterial;
    public float PillarWidth => m_PillarWidth;
    public float PillarDepth => m_PillarDepth;
    // End Pillar

    public int ID => m_StoreyID;
    public IEnumerable<Vector3> NextStoreyPoints => m_NextStoreyPoints;
    public IEnumerable<ControlPoint> ControlPoints => m_ControlPoints;

    public Vector3[] InsidePoints
    {
        get
        {
            m_InsidePoints = new Vector3[m_ControlPoints.Length];

            for (int i = 0; i < m_InsidePoints.Length; i++)
            {
                float w = Mathf.Lerp(-1, 1, m_WallDepth);
                m_InsidePoints[i] = m_ControlPoints[i].Position + m_ControlPoints[i].Forward + (m_ControlPoints[i].Forward * w);
            }

            return m_InsidePoints;
        }
    }

    public void SetControlPoints(IEnumerable<ControlPoint> controlPoints)
    {
        m_ControlPoints = controlPoints.ToArray();
    }

    public void SetID(int id)
    {
        m_StoreyID = id;
    }

    private void Reset()
    {
        Initialize(-1, null, 3, 0.5f, null, 0.1f, null, 0.5f, 0.5f, null);
    }

    public Storey Initialize(Storey storey)
    {
        return Initialize(storey.ID, storey.ControlPoints, storey.WallHeight, storey.WallDepth, storey.WallMaterial, storey.FloorHeight, storey.FloorMaterial, storey.PillarWidth, storey.PillarDepth, storey.PillarMaterial);
    }

    public Storey Initialize(int id, IEnumerable<ControlPoint> controlPoints, float wallHeight, float wallDepth, Material wallMaterial, float floorHeight, Material floorMaterial, float pillarWidth, float pillarDepth, Material pillarMaterial)
    {
        m_StoreyID = id;
        m_ControlPoints = controlPoints != null ? controlPoints.ToArray() : null;

        m_WallHeight = wallHeight;
        m_WallDepth = wallDepth;
        m_WallMaterial = wallMaterial;

        m_FloorMaterial = floorMaterial;
        m_FloorHeight = floorHeight;

        m_PillarDepth = pillarDepth;
        m_PillarWidth = pillarWidth;
        m_PillarMaterial = pillarMaterial;

        if (m_FloorMaterial == null)
        {
            m_FloorMaterial = BuiltinMaterials.defaultMaterial;
        }

        if (m_WallMaterial == null)
        {
            m_WallMaterial = BuiltinMaterials.defaultMaterial;
        }

        if (m_PillarMaterial == null)
        {
            m_PillarMaterial = BuiltinMaterials.defaultMaterial;
        }

        return this;
    }

    public Storey Build()
    {
        BuildPillars();
        m_InsidePoints = m_InsidePoints ?? InsidePoints;
        BuildExternalWalls();
        BuildFloor();

        return this;
    }

    private void BuildPillars()
    {
        GameObject pillars = new GameObject("Pillars");
        pillars.transform.SetParent(transform, false);

        //float topY = 0; 
        Vector3 scale = new Vector3(m_PillarWidth, m_WallHeight, m_PillarDepth);
        for (int i = 0; i < m_ControlPoints.Length; i++)
        {
            ProBuilderMesh pillar = ShapeGenerator.GenerateCube(PivotLocation.Center, scale);
            pillar.name = "Pillar " + i.ToString();
            pillar.transform.SetParent(transform, true);
            pillar.transform.localPosition = m_ControlPoints[i].Position;
            pillar.GetComponent<Renderer>().GetComponent<Renderer>().material = m_PillarMaterial;
            int index = m_ControlPoints.GetNext(i);

            pillar.transform.forward = pillar.transform.localPosition.DirectionToTarget(m_ControlPoints[index].Position);
            pillar.transform.localPosition += m_WallHeight / 2 * Vector3.up;
            pillar.transform.SetParent(pillars.transform, true);
        }
    }

    private void BuildExternalWalls()
    {
        Vector3 h = m_WallHeight * Vector3.up;

        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(transform, true);

        GameObject corners = new GameObject("Wall Corners");
        corners.transform.SetParent(walls.transform, true);

        // Construct the walls 
        for (int i = 0; i < m_ControlPoints.Length; i++)
        {
            int oneNext = m_ControlPoints.GetNext(i);
            int onePrevious = m_ControlPoints.GetPrevious(i);

            Vector3 nextControlPoint = m_ControlPoints[oneNext].Position;
            Vector3 oneNextInside = m_InsidePoints[oneNext];
            Vector3 onePreviousInside = m_InsidePoints[onePrevious];

            Vector3 nextForward = m_InsidePoints[i].DirectionToTarget(oneNextInside);
            Vector3 nextRight = Vector3.Cross(Vector3.up, nextForward) * m_WallDepth;

            Vector3 previousForward = m_InsidePoints[i].DirectionToTarget(onePreviousInside);
            Vector3 previousRight = Vector3.Cross(previousForward, Vector3.up) * m_WallDepth;

            Vector3 bottomLeft = m_InsidePoints[i];
            Vector3 topLeft = bottomLeft + h;
            Vector3 bottomRight = oneNextInside;
            Vector3 topRight = bottomRight + h;

            // Post Points
            Vector3 zero = m_ControlPoints[i].Position;
            Vector3 one = m_InsidePoints[i] + nextRight;
            Vector3 two = m_InsidePoints[i];
            Vector3 three = m_InsidePoints[i] + previousRight;

            bool isConcave = m_ControlPoints.IsConcave(out int[] concavePoints);

            if (isConcave)
            {
                for (int j = 0; j < concavePoints.Length; j++)
                {
                    if (concavePoints[j] == oneNext)
                    {
                        bottomRight = nextControlPoint - nextRight;
                        topRight = bottomRight + h;
                    }

                    if (concavePoints[j] == i)
                    {
                        bottomLeft = m_ControlPoints[i].Position - nextRight;
                        topLeft = bottomLeft + h;

                        one = m_ControlPoints[i].Position - nextRight;
                        three = m_ControlPoints[i].Position - previousRight;
                    }
                }
            }

            Vector3[] points = new Vector3[] { bottomLeft, topLeft, topRight, bottomRight };

            GameObject wall = new GameObject("Wall " + i.ToString(), typeof(Wall));
            wall.transform.SetParent(walls.transform, true);
            wall.GetComponent<Wall>().Initialize(points, m_WallDepth, m_WallMaterial).Build();

            ProBuilderMesh post = ProBuilderMesh.Create();
            post.name = "Corner";
            post.CreateShapeFromPolygon(new Vector3[] { zero, one, two, three }, m_WallHeight, false);
            post.GetComponent<Renderer>().sharedMaterial = m_WallMaterial;
            post.ToMesh();
            post.Refresh();

            post.transform.SetParent(corners.transform, true);
        }
    }

    private void BuildFloor()
    {
        ProBuilderMesh floor = ProBuilderMesh.Create();
        floor.name = "Floor";
        floor.transform.SetParent(transform, false);
        Vector3[] positions = PolygonRecognition.GetPositions(m_ControlPoints);
        floor.CreateShapeFromPolygon(positions, m_FloorHeight, false);
        floor.GetComponent<Renderer>().sharedMaterial = m_FloorMaterial;
        floor.ToMesh();
        floor.Refresh();
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.red;
        Handles.DrawAAPolyLine(InsidePoints);

    }
}
