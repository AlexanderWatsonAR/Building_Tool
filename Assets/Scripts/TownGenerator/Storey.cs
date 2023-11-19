using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class Storey : MonoBehaviour, IBuildable
{
    [SerializeField] private StoreyData m_Data;
    
    [SerializeField] List<Vector3[]> m_WallPoints;

    public event Action<StoreyData> OnDataChange;

    public StoreyData Data => m_Data;

    public void OnDataChange_Invoke()
    {
        OnDataChange?.Invoke(m_Data);
    }

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
            ProBuilderMesh pillarMesh = ProBuilderMesh.Create();
            pillarMesh.name = "Pillar " + i.ToString();
            pillarMesh.AddComponent<Pillar>().Initialize(m_Data.PillarData).Build();
            pillarMesh.transform.SetParent(pillars.transform, false);
            pillarMesh.transform.localPosition = m_Data.ControlPoints[i].Position;
            int index = m_Data.ControlPoints.GetNext(i);
            pillarMesh.transform.forward = pillarMesh.transform.localPosition.DirectionToTarget(m_Data.ControlPoints[index].Position);

            Pillar pillar = pillarMesh.GetComponent<Pillar>();
            pillar.OnDataChange += (PillarData data) => { m_Data.Pillars[data.ID] = data; };
        }
    }

    private void BuildCorners()
    {
        if (!m_Data.ActiveElements.IsElementActive(StoreyElement.Walls))
            return;

        if (m_WallPoints.Count != m_Data.ControlPoints.Length)
            return;

        m_Data.Corners = new CornerData[m_Data.ControlPoints.Length];

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
            Vector3 crossA = Vector3.Cross(dirA, Vector3.up) * wallData.Depth;

            Vector3 dirB = m_WallPoints[previous][0].DirectionToTarget(m_WallPoints[previous][1]);
            Vector3 crossB = Vector3.Cross(dirB, Vector3.up) * wallData.Depth;

            Vector3 intersection;

            Extensions.DoLinesIntersect(m_WallPoints[current][0] + crossA, m_WallPoints[current][1] + crossA, m_WallPoints[previous][0] + crossB, m_WallPoints[previous][1] + crossB, out intersection);

            int numberOfSamples = m_Data.CornerData.Sides + 1;

            Vector3[] cornerPoints = new Vector3[] { m_WallPoints[current][0], m_WallPoints[current][0] + crossA, m_WallPoints[current][0] + crossB, intersection };
            Vector3[] flatPoints = new Vector3[] { cornerPoints[0], cornerPoints[1], cornerPoints[2] };

            bool isInside = isConcave && concavePoints.Any(b => b == current);

            if (isInside)
            {
                Extensions.DoLinesIntersect(m_WallPoints[current][0], m_WallPoints[current][1], m_WallPoints[previous][0], m_WallPoints[previous][1], out intersection);
                cornerPoints = new Vector3[] { m_WallPoints[current][0], m_WallPoints[current][0] + crossA, m_WallPoints[previous][1], intersection };
                flatPoints = new Vector3[] { m_WallPoints[current][0], m_WallPoints[current][0] + crossA, m_WallPoints[previous][1] };
            }

            ProBuilderMesh cornerMesh = ProBuilderMesh.Create();
            Corner corner = cornerMesh.AddComponent<Corner>();
            corner.name = "Corner " + current.ToString();
            corner.transform.SetParent(corners.transform, false);
            corner.GetComponent<Renderer>().sharedMaterial = wallData.Material;

            CornerData cornerData = new CornerData(m_Data.CornerData)
            {
                CornerPoints = cornerPoints,
                FlatPoints = flatPoints.SortPointsClockwise().ToArray(),
                ID = current,
                IsInside = isInside,
                Height = m_Data.WallData.Height
            };

            corner.Initialize(cornerData).Build();
            corner.OnDataChange += (CornerData data) => { m_Data.Corners[data.ID] = data; };
        }
    }

    private void BuildExternalWalls()
    {
        if (!m_Data.ActiveElements.IsElementActive(StoreyElement.Walls))
            return;

        //if (m_Data.Walls.Length > 0)
        //    return;

        m_Data.Walls = new WallData[m_Data.ControlPoints.Length];

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
            

            ProBuilderMesh wallMesh = ProBuilderMesh.Create();
            wallMesh.name = "Wall " + i.ToString();
            wallMesh.AddComponent<Wall>();
            wallMesh.transform.SetParent(walls.transform, false);

            Wall wall = wallMesh.GetComponent<Wall>();
            
            WallData data = new WallData(wallData)
            {
                ID = i,
                ControlPoints = points
            };
            
            m_Data.Walls[i] = wallData;
            wall.Initialize(data).Build();
            wall.OnDataChange += (WallData data) => { m_Data.Walls[data.ID] = data; };
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
