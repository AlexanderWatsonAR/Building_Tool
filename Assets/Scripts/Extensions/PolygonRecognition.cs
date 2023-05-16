using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using ProMaths = UnityEngine.ProBuilder.Math;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using System;

public static class PolygonRecognition
{
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
    /// <param name="polyShape"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public static bool IsPointInside(this Polytool polyTool, Vector3 point)
    {
        return IsPointInsidePolygon(polyTool.Positions, point);
    }
    /// <summary>
    /// Assumes Polygon is orientated on the 'y'.
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
            c ^= controlPointsArray[i].z > point.z ^ controlPointsArray[j].z > point.z && point.x < (controlPointsArray[j].x - controlPointsArray[i].x) * (point.z - controlPointsArray[i].z) / (controlPointsArray[j].z - controlPointsArray[i].z) + controlPointsArray[i].x;
        }
        return c;
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
    public static bool IsLShaped(this Polytool polyTool, out int lPointIndex)
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


    /// <summary>
    /// Does the Polyshape resemble a 'T'? 
    /// </summary>
    /// <param name="polyTool"></param>
    /// <param name="tPointsIndex"></param>
    /// <returns></returns>
    public static bool IsTShaped(this Polytool polyTool, out int[] tPointsIndex)
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
    public static bool IsUShaped(this Polytool polyTool, out int[] uPointsIndex)
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

    public static bool IsEShaped(this Polytool polyTool, out int[] ePointsIndex)
    {
        return IsPolygonEShaped(polyTool.Positions, out ePointsIndex);
    }

    public static bool IsPolygonEShaped(this IEnumerable<Vector3> controlPoints, out int[] ePointsIndices)
    {
        int[] indices = controlPoints.GetConcaveIndexPoints();
        ePointsIndices = new int[4];

        Vector3[] points = controlPoints.ToArray();

        if (points.Length != 12 && indices.Length == ePointsIndices.Length)
            return false;

        // Organise points.
        for(int i = 0; i < indices.Length; i++)
        {
            int oneNext = controlPoints.GetNextControlPoint(indices[i]);
            int twoNext = controlPoints.GetNextControlPoint(oneNext);
            int threeNext = controlPoints.GetNextControlPoint(twoNext);

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

    public static bool IsXShaped(this Polytool polyTool, out int[] xPointIndices)
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

    public static bool IsNShaped(this Polytool polyTool, out int[] nPointIndices)
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

        int indices0Next = controlPoints.GetNextControlPoint(indices[0]);
        int indices0Next1 = controlPoints.GetNextControlPoint(indices0Next);
        int indices0Next2 = controlPoints.GetNextControlPoint(indices0Next1);
        int indices0Next3 = controlPoints.GetNextControlPoint(indices0Next2);

        int indices1Next = controlPoints.GetNextControlPoint(indices[1]);
        int indices1Next1 = controlPoints.GetNextControlPoint(indices1Next);
        int indices1Next2 = controlPoints.GetNextControlPoint(indices1Next1);
        int indices1Next3 = controlPoints.GetNextControlPoint(indices1Next2);

        if (indices[0] == indices1Next3 &&
            indices[1] == indices0Next3)
        {
            return true;
        }

        return false;
    }
    public static bool IsMShaped(this IEnumerable<ControlPoint> controlPoints, out int[] mPointIndices)
    {
        return IsPolygonMShaped(controlPoints.GetPositions(), out mPointIndices);
    }

    public static bool IsMShaped(this Polytool polyTool, out int[] mPointIndices)
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

    public static int[] GetConcaveIndexPoints(this IEnumerable<Vector3> controlPoints)
    {
        Vector3[] points = controlPoints.ToArray();

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
    public static int GetNextPoint(this Polytool polyTool, int index)
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
    public static int GetPreviousPoint(this Polytool polyTool, int index)
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

    public static bool IsConcave(this Polytool polyTool, out int[] concavePoints)
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
    /// </summary>
    /// <param name="polyTool"></param>
    /// <param name="oneLine"></param>
    /// <returns></returns>
    public static bool IsDescribableInOneLine(this Polytool polyTool, out Vector3[] oneLine)
    {
        return IsPolygonDescribableInOneLine(polyTool.Positions, out oneLine);
    }

    public static bool IsDescribableInOneLine(this IEnumerable<ControlPoint> controlPoints, out Vector3[] oneLine)
    {
        return IsPolygonDescribableInOneLine(GetPositions(controlPoints), out oneLine);
    }

    public static bool IsPolygonDescribableInOneLine(this IEnumerable<Vector3> controlPoints, out Vector3[] oneLine)
    {
        List<Vector3> points = controlPoints.ToList();
        oneLine = new Vector3[0];

        if (points.Count % 2 != 0)
            return false;
        
        switch (points.Count)
        {
            case 4:
                oneLine = new Vector3[2];
                oneLine[0] = Vector3.Lerp(points[0], points[1], 0.5f);
                oneLine[1] = Vector3.Lerp(points[2], points[3], 0.5f);
                return true;
            case 6:
                oneLine = new Vector3[3];
                int index;

                if (points.IsPolygonLShaped(out index))
                {
                    int onePointNext = points.GetNextControlPoint(index);
                    int twoPointNext = points.GetNextControlPoint(onePointNext);
                    int threePointNext = points.GetNextControlPoint(twoPointNext);

                    int onePointPrevious = points.GetPreviousControlPoint(index);
                    int twoPointPrevious = points.GetPreviousControlPoint(onePointPrevious);

                    oneLine[0] = Vector3.Lerp(points[onePointNext], points[twoPointNext], 0.5f);
                    oneLine[1] = Vector3.Lerp(points[index], points[threePointNext], 0.5f);
                    oneLine[2] = Vector3.Lerp(points[onePointPrevious], points[twoPointPrevious], 0.5f);
                    return true;
                }
                break;
            case 8:
                oneLine = new Vector3[4];
                int[] indices;

                Vector3 start = Vector3.zero, second = Vector3.zero, third = Vector3.zero, last = Vector3.zero;

                if (points.IsPolygonTShaped(out indices))
                {
                    int onePointNext = points.GetNextControlPoint(indices[0]);
                    int twoPointNext = points.GetNextControlPoint(onePointNext);

                    int onePointPrevious = points.GetPreviousControlPoint(indices[0]);
                    int twoPointPrevious = points.GetPreviousControlPoint(onePointPrevious);

                    start = Vector3.Lerp(points[onePointNext], points[twoPointNext], 0.5f);
                    second = Vector3.Lerp(points[onePointPrevious], points[twoPointPrevious], 0.5f);

                    onePointNext = points.GetNextControlPoint(indices[1]);
                    twoPointNext = points.GetNextControlPoint(onePointNext);

                    third = Vector3.Lerp(points[onePointNext], points[twoPointNext], 0.5f);
                    last = Vector3.Lerp(second, third, 0.5f);
                }
                if (points.IsPolygonUShaped(out indices))
                {
                    int onePointPrevious = points.GetPreviousControlPoint(indices[0]);
                    int twoPointPrevious = points.GetPreviousControlPoint(onePointPrevious);
                    int threePointPrevious = points.GetPreviousControlPoint(twoPointPrevious);

                    int onePointNext = points.GetNextControlPoint(indices[1]);
                    int twoPointNext = points.GetNextControlPoint(onePointNext);
                    int threePointNext = points.GetNextControlPoint(twoPointNext);

                    start = Vector3.Lerp(points[onePointPrevious], points[twoPointPrevious], 0.5f);
                    second = Vector3.Lerp(points[indices[0]], points[threePointPrevious], 0.5f);
                    third = Vector3.Lerp(points[indices[1]], points[threePointNext], 0.5f);
                    last = Vector3.Lerp(points[onePointNext], points[twoPointNext], 0.5f);
                }
                if(points.IsPolygonSimpleNShaped(out indices))
                {
                    int indices0Previous = points.GetPreviousControlPoint(indices[0]);
                    int indices0Previous1 = points.GetPreviousControlPoint(indices0Previous);
                    int indices0Previous2 = points.GetPreviousControlPoint(indices0Previous1);

                    int indices0Next = points.GetNextControlPoint(indices[0]);
                    int indices1Previous = points.GetPreviousControlPoint(indices[1]);
                    int indices1Previous1 = points.GetPreviousControlPoint(indices1Previous);

                    start = Vector3.Lerp(points[indices0Previous], points[indices0Previous1], 0.5f);
                    second = Vector3.Lerp(points[indices[0]], points[indices0Previous2], 0.5f);
                    third = Vector3.Lerp(points[indices[1]], points[indices0Next], 0.5f);
                    last = Vector3.Lerp(points[indices1Previous], points[indices1Previous1], 0.5f);
                }

                oneLine[0] = start;
                oneLine[1] = second;
                oneLine[2] = third;
                oneLine[3] = last;
                return true;
            case 10:
                if (points.IsPolygonNShaped(out int[] nPointIndices))
                {
                    oneLine = new Vector3[6];
                    int onePointPrevious = points.GetPreviousControlPoint(nPointIndices[0]);
                    int twoPointPrevious = points.GetPreviousControlPoint(onePointPrevious);
                    int threePointPrevious = points.GetPreviousControlPoint(twoPointPrevious);

                    int onePointNext = points.GetNextControlPoint(nPointIndices[1]);

                    oneLine[0] = Vector3.Lerp(points[onePointPrevious], points[twoPointPrevious], 0.5f);
                    oneLine[1] = Vector3.Lerp(points[nPointIndices[0]], points[threePointPrevious], 0.5f);
                    oneLine[2] = Vector3.Lerp(points[nPointIndices[0]], points[onePointNext], 0.5f);

                    onePointPrevious = points.GetPreviousControlPoint(nPointIndices[1]);
                    twoPointPrevious = points.GetPreviousControlPoint(onePointPrevious);
                    threePointPrevious = points.GetPreviousControlPoint(twoPointPrevious);

                    onePointNext = points.GetNextControlPoint(nPointIndices[0]);

                    oneLine[3] = Vector3.Lerp(points[nPointIndices[1]], points[onePointNext], 0.5f);
                    oneLine[4] = Vector3.Lerp(points[nPointIndices[1]], points[threePointPrevious], 0.5f);
                    oneLine[5] = Vector3.Lerp(points[onePointPrevious], points[twoPointPrevious], 0.5f);
                    return true;
                }
                if(points.IsPolygonSimpleMShaped(out int[] simpleMPointIndices))
                {
                    oneLine = new Vector3[5];

                    int onePrevious = points.GetControlPointIndex(simpleMPointIndices[0] - 1);
                    int twoPrevious = points.GetControlPointIndex(simpleMPointIndices[0] - 2);
                    int threePrevious = points.GetControlPointIndex(simpleMPointIndices[0] - 3);

                    int oneNext = points.GetControlPointIndex(simpleMPointIndices[0] + 1);

                    oneLine[0] = Vector3.Lerp(points[onePrevious], points[twoPrevious], 0.5f);
                    oneLine[1] = Vector3.Lerp(points[simpleMPointIndices[0]], points[threePrevious], 0.5f);
                    oneLine[2] = Vector3.Lerp(points[oneNext], points[simpleMPointIndices[1]], 0.5f);

                    onePrevious = points.GetControlPointIndex(simpleMPointIndices[1] - 1);
                    oneNext = points.GetControlPointIndex(simpleMPointIndices[2] + 1);
                    int twoNext = points.GetControlPointIndex(simpleMPointIndices[2] + 2);

                    oneLine[3] = Vector3.Lerp(points[simpleMPointIndices[2]], points[onePrevious], 0.5f);
                    oneLine[4] = Vector3.Lerp(points[oneNext], points[twoNext], 0.5f);
                    return true;
                }


                return false;
            case 12:
                if (points.IsPolygonEShaped(out int[] ePointIndices))
                {
                    oneLine = new Vector3[6];
                    int onePointPrevious = points.GetPreviousControlPoint(ePointIndices[0]);
                    int twoPointPrevious = points.GetPreviousControlPoint(onePointPrevious);
                    int threePointPrevious = points.GetPreviousControlPoint(twoPointPrevious);

                    int onePointNext = points.GetNextControlPoint(ePointIndices[3]);
                    int twoPointNext = points.GetNextControlPoint(onePointNext);
                    int threePointNext = points.GetNextControlPoint(twoPointNext);

                    oneLine[0] = Vector3.Lerp(points[onePointPrevious], points[twoPointPrevious], 0.5f);
                    oneLine[1] = Vector3.Lerp(points[ePointIndices[0]], points[threePointPrevious], 0.5f);
                    oneLine[2] = Vector3.Lerp(points[ePointIndices[3]], points[threePointNext], 0.5f);
                    oneLine[3] = Vector3.Lerp(points[onePointNext], points[twoPointNext], 0.5f);
                    oneLine[4] = Vector3.Lerp(oneLine[1], oneLine[2], 0.5f);

                    onePointPrevious = points.GetPreviousControlPoint(ePointIndices[2]);
                    twoPointPrevious = points.GetPreviousControlPoint(onePointPrevious);

                    oneLine[5] = Vector3.Lerp(points[onePointPrevious], points[twoPointPrevious], 0.5f);
                    return true;
                }

                if (points.IsPolygonXShaped(out int[] xPointIndices))
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
                    return true;
                }
                if (points.IsPolygonMShaped(out int[] mPointIndices))
                {
                    oneLine = new Vector3[7];
                    int onePointPrevious = points.GetPreviousControlPoint(mPointIndices[0]);
                    int twoPointPrevious = points.GetPreviousControlPoint(onePointPrevious);
                    int threePointPrevious = points.GetPreviousControlPoint(twoPointPrevious);
                    int fourPointPrevious = points.GetPreviousControlPoint(threePointPrevious);

                    int onePointNext = points.GetNextControlPoint(mPointIndices[0]);

                    oneLine[0] = Vector3.Lerp(points[onePointPrevious], points[twoPointPrevious], 0.5f);
                    oneLine[1] = Vector3.Lerp(points[mPointIndices[0]], points[threePointPrevious], 0.5f);
                    oneLine[2] = Vector3.Lerp(points[mPointIndices[0]], points[fourPointPrevious], 0.5f);
                    oneLine[3] = Vector3.Lerp(points[mPointIndices[1]], points[onePointNext], 0.5f);

                    onePointPrevious = points.GetPreviousControlPoint(mPointIndices[1]);
                    twoPointPrevious = points.GetPreviousControlPoint(onePointPrevious);
                    threePointPrevious = points.GetPreviousControlPoint(twoPointPrevious);
                    fourPointPrevious = points.GetPreviousControlPoint(threePointPrevious);

                    oneLine[4] = Vector3.Lerp(points[mPointIndices[2]], points[onePointPrevious], 0.5f);
                    oneLine[5] = Vector3.Lerp(points[mPointIndices[2]], points[twoPointPrevious], 0.5f);
                    oneLine[6] = Vector3.Lerp(points[threePointPrevious], points[fourPointPrevious], 0.5f);
                    return true;
                }

                return false;
        }

        return false;
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
