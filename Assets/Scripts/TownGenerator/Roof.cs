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
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.ParticleSystem;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

[System.Serializable]
public class Roof : MonoBehaviour
{
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
    [SerializeField] private bool m_Rotate; // For M shaped
    public float GableHeight => m_GableHeight;
    public bool Rotate => m_Rotate;
    // End Gable

    [SerializeField, HideInInspector] private ControlPoint[] m_ControlPoints;
    [SerializeField, HideInInspector] private ControlPoint[] m_OriginalControlPoints;

    public RoofType FrameType => m_FrameType;

    private Vector3[] m_TempOneLine;

    public IEnumerable<ControlPoint> ControlPoints => m_ControlPoints;

    public void SetControlPoints(IEnumerable<ControlPoint> controlPoints)
    {
        ControlPoint[] points = controlPoints.ToArray();
        m_ControlPoints = PolygonRecognition.Clone(points.ToArray());
        m_OriginalControlPoints = PolygonRecognition.Clone(m_ControlPoints);
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
        m_GableHeight = 1;
        m_Rotate = false;
        m_PyramidHeight = 1;
        m_MansardHeight = 1;
        m_MansardScale = 1;
        m_FrameType = RoofType.Pyramid;
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
        m_GableHeight = roof.GableHeight;
        m_Rotate = roof.Rotate;
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
            throw new NotImplementedException();

        int OpenGable = (int)RoofType.OpenGable;
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
            case RoofType.OpenGable:
                BuildOpenGable();
                break;
            case RoofType.Mansard:
                BuildMansard();
                break;
            case RoofType.Dormer:
                BuildMansard();
                BuildOpenGable();
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
    private void BuildOpenGable()
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
                    CreateRoofTile(new Vector3[] { m_ControlPoints[3].Position, oneLine[1], oneLine[0], m_ControlPoints[0].Position });
                    CreateRoofTile(new Vector3[] { m_ControlPoints[1].Position, oneLine[0], oneLine[1], m_ControlPoints[2].Position });

                    CreateWall(new Vector3[] { m_ControlPoints[1].Position, oneLine[0], oneLine[0], m_ControlPoints[0].Position });
                    CreateWall(new Vector3[] { m_ControlPoints[3].Position, oneLine[1], oneLine[1], m_ControlPoints[2].Position });
                    break;
                case 3:
                    if (m_ControlPoints.IsLShaped(out int index))
                    {
                        int next = m_ControlPoints.GetNext(index);
                        int twoNext = m_ControlPoints.GetNext(next);
                        int threeNext = m_ControlPoints.GetNext(twoNext);
                        int onePrevious = m_ControlPoints.GetPrevious(index);
                        int twoPrevious = m_ControlPoints.GetPrevious(onePrevious);
                        int threePrevious = m_ControlPoints.GetPrevious(twoPrevious);

                        CreateRoofTile(new Vector3[] { m_ControlPoints[index].Position, oneLine[1], oneLine[0], m_ControlPoints[next].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[twoNext].Position, oneLine[0], oneLine[1], m_ControlPoints[threeNext].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[onePrevious].Position, oneLine[2], oneLine[1], m_ControlPoints[index].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[threeNext].Position, oneLine[1], oneLine[2], m_ControlPoints[twoPrevious].Position }, false, true, false, true);

                        CreateWall(new Vector3[] { m_ControlPoints[onePrevious].Position, oneLine[2], oneLine[2], m_ControlPoints[twoPrevious].Position });
                        CreateWall(new Vector3[] { m_ControlPoints[twoNext].Position, oneLine[0], oneLine[0], m_ControlPoints[next].Position });
                    }
                    break;
                case 4:
                    if (m_ControlPoints.IsTShaped(out int[] tPointIndices))
                    {
                        int tPoint0 = tPointIndices[0];
                        int tPoint1 = tPointIndices[1];
                        int tPoint0Previous = m_ControlPoints.GetPrevious(tPoint0);
                        int tPoint0Previous1 = m_ControlPoints.GetPrevious(tPoint0Previous);
                        int tPoint0Next = m_ControlPoints.GetNext(tPoint0);
                        int tPoint1Previous = m_ControlPoints.GetPrevious(tPoint1);
                        int tPoint1Next = m_ControlPoints.GetNext(tPoint1);
                        int tPoint1Next1 = m_ControlPoints.GetNext(tPoint1Next);

                        CreateRoofTile(new Vector3[] { m_ControlPoints[tPoint0].Position, oneLine[^1], oneLine[0], m_ControlPoints[tPoint0Next].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[tPoint1Previous].Position, oneLine[0], oneLine[^1], m_ControlPoints[tPoint1].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[tPoint0Previous].Position, oneLine[1], oneLine[^1], m_ControlPoints[tPoint0].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[tPoint1].Position, oneLine[^1], oneLine[2], m_ControlPoints[tPoint1Next].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[tPoint1Next1].Position, oneLine[2], oneLine[1], m_ControlPoints[tPoint0Previous1].Position });

                        CreateWall(new Vector3[] { m_ControlPoints[tPoint1Previous].Position, oneLine[0], oneLine[0], m_ControlPoints[tPoint0Next].Position });
                        CreateWall(new Vector3[] { m_ControlPoints[tPoint0Previous].Position, oneLine[1], oneLine[1], m_ControlPoints[tPoint0Previous1].Position });
                        CreateWall(new Vector3[] { m_ControlPoints[tPoint1Next1].Position, oneLine[2], oneLine[2], m_ControlPoints[tPoint1Next].Position });
                    }
                    else if (m_ControlPoints.IsUShaped(out int[] uPointIndices))
                    {
                        int uPoint0 = uPointIndices[0];
                        int uPoint1 = uPointIndices[1];
                        int uPoint0Previous = m_ControlPoints.GetPrevious(uPoint0);
                        int uPoint0Previous1 = m_ControlPoints.GetPrevious(uPoint0Previous);
                        int uPoint0Previous2 = m_ControlPoints.GetPrevious(uPoint0Previous1);
                        int uPoint1Next = m_ControlPoints.GetNext(uPoint1);
                        int uPoint1Next1 = m_ControlPoints.GetNext(uPoint1Next);
                        int uPoint1Next2 = m_ControlPoints.GetNext(uPoint1Next1);

                        CreateRoofTile(new Vector3[] { m_ControlPoints[uPoint0Previous].Position, oneLine[0], oneLine[1], m_ControlPoints[uPoint0].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[uPoint0Previous2].Position, oneLine[1], oneLine[0], m_ControlPoints[uPoint0Previous1].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[uPoint0].Position, oneLine[1], oneLine[2], m_ControlPoints[uPoint1].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[uPoint1Next2].Position, oneLine[2], oneLine[1], m_ControlPoints[uPoint0Previous2].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[uPoint1].Position, oneLine[2], oneLine[^1], m_ControlPoints[uPoint1Next].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[uPoint1Next1].Position, oneLine[^1], oneLine[2], m_ControlPoints[uPoint1Next2].Position }, false, true, true, false);

                        CreateWall(new Vector3[] { m_ControlPoints[uPoint0Previous].Position, oneLine[0], oneLine[0], m_ControlPoints[uPoint0Previous1].Position });
                        CreateWall(new Vector3[] { m_ControlPoints[uPoint1Next1].Position, oneLine[^1], oneLine[^1], m_ControlPoints[uPoint1Next].Position });
                    }
                    else if(m_ControlPoints.IsSimpleNShaped(out int[] simpleNPointIndices))
                    {
                        int simpleNPoint0 = simpleNPointIndices[0];
                        int simpleNPoint0Previous = m_ControlPoints.GetPrevious(simpleNPoint0);
                        int simpleNPoint0Next = m_ControlPoints.GetNext(simpleNPoint0);
                        int simpleNPoint0Next1 = m_ControlPoints.GetNext(simpleNPoint0Next);

                        int simpleNPoint1 = simpleNPointIndices[1];
                        int simpleNPoint1Previous = m_ControlPoints.GetPrevious(simpleNPoint1);
                        int simpleNPoint1Next = m_ControlPoints.GetNext(simpleNPoint1);
                        int simpleNPoint1Next1 = m_ControlPoints.GetNext(simpleNPoint1Next);

                        CreateRoofTile(new Vector3[] { m_ControlPoints[simpleNPoint0Previous].Position, oneLine[0], oneLine[1], m_ControlPoints[simpleNPoint0].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[simpleNPoint0].Position, oneLine[1], oneLine[2], m_ControlPoints[simpleNPoint0Next].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[simpleNPoint0Next].Position, oneLine[2], oneLine[3], m_ControlPoints[simpleNPoint0Next1].Position }, false, true, false, true);

                        CreateRoofTile(new Vector3[] { m_ControlPoints[simpleNPoint1Previous].Position, oneLine[3], oneLine[2], m_ControlPoints[simpleNPoint1].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[simpleNPoint1].Position, oneLine[2], oneLine[1], m_ControlPoints[simpleNPoint1Next].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[simpleNPoint1Next].Position, oneLine[1], oneLine[0], m_ControlPoints[simpleNPoint1Next1].Position }, false, true, false, true);

                        CreateWall(new Vector3[] { m_ControlPoints[simpleNPoint0Previous].Position, oneLine[0], oneLine[0], m_ControlPoints[simpleNPoint1Next1].Position });
                        CreateWall(new Vector3[] { m_ControlPoints[simpleNPoint1Previous].Position, oneLine[^1], oneLine[^1], m_ControlPoints[simpleNPoint0Next1].Position });

                    }
                    break;
                case 5:
                    if (m_ControlPoints.IsXShaped(out int[] xPointIndices))
                    {
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

                            CreateWall(new Vector3[] { m_ControlPoints[next1].Position, oneLine[i], oneLine[i], m_ControlPoints[next].Position });
                        }

                    }
                    if(m_ControlPoints.IsSimpleMShaped(out int[] simpleMPointIndices))
                    {
                        // Define a map of the indices from a single ref to a concave point.
                        int[] indices = new int[m_ControlPoints.Length];
                        indices[0] = simpleMPointIndices[0];
                        
                        for(int i = 1; i < indices.Length; i++)
                        {
                            indices[i] = m_ControlPoints.GetPrevious(indices[i - 1]);
                        }

                        int simpleMPoint0 = simpleMPointIndices[0];
                        int simpleMPoint1 = simpleMPointIndices[1];
                        int simpleMPoint2 = simpleMPointIndices[2];
                        int simpleMPoint0Previous1 = m_ControlPoints.GetIndex(simpleMPoint0 - 1);
                        int simpleMPoint0Previous2 = m_ControlPoints.GetIndex(simpleMPoint0 - 2);
                        int simpleMPoint0Previous3 = m_ControlPoints.GetIndex(simpleMPoint0 - 3);
                        int simpleMPoint1Previous1 = m_ControlPoints.GetIndex(simpleMPoint1 - 1);
                        int simpleMPoint1Previous2 = m_ControlPoints.GetIndex(simpleMPoint1 - 2);
                        int simpleMPoint2Previous1 = m_ControlPoints.GetIndex(simpleMPoint2 - 1);
                        int simpleMPoint2Next1 = m_ControlPoints.GetIndex(simpleMPoint2 + 1);

                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[1]].Position, oneLine[0], oneLine[1], m_ControlPoints[indices[0]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[3]].Position, oneLine[1], oneLine[0], m_ControlPoints[indices[2]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[4]].Position, oneLine[2], oneLine[1], m_ControlPoints[indices[3]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[5]].Position, oneLine[3], oneLine[2], m_ControlPoints[indices[4]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[6]].Position, oneLine[4], oneLine[3], m_ControlPoints[indices[5]].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[8]].Position, oneLine[3], oneLine[4], m_ControlPoints[indices[7]].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[9]].Position, oneLine[2], oneLine[3], m_ControlPoints[indices[8]].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[indices[0]].Position, oneLine[1], oneLine[2], m_ControlPoints[indices[9]].Position }, false, true, false, false);

                        CreateWall(new Vector3[] { m_ControlPoints[simpleMPoint0Previous1].Position, oneLine[0], oneLine[0], m_ControlPoints[simpleMPoint0Previous2].Position });
                        CreateWall(new Vector3[] { m_ControlPoints[simpleMPoint1Previous2].Position, oneLine[4], oneLine[4], m_ControlPoints[simpleMPoint2Next1].Position });
                    }
                    break;
                case 6:
                    if (m_ControlPoints.IsNShaped(out int[] nPointIndices))
                    {
                        int nPoint0 = nPointIndices[0];
                        int nPoint1 = nPointIndices[1];
                        int nPoint0Previous = m_ControlPoints.GetPrevious(nPoint0);
                        int nPoint0Previous1 = m_ControlPoints.GetPrevious(nPoint0Previous);
                        int nPoint0Previous2 = m_ControlPoints.GetPrevious(nPoint0Previous1);
                        int nPoint1Previous = m_ControlPoints.GetPrevious(nPoint1);
                        int nPoint1Previous1 = m_ControlPoints.GetPrevious(nPoint1Previous);
                        int nPoint1Next = m_ControlPoints.GetNext(nPoint1);
                        int nPoint0Next = m_ControlPoints.GetNext(nPoint0);
                        int nPoint0Next1 = m_ControlPoints.GetNext(nPoint0Next);

                        CreateRoofTile(new Vector3[] { m_ControlPoints[nPoint0Previous].Position, oneLine[0], oneLine[1], m_ControlPoints[nPoint0].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[nPoint0Previous2].Position, oneLine[1], oneLine[0], m_ControlPoints[nPoint0Previous1].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[nPoint0].Position, oneLine[1], oneLine[2], m_ControlPoints[nPoint0].Position }, false, false, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[nPoint1Next].Position, oneLine[2], oneLine[1], m_ControlPoints[nPoint0Previous2].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[nPoint1].Position, oneLine[3], oneLine[2], m_ControlPoints[nPoint1Next].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[nPoint0].Position, oneLine[2], oneLine[3], m_ControlPoints[nPoint0Next].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[nPoint0Next].Position, oneLine[3], oneLine[4], m_ControlPoints[nPoint0Next1].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[nPoint1].Position, oneLine[4], oneLine[3], m_ControlPoints[nPoint1].Position }, false, false, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[nPoint1Previous].Position, oneLine[5], oneLine[4], m_ControlPoints[nPoint1].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[nPoint0Next1].Position, oneLine[4], oneLine[5], m_ControlPoints[nPoint1Previous1].Position }, false, true, false, true);

                        CreateWall(new Vector3[] { m_ControlPoints[nPoint0Previous].Position, oneLine[0], oneLine[0], m_ControlPoints[nPoint0Previous1].Position });
                        CreateWall(new Vector3[] { m_ControlPoints[nPoint1Previous].Position, oneLine[^1], oneLine[^1], m_ControlPoints[nPoint1Previous1].Position });
                    }
                    else if (m_ControlPoints.IsEShaped(out int[] ePointIndices))
                    {
                        int ePoint0 = ePointIndices[0];
                        int ePoint0Previous = m_ControlPoints.GetPrevious(ePoint0);
                        int ePoint0Previous1 = m_ControlPoints.GetPrevious(ePoint0Previous);
                        int ePoint0Previous2 = m_ControlPoints.GetPrevious(ePoint0Previous1);

                        int ePoint1 = ePointIndices[1];
                        int ePoint1Next = m_ControlPoints.GetNext(ePointIndices[1]);

                        int ePoint2 = ePointIndices[2];
                        int ePoint2Previous = m_ControlPoints.GetPrevious(ePointIndices[2]);

                        int ePoint3 = ePointIndices[3];
                        int ePoint3Next = m_ControlPoints.GetNext(ePointIndices[3]);
                        int ePoint3Next1 = m_ControlPoints.GetNext(ePoint3Next);
                        int ePoint3Next2 = m_ControlPoints.GetNext(ePoint3Next1);

                        CreateRoofTile(new Vector3[] { m_ControlPoints[ePoint0Previous].Position, oneLine[0], oneLine[1], m_ControlPoints[ePoint0].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[ePoint0Previous2].Position, oneLine[1], oneLine[0], m_ControlPoints[ePoint0Previous1].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[ePoint3Next2].Position, oneLine[2], oneLine[1], m_ControlPoints[ePoint0Previous2].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[ePoint0].Position, oneLine[1], oneLine[4], m_ControlPoints[ePoint1].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[ePoint1].Position, oneLine[4], oneLine[5], m_ControlPoints[ePoint1Next].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[ePoint2Previous].Position, oneLine[5], oneLine[4], m_ControlPoints[ePoint2].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[ePoint2].Position, oneLine[4], oneLine[2], m_ControlPoints[ePoint3].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[ePoint3].Position, oneLine[2], oneLine[3], m_ControlPoints[ePoint3Next].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[ePoint3Next1].Position, oneLine[3], oneLine[2], m_ControlPoints[ePoint3Next2].Position }, false, true, true, false);

                        CreateWall(new Vector3[] { m_ControlPoints[ePoint0Previous].Position, oneLine[0], oneLine[0], m_ControlPoints[ePoint0Previous1].Position });
                        CreateWall(new Vector3[] { m_ControlPoints[ePoint2Previous].Position, oneLine[5], oneLine[5], m_ControlPoints[ePoint1Next].Position });
                        CreateWall(new Vector3[] { m_ControlPoints[ePoint3Next1].Position, oneLine[3], oneLine[3], m_ControlPoints[ePoint3Next].Position });
                    }
                    break;
                case 7:
                    if (m_ControlPoints.IsMShaped(out int[] mPointIndices))
                    {
                        int mPoint0 = mPointIndices[0];
                        int mPoint0Previous = m_ControlPoints.GetPrevious(mPoint0);
                        int mPoint0Previous1 = m_ControlPoints.GetPrevious(mPoint0Previous);
                        int mPoint0Previous2 = m_ControlPoints.GetPrevious(mPoint0Previous1);
                        int mPoint1 = mPointIndices[1];
                        int mPoint1Previous = m_ControlPoints.GetPrevious(mPoint1);
                        int mPoint1Previous1 = m_ControlPoints.GetPrevious(mPoint1Previous);
                        int mPoint1Previous2 = m_ControlPoints.GetPrevious(mPoint1Previous1);
                        int mPoint1Next = m_ControlPoints.GetNext(mPoint1);
                        int mPoint2 = mPointIndices[2];
                        int mPoint2Previous = m_ControlPoints.GetPrevious(mPoint2);
                        int mPoint2Next = m_ControlPoints.GetNext(mPoint2);

                        CreateRoofTile(new Vector3[] { m_ControlPoints[mPoint0Previous].Position, oneLine[0], oneLine[1], m_ControlPoints[mPoint0].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[mPoint0Previous2].Position, oneLine[1], oneLine[0], m_ControlPoints[mPoint0Previous1].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[mPoint1Next].Position, oneLine[2], oneLine[1], m_ControlPoints[mPoint0Previous2].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[mPoint1].Position, oneLine[3], oneLine[2], m_ControlPoints[mPoint1Next].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[mPoint1Previous].Position, oneLine[4], oneLine[3], m_ControlPoints[mPoint1].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[mPoint1Previous1].Position, oneLine[5], oneLine[4], m_ControlPoints[mPoint1Previous].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[mPoint1Previous2].Position, oneLine[6], oneLine[5], m_ControlPoints[mPoint1Previous1].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[mPoint2].Position, oneLine[5], oneLine[6], m_ControlPoints[mPoint2Next].Position }, false, true, false, true);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[mPoint2].Position, oneLine[4], oneLine[5], m_ControlPoints[mPoint2].Position }, false, false, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[mPoint2Previous].Position, oneLine[3], oneLine[4], m_ControlPoints[mPoint2].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[mPoint0].Position, oneLine[2], oneLine[3], m_ControlPoints[mPoint2Previous].Position }, false, true, false, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[mPoint0].Position, oneLine[1], oneLine[2], m_ControlPoints[mPoint0].Position }, false, false, false, false);
                        //
                        CreateWall(new Vector3[] { m_ControlPoints[mPoint0Previous].Position, oneLine[0], oneLine[0], m_ControlPoints[mPoint0Previous1].Position });
                        CreateWall(new Vector3[] { m_ControlPoints[mPoint1Previous2].Position, oneLine[6], oneLine[6], m_ControlPoints[mPoint2Next].Position });

                    }
                    break;
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


        if (m_Rotate)
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

                if (m_Rotate)
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

        for (int i = 0; i < m_ControlPoints.Length; i++)
        {
            Handles.Label(m_ControlPoints[i].Position + transform.localPosition, i.ToString());
        }

        if (m_TempOneLine == null)
            return;

        foreach (Vector3 point in m_TempOneLine)
        {
            Handles.DrawSolidDisc(point, Vector3.up, 0.1f);
        }

    }
}
