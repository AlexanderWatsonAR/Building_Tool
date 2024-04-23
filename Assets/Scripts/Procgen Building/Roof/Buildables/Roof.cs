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
using OnlyInvalid.ProcGenBuilding.Common;
using OnlyInvalid.ProcGenBuilding.Wall;

namespace OnlyInvalid.ProcGenBuilding.Roof
{
    public class Roof : Buildable
    {
        [SerializeReference] RoofData m_RoofData;

        public override Buildable Initialize(DirtyData data)
        {
            base.Initialize(data);
            m_RoofData = data as RoofData;
            m_RoofData.TileData.Material ??= BuiltinMaterials.defaultMaterial;
            return this;
        }
        public override void Build()
        {
            transform.DeleteChildren();

            if (!m_RoofData.IsActive)
                return;

            switch (m_RoofData.RoofType)
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
                    break;
                case RoofType.Pyramid:
                    BuildPyramid();
                    break;
                case RoofType.PyramidHip:
                    BuildMansard();
                    BuildPyramid();
                    break;
            }
        }
        #region Calculate
        private RoofTileData CalculatePyramid(int index)
        {
            Vector3 middle = ProMaths.Average(m_RoofData.ControlPoints.GetPositions());

            int next = m_RoofData.ControlPoints.GetNext(index);

            bool extendHeightEnd = m_RoofData.RoofType != RoofType.PyramidHip;

            LerpPoint bottomLeft = new(m_RoofData.ControlPoints[index].Position);
            LerpPoint topLeft = new(middle);
            LerpPoint topRight = new(middle);
            LerpPoint bottomRight = new(m_RoofData.ControlPoints[next].Position);

            RoofTileData data = new RoofTileData(m_RoofData.TileData)
            {
                ID = index,
                Height = m_RoofData.PyramidHeight,
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
            int next = m_RoofData.ControlPoints.GetNext(index);

            bool isConvex = m_RoofData.IsConvex;
            Vector3[] positions = m_RoofData.ControlPoints.GetPositions();
            Vector3 middle = ProMaths.Average(positions);

            LerpPoint bottomLeft = new LerpPoint(m_RoofData.ControlPoints[index].Position);

            Vector3 tLEnd = isConvex ? middle : m_RoofData.ControlPoints[index].Position + (m_RoofData.ControlPoints[index].Forward * 2);
            Vector3 tREnd = isConvex ? middle : m_RoofData.ControlPoints[next].Position + (m_RoofData.ControlPoints[next].Forward * 2);

            LerpPoint topLeft = new LerpPoint(m_RoofData.ControlPoints[index].Position, tLEnd, m_RoofData.MansardScale);
            LerpPoint topRight = new LerpPoint(m_RoofData.ControlPoints[next].Position, tREnd, m_RoofData.MansardScale);

            LerpPoint bottomRight = new LerpPoint(m_RoofData.ControlPoints[next].Position);

            RoofTileData data = new RoofTileData(m_RoofData.TileData)
            {
                ID = index,
                ControlPoints = new LerpPoint[] { bottomLeft, topLeft, topRight, bottomRight },
                Height = m_RoofData.MansardHeight,
                ExtendHeightBeginning = false,
                ExtendHeightEnd = true,
                ExtendWidthBeginning = false,
                ExtendWidthEnd = false
            };

            return data;
        }
        private RoofTileData CalculateGableTile(int index)
        {
            ushort[] roofTileIndices = m_RoofData.GableData.indices[index];
            bool[] roofTileExtend = m_RoofData.GableData.extend[index];

            Vector3[] oneLine = new Vector3[m_RoofData.OneLine.Length];
            Array.Copy(m_RoofData.OneLine, oneLine, oneLine.Length);

            Vector3[] scaledOneLine = new Vector3[oneLine.Length];
            Array.Copy(oneLine, scaledOneLine, oneLine.Length);

            scaledOneLine.ScaleOneLine(m_RoofData.OneLineShape, 1);

            int[] relIndices = m_RoofData.PathPoints.RelativeIndices(m_RoofData.ShapeIndex);

            float gableScale = m_RoofData.IsOpen ? 1 : m_RoofData.GableScale;

            LerpPoint bottomLeft = new LerpPoint(m_RoofData.PathPoints[relIndices[roofTileIndices[0]]].Position);
            LerpPoint topLeft = new LerpPoint(scaledOneLine[roofTileIndices[1]], oneLine[roofTileIndices[1]], gableScale);
            LerpPoint topRight = new LerpPoint(scaledOneLine[roofTileIndices[2]], oneLine[roofTileIndices[2]], gableScale);
            LerpPoint bottomRight = new LerpPoint(m_RoofData.PathPoints[relIndices[roofTileIndices[3]]].Position);

            bool extendHeightEnd = m_RoofData.RoofType == RoofType.Dormer ? false : roofTileExtend[1];

            // Question: Is extending the width at the beginning & end always false when is open == false?
            bool extendWidthBeginning = m_RoofData.IsOpen && roofTileExtend[2];
            bool extendWidthEnd = m_RoofData.IsOpen && roofTileExtend[3];

            RoofTileData data = new RoofTileData(m_RoofData.TileData)
            {
                ID = index,
                Height = m_RoofData.GableHeight,
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
            if (!m_RoofData.AvailableFrames.Contains((int)RoofType.Gable))
                return null;

            ushort[] wallIndices = m_RoofData.GableData.indices[index].Reverse().ToArray();

            ControlPoint start = new ControlPoint(m_RoofData.PathPoints[wallIndices[0]]);
            start.SetForward(Vector3.zero);
            ControlPoint end = new ControlPoint(m_RoofData.PathPoints[wallIndices[^1]]);
            end.SetForward(Vector3.zero);

            //Vector3 dir = start.DirectionToTarget(end);
            //start += dir * 0.5f;
            //end -= dir * 0.5f;

            WallData data = new WallData()
            {
                ID = index,
                Height = m_RoofData.GableHeight,
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
            if (m_RoofData.PyramidTiles == null || m_RoofData.PyramidTiles.Length == 0)
                m_RoofData.PyramidTiles = new RoofTileData[m_RoofData.ControlPoints.Length];

            for (int i = 0; i < m_RoofData.PyramidTiles.Length; i++)
            {
                if (m_RoofData.PyramidTiles[i] == null || m_RoofData.PyramidTiles[i].ControlPoints == null || m_RoofData.PyramidTiles[i].ControlPoints.Length == 0)
                    m_RoofData.PyramidTiles[i] ??= CalculatePyramid(i);

                RoofTile pyramidTile = CreateRoofTile(m_RoofData.PyramidTiles[i]);
            }
        }
        private void BuildMansard()
        {
            m_RoofData.MansardTiles ??= new RoofTileData[m_RoofData.ControlPoints.Length];

            for (int i = 0; i < m_RoofData.MansardTiles.Length; i++)
            {
                m_RoofData.MansardTiles[i] ??= CalculateMansard(i);

                RoofTile mansardTile = CreateRoofTile(m_RoofData.MansardTiles[i]);
            }

            if (m_RoofData.RoofType == RoofType.Mansard)
            {
                ProBuilderMesh top = ProBuilderMesh.Create();
                top.name = "Lid";
                List<Vector3> lidPoints = new(m_RoofData.MansardTiles.Length);
                foreach (RoofTileData tile in m_RoofData.MansardTiles)
                {
                    lidPoints.Add(tile.TopLeft);
                }
                top.CreateShapeFromPolygon(lidPoints, m_RoofData.TileData.Thickness, false);
                top.GetComponent<Renderer>().sharedMaterial = m_RoofData.TileData.Material;
                top.transform.SetParent(transform, false);
            }
        }
        private void BuildGable()
        {
            if (!m_RoofData.AvailableFrames.Contains((int)RoofType.Gable))
                return;

            if (m_RoofData.GableTiles == null || m_RoofData.GableTiles.Length == 0)
                m_RoofData.GableTiles = new RoofTileData[m_RoofData.PathPoints.Length];

            for (int i = 0; i < m_RoofData.GableTiles.Length; i++)
            {
                if (m_RoofData.GableTiles[i] == null || m_RoofData.GableTiles[i].ControlPoints == null || m_RoofData.GableTiles[i].ControlPoints.Length == 0)
                    m_RoofData.GableTiles[i] = CalculateGableTile(i);
            }

            if (m_RoofData.Walls == null || m_RoofData.Walls.Length == 0)
                m_RoofData.Walls = new WallData[m_RoofData.GableData.wallIndices.Length];

            for (int i = 0; i < m_RoofData.Walls.Length; i++)
            {
                m_RoofData.Walls[i] ??= CalculateGableWall(m_RoofData.GableData.wallIndices[i]);
            }

            for (int i = 0; i < m_RoofData.GableTiles.Length; i++)
            {
                bool condition = m_RoofData.GableData.wallIndices.Any(x => x == i);

                if (condition && m_RoofData.IsOpen)
                {
                    GameObject wallGO = new GameObject("Wall", typeof(Wall.Wall));
                    wallGO.transform.SetParent(transform, false);
                    Wall.Wall wall = wallGO.GetComponent<Wall.Wall>();
                    wall.Initialize(m_RoofData.GetWallByID(i)).Build();
                    continue;
                }

                RoofTile gableTile = CreateRoofTile(m_RoofData.GableTiles[i]);
            }

        }
        #endregion

        private RoofTile CreateRoofTile(RoofTileData data)
        {
            ProBuilderMesh probuilderMesh = ProBuilderMesh.Create();
            probuilderMesh.transform.SetParent(transform, false);
            RoofTile tile = probuilderMesh.AddComponent<RoofTile>();

            tile.Initialize(data).Build();
            return tile;
        }

        public override void Demolish()
        {

        }
    }
}
