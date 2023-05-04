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

[System.Serializable]
public class Roof : MonoBehaviour
{
    [SerializeField] private RoofType m_FrameType;

    private GameObject m_SupportBeamPrefab;
    private GameObject m_CentreBeamPrefab;

    // Tile Data
    [SerializeField, Range(0, 10)] private float m_TileHeight;
    [SerializeField, Range(0, 10)] private float m_TileExtend;
    [SerializeField] private bool m_TileFlipFace;
    [SerializeField] private Material m_TileMaterial;
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
    [SerializeField, Range(-10, 10)] private float m_MansardScale;
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

    private Vector3[] m_TempVerts;

    public IEnumerable<ControlPoint> ControlPoints => m_ControlPoints;

    public void SetControlPoints(IEnumerable<ControlPoint> controlPoints)
    {
        m_ControlPoints = controlPoints.ToArray();
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
        m_TileHeight = 1;
        m_TileExtend = 0.2f;
        m_TileFlipFace = false;
        m_TileMaterial = BuiltinMaterials.defaultMaterial;

        if (TryGetComponent(out Building building))
        {
            m_ControlPoints = building.ControlPoints;
            m_OriginalControlPoints = m_ControlPoints.Clone() as ControlPoint[];
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

        m_ControlPoints = roof.ControlPoints.ToArray();
        m_OriginalControlPoints = m_ControlPoints.Clone() as ControlPoint[];
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
        int Flat = (int)RoofType.Flat;
        int Dormer = (int)RoofType.Dormer;
        int MShaped = (int)RoofType.MShaped;
        int Pyramid = (int)RoofType.Pyramid;
        int PyramidHip = (int)RoofType.PyramidHip;

        switch (m_ControlPoints.Length)
        {
            case 4:
                return new int[] { OpenGable, Mansard, Flat, Dormer, MShaped, Pyramid, PyramidHip };
            case 5:
                if (m_ControlPoints.IsPointInside(m_ControlPoints.Centre()))
                {
                    return new int[] { Mansard, Flat, Pyramid, PyramidHip };
                }
                return new int[] { Mansard, Flat };
            case 6:
                if (m_ControlPoints.IsLShaped(out _))
                {
                    if (m_ControlPoints.IsPointInside(m_ControlPoints.Centre()))
                    {
                        return new int[] { OpenGable, Mansard, Flat, Dormer, Pyramid, PyramidHip };
                    }

                    return new int[] { OpenGable, Mansard, Flat, Dormer };
                }
                if (m_ControlPoints.IsPointInside(m_ControlPoints.Centre()))
                {
                    return new int[] { Mansard, Flat, Pyramid, PyramidHip };
                }
                return new int[] { Mansard, Flat };
            case 7:
                if (m_ControlPoints.IsPointInside(m_ControlPoints.Centre()))
                {
                    return new int[] { Mansard, Flat, Pyramid, PyramidHip };
                }
                return new int[] { Mansard, Flat };
            case 8:
                if (m_ControlPoints.IsTShaped(out _) || m_ControlPoints.IsUShaped(out _))
                {
                    if (m_ControlPoints.IsPointInside(m_ControlPoints.Centre()))
                    {
                        return new int[] { OpenGable, Mansard, Flat, Dormer, Pyramid, PyramidHip };
                    }

                    return new int[] { OpenGable, Mansard, Flat, Dormer };
                }
                return new int[] { Mansard, Flat };
            default:
                return new int[] { OpenGable, Mansard, Flat, Dormer, Pyramid, PyramidHip };
        }
    }

    public Roof BuildFrame()
    {
        transform.DeleteChildren();

        m_ControlPoints = m_OriginalControlPoints;

        switch (m_FrameType)
        {
            case RoofType.OpenGable:
                BuildOpenGable();
                break;
            case RoofType.Mansard:
                BuildMansard();
                break;
            case RoofType.Flat:
                BuildFlat();
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
        GameObject roofFrame = new GameObject("Pyramid Roof Frame");
        roofFrame.transform.SetParent(transform, false);

        //Vector3 middle = ProMaths.Average(m_ControlPoints);
        //middle += (Vector3.up * m_PyramidHeight);

        for (int i = 0; i < m_ControlPoints.Length; i++)
        {
            //ProBuilderMesh beam = GenerateBeam(middle, m_ControlPoints[i]);
            //beam.transform.SetParent(roofFrame.transform, true);
        }

    }

    private void BuildMansard()
    {
        GameObject roofFrame = new("Mansard Roof Frame");
        roofFrame.transform.SetParent(transform, false);

        Vector3[] scaledControlPoints = PolyToolExtensions.ScalePolygon(m_ControlPoints, m_MansardScale);

        for (int i = 0; i < m_ControlPoints.Length; i++)
        {
            scaledControlPoints[i] += (Vector3.up * m_MansardHeight);

            //ProBuilderMesh beam = GenerateBeam(scaledControlPoints[i], m_ControlPoints[i]);
            //beam.transform.SetParent(roofFrame.transform, true);
        }

        //for (int i = 0; i < scaledControlPoints.Length; i++)
        //{
        //    int next = scaledControlPoints.GetNextControlPoint(i);
        //    ProBuilderMesh beam = GenerateBeam(scaledControlPoints[i], scaledControlPoints[next]);
        //    beam.transform.SetParent(roofFrame.transform, true);
        //}

        IList<IList<Vector3>> holes = new List<IList<Vector3>>();
        holes.Add(PolyToolExtensions.ScalePolygon(scaledControlPoints, m_BeamWidth*2, true));

        ProBuilderMesh mesh = ProBuilderMesh.Create();

        mesh.CreateShapeFromPolygon(scaledControlPoints, m_BeamDepth, false, holes);
        mesh.GetComponent<Renderer>().sharedMaterial = m_BeamMaterial;
        mesh.transform.SetParent(roofFrame.transform, false);
        mesh.ToMesh();
        mesh.Refresh();

        //m_ControlPoints = scaledControlPoints;
    }

    private void CreateRoofTile(Vector3[] points, bool heightStart = false, bool heightEnd = true, bool widthStart = true, bool widthEnd = true)
    {
        ProBuilderMesh roofTile = ProBuilderMesh.Create();
        roofTile.name = "Roof Tile";
        RoofTile tile = roofTile.AddComponent<RoofTile>();
        tile.SetControlPoints(points);
        tile.Initialize(m_TileHeight, m_TileExtend).Extend(heightStart, heightEnd, widthStart, widthEnd).Build();
        tile.GetComponent<Renderer>().material = TileMaterial;
        tile.transform.SetParent(transform, false);
    }

    private void BuildOpenGable()
    {
        if (m_ControlPoints.IsDescribableInOneLine(out Vector3[] oneLine))
        {
            GameObject roofFrame = new GameObject("Roof Frame");
            roofFrame.transform.SetParent(transform, false);

            //ProBuilderMesh baseFrame = BuildBaseFrame(m_ControlPoints);
           // baseFrame.transform.SetParent(roofFrame.transform, true);

            GameObject tiles = new GameObject("Tiles");
            tiles.transform.SetParent(transform, false);

            List<Tile> roofTiles = new();

            Vector3 beamDepth = Vector3.up * m_BeamDepth;
            Vector3 beamWidth = Vector3.up * m_BeamDepth;

            int supportSteps = 1;
            int centreSteps = 1;
            bool doesSupportHaveTC = false;
            bool doesCentreHaveTC = false;

            switch (oneLine.Length)
            {
                case 2:
                    if (oneLine.Length == 2) // Prevents scope error.
                    {
                        Vector3 start = oneLine[0] + (Vector3.up * m_GableHeight);
                        Vector3 end = oneLine[1] + (Vector3.up * m_GableHeight);

                        CreateRoofTile(new Vector3[] { m_ControlPoints[3].Position, end, start, m_ControlPoints[0].Position });
                        CreateRoofTile(new Vector3[] { m_ControlPoints[1].Position, start, end, m_ControlPoints[2].Position });
                    }
                    break;
                case 3:
                    if (oneLine.Length == 3)
                    {
                        Vector3 start = oneLine[0] + (Vector3.up * m_GableHeight);
                        Vector3 mid = oneLine[1] + (Vector3.up * m_GableHeight);
                        Vector3 end = oneLine[2] + (Vector3.up * m_GableHeight);

                        // Index Points
                        bool isL = m_ControlPoints.IsLShaped(out int index);
                        int next = m_ControlPoints.GetNext(index);
                        int twoNext = m_ControlPoints.GetNext(next);
                        int threeNext = m_ControlPoints.GetNext(twoNext);
                        int onePrevious = m_ControlPoints.GetPrevious(index);
                        int twoPrevious = m_ControlPoints.GetPrevious(onePrevious);
                        int threePrevious = m_ControlPoints.GetPrevious(twoPrevious);
                        

                        CreateRoofTile(new Vector3[] { m_ControlPoints[index].Position, mid, start, m_ControlPoints[next].Position }, false, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[twoNext].Position, start, mid, m_ControlPoints[threeNext].Position }, false, true, true, false);

                        CreateRoofTile(new Vector3[] { m_ControlPoints[onePrevious].Position, end, mid, m_ControlPoints[index].Position }, false, true, true, false);
                        CreateRoofTile(new Vector3[] { m_ControlPoints[threeNext].Position, mid, end, m_ControlPoints[twoPrevious].Position }, false, true, false);


                    }
                    break;
                case 4:
                    if (oneLine.Length == 4)
                    {
                        Vector3 first = oneLine[0] + (Vector3.up * m_GableHeight);
                        Vector3 second = oneLine[1] + (Vector3.up * m_GableHeight);
                        Vector3 third = oneLine[2] + (Vector3.up * m_GableHeight);
                        Vector3 fourth = oneLine[3] + (Vector3.up * m_GableHeight);


                        //steps = Mathf.FloorToInt(average); // multiply it by something? a 0 to 1 value, maybe? something specified by the player/dev.

                        if (m_ControlPoints.IsTShaped(out int[] tPointIndices))
                        {
                            // Indices
                            int tPoint0 = tPointIndices[0];
                            int tPoint1 = tPointIndices[1];
                            int tPoint0Previous = m_ControlPoints.GetPrevious(tPoint0);
                            int tPoint0Previous1 = m_ControlPoints.GetPrevious(tPoint0Previous);
                            int tPoint0Next = m_ControlPoints.GetNext(tPoint0);
                            int tPoint1Previous = m_ControlPoints.GetPrevious(tPoint1);
                            int tPoint1Next = m_ControlPoints.GetNext(tPoint1);
                            int tPoint1Next1 = m_ControlPoints.GetNext(tPoint1Next);


                            CreateRoofTile(new Vector3[] { m_ControlPoints[tPoint0].Position, fourth, first, m_ControlPoints[tPoint0Next].Position }, false, true, false, true);
                            CreateRoofTile(new Vector3[] { m_ControlPoints[tPoint1Previous].Position, first, fourth, m_ControlPoints[tPoint1].Position }, false, true, true, false);
                            CreateRoofTile(new Vector3[] { m_ControlPoints[tPoint0Previous].Position, second, fourth, m_ControlPoints[tPoint0].Position }, false, true, true, false);
                            CreateRoofTile(new Vector3[] { m_ControlPoints[tPoint1].Position, fourth, third, m_ControlPoints[tPoint1Next].Position }, false, true, false, true);

                            CreateRoofTile(new Vector3[] { m_ControlPoints[tPoint1Next1].Position, third, second, m_ControlPoints[tPoint0Previous1].Position });
                        }

                        if (m_ControlPoints.IsUShaped(out int[] uPointIndices))
                        {
                            // Indices
                            int uPoint0 = uPointIndices[0];
                            int uPoint1 = uPointIndices[1];
                            int uPoint0Previous = m_ControlPoints.GetPrevious(uPoint0);
                            int uPoint0Previous1 = m_ControlPoints.GetPrevious(uPoint0Previous);
                            int uPoint0Previous2 = m_ControlPoints.GetPrevious(uPoint0Previous1);
                            int uPoint1Next = m_ControlPoints.GetNext(uPoint1);
                            int uPoint1Next1 = m_ControlPoints.GetNext(uPoint1Next);
                            int uPoint1Next2 = m_ControlPoints.GetNext(uPoint1Next1);

                            CreateRoofTile(new Vector3[] { m_ControlPoints[uPoint0Previous].Position, first, second, m_ControlPoints[uPoint0].Position }, false, true, true, false);
                            CreateRoofTile(new Vector3[] { m_ControlPoints[uPoint0Previous2].Position, second, first, m_ControlPoints[uPoint0Previous1].Position }, false, true, false, true);
                            CreateRoofTile(new Vector3[] { m_ControlPoints[uPoint0].Position, second, third, m_ControlPoints[uPoint1].Position }, false, true, false, false);
                            CreateRoofTile(new Vector3[] { m_ControlPoints[uPoint1Next2].Position, third, second, m_ControlPoints[uPoint0Previous2].Position }, false, true, false, false);
                            CreateRoofTile(new Vector3[] { m_ControlPoints[uPoint1].Position, third, fourth, m_ControlPoints[uPoint1Next].Position }, false, true, false, true);
                            CreateRoofTile(new Vector3[] { m_ControlPoints[uPoint1Next1].Position, fourth, third, m_ControlPoints[uPoint1Next2].Position }, false, true, true, false);
                        }
                    }
                    break;
            }

            foreach (Tile roofTile in roofTiles)
            {
                roofTile.transform.SetParent(tiles.transform, false);
                roofTile.SetMaterial(m_TileMaterial);
                roofTile.SetHeight(m_TileHeight);
                roofTile.SetExtendDistance(m_TileExtend);
                //roofTile.SetUVOffset(internalOffset, externalOffset);
                roofTile.StartConstruction();
            }

            //tiles.transform.localPosition = Vector3.up * 0.6f;
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
            if(m.IsPolygonDescribableInOneLine(out Vector3[] oneLine))
            {
                Vector3 start = oneLine[0] + (Vector3.up * m_GableHeight);
                Vector3 end = oneLine[1] + (Vector3.up * m_GableHeight);

                if(m_Rotate)
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

    /// <summary>
    /// Finds an avialable roof frame & sets the height to 0.
    /// </summary>
    private void BuildFlat()
    {
        int[] available = AvailableRoofFrames();

        RoofType random = (RoofType)Rando.Range(0, available.Length);

        while (random == RoofType.Flat || random == RoofType.Mansard)
        {
            random = (RoofType)Rando.Range(0, available.Length);
        }

        m_FrameType = random;

        BuildFrame();
    }

    private void OnDrawGizmosSelected()
    {
        //if (m_TempVerts == null | m_TempVerts.Length == 0)
        //    return;

        for (int i = 0; i < m_ControlPoints.Length; i++)
        {
            Handles.Label(m_ControlPoints[i].Position + transform.localPosition, i.ToString());
        }
    }
}
