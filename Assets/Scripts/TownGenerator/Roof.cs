using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.Shapes;
using Rando = UnityEngine.Random;

[System.Serializable]
public class Roof : MonoBehaviour
{
    [SerializeField] private float m_Height, m_MansardHeight;
    [SerializeField] private RoofType m_FrameType;
    [SerializeField] private GameObject m_CentreBeamPrefab;
    [SerializeField] private GameObject m_SupportBeamPrefab;
    [SerializeField] private Material m_RoofTileMaterial;
    [SerializeField, HideInInspector] private Vector3[] m_ControlPoints;
    [SerializeField, HideInInspector] private Vector3[] m_OriginalControlPoints;

    public float MansardHeight => m_MansardHeight;
    public float Height => m_Height;
    public RoofType FrameType => m_FrameType;
    public Material RoofTileMaterial => m_RoofTileMaterial;
    public GameObject SupportBeamPrefab => m_SupportBeamPrefab;
    public GameObject CentreBeamPrefab => m_CentreBeamPrefab;

    public IEnumerable<Vector3> ControlPoints => m_ControlPoints;

    public Roof Initialize(Roof roof, IEnumerable<Vector3> controlPoints)
    {
        m_Height = roof.Height;
        m_MansardHeight = roof.MansardHeight;
        m_FrameType = roof.FrameType;
        m_RoofTileMaterial = roof.RoofTileMaterial;
        m_ControlPoints = controlPoints.ToArray();
        m_OriginalControlPoints = m_ControlPoints;
        m_CentreBeamPrefab = roof.CentreBeamPrefab;
        m_SupportBeamPrefab = roof.SupportBeamPrefab;
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

        switch (ControlPoints.Count())
        {
            case 4:
                return new int[] { OpenGable, Mansard, Flat, Dormer, MShaped, Pyramid, PyramidHip };
            case 5:
                if (m_ControlPoints.IsPointInsidePolygon(m_ControlPoints.PolygonCentre()))
                {
                    return new int[] { Mansard, Flat, Pyramid, PyramidHip };
                }
                return new int[] { Mansard, Flat };
            case 6:
                if (m_ControlPoints.IsPolygonLShaped(out _))
                {
                    if(m_ControlPoints.IsPointInsidePolygon(m_ControlPoints.PolygonCentre()))
                    {
                        return new int[] { OpenGable, Mansard, Flat, Dormer, Pyramid, PyramidHip };
                    }

                    return new int[] { OpenGable, Mansard, Flat, Dormer };
                }
                if (m_ControlPoints.IsPointInsidePolygon(m_ControlPoints.PolygonCentre()))
                {
                    return new int[] { Mansard, Flat, Pyramid, PyramidHip };
                }
                return new int[] { Mansard, Flat };
            case 7:
                if (m_ControlPoints.IsPointInsidePolygon(m_ControlPoints.PolygonCentre()))
                {
                    return new int[] { Mansard, Flat, Pyramid, PyramidHip };
                }
                return new int[] { Mansard, Flat };
            case 8:
                if(m_ControlPoints.IsPolygonTShaped(out _) || m_ControlPoints.IsPolygonUShaped(out _))
                {
                    if (m_ControlPoints.IsPointInsidePolygon(m_ControlPoints.PolygonCentre()))
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

    public Roof ConstructFrame()
    {
        Deconstruct();

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
        Vector3[] controlPointsArray = m_ControlPoints.ToArray();

        GameObject roofFrame = new GameObject("Pyramid Roof Frame");
        roofFrame.transform.SetParent(transform, false);

        Vector3 middle = UnityEngine.ProBuilder.Math.Average(controlPointsArray);
        middle += (Vector3.up * m_Height);

        int steps = Mathf.FloorToInt(Vector3.Distance(controlPointsArray[0], controlPointsArray[1]) / 5);

        for (int i = 1; i < controlPointsArray.Length; i++)
        {
            int next = controlPointsArray.GetNextControlPoint(i);

            int tempSteps = Mathf.FloorToInt(Vector3.Distance(controlPointsArray[i], controlPointsArray[next]) / 5);

            if (tempSteps > steps)
            {
                steps = tempSteps;
            }
        }

        Beam[] supportBeams = new Beam[controlPointsArray.Length];

        for(int i = 0; i < supportBeams.Length; i++)
        {
            supportBeams[i] = Beam.Create(m_SupportBeamPrefab, transform).Build(middle, controlPointsArray[i], steps);
            supportBeams[i].transform.SetParent(roofFrame.transform, true);
        }

        GameObject tiles = new GameObject("Tiles");
        tiles.transform.SetParent(transform, false);

        for (int i = 0; i < supportBeams.Length; i++)
        {
            int next = controlPointsArray.GetNextControlPoint(i);

            Extrudable[] extrudes = { supportBeams[i], supportBeams[next] };

            GameObject tile = new GameObject("Tile " + i.ToString());
            Tile roofTile = tile.AddComponent<Tile>().Initialize(extrudes).Extend(false, true, false, false);
            roofTile.transform.SetParent(tiles.transform, true);
            roofTile.SetMaterial(m_RoofTileMaterial);
            roofTile.StartConstruction();
        }
    }

    private void BuildMansard()
    {
        Vector3[] controlPointsArray = m_ControlPoints.ToArray();

        GameObject roofFrame = new ("Mansard Roof Frame");
        roofFrame.transform.SetParent(transform, false);

        GameObject tiles = new ("Mansard Roof Tiles");
        tiles.transform.SetParent(transform, false);

        List<Extrudable> supportPoints = new();
        Vector3[] startPositions = new Vector3[controlPointsArray.Length];

        for (int i = 0; i < startPositions.Length; i++)
        {
            int previousPoint = controlPointsArray.GetPreviousControlPoint(i);
            int nextPoint = controlPointsArray.GetNextControlPoint(i);

            Vector3 a = controlPointsArray[i].GetDirectionToTarget(controlPointsArray[nextPoint]);
            Vector3 b = controlPointsArray[i].GetDirectionToTarget(controlPointsArray[previousPoint]);
            Vector3 c = Vector3.Lerp(a, b, 0.5f);

            Vector3 pos = controlPointsArray[i] + c;

            if (!controlPointsArray.IsPointInsidePolygon(pos))
            {
                pos = controlPointsArray[i] - c;
            }

            startPositions[i] = pos + (Vector3.up * m_MansardHeight);
        }

        for (int i = 0; i < controlPointsArray.Length; i++)
        {
            Beam beam = Beam.Create(m_SupportBeamPrefab, transform).Build(startPositions[i], controlPointsArray[i], 1);
            beam.transform.SetParent(roofFrame.transform, true);
            supportPoints.Add(beam);
        }

        for (int i = 0; i < supportPoints.Count; i++)
        {
            int next = controlPointsArray.GetNextControlPoint(i);
            Beam beam = Beam.Create(m_SupportBeamPrefab, transform).Build(supportPoints[i].transform.localPosition, supportPoints[next].transform.localPosition, 0);
            beam.transform.SetParent(roofFrame.transform, true);
        }

        for (int i = 0; i < supportPoints.Count; i++)
        {
            int next = controlPointsArray.GetNextControlPoint(i);
            Extrudable[] extrudables = { supportPoints[i], supportPoints[next] };
            GameObject tile = new GameObject("Tile " + i.ToString());

            Tile roofTile = tile.AddComponent<Tile>().Initialize(extrudables, true);
            roofTile.transform.SetParent(tiles.transform, true);
            roofTile.SetMaterial(m_RoofTileMaterial);
            roofTile.StartConstruction();

        }

        m_ControlPoints = startPositions;
    }

    private void BuildOpenGable()
    {
        Vector3[] controlPointsArray = m_ControlPoints.ToArray();

        if (m_ControlPoints.IsPolygonDescribableInOneLine(out Vector3[] oneLine))
        {
            GameObject roofFrame = new GameObject("Roof Frame");
            roofFrame.transform.SetParent(transform, false);

            GameObject tiles = new GameObject("Tiles");
            tiles.transform.SetParent(transform, false);

            List<Tile> roofTiles = new ();

            // I should probably write code comments.

            switch (oneLine.Length)
            {
                case 2:
                    if (oneLine.Length == 2) // Prevents scope error.
                    {
                        Vector3 start = oneLine[0] + (Vector3.up * m_Height);
                        Vector3 end = oneLine[1] + (Vector3.up * m_Height);

                        Beam centreBeam = Beam.Create(m_CentreBeamPrefab, transform).Build(start, end, 10);
                        centreBeam.transform.SetParent(roofFrame.transform, true);

                        Vector3[] localCentreBeamPoints = transform.InverseTransformPoints(centreBeam.ExtrusionPositions).ToArray();

                        float distance = Vector3.Distance(start, end);
                        int steps = Mathf.FloorToInt(distance / 5);
                        int size = localCentreBeamPoints.Length;

                        Beam[] firstSupportBeamSet = Beam.Create(m_SupportBeamPrefab, transform, size).ToArray();
                        Beam[] secondSupportBeamSet = Beam.Create(m_SupportBeamPrefab, transform, size).ToArray();

                        Vector3[] endPointsA = Vector3Extensions.LerpCollection(controlPointsArray[1], controlPointsArray[2], size).ToArray();
                        Vector3[] endPointsB = Vector3Extensions.LerpCollection(controlPointsArray[0], controlPointsArray[3], size).ToArray();
                        
                        for (int i = 0; i < size; i++)
                        {
                            Connector connector = new (new Beam[]
                            { 
                                firstSupportBeamSet[i].Build(localCentreBeamPoints[i], endPointsA[i], steps),
                                secondSupportBeamSet[i].Build(localCentreBeamPoints[i], endPointsB[i], steps)
                            });

                            centreBeam.AddConnector(connector);
                        }

                        Beam[] combined = firstSupportBeamSet.Concat(secondSupportBeamSet).ToArray();

                        foreach (Beam extruder in combined)
                        {
                            extruder.transform.SetParent(roofFrame.transform, true);
                        }

                        roofTiles.Add(new GameObject("First Tile").AddComponent<Tile>().Initialize(firstSupportBeamSet, true).Extend(false, true, true, true));
                        roofTiles.Add(new GameObject("Second Tile").AddComponent<Tile>().Initialize(secondSupportBeamSet).Extend(false, true, true, true));
                    }
                    break;
                case 3:
                    if (oneLine.Length == 3)
                    {
                        Vector3 start = oneLine[0] + (Vector3.up * m_Height);
                        Vector3 mid = oneLine[1] + (Vector3.up * m_Height);
                        Vector3 end = oneLine[2] + (Vector3.up * m_Height);

                        // Index Points
                        bool isL = m_ControlPoints.IsPolygonLShaped(out int index);
                        int next = m_ControlPoints.GetNextControlPoint(index);
                        int twoNext = m_ControlPoints.GetNextControlPoint(next);
                        int threeNext = m_ControlPoints.GetNextControlPoint(twoNext);
                        int onePrevious = m_ControlPoints.GetPreviousControlPoint(index);
                        int twoPrevious = m_ControlPoints.GetPreviousControlPoint(onePrevious);
                        int threePrevious = m_ControlPoints.GetPreviousControlPoint(twoPrevious);

                        // TODO: Merge steps & size variables.

                        float distanceA = Vector3.Distance(start, mid);
                        float distanceB = Vector3.Distance(mid, end);
                        float average = (distanceA + distanceB) * 0.5f;
                        int steps = Mathf.FloorToInt(average);

                        Beam firstBeam = Beam.Create(m_CentreBeamPrefab, transform).Build(start, mid, steps);
                        firstBeam.transform.SetParent(roofFrame.transform, true);

                        int size = firstBeam.ExtrusionPositions.Length;

                        Vector3[] localFirstBeamPoints = transform.InverseTransformPoints(firstBeam.ExtrusionPositions).ToArray();

                        Beam[] first = Beam.Create(m_SupportBeamPrefab, transform, size).ToArray();
                        Beam[] second = Beam.Create(m_SupportBeamPrefab, transform, size).ToArray();

                        Vector3[] endPointsA = Vector3Extensions.LerpCollection(controlPointsArray[next], controlPointsArray[index], size).ToArray();
                        Vector3[] endPointsB = Vector3Extensions.LerpCollection(controlPointsArray[twoNext], controlPointsArray[threeNext], size).ToArray();

                        for (int i = 0; i < size; i++)
                        {
                            Connector connector = new(new Beam[]
                            {
                                first[i].Build(localFirstBeamPoints[i], endPointsA[i], steps),
                                second[i].Build(localFirstBeamPoints[i], endPointsB[i], steps)
                            });

                            firstBeam.AddConnector(connector);
                        }
                        
                        Beam secondBeam = Beam.Create(m_CentreBeamPrefab, transform).Build(end, mid, steps);
                        secondBeam.transform.SetParent(roofFrame.transform, true);

                        Vector3[] localSecondBeamPoints = transform.InverseTransformPoints(secondBeam.ExtrusionPositions).ToArray();

                        Beam[] third = Beam.Create(m_SupportBeamPrefab, transform, size).ToArray();
                        Beam[] fourth = Beam.Create(m_SupportBeamPrefab, transform, size).ToArray();

                        Vector3[] endPointsC = Vector3Extensions.LerpCollection(controlPointsArray[onePrevious], controlPointsArray[index], size).ToArray();
                        Vector3[] endPointsD = Vector3Extensions.LerpCollection(controlPointsArray[twoPrevious], controlPointsArray[threePrevious], size).ToArray();

                        for (int i = 0; i < size - 1; i++)
                        {
                            Connector connector = new(new Beam[]
                            {
                                third[i].Build(localSecondBeamPoints[i], endPointsC[i], steps),
                                fourth[i].Build(localSecondBeamPoints[i], endPointsD[i], steps)
                            });

                            secondBeam.AddConnector(connector);
                        }

                        third[^1] = first[^1];
                        fourth[^ 1] = second[^1];
                        secondBeam.AddConnector(firstBeam.GetLastConnector());

                        Extrudable[] combined = first.Concat(second).Concat(third).Concat(fourth).ToArray();

                        foreach (Extrudable extruder in combined)
                        {
                            extruder.transform.SetParent(roofFrame.transform, true);
                        }

                        roofTiles.Add(new GameObject("First Tile").AddComponent<Tile>().Initialize(first, true).Extend(false, true, true, false));
                        roofTiles.Add(new GameObject("Second Tile").AddComponent<Tile>().Initialize(second).Extend(false, true, true, false));
                        roofTiles.Add(new GameObject("Third Tile").AddComponent<Tile>().Initialize(third).Extend(false, true, true, false));
                        roofTiles.Add(new GameObject("Fourth Tile").AddComponent<Tile>().Initialize(fourth, true).Extend(false, true, true, false));

                        // Suspend construction if the centre beam is being reshaped.
                        firstBeam.OnConnectorBeginReshape += roofTiles[0].SuspendConstruction;
                        firstBeam.OnConnectorBeginReshape += roofTiles[1].SuspendConstruction;
                        secondBeam.OnConnectorBeginReshape += roofTiles[2].SuspendConstruction;
                        secondBeam.OnConnectorBeginReshape += roofTiles[3].SuspendConstruction;

                        firstBeam.OnConnectorEndReshape += roofTiles[0].StartConstruction;
                        firstBeam.OnConnectorEndReshape += roofTiles[1].StartConstruction;
                        secondBeam.OnConnectorEndReshape += roofTiles[2].StartConstruction;
                        secondBeam.OnConnectorEndReshape += roofTiles[3].StartConstruction;

                    }
                    break;
                case 4:
                    if (oneLine.Length == 4)
                    {
                        Vector3 first = oneLine[0] + (Vector3.up * m_Height);
                        Vector3 second = oneLine[1] + (Vector3.up * m_Height);
                        Vector3 third = oneLine[2] + (Vector3.up * m_Height);
                        Vector3 fourth = oneLine[3] + (Vector3.up * m_Height);

                        float distanceA = Vector3.Distance(first, fourth);
                        float distanceB = Vector3.Distance(second, fourth);
                        float distanceC = Vector3.Distance(third, fourth);
                        float average = (distanceA + distanceB + distanceC) * 0.333333f;

                        int steps = Mathf.FloorToInt(average); // multiply it by something? a 0 to 1 value, maybe? something specified by the player/dev.

                        if (m_ControlPoints.IsPolygonTShaped(out int[] tPointIndices))
                        {
                            // Indices
                            int tPoint0Previous = m_ControlPoints.GetPreviousControlPoint(tPointIndices[0]);
                            int tPoint0Previous1 = m_ControlPoints.GetPreviousControlPoint(tPoint0Previous);
                            int tPoint0Next = m_ControlPoints.GetNextControlPoint(tPointIndices[0]);
                            int tPoint1Previous = m_ControlPoints.GetPreviousControlPoint(tPointIndices[1]);
                            int tPoint1Next = m_ControlPoints.GetNextControlPoint(tPointIndices[1]);
                            int tPoint1Next1 = m_ControlPoints.GetNextControlPoint(tPoint1Next);

                            Beam firstBeam = Beam.Create(m_CentreBeamPrefab, transform, "First Beam").Build(first, fourth, steps);
                            int size = firstBeam.ExtrusionPositions.Length;
                            Vector3[] localFirstBeamPoints = transform.InverseTransformPoints(firstBeam.ExtrusionPositions).ToArray();

                            // Supports
                            Beam[] firstBeams = Beam.Create(m_SupportBeamPrefab, transform, size).ToArray();
                            Beam[] secondBeams = Beam.Create(m_SupportBeamPrefab, transform, size).ToArray();

                            Vector3[] endPointsA = Vector3Extensions.LerpCollection(controlPointsArray[tPoint0Next], controlPointsArray[tPointIndices[0]], size).ToArray();
                            Vector3[] endPointsB = Vector3Extensions.LerpCollection(controlPointsArray[tPoint1Previous], controlPointsArray[tPointIndices[1]], size).ToArray();

                            for (int i = 0; i < size; i++)
                            {
                                Connector connector = new(new Beam[]
                                {
                                    firstBeams[i].Build(localFirstBeamPoints[i], endPointsA[i], steps),
                                    secondBeams[i].Build(localFirstBeamPoints[i], endPointsB[i], steps)
                                });

                                firstBeam.AddConnector(connector);
                            }
                            // Centre beams
                            Beam secondBeam = Beam.Create(m_CentreBeamPrefab, transform, "Second Beam").Build(second, fourth, steps);
                            Beam thirdBeam = Beam.Create(m_CentreBeamPrefab, transform, "Third Beam").Build(third, fourth, steps);

                            Vector3[] localSecondBeamPoints = transform.InverseTransformPoints(secondBeam.ExtrusionPositions).ToArray();
                            Vector3[] localThirdBeamPoints = transform.InverseTransformPoints(thirdBeam.ExtrusionPositions).ToArray();

                            // Support Beams
                            Beam[] thirdBeams = Beam.Create(m_SupportBeamPrefab, transform, size-1).ToArray();
                            Beam[] fourthBeams = Beam.Create(m_SupportBeamPrefab, transform, size-1).ToArray();

                            Vector3[] endPointsC = Vector3Extensions.LerpCollection(controlPointsArray[tPoint0Previous], controlPointsArray[tPointIndices[0]], size).ToArray();
                            Vector3[] endPointsD = Vector3Extensions.LerpCollection(controlPointsArray[tPoint1Next], controlPointsArray[tPointIndices[1]], size).ToArray();

                            for (int i = 0; i < size-1; i++)
                            {
                                thirdBeams[i].Build(localSecondBeamPoints[i], endPointsC[i], steps);
                                fourthBeams[i].Build(localThirdBeamPoints[i], endPointsD[i], steps);
                            }

                            localSecondBeamPoints = localSecondBeamPoints[..^1];
                            Vector3[] longPoints = localSecondBeamPoints.Concat(localThirdBeamPoints.Reverse()).ToArray();
                            Vector3[] endPointsE = Vector3Extensions.LerpCollection(controlPointsArray[tPoint0Previous1], controlPointsArray[tPoint1Next1], longPoints.Length).ToArray();

                            Beam[] fifthBeams = Beam.Create(m_SupportBeamPrefab, transform, longPoints.Length).ToArray();

                            for(int i = 0; i < longPoints.Length; i++)
                            {
                                fifthBeams[i].Build(longPoints[i], endPointsE[i], steps);
                            }

                            for (int i = 0; i < localSecondBeamPoints.Length; i++)
                            {
                                Connector connector = new(new Beam[]
                                {
                                    thirdBeams[i], fifthBeams[i]
                                });
                                secondBeam.AddConnector(connector);
                            }

                            int count = 0;
                            for (int i = fifthBeams.Length-1; i > localSecondBeamPoints.Length; i--)
                            {
                                Connector connector = new(new Beam[]
                                {
                                    fourthBeams[count], fifthBeams[i]
                                });

                                thirdBeam.AddConnector(connector);
                                count++;
                            }

                            Array.Resize(ref thirdBeams, thirdBeams.Length + 1);
                            thirdBeams[^1] = firstBeams[^1];

                            Array.Resize(ref fourthBeams, fourthBeams.Length + 1);
                            fourthBeams[^1] = secondBeams[^1];

                            List<Beam> combined = firstBeams.Concat(secondBeams).Concat(thirdBeams).Concat(fourthBeams).Concat(fifthBeams).ToList();
                            combined.Add(firstBeam);
                            combined.Add(secondBeam);
                            combined.Add(thirdBeam);

                            foreach (Extrudable extruder in combined)
                            {
                                extruder.transform.SetParent(roofFrame.transform, true);
                            }

                            roofTiles.Add(new GameObject("First Tile").AddComponent<Tile>().Initialize(firstBeams).Extend(false, true, true, false));
                            roofTiles.Add(new GameObject("Second Tile").AddComponent<Tile>().Initialize(secondBeams, true).Extend(false, true, true, false));
                            roofTiles.Add(new GameObject("Third Tile").AddComponent<Tile>().Initialize(thirdBeams, true).Extend(false, true, true, false));
                            roofTiles.Add(new GameObject("Fourth Tile").AddComponent<Tile>().Initialize(fourthBeams).Extend(false, true, true, false));
                            roofTiles.Add(new GameObject("Fifth Tile").AddComponent<Tile>().Initialize(fifthBeams).Extend(false, true, true, true));

                            // Suspend construction if the centre beam is being reshaped.
                            firstBeam.OnConnectorBeginReshape += roofTiles[0].SuspendConstruction;
                            firstBeam.OnConnectorBeginReshape += roofTiles[1].SuspendConstruction;
                            secondBeam.OnConnectorBeginReshape += roofTiles[2].SuspendConstruction;
                            secondBeam.OnConnectorBeginReshape += roofTiles[4].SuspendConstruction;
                            thirdBeam.OnConnectorBeginReshape += roofTiles[3].SuspendConstruction;
                            thirdBeam.OnConnectorBeginReshape += roofTiles[4].SuspendConstruction;

                            firstBeam.OnConnectorEndReshape += roofTiles[0].StartConstruction;
                            firstBeam.OnConnectorEndReshape += roofTiles[1].StartConstruction;
                            secondBeam.OnConnectorEndReshape += roofTiles[2].StartConstruction;
                            secondBeam.OnConnectorEndReshape += roofTiles[4].StartConstruction;
                            thirdBeam.OnConnectorEndReshape += roofTiles[3].StartConstruction;
                            thirdBeam.OnConnectorEndReshape += roofTiles[4].StartConstruction;
                        }

                        if (m_ControlPoints.IsPolygonUShaped(out int[] uPointIndices))
                        {
                            // Indices
                            int onePointPrevious = m_ControlPoints.GetPreviousControlPoint(uPointIndices[0]);
                            int twoPointPrevious = m_ControlPoints.GetPreviousControlPoint(onePointPrevious);
                            int threePointPrevious = m_ControlPoints.GetPreviousControlPoint(twoPointPrevious);
                            int onePointNext = m_ControlPoints.GetNextControlPoint(uPointIndices[1]);
                            int twoPointNext = m_ControlPoints.GetNextControlPoint(onePointNext);
                            int threePointNext = m_ControlPoints.GetNextControlPoint(twoPointNext);

                            Beam firstBeam = Beam.Create(m_CentreBeamPrefab, transform, "First Beam").Build(first, second, steps);
                            Beam secondBeam = Beam.Create(m_CentreBeamPrefab, transform, "Second Beam").Build(second, third, steps);
                            Beam thirdBeam = Beam.Create(m_CentreBeamPrefab, transform, "Third Beam").Build(fourth, third, steps);

                            int size = firstBeam.ExtrusionPositions.Length;
                            Vector3[] localFirstBeamPoints = transform.InverseTransformPoints(firstBeam.ExtrusionPositions).ToArray();
                            Vector3[] localSecondBeamPoints = transform.InverseTransformPoints(secondBeam.ExtrusionPositions).ToArray();
                            Vector3[] localThirdBeamPoints = transform.InverseTransformPoints(thirdBeam.ExtrusionPositions).ToArray();

                            // Supports
                            Beam[] firstBeams = Beam.Create(m_SupportBeamPrefab, transform, size - 1).ToArray();
                            Beam[] secondBeams = Beam.Create(m_SupportBeamPrefab, transform, size - 1).ToArray();
                            Beam[] thirdBeams = Beam.Create(m_SupportBeamPrefab, transform, size - 1).ToArray();
                            Beam[] fourthBeams = Beam.Create(m_SupportBeamPrefab, transform, size - 1).ToArray();
                            Beam[] fifthBeams = Beam.Create(m_SupportBeamPrefab, transform, size).ToArray();
                            Beam[] sixthBeams = Beam.Create(m_SupportBeamPrefab, transform, size).ToArray();

                            Vector3[] endPointsA = Vector3Extensions.LerpCollection(controlPointsArray[twoPointPrevious], controlPointsArray[threePointPrevious], size).ToArray();
                            Vector3[] endPointsB = Vector3Extensions.LerpCollection(controlPointsArray[onePointPrevious], controlPointsArray[uPointIndices[0]], size).ToArray();
                            Vector3[] endPointsC = Vector3Extensions.LerpCollection(controlPointsArray[threePointPrevious], controlPointsArray[threePointNext], size).ToArray();
                            Vector3[] endPointsD = Vector3Extensions.LerpCollection(controlPointsArray[uPointIndices[0]], controlPointsArray[uPointIndices[1]], size).ToArray();
                            Vector3[] endPointsE = Vector3Extensions.LerpCollection(controlPointsArray[twoPointNext], controlPointsArray[threePointNext], size).ToArray();
                            Vector3[] endPointsF = Vector3Extensions.LerpCollection(controlPointsArray[onePointNext], controlPointsArray[uPointIndices[1]], size).ToArray();

                            for (int i = 0; i < size; i++)
                            {
                                if (i < size - 1)
                                {
                                    Connector connectorA = new(new Beam[]
                                    {
                                    firstBeams[i].Build(localFirstBeamPoints[i], endPointsA[i], steps),
                                    secondBeams[i].Build(localFirstBeamPoints[i], endPointsB[i], steps)
                                    });

                                    firstBeam.AddConnector(connectorA);

                                    Connector connectorB = new(new Beam[]
                                    {
                                    thirdBeams[i].Build(localSecondBeamPoints[i], endPointsC[i], steps),
                                    fourthBeams[i].Build(localSecondBeamPoints[i], endPointsD[i], steps)
                                    });

                                    secondBeam.AddConnector(connectorB);
                                }

                                Connector connectorC = new(new Beam[]
                                {
                                    fifthBeams[i].Build(localThirdBeamPoints[i], endPointsE[i], steps),
                                    sixthBeams[i].Build(localThirdBeamPoints[i], endPointsF[i], steps)
                                });

                                thirdBeam.AddConnector(connectorC);
                            }

                            // The last element of first should be the same as the last element for third.
                            Array.Resize(ref firstBeams, size);
                            firstBeams[^1] = thirdBeams[0];

                            Array.Resize(ref secondBeams, size);
                            secondBeams[^1] = fourthBeams[0];

                            Array.Resize(ref thirdBeams, size);
                            thirdBeams[^1] = fifthBeams[^1];

                            Array.Resize(ref fourthBeams, size);
                            fourthBeams[^1] = sixthBeams[^1];

                            Extrudable[] combined = firstBeams.Concat(secondBeams).Concat(thirdBeams).Concat(fourthBeams).Concat(fifthBeams).Concat(sixthBeams).ToArray();

                            foreach (Extrudable extruder in combined)
                            {
                                extruder.transform.SetParent(roofFrame.transform, true);
                            }

                            roofTiles.Add(new GameObject("First Tile").AddComponent<Tile>().Initialize(firstBeams, true).Extend(false, true, true, false));
                            roofTiles.Add(new GameObject("Second Tile").AddComponent<Tile>().Initialize(secondBeams).Extend(false, true, true, false));
                            roofTiles.Add(new GameObject("Third Tile").AddComponent<Tile>().Initialize(thirdBeams, true).Extend(false, true, false, false));
                            roofTiles.Add(new GameObject("Fourth Tile").AddComponent<Tile>().Initialize(fourthBeams).Extend(false, true, false, false));
                            roofTiles.Add(new GameObject("Fifth Tile").AddComponent<Tile>().Initialize(fifthBeams).Extend(false, true, true, false));
                            roofTiles.Add(new GameObject("Fifth Tile").AddComponent<Tile>().Initialize(sixthBeams, true).Extend(false, true, true, false));
                        }
                    }
                    break;
            }

            foreach (Tile roofTile in roofTiles)
            {
                roofTile.transform.SetParent(tiles.transform, false);
                roofTile.SetMaterial(m_RoofTileMaterial);
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
        Vector3[] controlPointsArray = m_ControlPoints.ToArray();
        if (controlPointsArray.Length == 4)
        {
            Vector3[] mPointsA = new Vector3[4];
            Vector3[] mPointsB = new Vector3[4];

            mPointsA[0] = controlPointsArray[0];
            mPointsA[1] = controlPointsArray[1];
            mPointsB[2] = controlPointsArray[2];
            mPointsB[3] = controlPointsArray[3];

            mPointsB[0] = Vector3.Lerp(mPointsA[0], mPointsB[3], 0.5f);
            mPointsB[1] = Vector3.Lerp(mPointsA[1], mPointsB[2], 0.5f);

            mPointsA[2] = mPointsB[1];
            mPointsA[3] = mPointsB[0];

            // Rotate Points 90

            Vector3[] tempA = mPointsA.Clone() as Vector3[];

            for (int i = 0; i < mPointsA.Length; i++)
            {
                int index = controlPointsArray.GetNextControlPoint(i);
                tempA[i] = mPointsA[index];

            }

            mPointsA = tempA;

            Vector3[] tempB = mPointsB.Clone() as Vector3[];

            for (int i = 0; i < mPointsB.Length; i++)
            {
                int index = controlPointsArray.GetNextControlPoint(i);
                tempB[i] = mPointsB[index];

            }

            mPointsB = tempB;

            m_ControlPoints = mPointsA;
            BuildOpenGable();
            m_ControlPoints = mPointsB;
            BuildOpenGable();

            m_ControlPoints = controlPointsArray; // Reset. // Any reason to do this?

        }
    }

    /// <summary>
    /// Finds an avialable roof frame & sets the height to 0.
    /// </summary>
    private void BuildFlat()
    {
        m_Height = 0;
        int[] available = AvailableRoofFrames();

        RoofType random = (RoofType) Rando.Range(0, available.Length);

        while(random == RoofType.Flat || random == RoofType.Mansard)
        {
            random = (RoofType)Rando.Range(0, available.Length);
        }

        m_FrameType = random;

        ConstructFrame();
    }

    private void Deconstruct()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (Application.isEditor)
            {
                DestroyImmediate(child);
            }
            else
            {
                Destroy(child);
            }

        }

        if (transform.childCount > 0)
        {
            Deconstruct();
        }
    }
}
