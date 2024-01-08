using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using ProMaths = UnityEngine.ProBuilder.Math;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using System;
using UnityEngine.UIElements;

public static class PolygonRecognition
{
    public static IEnumerable<ControlPoint> FindPathPoints(this IEnumerable<ControlPoint> controlPoints)
    {
        ControlPoint[] points = controlPoints.ToArray();
        List<ControlPoint> pathPoints = new();

        int[] indices = controlPoints.FindPathPointsIndices().ToArray();

        for (int i = 0; i < indices.Length; i++)
        {
            pathPoints.Add(points[indices[i]]);
        }

        return pathPoints;
    }

    public static IEnumerable<Vector3> FindPathPoints(this IEnumerable<Vector3> controlPoints)
    {
        Vector3[] points = controlPoints.ToArray();
        List<Vector3> pathPoints = new();

        int[] indices = controlPoints.FindPathPointsIndices().ToArray();

        for (int i = 0; i < indices.Length; i++)
        {
            pathPoints.Add(points[indices[i]]);
        }

        return pathPoints;
    }

    public static IEnumerable<int> FindPathPointsIndices(this IEnumerable<ControlPoint> controlPoints)
    {
        return FindPathPointsIndices(controlPoints.GetPositions());
    }

    /// This function finds the key control points required for a shape detection & removes the unnecessary ones.
    public static IEnumerable<int> FindPathPointsIndices(this IEnumerable<Vector3> controlPoints)
    {
        Vector3[] points = controlPoints.ToArray();

        int breakPoint = points.Length;

        List<int> pathPoints = new ();
        pathPoints.Add(0); // Issue: the first point may not be a key path point.

        int breakCount = 0;
        int startPoint = 0;
        float angle = 5;

        do
        {
            int nextAfterStart = points.GetNextControlPoint(startPoint);
            //int twoNext = points.GetNextControlPoint(next);
            Vector3 keyDirection = points[startPoint].DirectionToTarget(points[nextAfterStart]);

            for (int i = nextAfterStart; i < points.Length; i++)
            {
                int nextPoint = points.GetNextControlPoint(i);
                //int previous = points.GetPreviousControlPoint(i);
                Vector3 dir = points[startPoint].DirectionToTarget(points[nextPoint]);

                float dot = Vector3.Dot(dir, keyDirection);
                //float cosineTheta = dot / (dir.magnitude * keyDirection.magnitude);
                float rads = Mathf.Acos(dot);
                float degrees = Mathf.Rad2Deg * rads;

                // idea: try using this instead. 
                //float angle = Vector3.Angle(dir, keyDirection);

                if (degrees > angle)
                {
                    pathPoints.Add(i);
                    startPoint = i;
                    break;
                }
            }

            breakCount++;

            if(breakCount > breakPoint)
            {
                //Debug.Log("Hit Break Count");
                break;
            }
                

        } while (startPoint != 0);

        if (pathPoints[0] == pathPoints[^1])
            pathPoints.RemoveAt(pathPoints.Count - 1);

        return pathPoints;
    }

    /// <summary>
    /// 2D XZ Polygon Sort Function. Only for convex shapes.
    /// </summary>
    /// <param name="controlPoints"></param>
    public static IEnumerable<Vector3> SortPointsClockwise(this IEnumerable<Vector3> controlPoints, Vector3? sortPoint = null)
    {
        if (controlPoints.Count() <= 1)
            return controlPoints;

        Vector3[] points = controlPoints.ToArray();

        Vector3 sp;

        if (sortPoint == null)
        {
            sp = ProMaths.Average(points);
        }
        else
        {
            sp = sortPoint.Value;
        }

        // Sort points based on angle relative to sp
        Array.Sort(points, (a, b) =>
        {
            Vector3 dirA = a.DirectionToTarget(sp);
            Vector3 dirB = b.DirectionToTarget(sp);
            float angleA = Mathf.Atan2(dirA.z, dirA.x);
            float angleB = Mathf.Atan2(dirB.z, dirB.x);
            return angleA.CompareTo(angleB);
        });

        return points;
    }

    public static Vector3 Centre(this IEnumerable<ControlPoint> controlPoints)
    {
        return PolygonCentre(GetPositions(controlPoints));
    }

    public static Vector3 PolygonCentre(this IEnumerable<Vector3> controlPoints)
    {
        return ProMaths.Average(controlPoints.ToArray());
    }
    public static bool IsPointInside(this IEnumerable<ControlPoint> controlPoints, Vector3 point)
    {
        return IsPointInsidePolygon(GetPositions(controlPoints), point);
    }

    // https://codereview.stackexchange.com/questions/108857/point-inside-polygon-check
    /// <summary>
    /// Uses World Coords. 2D calculation.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public static bool IsPointInside(this PolyPath path, Vector3 point)
    {
        return IsPointInsidePolygon(path.Positions, point);
    }
    /// <summary>
    /// Assumes Polygon is orientated on the XZ plane.
    /// </summary>
    /// <param name="controlPoints"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public static bool IsPointInsidePolygon(this IEnumerable<Vector3> controlPoints, Vector3 point)
    {
        Vector3[] controlPointsArray = controlPoints.ToArray();
        int j = controlPointsArray.Length - 1;
        bool c = false;
        for (int i = 0; i < controlPointsArray.Length; j = i++)
        {
            c ^= controlPointsArray[i].z >= point.z ^ controlPointsArray[j].z >= point.z && point.x <= (controlPointsArray[j].x - controlPointsArray[i].x) * (point.z - controlPointsArray[i].z) / (controlPointsArray[j].z - controlPointsArray[i].z) + controlPointsArray[i].x;
        }
        return c;
    }

    public static bool IsInterlockingYShaped(this IEnumerable<ControlPoint> controlPoints, out int[] interYIndices)
    {
        return IsPolygonInterlockingYShaped(controlPoints.GetPositions(), out interYIndices);
    }

    public static bool IsPolygonInterlockingYShaped(this IEnumerable<Vector3> controlPoints, out int[] interYIndices)
    {
        int[] concaveIndices = controlPoints.GetConcaveIndexPoints();

        interYIndices = new int[] { -1,-1, -1, -1 } ;

        if(controlPoints.Count() != 12 | concaveIndices.Length != interYIndices.Length)
            return false;

        int i;
        for(i = 0; i < concaveIndices.Length; i++)
        {
            int fourNext = controlPoints.GetControlPointIndex(concaveIndices[i] + 4);
            int fiveNext = controlPoints.GetControlPointIndex(concaveIndices[i] + 5);
            int nineNext = controlPoints.GetControlPointIndex(concaveIndices[i] + 9);
            int threePrevious = controlPoints.GetControlPointIndex(concaveIndices[i] -3);

            bool conditionA = concaveIndices.Any(x => x == fourNext);
            bool conditionB = concaveIndices.Any(x => x == fiveNext);
            bool conditionC = concaveIndices.Any(x => x == nineNext);
            bool conditionD = concaveIndices.Any(x => x == threePrevious);

            if(conditionA && conditionB && conditionC && conditionD)
            {
                interYIndices[0] = concaveIndices[i];
                interYIndices[1] = fourNext;
                interYIndices[2] = fiveNext;
                interYIndices[3] = nineNext;
                break;
            }
        }

        if (interYIndices[0] == -1)
            return false;

        return true;
    }

    public static bool IsArrowShaped(this IEnumerable<ControlPoint> controlPoints, out int[] arrowPointIndices)
    {
        return IsPolygonArrowShaped(controlPoints.GetPositions(), out arrowPointIndices);
    }

    public static bool IsPolygonArrowShaped(this IEnumerable<Vector3> controlPoints, out int[] arrowPointIndices)
    {
        int[] concaveIndices = controlPoints.GetConcaveIndexPoints();
        arrowPointIndices = new int[] { -1, -1 };

        if (controlPoints.Count() != 9 | concaveIndices.Length != arrowPointIndices.Length)
            return false;

        int i;
        for(i = 0; i < concaveIndices.Length; i++)
        {
            int sixNext = controlPoints.GetControlPointIndex(concaveIndices[i] + 6);
            int threePrevious = controlPoints.GetControlPointIndex(concaveIndices[i] - 3);

            bool conditionA = concaveIndices.Any(x => x == sixNext);
            bool conditionB = concaveIndices.Any(x => x == threePrevious);

            if(conditionA && conditionB)
            {
                arrowPointIndices[0] = concaveIndices[i];
                break;
            }
        }

        if (arrowPointIndices[0] == -1)
            return false;

        if(i == 0)
        {
            arrowPointIndices[1] = concaveIndices[1];
        }
        else
        {
            arrowPointIndices[1] = concaveIndices[0];
        }

        return true;
    }

    public static bool IsLShaped(this IEnumerable<ControlPoint> controlPoints, out int lPointIndex)
    {
        return IsPolygonLShaped(GetPositions(controlPoints), out lPointIndex);
    }

    /// <summary>
    /// Does the Polyshape resemble an 'L'? 
    /// </summary>
    /// <param name="polyTool"></param>
    /// <param name="lPointIndex"></param>
    /// <returns></returns>
    public static bool IsLShaped(this PolyPath polyTool, out int lPointIndex)
    {
        return IsPolygonLShaped(polyTool.Positions, out lPointIndex);
    }
    /// <summary>
    /// Does the Polygon resemble an 'L'?
    /// </summary>
    /// <param name="controlPoints"></param>
    /// <param name="lPointIndex"></param>
    /// <returns></returns>
    public static bool IsPolygonLShaped(this IEnumerable<Vector3> controlPoints, out int lPointIndex)
    {
        lPointIndex = 0;

        if (controlPoints.Count() != 6)
            return false;

        int[] indices = GetConcaveIndexPoints(controlPoints);

        if (indices.Length != 1)
            return false;

        lPointIndex = indices[0];

        return true;
    }

    public static bool IsAntennaShaped(this IEnumerable<ControlPoint> controlPoints, out int[] antPointsIndex)
    {
        return IsPolygonAntennaShaped(controlPoints.GetPositions(), out antPointsIndex);
    }

    public static bool IsPolygonAntennaShaped(this IEnumerable<Vector3> controlPoints, out int[] antPointsIndex)
    {
        int[] concaveIndices = controlPoints.GetConcaveIndexPoints();
        antPointsIndex = new int[concaveIndices.Length];

        if(antPointsIndex.Length == 0)
            return false;

        antPointsIndex[0] = -1;

        if (controlPoints.Count() < 20 || controlPoints.Count() % 2 != 0 ||
            concaveIndices.Length < 8 || concaveIndices.Length % 2 != 0 ||
            concaveIndices.Length % 4 != 0)
            return false;

        if(controlPoints.Count() > 20)
        {
            int count = controlPoints.Count() - 20;

            if (count % 8 != 0)
                return false;
        }

        int i;
        for (i = 0; i < antPointsIndex.Length; i++)
        {
            int previous = controlPoints.GetControlPointIndex(concaveIndices[i] - 1);
            int next = controlPoints.GetControlPointIndex(concaveIndices[i] + 1);
            int fiveNext = controlPoints.GetControlPointIndex(concaveIndices[i] + 5);
            int fourPrevious = controlPoints.GetControlPointIndex(concaveIndices[i] - 4);
            int fivePrevious = controlPoints.GetControlPointIndex(concaveIndices[i] - 5);

            bool conditionA = concaveIndices.Any(x => x == previous);
            bool conditionB = concaveIndices.Any(x => x == next);
            bool conditionC = concaveIndices.Any(x => x == fiveNext);
            bool conditionD = concaveIndices.Any(x => x == fourPrevious);
            bool conditionE = concaveIndices.Any(x => x == fivePrevious);

            if (conditionA && !conditionB && conditionC && conditionD && conditionE)
            {
                antPointsIndex[0] = concaveIndices[i];
                break;
            }
        }

        if (antPointsIndex[0] == -1)
            return false;

        for(int j = 0; j < antPointsIndex.Length; j++)
        {
            antPointsIndex[j] = concaveIndices[i];
            i++;

            if (i >= antPointsIndex.Length)
                i = 0;
        }


        return true;
    }

    /// <summary>
    /// Does the Polyshape resemble a 'T'? 
    /// </summary>
    /// <param name="polyTool"></param>
    /// <param name="tPointsIndex"></param>
    /// <returns></returns>
    public static bool IsTShaped(this PolyPath polyTool, out int[] tPointsIndex)
    {
        return IsPolygonTShaped(polyTool.Positions, out tPointsIndex);
    }

    public static bool IsTShaped(this IEnumerable<ControlPoint> controlPoints, out int[] tPointsIndex)
    {
        return IsPolygonTShaped(GetPositions(controlPoints), out tPointsIndex);
    }

    public static bool IsPolygonTShaped(this IEnumerable<Vector3> controlPoints, out int[] tPointsIndex)
    {
        Vector3[] points = controlPoints.ToArray();
        tPointsIndex = new int[2];

        if (points.Length != 8)
            return false;

        int[] indices = GetConcaveIndexPoints(points);

        if (indices.Length != 2)
            return false;

        int[] pointsNext = new int[5];
        pointsNext[0] = controlPoints.GetNextControlPoint(indices[0]);
        pointsNext[1] = controlPoints.GetNextControlPoint(pointsNext[0]);
        pointsNext[2] = controlPoints.GetNextControlPoint(pointsNext[1]);
        pointsNext[3] = controlPoints.GetNextControlPoint(pointsNext[2]);
        pointsNext[4] = controlPoints.GetNextControlPoint(pointsNext[3]);

        int[] pointsPrevious = new int[5];
        pointsPrevious[0] = controlPoints.GetPreviousControlPoint(indices[0]);
        pointsPrevious[1] = controlPoints.GetPreviousControlPoint(pointsPrevious[0]);
        pointsPrevious[2] = controlPoints.GetPreviousControlPoint(pointsPrevious[1]);
        pointsPrevious[3] = controlPoints.GetPreviousControlPoint(pointsPrevious[2]);
        pointsPrevious[4] = controlPoints.GetPreviousControlPoint(pointsPrevious[3]);


        if (pointsNext[4] == indices[1] &&
            pointsPrevious[2] == indices[1])
        {
            tPointsIndex = indices.Clone() as int[];
            tPointsIndex = tPointsIndex.Reverse().ToArray();
            return true;
        }
        else if(pointsPrevious[4] == indices[1] &&
                pointsNext[2] == indices[1])
        {
            tPointsIndex = indices.Clone() as int[];
            return true;
        }


        return false;
    }

    /// <summary>
    /// Does the Polyshape resemble a 'U'? 
    /// </summary>
    /// <param name="polyTool"></param>
    /// <param name="uPointsIndex"></param>
    /// <returns></returns>
    public static bool IsUShaped(this PolyPath polyTool, out int[] uPointsIndex)
    {
        return IsPolygonUShaped(polyTool.Positions, out uPointsIndex);
    }

    public static bool IsUShaped(this IEnumerable<ControlPoint> controlPoints, out int[] uPointsIndex)
    {
        return IsPolygonUShaped(GetPositions(controlPoints), out uPointsIndex);
    }

    public static bool IsPolygonUShaped(this IEnumerable<Vector3> controlPoints, out int[] uPointsIndex)
    {
        uPointsIndex = new int[2];

        int[] concavePoints = GetConcaveIndexPoints(controlPoints);

        if (concavePoints.Length != 2)
            return false;

        // We know if it's the u shape if the indices points are next to each other.
        int next = controlPoints.GetNextControlPoint(concavePoints[0]);
        int previous = controlPoints.GetPreviousControlPoint(concavePoints[0]);

        if (next == concavePoints[1] ||
            previous == concavePoints[1])
        {
            uPointsIndex = concavePoints.Clone() as int[];

            if (previous == concavePoints[1])
                uPointsIndex = uPointsIndex.Reverse().ToArray();
            return true;
        }

        return false;
    }

    public static bool IsEShaped(this IEnumerable<ControlPoint> controlPoints, out int[] ePointsIndex)
    {
        return IsPolygonEShaped(controlPoints.GetPositions(), out ePointsIndex);
    }

    public static bool IsEShaped(this PolyPath polyTool, out int[] ePointsIndex)
    {
        return IsPolygonEShaped(polyTool.Positions, out ePointsIndex);
    }

    public static bool IsPolygonEShaped(this IEnumerable<Vector3> controlPoints, out int[] ePointsIndices)
    {
        int[] indices = controlPoints.GetConcaveIndexPoints();
        ePointsIndices = new int[4];

        if (controlPoints.Count() != 12 | indices.Length != ePointsIndices.Length)
            return false;

        // Organise points.
        for(int i = 0; i < indices.Length; i++)
        {
            int threeNext = controlPoints.GetControlPointIndex(indices[i] + 3);

            try
            {
                indices.Single(s => s == threeNext);
            }
            catch (InvalidOperationException)
            {
                continue;
            }
            ePointsIndices[1] = indices[i];
            ePointsIndices[2] = threeNext;
        }

        ePointsIndices[0] = controlPoints.GetPreviousControlPoint(ePointsIndices[1]);
        ePointsIndices[3] = controlPoints.GetNextControlPoint(ePointsIndices[2]);

        int count = 0;

        for(int i = 0; i < ePointsIndices.Length; i++)
        {
            if (ePointsIndices[i] == 0)
                count++;
        }

        if (count > 1)
            return false;

        if(!indices.Contains(ePointsIndices[0]) ||
           !indices.Contains(ePointsIndices[1]) ||
           !indices.Contains(ePointsIndices[2]) ||
           !indices.Contains(ePointsIndices[3]))
        {
            return false;
        }

        return true;
    }

    public static bool IsXShaped(this PolyPath polyTool, out int[] xPointIndices)
    {
        return IsPolygonXShaped(polyTool.Positions, out xPointIndices);
    }

    public static bool IsXShaped(this IEnumerable<ControlPoint> controlPoints, out int[] xPointIndices)
    {
        return IsPolygonXShaped(controlPoints.GetPositions(), out xPointIndices);
    }

    public static bool IsPolygonXShaped(this IEnumerable<Vector3> controlPoints, out int[] xPointIndices)
    {
        int[] indices = GetConcaveIndexPoints(controlPoints);
        xPointIndices = new int[4];

        Vector3[] points = controlPoints.ToArray();
        
        if (points.Length != 12 | indices.Length != xPointIndices.Length)
            return false;

        if(Mathf.Abs(indices[0] - indices[1]) == 3 &&
           Mathf.Abs(indices[1] - indices[2]) == 3 &&
           Mathf.Abs(indices[2] - indices[3]) == 3 &&
           Mathf.Abs(indices[3] - indices[0]) == 9)
        {
            xPointIndices[0] = indices[0];
            xPointIndices[1] = indices[1];
            xPointIndices[2] = indices[2];
            xPointIndices[3] = indices[3];
            return true;
        }

        return false;
        
    }

    public static bool IsNShaped(this IEnumerable<ControlPoint> controlPoints, out int[] nPointIndices)
    {
        return IsPolygonNShaped(controlPoints.GetPositions(), out nPointIndices);
    }

    public static bool IsNShaped(this PolyPath polyTool, out int[] nPointIndices)
    {
        return IsPolygonNShaped(polyTool.Positions, out nPointIndices);
    }

    public static bool IsPolygonNShaped(this IEnumerable<Vector3> controlPoints, out int[] nPointIndices)
    {
        int[] indices = GetConcaveIndexPoints(controlPoints);
        nPointIndices = new int[2];

        Vector3[] controlPointsArray = controlPoints.ToArray();

        if (controlPointsArray.Length != 10 | indices.Length != nPointIndices.Length)
            return false;

        nPointIndices = indices;
        return true;
    }

    public static bool IsSimpleNShaped(this IEnumerable<ControlPoint> controlPoints, out int[] simpleNPointIndices)
    {
        return IsPolygonSimpleNShaped(controlPoints.GetPositions(), out simpleNPointIndices);
    }

    public static bool IsPolygonSimpleNShaped(this IEnumerable<Vector3> controlPoints, out int[] simpleNPointIndices)
    {
        int[] indices = GetConcaveIndexPoints(controlPoints);
        simpleNPointIndices = new int[2];

        if (controlPoints.Count() != 8 | indices.Length != simpleNPointIndices.Length)
            return false;

        simpleNPointIndices = indices;

        int indices0Next4 = controlPoints.GetControlPointIndex(indices[0] + 4);
        int indices1Next4 = controlPoints.GetControlPointIndex(indices[1] + 4);

        if (indices[0] == indices1Next4 &&
            indices[1] == indices0Next4)
        {
            Debug.Log("NShape");
            return true;
        }

        return false;
    }
    public static bool IsMShaped(this IEnumerable<ControlPoint> controlPoints, out int[] mPointIndices)
    {
        return IsPolygonMShaped(controlPoints.GetPositions(), out mPointIndices);
    }

    public static bool IsMShaped(this PolyPath polyTool, out int[] mPointIndices)
    {
        return IsPolygonMShaped(polyTool.Positions, out mPointIndices);
    }

    public static bool IsPolygonMShaped(this IEnumerable<Vector3> controlPoints, out int[] mPointIndices)
    {
        int[] indices = GetConcaveIndexPoints(controlPoints);
        mPointIndices = new int[3];

        if (controlPoints.Count() != 12 | indices.Length != mPointIndices.Length)
            return false;

        // Organise the m points.
        for (int i = 0; i< indices.Length; i++)
        {
            int twoNext = controlPoints.GetControlPointIndex(indices[i] + 2);

            try
            {
                indices.Single(s => s == twoNext);
            }
            catch (InvalidOperationException)
            {
                continue;
            }

            mPointIndices[0] = indices[i];
            mPointIndices[2] = twoNext;
            break;
        }

        for (int i = 0; i < indices.Length; i++)
        {
            int fiveNext = controlPoints.GetControlPointIndex(indices[i] + 5);
            int fivePrevious = controlPoints.GetControlPointIndex(indices[i] - 5);

            if (fiveNext == mPointIndices[0] &&
                fivePrevious == mPointIndices[2])
            {
                mPointIndices[1] = indices[i];
            }
        }

        int count = 0;

        for (int i = 0; i < mPointIndices.Length; i++)
        {
            if (mPointIndices[i] == 0)
                count++;
        }

        if (count > 1)
            return false;

        return true;
    }

    public static bool IsSimpleMShaped(this IEnumerable<ControlPoint> controlPoints, out int[] simpleMPointIndices)
    {
        return IsPolygonSimpleMShaped(controlPoints.GetPositions(), out simpleMPointIndices);
    }

    public static bool IsPolygonSimpleMShaped(this IEnumerable<Vector3> controlPoints, out int[] simpleMPointIndices)
    {
        simpleMPointIndices = new int[3];
        int[] concavePoints = controlPoints.GetConcaveIndexPoints();

        if (controlPoints.Count() != 10 | concavePoints.Length != simpleMPointIndices.Length)
            return false;

        for (int i = 0; i < concavePoints.Length; i++)
        {
            int twoNext = controlPoints.GetControlPointIndex(concavePoints[i] + 2);

            try
            {
                concavePoints.Single(s => s == twoNext);
            }
            catch (InvalidOperationException)
            {
                continue;
            }

            simpleMPointIndices[0] = concavePoints[i];
            simpleMPointIndices[2] = twoNext;
            break;
        }

        for (int i = 0; i < concavePoints.Length; i++)
        {
            int fiveNext = controlPoints.GetControlPointIndex(concavePoints[i] + 4);
            int fivePrevious = controlPoints.GetControlPointIndex(concavePoints[i] - 4);

            if (fiveNext == simpleMPointIndices[0] &&
                fivePrevious == simpleMPointIndices[2])
            {
                simpleMPointIndices[1] = concavePoints[i];
            }
        }

        int count = 0;

        for (int i = 0; i < simpleMPointIndices.Length; i++)
        {
            if (simpleMPointIndices[i] == 0)
                count++;
        }

        if (count > 1)
            return false;

        return true;
    }

    public static bool IsFShaped(this IEnumerable<ControlPoint> controlPoints, out int[] fIndices)
    {
        return IsPolygonFShaped(controlPoints.GetPositions(), out fIndices);
    }

    public static bool IsPolygonFShaped(this IEnumerable<Vector3> controlPoints, out int[] fIndices)
    {
        fIndices = new int[] {-1,-1,-1};
        int[] indices = controlPoints.GetConcaveIndexPoints();

        if (controlPoints.Count() != 10 | indices.Length != fIndices.Length)
            return false;

        for(int i = 0; i < indices.Length; i++)
        {
            int sixPlus = controlPoints.GetControlPointIndex(indices[i] +6); // prev -7
            int threePrevious = controlPoints.GetControlPointIndex(indices[i] - 3); // prev + 3
            int fourPrevious = controlPoints.GetControlPointIndex(indices[i] - 4); // pre + 4

            bool conA = indices.Any(x => x == sixPlus);
            bool conB = indices.Any(x => x == threePrevious);
            bool conC = indices.Any(x => x == fourPrevious);

            if(conA && conB && conC)
            {
                fIndices[0] = indices[i];
                fIndices[1] = threePrevious;
                fIndices[2] = fourPrevious;
                break;
            }
        }

        if (fIndices.Any(x => x == -1))
            return false;

        return true;
    }

    public static bool IsHShaped(this IEnumerable<ControlPoint> controlPoints, out int[] hPointIndices)
    {
        return IsPolygonHShaped(controlPoints.GetPositions(), out hPointIndices);
    }

    public static bool IsPolygonHShaped(this IEnumerable<Vector3> controlPoints, out int[] hPointIndices)
    {
        hPointIndices = new int[] { -1, -1, -1, -1 };
        int[] indices = controlPoints.GetConcaveIndexPoints();

        if (controlPoints.Count() != 12 | indices.Length != hPointIndices.Length)
            return false;

        for (int i = 0; i < indices.Length; i++)
        {
            int fivePrevious = controlPoints.GetControlPointIndex(indices[i] - 5);
            int oneNext = controlPoints.GetControlPointIndex(indices[i] + 1);
            int sixNext = controlPoints.GetControlPointIndex(indices[i] + 6);
            int sevenNext = controlPoints.GetControlPointIndex(indices[i] + 7);

            bool conA = indices.Any(x => x == fivePrevious);
            bool conB = indices.Any(x => x == oneNext);
            bool conC = indices.Any(x => x == sixNext);
            bool conD = indices.Any(x => x == sevenNext);

            if (conA && conB && conC && conD)
            {
                hPointIndices[0] = indices[i];
                hPointIndices[1] = oneNext;
                hPointIndices[2] = sixNext;
                hPointIndices[3] = sevenNext;
                break;
            }
        }

        if (hPointIndices.Any(x => x == -1))
            return false;

        return true;
    }

    public static bool IsKShaped(this IEnumerable<ControlPoint> controlPoints, out int[] kPointIndices)
    {
        return IsPolygonKShaped(controlPoints.GetPositions(), out kPointIndices);
    }

    public static bool IsPolygonKShaped(this IEnumerable<Vector3> controlPoints, out int[] kPointIndices)
    {
        kPointIndices = new int[] { -1, -1, -1, -1 };
        int[] indices = controlPoints.GetConcaveIndexPoints();

        if (controlPoints.Count() != 12 | indices.Length != 4)
            return false;

        for (int i = 0; i < indices.Length; i++)
        {
            int next = controlPoints.GetControlPointIndex(indices[i] + 1);
            int fourNext = controlPoints.GetControlPointIndex(indices[i] + 4);
            int sevenNext = controlPoints.GetControlPointIndex(indices[i] + 7);

            bool conA = indices.Any(x => x == next);
            bool conB = indices.Any(x => x == fourNext);
            bool conC = indices.Any(x => x == sevenNext);

            if (conA && conB && conC)
            {
                kPointIndices[0] = indices[i];
                kPointIndices[1] = next;
                kPointIndices[2] = fourNext;
                kPointIndices[3] = sevenNext;
                break;
            }
        }

        if (kPointIndices.Any(x => x == -1))
            return false;


        return true;
    }

    public static bool IsYShaped(this IEnumerable<ControlPoint> controlPoints, out int[] yPointIndices)
    {
        return IsPolygonYShaped(controlPoints.GetPositions(), out yPointIndices);
    }

    public static bool IsPolygonYShaped(this IEnumerable<Vector3> controlPoints, out int[] yPointIndices)
    {
        yPointIndices = new int[] { -1, -1, -1 };
        int[] indices = controlPoints.GetConcaveIndexPoints();

        if (controlPoints.Count() != 9 | indices.Length != 3)
            return false;

        for( int i = 0; i < indices.Length; i++)
        {
            int threePrevious = controlPoints.GetControlPointIndex(indices[i] - 3);
            int threeNext = controlPoints.GetControlPointIndex(indices[i] + 3);

            bool conA = indices.Any(x => x == threePrevious);
            bool conB = indices.Any(x => x == threeNext);

            if(conA && conB)
            {
                yPointIndices[0] = indices[i];
                yPointIndices[1] = threeNext;
                yPointIndices[2] = threePrevious;
                break;
            }
        }

        if (yPointIndices.Any(x => x == -1))
            return false;

        return true;
    }

    public static bool IsZigZagShaped(this IEnumerable<ControlPoint> controlPoints, out int[] zigZagPointIndices)
    {
        return IsPolygonZigZagShaped(controlPoints.GetPositions(), out zigZagPointIndices);
    }

    public static bool IsPolygonZigZagShaped(this IEnumerable<Vector3> controlPoints, out int[] zigZagPointIndices)
    {
        int[] concavePoints = controlPoints.GetConcaveIndexPoints();
        zigZagPointIndices = new int[concavePoints.Length];

        if (controlPoints.Count() < 14 | controlPoints.Count() % 2 != 0 |
            concavePoints.Count() < 5  | concavePoints.Count() % 2 == 0)
            return false;

        zigZagPointIndices[0] = -1;

        int count = 0;

        List<Tuple<int, int, int>> data = new();

        // Find the first point.
        for (int i = 0; i < concavePoints.Length; i++)
        {
            int onePrevious = controlPoints.GetControlPointIndex(concavePoints[i] - 1);
            int twoPrevious = controlPoints.GetControlPointIndex(concavePoints[i] - 2);
            int threePrevious = controlPoints.GetControlPointIndex(concavePoints[i] - 3);
            int twoNext = controlPoints.GetControlPointIndex(concavePoints[i] + 2);

            bool conditionA = concavePoints.Any(a => a == onePrevious);
            bool conditionB = concavePoints.Any(b => b == twoPrevious);
            bool conditionC = concavePoints.Any(c => c == threePrevious);
            bool conditionD = concavePoints.Any(d => d == twoNext);

            // if true, we have found either end of the zig zag.
            if (!conditionA && !conditionB && !conditionC && conditionD)
            {
                // Looking at the zigzag, where the ends of the shape are on the left & right,
                // we can see that the number of concave points on the top & bottom of the shape differ.
                // With that we can find a start point.

                int numberOfConcavePoints = 1;

                while(concavePoints.Any(d => d == twoNext))
                {
                    twoNext = controlPoints.GetControlPointIndex(twoNext + 2);
                    numberOfConcavePoints++;
                }

                data.Add(new Tuple<int, int, int> (i, numberOfConcavePoints, count));

            }
            count++;
        }

        if (data.Count != 2)
            return false;

        if (data[0].Item2 > data[1].Item2)
        {
            zigZagPointIndices[0] = data[0].Item1;
            count = data[0].Item3;
        }
        else
        {
            zigZagPointIndices[0] = data[1].Item1;
            count = data[1].Item3;
        }

        if (zigZagPointIndices[0] == -1)
            return false;

        for(int i = 0; i < concavePoints.Length; i++)
        {
            zigZagPointIndices[i] = concavePoints[count];
            count++;

            if (count > concavePoints.Length - 1)
                count = 0;
        }

        if(zigZagPointIndices.Distinct().Count() != concavePoints.Count())
            return false;

        return true;
    }

    public static bool IsCrenelShaped(this IEnumerable<ControlPoint> controlPoints, out int[] crenelPointIndices)
    {
        return IsPolygonCrenelShaped(controlPoints.GetPositions(), out crenelPointIndices);
    }

    public static bool IsPolygonCrenelShaped(this IEnumerable<Vector3> controlPoints, out int[] crenelIndices)
    {
        int[] concavePoints = controlPoints.GetConcaveIndexPoints();
        crenelIndices = new int[concavePoints.Length];

        if(controlPoints.Count() < 16 | controlPoints.Count() % 2 != 0 |
            concavePoints.Length < 6 | concavePoints.Count() % 2 != 0 )
        {
            return false;
        }

        crenelIndices[0] = -1;
        int i;

        for(i = 0; i < concavePoints.Length; i++)
        {
            int threePrevious = controlPoints.GetControlPointIndex(concavePoints[i] - 3);
            int fourPrevious = controlPoints.GetControlPointIndex(concavePoints[i] - 4);
            int sevenPrevious = controlPoints.GetControlPointIndex(concavePoints[i] - 7);

            int oneNext = controlPoints.GetControlPointIndex(concavePoints[i] + 1);
            int fourNext = controlPoints.GetControlPointIndex(concavePoints[i] + 4);
            int fiveNext = controlPoints.GetControlPointIndex(concavePoints[i] + 5);
            int eightNext = controlPoints.GetControlPointIndex(concavePoints[i] + 8);
            int nineNext = controlPoints.GetControlPointIndex(concavePoints[i] + 9);

            bool conditionA = concavePoints.Any(a => a == threePrevious);
            bool conditionB = concavePoints.Any(b => b == fourPrevious);
            bool conditionC = concavePoints.Any(c => c == oneNext);
            bool conditionD = concavePoints.Any(d => d == fourNext);
            bool conditionE = concavePoints.Any(e => e == fiveNext);
            bool conditionF = concavePoints.Any(f => f == eightNext);
            bool conditionG = concavePoints.Any(g => g == nineNext);
            bool conditionH = concavePoints.Any(h => h == sevenPrevious);

            if (!conditionA && !conditionB &&
                conditionC && conditionD && conditionE &&
                conditionF && conditionG && conditionH)
            {
                crenelIndices[0] = concavePoints[i];
                break;
            }
        }

        if (crenelIndices[0] == -1)
            return false;

        for(int j = 0; j < concavePoints.Length; j++)
        {
            crenelIndices[j] = concavePoints[i];
            i++;

            if (i > concavePoints.Length - 1)
                i = 0;
        }
        

        return true;
    }

    public static bool IsAsteriskShaped(this IEnumerable<ControlPoint> controlPoints, out int[] asteriskPointIndices)
    {
        return IsPolygonAsteriskShaped(controlPoints.GetPositions(), out asteriskPointIndices);
    }

    public static bool IsPolygonAsteriskShaped(this IEnumerable<Vector3> controlPoints, out int[] asteriskPointIndices)
    {
        int[] concavePoints = controlPoints.GetConcaveIndexPoints();
        asteriskPointIndices = new int[concavePoints.Length];

        if (controlPoints.Count() < 15 | controlPoints.Count() % 2 == 0 |
            concavePoints.Length < 5 | concavePoints.Count() % 2 == 0)
        {
            return false;
        }

        asteriskPointIndices[0] = -1;

        int oneNext = controlPoints.GetControlPointIndex(concavePoints[0] + 1);
        int twoNext = controlPoints.GetControlPointIndex(concavePoints[0] + 2);
        int threeNext = controlPoints.GetControlPointIndex(concavePoints[0] + 3);

        bool conditionA = concavePoints.Any(x => x == oneNext);
        bool conditionB = concavePoints.Any(x => x == twoNext);
        bool conditionC = concavePoints.Any(x => x == threeNext);

        if(!conditionA && !conditionB && conditionC)
        {
            asteriskPointIndices = concavePoints;
        }

        if (asteriskPointIndices[0] == -1)
            return false;

        return true;
    }
    /// <summary>
    /// Only for polygons on the XZ plane.
    /// </summary>
    /// <param name="controlPoints"></param>
    /// <returns></returns>
    public static int[] GetConcaveIndexPoints(this IEnumerable<Vector3> controlPoints)
    {
        Vector3[] points = controlPoints.ToArray();

        Vector3 normal = points.CalculatePolygonFaceNormal();

        if(normal != Vector3.up || normal != Vector3.down)
        {
            Vector3 position = ProMaths.Average(points);
            Quaternion rotation = Quaternion.FromToRotation(normal, Vector3.up);

            for(int i = 0; i < points.Length; i++)
            {
                Vector3 firstEuler = rotation.eulerAngles;
                Vector3 v = Quaternion.Euler(firstEuler) * (points[i] - position) + position;
                points[i] = v;
            }
        }

        List<int> indices = new List<int>();

        for (int i = 0; i < points.Length; i++)
        {
            int previousPoint = points.GetPreviousControlPoint(i);
            int nextPoint = points.GetNextControlPoint(i);

            Vector3 nextForward = Vector3Extensions.DirectionToTarget(points[i], points[nextPoint]);
            Vector3 previousForward = Vector3Extensions.DirectionToTarget(points[i], points[previousPoint]);
            Vector3 inbetweenForward = Vector3.Lerp(nextForward, previousForward, 0.5f);

            Vector3 a = points[i] + inbetweenForward;

            if (!points.IsPointInsidePolygon(a))
            {
                indices.Add(i);
            }
        }

        return indices.ToArray();
    }

    public static int[] GetConvexIndexPoints(this IEnumerable<Vector3> controlPoints)
    {
        int[] concavePoints = GetConcaveIndexPoints(controlPoints);
        if(concavePoints.Length == 0)
            return concavePoints;

        int[] indices = Enumerable.Range(0, controlPoints.Count()).ToArray();
        indices = indices.Except(concavePoints).ToArray();
        return indices;
    }
    /// <summary>
    /// For concave polygons, the scaling is based upon the defined forward vector for the control point.
    /// </summary>
    /// <param name="controlPoints"></param>
    /// <param name="scaleFactor"></param>
    /// <returns></returns>
    public static ControlPoint[] ScalePolygon(this IEnumerable<ControlPoint> controlPoints, float scaleFactor, bool convexScaling = false)
    {
        if (GetConcaveIndexPoints(controlPoints.GetPositions()).Length == 0 && convexScaling == false)
            controlPoints.SetPositions(ConvexScale(GetPositions(controlPoints), scaleFactor));

        ControlPoint[] points = controlPoints.ToArray();

        for(int i = 0; i < points.Length; i++)
        {
            points[i] += points[i].Forward * scaleFactor;
        }
        
        return points;
    }

    public static Vector3[] ConvexScale(this IEnumerable<Vector3> controlPoints, float scaleFactor)
    {
        Vector3[] points = controlPoints.ToArray();

        if(GetConcaveIndexPoints(controlPoints).Length != 0)
            return points;

        Vector3 centre = ProMaths.Average(points);

        for(int i = 0; i < points.Length; i++)
        {
            Vector3 point = points[i] - centre;
            Vector3 v = point * scaleFactor + centre;
            points[i] = v;
        }
        return points;
    }

    /// <summary>
    /// Returns an index to the next control point.
    /// </summary>
    /// <param name="polyShape"></param>
    /// <param name="currentIndex"></param>
    /// <returns></returns>
    public static int GetNextPoint(this PolyPath polyTool, int index)
    {
        return polyTool.Positions.GetNextControlPoint(index);
    }

    public static int GetNext(this IEnumerable<ControlPoint> controlPoints, int index)
    {
        return GetNextControlPoint(GetPositions(controlPoints), index);
    }

    public static int GetIndex(this IEnumerable<ControlPoint> controlPoints, int index)
    {
        return GetControlPointIndex(controlPoints.GetPositions(), index);
    }

    public static int GetControlPointIndex(this IEnumerable<Vector3> controlPoints, int index)
    {
        while (index < 0)
        {
            index = controlPoints.Count() - Mathf.Abs(index);
        }

        while (index >= controlPoints.Count())
        {
            index -= controlPoints.Count();
        }

        return index;
    }

    public static int GetNextControlPoint(this IEnumerable<Vector3> controlPoints, int index)
    {
        index = controlPoints.GetControlPointIndex(index);

        int next = 1;

        if (index == controlPoints.Count() - 1)
            next = -(controlPoints.Count() - 1);

        return index + next;
    }

    /// <summary>
    /// Returns an index to the previous control point.
    /// </summary>
    /// <param name="polyTool"></param>
    /// <param name="currentIndex"></param>
    /// <returns></returns>
    public static int GetPreviousPoint(this PolyPath polyTool, int index)
    {
        return polyTool.Positions.GetPreviousControlPoint(index);
    }

    public static int GetPrevious(this IEnumerable<ControlPoint> controlPoints, int index)
    {
        return GetPreviousControlPoint(GetPositions(controlPoints), index);
    }

    public static int GetPreviousControlPoint(this IEnumerable<Vector3> controlPoints, int index)
    {
        index = controlPoints.GetControlPointIndex(index);
        int previous = -1;

        if (index == 0)
            previous = controlPoints.Count() - 1;

        return index + previous;
    }

    public static bool IsConcave(this PolyPath polyTool, out int[] concavePoints)
    {
        concavePoints = GetConcaveIndexPoints(polyTool.Positions);

        return concavePoints.Length > 0;
    }

    public static bool IsConcave(this IEnumerable<ControlPoint> controlPoints, out int[] concavePoints)
    {
        concavePoints = GetConcaveIndexPoints(GetPositions(controlPoints));

        return concavePoints.Length > 0;
    }

    public static bool IsPolygonConcave(this IEnumerable<Vector3> controlPoints, out int[] concavePoints)
    {
        concavePoints = GetConcaveIndexPoints(controlPoints);

        return concavePoints.Length > 0;
    }


    /// <summary>
    /// If true, out returns a 1D representation of the polyshape.
    /// Do points converge to create a line through the centre of the shape.
    /// </summary>
    /// <param name="polyTool"></param>
    /// <param name="oneLine"></param>
    /// <returns></returns>
    public static bool IsDescribableInOneLine(this PolyPath polyTool, out Vector3[] oneLine, out OneLineShape shape, out int shapeIndex)
    {
        return IsPolygonDescribableInOneLine(polyTool.Positions, out oneLine, out shape, out shapeIndex);
    }

    public static bool IsDescribableInOneLine(this IEnumerable<ControlPoint> controlPoints, out Vector3[] oneLine, out OneLineShape shape, out int shapeIndex)
    {
        return IsPolygonDescribableInOneLine(GetPositions(controlPoints), out oneLine, out shape, out shapeIndex);
    }

    public static bool IsPolygonDescribableInOneLine(this IEnumerable<Vector3> controlPoints, out Vector3[] oneLine, out OneLineShape shape, out int shapeIndex)
    {
        List<Vector3> points = FindPathPoints(controlPoints).ToList();

        oneLine = new Vector3[0];
        shape = OneLineShape.Unknown;
        shapeIndex = -1;

        switch (points.Count)
        {
            case 4:
                oneLine = new Vector3[2];
                oneLine[0] = Vector3.Lerp(points[0], points[1], 0.5f);
                oneLine[1] = Vector3.Lerp(points[2], points[3], 0.5f);
                shape = OneLineShape.Square;
                shapeIndex = 0;
                return true;
            case 6:
                oneLine = new Vector3[3];
                int index;

                if (points.IsPolygonLShaped(out index))
                {
                    int[] lIndices = points.RelativeIndices(index);

                    oneLine[0] = Vector3.Lerp(points[lIndices[1]], points[lIndices[2]], 0.5f);
                    oneLine[1] = Vector3.Lerp(points[lIndices[0]], points[lIndices[3]], 0.5f);
                    oneLine[2] = Vector3.Lerp(points[lIndices[5]], points[lIndices[4]], 0.5f);
                    shape = OneLineShape.L;
                    shapeIndex = index;
                    return true;
                }
                break;
            case 8:
                oneLine = new Vector3[4];
                int[] indices;

                Vector3 start = Vector3.zero, second = Vector3.zero, third = Vector3.zero, last = Vector3.zero;

                if (points.IsPolygonTShaped(out indices))
                {
                    int[] tIndices = points.RelativeIndices(indices[0]);

                    start = Vector3.Lerp(points[tIndices[1]], points[tIndices[2]], 0.5f);
                    second = Vector3.Lerp(points[tIndices[7]], points[tIndices[6]], 0.5f);
                    third = Vector3.Lerp(points[tIndices[4]], points[tIndices[5]], 0.5f);
                    last = Vector3.Lerp(second, third, 0.5f);
                    shape = OneLineShape.T;
                    shapeIndex = tIndices[0];
                }
                if (points.IsPolygonUShaped(out indices))
                {
                    int[] uIndices = points.RelativeIndices(indices[0]);

                    start = Vector3.Lerp(points[uIndices[7]], points[uIndices[6]], 0.5f);
                    second = Vector3.Lerp(points[uIndices[0]], points[uIndices[5]], 0.5f);
                    third = Vector3.Lerp(points[uIndices[1]], points[uIndices[4]], 0.5f);
                    last = Vector3.Lerp(points[uIndices[2]], points[uIndices[3]], 0.5f);
                    shape = OneLineShape.U;
                    shapeIndex = uIndices[0];
                }
                if (points.IsPolygonSimpleNShaped(out indices))
                {
                    int[] nIndices = points.RelativeIndices(indices[0]);

                    start = Vector3.Lerp(points[nIndices[1]], points[nIndices[2]], 0.5f);
                    second = Vector3.Lerp(points[nIndices[0]], points[nIndices[3]], 0.5f);
                    third = Vector3.Lerp(points[nIndices[4]], points[nIndices[7]], 0.5f);
                    last = Vector3.Lerp(points[nIndices[5]], points[nIndices[6]], 0.5f);
                    shape = OneLineShape.SimpleN;
                    shapeIndex = nIndices[0];
                }

                oneLine[0] = start;
                oneLine[1] = second;
                oneLine[2] = third;
                oneLine[3] = last;
                return true;
            case 9:
                if (points.IsPolygonArrowShaped(out int[] arrowPointIndices))
                {
                    oneLine = new Vector3[4];

                    int[] relativeIndices = points.RelativeIndices(arrowPointIndices[0]);

                    oneLine[0] = Vector3.Lerp(points[relativeIndices[8]], points[relativeIndices[7]], 0.5f);
                    oneLine[1] = Vector3.Lerp(points[relativeIndices[1]], points[relativeIndices[2]], 0.5f);
                    oneLine[2] = Vector3.Lerp(points[relativeIndices[4]], points[relativeIndices[5]], 0.5f);
                    Extensions.DoLinesIntersect(oneLine[0], points[relativeIndices[3]], oneLine[1], Vector3.Lerp(points[relativeIndices[0]], points[relativeIndices[3]], 0.5f), out oneLine[3]);
                    shape = OneLineShape.Arrow;
                    shapeIndex = arrowPointIndices[0];
                    return true;

                }
                else if (points.IsPolygonYShaped(out int[] yPointIndices))
                {
                    oneLine = new Vector3[4];

                    int[] relativeIndices = points.RelativeIndices(yPointIndices[0]);

                    oneLine[0] = Vector3.Lerp(points[relativeIndices[1]], points[relativeIndices[2]], 0.5f);
                    oneLine[1] = Vector3.Lerp(points[relativeIndices[4]], points[relativeIndices[5]], 0.5f);
                    oneLine[2] = Vector3.Lerp(points[relativeIndices[7]], points[relativeIndices[8]], 0.5f);
                    oneLine[3] = ProMaths.Average(new Vector3[] { points[yPointIndices[0]], points[yPointIndices[1]], points[yPointIndices[2]] });
                    shape = OneLineShape.Y;
                    shapeIndex = yPointIndices[0];
                    return true;
                }
                return false;
            case 10:
                if (points.IsPolygonNShaped(out int[] nPointIndices))
                {
                    oneLine = new Vector3[6];
                    int[] nIndices = points.RelativeIndices(nPointIndices[0]);

                    oneLine[0] = Vector3.Lerp(points[nIndices[1]], points[nIndices[2]], 0.5f);
                    oneLine[1] = Vector3.Lerp(points[nIndices[0]], points[nIndices[3]], 0.5f);
                    oneLine[2] = Vector3.Lerp(points[nIndices[0]], points[nIndices[4]], 0.5f);
                    oneLine[3] = Vector3.Lerp(points[nIndices[5]], points[nIndices[9]], 0.5f);
                    oneLine[4] = Vector3.Lerp(points[nIndices[5]], points[nIndices[8]], 0.5f);
                    oneLine[5] = Vector3.Lerp(points[nIndices[6]], points[nIndices[7]], 0.5f);
                    shape = OneLineShape.N;
                    shapeIndex = nPointIndices[0];
                    return true;
                }
                else if(points.IsPolygonSimpleMShaped(out int[] simpleMPointIndices))
                {
                    oneLine = new Vector3[5];
                    int[] mIndices = points.RelativeIndices(simpleMPointIndices[0]);

                    oneLine[0] = Vector3.Lerp(points[mIndices[9]], points[mIndices[8]], 0.5f);
                    oneLine[1] = Vector3.Lerp(points[mIndices[0]], points[mIndices[7]], 0.5f);
                    oneLine[2] = Vector3.Lerp(points[mIndices[1]], points[mIndices[6]], 0.5f);
                    oneLine[3] = Vector3.Lerp(points[mIndices[2]], points[mIndices[5]], 0.5f);
                    oneLine[4] = Vector3.Lerp(points[mIndices[3]], points[mIndices[4]], 0.5f);
                    shape = OneLineShape.SimpleM;
                    shapeIndex = simpleMPointIndices[0];
                    return true;
                }
                else if(points.IsPolygonFShaped(out int[] fIndices))
                {
                    oneLine = new Vector3[5];
                    int[] relIndices = points.RelativeIndices(fIndices[0]);

                    oneLine[0] = Vector3.Lerp(points[relIndices[1]], points[relIndices[2]], 0.5f);
                    oneLine[2] = Vector3.Lerp(points[relIndices[8]], points[relIndices[9]], 0.5f);
                    oneLine[3] = Vector3.Lerp(points[relIndices[3]], points[relIndices[6]], 0.5f);
                    oneLine[4] = Vector3.Lerp(points[relIndices[4]], points[relIndices[5]], 0.5f);

                    Vector3 line2Start = Vector3.Lerp(points[relIndices[0]], points[relIndices[7]], 0.5f);
                    Vector3 dir = points[relIndices[0]].DirectionToTarget(points[relIndices[7]]);
                    Vector3 targetDirection = Vector3.Cross(dir, Vector3.up);
                    Vector3 line2End = line2Start + targetDirection * points.PolygonLength();

                    if(Extensions.DoLinesIntersect(oneLine[0], oneLine[3], line2Start, line2End, out Vector3 intersection))
                    {
                        oneLine[1] = intersection;
                    }
                    shape = OneLineShape.F;
                    shapeIndex = fIndices[0];
                    return true;
                }
                return false;
            case 12:
                if (points.IsPolygonEShaped(out int[] ePointIndices))
                {
                    oneLine = new Vector3[6];

                    int[] eIndices = points.RelativeIndices(ePointIndices[0]);

                    // Creating a line segment to calculate oneline 4.
                    Vector3 line2Start = Vector3.Lerp(points[eIndices[1]], points[eIndices[4]], 0.5f);
                    Vector3 dir = points[eIndices[1]].DirectionToTarget(points[eIndices[4]]);
                    Vector3 targetDirection = Vector3.Cross(dir, Vector3.up);
                    Vector3 line2End = line2Start + targetDirection * points.PolygonLength();

                    oneLine[0] = Vector3.Lerp(points[eIndices[11]], points[eIndices[10]], 0.5f);
                    oneLine[1] = Vector3.Lerp(points[eIndices[0]], points[eIndices[9]], 0.5f);
                    oneLine[2] = Vector3.Lerp(points[eIndices[5]], points[eIndices[8]], 0.5f);
                    oneLine[3] = Vector3.Lerp(points[eIndices[6]], points[eIndices[7]], 0.5f);

                    if(Extensions.DoLinesIntersect(oneLine[1], oneLine[2], line2Start, line2End, out Vector3 intersection))
                    {
                        oneLine[4] = intersection;
                    }
                    oneLine[5] = Vector3.Lerp(points[eIndices[2]], points[eIndices[3]], 0.5f);
                    shape = OneLineShape.E;
                    shapeIndex = ePointIndices[0];
                    return true;
                }
                else if (points.IsPolygonHShaped(out int[] hIndices))
                {
                    oneLine = new Vector3[6];

                    int[] relIndices = points.RelativeIndices(hIndices[0]);

                    oneLine[0] = Vector3.Lerp(points[relIndices[11]], points[relIndices[10]], 0.5f);
                    oneLine[2] = Vector3.Lerp(points[relIndices[9]], points[relIndices[8]], 0.5f);
                    oneLine[3] = Vector3.Lerp(points[relIndices[5]], points[relIndices[4]], 0.5f);
                    oneLine[5] = Vector3.Lerp(points[relIndices[2]], points[relIndices[3]], 0.5f);

                    Vector3 mid = Vector3.Lerp(points[relIndices[0]], points[relIndices[7]], 0.5f);
                    Vector3 dir = points[relIndices[0]].DirectionToTarget(points[relIndices[7]]);
                    Vector3 targetDirection = Vector3.Cross(dir, Vector3.up);
                    float polygonLength = points.PolygonLength();
                    Vector3 line2Start = mid + targetDirection * polygonLength;
                    Vector3 line2End = mid + ((-targetDirection) * polygonLength);

                    Vector3 intersection;

                    if (Extensions.DoLinesIntersect(oneLine[0], oneLine[2], line2Start, line2End, out intersection))
                    {
                        oneLine[1] = intersection;
                    }

                    mid = Vector3.Lerp(points[relIndices[1]], points[relIndices[6]], 0.5f);
                    dir = points[relIndices[1]].DirectionToTarget(points[relIndices[6]]);
                    targetDirection = Vector3.Cross(Vector3.up, dir);
                    line2Start = mid + targetDirection * polygonLength;
                    line2End = mid + ((-targetDirection) * polygonLength);

                    if (Extensions.DoLinesIntersect(oneLine[3], oneLine[5], line2Start, line2End, out intersection))
                    {
                        oneLine[4] = intersection;
                    }
                    shape = OneLineShape.H;
                    shapeIndex = hIndices[0];
                    return true;
                }
                else if (points.IsPolygonMShaped(out int[] mPointIndices))
                {
                    oneLine = new Vector3[7];

                    int[] mIndices = points.RelativeIndices(mPointIndices[0]);

                    oneLine[0] = Vector3.Lerp(points[mIndices[11]], points[mIndices[10]], 0.5f);
                    oneLine[1] = Vector3.Lerp(points[mIndices[0]], points[mIndices[9]], 0.5f);
                    oneLine[2] = Vector3.Lerp(points[mIndices[0]], points[mIndices[8]], 0.5f);
                    oneLine[3] = Vector3.Lerp(points[mIndices[1]], points[mIndices[7]], 0.5f);
                    oneLine[4] = Vector3.Lerp(points[mIndices[2]], points[mIndices[6]], 0.5f);
                    oneLine[5] = Vector3.Lerp(points[mIndices[2]], points[mIndices[5]], 0.5f);
                    oneLine[6] = Vector3.Lerp(points[mIndices[3]], points[mIndices[4]], 0.5f);
                    shape = OneLineShape.M;
                    shapeIndex = mPointIndices[0];
                    return true;
                }
                else if(points.IsPolygonKShaped(out int[] kPointIndices))
                {
                    oneLine = new Vector3[6];

                    int[] kIndices = points.RelativeIndices(kPointIndices[0]);

                    oneLine[0] = Vector3.Lerp(points[kIndices[11]], points[kIndices[10]], 0.5f);
                    oneLine[2] = Vector3.Lerp(points[kIndices[9]], points[kIndices[8]], 0.5f);
                    oneLine[3] = Vector3.Lerp(points[kIndices[6]], points[kIndices[5]], 0.5f);
                    oneLine[4] = Vector3.Lerp(points[kIndices[3]], points[kIndices[2]], 0.5f);

                    Vector3 line2End = Vector3.Lerp(points[kIndices[0]], points[kIndices[7]], 0.5f);

                    Extensions.DoLinesIntersect(oneLine[0], oneLine[2], oneLine[3], line2End, out oneLine[1]);

                    Vector3 line1End = Vector3.Lerp(points[kIndices[1]], points[kIndices[4]], 0.5f);

                    Extensions.DoLinesIntersect(oneLine[4], line1End, oneLine[3], line2End, out oneLine[5]);
                    shape = OneLineShape.K;
                    shapeIndex = kPointIndices[0];
                    return true;
                }
                else if (points.IsPolygonXShaped(out int[] xPointIndices))
                {
                    oneLine = new Vector3[5];

                    for (int i = 0; i < xPointIndices.Length; i++)
                    {
                        int onePointNext = points.GetNextControlPoint(xPointIndices[i]);
                        int twoPointNext = points.GetNextControlPoint(onePointNext);
                        oneLine[i] = Vector3.Lerp(points[onePointNext], points[twoPointNext], 0.5f);
                    }

                    Vector3[] xPoints = new Vector3[] { points[xPointIndices[0]],
                        points[xPointIndices[1]],
                        points[xPointIndices[2]],
                        points[xPointIndices[3]] };

                    oneLine[4] = ProMaths.Average(xPoints);
                    shape = OneLineShape.X;
                    shapeIndex = xPointIndices[0];
                    return true;
                }
                else if (points.IsPolygonInterlockingYShaped(out int[] interlockingYIndices))
                {
                    oneLine = new Vector3[6];

                    int[] relativeIndices = points.RelativeIndices(interlockingYIndices[0]);

                    oneLine[0] = Vector3.Lerp(points[relativeIndices[6]], points[relativeIndices[7]], 0.5f);
                    oneLine[1] = Vector3.Lerp(points[relativeIndices[5]], points[relativeIndices[8]], 0.5f);
                    oneLine[2] = ProMaths.Average(new Vector3[] { points[relativeIndices[0]], points[relativeIndices[4]], points[relativeIndices[5]], points[relativeIndices[9]] });
                    oneLine[3] = Vector3.Lerp(points[relativeIndices[1]], points[relativeIndices[4]], 0.5f);
                    oneLine[4] = Vector3.Lerp(points[relativeIndices[2]], points[relativeIndices[3]], 0.5f);
                    oneLine[5] = Vector3.Lerp(points[relativeIndices[10]], points[relativeIndices[11]], 0.5f);
                    shape = OneLineShape.InterlockY;
                    shapeIndex = interlockingYIndices[0];
                    return true;
                }

                return false;
        }

        if(points.IsPolygonZigZagShaped(out int[] zigZagPointIndices))
        {
            int length = points.Count() / 2;
            oneLine = new Vector3[length];


            int[] relative = points.RelativeIndices(zigZagPointIndices[0]);
            int previous = relative.Length - 3;

            oneLine[0] = Vector3.Lerp(points[relative[^1]], points[relative[^2]], 0.5f);

            int itr = 1;
            for (int i = 0; i < zigZagPointIndices.Length + 1; i++)
            {
                oneLine[itr] = Vector3.Lerp(points[relative[i]], points[relative[previous]], 0.5f);

                previous --;
                itr++;
            }
            shape = OneLineShape.ZigZag;
            shapeIndex = zigZagPointIndices[0];
            return true;
        }
        else if(points.IsPolygonCrenelShaped(out int[] crenelPointIndices))
        {
            int length = points.Count() / 2;//
            oneLine = new Vector3[length];

            int[] startIndices = points.RelativeIndices(crenelPointIndices[0]);
            int[] endIndices = points.RelativeIndices(crenelPointIndices[^1]);

            // Start Points
            oneLine[0] = Vector3.Lerp(points[startIndices[^1]], points[startIndices[^2]], 0.5f);
            oneLine[1] = Vector3.Lerp(points[startIndices[0]], points[startIndices[^3]], 0.5f);

            // End Points
            oneLine[^2] = Vector3.Lerp(points[endIndices[0]], points[endIndices[3]], 0.5f);
            oneLine[^1] = Vector3.Lerp(points[endIndices[1]], points[endIndices[2]], 0.5f);

            int count = crenelPointIndices.Length;

            int start = 1;
            int end = 4;

            float polygonLength = points.PolygonLength();

            for (int i = 2; i < count; i+= 2)
            {
                Vector3 line2Start = Vector3.Lerp(points[startIndices[start]], points[startIndices[end]], 0.5f);
                Vector3 dir = points[startIndices[1]].DirectionToTarget(points[startIndices[4]]);
                Vector3 targetDirection = Vector3.Cross(dir, Vector3.up);
                Vector3 line2End = line2Start + targetDirection * polygonLength;

                if(Extensions.DoLinesIntersect(oneLine[1], oneLine[^2], line2Start, line2End, out Vector3 intersection))
                {
                    oneLine[i] = intersection;
                }
                oneLine[i+1] = Vector3.Lerp(points[startIndices[start + 1]], points[startIndices[end - 1]], 0.5f);

                start += 4;
                end += 4;
            }

            shape = OneLineShape.Crenel;
            shapeIndex = crenelPointIndices[0];
            return true;
        }
        else if(points.IsPolygonAsteriskShaped(out int[] asteriskPointIndices))
        {
            oneLine = new Vector3[asteriskPointIndices.Length + 1];

            Vector3[] asteriskPoints = new Vector3[asteriskPointIndices.Length];

            for (int i = 0; i < asteriskPointIndices.Length; i++)
            {
                int current = asteriskPointIndices[i];
                int nextOne = points.GetControlPointIndex(current + 1);
                int nextTwo = points.GetControlPointIndex(current + 2);

                asteriskPoints[i] = points[current];
                oneLine[i] = Vector3.Lerp(points[nextOne], points[nextTwo], 0.5f);

            }

            oneLine[^1] = ProMaths.Average(asteriskPoints);
            shapeIndex = asteriskPointIndices[0];
            return true;
        }
        else if(points.IsPolygonAntennaShaped(out int[] antPointIndices)) // This implementation is quite hacky.
        {
            int size = 9;

            if(points.Count() > 20)
            {
                int count = points.Count() - 20;
                int itr = (count / 8) * 3;
                size += itr;
            }

            oneLine = new Vector3[size];
            Vector3[] unsortedPoints = new Vector3[size];

            List<Vector3> top = new();
            List<Vector3> bottom = new();
            List<Vector3> middle = new();

            int j = 0;
            for (int i = 0; i < antPointIndices.Length; i+= 2)
            {
                int current = antPointIndices[i];
                
                int nextOne = points.GetControlPointIndex(current + 1);
                int nextTwo = points.GetControlPointIndex(current + 2);

                unsortedPoints[j] = Vector3.Lerp(points[nextOne], points[nextTwo], 0.5f);

                if(i > 0 && i < antPointIndices.Length / 2)
                {
                    top.Add(unsortedPoints[j]);
                }
                if(i > antPointIndices.Length / 2)
                {
                    bottom.Add(unsortedPoints[j]);
                }

                if(i == 0 || i == antPointIndices.Length/2)
                {
                    int previous = points.GetControlPointIndex(current - 1);
                    int nextThree = points.GetControlPointIndex(current + 3);
                    int nextFour = points.GetControlPointIndex(current + 4);
                    int nextFive = points.GetControlPointIndex(current + 5);
                    int nextSix = points.GetControlPointIndex(current + 6);

                    
                    Vector3 next = Vector3.Lerp(points[nextThree], points[nextFour], 0.5f);
                    Vector3 line2Start = Vector3.Lerp(points[previous], points[nextSix], 0.5f);
                    Vector3 line2End = Vector3.Lerp(points[current], points[nextFive], 0.5f);

                    j++;
                    Extensions.DoLinesIntersect(unsortedPoints[j - 1], next, line2Start, line2End,  out unsortedPoints[j]);
                    j++;
                    unsortedPoints[j] = next;
                }
                else
                {
                    middle.Add(Vector3.Lerp(points[current], points[antPointIndices[i + 1]], 0.5f));
                }
                j++;
            }

            int mid = size / 2;

            if (size == 9)
                mid++;

            oneLine[0] = unsortedPoints[0];
            oneLine[1] = unsortedPoints[1];
            oneLine[2] = unsortedPoints[2];

            int index = 4;
            int last = 1;
            for (int i = 0; i < middle.Count/2; i++)
            {
                oneLine[index] = Vector3.Lerp(middle[i], middle[^last], 0.5f);
                index += 3;
                last++;
            }

            bottom.Reverse();
            int b = 3;
            int t = 5;

            for (int i = 0; i < top.Count; i++)
            {
                oneLine[b] = bottom[i];
                oneLine[t] = top[i];
                b += 3;
                t += 3;
            }

            oneLine[^1] = unsortedPoints[mid-1];
            oneLine[^2] = unsortedPoints[mid];
            oneLine[^3] = unsortedPoints[mid+1];

            shape = OneLineShape.Antenna;
            shapeIndex = antPointIndices[0];
            return true;
        }

        return false;
    }
    /// <summary>
    /// Indices for indices
    /// </summary>
    /// <param name="controlPoints"></param>
    /// <param name="startIndex"></param>
    /// <returns></returns>
    public static int[] RelativeIndices(this IEnumerable<ControlPoint> controlPoints, int startIndex)
    {
        return RelativeIndices(controlPoints.GetPositions(), startIndex);
    }

    public static int[] RelativeIndices(this IEnumerable<Vector3> controlPoints, int startIndex)
    {
        int[] relativeIndices = new int[controlPoints.Count()];
        relativeIndices[0] = startIndex;

        for (int i = 1; i < relativeIndices.Length; i++)
        {
            relativeIndices[i] = controlPoints.GetNextControlPoint(relativeIndices[i - 1]);
        }

        return relativeIndices;
    }

    public static Vector3[] GetPositions(this IEnumerable<ControlPoint> controlPoints)
    {
        ControlPoint[] points = controlPoints.ToArray();
        Vector3[] positions = new Vector3[points.Length];

        for(int i = 0; i < points.Length; i++)
        {
            positions[i] = points[i].Position;
        }

        return positions;
    }

    public static void SetPositions(this IEnumerable<ControlPoint> controlPoints, Vector3[] positions)
    {
        ControlPoint[] points = controlPoints.ToArray();

        for (int i = 0; i < points.Length; i++)
        {
            points[i].SetPosition(positions[i]);
        }

        controlPoints = points;
    }

    public static void SetPositions(this IEnumerable<ControlPoint> controlPoints, ControlPoint[] other)
    {
        SetPositions(controlPoints, other.GetPositions());
    }

    public static ControlPoint[] Clone(this ControlPoint[] controlPoints)
    {
        ControlPoint[] clone = new ControlPoint[controlPoints.Length];

        for(int i = 0; i < clone.Length; i++ )
        {
            clone[i] = new ControlPoint(controlPoints[i]);
        }

        return clone;
    }
}
