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

public class Storey : MonoBehaviour, IBuildable, IDataChangeEvent
{
    [SerializeField] StoreyDataWrapper m_Wrapper;

    //[SerializeReference] private StoreyData m_Data;
    //
    public event Action<IData> OnDataChange;

    public StoreyData Data => m_Wrapper.Data;

    public void OnDataChange_Invoke()
    {

        //OnDataChange.GetInvocationList()


        //OnDataChange?.Invoke(m_Data);
        //Debug.Log("Storey data change invoke");
    }

    public IBuildable Initialize(IData data)
    {
        m_Wrapper = new StoreyDataWrapper(data as StoreyData);

        if (Data.WallData.Material == null)
        {
            Data.WallData.Material = BuiltinMaterials.defaultMaterial;
        }

        if (Data.PillarData.Material == null)
        {
            Data.PillarData.Material = BuiltinMaterials.defaultMaterial;
        }

        if (Data.FloorData.Material == null)
        {
            Data.FloorData.Material = BuiltinMaterials.defaultMaterial;
        }

        return this;
    }

    #region Build
    public void Build()
    {
        //Building aBuilding = GetComponentInParent<Building>();

        //Debug.Log(ReferenceEquals(m_Data.Building, aBuilding.Data));

        transform.DeleteChildren();

        BuildPillars();
        BuildExternalWalls();
        BuildCorners();
        BuildFloor();

        OnDataChange_Invoke();
    }
    private void BuildPillars()
    {
        if (!Data.ActiveElements.IsElementActive(StoreyElement.Pillars))
            return;

        if(Data.Pillars == null || Data.Pillars.Length != Data.ControlPoints.Length)
        {
            Data.Pillars = new PillarData[Data.ControlPoints.Length];
        }


        GameObject pillars = new GameObject("Pillars");
        pillars.transform.SetParent(transform, false);

        for (int i = 0; i < m_Wrapper.Data.ControlPoints.Length; i++)
        {
            ProBuilderMesh pillarMesh = ProBuilderMesh.Create();
            pillarMesh.name = "Pillar " + i.ToString();
            pillarMesh.AddComponent<Pillar>();
            pillarMesh.transform.SetParent(pillars.transform, false);
            pillarMesh.transform.localPosition = Data.ControlPoints[i].Position;
            int index = m_Wrapper.Data.ControlPoints.GetNext(i);
            pillarMesh.transform.forward = pillarMesh.transform.localPosition.DirectionToTarget(Data.ControlPoints[index].Position);

            Pillar pillar = pillarMesh.GetComponent<Pillar>();
            Data.Pillars[i] ??= CalculatePillar(i);

            pillar.Initialize(Data.Pillars[i]).Build();
            pillar.OnDataChange += data =>
            { 
                PillarData pillarData = data as PillarData;
                Data.Pillars[pillarData.ID] = pillarData;
                OnDataChange_Invoke();
            };
        }
    }
    private void BuildCorners()
    {
        if (!Data.ActiveElements.IsElementActive(StoreyElement.Walls))
            return;

        GameObject corners = new GameObject("Corners");
        corners.transform.SetParent(transform, false);

        if(Data.Corners == null || Data.Corners.Length != Data.ControlPoints.Length)
        {
            Data.Corners = new CornerData[Data.ControlPoints.Length];
        }

        for (int i = 0; i < Data.ControlPoints.Length; i++)
        {
            ProBuilderMesh cornerMesh = ProBuilderMesh.Create();
            Corner corner = cornerMesh.AddComponent<Corner>();
            corner.name = "Corner " + i.ToString();
            corner.transform.SetParent(corners.transform, false);
            corner.GetComponent<Renderer>().sharedMaterial = BuiltinMaterials.defaultMaterial;

            Data.Corners[i] = CalculateCorner(i);
            corner.Initialize(Data.Corners[i]).Build();
            corner.OnDataChange += (CornerData data) => { Data.Corners[data.ID] = data; OnDataChange_Invoke(); };
        }
    }
    private void BuildExternalWalls()
    {
        if (!Data.ActiveElements.IsElementActive(StoreyElement.Walls))
            return;

        if(Data.Walls == null || Data.Walls.Length != Data.ControlPoints.Length)
        {
            Data.Walls = new WallData[Data.ControlPoints.Length];
        }

        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(transform, false);

        //Vector3[] insidePoints = m_Data.InsidePoints;

        // Construct the walls 
        for (int i = 0; i < Data.ControlPoints.Length; i++)
        {
            Data.Walls[i] ??= CalculateWall(i);

            Wall wall = BuildWall(Data.Walls[i]);
            wall.transform.SetParent(walls.transform, true);
        }
    }
    private Wall BuildWall(WallData data)
    {
        ProBuilderMesh wallMesh = ProBuilderMesh.Create();
        wallMesh.name = "Wall " + data.ID.ToString();
        Wall wall = wallMesh.AddComponent<Wall>(); 
        wall.Initialize(data).Build();
        wall.OnDataChange += (IData data) =>
        {
            WallData wallData = data as WallData;
            Data.Walls[wallData.ID] = wallData;
            OnDataChange_Invoke();
        };
        return wall;
    }
    private void BuildFloor()
    {
        if (!Data.ActiveElements.IsElementActive(StoreyElement.Floor))
            return;

        ProBuilderMesh floor = ProBuilderMesh.Create();
        
        ControlPoint[] points = new ControlPoint[Data.ControlPoints.Length];
        Array.Copy(Data.ControlPoints, points, points.Length);

        floor.AddComponent<Floor>().Initialize(new FloorData() { ControlPoints = points }).Build();

        floor.transform.SetParent(transform, false);
        floor.transform.localPosition = Vector3.zero;

    }
    #endregion

    #region Calculate
    private CornerData CalculateCorner(int cornerIndex)
    {
        bool isConcave = Data.ControlPoints.IsConcave(out int[] concavePoints);
        WallData wallData = Data.WallData;

        int current = cornerIndex;
        int previous = Data.ControlPoints.GetPrevious(current);
        int next = Data.ControlPoints.GetNext(current);


        Vector3 dirA = Data.Walls[current].StartPosition.DirectionToTarget(Data.Walls[current].EndPosition);
        Vector3 crossA = Vector3.Cross(dirA, Vector3.up) * wallData.Depth;

        Vector3 dirB = Data.Walls[previous].StartPosition.DirectionToTarget(Data.Walls[previous].EndPosition);
        Vector3 crossB = Vector3.Cross(dirB, Vector3.up) * wallData.Depth;

        Vector3 intersection;

        Extensions.DoLinesIntersect(Data.Walls[current].StartPosition + crossA, Data.Walls[current].EndPosition + crossA, Data.Walls[previous].StartPosition + crossB, Data.Walls[previous].EndPosition + crossB, out intersection);

        int numberOfSamples = Data.CornerData.Sides + 1;

        Vector3[] cornerPoints = new Vector3[] { Data.Walls[current].StartPosition, Data.Walls[current].StartPosition + crossA, Data.Walls[current].StartPosition + crossB, intersection };
        Vector3[] flatPoints = new Vector3[] { cornerPoints[0], cornerPoints[1], cornerPoints[2] };

        bool isInside = isConcave && concavePoints.Any(b => b == current);

        if (isInside)
        {
            Extensions.DoLinesIntersect(Data.Walls[current].StartPosition, Data.Walls[current].EndPosition, Data.Walls[previous].StartPosition, Data.Walls[previous].EndPosition, out intersection);
            cornerPoints = new Vector3[] { Data.Walls[current].StartPosition, Data.Walls[current].StartPosition + crossA, Data.Walls[previous].EndPosition, intersection };
            flatPoints = new Vector3[] { Data.Walls[current].StartPosition, Data.Walls[current].StartPosition + crossA, Data.Walls[previous].EndPosition };
        }

        CornerData cornerData = new CornerData(Data.CornerData)
        {
            CornerPoints = cornerPoints,
            FlatPoints = flatPoints.SortPointsClockwise().ToArray(),
            ID = current,
            IsInside = isInside,
            Height = Data.WallData.Height
        };

        return cornerData;
    }
    private WallData CalculateWall(int wallIndex)
    {
        WallData wallData = Data.WallData;
        wallData.Sections = new WallSectionData[wallData.Columns, wallData.Rows];

        Data.WallPoints = new WallPoints[Data.ControlPoints.Length];

        int current = wallIndex;
        int next = Data.ControlPoints.GetNext(current);

        Vector3 nextDir = Data.ControlPoints[current].DirectionToTarget(Data.ControlPoints[next]);
        Vector3 wallForward = Vector3.Cross(nextDir, Vector3.up) * wallData.Depth;

        ControlPoint start = new ControlPoint(Data.ControlPoints[current]);
        ControlPoint end = new ControlPoint(Data.ControlPoints[next]);

        bool isConcave = Data.ControlPoints.IsConcave(out int[] concavePoints);

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
        PillarData data = new PillarData(Data.PillarData)
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
