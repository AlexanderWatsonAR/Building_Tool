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
    [SerializeReference] private StoreyData m_Data;

    public StoreyData Data => m_Data;

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

        return this;
    }

    #region Build
    public void Build()
    {
        transform.DeleteChildren();

        BuildPillars();
        BuildExternalWalls();
        BuildCorners();
        BuildFloor();
    }
    private void BuildPillars()
    {
        if (!m_Data.ActiveElements.IsElementActive(StoreyElement.Pillars))
            return;

        if(m_Data.Pillars == null || m_Data.Pillars.Length != m_Data.ControlPoints.Length)
        {
            m_Data.Pillars = new PillarData[m_Data.ControlPoints.Length];
        }

        GameObject pillars = new GameObject("Pillars");
        pillars.transform.SetParent(transform, false);

        for (int i = 0; i < m_Data.ControlPoints.Length; i++)
        {
            ProBuilderMesh pillarMesh = ProBuilderMesh.Create();
            pillarMesh.name = "Pillar " + i.ToString();
            pillarMesh.AddComponent<Pillar>();
            pillarMesh.transform.SetParent(pillars.transform, false);
            pillarMesh.transform.localPosition = m_Data.ControlPoints[i].Position;
            int index = m_Data.ControlPoints.GetNext(i);
            pillarMesh.transform.forward = pillarMesh.transform.localPosition.DirectionToTarget(m_Data.ControlPoints[index].Position);

            Pillar pillar = pillarMesh.GetComponent<Pillar>();
            m_Data.Pillars[i] ??= CalculatePillar(i);

            pillar.Initialize(m_Data.Pillars[i]).Build();
        }
    }
    private void BuildCorners()
    {
        if (!m_Data.ActiveElements.IsElementActive(StoreyElement.Walls))
            return;

        GameObject corners = new GameObject("Corners");
        corners.transform.SetParent(transform, false);

        if(m_Data.Corners == null || m_Data.Corners.Length != m_Data.ControlPoints.Length)
        {
            m_Data.Corners = new CornerData[m_Data.ControlPoints.Length];
        }

        for (int i = 0; i < m_Data.ControlPoints.Length; i++)
        {
            ProBuilderMesh cornerMesh = ProBuilderMesh.Create();
            Corner corner = cornerMesh.AddComponent<Corner>();
            corner.name = "Corner " + i.ToString();
            corner.transform.SetParent(corners.transform, false);
            corner.GetComponent<Renderer>().sharedMaterial = BuiltinMaterials.defaultMaterial;

            m_Data.Corners[i] = CalculateCorner(i);
            corner.Initialize(m_Data.Corners[i]).Build();
        }
    }
    private void BuildExternalWalls()
    {
        if (!m_Data.ActiveElements.IsElementActive(StoreyElement.Walls))
            return;

        if(m_Data.Walls == null || m_Data.Walls.Length != m_Data.ControlPoints.Length)
        {
            m_Data.Walls = new WallData[m_Data.ControlPoints.Length];
        }

        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(transform, false);

        //Vector3[] insidePoints = m_Data.InsidePoints;

        // Construct the walls 
        for (int i = 0; i < m_Data.ControlPoints.Length; i++)
        {
            m_Data.Walls[i] ??= CalculateWall(i);

            Wall wall = BuildWall(m_Data.Walls[i]);
            wall.transform.SetParent(walls.transform, true);
        }
    }
    private Wall BuildWall(WallData data)
    {
        ProBuilderMesh wallMesh = ProBuilderMesh.Create();
        wallMesh.name = "Wall " + data.ID.ToString();
        Wall wall = wallMesh.AddComponent<Wall>(); 
        wall.Initialize(data).Build();
        return wall;
    }
    private void BuildFloor()
    {
        if (!m_Data.ActiveElements.IsElementActive(StoreyElement.Floor))
            return;

        ProBuilderMesh floor = ProBuilderMesh.Create();
        
        ControlPoint[] points = new ControlPoint[m_Data.ControlPoints.Length];
        Array.Copy(m_Data.ControlPoints, points, points.Length);

        floor.AddComponent<Floor>().Initialize(new FloorData() { ControlPoints = points }).Build();

        floor.transform.SetParent(transform, false);
        floor.transform.localPosition = Vector3.zero;

    }
    #endregion

    #region Calculate
    private CornerData CalculateCorner(int cornerIndex)
    {
        bool isConcave = m_Data.ControlPoints.IsConcave(out int[] concavePoints);
        WallData wallData = m_Data.WallData;

        int current = cornerIndex;
        int previous = m_Data.ControlPoints.GetPrevious(current);
        int next = m_Data.ControlPoints.GetNext(current);


        Vector3 dirA = m_Data.Walls[current].StartPosition.DirectionToTarget(m_Data.Walls[current].EndPosition);
        Vector3 crossA = Vector3.Cross(dirA, Vector3.up) * wallData.Depth;

        Vector3 dirB = m_Data.Walls[previous].StartPosition.DirectionToTarget(m_Data.Walls[previous].EndPosition);
        Vector3 crossB = Vector3.Cross(dirB, Vector3.up) * wallData.Depth;

        Vector3 intersection;

        Extensions.DoLinesIntersect(m_Data.Walls[current].StartPosition + crossA, m_Data.Walls[current].EndPosition + crossA, m_Data.Walls[previous].StartPosition + crossB, m_Data.Walls[previous].EndPosition + crossB, out intersection);

        int numberOfSamples = m_Data.CornerData.Sides + 1;

        Vector3[] cornerPoints = new Vector3[] { m_Data.Walls[current].StartPosition, m_Data.Walls[current].StartPosition + crossA, m_Data.Walls[current].StartPosition + crossB, intersection };
        Vector3[] flatPoints = new Vector3[] { cornerPoints[0], cornerPoints[1], cornerPoints[2] };

        bool isInside = isConcave && concavePoints.Any(b => b == current);

        if (isInside)
        {
            Extensions.DoLinesIntersect(m_Data.Walls[current].StartPosition, m_Data.Walls[current].EndPosition, m_Data.Walls[previous].StartPosition, m_Data.Walls[previous].EndPosition, out intersection);
            cornerPoints = new Vector3[] { m_Data.Walls[current].StartPosition, m_Data.Walls[current].StartPosition + crossA, m_Data.Walls[previous].EndPosition, intersection };
            flatPoints = new Vector3[] { m_Data.Walls[current].StartPosition, m_Data.Walls[current].StartPosition + crossA, m_Data.Walls[previous].EndPosition };
        }

        CornerData cornerData = new CornerData(m_Data.CornerData)
        {
            CornerPoints = cornerPoints,
            FlatPoints = flatPoints.SortPointsClockwise().ToArray(),
            ID = current,
            IsInside = isInside,
            Height = m_Data.WallData.Height
        };

        return cornerData;
    }
    private WallData CalculateWall(int wallIndex)
    {
        WallData wallData = m_Data.WallData;
        wallData.Sections = new WallSectionData[wallData.Columns, wallData.Rows];

        m_Data.WallPoints = new WallPoints[m_Data.ControlPoints.Length];

        int current = wallIndex;
        int next = m_Data.ControlPoints.GetNext(current);

        Vector3 nextDir = m_Data.ControlPoints[current].DirectionToTarget(m_Data.ControlPoints[next]);
        Vector3 wallForward = Vector3.Cross(nextDir, Vector3.up) * wallData.Depth;

        ControlPoint start = new ControlPoint(m_Data.ControlPoints[current]);
        ControlPoint end = new ControlPoint(m_Data.ControlPoints[next]);

        bool isConcave = m_Data.ControlPoints.IsConcave(out int[] concavePoints);

        if (isConcave)
        {
            bool conditionA = concavePoints.Any(a => a == next);
            bool conditionB = concavePoints.Any(b => b == current);

            if (conditionA)
            {
                end.SetForward(-wallForward);
            }
            if (conditionB)
            {
                start.SetForward(-wallForward);
            }
        }

        WallData data = new WallData(wallData)
        {
            ID = wallIndex,
            Start = start,
            End = end,
            Normal = wallForward.normalized
        };

        return data;
    }
    private PillarData CalculatePillar(int pillarIndex)
    {
        PillarData data = new PillarData(m_Data.PillarData)
        {
            ID = pillarIndex
        };

        return data;
    }
    #endregion

    public void Demolish()
    {

    }


}
