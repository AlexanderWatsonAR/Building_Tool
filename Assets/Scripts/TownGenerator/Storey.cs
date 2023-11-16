using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class Storey : MonoBehaviour, IBuildable
{
    [SerializeField] private StoreyData m_Data;
    private bool m_HasInitialized;
    
    List<Vector3[]> m_WallPoints;

    public StoreyData Data => m_Data;
    public bool HasInitialized => m_HasInitialized;

    public IBuildable Initialize(IData data)
    {
        m_Data = data as StoreyData;


        if (m_Data.WallData.Material == null)
        {
            m_Data.WallData.Material = BuiltinMaterials.defaultMaterial;
        }

        if (m_Data.PillarData.Material == null)
        {
            m_Data.PillarData.Material = BuiltinMaterials.defaultMaterial;
        }

        if (m_Data.FloorData.Material == null)
        {
            m_Data.FloorData.Material = BuiltinMaterials.defaultMaterial;
        }

        m_WallPoints = new();
        m_HasInitialized = true;

        return this;
    }

    public void Build()
    {
        transform.DeleteChildren();
        m_WallPoints.Clear();

        BuildPillars();
        BuildExternalWalls();
        BuildCorners();
        BuildFloor();
    }

    private void BuildPillars()
    {
        if (!m_Data.ActiveElements.IsElementActive(StoreyElement.Pillars))
            return;

        GameObject pillars = new GameObject("Pillars");
        pillars.transform.SetParent(transform, false);

        for (int i = 0; i < m_Data.ControlPoints.Length; i++)
        {
            ProBuilderMesh pillar = ProBuilderMesh.Create();
            pillar.name = "Pillar " + i.ToString();
            pillar.AddComponent<Pillar>().Initialize(m_Data.PillarData).Build();
            pillar.transform.SetParent(pillars.transform, false);
            pillar.transform.localPosition = m_Data.ControlPoints[i].Position;
            int index = m_Data.ControlPoints.GetNext(i);
            pillar.transform.forward = pillar.transform.localPosition.DirectionToTarget(m_Data.ControlPoints[index].Position);
        }
    }

    private void BuildCorners()
    {
        if (!m_Data.ActiveElements.IsElementActive(StoreyElement.Walls))
            return;

        if (m_WallPoints.Count != m_Data.ControlPoints.Length)
            return;

        GameObject corners = new GameObject("Corners");
        corners.transform.SetParent(transform, false);

        bool isConcave = m_Data.ControlPoints.IsConcave(out int[] concavePoints);

        WallData wallData = m_Data.WallData;

        for (int i = 0; i < m_WallPoints.Count; i++)
        {
            int current = i;
            int previous = m_Data.ControlPoints.GetPrevious(i);
            int next = m_Data.ControlPoints.GetNext(i);

            Vector3 dirA = m_WallPoints[current][0].DirectionToTarget(m_WallPoints[current][1]);
            //Vector3 crossA = Vector3.Cross(Vector3.up, dirA) * m_WallData.Depth;
            Vector3 crossA = Vector3.Cross(dirA, Vector3.up) * wallData.Depth;

            Vector3 dirB = m_WallPoints[previous][0].DirectionToTarget(m_WallPoints[previous][1]);
            //Vector3 crossB = Vector3.Cross(Vector3.up, dirB) * m_WallData.Depth;
            Vector3 crossB = Vector3.Cross(dirB, Vector3.up) * wallData.Depth;

            Vector3 intersection;

            Extensions.DoLinesIntersect(m_WallPoints[current][0] + crossA, m_WallPoints[current][1] + crossA, m_WallPoints[previous][0] + crossB, m_WallPoints[previous][1] + crossB, out intersection);

            int numberOfSamples = m_Data.CornerData.Sides + 1;

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

            switch (m_Data.CornerData.Type)
            {
                case CornerType.Point:
                    post.CreateShapeFromPolygon(cornerPoints, wallData.Height, false);
                    break;
                case CornerType.Round:
                    post.CreateShapeFromPolygon(points, 0, false);
                    Face[] extrudeFaces = post.Extrude(new Face[] { post.faces[0] }, ExtrudeMethod.FaceNormal, wallData.Height);
                    //Smoothing.ApplySmoothingGroups(post, extrudeFaces, 360);
                    break;
                case CornerType.Flat:
                    post.CreateShapeFromPolygon(flatPoints, wallData.Height, false);
                    break;
            }
                
            post.GetComponent<Renderer>().sharedMaterial = wallData.Material;
            post.ToMesh();
            post.Refresh();

            post.transform.SetParent(corners.transform, false);

        }
    }

    private void BuildExternalWalls()
    {
        if (!m_Data.ActiveElements.IsElementActive(StoreyElement.Walls))
            return;

        WallData wallData = m_Data.WallData;

        Vector3 h = wallData.Height * Vector3.up;

        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(transform, false);

        Vector3[] insidePoints = m_Data.InsidePoints;

        // Construct the walls 
        for (int i = 0; i < m_Data.ControlPoints.Length; i++)
        {
            int current = i;
            int next = m_Data.ControlPoints.GetNext(i);
            int previous = m_Data.ControlPoints.GetPrevious(i);

            Vector3 nextControlPoint = m_Data.ControlPoints[next].Position;
            Vector3 oneNextInside = insidePoints[next];
            Vector3 onePreviousInside = insidePoints[previous];

            Vector3 nextForward = insidePoints[i].DirectionToTarget(oneNextInside);
            //Vector3 nextRight = Vector3.Cross(Vector3.up, nextForward) * m_WallData.Depth;
            Vector3 nextRight = Vector3.Cross(nextForward, Vector3.up) * wallData.Depth;

            Vector3 previousForward = insidePoints[i].DirectionToTarget(onePreviousInside);
            //Vector3 previousRight = Vector3.Cross(previousForward, Vector3.up) * m_WallData.Depth;
            Vector3 previousRight = Vector3.Cross(Vector3.up, previousForward) * wallData.Depth;

            Vector3 bottomLeft = insidePoints[i];
            Vector3 topLeft = bottomLeft + h;
            Vector3 bottomRight = oneNextInside;
            Vector3 topRight = bottomRight + h;

            // Post Points
            Vector3 zero = m_Data.ControlPoints[i].Position;
            Vector3 one = insidePoints[i] + nextRight;
            Vector3 two = insidePoints[i];
            Vector3 three = insidePoints[i] + previousRight;

            bool isConcave = m_Data.ControlPoints.IsConcave(out int[] concavePoints);

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
                    bottomLeft = m_Data.ControlPoints[i].Position - nextRight;
                    topLeft = bottomLeft + h;
                }

            }

            m_WallPoints.Add(new Vector3[] { bottomLeft, bottomRight });

            Vector3[] points = new Vector3[] { bottomLeft, topLeft, topRight, bottomRight };
            

            ProBuilderMesh wall = ProBuilderMesh.Create();
            wall.name = "Wall " + i.ToString();
            wall.AddComponent<Wall>();
            wall.transform.SetParent(walls.transform, false);
            WallData data = new WallData(wallData);
            data.SetControlPoints(points);
            wall.GetComponent<Wall>().Initialize(data).Build();

            
        }
    }

    private void BuildFloor()
    {
        if (!m_Data.ActiveElements.IsElementActive(StoreyElement.Floor))
            return;

        ProBuilderMesh floor = ProBuilderMesh.Create();
        floor.name = "Floor";

        ControlPoint[] points = new ControlPoint[m_Data.ControlPoints.Length];
        Array.Copy(m_Data.ControlPoints, points, points.Length);

        floor.AddComponent<Floor>().Initialize(new FloorData() { ControlPoints = points }).Build();

        floor.transform.SetParent(transform, false);
        floor.transform.localPosition = Vector3.zero;
        
    }
}
