using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class Storey : MonoBehaviour
{
    [SerializeField, HideInInspector] private int m_StoreyID;
    [SerializeField] private ControlPoint[] m_ControlPoints;
    [SerializeField] private StoreyElement m_ActiveElements;

    // Wall
    [SerializeField] private WallData m_WallData; 
    [SerializeField] private Vector3[] m_InsidePoints;
    [SerializeField] private CornerType m_CornerType;
    //[SerializeField] private bool m_CurvedCorners;
    [SerializeField, Range(3, 15)] private int m_CurvedCornersSides;

    public List<StoreyData> testList = new();

    public WallData WallData => m_WallData;
    public int CurvedCornersSides => m_CurvedCornersSides;
    public CornerType CornerType => m_CornerType;
    //public bool CurvedCorners => m_CurvedCorners;
    // End Wall

    // Floor
    [SerializeField] private Material m_FloorMaterial;
    [SerializeField, Range(0.00001f, 1)] private float m_FloorHeight;
    public float FloorHeight => m_FloorHeight;
    public Material FloorMaterial => m_FloorMaterial;
    // End Floor

    // Pillar
    [SerializeField] private PillarData m_PillarData;
    
    public PillarData PillarData => m_PillarData;
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
                float w = Mathf.Lerp(-1, 1, m_WallData.Depth);
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
        m_WallData.SetHeight(height);
    }

    private void Reset()
    {
        Initialize();
    }

    public Storey Initialize()
    {
        return Initialize(-1, null, StoreyElement.Everything, new WallData(), CornerType.Point, 4, 0.1f, null, new PillarData());
    }

    public Storey Initialize(Storey storey)
    {
        return Initialize(storey.ID, storey.ControlPoints, storey.ActiveElements, storey.WallData, storey.CornerType, storey.CurvedCornersSides, storey.FloorHeight, storey.FloorMaterial, storey.PillarData);
    }

    public Storey Initialize(int id, IEnumerable<ControlPoint> controlPoints, StoreyElement activeElements, WallData wallData, CornerType cornerType, int cornerSides, float floorHeight, Material floorMaterial, PillarData pillarData)
    {
        m_StoreyID = id;
        m_ControlPoints = controlPoints != null ? controlPoints.ToArray() : null;
        m_ActiveElements = activeElements;

        m_WallData = wallData;
        m_CornerType = cornerType;
        m_CurvedCornersSides = cornerSides;

        m_FloorMaterial = floorMaterial;
        m_FloorHeight = floorHeight;

        m_PillarData = pillarData;
        m_PillarData.SetHeight(m_WallData.Height);

        if (m_WallData.Material == null)
        {
            m_WallData.SetMaterial(BuiltinMaterials.defaultMaterial);
        }

        if (m_PillarData.Material == null)
        {
            m_PillarData.SetMaterial(BuiltinMaterials.defaultMaterial);
        }

        if (m_FloorMaterial == null)
        {
            m_FloorMaterial = BuiltinMaterials.defaultMaterial;
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

        for (int i = 0; i < m_ControlPoints.Length; i++)
        {
            ProBuilderMesh pillar = ProBuilderMesh.Create();
            pillar.name = "Pillar " + i.ToString();
            pillar.AddComponent<Pillar>().Initialize(m_PillarData).Build();
            pillar.transform.SetParent(pillars.transform, false);
            pillar.transform.localPosition = m_ControlPoints[i].Position;
            int index = m_ControlPoints.GetNext(i);
            pillar.transform.forward = pillar.transform.localPosition.DirectionToTarget(m_ControlPoints[index].Position);
            
        }
    }

    private void BuildCorners()
    {
        if (!IsElementActive(StoreyElement.Walls))
            return;

        if (m_WallPoints.Count != m_ControlPoints.Length)
            return;

        GameObject corners = new GameObject("Corners");
        corners.transform.SetParent(transform, false);

        bool isConcave = m_ControlPoints.IsConcave(out int[] concavePoints);

        for (int i = 0; i < m_WallPoints.Count; i++)
        {
            int current = i;
            int previous = m_ControlPoints.GetPrevious(i);
            int next = m_ControlPoints.GetNext(i);

            Vector3 dirA = m_WallPoints[current][0].DirectionToTarget(m_WallPoints[current][1]);
            Vector3 crossA = Vector3.Cross(Vector3.up, dirA) * m_WallData.Depth;

            Vector3 dirB = m_WallPoints[previous][0].DirectionToTarget(m_WallPoints[previous][1]);
            Vector3 crossB = Vector3.Cross(Vector3.up, dirB) * m_WallData.Depth;

            Vector3 intersection;

            Extensions.DoLinesIntersect(m_WallPoints[current][0] + crossA, m_WallPoints[current][1] + crossA, m_WallPoints[previous][0] + crossB, m_WallPoints[previous][1] + crossB, out intersection);

            int numberOfSamples = m_CurvedCornersSides + 1;

            Vector3[] cornerPoints = new Vector3[] { m_WallPoints[current][0], m_WallPoints[current][0] + crossA, m_WallPoints[current][0] + crossB, intersection };
            Vector3[] flatPoints = new Vector3[] { cornerPoints[0], cornerPoints[1], cornerPoints[2] };
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
                    flatPoints = new Vector3[] { m_WallPoints[current][0], m_WallPoints[current][0] + crossA, m_WallPoints[previous][1] };
                    points[0] = cornerPoints[1];
                }

            }

            for(int j = 0; j < curveyPoints.Length; j++)
            {
                points[j+1] = curveyPoints[j];
            }

            points = points.SortPointsClockwise().ToArray();
            cornerPoints = cornerPoints.SortPointsClockwise().ToArray();
            flatPoints = flatPoints.SortPointsClockwise().ToArray();

            ProBuilderMesh post = ProBuilderMesh.Create();
            post.name = "Corner";

            switch (m_CornerType)
            {
                case CornerType.Point:
                    post.CreateShapeFromPolygon(cornerPoints, m_WallData.Height, false);
                    break;
                case CornerType.Round:
                    post.CreateShapeFromPolygon(points, 0, false);
                    Face[] extrudeFaces = post.Extrude(new Face[] { post.faces[0] }, ExtrudeMethod.FaceNormal, m_WallData.Height);
                    //Smoothing.ApplySmoothingGroups(post, extrudeFaces, 360);
                    break;
                case CornerType.Flat:
                    post.CreateShapeFromPolygon(flatPoints, m_WallData.Height, false);
                    break;
            }
                
            post.GetComponent<Renderer>().sharedMaterial = m_WallData.Material;
            post.ToMesh();
            post.Refresh();

            post.transform.SetParent(corners.transform, false);

        }
    }

    private void BuildExternalWalls()
    {
        if (!IsElementActive(StoreyElement.Walls))
            return;

        Vector3 h = m_WallData.Height * Vector3.up;

        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(transform, false);

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
            Vector3 nextRight = Vector3.Cross(Vector3.up, nextForward) * m_WallData.Depth;

            Vector3 previousForward = m_InsidePoints[i].DirectionToTarget(onePreviousInside);
            Vector3 previousRight = Vector3.Cross(previousForward, Vector3.up) * m_WallData.Depth;

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
            

            ProBuilderMesh wall = ProBuilderMesh.Create();
            wall.name = "Wall " + i.ToString();
            wall.AddComponent<Wall>();
            wall.transform.SetParent(walls.transform, false);
            WallData data = new WallData(m_WallData);
            data.SetControlPoints(points);
            wall.GetComponent<Wall>().Initialize(data).Build();

            
        }
    }

    private void BuildFloor()
    {
        if (!IsElementActive(StoreyElement.Floor))
            return;

        ProBuilderMesh floor = ProBuilderMesh.Create();
        floor.name = "Floor";

        ControlPoint[] points = new ControlPoint[m_ControlPoints.Length];
        Array.Copy(m_ControlPoints, points, points.Length);

        //for (int i = 0; i < points.Length; i++)
        //{
        //    points[i] += points[i].Forward * (m_WallData.Depth * 0.5f);
        //}


        floor.AddComponent<Floor>().Initialize(new FloorData() { ControlPoints = points }).Build();

        floor.transform.SetParent(transform, false);
        floor.transform.localPosition = Vector3.zero;
        
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
