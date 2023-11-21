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
using static UnityEngine.UI.GridLayoutGroup;

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
        m_WallPoints?.Clear();

        BuildPillars();
        BuildExternalWalls();
        BuildCorners();
        BuildFloor();

        OnDataChange_Invoke();
    }

    private void BuildPillars()
    {
        if (!m_Data.ActiveElements.IsElementActive(StoreyElement.Pillars))
            return;

        m_Data.Pillars ??= new PillarData[m_Data.ControlPoints.Length];

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
            m_Data.Pillars[i] = CalculatePillar(i);

            pillar.Initialize(m_Data.Pillars[i]).Build();
            pillar.OnDataChange += (PillarData data) => { m_Data.Pillars[data.ID] = data; OnDataChange_Invoke(); };
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

        m_Data.Corners ??= new CornerData[m_Data.ControlPoints.Length];

        for (int i = 0; i < m_Data.ControlPoints.Length; i++)
        {
            ProBuilderMesh cornerMesh = ProBuilderMesh.Create();
            Corner corner = cornerMesh.AddComponent<Corner>();
            corner.name = "Corner " + i.ToString();
            corner.transform.SetParent(corners.transform, false);
            corner.GetComponent<Renderer>().sharedMaterial = BuiltinMaterials.defaultMaterial;

            m_Data.Corners[i] = CalculateCorner(i);
            corner.Initialize(m_Data.Corners[i]).Build();
            corner.OnDataChange += (CornerData data) => { m_Data.Corners[data.ID] = data; OnDataChange_Invoke(); };
        }
    }
    private void BuildExternalWalls()
    {
        if (!m_Data.ActiveElements.IsElementActive(StoreyElement.Walls))
            return;

        m_Data.Walls ??= new WallData[m_Data.ControlPoints.Length];

        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(transform, false);

        Vector3[] insidePoints = m_Data.InsidePoints;

        // Construct the walls 
        for (int i = 0; i < m_Data.ControlPoints.Length; i++)
        {
            ProBuilderMesh wallMesh = ProBuilderMesh.Create();
            wallMesh.name = "Wall " + i.ToString();
            wallMesh.AddComponent<Wall>();
            wallMesh.transform.SetParent(walls.transform, false);

            Wall wall = wallMesh.GetComponent<Wall>();

            m_Data.Walls[i] = CalculateWall(i);

            wall.Initialize(m_Data.Walls[i]).Build();
            wall.OnDataChange += (WallData data) => { m_Data.Walls[data.ID] = data; OnDataChange_Invoke(); };
        }
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

    #region Calculate
    private CornerData CalculateCorner(int cornerIndex)
    {
        bool isConcave = m_Data.ControlPoints.IsConcave(out int[] concavePoints);
        WallData wallData = m_Data.WallData;

        int current = cornerIndex;
        int previous = m_Data.ControlPoints.GetPrevious(current);
        int next = m_Data.ControlPoints.GetNext(current);

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

        Vector3 h = wallData.Height * Vector3.up;

        Vector3[] insidePoints = m_Data.InsidePoints;

        int current = wallIndex;
        int next = m_Data.ControlPoints.GetNext(current);
        int previous = m_Data.ControlPoints.GetPrevious(current);

        Vector3 nextControlPoint = m_Data.ControlPoints[next].Position;
        Vector3 oneNextInside = insidePoints[next];
        Vector3 onePreviousInside = insidePoints[previous];

        Vector3 nextForward = insidePoints[current].DirectionToTarget(oneNextInside);
        //Vector3 nextRight = Vector3.Cross(Vector3.up, nextForward) * m_WallData.Depth;
        Vector3 nextRight = Vector3.Cross(nextForward, Vector3.up) * wallData.Depth;

        Vector3 previousForward = insidePoints[current].DirectionToTarget(onePreviousInside);
        //Vector3 previousRight = Vector3.Cross(previousForward, Vector3.up) * m_WallData.Depth;
        Vector3 previousRight = Vector3.Cross(Vector3.up, previousForward) * wallData.Depth;

        Vector3 bottomLeft = insidePoints[current];
        Vector3 topLeft = bottomLeft + h;
        Vector3 bottomRight = oneNextInside;
        Vector3 topRight = bottomRight + h;

        // Post Points
        Vector3 zero = m_Data.ControlPoints[current].Position;
        Vector3 one = insidePoints[current] + nextRight;
        Vector3 two = insidePoints[current];
        Vector3 three = insidePoints[current] + previousRight;

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
                bottomLeft = m_Data.ControlPoints[current].Position - nextRight;
                topLeft = bottomLeft + h;
            }

        }

        m_WallPoints.Add(new Vector3[] { bottomLeft, bottomRight });

        Vector3[] points = new Vector3[] { bottomLeft, topLeft, topRight, bottomRight };

        WallData data = new WallData(wallData)
        {
            ID = wallIndex,
            ControlPoints = points
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
}
