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

public class Roof : MonoBehaviour, IBuildable
{
    [SerializeField] private RoofData m_Data;
    // Wall
    [SerializeField] private Storey m_LastStorey;
    [SerializeField] private List<Wall> m_Walls;
    // End Wall

    // Beam
    [SerializeField, Range(0.01f, 10)] private float m_BeamWidth, m_BeamDepth;
    [SerializeField, Range(0, 1)] private float m_SupportBeamDensity;
    [SerializeField] private Material m_BeamMaterial;
    public float BeamWidth => m_BeamWidth;
    public float BeamDepth => m_BeamDepth;
    public float SupportBeamDensity => m_SupportBeamDensity;
    public Material BeamMaterial => m_BeamMaterial;
    //End Beam

    [SerializeField] private List<RoofTile> m_Tiles;
    [SerializeField] private List<RoofTileData> m_TilesData;
    public RoofData Data => m_Data;

    public event Action<RoofData> OnDataChange; // Building should sub to this.
    ///public event Action<IData> OnDataChange;

    public void OnAnyRoofChange_Invoke()
    {
        OnDataChange?.Invoke(m_Data);
    }

    private void Reset()
    {
        Initialize(new RoofData());
    }

    public IBuildable Initialize(IData data)
    {
        m_Data = new RoofData(data as RoofData);
        m_Data.TileData.Material = BuiltinMaterials.defaultMaterial;

        m_Tiles = new List<RoofTile>();
        m_Walls = new List<Wall>();
        return this;
    }

    /// <summary>
    /// Determines frame types that can be applied to this building.
    /// </summary>
    /// <returns></returns>
    public int[] AvailableRoofFrames()
    {
        if (m_Data.ControlPoints == null | m_Data.ControlPoints.Length == 0)
            return new int[0];

        int Gable = (int)RoofType.Gable;
        int Mansard = (int)RoofType.Mansard;
        int Dormer = (int)RoofType.Dormer;
        int MShaped = (int)RoofType.MShaped;
        int Pyramid = (int)RoofType.Pyramid;
        int PyramidHip = (int)RoofType.PyramidHip;

        if(m_Data.ControlPoints.FindPathPoints().Count() == 4)
            return new int[] { Gable, Mansard, Dormer, MShaped, Pyramid, PyramidHip };

        if(m_Data.ControlPoints.IsDescribableInOneLine(out _))
        {
            if (m_Data.ControlPoints.IsPointInside(m_Data.ControlPoints.Centre()))
            {
                return new int[] { Gable, Mansard, Dormer, Pyramid, PyramidHip };
            }
            return new int[] { Gable, Mansard, Dormer };
        }

        if (m_Data.ControlPoints.IsPointInside(m_Data.ControlPoints.Centre()))
        {
            return new int[] {  Mansard, Pyramid, PyramidHip };
        }

        return new int[] { Mansard };
    }

    public void Build()
    {
        transform.DeleteChildren();

        m_Tiles ??= new();

        m_Tiles.Clear();
        m_Walls.Clear();

        if (!m_Data.IsActive)
            return;

        //if (TryGetComponent(out Building building))
        //{
        //    m_LastStorey = building.GetComponents<Storey>()[^1];
        //}
        //else if(TryGetComponent(out WallSection wallSection))
        //{
        //    m_LastStorey = wallSection.GetComponent<Storey>();
        //}
        //else
        //{
        //    m_LastStorey = transform.parent.GetComponents<Storey>()[^1];
        //}

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
    }

    private void BuildPyramid()
    {
        Vector3 middle = ProMaths.Average(m_Data.ControlPoints.GetPositions());
        middle += (Vector3.up * m_Data.PyramidHeight);

        List<RoofTile> pyramidTiles = new();

        for (int i = 0; i < m_Data.ControlPoints.Length; i++)
        {
            int next = m_Data.ControlPoints.GetNext(i);

            RoofTileData data = new RoofTileData(m_Data.TileData)
            {
                ControlPoints = new Vector3[] { m_Data.ControlPoints[i].Position, middle, middle, m_Data.ControlPoints[next].Position },
                ExtendHeightBeginning = false,
                ExtendHeightEnd = true,
                ExtendWidthBeginning = false,
                ExtendWidthEnd = false

            };

            CreateRoofTile(data);
            pyramidTiles.Add(m_Tiles[^1]);
        }

        if (m_Data.RoofType == RoofType.PyramidHip)
        {
            foreach(RoofTile tile in pyramidTiles)
            {
                tile.Data.ExtendHeightEnd = false;
                tile.Build();
            }
        }
    }

    private void BuildMansard()
    {
        ControlPoint[] scaledControlPoints = m_Data.ScaledControlPoints;

        for (int i = 0; i < m_Data.ControlPoints.Length; i++)
        {
            int next = m_Data.ControlPoints.GetNext(i);

            RoofTileData data = new RoofTileData(m_Data.TileData)
            {
                ControlPoints = new Vector3[] { m_Data.ControlPoints[i].Position, scaledControlPoints[i].Position, scaledControlPoints[next].Position, m_Data.ControlPoints[next].Position },
                ExtendHeightBeginning = false,
                ExtendHeightEnd = true,
                ExtendWidthBeginning = false,
                ExtendWidthEnd = false

            };

            CreateRoofTile(data);

            m_Tiles.RemoveAt(m_Tiles.Count - 1);
        }

        if(m_Data.RoofType == RoofType.Mansard)
        {
            ProBuilderMesh top = ProBuilderMesh.Create();
            top.name = "Lid";
            top.CreateShapeFromPolygon(scaledControlPoints.GetPositions(), m_Data.TileData.Height, false);
            top.GetComponent<Renderer>().sharedMaterial = m_Data.TileData.Material;
            top.transform.SetParent(transform, false);
            return;
        }

    }

    private void CreateRoofTiles(Vector3[] oneLine, int startIndex, int[][] indices, bool[][] extend)
    {
        ControlPoint[] points = m_Data.IsGable ? m_Data.ControlPoints.FindPathPoints().ToArray() : m_Data.ControlPoints;

        int[] relativeIndices = points.RelativeIndices(startIndex);

        for (int i = 0; i < indices.Length; i++)
        {
            int[] tileIndices = indices[i];
            bool[] tileExtend = extend[i];

            if (tileIndices[1] == tileIndices[2] && m_Data.IsOpen)
            {
                tileIndices = tileIndices.Reverse().ToArray();
                CreateWall(new Vector3[] { points[relativeIndices[tileIndices[0]]].Position, oneLine[tileIndices[1]], oneLine[tileIndices[2]], points[relativeIndices[tileIndices[3]]].Position });
                continue;
            }

            RoofTileData data = new RoofTileData(m_Data.TileData)
            {
                ControlPoints = new Vector3[] { points[relativeIndices[tileIndices[0]]].Position, oneLine[tileIndices[1]], oneLine[tileIndices[2]], points[relativeIndices[tileIndices[3]]].Position },
                IsInside = m_Data.IsHip,
                ExtendHeightBeginning = tileExtend[0],
                ExtendHeightEnd = tileExtend[1],
                ExtendWidthBeginning = tileExtend[2],
                ExtendWidthEnd = tileExtend[3]
            };

            CreateRoofTile(data);
        }
    }

    private void CreateRoofTile(Vector3[] points, bool heightBeginning = false, bool heightEnd = true, bool widthBeginning = true, bool widthEnd = true)
    {
        RoofTileData data = new RoofTileData(m_Data.TileData)
        {
            ControlPoints = points,
            ExtendHeightBeginning = heightBeginning,
            ExtendHeightEnd = heightEnd,
            ExtendWidthBeginning = widthBeginning,
            ExtendWidthEnd = widthEnd
        };

        CreateRoofTile(data);
    }

    private void CreateRoofTile(RoofTileData data)
    {
        ProBuilderMesh probuilderMesh = ProBuilderMesh.Create();
        probuilderMesh.transform.SetParent(transform, false);
        RoofTile tile = probuilderMesh.AddComponent<RoofTile>();
        //data.IsInside = m_Data.IsHip;
        
        tile.Initialize(data).Build();
        tile.OnDataChange += Tile_OnDataChange;
        m_Tiles.Add(tile);
        //m_TilesData.Add(data);
    }

    private void Tile_OnDataChange(IData obj)
    {
        Debug.Break();
    }

    private void CreateWall(Vector3[] points)
    {
        GameObject wall = new GameObject("Wall", typeof(Wall));
        wall.transform.SetParent(transform, false);

        WallData data = new WallData()
        {
            Start = points[0],
            End = points[^1]
        };
        wall.GetComponent<Wall>().Initialize(data).Build();
        m_Walls.Add(wall.GetComponent<Wall>());
    }

    private void BuildGable()
    {
        ControlPoint[] pathPoints = m_Data.ScaledControlPoints.FindPathPoints().ToArray();

        if (pathPoints.IsDescribableInOneLine(out Vector3[] oneLine))
        {
            if (m_Data.IsHip)
            {
                for (int i = 0; i < pathPoints.Length; i++)
                {
                    pathPoints[i] += Vector3.up * m_Data.TileData.Height;
                }
                oneLine = oneLine.ScaleRoofRidge(pathPoints, m_Data.GableScale);
            }

            for (int i = 0; i < oneLine.Length; i++)
            {
                oneLine[i] += Vector3.up * m_Data.GableHeight;
            }


            RoofTileData data = new RoofTileData(m_Data.TileData)
            {
                IsInside = m_Data.IsHip
            };

            #region Static Patterns
            switch (oneLine.Length)
            {
                case 2:
                    CreateRoofTiles(oneLine, 0, GableData.squareIndices, GableData.squareExtend);
                    break;
                case 3:
                    if (pathPoints.IsLShaped(out int index))
                    {
                        CreateRoofTiles(oneLine, index, GableData.lIndices, GableData.lExtend);
                    }
                    break;
                case 4:
                    if(pathPoints.IsArrowShaped(out int[] arrowPointIndices))
                    {
                        CreateRoofTiles(oneLine, arrowPointIndices[0], GableData.arrowIndices, GableData.arrowExtend);
                    }
                    else if (pathPoints.IsSimpleNShaped(out int[] simpleNPointIndices))
                    {
                        CreateRoofTiles(oneLine, simpleNPointIndices[0], GableData.simpleNIndices, GableData.simpleNExtend);
                    }
                    else if (pathPoints.IsTShaped(out int[] tPointIndices))
                    {
                        CreateRoofTiles(oneLine, tPointIndices[0], GableData.tIndices, GableData.tExtend);
                    }
                    else if (pathPoints.IsUShaped(out int[] uPointIndices))
                    {
                        CreateRoofTiles(oneLine, uPointIndices[0], GableData.uIndices, GableData.uExtend);
                    }
                    else if (pathPoints.IsYShaped(out int[] yPointIndices))
                    {
                        CreateRoofTiles(oneLine, yPointIndices[0], GableData.yIndices, GableData.yExtend);
                    }
                    break;
                case 5:
                    if(pathPoints.IsFShaped(out int[] fIndices))
                    {
                        CreateRoofTiles(oneLine, fIndices[0], GableData.fIndices, GableData.fExtend);
                    }
                    else if (pathPoints.IsSimpleMShaped(out int[] simpleMPointIndices))
                    {
                        CreateRoofTiles(oneLine, simpleMPointIndices[0], GableData.simpleMIndices, GableData.simpleMExtend);
                    }
                    else if (pathPoints.IsXShaped(out int[] xPointIndices))
                    {
                        for (int i = 0; i < xPointIndices.Length; i++)
                        {
                            int current = xPointIndices[i];
                            int previous = pathPoints.GetPrevious(current);
                            int next = pathPoints.GetNext(current);
                            int next1 = pathPoints.GetNext(next);

                            int previousOneLine = oneLine.GetPreviousControlPoint(i);
                            previousOneLine = previousOneLine == oneLine.Length - 1 ? previousOneLine - 1 : previousOneLine;

                            CreateRoofTile(new Vector3[] { pathPoints[current].Position, oneLine[4], oneLine[i], pathPoints[next].Position }, false, true, false, true);
                            CreateRoofTile(new Vector3[] { pathPoints[previous].Position, oneLine[previousOneLine], oneLine[4], pathPoints[current].Position }, false, true, true, false);

                            if (m_Data.IsOpen)
                            {
                                CreateWall(new Vector3[] { pathPoints[next1].Position, oneLine[i], oneLine[i], pathPoints[next].Position });
                            }
                            else
                            {
                                CreateRoofTile(new Vector3[] { pathPoints[next].Position, oneLine[i], oneLine[i], pathPoints[next1].Position }, false, true, false, false);
                            }
                        }

                    }
                    break;
                case 6:
                    if (pathPoints.IsNShaped(out int[] nPointIndices))
                    {
                        CreateRoofTiles(oneLine, nPointIndices[0], GableData.nIndices, GableData.nExtend);
                    }
                    else if (pathPoints.IsEShaped(out int[] ePointIndices))
                    {
                        CreateRoofTiles(oneLine, ePointIndices[0], GableData.eIndices, GableData.eExtend);

                    }
                    else if(pathPoints.IsHShaped(out int[] hPointIndices))
                    {

                        CreateRoofTiles(oneLine, hPointIndices[0], GableData.hIndices, GableData.hExtend);
                    }
                    else if(pathPoints.IsKShaped(out int[] kPointIndices))
                    {
                        CreateRoofTiles(oneLine, kPointIndices[0], GableData.kIndices, GableData.kExtend);
                    }
                    else if(pathPoints.IsInterlockingYShaped(out int[] interYIndices))
                    {
                        CreateRoofTiles(oneLine, interYIndices[0], GableData.interYIndices, GableData.interYExtend);
                    }
                    break;
                case 7:
                    if (pathPoints.IsMShaped(out int[] mPointIndices))
                    {
                        CreateRoofTiles(oneLine, mPointIndices[0], GableData.mIndices, GableData.mExtend);
                    }
                    break;
            }
            #endregion

            #region Repeating Patterns
            if (pathPoints.IsZigZagShaped(out int[] zigZagIndices))
            {
                int[] indices = pathPoints.RelativeIndices(zigZagIndices[0]);
                int previous = indices[^1];
                int start = indices[^3];
                int end = indices[^2];
                
                for (int i = 0; i < oneLine.Length-1; i++)
                {
                    if (i == 0)
                    {
                        CreateRoofTile(new Vector3[] { pathPoints[previous].Position, oneLine[i], oneLine[i + 1], pathPoints[indices[i]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { pathPoints[start].Position, oneLine[i + 1], oneLine[i], pathPoints[end].Position }, false, true, false, true);

                        if(m_Data.IsOpen)
                        {
                            CreateWall(new Vector3[] { pathPoints[previous].Position, oneLine[i], oneLine[i], pathPoints[end].Position });
                            
                        }
                        else
                        {
                            CreateRoofTile(new Vector3[] { pathPoints[end].Position, oneLine[i], oneLine[i], pathPoints[previous].Position }, false, true, false, false);
                        }
                    }
                    else if(i == oneLine.Length-2)
                    {
                        CreateRoofTile(new Vector3[] { pathPoints[previous].Position, oneLine[i], oneLine[i + 1], pathPoints[indices[i]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { pathPoints[start].Position, oneLine[i + 1], oneLine[i], pathPoints[end].Position }, false, true, true, false);
                        
                        if (m_Data.IsOpen)
                        {
                            CreateWall(new Vector3[] { pathPoints[end -1].Position, oneLine[i+1], oneLine[i+1], pathPoints[previous + 1].Position });

                        }
                        else
                        {
                            CreateRoofTile(new Vector3[] { pathPoints[previous + 1].Position, oneLine[i+1], oneLine[i+1], pathPoints[end-1].Position }, false, true, false, false);
                        }
                    }
                    else
                    {
                        CreateRoofTile(new Vector3[] { pathPoints[previous].Position, oneLine[i], oneLine[i + 1], pathPoints[indices[i]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { pathPoints[start].Position, oneLine[i + 1], oneLine[i], pathPoints[end].Position }, false, true, false, false);
                    }

                    previous = indices[i];
                    start--;
                    end--;
                }

            }
            else if(pathPoints.IsCrenelShaped(out int[] crenelIndices))
            {
                int[] startIndices = pathPoints.RelativeIndices(crenelIndices[0]);
                int[] endIndices = pathPoints.RelativeIndices(crenelIndices[^1]);

                CreateRoofTile(new Vector3[] { pathPoints[startIndices[^1]].Position, oneLine[0], oneLine[1], pathPoints[startIndices[0]].Position }, false, true, false, false);
                CreateRoofTile(new Vector3[] { pathPoints[startIndices[^3]].Position, oneLine[1], oneLine[0], pathPoints[startIndices[^2]].Position }, false, true, false, false);
                CreateRoofTile(new Vector3[] { pathPoints[startIndices[^4]].Position, oneLine[^2], oneLine[1], pathPoints[startIndices[^3]].Position }, false, true, false, false);
                CreateRoofTile(new Vector3[] { pathPoints[startIndices[^5]].Position, oneLine[^1], oneLine[^2], pathPoints[startIndices[^4]].Position }, false, true, false, false);
                CreateRoofTile(new Vector3[] { pathPoints[endIndices[0]].Position, oneLine[^2], oneLine[^1], pathPoints[endIndices[1]].Position }, false, true, false, false);
                CreateRoofTile(new Vector3[] { pathPoints[startIndices[0]].Position, oneLine[1], oneLine[2], pathPoints[startIndices[1]].Position }, false, true, false, false);

                if(m_Data.IsOpen)
                {
                    CreateWall(new Vector3[] { pathPoints[startIndices[^1]].Position, oneLine[0], oneLine[0], pathPoints[startIndices[^2]].Position });
                    CreateWall(new Vector3[] { pathPoints[endIndices[2]].Position, oneLine[^1], oneLine[^1], pathPoints[endIndices[1]].Position });
                }
                else
                {
                    CreateRoofTile(new Vector3[] { pathPoints[startIndices[^2]].Position, oneLine[0], oneLine[0], pathPoints[startIndices[^1]].Position }, false, true, false, false);
                    CreateRoofTile(new Vector3[] { pathPoints[endIndices[1]].Position, oneLine[^1], oneLine[^1], pathPoints[endIndices[2]].Position }, false, true, false, false);
                }

                int fourCount = 0;
                int twoCount = 0;

                int length = ((crenelIndices.Length / 2) + 1) - 2;

                for(int i = 0; i < length; i++)
                {
                    CreateRoofTile(new Vector3[] { pathPoints[startIndices[4 + fourCount]].Position, oneLine[2 + twoCount], oneLine[4 + twoCount], pathPoints[startIndices[5 + fourCount]].Position }, false, true, false, false);
                    CreateRoofTile(new Vector3[] { pathPoints[startIndices[1 + fourCount]].Position, oneLine[2 + twoCount], oneLine[3 + twoCount], pathPoints[startIndices[2 + fourCount]].Position }, false, true, false, false);
                    CreateRoofTile(new Vector3[] { pathPoints[startIndices[3 + fourCount]].Position, oneLine[3 + twoCount], oneLine[2 + twoCount], pathPoints[startIndices[4 + fourCount]].Position }, false, true, false, false);

                    if (m_Data.IsOpen)
                    {
                        CreateWall(new Vector3[] { pathPoints[startIndices[3 + fourCount]].Position, oneLine[3 + twoCount], oneLine[3 + twoCount], pathPoints[startIndices[2 + fourCount]].Position });
                    }
                    else
                    {
                        CreateRoofTile(new Vector3[] { pathPoints[startIndices[2 + fourCount]].Position, oneLine[3 + twoCount], oneLine[3 + twoCount], pathPoints[startIndices[3 + fourCount]].Position }, false, true, false, false);
                    }

                    fourCount += 4;
                    twoCount += 2;
                }
            }
            else if(pathPoints.IsAsteriskShaped(out int[] asteriskPointIndices))
            {
                int count = oneLine.Length-2;
                for(int i = 0; i < asteriskPointIndices.Length; i++)
                {
                    int current = asteriskPointIndices[i];
                    int previous = pathPoints.GetIndex(current - 1);
                    int next = pathPoints.GetIndex(current + 1);

                    CreateRoofTile(new Vector3[] { pathPoints[current].Position, oneLine[^1], oneLine[i], pathPoints[next].Position }, false, true, false, true);
                    CreateRoofTile(new Vector3[] { pathPoints[previous].Position, oneLine[count], oneLine[^1], pathPoints[current].Position }, false, true, true, false);
                    count++;

                    if(count > oneLine.Length-2)
                    {
                        count = 0;
                    }

                    int nextTwo = pathPoints.GetIndex(current + 2);

                    if (m_Data.IsOpen)
                    { 
                        CreateWall(new Vector3[] { pathPoints[nextTwo].Position, oneLine[i], oneLine[i], pathPoints[next].Position });
                    }
                    else
                    {
                        CreateRoofTile(new Vector3[] { pathPoints[next].Position, oneLine[i], oneLine[i], pathPoints[nextTwo].Position }, false, true, false, true);
                    }
                }
            }
            else if(pathPoints.IsAntennaShaped(out int[] antPointIndices))
            {
                // Middle Vertical
                int current = pathPoints.GetIndex(antPointIndices[0] - 4);
                for(int i = 3; i < oneLine.Length - 3; i+= 3)
                {
                    int next = pathPoints.GetIndex(current + 1);
                    int nextTwo = pathPoints.GetIndex(current + 2);
                    int nextThree = pathPoints.GetIndex(current + 3);
                    CreateRoofTile(new Vector3[] { pathPoints[current].Position, oneLine[i+1], oneLine[i], pathPoints[next].Position }, false, true, false, true);
                    CreateRoofTile(new Vector3[] { pathPoints[nextTwo].Position, oneLine[i], oneLine[i+1], pathPoints[nextThree].Position }, false, true, true, false);

                    if(m_Data.IsOpen)
                    {
                        CreateWall(new Vector3[] { pathPoints[nextTwo].Position, oneLine[i], oneLine[i], pathPoints[next].Position });
                    }
                    else
                    {
                        CreateRoofTile(new Vector3[] { pathPoints[next].Position, oneLine[i], oneLine[i], pathPoints[nextTwo].Position }, false, true, false, false);
                    }

                    current = pathPoints.GetIndex(current - 4);
                }

                current = pathPoints.GetIndex(current - 3);
                for (int i = oneLine.Length - 4; i > 3; i -= 3)
                {
                    int previous = pathPoints.GetIndex(current - 1);
                    int previousTwo = pathPoints.GetIndex(current - 2);
                    int previousThree = pathPoints.GetIndex(current - 3);
                    CreateRoofTile(new Vector3[] { pathPoints[previous].Position, oneLine[i], oneLine[i-1], pathPoints[current].Position }, false, true, true, false);
                    CreateRoofTile(new Vector3[] { pathPoints[previousThree].Position, oneLine[i-1], oneLine[i], pathPoints[previousTwo].Position }, false, true, false, true);

                    if (m_Data.IsOpen)
                    {
                        CreateWall(new Vector3[] { pathPoints[previous].Position, oneLine[i], oneLine[i], pathPoints[previousTwo].Position });
                    }
                    else
                    {
                        CreateRoofTile(new Vector3[] { pathPoints[previousTwo].Position, oneLine[i], oneLine[i], pathPoints[previous].Position }, false, true, false, false);
                    }

                    current = pathPoints.GetIndex(current - 4);
                }


                // Middle Horizontal
                current = pathPoints.GetIndex(antPointIndices[0]);
                int nextFive = pathPoints.GetIndex(current + 5);
                int index = 1;
                int loop = (oneLine.Length / 3) - 1;
                for (int i = 0; i < loop; i++)
                {
                    int previous = pathPoints.GetIndex(current - 1);
                    int next = pathPoints.GetIndex(nextFive + 1);

                    CreateRoofTile(new Vector3[] { pathPoints[previous].Position, oneLine[index + 3], oneLine[index], pathPoints[current].Position }, false, true, false, false);
                    CreateRoofTile(new Vector3[] { pathPoints[nextFive].Position, oneLine[index], oneLine[index + 3], pathPoints[next].Position }, false, true, false, false);

                    current = pathPoints.GetIndex(current - 4);
                    nextFive = pathPoints.GetIndex(nextFive + 4);
                    index += 3;
                }

                // Ends
                current = pathPoints.GetIndex(antPointIndices[0]);
                index = 0;

                for (int i = 0; i < 2; i++)
                {
                    int next = pathPoints.GetIndex(current + 1);
                    int nextTwo = pathPoints.GetIndex(current + 2);
                    int nextThree = pathPoints.GetIndex(current + 3);
                    int nextFour = pathPoints.GetIndex(current + 4);
                    nextFive = pathPoints.GetIndex(current + 5);

                    if(i == 0)
                    {
                        CreateRoofTile(new Vector3[] { pathPoints[current].Position, oneLine[index + 1], oneLine[index], pathPoints[next].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { pathPoints[nextTwo].Position, oneLine[index], oneLine[index + 2], pathPoints[nextThree].Position }, false, true, true, true);
                        CreateRoofTile(new Vector3[] { pathPoints[nextFour].Position, oneLine[index + 2], oneLine[index + 1], pathPoints[nextFive].Position }, false, true, true, false);

                        if (m_Data.IsOpen)
                        {
                            CreateWall(new Vector3[] { pathPoints[nextTwo].Position, oneLine[index], oneLine[index], pathPoints[next].Position });
                            CreateWall(new Vector3[] { pathPoints[nextFour].Position, oneLine[index + 2], oneLine[index + 2], pathPoints[nextThree].Position });
                        }
                        else
                        {
                            CreateRoofTile(new Vector3[] { pathPoints[next].Position, oneLine[index], oneLine[index], pathPoints[nextTwo].Position }, false, true, false, false);
                            CreateRoofTile(new Vector3[] { pathPoints[nextThree].Position, oneLine[index + 2], oneLine[index + 2], pathPoints[nextFour].Position }, false, true, false, false);
                        }
                    }
                    else
                    {
                        CreateRoofTile(new Vector3[] { pathPoints[current].Position, oneLine[index + 1], oneLine[index + 2], pathPoints[next].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { pathPoints[nextTwo].Position, oneLine[index + 2], oneLine[index], pathPoints[nextThree].Position }, false, true, true, true);
                        CreateRoofTile(new Vector3[] { pathPoints[nextFour].Position, oneLine[index], oneLine[index + 1], pathPoints[nextFive].Position }, false, true, true, false);

                        if (m_Data.IsOpen)
                        {
                            CreateWall(new Vector3[] { pathPoints[nextTwo].Position, oneLine[index + 2], oneLine[index + 2], pathPoints[next].Position });
                            CreateWall(new Vector3[] { pathPoints[nextFour].Position, oneLine[index], oneLine[index], pathPoints[nextThree].Position });
                        }
                        else
                        {
                            CreateRoofTile(new Vector3[] { pathPoints[next].Position, oneLine[index + 2], oneLine[index + 2], pathPoints[nextTwo].Position }, false, true, false, false);
                            CreateRoofTile(new Vector3[] { pathPoints[nextThree].Position, oneLine[index], oneLine[index], pathPoints[nextFour].Position }, false, true, false, false);
                        }
                    }

                    index += oneLine.Length - 3;
                    current = pathPoints.GetIndex(current + (pathPoints.Length/2));
                }
            }
            #endregion
        }

        if (!m_Data.IsOpen)
        {
            foreach (RoofTile tile in m_Tiles)
            {
                tile.Data.ExtendWidthBeginning = false;
                tile.Data.ExtendWidthEnd = false;
                tile.Build();
            }
        }

        if(m_Data.RoofType == RoofType.Dormer)
        {
            foreach(RoofTile tile in m_Tiles)
            {
                tile.Data.IsInside = false;
                tile.Data.ExtendHeightEnd = false;
                tile.Build();
            }

            foreach(Wall wall in m_Walls)
            {
                Vector3 start = wall.WallData.Start;
                Vector3 end = wall.WallData.End;

                Vector3 dir = start.DirectionToTarget(end);

                start += dir * 0.5f;
                end -= dir * 0.5f;

                WallData data = new WallData(wall.WallData)
                {
                    Start = start,
                    End = end
                };
                wall.Initialize(data).Build();

            }
        }
    }

    /// <summary>
    /// Only for a 4 walled building.
    /// </summary>
    private void BuildM()
    {
        Vector3[] mPointsA = new Vector3[4];
        Vector3[] mPointsB = new Vector3[4];

        if (m_Data.IsFlipped)
        {
            mPointsA[1] = m_Data.ControlPoints[0].Position;
            mPointsA[2] = m_Data.ControlPoints[1].Position;

            mPointsB[0] = m_Data.ControlPoints[3].Position;
            mPointsB[3] = m_Data.ControlPoints[2].Position;

            Vector3 midA = Vector3.Lerp(m_Data.ControlPoints[0].Position, m_Data.ControlPoints[3].Position, 0.5f);
            Vector3 midB = Vector3.Lerp(m_Data.ControlPoints[1].Position, m_Data.ControlPoints[2].Position, 0.5f);

            Vector3 dirA = m_Data.ControlPoints[0].Position.DirectionToTarget(m_Data.ControlPoints[3].Position);
            Vector3 dirB = m_Data.ControlPoints[1].Position.DirectionToTarget(m_Data.ControlPoints[2].Position);

            mPointsA[0] = midA - (dirA * m_Data.TileData.Height);
            mPointsA[3] = midB - (dirB * m_Data.TileData.Height);

            mPointsB[1] = midA + (dirA * m_Data.TileData.Height);
            mPointsB[2] = midB + (dirB * m_Data.TileData.Height);

        }
        else
        {
            mPointsA[0] = m_Data.ControlPoints[0].Position;
            mPointsA[3] = m_Data.ControlPoints[3].Position;

            mPointsB[1] = m_Data.ControlPoints[1].Position;
            mPointsB[2] = m_Data.ControlPoints[2].Position;

            Vector3 dirA = mPointsA[0].DirectionToTarget(mPointsB[1]);
            Vector3 dirB = mPointsA[3].DirectionToTarget(mPointsB[2]);

            Vector3 midA = Vector3.Lerp(mPointsA[0], mPointsB[1], 0.5f);
            Vector3 midB = Vector3.Lerp(mPointsA[3], mPointsB[2], 0.5f);

            mPointsA[1] = midA - (dirA * m_Data.TileData.Height);
            mPointsA[2] = midB - (dirB * m_Data.TileData.Height);

            mPointsB[0] = midA + (dirA * m_Data.TileData.Height);
            mPointsB[3] = midB + (dirB * m_Data.TileData.Height);
        }

        List<Vector3[]> mArrays = new List<Vector3[]>();
        mArrays.Add(mPointsA);
        mArrays.Add(mPointsB);

        int iterator = 0;
        foreach (Vector3[] m in mArrays)
        {
            if (m.IsPolygonDescribableInOneLine(out Vector3[] oneLine))
            {
                Vector3 start = oneLine[0] + (Vector3.up * m_Data.GableHeight);
                Vector3 end = oneLine[1] + (Vector3.up * m_Data.GableHeight);

                if (m_Data.IsFlipped)
                {
                    if (iterator == 0)
                    {
                        CreateRoofTile(new Vector3[] { m[1], start, end, m[2] });
                        CreateRoofTile(new Vector3[] { m[3], end, start, m[0] }, false, false);

                    }

                    if (iterator == mArrays.Count - 1)
                    {
                        CreateRoofTile(new Vector3[] { m[1], start, end, m[2] }, false, false);
                        CreateRoofTile(new Vector3[] { m[3], end, start, m[0] });
                    }
                }
                else
                {
                    if (iterator == 0)
                    {
                        CreateRoofTile(new Vector3[] { m[3], end, start, m[0] });
                        CreateRoofTile(new Vector3[] { m[1], start, end, m[2] }, false, false);
                    }

                    if (iterator == mArrays.Count - 1)
                    {
                        CreateRoofTile(new Vector3[] { m[3], end, start, m[0] }, false, false);
                        CreateRoofTile(new Vector3[] { m[1], start, end, m[2] });
                    }
                }

            }

            iterator++;
        }
    }

}
