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

                        float distanceA = Vector3.Distance(start, mid);
                        float distanceB = Vector3.Distance(mid, end);
                        float average = (distanceA + distanceB) * 0.5f;
                        int numberOfSupportBeams = Mathf.FloorToInt(average);

                        

                        CreateRoofTile(new Vector3[] { m_ControlPoints[index].Position, mid, start, m_ControlPoints[next].Position }, false, false, false, false);

                        //ProBuilderMesh firstBeam = GenerateBeam(start, mid);
                        //firstBeam.transform.SetParent(roofFrame.transform, true);

                        //Vector3 forward = start.DirectionToTarget(end);
                        //Vector3 dir = Vector3.Cross(Vector3.up, forward);

                        //Vector3[] pointsA = new Vector3[]
                        //{
                        //    start + (0.5f * m_BeamWidth * -dir) - (beamDepth * 0.5f),
                        //    mid + (0.5f * m_BeamWidth * -dir) + (forward * m_BeamWidth) - (beamDepth * 0.5f),
                        //    m_ControlPoints[next] + (m_ControlPoints[next].DirectionToTarget(m_ControlPoints[twoNext]) * m_BeamDepth) + (Vector3.up * m_BeamDepth),
                        //    m_ControlPoints[index] + (m_ControlPoints[index].DirectionToTarget(m_ControlPoints[threeNext]) * m_BeamDepth) /*+ (m_ControlPoints[2].GetDirectionToTarget(m_ControlPoints[1]) * m_BeamWidth)*/ + (Vector3.up * m_BeamDepth)
                        //};

                    }
                    break;
                case 4:
                    if (oneLine.Length == 4)
                    {
                        Vector3 first = oneLine[0] + (Vector3.up * m_GableHeight);
                        Vector3 second = oneLine[1] + (Vector3.up * m_GableHeight);
                        Vector3 third = oneLine[2] + (Vector3.up * m_GableHeight);
                        Vector3 fourth = oneLine[3] + (Vector3.up * m_GableHeight);

                        float distanceA = Vector3.Distance(first, fourth);
                        float distanceB = Vector3.Distance(second, fourth);
                        float distanceC = Vector3.Distance(third, fourth);
                        float average = (distanceA + distanceB + distanceC) * 0.333333f;

                        int possibleSteps = Mathf.FloorToInt(average);

                        if (doesCentreHaveTC)
                            centreSteps = possibleSteps;

                        if (doesSupportHaveTC)
                            supportSteps = possibleSteps;


                        //steps = Mathf.FloorToInt(average); // multiply it by something? a 0 to 1 value, maybe? something specified by the player/dev.

                        //if (m_ControlPoints.IsTShaped(out int[] tPointIndices))
                        //{
                        //    // Indices
                        //    int tPoint0Previous = m_ControlPoints.GetPrevious(tPointIndices[0]);
                        //    int tPoint0Previous1 = m_ControlPoints.GetPrevious(tPoint0Previous);
                        //    int tPoint0Next = m_ControlPoints.GetNext(tPointIndices[0]);
                        //    int tPoint1Previous = m_ControlPoints.GetPrevious(tPointIndices[1]);
                        //    int tPoint1Next = m_ControlPoints.GetNext(tPointIndices[1]);
                        //    int tPoint1Next1 = m_ControlPoints.GetNext(tPoint1Next);

                        //    Beam firstBeam = Beam.Create(m_CentreBeamPrefab, transform, "First Beam").Build(first, fourth, centreSteps);
                        //    int size = firstBeam.ExtrusionPositions.Length;
                        //    Vector3[] localFirstBeamPoints = transform.InverseTransformPoints(firstBeam.ExtrusionPositions).ToArray();

                        //    // Supports
                        //    Beam[] firstBeams = Beam.Create(m_SupportBeamPrefab, transform, size).ToArray();
                        //    Beam[] secondBeams = Beam.Create(m_SupportBeamPrefab, transform, size).ToArray();

                        //    Vector3[] endPointsA = Vector3Extensions.LerpCollection(m_ControlPoints[tPoint0Next], m_ControlPoints[tPointIndices[0]], size).ToArray();
                        //    Vector3[] endPointsB = Vector3Extensions.LerpCollection(m_ControlPoints[tPoint1Previous], m_ControlPoints[tPointIndices[1]], size).ToArray();

                        //    for (int i = 0; i < size; i++)
                        //    {
                        //        Connector connector = new(new Beam[]
                        //        {
                        //            firstBeams[i].Build(localFirstBeamPoints[i], endPointsA[i], supportSteps),
                        //            secondBeams[i].Build(localFirstBeamPoints[i], endPointsB[i], supportSteps)
                        //        });

                        //        firstBeam.AddConnector(connector);
                        //    }
                        //    // Centre beams
                        //    Beam secondBeam = Beam.Create(m_CentreBeamPrefab, transform, "Second Beam").Build(second, fourth, centreSteps);
                        //    Beam thirdBeam = Beam.Create(m_CentreBeamPrefab, transform, "Third Beam").Build(third, fourth, centreSteps);

                        //    Vector3[] localSecondBeamPoints = transform.InverseTransformPoints(secondBeam.ExtrusionPositions).ToArray();
                        //    Vector3[] localThirdBeamPoints = transform.InverseTransformPoints(thirdBeam.ExtrusionPositions).ToArray();

                        //    // Support Beams
                        //    Beam[] thirdBeams = Beam.Create(m_SupportBeamPrefab, transform, size - 1).ToArray();
                        //    Beam[] fourthBeams = Beam.Create(m_SupportBeamPrefab, transform, size - 1).ToArray();

                        //    Vector3[] endPointsC = Vector3Extensions.LerpCollection(m_ControlPoints[tPoint0Previous], m_ControlPoints[tPointIndices[0]], size).ToArray();
                        //    Vector3[] endPointsD = Vector3Extensions.LerpCollection(m_ControlPoints[tPoint1Next], m_ControlPoints[tPointIndices[1]], size).ToArray();

                        //    for (int i = 0; i < size - 1; i++)
                        //    {
                        //        thirdBeams[i].Build(localSecondBeamPoints[i], endPointsC[i], supportSteps);
                        //        fourthBeams[i].Build(localThirdBeamPoints[i], endPointsD[i], supportSteps);
                        //    }

                        //    localSecondBeamPoints = localSecondBeamPoints[..^1];
                        //    Vector3[] longPoints = localSecondBeamPoints.Concat(localThirdBeamPoints.Reverse()).ToArray();
                        //    Vector3[] endPointsE = Vector3Extensions.LerpCollection(m_ControlPoints[tPoint0Previous1], m_ControlPoints[tPoint1Next1], longPoints.Length).ToArray();

                        //    Beam[] fifthBeams = Beam.Create(m_SupportBeamPrefab, transform, longPoints.Length).ToArray();

                        //    for (int i = 0; i < longPoints.Length; i++)
                        //    {
                        //        fifthBeams[i].Build(longPoints[i], endPointsE[i], supportSteps);
                        //    }

                        //    for (int i = 0; i < localSecondBeamPoints.Length; i++)
                        //    {
                        //        Connector connector = new(new Beam[]
                        //        {
                        //            thirdBeams[i], fifthBeams[i]
                        //        });
                        //        secondBeam.AddConnector(connector);
                        //    }

                        //    int count = 0;
                        //    for (int i = fifthBeams.Length - 1; i > localSecondBeamPoints.Length; i--)
                        //    {
                        //        Connector connector = new(new Beam[]
                        //        {
                        //            fourthBeams[count], fifthBeams[i]
                        //        });

                        //        thirdBeam.AddConnector(connector);
                        //        count++;
                        //    }

                        //    Array.Resize(ref thirdBeams, thirdBeams.Length + 1);
                        //    thirdBeams[^1] = firstBeams[^1];

                        //    Array.Resize(ref fourthBeams, fourthBeams.Length + 1);
                        //    fourthBeams[^1] = secondBeams[^1];

                        //    List<Beam> combined = firstBeams.Concat(secondBeams).Concat(thirdBeams).Concat(fourthBeams).Concat(fifthBeams).ToList();
                        //    combined.Add(firstBeam);
                        //    combined.Add(secondBeam);
                        //    combined.Add(thirdBeam);

                        //    foreach (Extrudable extruder in combined)
                        //    {
                        //        extruder.transform.SetParent(roofFrame.transform, true);
                        //    }

                        //    roofTiles.Add(new GameObject("First Tile").AddComponent<Tile>().Initialize(firstBeams).Extend(false, true, true, false));
                        //    roofTiles.Add(new GameObject("Second Tile").AddComponent<Tile>().Initialize(secondBeams, true).Extend(false, true, true, false));
                        //    roofTiles.Add(new GameObject("Third Tile").AddComponent<Tile>().Initialize(thirdBeams, true).Extend(false, true, true, false));
                        //    roofTiles.Add(new GameObject("Fourth Tile").AddComponent<Tile>().Initialize(fourthBeams).Extend(false, true, true, false));
                        //    roofTiles.Add(new GameObject("Fifth Tile").AddComponent<Tile>().Initialize(fifthBeams).Extend(false, true, true, true));

                        //    // Suspend construction if the centre beam is being reshaped.
                        //    firstBeam.OnConnectorBeginReshape += roofTiles[0].SuspendConstruction;
                        //    firstBeam.OnConnectorBeginReshape += roofTiles[1].SuspendConstruction;
                        //    secondBeam.OnConnectorBeginReshape += roofTiles[2].SuspendConstruction;
                        //    secondBeam.OnConnectorBeginReshape += roofTiles[4].SuspendConstruction;
                        //    thirdBeam.OnConnectorBeginReshape += roofTiles[3].SuspendConstruction;
                        //    thirdBeam.OnConnectorBeginReshape += roofTiles[4].SuspendConstruction;

                        //    firstBeam.OnConnectorEndReshape += roofTiles[0].StartConstruction;
                        //    firstBeam.OnConnectorEndReshape += roofTiles[1].StartConstruction;
                        //    secondBeam.OnConnectorEndReshape += roofTiles[2].StartConstruction;
                        //    secondBeam.OnConnectorEndReshape += roofTiles[4].StartConstruction;
                        //    thirdBeam.OnConnectorEndReshape += roofTiles[3].StartConstruction;
                        //    thirdBeam.OnConnectorEndReshape += roofTiles[4].StartConstruction;
                        //}

                    //    if (m_ControlPoints.IsUShaped(out int[] uPointIndices))
                    //    {
                    //        // Indices
                    //        int onePointPrevious = m_ControlPoints.GetPrevious(uPointIndices[0]);
                    //        int twoPointPrevious = m_ControlPoints.GetPrevious(onePointPrevious);
                    //        int threePointPrevious = m_ControlPoints.GetPrevious(twoPointPrevious);
                    //        int onePointNext = m_ControlPoints.GetNext(uPointIndices[1]);
                    //        int twoPointNext = m_ControlPoints.GetNext(onePointNext);
                    //        int threePointNext = m_ControlPoints.GetNext(twoPointNext);

                    //        Beam firstBeam = Beam.Create(m_CentreBeamPrefab, transform, "First Beam").Build(first, second, centreSteps);
                    //        Beam secondBeam = Beam.Create(m_CentreBeamPrefab, transform, "Second Beam").Build(second, third, centreSteps);
                    //        Beam thirdBeam = Beam.Create(m_CentreBeamPrefab, transform, "Third Beam").Build(fourth, third, centreSteps);

                    //        int size = firstBeam.ExtrusionPositions.Length;
                    //        Vector3[] localFirstBeamPoints = transform.InverseTransformPoints(firstBeam.ExtrusionPositions).ToArray();
                    //        Vector3[] localSecondBeamPoints = transform.InverseTransformPoints(secondBeam.ExtrusionPositions).ToArray();
                    //        Vector3[] localThirdBeamPoints = transform.InverseTransformPoints(thirdBeam.ExtrusionPositions).ToArray();

                    //        // Supports
                    //        Beam[] firstBeams = Beam.Create(m_SupportBeamPrefab, transform, size - 1).ToArray();
                    //        Beam[] secondBeams = Beam.Create(m_SupportBeamPrefab, transform, size - 1).ToArray();
                    //        Beam[] thirdBeams = Beam.Create(m_SupportBeamPrefab, transform, size - 1).ToArray();
                    //        Beam[] fourthBeams = Beam.Create(m_SupportBeamPrefab, transform, size - 1).ToArray();
                    //        Beam[] fifthBeams = Beam.Create(m_SupportBeamPrefab, transform, size).ToArray();
                    //        Beam[] sixthBeams = Beam.Create(m_SupportBeamPrefab, transform, size).ToArray();

                    //        Vector3[] endPointsA = Vector3Extensions.LerpCollection(m_ControlPoints[twoPointPrevious], m_ControlPoints[threePointPrevious], size).ToArray();
                    //        Vector3[] endPointsB = Vector3Extensions.LerpCollection(m_ControlPoints[onePointPrevious], m_ControlPoints[uPointIndices[0]], size).ToArray();
                    //        Vector3[] endPointsC = Vector3Extensions.LerpCollection(m_ControlPoints[threePointPrevious], m_ControlPoints[threePointNext], size).ToArray();
                    //        Vector3[] endPointsD = Vector3Extensions.LerpCollection(m_ControlPoints[uPointIndices[0]], m_ControlPoints[uPointIndices[1]], size).ToArray();
                    //        Vector3[] endPointsE = Vector3Extensions.LerpCollection(m_ControlPoints[twoPointNext], m_ControlPoints[threePointNext], size).ToArray();
                    //        Vector3[] endPointsF = Vector3Extensions.LerpCollection(m_ControlPoints[onePointNext], m_ControlPoints[uPointIndices[1]], size).ToArray();

                    //        for (int i = 0; i < size; i++)
                    //        {
                    //            if (i < size - 1)
                    //            {
                    //                Connector connectorA = new(new Beam[]
                    //                {
                    //                firstBeams[i].Build(localFirstBeamPoints[i], endPointsA[i], supportSteps),
                    //                secondBeams[i].Build(localFirstBeamPoints[i], endPointsB[i], supportSteps)
                    //                });

                    //                firstBeam.AddConnector(connectorA);

                    //                Connector connectorB = new(new Beam[]
                    //                {
                    //                thirdBeams[i].Build(localSecondBeamPoints[i], endPointsC[i], supportSteps),
                    //                fourthBeams[i].Build(localSecondBeamPoints[i], endPointsD[i], supportSteps)
                    //                });

                    //                secondBeam.AddConnector(connectorB);
                    //            }

                    //            Connector connectorC = new(new Beam[]
                    //            {
                    //                fifthBeams[i].Build(localThirdBeamPoints[i], endPointsE[i], supportSteps),
                    //                sixthBeams[i].Build(localThirdBeamPoints[i], endPointsF[i], supportSteps)
                    //            });

                    //            thirdBeam.AddConnector(connectorC);
                    //        }

                    //        // The last element of first should be the same as the last element for third.
                    //        Array.Resize(ref firstBeams, size);
                    //        firstBeams[^1] = thirdBeams[0];

                    //        Array.Resize(ref secondBeams, size);
                    //        secondBeams[^1] = fourthBeams[0];

                    //        Array.Resize(ref thirdBeams, size);
                    //        thirdBeams[^1] = fifthBeams[^1];

                    //        Array.Resize(ref fourthBeams, size);
                    //        fourthBeams[^1] = sixthBeams[^1];

                    //        Extrudable[] combined = firstBeams.Concat(secondBeams).Concat(thirdBeams).Concat(fourthBeams).Concat(fifthBeams).Concat(sixthBeams).ToArray();

                    //        foreach (Extrudable extruder in combined)
                    //        {
                    //            extruder.transform.SetParent(roofFrame.transform, true);
                    //        }

                    //        roofTiles.Add(new GameObject("First Tile").AddComponent<Tile>().Initialize(firstBeams, true).Extend(false, true, true, false));
                    //        roofTiles.Add(new GameObject("Second Tile").AddComponent<Tile>().Initialize(secondBeams).Extend(false, true, true, false));
                    //        roofTiles.Add(new GameObject("Third Tile").AddComponent<Tile>().Initialize(thirdBeams, true).Extend(false, true, false, false));
                    //        roofTiles.Add(new GameObject("Fourth Tile").AddComponent<Tile>().Initialize(fourthBeams).Extend(false, true, false, false));
                    //        roofTiles.Add(new GameObject("Fifth Tile").AddComponent<Tile>().Initialize(fifthBeams).Extend(false, true, true, false));
                    //        roofTiles.Add(new GameObject("Sixth Tile").AddComponent<Tile>().Initialize(sixthBeams, true).Extend(false, true, true, false));

                    //        firstBeam.OnConnectorBeginReshape += roofTiles[0].SuspendConstruction;
                    //        firstBeam.OnConnectorBeginReshape += roofTiles[1].SuspendConstruction;
                    //        secondBeam.OnConnectorBeginReshape += roofTiles[2].SuspendConstruction;
                    //        secondBeam.OnConnectorBeginReshape += roofTiles[3].SuspendConstruction;
                    //        thirdBeam.OnConnectorBeginReshape += roofTiles[4].SuspendConstruction;
                    //        thirdBeam.OnConnectorBeginReshape += roofTiles[5].SuspendConstruction;

                    //        firstBeam.OnConnectorEndReshape += roofTiles[0].StartConstruction;
                    //        firstBeam.OnConnectorEndReshape += roofTiles[1].StartConstruction;
                    //        secondBeam.OnConnectorEndReshape += roofTiles[2].StartConstruction;
                    //        secondBeam.OnConnectorEndReshape += roofTiles[3].StartConstruction;
                    //        thirdBeam.OnConnectorEndReshape += roofTiles[4].StartConstruction;
                    //        thirdBeam.OnConnectorEndReshape += roofTiles[5].StartConstruction;
                    //    }
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
