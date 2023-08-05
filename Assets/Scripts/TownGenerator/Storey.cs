using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UIElements;

[System.Serializable]
public class Storey : MonoBehaviour
{
    [SerializeField, HideInInspector] private int m_StoreyID;
    [SerializeField] private ControlPoint[] m_ControlPoints;
    [SerializeField] private StoreyElement m_ActiveElements;

    // Wall
    [SerializeField, Range(1, 100)] private float m_WallHeight;
    [SerializeField, Range(0, 1)] private float m_WallDepth;
    [SerializeField] private Material m_WallMaterial;
    [SerializeField] private Vector3[] m_InsidePoints;
    [SerializeField] private bool m_CurvedCorners;
    [SerializeField, Range(3, 15)] private int m_CurvedCornersSides;

    public int CurvedCornersSides => m_CurvedCornersSides;
    public bool CurvedCorners => m_CurvedCorners;
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
    [SerializeField, Range(3, 32)] private int m_PillarSides;
    [SerializeField] private bool m_IsPillarSmooth;
    [SerializeField] private Material m_PillarMaterial;

    public Material PillarMaterial => m_PillarMaterial;
    public float PillarWidth => m_PillarWidth;
    public float PillarDepth => m_PillarDepth;
    public int PillarSides => m_PillarSides;
    public bool IsPillarSmooth => m_IsPillarSmooth;
    // End Pillar

    public int ID => m_StoreyID;
    public IEnumerable<ControlPoint> ControlPoints => m_ControlPoints;
    public StoreyElement ActiveElements => m_ActiveElements;
    public bool AreWallsActive => IsElementActive(StoreyElement.Walls);
    public bool ArePillarsActive => IsElementActive(StoreyElement.Pillars);
    public bool IsFloorActive => IsElementActive(StoreyElement.Floor);


    List<Vector3[]> m_WallPoints;


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

    public void SetWallHeight(float height)
    {
        m_WallHeight = height;
    }

    private void Reset()
    {
        Initialize();
    }

    public Storey Initialize()
    {
        return Initialize(-1, null, StoreyElement.Everything, 3, 0.5f, false, 4, null, 0.1f, null, 0.5f, 0.5f, 4, false, null);
    }

    public Storey Initialize(Storey storey)
    {
        return Initialize(storey.ID, storey.ControlPoints, storey.ActiveElements, storey.WallHeight, storey.WallDepth, storey.CurvedCorners, storey.CurvedCornersSides, storey.WallMaterial, storey.FloorHeight, storey.FloorMaterial, storey.PillarWidth, storey.PillarDepth, storey.PillarSides, storey.IsPillarSmooth, storey.PillarMaterial);
    }

    public Storey Initialize(int id, IEnumerable<ControlPoint> controlPoints, StoreyElement activeElements, float wallHeight, float wallDepth, bool curvedCorners, int cornerSides, Material wallMaterial, float floorHeight, Material floorMaterial, float pillarWidth, float pillarDepth, int pillarSides, bool isPillarSmooth, Material pillarMaterial)
    {
        m_StoreyID = id;
        m_ControlPoints = controlPoints != null ? controlPoints.ToArray() : null;
        m_ActiveElements = activeElements;

        m_WallHeight = wallHeight;
        m_WallDepth = wallDepth;
        m_CurvedCorners = curvedCorners;
        m_CurvedCornersSides = cornerSides;
        m_WallMaterial = wallMaterial;

        m_FloorMaterial = floorMaterial;
        m_FloorHeight = floorHeight;

        m_PillarDepth = pillarDepth;
        m_PillarWidth = pillarWidth;
        m_PillarSides = pillarSides;
        m_IsPillarSmooth = isPillarSmooth;
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

        m_WallPoints = new();

        return this;
    }

    public Storey Build()
    {
        transform.DeleteChildren();
        m_WallPoints.Clear();
        BuildPillars();
        m_InsidePoints = InsidePoints;
        
        BuildExternalWalls();
        BuildCorners();
        BuildFloor();

        return this;
    }

    private void BuildPillars()
    {
        if (!IsElementActive(StoreyElement.Pillars))
            return;

        GameObject pillars = new GameObject("Pillars");
        pillars.transform.SetParent(transform, false);

        float halfWidth = m_PillarWidth * 0.5f;
        float halfDepth = m_PillarDepth * 0.5f;

        Vector3[] pillarPoints = new Vector3[]
        {
            new Vector3(-halfWidth, -halfDepth),
            new Vector3(-halfWidth, halfDepth),
            new Vector3(halfWidth, halfDepth),
            new Vector3(halfWidth, -halfDepth)
        };

        Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.up);

        if (m_PillarSides != 4)
        {
            pillarPoints = MeshMaker.CreateNPolygon(m_PillarSides, halfWidth, halfDepth); 
        }

        for (int i = 0; i < pillarPoints.Length; i++)
        {
            Vector3 euler = rotation.eulerAngles;
            Vector3 v = Quaternion.Euler(euler) * pillarPoints[i];
            pillarPoints[i] = v;
        }

        for (int i = 0; i < m_ControlPoints.Length; i++)
        {
            ProBuilderMesh pillar = ProBuilderMesh.Create();
            pillar.CreateShapeFromPolygon(pillarPoints, 0, false);
            pillar.ToMesh();
            Face[] faces = pillar.Extrude(pillar.faces, ExtrudeMethod.FaceNormal, m_WallHeight);
            pillar.name = "Pillar " + i.ToString();
            pillar.transform.SetParent(transform, true);
            pillar.transform.localPosition = m_ControlPoints[i].Position;
            pillar.GetComponent<Renderer>().GetComponent<Renderer>().material = m_PillarMaterial;
            int index = m_ControlPoints.GetNext(i);

            pillar.transform.forward = pillar.transform.localPosition.DirectionToTarget(m_ControlPoints[index].Position);
            pillar.transform.SetParent(pillars.transform, true);

            if(m_IsPillarSmooth)
                Smoothing.ApplySmoothingGroups(pillar, faces, 360f / m_PillarSides);

            pillar.ToMesh();
            pillar.Refresh();
        }
    }

    private void BuildCorners()
    {
        if (!IsElementActive(StoreyElement.Walls))
            return;

        if (m_WallPoints.Count != m_ControlPoints.Length)
            return;

        GameObject corners = new GameObject("Corners");
        corners.transform.SetParent(transform, true);

        bool isConcave = m_ControlPoints.IsConcave(out int[] concavePoints);

        for (int i = 0; i < m_WallPoints.Count; i++)
        {
            int current = i;
            int previous = m_ControlPoints.GetPrevious(i);
            int next = m_ControlPoints.GetNext(i);

            Vector3 dirA = m_WallPoints[current][0].DirectionToTarget(m_WallPoints[current][1]);
            Vector3 crossA = Vector3.Cross(Vector3.up, dirA) * m_WallDepth;

            Vector3 dirB = m_WallPoints[previous][0].DirectionToTarget(m_WallPoints[previous][1]);
            Vector3 crossB = Vector3.Cross(Vector3.up, dirB) * m_WallDepth;

            Vector3 intersection;

            Extensions.DoLinesIntersect(m_WallPoints[current][0] + crossA, m_WallPoints[current][1] + crossA, m_WallPoints[previous][0] + crossB, m_WallPoints[previous][1] + crossB, out intersection);

            int numberOfSamples = m_CurvedCornersSides + 1;

            Vector3[] cornerPoints = new Vector3[] { m_WallPoints[current][0], m_WallPoints[current][0] + crossA, m_WallPoints[current][0] + crossB, intersection };
            Vector3[] curveyPoints = Vector3Extensions.QuadraticLerpCollection(cornerPoints[1], cornerPoints[3], cornerPoints[2], numberOfSamples);
            Vector3[] points = new Vector3[curveyPoints.Length + 1];
            points[0] = cornerPoints[0];

            if (isConcave)
            {
                if(concavePoints.Any(b => b == current))
                {
                    Extensions.DoLinesIntersect(m_WallPoints[current][0], m_WallPoints[current][1], m_WallPoints[previous][0], m_WallPoints[previous][1], out intersection);
                    cornerPoints = new Vector3[] { m_WallPoints[current][0], m_WallPoints[current][0] + crossA, m_WallPoints[previous][1], intersection };
                    curveyPoints = Vector3Extensions.QuadraticLerpCollection(cornerPoints[2], cornerPoints[3], cornerPoints[0], numberOfSamples);
                    points[0] = cornerPoints[1];
                }

            }

            for(int j = 0; j < curveyPoints.Length; j++)
            {
                points[j+1] = curveyPoints[j];
            }

            points = points.SortPointsClockwise().ToArray();
            cornerPoints = cornerPoints.SortPointsClockwise().ToArray();

            ProBuilderMesh post = ProBuilderMesh.Create();
            post.name = "Corner";
            if (m_CurvedCorners)
            {
                post.CreateShapeFromPolygon(points, 0, false);
                Face[] extrudeFaces = post.Extrude(new Face[] { post.faces[0] }, ExtrudeMethod.FaceNormal, m_WallHeight);
                //Smoothing.ApplySmoothingGroups(post, extrudeFaces, 360);
            }
            else
                post.CreateShapeFromPolygon(cornerPoints, m_WallHeight, false);
            post.GetComponent<Renderer>().sharedMaterial = m_WallMaterial;
            post.ToMesh();
            post.Refresh();

            post.transform.SetParent(corners.transform, true);

        }
    }

    private void BuildExternalWalls()
    {
        if (!IsElementActive(StoreyElement.Walls))
            return;

        Vector3 h = m_WallHeight * Vector3.up;

        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(transform, true);

        // Construct the walls 
        for (int i = 0; i < m_ControlPoints.Length; i++)
        {
            int current = i;
            int next = m_ControlPoints.GetNext(i);
            int previous = m_ControlPoints.GetPrevious(i);

            Vector3 nextControlPoint = m_ControlPoints[next].Position;
            Vector3 oneNextInside = m_InsidePoints[next];
            Vector3 onePreviousInside = m_InsidePoints[previous];

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
                bool conditionA = concavePoints.Any(a => a == next);
                bool conditionB = concavePoints.Any(b => b == current);

                if (conditionA)
                {
                    bottomRight = nextControlPoint - nextRight;
                    topRight = bottomRight + h;
                }

                if (conditionB)
                {
                    bottomLeft = m_ControlPoints[i].Position - nextRight;
                    topLeft = bottomLeft + h;
                }

            }

            m_WallPoints.Add(new Vector3[] { bottomLeft, bottomRight });

            Vector3[] points = new Vector3[] { bottomLeft, topLeft, topRight, bottomRight };

            GameObject wall = new GameObject("Wall " + i.ToString(), typeof(Wall));
            wall.transform.SetParent(walls.transform, true);
            wall.GetComponent<Wall>().Initialize(points, m_WallDepth, m_WallMaterial).Build();
        }
    }

    private void BuildFloor()
    {
        if (!IsElementActive(StoreyElement.Floor))
            return;

        ProBuilderMesh floor = ProBuilderMesh.Create();
        floor.name = "Floor";
        floor.transform.SetParent(transform, false);
        Vector3[] positions = PolygonRecognition.GetPositions(m_ControlPoints);
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] += m_ControlPoints[i].Forward * (m_WallDepth * 0.5f);
        }
        floor.CreateShapeFromPolygon(positions, m_FloorHeight, false);
        floor.GetComponent<Renderer>().sharedMaterial = m_FloorMaterial;
        floor.ToMesh();
        floor.Refresh();
    }

    private bool IsElementActive(StoreyElement storeyElement)
    {
        return m_ActiveElements == StoreyElement.Nothing ? false : (m_ActiveElements & storeyElement) != 0;
    }

    private void OnDrawGizmosSelected()
    {
        //if(curvePoints != null)
        //{
        //    if(curvePoints.Length > 0)
        //    {
        //        for(int i = 0; i < curvePoints.Length; i++)
        //        {
        //            Handles.DrawSolidDisc(curvePoints[i], Vector3.up, 0.05f);
        //            //Handles.Disc(-1, Quaternion.identity, curvePoints[i], Vector3.up, 0.1f, false, 1);
        //        }
        //    }
        //}

        //if (m_ConcaveCornerPoints != null)
        //{
        //    if (m_ConcaveCornerPoints.Length > 0)
        //    {
        //        for (int i = 0; i < m_ConcaveCornerPoints.Length; i++)
        //        {
        //            Handles.Label(m_ConcaveCornerPoints[i] + (Vector3.up * m_WallHeight), new GUIContent(i.ToString()));
        //        }
        //    }
        //}
    }
}
