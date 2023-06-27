using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.Shapes;
using Vertex = UnityEngine.ProBuilder.Vertex;
using Rando = UnityEngine.Random;
using ProMaths = UnityEngine.ProBuilder.Math;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UIElements;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

[System.Serializable]
public class Roof : MonoBehaviour
{
    [SerializeField] private bool m_IsRoofActive;
    [SerializeField] private RoofType m_FrameType;

    // Wall
    [SerializeField] Storey m_LastStorey;
    [SerializeField] List<Wall> m_Walls;
    // End Wall

    // Tile Data
    [SerializeField, Range(0, 10)] private float m_TileHeight;
    [SerializeField, Range(0, 10)] private float m_TileExtend;
    [SerializeField] private bool m_TileFlipFace;
    [SerializeField] private Material m_TileMaterial;
    [SerializeField] private List<RoofTile> m_Tiles;
    public float TileHeight => m_TileHeight;
    public float TileExtend => m_TileExtend;
    public bool TileFlipFace => m_TileFlipFace;
    public Material TileMaterial => m_TileMaterial;
    // End Tile

    // Beam
    [SerializeField, Range(0.01f, 10)] private float m_BeamWidth, m_BeamDepth;
    [SerializeField, Range(0, 1)] private float m_SupportBeamDensity;
    [SerializeField] private Material m_BeamMaterial;
    public float BeamWidth => m_BeamWidth;
    public float BeamDepth => m_BeamDepth;
    public float SupportBeamDensity => m_SupportBeamDensity;
    public Material BeamMaterial => m_BeamMaterial;
    //End Beam

    // Mansard
    [SerializeField, Range(-10, 10)] private float m_MansardHeight;
    [SerializeField, Range(0, 2)] private float m_MansardScale;
    public float MansardScale => m_MansardScale;
    public float MansardHeight => m_MansardHeight;
    // End Mansard

    // Pyramid
    [SerializeField, Range(-10, 10)] private float m_PyramidHeight;
    public float PyramidHeight => m_PyramidHeight;
    // End Pyramid

    // Gable 
    [SerializeField, Range(-10, 10)] private float m_GableHeight;
    [SerializeField, Range(0, 1)] private float m_GableScale;
    [SerializeField] private bool m_IsFlipped; // For M shaped
    [SerializeField] private bool m_IsOpen;
    public float GableHeight => m_GableHeight;
    public bool IsFlipped => m_IsFlipped;
    public bool IsOpen => m_IsOpen;
    public float GableScale => m_GableScale;
    // End Gable

    [SerializeField, HideInInspector] private ControlPoint[] m_ControlPoints;
    [SerializeField, HideInInspector] private ControlPoint[] m_OriginalControlPoints;

    public bool IsRoofActive => m_IsRoofActive;
    public RoofType FrameType => m_FrameType;

    private Vector3[] m_TempOneLine;

    public IEnumerable<ControlPoint> ControlPoints => m_ControlPoints;

    public Roof SetRoofActive(bool value)
    {
        m_IsRoofActive = value;
        return this;
    }

    public Roof SetControlPoints(IEnumerable<ControlPoint> controlPoints)
    {
        ControlPoint[] points = controlPoints.ToArray();
        m_ControlPoints = PolygonRecognition.Clone(points.ToArray());
        m_OriginalControlPoints = PolygonRecognition.Clone(m_ControlPoints);
        return this;
    }

    public event Action<Roof> OnAnyRoofChange; // Building should sub to this.

    public void OnAnyRoofChange_Invoke()
    {
        OnAnyRoofChange?.Invoke(this);
    }

    private void Reset()
    {
        Initialize();
    }

    public Roof Initialize()
    {
        m_IsRoofActive = true;
        m_GableHeight = 1;
        m_GableScale = 0.75f;
        m_IsOpen = false;
        m_IsFlipped = false;
        m_PyramidHeight = 1;
        m_MansardHeight = 1;
        m_MansardScale = 1;
        m_FrameType = RoofType.Mansard;
        m_BeamDepth = 0.2f;
        m_BeamWidth = 0.2f;
        m_SupportBeamDensity = 0.5f;
        m_BeamMaterial = BuiltinMaterials.defaultMaterial;
        m_TileHeight = 0.25f;
        m_TileExtend = 0.2f;
        m_TileFlipFace = false;
        m_TileMaterial = BuiltinMaterials.defaultMaterial;
        m_Tiles = new List<RoofTile>();
        m_Walls = new List<Wall>();

        if (TryGetComponent(out Building building))
        {
            m_ControlPoints = PolygonRecognition.Clone(building.ControlPoints.ToArray());
            m_OriginalControlPoints = PolygonRecognition.Clone(m_ControlPoints);
        }

        return this;
    }

    public Roof Initialize(Roof roof)
    {
        m_IsRoofActive = roof.IsRoofActive;
        m_GableHeight = roof.GableHeight;
        m_GableScale = roof.GableScale;
        m_IsOpen = roof.IsOpen;
        m_IsFlipped = roof.IsFlipped;
        m_PyramidHeight = roof.PyramidHeight;
        m_MansardHeight = roof.MansardHeight;
        m_MansardScale = roof.MansardScale;
        m_FrameType = roof.FrameType;
        m_BeamDepth = roof.BeamDepth;
        m_BeamWidth = roof.m_BeamWidth;
        m_SupportBeamDensity = roof.SupportBeamDensity;
        m_BeamMaterial = roof.m_BeamMaterial;

        //m_ControlPoints = roof.ControlPoints.ToArray();
        //m_OriginalControlPoints = m_ControlPoints.Clone() as ControlPoint[];
        m_TileHeight = roof.TileHeight;
        m_TileExtend = roof.TileExtend;
        m_TileFlipFace = roof.TileFlipFace;
        m_TileMaterial = roof.TileMaterial;

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

        int OpenGable = (int)RoofType.Gable;
        int Mansard = (int)RoofType.Mansard;
        int Dormer = (int)RoofType.Dormer;
        int MShaped = (int)RoofType.MShaped;
        int Pyramid = (int)RoofType.Pyramid;
        int PyramidHip = (int)RoofType.PyramidHip;

        switch (m_ControlPoints.Length)
        {
            case 4:
                return new int[] { OpenGable, Mansard, Dormer, MShaped, Pyramid, PyramidHip };
            case 5:
                if (m_ControlPoints.IsPointInside(m_ControlPoints.Centre()))
                {
                    return new int[] { Mansard, Pyramid, PyramidHip };
                }
                return new int[] { Mansard };
            case 6:
                if (m_ControlPoints.IsLShaped(out _))
                {
                    if (m_ControlPoints.IsPointInside(m_ControlPoints.Centre()))
                    {
                        return new int[] { OpenGable, Mansard, Dormer, Pyramid, PyramidHip };
                    }

                    return new int[] { OpenGable, Mansard, Dormer };
                }
                if (m_ControlPoints.IsPointInside(m_ControlPoints.Centre()))
                {
                    return new int[] { Mansard, Pyramid, PyramidHip };
                }
                return new int[] { Mansard };
            case 7:
                if (m_ControlPoints.IsPointInside(m_ControlPoints.Centre()))
                {
                    return new int[] { Mansard, Pyramid, PyramidHip };
                }
                return new int[] { Mansard };
            case 8:
                if (m_ControlPoints.IsTShaped(out _) || m_ControlPoints.IsUShaped(out _) || m_ControlPoints.IsSimpleNShaped(out _))
                {
                    if (m_ControlPoints.IsPointInside(m_ControlPoints.Centre()))
                    {
                        return new int[] { OpenGable, Mansard, Dormer, Pyramid, PyramidHip };
                    }

                    return new int[] { OpenGable, Mansard, Dormer };
                }
                return new int[] { Mansard };
            default:
                return new int[] { OpenGable, Mansard, Dormer, Pyramid, PyramidHip };
        }
    }

    public Roof BuildFrame()
    {
        transform.DeleteChildren();

        m_Tiles.Clear();
        m_Walls.Clear();

        if (!m_IsRoofActive)
            return this;

        m_ControlPoints.SetPositions(m_OriginalControlPoints);

        if (TryGetComponent(out Building building))
        {
            m_LastStorey = building.GetComponents<Storey>()[^1];
        }
        else
        {
            m_LastStorey = transform.parent.GetComponents<Storey>()[^1];
        }

        switch (m_FrameType)
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
        middle += (Vector3.up * m_PyramidHeight);

        List<RoofTile> pyramidTiles = new();

        for (int i = 0; i < m_ControlPoints.Length; i++)
        {
            int next = m_ControlPoints.GetNext(i);
            CreateRoofTile(new Vector3[] { m_ControlPoints[i].Position, middle, middle, m_ControlPoints[next].Position }, false, true, false, false);
            pyramidTiles.Add(m_Tiles[^1]);
        }

        if (m_FrameType == RoofType.PyramidHip)
        {
            foreach(RoofTile tile in pyramidTiles)
            {
                tile.Initialize(m_TileHeight, m_TileExtend, false).Extend(false, false, false, false).Build();
            }
        }
    }

    private void BuildMansard()
    {
        ControlPoint[] scaledControlPoints = PolygonRecognition.Clone(m_ControlPoints);
        scaledControlPoints = scaledControlPoints.ScalePolygon(m_MansardScale);

        for (int i = 0; i < m_ControlPoints.Length; i++)
        {
            scaledControlPoints[i] += Vector3.up * m_MansardHeight;
        }

        for (int i = 0; i < m_ControlPoints.Length; i++)
        {
            int next = m_ControlPoints.GetNext(i);
            CreateRoofTile(new Vector3[] { m_ControlPoints[i].Position, scaledControlPoints[i].Position, scaledControlPoints[next].Position, m_ControlPoints[next].Position }, false, true, false, false);
            m_Tiles.RemoveAt(m_Tiles.Count - 1);
        }

        if(m_FrameType == RoofType.Mansard)
        {
            ProBuilderMesh top = ProBuilderMesh.Create();
            top.name = "Lid";
            top.CreateShapeFromPolygon(scaledControlPoints.GetPositions(), m_TileHeight, false);
            top.GetComponent<Renderer>().material = TileMaterial;
            top.transform.SetParent(transform, false);
            return;
        }

        for (int i = 0; i < m_ControlPoints.Length; i++)
        {
            scaledControlPoints[i] += Vector3.up * m_TileHeight;
        }

        m_ControlPoints.SetPositions(scaledControlPoints);
    }

    private void CreateRoofTile(Vector3[] points, bool heightStart = false, bool heightEnd = true, bool widthStart = true, bool widthEnd = true)
    {
        ProBuilderMesh roofTile = ProBuilderMesh.Create();
        roofTile.name = "Roof Tile";
        RoofTile tile = roofTile.AddComponent<RoofTile>();
        tile.SetControlPoints(points);
        tile.GetComponent<Renderer>().material = TileMaterial;
        tile.transform.SetParent(transform, false);
        tile.Initialize(m_TileHeight, m_TileExtend).Extend(heightStart, heightEnd, widthStart, widthEnd).Build();
        m_Tiles.Add(tile);
    }

    private void CreateWall(Vector3[] points)
    {
        GameObject wall = new GameObject("Wall", typeof(Wall));
        wall.transform.SetParent(transform, false);
        wall.GetComponent<Wall>().Initialize(points, m_LastStorey.WallDepth, m_LastStorey.WallMaterial).Build();
        m_Walls.Add(wall.GetComponent<Wall>());
    }

    private void BuildGable()
    {
        if (m_ControlPoints.IsDescribableInOneLine(out Vector3[] oneLine))
        {
            for (int i = 0; i < oneLine.Length; i++)
            {
                oneLine[i] += Vector3.up * m_GableHeight;
            }

            m_TempOneLine = oneLine;

            switch (oneLine.Length)
            {
                case 2:
                    if (!m_IsOpen)
                    {
                        Vector3 mid = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                        oneLine[0] = Vector3.Lerp(oneLine[0], mid, m_GableScale);
                        oneLine[1] = Vector3.Lerp(oneLine[1], mid, m_GableScale);
                    }

                    CreateRoofTile(new Vector3[] { m_ControlPoints[3].Position, oneLine[1], oneLine[0], m_ControlPoints[0].Position });
                    CreateRoofTile(new Vector3[] { m_ControlPoints[1].Position, oneLine[0], oneLine[1], m_ControlPoints[2].Position });

                    if (m_IsOpen)
                    {
                        CreateWall(new Vector3[] { m_ControlPoints[1].Position, oneLine[0], oneLine[0], m_ControlPoints[0].Position });
                        CreateWall(new Vector3[] { m_ControlPoints[3].Position, oneLine[1], oneLine[1], m_ControlPoints[2].Position });
                    }
                    else
                    {
                        CreateRoofTile(new Vector3[] { m_ControlPoints[0].Position, oneLine[0], oneLine[0], m_ControlPoints[1].Position });
                        CreateRoofTile(new Vector3[] { m_ControlPoints[2].Position, oneLine[1], oneLine[1], m_ControlPoints[3].Position });
                    }
                    break;
                case 3:
                    if (m_ControlPoints.IsLShaped(out int index))
                    {
                        if(!m_IsOpen)
                        {
                            Vector3 lp1 = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                            Vector3 lp2 = Vector3.Lerp(oneLine[2], oneLine[1], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], lp1, m_GableScale);
                            oneLine[2] = Vector3.Lerp(oneLine[2], lp2, m_GableScale);
                        }

                        int[] indices = m_ControlPoints.RelativeIndices(index);

                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[0]].Position, oneLine[1], oneLine[0], m_ControlPoints[indices[1]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[2]].Position, oneLine[0], oneLine[1], m_ControlPoints[indices[3]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[5]].Position, oneLine[2], oneLine[1], m_ControlPoints[indices[0]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[3]].Position, oneLine[1], oneLine[2], m_ControlPoints[indices[4]].Position }, false, true, false, true);

                        if (m_IsOpen)
                        {
                            CreateWall(new Vector3[] { m_ControlPoints[indices[5]].Position, oneLine[2], oneLine[2], m_ControlPoints[indices[4]].Position });
                            CreateWall(new Vector3[] { m_ControlPoints[indices[2]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[1]].Position });
                        }
                        else
                        {
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[4]].Position, oneLine[2], oneLine[2], m_ControlPoints[indices[5]].Position }, false, true, true, false);
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[1]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[2]].Position }, false, true, false, true);
                        }
                    }
                    break;
                case 4:
                    if (m_ControlPoints.IsSimpleNShaped(out int[] simpleNPointIndices))
                    {
                        if (!m_IsOpen)
                        {
                            Vector3 simpleNP1 = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                            Vector3 simpleNP2 = Vector3.Lerp(oneLine[3], oneLine[2], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], simpleNP1, m_GableScale);
                            oneLine[3] = Vector3.Lerp(oneLine[3], simpleNP2, m_GableScale);
                        }

                        int[] indices = m_ControlPoints.RelativeIndices(simpleNPointIndices[0]);

                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[0]].Position, oneLine[1], oneLine[2], m_ControlPoints[indices[1]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[1]].Position, oneLine[2], oneLine[3], m_ControlPoints[indices[2]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[3]].Position, oneLine[3], oneLine[2], m_ControlPoints[indices[4]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[4]].Position, oneLine[2], oneLine[1], m_ControlPoints[indices[5]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[5]].Position, oneLine[1], oneLine[0], m_ControlPoints[indices[6]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[7]].Position, oneLine[0], oneLine[1], m_ControlPoints[indices[0]].Position }, false, true, true, false);

                        if (m_IsOpen)
                        {
                            CreateWall(new Vector3[] { m_ControlPoints[indices[7]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[6]].Position });
                            CreateWall(new Vector3[] { m_ControlPoints[indices[3]].Position, oneLine[^1], oneLine[^1], m_ControlPoints[indices[2]].Position });
                        }
                        else
                        {
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[2]].Position, oneLine[3], oneLine[3], m_ControlPoints[indices[3]].Position }, false, true, false, false);
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[6]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[7]].Position }, false, true, false, false);
                        }

                    }
                    else if (m_ControlPoints.IsTShaped(out int[] tPointIndices))
                    {
                        if(!m_IsOpen)
                        {
                            Vector3 tp1 = Vector3.Lerp(oneLine[0], oneLine[3], 0.5f);
                            Vector3 tp2 = Vector3.Lerp(oneLine[1], oneLine[3], 0.5f);
                            Vector3 tp3 = Vector3.Lerp(oneLine[2], oneLine[3], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], tp1, m_GableScale);
                            oneLine[1] = Vector3.Lerp(oneLine[1], tp2, m_GableScale);
                            oneLine[2] = Vector3.Lerp(oneLine[2], tp3, m_GableScale);
                        }

                        int[] indices = m_ControlPoints.RelativeIndices(tPointIndices[0]);

                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[0]].Position, oneLine[3], oneLine[0], m_ControlPoints[indices[1]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[2]].Position, oneLine[0], oneLine[3], m_ControlPoints[indices[3]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[3]].Position, oneLine[3], oneLine[2], m_ControlPoints[indices[4]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[5]].Position, oneLine[2], oneLine[1], m_ControlPoints[indices[6]].Position }, false, true, true, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[7]].Position, oneLine[1], oneLine[3], m_ControlPoints[indices[0]].Position }, false, true, true, false);

                        if (m_IsOpen)
                        {
                            CreateWall(new Vector3[] { m_ControlPoints[indices[2]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[1]].Position });
                            CreateWall(new Vector3[] { m_ControlPoints[indices[7]].Position, oneLine[1], oneLine[1], m_ControlPoints[indices[6]].Position });
                            CreateWall(new Vector3[] { m_ControlPoints[indices[5]].Position, oneLine[2], oneLine[2], m_ControlPoints[indices[4]].Position });
                        }
                        else
                        {
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[1]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[2]].Position }, false, true, false, false);
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[6]].Position, oneLine[1], oneLine[1], m_ControlPoints[indices[7]].Position }, false, true, false, false);
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[4]].Position, oneLine[2], oneLine[2], m_ControlPoints[indices[5]].Position }, false, true, false, false);

                        }
                    }
                    else if (m_ControlPoints.IsUShaped(out int[] uPointIndices))
                    {
                        if (!m_IsOpen)
                        {
                            Vector3 up1 = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                            Vector3 up2 = Vector3.Lerp(oneLine[3], oneLine[2], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], up1, m_GableScale);
                            oneLine[3] = Vector3.Lerp(oneLine[3], up2, m_GableScale);
                        }

                        int[] indices = m_ControlPoints.RelativeIndices(uPointIndices[0]);

                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[0]].Position, oneLine[1], oneLine[2], m_ControlPoints[indices[1]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[1]].Position, oneLine[2], oneLine[3], m_ControlPoints[indices[2]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[3]].Position, oneLine[3], oneLine[2], m_ControlPoints[indices[4]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[4]].Position, oneLine[2], oneLine[1], m_ControlPoints[indices[5]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[5]].Position, oneLine[1], oneLine[0], m_ControlPoints[indices[6]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[7]].Position, oneLine[0], oneLine[1], m_ControlPoints[indices[0]].Position }, false, true, true, false);

                        if(m_IsOpen)
                        {
                            CreateWall(new Vector3[] { m_ControlPoints[indices[7]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[6]].Position });
                            CreateWall(new Vector3[] { m_ControlPoints[indices[3]].Position, oneLine[3], oneLine[3], m_ControlPoints[indices[2]].Position });
                        }
                        else
                        {
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[2]].Position, oneLine[3], oneLine[3], m_ControlPoints[indices[3]].Position }, false, true, false, false);
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[6]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[7]].Position }, false, true, false, false);
                        }

                    }

                    else if (m_ControlPoints.IsYShaped(out int[] yPointIndices))
                    {
                        if (!m_IsOpen)
                        {
                            Vector3 yp1 = Vector3.Lerp(oneLine[0], oneLine[3], 0.5f);
                            Vector3 yp2 = Vector3.Lerp(oneLine[1], oneLine[3], 0.5f);
                            Vector3 yp3 = Vector3.Lerp(oneLine[2], oneLine[3], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], yp1, m_GableScale);
                            oneLine[1] = Vector3.Lerp(oneLine[1], yp2, m_GableScale);
                            oneLine[2] = Vector3.Lerp(oneLine[2], yp3, m_GableScale);
                        }

                        int[] indices = m_ControlPoints.RelativeIndices(yPointIndices[0]);

                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[0]].Position, oneLine[3], oneLine[0], m_ControlPoints[indices[1]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[2]].Position, oneLine[0], oneLine[3], m_ControlPoints[indices[3]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[3]].Position, oneLine[3], oneLine[1], m_ControlPoints[indices[4]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[5]].Position, oneLine[1], oneLine[3], m_ControlPoints[indices[6]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[6]].Position, oneLine[3], oneLine[2], m_ControlPoints[indices[7]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[8]].Position, oneLine[2], oneLine[3], m_ControlPoints[indices[0]].Position }, false, true, true, false);

                        if (m_IsOpen)
                        {
                            CreateWall(new Vector3[] { m_ControlPoints[indices[2]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[1]].Position });
                            CreateWall(new Vector3[] { m_ControlPoints[indices[5]].Position, oneLine[1], oneLine[1], m_ControlPoints[indices[4]].Position });
                            CreateWall(new Vector3[] { m_ControlPoints[indices[8]].Position, oneLine[2], oneLine[2], m_ControlPoints[indices[7]].Position });
                        }
                        else
                        {
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[1]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[2]].Position }, false, true, false, false);
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[4]].Position, oneLine[1], oneLine[1], m_ControlPoints[indices[5]].Position }, false, true, false, false);
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[7]].Position, oneLine[2], oneLine[2], m_ControlPoints[indices[8]].Position }, false, true, false, false);
                        }
                    }
                    break;
                case 5:
                    if(m_ControlPoints.IsFShaped(out int[] fIndices))
                    {
                        if (!m_IsOpen)
                        {
                            Vector3 a = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                            Vector3 b = Vector3.Lerp(oneLine[2], oneLine[1], 0.5f);
                            Vector3 c = Vector3.Lerp(oneLine[4], oneLine[3], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], a, m_GableScale);
                            oneLine[2] = Vector3.Lerp(oneLine[2], b, m_GableScale);
                            oneLine[4] = Vector3.Lerp(oneLine[4], c, m_GableScale);
                        }

                        int[] indices = m_ControlPoints.RelativeIndices(fIndices[0]);

                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[9]].Position, oneLine[0], oneLine[1], m_ControlPoints[indices[0]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[0]].Position, oneLine[1], oneLine[2], m_ControlPoints[indices[1]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[2]].Position, oneLine[2], oneLine[1], m_ControlPoints[indices[3]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[3]].Position, oneLine[1], oneLine[3], m_ControlPoints[indices[4]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[4]].Position, oneLine[3], oneLine[4], m_ControlPoints[indices[5]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[6]].Position, oneLine[4], oneLine[3], m_ControlPoints[indices[7]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[7]].Position, oneLine[3], oneLine[0], m_ControlPoints[indices[8]].Position }, false, true, false, true);

                        if (m_IsOpen)
                        {
                            CreateWall(new Vector3[] { m_ControlPoints[indices[9]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[8]].Position });
                            CreateWall(new Vector3[] { m_ControlPoints[indices[2]].Position, oneLine[2], oneLine[2], m_ControlPoints[indices[1]].Position });
                            CreateWall(new Vector3[] { m_ControlPoints[indices[6]].Position, oneLine[4], oneLine[4], m_ControlPoints[indices[5]].Position });
                        }
                        else
                        {
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[8]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[9]].Position });
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[1]].Position, oneLine[2], oneLine[2], m_ControlPoints[indices[2]].Position });
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[5]].Position, oneLine[4], oneLine[4], m_ControlPoints[indices[6]].Position });
                        }
                    }

                    else if (m_ControlPoints.IsSimpleMShaped(out int[] simpleMPointIndices))
                    {

                        if (!m_IsOpen)
                        {
                            Vector3 simpleMP1 = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                            Vector3 simpleMP2 = Vector3.Lerp(oneLine[^1], oneLine[^2], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], simpleMP1, m_GableScale);
                            oneLine[^1] = Vector3.Lerp(oneLine[^1], simpleMP2, m_GableScale);
                        }

                        // Define a map of the indices from a single ref to a concave point.
                        int[] indices = m_ControlPoints.RelativeIndices(simpleMPointIndices[0]);

                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[0]].Position, oneLine[1], oneLine[2], m_ControlPoints[indices[1]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[1]].Position, oneLine[2], oneLine[3], m_ControlPoints[indices[2]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[2]].Position, oneLine[3], oneLine[4], m_ControlPoints[indices[3]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[4]].Position, oneLine[4], oneLine[3], m_ControlPoints[indices[5]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[5]].Position, oneLine[3], oneLine[2], m_ControlPoints[indices[6]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[6]].Position, oneLine[2], oneLine[1], m_ControlPoints[indices[7]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[7]].Position, oneLine[1], oneLine[0], m_ControlPoints[indices[8]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[9]].Position, oneLine[0], oneLine[1], m_ControlPoints[indices[0]].Position }, false, true, true, false);

                        if (m_IsOpen)
                        {
                            CreateWall(new Vector3[] { m_ControlPoints[indices[9]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[8]].Position });
                            CreateWall(new Vector3[] { m_ControlPoints[indices[4]].Position, oneLine[4], oneLine[4], m_ControlPoints[indices[3]].Position });
                        }
                        else
                        {
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[3]].Position, oneLine[4], oneLine[4], m_ControlPoints[indices[4]].Position }, false, true, false, false);
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[8]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[9]].Position }, false, true, false, false);
                        }

                    }
                    else if (m_ControlPoints.IsXShaped(out int[] xPointIndices))
                    {
                        if (!m_IsOpen)
                        {
                            Vector3 mid = ProMaths.Average(oneLine);

                            for (int i = 0; i < oneLine.Length - 1; i++)
                            {
                                Vector3 xP = Vector3.Lerp(oneLine[i], mid, 0.5f);
                                oneLine[i] = Vector3.Lerp(oneLine[i], xP, GableScale);
                            }

                        }

                        for (int i = 0; i < xPointIndices.Length; i++)
                        {
                            int current = xPointIndices[i];
                            int previous = m_ControlPoints.GetPrevious(current);
                            int next = m_ControlPoints.GetNext(current);
                            int next1 = m_ControlPoints.GetNext(next);

                            int previousOneLine = oneLine.GetPreviousControlPoint(i);
                            previousOneLine = previousOneLine == oneLine.Length - 1 ? previousOneLine - 1 : previousOneLine;

                            CreateRoofTile(new Vector3[] { m_ControlPoints[current].Position, oneLine[4], oneLine[i], m_ControlPoints[next].Position }, false, true, false, true);
                            CreateRoofTile(new Vector3[] { m_ControlPoints[previous].Position, oneLine[previousOneLine], oneLine[4], m_ControlPoints[current].Position }, false, true, true, false);

                            if (m_IsOpen)
                            {
                                CreateWall(new Vector3[] { m_ControlPoints[next1].Position, oneLine[i], oneLine[i], m_ControlPoints[next].Position });
                            }
                            else
                            {
                                CreateRoofTile(new Vector3[] { m_ControlPoints[next].Position, oneLine[i], oneLine[i], m_ControlPoints[next1].Position }, false, true, false, false);
                            }
                        }

                    }
                    break;
                case 6:
                    if (m_ControlPoints.IsNShaped(out int[] nPointIndices))
                    {
                        if (!m_IsOpen)
                        {
                            Vector3 nP1 = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                            Vector3 nP2 = Vector3.Lerp(oneLine[^1], oneLine[^2], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], nP1, m_GableScale);
                            oneLine[^1] = Vector3.Lerp(oneLine[^1], nP2, m_GableScale);
                        }

                        int[] indices = m_ControlPoints.RelativeIndices(nPointIndices[0]);

                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[0]].Position, oneLine[1], oneLine[2], m_ControlPoints[indices[0]].Position }, false, false, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[0]].Position, oneLine[2], oneLine[3], m_ControlPoints[indices[1]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[1]].Position, oneLine[3], oneLine[4], m_ControlPoints[indices[2]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[2]].Position, oneLine[4], oneLine[5], m_ControlPoints[indices[3]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[4]].Position, oneLine[5], oneLine[4], m_ControlPoints[indices[5]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[5]].Position, oneLine[3], oneLine[2], m_ControlPoints[indices[6]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[5]].Position, oneLine[4], oneLine[3], m_ControlPoints[indices[5]].Position }, false, false, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[6]].Position, oneLine[2], oneLine[1], m_ControlPoints[indices[7]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[7]].Position, oneLine[1], oneLine[0], m_ControlPoints[indices[8]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[9]].Position, oneLine[0], oneLine[1], m_ControlPoints[indices[0]].Position }, false, true, true, false);

                        if(m_IsOpen)
                        {
                            CreateWall(new Vector3[] { m_ControlPoints[indices[9]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[8]].Position });
                            CreateWall(new Vector3[] { m_ControlPoints[indices[4]].Position, oneLine[5], oneLine[5], m_ControlPoints[indices[3]].Position });
                        }
                        else
                        {
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[8]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[9]].Position }, false, true, false, false);
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[3]].Position, oneLine[5], oneLine[5], m_ControlPoints[indices[4]].Position }, false, true, false, false);
                        }

                    }
                    else if (m_ControlPoints.IsEShaped(out int[] ePointIndices))
                    {
                        if (!m_IsOpen)
                        {
                            Vector3 eP1 = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                            Vector3 eP2 = Vector3.Lerp(oneLine[3], oneLine[2], 0.5f);
                            Vector3 eP3 = Vector3.Lerp(oneLine[5], oneLine[4], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], eP1, m_GableScale);
                            oneLine[3] = Vector3.Lerp(oneLine[3], eP2, m_GableScale);
                            oneLine[5] = Vector3.Lerp(oneLine[5], eP3, m_GableScale);
                        }

                        int[] indices = m_ControlPoints.RelativeIndices(ePointIndices[0]);
                        
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[0]].Position, oneLine[1], oneLine[4], m_ControlPoints[indices[1]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[1]].Position, oneLine[4], oneLine[5], m_ControlPoints[indices[2]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[3]].Position, oneLine[5], oneLine[4], m_ControlPoints[indices[4]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[4]].Position, oneLine[4], oneLine[2], m_ControlPoints[indices[5]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[5]].Position, oneLine[2], oneLine[3], m_ControlPoints[indices[6]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[7]].Position, oneLine[3], oneLine[2], m_ControlPoints[indices[8]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[8]].Position, oneLine[2], oneLine[1], m_ControlPoints[indices[9]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[9]].Position, oneLine[1], oneLine[0], m_ControlPoints[indices[10]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[11]].Position, oneLine[0], oneLine[1], m_ControlPoints[indices[0]].Position }, false, true, true, false);

                        if(m_IsOpen)
                        {
                            CreateWall(new Vector3[] { m_ControlPoints[indices[11]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[10]].Position });
                            CreateWall(new Vector3[] { m_ControlPoints[indices[3]].Position, oneLine[5], oneLine[5], m_ControlPoints[indices[2]].Position });
                            CreateWall(new Vector3[] { m_ControlPoints[indices[7]].Position, oneLine[3], oneLine[3], m_ControlPoints[indices[6]].Position });
                        }
                        else
                        {
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[10]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[11]].Position }, false, true, false, false); 
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[2]].Position, oneLine[5], oneLine[5], m_ControlPoints[indices[3]].Position }, false, true, false, false); 
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[6]].Position, oneLine[3], oneLine[3], m_ControlPoints[indices[7]].Position }, false, true, false, false);
                        }

                    }
                    else if(m_ControlPoints.IsHShaped(out int[] hPointIndices))
                    {
                        if (!m_IsOpen)
                        {
                            Vector3 a = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                            Vector3 b = Vector3.Lerp(oneLine[2], oneLine[1], 0.5f);
                            Vector3 c = Vector3.Lerp(oneLine[3], oneLine[4], 0.5f);
                            Vector3 d = Vector3.Lerp(oneLine[5], oneLine[4], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], a, m_GableScale);
                            oneLine[2] = Vector3.Lerp(oneLine[2], b, m_GableScale);
                            oneLine[3] = Vector3.Lerp(oneLine[3], c, m_GableScale);
                            oneLine[5] = Vector3.Lerp(oneLine[5], d, m_GableScale);
                        }

                        int[] indices = m_ControlPoints.RelativeIndices(hPointIndices[0]);

                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[11]].Position, oneLine[0], oneLine[1], m_ControlPoints[indices[0]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[0]].Position, oneLine[1], oneLine[4], m_ControlPoints[indices[1]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[1]].Position, oneLine[4], oneLine[5], m_ControlPoints[indices[2]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[3]].Position, oneLine[5], oneLine[3], m_ControlPoints[indices[4]].Position }, false, true, true, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[5]].Position, oneLine[3], oneLine[4], m_ControlPoints[indices[6]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[6]].Position, oneLine[4], oneLine[1], m_ControlPoints[indices[7]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[7]].Position, oneLine[1], oneLine[2], m_ControlPoints[indices[8]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[9]].Position, oneLine[2], oneLine[0], m_ControlPoints[indices[10]].Position }, false, true, true, true);

                        if (m_IsOpen)
                        {
                            CreateWall(new Vector3[] { m_ControlPoints[indices[11]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[10]].Position });
                            CreateWall(new Vector3[] { m_ControlPoints[indices[9]].Position, oneLine[2], oneLine[2], m_ControlPoints[indices[8]].Position });
                            CreateWall(new Vector3[] { m_ControlPoints[indices[5]].Position, oneLine[3], oneLine[3], m_ControlPoints[indices[4]].Position });
                            CreateWall(new Vector3[] { m_ControlPoints[indices[3]].Position, oneLine[5], oneLine[5], m_ControlPoints[indices[2]].Position });
                        }
                        else
                        {
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[10]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[11]].Position });
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[8]].Position, oneLine[2], oneLine[2], m_ControlPoints[indices[9]].Position });
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[4]].Position, oneLine[3], oneLine[3], m_ControlPoints[indices[5]].Position });
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[2]].Position, oneLine[5], oneLine[5], m_ControlPoints[indices[3]].Position });
                        }
                    }
                    else if(m_ControlPoints.IsKShaped(out int[] kPointIndices))
                    {
                        if (!m_IsOpen)
                        {
                            Vector3 a = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                            Vector3 b = Vector3.Lerp(oneLine[2], oneLine[1], 0.5f);
                            Vector3 c = Vector3.Lerp(oneLine[3], oneLine[5], 0.5f);
                            Vector3 d = Vector3.Lerp(oneLine[4], oneLine[5], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], a, m_GableScale);
                            oneLine[2] = Vector3.Lerp(oneLine[2], b, m_GableScale);
                            oneLine[3] = Vector3.Lerp(oneLine[3], c, m_GableScale);
                            oneLine[4] = Vector3.Lerp(oneLine[4], d, m_GableScale);
                        }


                        int[] indices = m_ControlPoints.RelativeIndices(kPointIndices[0]);

                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[11]].Position, oneLine[0], oneLine[1], m_ControlPoints[indices[0]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[0]].Position, oneLine[1], oneLine[5], m_ControlPoints[indices[1]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[1]].Position, oneLine[5], oneLine[4], m_ControlPoints[indices[2]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[3]].Position, oneLine[4], oneLine[5], m_ControlPoints[indices[4]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[4]].Position, oneLine[5], oneLine[3], m_ControlPoints[indices[5]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[6]].Position, oneLine[3], oneLine[1], m_ControlPoints[indices[7]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[7]].Position, oneLine[1], oneLine[2], m_ControlPoints[indices[8]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[9]].Position, oneLine[2], oneLine[0], m_ControlPoints[indices[10]].Position }, false, true, true, true);

                        if (m_IsOpen)
                        {
                            CreateWall(new Vector3[] { m_ControlPoints[indices[11]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[10]].Position });
                            CreateWall(new Vector3[] { m_ControlPoints[indices[9]].Position, oneLine[2], oneLine[2], m_ControlPoints[indices[8]].Position });
                            CreateWall(new Vector3[] { m_ControlPoints[indices[6]].Position, oneLine[3], oneLine[3], m_ControlPoints[indices[5]].Position });
                            CreateWall(new Vector3[] { m_ControlPoints[indices[3]].Position, oneLine[4], oneLine[4], m_ControlPoints[indices[2]].Position });
                        }
                        else
                        {
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[10]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[11]].Position });
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[8]].Position, oneLine[2], oneLine[2], m_ControlPoints[indices[9]].Position });
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[5]].Position, oneLine[3], oneLine[3], m_ControlPoints[indices[6]].Position });
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[2]].Position, oneLine[4], oneLine[4], m_ControlPoints[indices[3]].Position });
                        }

                    }
                    break;
                case 7:
                    if (m_ControlPoints.IsMShaped(out int[] mPointIndices))
                    {
                        if (!m_IsOpen)
                        {
                            Vector3 mP1 = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                            Vector3 mP2 = Vector3.Lerp(oneLine[^1], oneLine[^2], 0.5f);

                            oneLine[0] = Vector3.Lerp(oneLine[0], mP1, m_GableScale);
                            oneLine[^1] = Vector3.Lerp(oneLine[^1], mP2, m_GableScale);
                        }

                        int[] indices = m_ControlPoints.RelativeIndices(mPointIndices[0]);

                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[0]].Position, oneLine[1], oneLine[2], m_ControlPoints[indices[0]].Position }, false, false, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[0]].Position, oneLine[2], oneLine[3], m_ControlPoints[indices[1]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[1]].Position, oneLine[3], oneLine[4], m_ControlPoints[indices[2]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[2]].Position, oneLine[4], oneLine[5], m_ControlPoints[indices[2]].Position }, false, false, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[2]].Position, oneLine[5], oneLine[6], m_ControlPoints[indices[3]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[4]].Position, oneLine[6], oneLine[5], m_ControlPoints[indices[5]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[5]].Position, oneLine[5], oneLine[4], m_ControlPoints[indices[6]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[6]].Position, oneLine[4], oneLine[3], m_ControlPoints[indices[7]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[7]].Position, oneLine[3], oneLine[2], m_ControlPoints[indices[8]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[8]].Position, oneLine[2], oneLine[1], m_ControlPoints[indices[9]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[9]].Position, oneLine[1], oneLine[0], m_ControlPoints[indices[10]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[11]].Position, oneLine[0], oneLine[1], m_ControlPoints[indices[0]].Position }, false, true, true, false);

                        if (m_IsOpen)
                        {
                            CreateWall(new Vector3[] { m_ControlPoints[indices[11]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[10]].Position });
                            CreateWall(new Vector3[] { m_ControlPoints[indices[4]].Position, oneLine[6], oneLine[6], m_ControlPoints[indices[3]].Position });
                        }
                        else
                        {
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[3]].Position, oneLine[6], oneLine[6], m_ControlPoints[indices[4]].Position }, false, true, false, false);
                            CreateRoofTile(new Vector3[] { m_ControlPoints[indices[10]].Position, oneLine[0], oneLine[0], m_ControlPoints[indices[11]].Position }, false, true, false, false);
                        }

                    }
                    break;
            }

            // Repeating patterns
            if(m_ControlPoints.IsZigZagShaped(out int[] zigZagIndices))
            {
                if (!m_IsOpen)
                {
                    Vector3 zigP1 = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                    Vector3 zigP2 = Vector3.Lerp(oneLine[^1], oneLine[^2], 0.5f);

                    oneLine[0] = Vector3.Lerp(oneLine[0], zigP1, m_GableScale);
                    oneLine[^1] = Vector3.Lerp(oneLine[^1], zigP2, m_GableScale);
                }

                int[] indices = m_ControlPoints.RelativeIndices(zigZagIndices[0]);
                int previous = indices[^1];
                int start = indices[^3];
                int end = indices[^2];
                
                for (int i = 0; i < oneLine.Length-1; i++)
                {
                    if (i == 0)
                    {
                        CreateRoofTile(new Vector3[] { m_ControlPoints[previous].Position, oneLine[i], oneLine[i + 1], m_ControlPoints[indices[i]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[start].Position, oneLine[i + 1], oneLine[i], m_ControlPoints[end].Position }, false, true, false, true);

                        if(m_IsOpen)
                        {
                            CreateWall(new Vector3[] { m_ControlPoints[previous].Position, oneLine[i], oneLine[i], m_ControlPoints[end].Position });
                            
                        }
                        else
                        {
                            CreateRoofTile(new Vector3[] { m_ControlPoints[end].Position, oneLine[i], oneLine[i], m_ControlPoints[previous].Position }, false, true, false, false);
                        }
                    }
                    else if(i == oneLine.Length-2)
                    {
                        CreateRoofTile(new Vector3[] { m_ControlPoints[previous].Position, oneLine[i], oneLine[i + 1], m_ControlPoints[indices[i]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[start].Position, oneLine[i + 1], oneLine[i], m_ControlPoints[end].Position }, false, true, true, false);
                        
                        if (m_IsOpen)
                        {
                            CreateWall(new Vector3[] { m_ControlPoints[end -1].Position, oneLine[i+1], oneLine[i+1], m_ControlPoints[previous + 1].Position });

                        }
                        else
                        {
                            CreateRoofTile(new Vector3[] { m_ControlPoints[previous + 1].Position, oneLine[i+1], oneLine[i+1], m_ControlPoints[end-1].Position }, false, true, false, false);
                        }
                    }
                    else
                    {
                        CreateRoofTile(new Vector3[] { m_ControlPoints[previous].Position, oneLine[i], oneLine[i + 1], m_ControlPoints[indices[i]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[start].Position, oneLine[i + 1], oneLine[i], m_ControlPoints[end].Position }, false, true, false, false);
                    }

                    previous = indices[i];
                    start--;
                    end--;
                }

            }
            else if(m_ControlPoints.IsCrenelShaped(out int[] crenelIndices))
            {
                if (!m_IsOpen)
                {
                    Vector3 crenelP1 = Vector3.Lerp(oneLine[0], oneLine[1], 0.5f);
                    Vector3 crenelP2 = Vector3.Lerp(oneLine[^1], oneLine[^2], 0.5f);

                    oneLine[0] = Vector3.Lerp(oneLine[0], crenelP1, m_GableScale);
                    oneLine[^1] = Vector3.Lerp(oneLine[^1], crenelP2, m_GableScale);

                    for (int i = 2; i < oneLine.Length -2; i+= 2)
                    {
                        Vector3 a = Vector3.Lerp(oneLine[i], oneLine[i+1], 0.5f);
                        oneLine[i+1] = Vector3.Lerp(oneLine[i+1], a, m_GableScale);
                    }
                }

                int[] startIndices = m_ControlPoints.RelativeIndices(crenelIndices[0]);
                int[] endIndices = m_ControlPoints.RelativeIndices(crenelIndices[^1]);

                CreateRoofTile(new Vector3[] { m_ControlPoints[startIndices[^1]].Position, oneLine[0], oneLine[1], m_ControlPoints[startIndices[0]].Position }, false, true, false, false);
                CreateRoofTile(new Vector3[] { m_ControlPoints[startIndices[^3]].Position, oneLine[1], oneLine[0], m_ControlPoints[startIndices[^2]].Position }, false, true, false, false);
                CreateRoofTile(new Vector3[] { m_ControlPoints[startIndices[^4]].Position, oneLine[^2], oneLine[1], m_ControlPoints[startIndices[^3]].Position }, false, true, false, false);
                CreateRoofTile(new Vector3[] { m_ControlPoints[startIndices[^5]].Position, oneLine[^1], oneLine[^2], m_ControlPoints[startIndices[^4]].Position }, false, true, false, false);
                CreateRoofTile(new Vector3[] { m_ControlPoints[endIndices[0]].Position, oneLine[^2], oneLine[^1], m_ControlPoints[endIndices[1]].Position }, false, true, false, false);
                CreateRoofTile(new Vector3[] { m_ControlPoints[startIndices[0]].Position, oneLine[1], oneLine[2], m_ControlPoints[startIndices[1]].Position }, false, true, false, false);

                if(m_IsOpen)
                {
                    CreateWall(new Vector3[] { m_ControlPoints[startIndices[^1]].Position, oneLine[0], oneLine[0], m_ControlPoints[startIndices[^2]].Position });
                    CreateWall(new Vector3[] { m_ControlPoints[endIndices[2]].Position, oneLine[^1], oneLine[^1], m_ControlPoints[endIndices[1]].Position });
                }
                else
                {
                    CreateRoofTile(new Vector3[] { m_ControlPoints[startIndices[^2]].Position, oneLine[0], oneLine[0], m_ControlPoints[startIndices[^1]].Position }, false, true, false, false);
                    CreateRoofTile(new Vector3[] { m_ControlPoints[endIndices[1]].Position, oneLine[^1], oneLine[^1], m_ControlPoints[endIndices[2]].Position }, false, true, false, false);
                }

                int fourCount = 0;
                int twoCount = 0;

                int length = ((crenelIndices.Length / 2) + 1) - 2;

                for(int i = 0; i < length; i++)
                {
                    CreateRoofTile(new Vector3[] { m_ControlPoints[startIndices[4 + fourCount]].Position, oneLine[2 + twoCount], oneLine[4 + twoCount], m_ControlPoints[startIndices[5 + fourCount]].Position }, false, true, false, false);
                    CreateRoofTile(new Vector3[] { m_ControlPoints[startIndices[1 + fourCount]].Position, oneLine[2 + twoCount], oneLine[3 + twoCount], m_ControlPoints[startIndices[2 + fourCount]].Position }, false, true, false, false);
                    CreateRoofTile(new Vector3[] { m_ControlPoints[startIndices[3 + fourCount]].Position, oneLine[3 + twoCount], oneLine[2 + twoCount], m_ControlPoints[startIndices[4 + fourCount]].Position }, false, true, false, false);

                    if (m_IsOpen)
                    {
                        CreateWall(new Vector3[] { m_ControlPoints[startIndices[3 + fourCount]].Position, oneLine[3 + twoCount], oneLine[3 + twoCount], m_ControlPoints[startIndices[2 + fourCount]].Position });
                    }
                    else
                    {
                        CreateRoofTile(new Vector3[] { m_ControlPoints[startIndices[2 + fourCount]].Position, oneLine[3 + twoCount], oneLine[3 + twoCount], m_ControlPoints[startIndices[3 + fourCount]].Position }, false, true, false, false);
                    }

                    fourCount += 4;
                    twoCount += 2;
                }
            }
            else if(m_ControlPoints.IsAsteriskShaped(out int[] asteriskPointIndices))
            {
                if(!m_IsOpen)
                {
                    for(int i = 0; i < oneLine.Length-1; i++)
                    {
                        Vector3 a = Vector3.Lerp(oneLine[i], oneLine[^1], 0.5f);
                        oneLine[i] = Vector3.Lerp(oneLine[i], a, m_GableScale);
                    }
                }

                int count = oneLine.Length-2;
                for(int i = 0; i < asteriskPointIndices.Length; i++)
                {
                    int current = asteriskPointIndices[i];
                    int previous = m_ControlPoints.GetIndex(current - 1);
                    int next = m_ControlPoints.GetIndex(current + 1);

                    CreateRoofTile(new Vector3[] { m_ControlPoints[current].Position, oneLine[^1], oneLine[i], m_ControlPoints[next].Position }, false, true, false, true);
                    CreateRoofTile(new Vector3[] { m_ControlPoints[previous].Position, oneLine[count], oneLine[^1], m_ControlPoints[current].Position }, false, true, true, false);
                    count++;

                    if(count > oneLine.Length-2)
                    {
                        count = 0;
                    }

                    int nextTwo = m_ControlPoints.GetIndex(current + 2);

                    if (m_IsOpen)
                    { 
                        CreateWall(new Vector3[] { m_ControlPoints[nextTwo].Position, oneLine[i], oneLine[i], m_ControlPoints[next].Position });
                    }
                    else
                    {
                        CreateRoofTile(new Vector3[] { m_ControlPoints[next].Position, oneLine[i], oneLine[i], m_ControlPoints[nextTwo].Position }, false, true, false, true);
                    }
                }
            }
            else if(m_ControlPoints.IsAntennaShaped(out int[] antPointIndices))
            {
                int length = oneLine.Length / 3;
                int bIndex = 0;
                int mIndex = 1;
                int tIndex = 2;
                for(int i = 0; i < length; i++)
                {
                    Vector3 a = Vector3.Lerp(oneLine[tIndex], oneLine[mIndex], 0.5f);
                    Vector3 b = Vector3.Lerp(oneLine[bIndex], oneLine[mIndex], 0.5f);

                    oneLine[tIndex] = Vector3.Lerp(oneLine[tIndex], a, m_GableScale);
                    oneLine[bIndex] = Vector3.Lerp(oneLine[bIndex], b, m_GableScale);

                    bIndex += 3;
                    mIndex += 3;
                    tIndex += 3;
                }

                // Middle Vertical
                int current = m_ControlPoints.GetIndex(antPointIndices[0] - 4);
                for(int i = 3; i < oneLine.Length - 3; i+= 3)
                {
                    int next = m_ControlPoints.GetIndex(current + 1);
                    int nextTwo = m_ControlPoints.GetIndex(current + 2);
                    int nextThree = m_ControlPoints.GetIndex(current + 3);
                    CreateRoofTile(new Vector3[] { m_ControlPoints[current].Position, oneLine[i+1], oneLine[i], m_ControlPoints[next].Position }, false, true, false, true);
                    CreateRoofTile(new Vector3[] { m_ControlPoints[nextTwo].Position, oneLine[i], oneLine[i+1], m_ControlPoints[nextThree].Position }, false, true, true, false);

                    if(m_IsOpen)
                    {
                        CreateWall(new Vector3[] { m_ControlPoints[nextTwo].Position, oneLine[i], oneLine[i], m_ControlPoints[next].Position });
                    }
                    else
                    {
                        CreateRoofTile(new Vector3[] { m_ControlPoints[next].Position, oneLine[i], oneLine[i], m_ControlPoints[nextTwo].Position }, false, true, false, false);
                    }

                    current = m_ControlPoints.GetIndex(current - 4);
                }

                current = m_ControlPoints.GetIndex(current - 3);
                for (int i = oneLine.Length - 4; i > 3; i -= 3)
                {
                    int previous = m_ControlPoints.GetIndex(current - 1);
                    int previousTwo = m_ControlPoints.GetIndex(current - 2);
                    int previousThree = m_ControlPoints.GetIndex(current - 3);
                    CreateRoofTile(new Vector3[] { m_ControlPoints[previous].Position, oneLine[i], oneLine[i-1], m_ControlPoints[current].Position }, false, true, true, false);
                    CreateRoofTile(new Vector3[] { m_ControlPoints[previousThree].Position, oneLine[i-1], oneLine[i], m_ControlPoints[previousTwo].Position }, false, true, false, true);

                    if (m_IsOpen)
                    {
                        CreateWall(new Vector3[] { m_ControlPoints[previous].Position, oneLine[i], oneLine[i], m_ControlPoints[previousTwo].Position });
                    }
                    else
                    {
                        CreateRoofTile(new Vector3[] { m_ControlPoints[previousTwo].Position, oneLine[i], oneLine[i], m_ControlPoints[previous].Position }, false, true, false, false);
                    }

                    current = m_ControlPoints.GetIndex(current - 4);
                }


                // Middle Horizontal
                current = m_ControlPoints.GetIndex(antPointIndices[0]);
                int nextFive = m_ControlPoints.GetIndex(current + 5);
                int index = 1;
                int loop = (oneLine.Length / 3) - 1;
                for (int i = 0; i < loop; i++)
                {
                    int previous = m_ControlPoints.GetIndex(current - 1);
                    int next = m_ControlPoints.GetIndex(nextFive + 1);

                    CreateRoofTile(new Vector3[] { m_ControlPoints[previous].Position, oneLine[index + 3], oneLine[index], m_ControlPoints[current].Position }, false, true, false, false);
                    CreateRoofTile(new Vector3[] { m_ControlPoints[nextFive].Position, oneLine[index], oneLine[index + 3], m_ControlPoints[next].Position }, false, true, false, false);

                    current = m_ControlPoints.GetIndex(current - 4);
                    nextFive = m_ControlPoints.GetIndex(nextFive + 4);
                    index += 3;
                }

                // Ends
                current = m_ControlPoints.GetIndex(antPointIndices[0]);
                index = 0;

                for (int i = 0; i < 2; i++)
                {
                    int next = m_ControlPoints.GetIndex(current + 1);
                    int nextTwo = m_ControlPoints.GetIndex(current + 2);
                    int nextThree = m_ControlPoints.GetIndex(current + 3);
                    int nextFour = m_ControlPoints.GetIndex(current + 4);
                    nextFive = m_ControlPoints.GetIndex(current + 5);

                    if(i == 0)
                    {
                        CreateRoofTile(new Vector3[] { m_ControlPoints[current].Position, oneLine[index + 1], oneLine[index], m_ControlPoints[next].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[nextTwo].Position, oneLine[index], oneLine[index + 2], m_ControlPoints[nextThree].Position }, false, true, true, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[nextFour].Position, oneLine[index + 2], oneLine[index + 1], m_ControlPoints[nextFive].Position }, false, true, true, false);

                        if (m_IsOpen)
                        {
                            CreateWall(new Vector3[] { m_ControlPoints[nextTwo].Position, oneLine[index], oneLine[index], m_ControlPoints[next].Position });
                            CreateWall(new Vector3[] { m_ControlPoints[nextFour].Position, oneLine[index + 2], oneLine[index + 2], m_ControlPoints[nextThree].Position });
                        }
                        else
                        {
                            CreateRoofTile(new Vector3[] { m_ControlPoints[next].Position, oneLine[index], oneLine[index], m_ControlPoints[nextTwo].Position }, false, true, false, false);
                            CreateRoofTile(new Vector3[] { m_ControlPoints[nextThree].Position, oneLine[index + 2], oneLine[index + 2], m_ControlPoints[nextFour].Position }, false, true, false, false);
                        }
                    }
                    else
                    {
                        CreateRoofTile(new Vector3[] { m_ControlPoints[current].Position, oneLine[index + 1], oneLine[index + 2], m_ControlPoints[next].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[nextTwo].Position, oneLine[index + 2], oneLine[index], m_ControlPoints[nextThree].Position }, false, true, true, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[nextFour].Position, oneLine[index], oneLine[index + 1], m_ControlPoints[nextFive].Position }, false, true, true, false);

                        if (m_IsOpen)
                        {
                            CreateWall(new Vector3[] { m_ControlPoints[nextTwo].Position, oneLine[index + 2], oneLine[index + 2], m_ControlPoints[next].Position });
                            CreateWall(new Vector3[] { m_ControlPoints[nextFour].Position, oneLine[index], oneLine[index], m_ControlPoints[nextThree].Position });
                        }
                        else
                        {
                            CreateRoofTile(new Vector3[] { m_ControlPoints[next].Position, oneLine[index + 2], oneLine[index + 2], m_ControlPoints[nextTwo].Position }, false, true, false, false);
                            CreateRoofTile(new Vector3[] { m_ControlPoints[nextThree].Position, oneLine[index], oneLine[index], m_ControlPoints[nextFour].Position }, false, true, false, false);
                        }
                    }

                    index += oneLine.Length - 3;
                    current = m_ControlPoints.GetIndex(current + (m_ControlPoints.Length/2));
                }
            }
        }

        if(!m_IsOpen)
        {
            foreach (RoofTile tile in m_Tiles)
            {
                tile.Initialize(m_TileHeight, m_TileExtend).Extend(tile.ExtendHeightBeginning, tile.ExtendHeightEnd, false, false).Build();
            }
        }

        if(m_FrameType == RoofType.Dormer)
        {
            foreach(RoofTile tile in m_Tiles)
            {
                tile.Initialize(m_TileHeight, m_TileExtend, false).Extend(tile.ExtendHeightBeginning, false, tile.ExtendWidthBeginning, tile.ExtendWidthEnd).Build();
            }

            foreach(Wall wall in m_Walls)
            {
                Vector3[] points = wall.ControlPoints;

                Vector3 dir = points[0].DirectionToTarget(points[3]);

                points[0] += dir * 0.5f;
                points[3] -= dir * 0.5f;

                wall.Initialize(points, wall.Depth, wall.Material).Build();

            }
        }
    }

    /// <summary>
    /// Only for a 4 walled building.
    /// </summary>
    private void BuildM()
    {//
        if (m_ControlPoints.Length != 4)
            return;

        Vector3[] mPointsA = new Vector3[4];
        Vector3[] mPointsB = new Vector3[4];


        if (m_IsFlipped)
        {
            mPointsA[1] = m_ControlPoints[0].Position;
            mPointsA[2] = m_ControlPoints[1].Position;

            mPointsB[0] = m_ControlPoints[3].Position;
            mPointsB[3] = m_ControlPoints[2].Position;

            Vector3 midA = Vector3.Lerp(m_ControlPoints[0].Position, m_ControlPoints[3].Position, 0.5f);
            Vector3 midB = Vector3.Lerp(m_ControlPoints[1].Position, m_ControlPoints[2].Position, 0.5f);

            Vector3 dirA = m_ControlPoints[0].Position.DirectionToTarget(m_ControlPoints[3].Position);
            Vector3 dirB = m_ControlPoints[1].Position.DirectionToTarget(m_ControlPoints[2].Position);

            mPointsA[0] = midA - (dirA * m_TileHeight);
            mPointsA[3] = midB - (dirB * m_TileHeight);

            mPointsB[1] = midA + (dirA * m_TileHeight);
            mPointsB[2] = midB + (dirB * m_TileHeight);

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

            mPointsA[1] = midA - (dirA * m_TileHeight);
            mPointsA[2] = midB - (dirB * m_TileHeight);

            mPointsB[0] = midA + (dirA * m_TileHeight);
            mPointsB[3] = midB + (dirB * m_TileHeight);
        }

        List<Vector3[]> mArrays = new List<Vector3[]>();
        mArrays.Add(mPointsA);
        mArrays.Add(mPointsB);

        int iterator = 0;
        foreach (Vector3[] m in mArrays)
        {
            if (m.IsPolygonDescribableInOneLine(out Vector3[] oneLine))
            {
                Vector3 start = oneLine[0] + (Vector3.up * m_GableHeight);
                Vector3 end = oneLine[1] + (Vector3.up * m_GableHeight);

                if (m_IsFlipped)
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
