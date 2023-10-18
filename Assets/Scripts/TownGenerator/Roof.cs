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
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.ProBuilder.MeshOperations;
using TMPro;

public class Roof : MonoBehaviour
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

    [SerializeField, HideInInspector] private ControlPoint[] m_ControlPoints;
    [SerializeField, HideInInspector] private ControlPoint[] m_OriginalControlPoints;

    private Vector3[] m_TempOneLine;

    public IEnumerable<ControlPoint> ControlPoints => m_ControlPoints;
    public RoofData Data => m_Data;

    public Roof SetRoofActive(bool value)
    {
        m_Data.SetActive(value);
        return this;
    }

    public Roof SetControlPoints(IEnumerable<ControlPoint> controlPoints)
    {
        ControlPoint[] points = controlPoints.ToArray();
        m_ControlPoints = PolygonRecognition.Clone(points.ToArray());
        m_OriginalControlPoints = PolygonRecognition.Clone(m_ControlPoints);
        return this;
    }

    public event Action<RoofData> OnAnyRoofChange; // Building should sub to this.

    public void OnAnyRoofChange_Invoke()
    {
        OnAnyRoofChange?.Invoke(m_Data);
    }

    private void Reset()
    {
        Initialize();
    }

    public Roof Initialize()
    {
        m_Data = new RoofData();
        m_Tiles = new List<RoofTile>();
        m_Walls = new List<Wall>();

        m_Data.RoofTileData.SetMaterial(BuiltinMaterials.defaultMaterial);

        //if (TryGetComponent(out Building building))
        //{
        //    m_ControlPoints = PolygonRecognition.Clone(building.LocalControlPoints);
        //    m_OriginalControlPoints = PolygonRecognition.Clone(m_ControlPoints);
        //}

        return this;
    }

    public Roof Initialize(RoofData data)
    {
        m_Data = new RoofData(data);


        return this;
    }
    /// <summary>
    /// Determines frame types that can be applied to this building.
    /// </summary>
    /// <returns></returns>
    public int[] AvailableRoofFrames()
    {
        if (m_ControlPoints == null)
            return new int[0];

        int Gable = (int)RoofType.Gable;
        int Mansard = (int)RoofType.Mansard;
        int Dormer = (int)RoofType.Dormer;
        int MShaped = (int)RoofType.MShaped;
        int Pyramid = (int)RoofType.Pyramid;
        int PyramidHip = (int)RoofType.PyramidHip;

        if(m_ControlPoints.Length == 4)
            return new int[] { Gable, Mansard, Dormer, MShaped, Pyramid, PyramidHip };

        if(m_ControlPoints.IsDescribableInOneLine(out _))
        {
            if (m_ControlPoints.IsPointInside(m_ControlPoints.Centre()))
            {
                return new int[] { Gable, Mansard, Dormer, Pyramid, PyramidHip };
            }
            return new int[] { Gable, Mansard, Dormer };
        }

        if (m_ControlPoints.IsPointInside(m_ControlPoints.Centre()))
        {
            return new int[] {  Mansard, Pyramid, PyramidHip };
        }

        return new int[] { Mansard };
    }

    public Roof BuildFrame()
    {
        transform.DeleteChildren();

        m_Tiles ??= new();

        m_Tiles.Clear();
        m_Walls.Clear();

        if (!m_Data.IsActive)
            return this;

        m_ControlPoints.SetPositions(m_OriginalControlPoints);

        if (TryGetComponent(out Building building))
        {
            m_LastStorey = building.GetComponents<Storey>()[^1];
        }
        else if(TryGetComponent(out WallSection wallSection))
        {
            m_LastStorey = wallSection.GetComponent<Storey>();
        }
        else
        {
            m_LastStorey = transform.parent.GetComponents<Storey>()[^1];
        }

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

        return this;
    }

    private void BuildPyramid()
    {
        Vector3 middle = ProMaths.Average(m_ControlPoints.GetPositions());
        middle += (Vector3.up * m_Data.PyramidHeight);

        List<RoofTile> pyramidTiles = new();

        for (int i = 0; i < m_ControlPoints.Length; i++)
        {
            int next = m_ControlPoints.GetNext(i);
            CreateRoofTile(new Vector3[] { m_ControlPoints[i].Position, middle, middle, m_ControlPoints[next].Position }, false, true, false, false);
            pyramidTiles.Add(m_Tiles[^1]);
        }

        if (m_Data.RoofType == RoofType.PyramidHip)
        {
            foreach(RoofTile tile in pyramidTiles)
            {
                tile.Initialize(m_Data.RoofTileData.Height, m_Data.RoofTileData.Extend, false).Extend(false, false, false, false).Build();
            }
        }
    }

    private void BuildMansard()
    {
        ControlPoint[] scaledControlPoints = PolygonRecognition.Clone(m_ControlPoints);
        scaledControlPoints = scaledControlPoints.ScalePolygon(m_Data.MansardScale);

        for (int i = 0; i < m_ControlPoints.Length; i++)
        {
            scaledControlPoints[i] += Vector3.up * m_Data.MansardHeight;
        }

        for (int i = 0; i < m_ControlPoints.Length; i++)
        {
            int next = m_ControlPoints.GetNext(i);
            CreateRoofTile(new Vector3[] { m_ControlPoints[i].Position, scaledControlPoints[i].Position, scaledControlPoints[next].Position, m_ControlPoints[next].Position }, false, true, false, false);
            m_Tiles.RemoveAt(m_Tiles.Count - 1);
        }

        if(m_Data.RoofType == RoofType.Mansard)
        {
            ProBuilderMesh top = ProBuilderMesh.Create();
            top.name = "Lid";
            top.CreateShapeFromPolygon(scaledControlPoints.GetPositions(), m_Data.RoofTileData.Height, false);
            top.GetComponent<Renderer>().material = m_Data.RoofTileData.Material;
            top.transform.SetParent(transform, false);
            return;
        }

        for (int i = 0; i < m_ControlPoints.Length; i++)
        {
            scaledControlPoints[i] += Vector3.up * m_Data.RoofTileData.Height;
        }

        m_ControlPoints.SetPositions(scaledControlPoints);
    }

    private void CreateRoofTiles(Vector3[] oneLine, int startIndex, int[][] indices, bool[][] extend)
    {
        ControlPoint[] points = m_Data.IsGable ? m_ControlPoints.FindPathPoints().ToArray() : m_ControlPoints;

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

            CreateRoofTile(new Vector3[] { points[relativeIndices[tileIndices[0]]].Position, oneLine[tileIndices[1]], oneLine[tileIndices[2]], points[relativeIndices[tileIndices[3]]].Position }, tileExtend);
        }
    }

    private void CreateRoofTile(Vector3[] points, bool[] extend)
    {
        if (extend.Length != 4)
            return;

        CreateRoofTile(points, extend[0], extend[1], extend[2], extend[3]);
    }

    private void CreateRoofTile(Vector3[] points, bool heightStart = false, bool heightEnd = true, bool widthStart = true, bool widthEnd = true)
    {
        ProBuilderMesh roofTile = ProBuilderMesh.Create();
        roofTile.name = "Roof Tile";
        RoofTile tile = roofTile.AddComponent<RoofTile>();
        tile.SetControlPoints(points);
        tile.SetMaterial(m_Data.RoofTileData.Material);
        tile.transform.SetParent(transform, false);
        tile.Initialize(m_Data.RoofTileData.Height, m_Data.RoofTileData.Extend).Extend(heightStart, heightEnd, widthStart, widthEnd).Build();
        m_Tiles.Add(tile);
    }

    private void CreateWall(Vector3[] points)
    {
        GameObject wall = new GameObject("Wall", typeof(Wall));
        wall.transform.SetParent(transform, false);
        WallData data = new WallData(m_LastStorey.WallData);
        data.SetControlPoints(points);
        wall.GetComponent<Wall>().Initialize(data).Build();
        m_Walls.Add(wall.GetComponent<Wall>());
    }

    private void BuildGable()
    {
        if (m_ControlPoints.IsDescribableInOneLine(out Vector3[] oneLine))
        {
            ControlPoint[] pathPoints = m_ControlPoints.FindPathPoints().ToArray();

            for (int i = 0; i < oneLine.Length; i++)
            {
                oneLine[i] += Vector3.up * m_Data.GableHeight;
            }

            m_TempOneLine = oneLine;

            switch (oneLine.Length)
            {
                case 2:
                    if (!m_Data.IsOpen)
                    {
                        Vector3 mid = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                        oneLine[0] = Vector3.Lerp(oneLine[0], mid, m_Data.GableScale);
                        oneLine[1] = Vector3.Lerp(oneLine[1], mid, m_Data.GableScale);
                    }

                    CreateRoofTile(new Vector3[] { pathPoints[3].Position, oneLine[1], oneLine[0], pathPoints[0].Position });
                    CreateRoofTile(new Vector3[] { pathPoints[1].Position, oneLine[0], oneLine[1], pathPoints[2].Position });

                    if (m_Data.IsOpen)
                    {
                        CreateWall(new Vector3[] { pathPoints[1].Position, oneLine[0], oneLine[0], pathPoints[0].Position });
                        CreateWall(new Vector3[] { pathPoints[3].Position, oneLine[1], oneLine[1], pathPoints[2].Position });
                    }
                    else
                    {
                        CreateRoofTile(new Vector3[] { pathPoints[0].Position, oneLine[0], oneLine[0], pathPoints[1].Position });
                        CreateRoofTile(new Vector3[] { pathPoints[2].Position, oneLine[1], oneLine[1], pathPoints[3].Position });
                    }
                    break;
                case 3:
                    if (pathPoints.IsLShaped(out int index))
                    {
                        if(!m_Data.IsOpen)
                        {
                            Vector3 lp1 = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                            Vector3 lp2 = Vector3.Lerp(oneLine[2], oneLine[1], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], lp1, m_Data.GableScale);
                            oneLine[2] = Vector3.Lerp(oneLine[2], lp2, m_Data.GableScale);
                        }

                        CreateRoofTiles(oneLine, index, GableData.lIndices, GableData.lExtend);
                    }
                    break;
                case 4:
                    if(pathPoints.IsArrowShaped(out int[] arrowPointIndices))
                    {
                        if (!m_Data.IsOpen)
                        {
                            Vector3 ap1 = Vector3.Lerp(oneLine[0], oneLine[3], 0.5f);
                            Vector3 ap2 = Vector3.Lerp(oneLine[1], oneLine[3], 0.5f);
                            Vector3 ap3 = Vector3.Lerp(oneLine[2], oneLine[3], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], ap1, m_Data.GableScale);
                            oneLine[1] = Vector3.Lerp(oneLine[1], ap2, m_Data.GableScale);
                            oneLine[2] = Vector3.Lerp(oneLine[2], ap3, m_Data.GableScale);
                        }

                        CreateRoofTiles(oneLine, arrowPointIndices[0], GableData.arrowIndices, GableData.arrowExtend);
                    }
            
                    else if (pathPoints.IsSimpleNShaped(out int[] simpleNPointIndices))
                    {
                        if (!m_Data.IsOpen)
                        {
                            Vector3 simpleNP1 = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                            Vector3 simpleNP2 = Vector3.Lerp(oneLine[3], oneLine[2], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], simpleNP1, m_Data.GableScale);
                            oneLine[3] = Vector3.Lerp(oneLine[3], simpleNP2, m_Data.GableScale);
                        }

                        CreateRoofTiles(oneLine, simpleNPointIndices[0], GableData.simpleNIndices, GableData.simpleNExtend);
                    }
                    else if (pathPoints.IsTShaped(out int[] tPointIndices))
                    {
                        if(!m_Data.IsOpen)
                        {
                            Vector3 tp1 = Vector3.Lerp(oneLine[0], oneLine[3], 0.5f);
                            Vector3 tp2 = Vector3.Lerp(oneLine[1], oneLine[3], 0.5f);
                            Vector3 tp3 = Vector3.Lerp(oneLine[2], oneLine[3], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], tp1, m_Data.GableScale);
                            oneLine[1] = Vector3.Lerp(oneLine[1], tp2, m_Data.GableScale);
                            oneLine[2] = Vector3.Lerp(oneLine[2], tp3, m_Data.GableScale);
                        }

                        CreateRoofTiles(oneLine, tPointIndices[0], GableData.tIndices, GableData.tExtend);
                    }
                    else if (pathPoints.IsUShaped(out int[] uPointIndices))
                    {
                        if (!m_Data.IsOpen)
                        {
                            Vector3 up1 = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                            Vector3 up2 = Vector3.Lerp(oneLine[3], oneLine[2], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], up1, m_Data.GableScale);
                            oneLine[3] = Vector3.Lerp(oneLine[3], up2, m_Data.GableScale);
                        }

                        CreateRoofTiles(oneLine, uPointIndices[0], GableData.uIndices, GableData.uExtend);

                    }
                    else if (pathPoints.IsYShaped(out int[] yPointIndices))
                    {
                        if (!m_Data.IsOpen)
                        {
                            Vector3 yp1 = Vector3.Lerp(oneLine[0], oneLine[3], 0.5f);
                            Vector3 yp2 = Vector3.Lerp(oneLine[1], oneLine[3], 0.5f);
                            Vector3 yp3 = Vector3.Lerp(oneLine[2], oneLine[3], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], yp1, m_Data.GableScale);
                            oneLine[1] = Vector3.Lerp(oneLine[1], yp2, m_Data.GableScale);
                            oneLine[2] = Vector3.Lerp(oneLine[2], yp3, m_Data.GableScale);
                        }

                        CreateRoofTiles(oneLine, yPointIndices[0], GableData.yIndices, GableData.yExtend);
                    }
                    break;
                case 5:
                    if(pathPoints.IsFShaped(out int[] fIndices))
                    {
                        if (!m_Data.IsOpen)
                        {
                            Vector3 a = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                            Vector3 b = Vector3.Lerp(oneLine[2], oneLine[1], 0.5f);
                            Vector3 c = Vector3.Lerp(oneLine[4], oneLine[3], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], a, m_Data.GableScale);
                            oneLine[2] = Vector3.Lerp(oneLine[2], b, m_Data.GableScale);
                            oneLine[4] = Vector3.Lerp(oneLine[4], c, m_Data.GableScale);
                        }

                        CreateRoofTiles(oneLine, fIndices[0], GableData.fIndices, GableData.fExtend);
                    }

                    else if (pathPoints.IsSimpleMShaped(out int[] simpleMPointIndices))
                    {

                        if (!m_Data.IsOpen)
                        {
                            Vector3 simpleMP1 = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                            Vector3 simpleMP2 = Vector3.Lerp(oneLine[^1], oneLine[^2], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], simpleMP1, m_Data.GableScale);
                            oneLine[^1] = Vector3.Lerp(oneLine[^1], simpleMP2, m_Data.GableScale);
                        }

                        CreateRoofTiles(oneLine, simpleMPointIndices[0], GableData.simpleMIndices, GableData.simpleMExtend);
                    }
                    else if (pathPoints.IsXShaped(out int[] xPointIndices))
                    {
                        if (!m_Data.IsOpen)
                        {
                            Vector3 mid = ProMaths.Average(oneLine);

                            for (int i = 0; i < oneLine.Length - 1; i++)
                            {
                                Vector3 xP = Vector3.Lerp(oneLine[i], mid, 0.5f);
                                oneLine[i] = Vector3.Lerp(oneLine[i], xP, m_Data.GableScale);
                            }

                        }

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
                        if (!m_Data.IsOpen)
                        {
                            Vector3 nP1 = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                            Vector3 nP2 = Vector3.Lerp(oneLine[^1], oneLine[^2], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], nP1, m_Data.GableScale);
                            oneLine[^1] = Vector3.Lerp(oneLine[^1], nP2, m_Data.GableScale);
                        }

                        CreateRoofTiles(oneLine, nPointIndices[0], GableData.nIndices, GableData.nExtend);
                    }
                    else if (pathPoints.IsEShaped(out int[] ePointIndices))
                    {
                        if (!m_Data.IsOpen)
                        {
                            Vector3 eP1 = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                            Vector3 eP2 = Vector3.Lerp(oneLine[3], oneLine[2], 0.5f);
                            Vector3 eP3 = Vector3.Lerp(oneLine[5], oneLine[4], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], eP1, m_Data.GableScale);
                            oneLine[3] = Vector3.Lerp(oneLine[3], eP2, m_Data.GableScale);
                            oneLine[5] = Vector3.Lerp(oneLine[5], eP3, m_Data.GableScale);
                        }

                        CreateRoofTiles(oneLine, ePointIndices[0], GableData.eIndices, GableData.eExtend);

                    }
                    else if(pathPoints.IsHShaped(out int[] hPointIndices))
                    {
                        if (!m_Data.IsOpen)
                        {
                            Vector3 a = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                            Vector3 b = Vector3.Lerp(oneLine[2], oneLine[1], 0.5f);
                            Vector3 c = Vector3.Lerp(oneLine[3], oneLine[4], 0.5f);
                            Vector3 d = Vector3.Lerp(oneLine[5], oneLine[4], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], a, m_Data.GableScale);
                            oneLine[2] = Vector3.Lerp(oneLine[2], b, m_Data.GableScale);
                            oneLine[3] = Vector3.Lerp(oneLine[3], c, m_Data.GableScale);
                            oneLine[5] = Vector3.Lerp(oneLine[5], d, m_Data.GableScale);
                        }

                        CreateRoofTiles(oneLine, hPointIndices[0], GableData.hIndices, GableData.hExtend);
                    }
                    else if(pathPoints.IsKShaped(out int[] kPointIndices))
                    {
                        if (!m_Data.IsOpen)
                        {
                            Vector3 a = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                            Vector3 b = Vector3.Lerp(oneLine[2], oneLine[1], 0.5f);
                            Vector3 c = Vector3.Lerp(oneLine[3], oneLine[5], 0.5f);
                            Vector3 d = Vector3.Lerp(oneLine[4], oneLine[5], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], a, m_Data.GableScale);
                            oneLine[2] = Vector3.Lerp(oneLine[2], b, m_Data.GableScale);
                            oneLine[3] = Vector3.Lerp(oneLine[3], c, m_Data.GableScale);
                            oneLine[4] = Vector3.Lerp(oneLine[4], d, m_Data.GableScale);
                        }

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
                        if (!m_Data.IsOpen)
                        {
                            Vector3 mP1 = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                            Vector3 mP2 = Vector3.Lerp(oneLine[^1], oneLine[^2], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], mP1, m_Data.GableScale);
                            oneLine[^1] = Vector3.Lerp(oneLine[^1], mP2, m_Data.GableScale);
                        }

                        CreateRoofTiles(oneLine, mPointIndices[0], GableData.mIndices, GableData.mExtend);
                    }
                    break;
            }

            // Repeating patterns
            if(pathPoints.IsZigZagShaped(out int[] zigZagIndices))
            {
                if (!m_Data.IsOpen)
                {
                    Vector3 zigP1 = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                    Vector3 zigP2 = Vector3.Lerp(oneLine[^1], oneLine[^2], 0.5f);

                    oneLine[0] = Vector3.Lerp(oneLine[0], zigP1, m_Data.GableScale);
                    oneLine[^1] = Vector3.Lerp(oneLine[^1], zigP2, m_Data.GableScale);
                }

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
                if (!m_Data.IsOpen)
                {
                    Vector3 crenelP1 = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                    Vector3 crenelP2 = Vector3.Lerp(oneLine[^1], oneLine[^2], 0.5f);

                    oneLine[0] = Vector3.Lerp(oneLine[0], crenelP1, m_Data.GableScale);
                    oneLine[^1] = Vector3.Lerp(oneLine[^1], crenelP2, m_Data.GableScale);

                    for (int i = 2; i < oneLine.Length -2; i+= 2)
                    {
                        Vector3 a = Vector3.Lerp(oneLine[i], oneLine[i+1], 0.5f);
                        oneLine[i+1] = Vector3.Lerp(oneLine[i+1], a, m_Data.GableScale);
                    }
                }

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
                if(!m_Data.IsOpen)
                {
                    for(int i = 0; i < oneLine.Length-1; i++)
                    {
                        Vector3 a = Vector3.Lerp(oneLine[i], oneLine[^1], 0.5f);
                        oneLine[i] = Vector3.Lerp(oneLine[i], a, m_Data.GableScale);
                    }
                }

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
                int length = oneLine.Length / 3;
                int bIndex = 0;
                int mIndex = 1;
                int tIndex = 2;
                for(int i = 0; i < length; i++)
                {
                    Vector3 a = Vector3.Lerp(oneLine[tIndex], oneLine[mIndex], 0.5f);
                    Vector3 b = Vector3.Lerp(oneLine[bIndex], oneLine[mIndex], 0.5f);

                    oneLine[tIndex] = Vector3.Lerp(oneLine[tIndex], a, m_Data.GableScale);
                    oneLine[bIndex] = Vector3.Lerp(oneLine[bIndex], b, m_Data.GableScale);

                    bIndex += 3;
                    mIndex += 3;
                    tIndex += 3;
                }

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
        }

        if(!m_Data.IsOpen)
        {
            foreach (RoofTile tile in m_Tiles)
            {
                tile.Initialize(m_Data.RoofTileData.Height, m_Data.RoofTileData.Extend).Extend(tile.ExtendHeightBeginning, tile.ExtendHeightEnd, false, false).Build();
            }
        }

        if(m_Data.RoofType == RoofType.Dormer)
        {
            foreach(RoofTile tile in m_Tiles)
            {
                tile.Initialize(m_Data.RoofTileData.Height, m_Data.RoofTileData.Extend, false).Extend(tile.ExtendHeightBeginning, false, tile.ExtendWidthBeginning, tile.ExtendWidthEnd).Build();
            }

            foreach(Wall wall in m_Walls)
            {
                Vector3[] points = wall.WallData.ControlPoints;

                Vector3 dir = points[0].DirectionToTarget(points[3]);

                points[0] += dir * 0.5f;
                points[3] -= dir * 0.5f;

                WallData data = new WallData(wall.WallData);
                data.SetControlPoints(points);

                wall.Initialize(data).Build();

            }
        }
    }

    /// <summary>
    /// Only for a 4 walled building.
    /// </summary>
    private void BuildM()
    {
        if (m_ControlPoints.Length != 4)
            return;

        Vector3[] mPointsA = new Vector3[4];
        Vector3[] mPointsB = new Vector3[4];

        if (m_Data.IsFlipped)
        {
            mPointsA[1] = m_ControlPoints[0].Position;
            mPointsA[2] = m_ControlPoints[1].Position;

            mPointsB[0] = m_ControlPoints[3].Position;
            mPointsB[3] = m_ControlPoints[2].Position;

            Vector3 midA = Vector3.Lerp(m_ControlPoints[0].Position, m_ControlPoints[3].Position, 0.5f);
            Vector3 midB = Vector3.Lerp(m_ControlPoints[1].Position, m_ControlPoints[2].Position, 0.5f);

            Vector3 dirA = m_ControlPoints[0].Position.DirectionToTarget(m_ControlPoints[3].Position);
            Vector3 dirB = m_ControlPoints[1].Position.DirectionToTarget(m_ControlPoints[2].Position);

            mPointsA[0] = midA - (dirA * m_Data.RoofTileData.Height);
            mPointsA[3] = midB - (dirB * m_Data.RoofTileData.Height);

            mPointsB[1] = midA + (dirA * m_Data.RoofTileData.Height);
            mPointsB[2] = midB + (dirB * m_Data.RoofTileData.Height);

        }
        else
        {
            mPointsA[0] = m_ControlPoints[0].Position;
            mPointsA[3] = m_ControlPoints[3].Position;

            mPointsB[1] = m_ControlPoints[1].Position;
            mPointsB[2] = m_ControlPoints[2].Position;

            Vector3 dirA = mPointsA[0].DirectionToTarget(mPointsB[1]);
            Vector3 dirB = mPointsA[3].DirectionToTarget(mPointsB[2]);

            Vector3 midA = Vector3.Lerp(mPointsA[0], mPointsB[1], 0.5f);
            Vector3 midB = Vector3.Lerp(mPointsA[3], mPointsB[2], 0.5f);

            mPointsA[1] = midA - (dirA * m_Data.RoofTileData.Height);
            mPointsA[2] = midB - (dirB * m_Data.RoofTileData.Height);

            mPointsB[0] = midA + (dirA * m_Data.RoofTileData.Height);
            mPointsB[3] = midB + (dirB * m_Data.RoofTileData.Height);
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

    private void OnDrawGizmosSelected()
    {
        //if (m_TempVerts == null | m_TempVerts.Length == 0)
        //    return;
        if (m_ControlPoints == null)
            return;


        for (int i = 0; i < m_ControlPoints.Length; i++)
        {
            Handles.color = Color.white;
            Handles.Label(m_ControlPoints[i].Position + transform.localPosition, i.ToString());
        }

        if (m_TempOneLine == null)
            return;

        int itr = 0;
        GUIStyle style = new GUIStyle();
        style.fontSize = 16;
        style.normal.textColor = Color.red;

        foreach (Vector3 point in m_TempOneLine)
        {
            Handles.Label(point, itr.ToString(), style);
            itr++;
            //Handles.DrawSolidDisc(point, Vector3.up, 0.1f);
        }

    }
}
