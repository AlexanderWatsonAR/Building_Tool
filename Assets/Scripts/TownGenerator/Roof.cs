using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using Vertex = UnityEngine.ProBuilder.Vertex;
using Rando = UnityEngine.Random;
using ProMaths = UnityEngine.ProBuilder.Math;
using UnityEngine.ProBuilder.MeshOperations;
using Unity.VisualScripting.FullSerializer;
using UnityEngine.Assertions.Must;

public class Roof : MonoBehaviour, IBuildable
{
    [SerializeField] private RoofData m_Data;

    public RoofData Data => m_Data;

    public event Action<RoofData> OnDataChange; // Building should sub to this.

    public void OnDataChange_Invoke()
    {
        OnDataChange?.Invoke(m_Data);
    }
    public IBuildable Initialize(IData data)
    {
        m_Data = data as RoofData;
        m_Data.TileData.Material ??= BuiltinMaterials.defaultMaterial;
        return this;
    }
    public void Build()
    {
        transform.DeleteChildren();

        if (!m_Data.IsActive)
            return;

        switch (m_Data.RoofType)
        {
            case RoofType.Gable:
                BuildGable();
                break;
            case RoofType.Mansard:
                BuildMansard();
                break;
            case RoofType.Dormer:
                BuildMansard();
                BuildGable();
                break;
            case RoofType.MShaped:
                BuildM();
                break;
            case RoofType.Pyramid:
                BuildPyramid();
                break;
            case RoofType.PyramidHip:
                BuildMansard();
                BuildPyramid();
                break;
        }

        OnDataChange_Invoke();
    }
    #region Calculate
    private RoofTileData CalculatePyramid(int index)
    {
        Vector3 middle = ProMaths.Average(m_Data.ControlPoints.GetPositions());

        int next = m_Data.ControlPoints.GetNext(index);

        bool extendHeightEnd = m_Data.RoofType != RoofType.PyramidHip;

        LerpPoint bottomLeft = new (m_Data.ControlPoints[index].Position);
        LerpPoint topLeft = new (middle);
        LerpPoint topRight = new (middle);
        LerpPoint bottomRight = new (m_Data.ControlPoints[next].Position);

        RoofTileData data = new RoofTileData(m_Data.TileData)
        {
            ID = index,
            Height = m_Data.PyramidHeight,
            ControlPoints = new LerpPoint[] { bottomLeft, topLeft, topRight, bottomRight },
            ExtendHeightBeginning = false,
            ExtendHeightEnd = extendHeightEnd,
            ExtendWidthBeginning = false,
            ExtendWidthEnd = false
        };

        return data;
    }
    private RoofTileData CalculateMansard(int index)
    {
        //ControlPoint[] scaledControlPoints = m_Data.ScaledControlPoints;
        int next = m_Data.ControlPoints.GetNext(index);

        bool isConvex = m_Data.IsConvex;
        Vector3[] positions = m_Data.ControlPoints.GetPositions();
        Vector3 middle = ProMaths.Average(positions);

        LerpPoint bottomLeft = new LerpPoint(m_Data.ControlPoints[index].Position);

        Vector3 tLEnd = isConvex ? middle : m_Data.ControlPoints[index].Position + (m_Data.ControlPoints[index].Forward * 2);
        Vector3 tREnd = isConvex ? middle : m_Data.ControlPoints[next].Position + (m_Data.ControlPoints[next].Forward * 2);

        LerpPoint topLeft = new LerpPoint(m_Data.ControlPoints[index].Position, tLEnd, m_Data.MansardScale);
        LerpPoint topRight = new LerpPoint(m_Data.ControlPoints[next].Position, tREnd, m_Data.MansardScale);

        LerpPoint bottomRight = new LerpPoint(m_Data.ControlPoints[next].Position);

        RoofTileData data = new RoofTileData(m_Data.TileData)
        {
            ID = index,
            ControlPoints = new LerpPoint[] { bottomLeft, topLeft, topRight, bottomRight },
            Height = m_Data.MansardHeight,
            ExtendHeightBeginning = false,
            ExtendHeightEnd = true,
            ExtendWidthBeginning = false,
            ExtendWidthEnd = false
        };

        return data;
    }
    private RoofTileData CalculateGableTile(int index)
    {
        ushort[] roofTileIndices = m_Data.GableData.indices[index];
        bool[] roofTileExtend = m_Data.GableData.extend[index];

        Vector3[] oneLine = new Vector3[m_Data.OneLine.Length];
        Array.Copy(m_Data.OneLine, oneLine, oneLine.Length);

        Vector3[] scaledOneLine = new Vector3[oneLine.Length];
        Array.Copy(oneLine, scaledOneLine, oneLine.Length);

        scaledOneLine.ScaleOneLine(m_Data.OneLineShape, 1);

        int[] relIndices = m_Data.PathPoints.RelativeIndices(m_Data.ShapeIndex);

        float gableScale = m_Data.IsOpen ? 1 : m_Data.GableScale;

        LerpPoint bottomLeft = new LerpPoint(m_Data.PathPoints[relIndices[roofTileIndices[0]]].Position);
        LerpPoint topLeft = new LerpPoint(scaledOneLine[roofTileIndices[1]], oneLine[roofTileIndices[1]], gableScale);
        LerpPoint topRight = new LerpPoint(scaledOneLine[roofTileIndices[2]], oneLine[roofTileIndices[2]], gableScale);
        LerpPoint bottomRight = new LerpPoint(m_Data.PathPoints[relIndices[roofTileIndices[3]]].Position);

        bool extendHeightEnd = m_Data.RoofType == RoofType.Dormer ? false : roofTileExtend[1];

        // Question: Is extending the width at the beginning & end always false when is open == false?
        bool extendWidthBeginning = m_Data.IsOpen && roofTileExtend[2];
        bool extendWidthEnd = m_Data.IsOpen && roofTileExtend[3];

        RoofTileData data = new RoofTileData(m_Data.TileData)
        {
            ID = index,
            Height = m_Data.GableHeight,
            ControlPoints = new LerpPoint[] { bottomLeft, topLeft, topRight, bottomRight },
            ExtendHeightBeginning = roofTileExtend[0],
            ExtendHeightEnd = extendHeightEnd,
            ExtendWidthBeginning = extendWidthBeginning,
            ExtendWidthEnd = extendWidthEnd
        };

        return data;
    }
    private WallData CalculateGableWall(int index)
    {
        if (!m_Data.AvailableFrames.Contains((int)RoofType.Gable))
            return null;

        ushort[] wallIndices = m_Data.GableData.indices[index].Reverse().ToArray();

        ControlPoint start = new ControlPoint(m_Data.PathPoints[wallIndices[0]]);
        start.SetForward(Vector3.zero);
        ControlPoint end = new ControlPoint(m_Data.PathPoints[wallIndices[^1]]);
        end.SetForward(Vector3.zero);

        //Vector3 dir = start.DirectionToTarget(end);
        //start += dir * 0.5f;
        //end -= dir * 0.5f;

        WallData data = new WallData()
        {
            ID = index,
            Height = m_Data.GableHeight,
            Material = BuiltinMaterials.defaultMaterial,
            IsTriangle = true,
            Start = start,
            End = end,
        };

        return data;
    }
    #endregion

    #region Build
    private void BuildPyramid()
    {
        if(m_Data.PyramidTiles == null || m_Data.PyramidTiles.Length == 0)
            m_Data.PyramidTiles = new RoofTileData[m_Data.ControlPoints.Length];

        for (int i = 0; i < m_Data.PyramidTiles.Length; i++)
        {
            if (m_Data.PyramidTiles[i] == null || m_Data.PyramidTiles[i].ControlPoints == null || m_Data.PyramidTiles[i].ControlPoints.Length == 0)
                m_Data.PyramidTiles[i] ??= CalculatePyramid(i);

            RoofTile pyramidTile = CreateRoofTile(m_Data.PyramidTiles[i]);
            pyramidTile.OnDataChange += data => { m_Data.PyramidTiles[data.ID] = data; OnDataChange_Invoke(); };
        }
    }
    private void BuildMansard()
    {
        m_Data.MansardTiles ??= new RoofTileData[m_Data.ControlPoints.Length];

        for (int i = 0; i < m_Data.MansardTiles.Length; i++)
        {
            m_Data.MansardTiles[i] ??= CalculateMansard(i);

            RoofTile mansardTile = CreateRoofTile(m_Data.MansardTiles[i]);
            mansardTile.OnDataChange += data => { m_Data.MansardTiles[data.ID] = data; OnDataChange_Invoke(); };
        }

        if (m_Data.RoofType == RoofType.Mansard)
        {
            ProBuilderMesh top = ProBuilderMesh.Create();
            top.name = "Lid";
            List<Vector3> lidPoints = new (m_Data.MansardTiles.Length);
            foreach(RoofTileData tile in m_Data.MansardTiles)
            {
                lidPoints.Add(tile.TopLeft);
            }
            top.CreateShapeFromPolygon(lidPoints, m_Data.TileData.Thickness, false);
            top.GetComponent<Renderer>().sharedMaterial = m_Data.TileData.Material;
            top.transform.SetParent(transform, false);
        }
    }
    private void BuildGable()
    {
        if (!m_Data.AvailableFrames.Contains((int)RoofType.Gable))
            return;

        if (m_Data.GableTiles == null || m_Data.GableTiles.Length == 0)
            m_Data.GableTiles = new RoofTileData[m_Data.PathPoints.Length];

        for(int i = 0; i < m_Data.GableTiles.Length; i++)
        {
            if (m_Data.GableTiles[i] == null || m_Data.GableTiles[i].ControlPoints == null ||  m_Data.GableTiles[i].ControlPoints.Length == 0)
                m_Data.GableTiles[i] = CalculateGableTile(i);
        }

        if(m_Data.Walls == null || m_Data.Walls.Length == 0)
            m_Data.Walls = new WallData[m_Data.GableData.wallIndices.Length];

        for (int i = 0; i < m_Data.Walls.Length; i++)
        {
            m_Data.Walls[i] ??= CalculateGableWall(m_Data.GableData.wallIndices[i]);
        }

        for (int i = 0; i < m_Data.GableTiles.Length; i++)
        {
            bool condition = m_Data.GableData.wallIndices.Any(x => x == i);

            if (condition && m_Data.IsOpen)
            {
                GameObject wallGO = new GameObject("Wall", typeof(Wall));
                wallGO.transform.SetParent(transform, false);
                Wall wall = wallGO.GetComponent<Wall>();
                wall.Initialize(m_Data.GetWallByID(i)).Build();
                wall.OnDataChange += data => { m_Data.Walls[data.ID] = data; OnDataChange_Invoke(); };
                continue;
            }

            RoofTile gableTile = CreateRoofTile(m_Data.GableTiles[i]);

            gableTile.OnDataChange += data => { m_Data.GableTiles[data.ID] = data; OnDataChange_Invoke(); };
        }

    }
    #endregion
    private void BuildM()
    {
        //Vector3[] mPointsA = new Vector3[4];
        //Vector3[] mPointsB = new Vector3[4];

        //if (m_Data.IsFlipped)
        //{
        //    mPointsA[1] = m_Data.ControlPoints[0].Position;
        //    mPointsA[2] = m_Data.ControlPoints[1].Position;

        //    mPointsB[0] = m_Data.ControlPoints[3].Position;
        //    mPointsB[3] = m_Data.ControlPoints[2].Position;

        //    Vector3 midA = Vector3.Lerp(m_Data.ControlPoints[0].Position, m_Data.ControlPoints[3].Position, 0.5f);
        //    Vector3 midB = Vector3.Lerp(m_Data.ControlPoints[1].Position, m_Data.ControlPoints[2].Position, 0.5f);

        //    Vector3 dirA = m_Data.ControlPoints[0].Position.DirectionToTarget(m_Data.ControlPoints[3].Position);
        //    Vector3 dirB = m_Data.ControlPoints[1].Position.DirectionToTarget(m_Data.ControlPoints[2].Position);

        //    mPointsA[0] = midA - (dirA * m_Data.TileData.Height);
        //    mPointsA[3] = midB - (dirB * m_Data.TileData.Height);

        //    mPointsB[1] = midA + (dirA * m_Data.TileData.Height);
        //    mPointsB[2] = midB + (dirB * m_Data.TileData.Height);

        //}
        //else
        //{
        //    mPointsA[0] = m_Data.ControlPoints[0].Position;
        //    mPointsA[3] = m_Data.ControlPoints[3].Position;

        //    mPointsB[1] = m_Data.ControlPoints[1].Position;
        //    mPointsB[2] = m_Data.ControlPoints[2].Position;

        //    Vector3 dirA = mPointsA[0].DirectionToTarget(mPointsB[1]);
        //    Vector3 dirB = mPointsA[3].DirectionToTarget(mPointsB[2]);

        //    Vector3 midA = Vector3.Lerp(mPointsA[0], mPointsB[1], 0.5f);
        //    Vector3 midB = Vector3.Lerp(mPointsA[3], mPointsB[2], 0.5f);

        //    mPointsA[1] = midA - (dirA * m_Data.TileData.Height);
        //    mPointsA[2] = midB - (dirB * m_Data.TileData.Height);

        //    mPointsB[0] = midA + (dirA * m_Data.TileData.Height);
        //    mPointsB[3] = midB + (dirB * m_Data.TileData.Height);
        //}

        //List<Vector3[]> mArrays = new List<Vector3[]>();
        //mArrays.Add(mPointsA);
        //mArrays.Add(mPointsB);

        //int iterator = 0;
        //foreach (Vector3[] m in mArrays)
        //{
        //    if (m.IsPolygonDescribableInOneLine(out Vector3[] oneLine))
        //    {
        //        Vector3 start = oneLine[0] + (Vector3.up * m_Data.GableHeight);
        //        Vector3 end = oneLine[1] + (Vector3.up * m_Data.GableHeight);

        //        if (m_Data.IsFlipped)
        //        {
        //            if (iterator == 0)
        //            {
        //                CreateRoofTile(new Vector3[] { m[1], start, end, m[2] });
        //                CreateRoofTile(new Vector3[] { m[3], end, start, m[0] }, false, false);

        //            }

        //            if (iterator == mArrays.Count - 1)
        //            {
        //                CreateRoofTile(new Vector3[] { m[1], start, end, m[2] }, false, false);
        //                CreateRoofTile(new Vector3[] { m[3], end, start, m[0] });
        //            }
        //        }
        //        else
        //        {
        //            if (iterator == 0)
        //            {
        //                CreateRoofTile(new Vector3[] { m[3], end, start, m[0] });
        //                CreateRoofTile(new Vector3[] { m[1], start, end, m[2] }, false, false);
        //            }

        //            if (iterator == mArrays.Count - 1)
        //            {
        //                CreateRoofTile(new Vector3[] { m[3], end, start, m[0] }, false, false);
        //                CreateRoofTile(new Vector3[] { m[1], start, end, m[2] });
        //            }
        //        }

        //    }

        //    iterator++;
        //}
    }
    private RoofTile CreateRoofTile(RoofTileData data)
    {
        ProBuilderMesh probuilderMesh = ProBuilderMesh.Create();
        probuilderMesh.transform.SetParent(transform, false);
        RoofTile tile = probuilderMesh.AddComponent<RoofTile>();
        
        tile.Initialize(data).Build();
        return tile;
    }


    

}
