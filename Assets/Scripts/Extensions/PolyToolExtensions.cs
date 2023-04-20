using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using ProMaths = UnityEngine.ProBuilder.Math;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using System;

public static class PolyToolExtensions
{
    public static Vector3 PolygonCentre(this IEnumerable<Vector3> controlPoints)
    {
        return UnityEngine.ProBuilder.Math.Average(controlPoints.ToArray());
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
        return IsPointInsidePolygon(polyTool.ControlPoints, point);
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

    /// <summary>
    /// Does the Polyshape resemble an 'L'? 
    /// </summary>
    /// <param name="polyTool"></param>
    /// <param name="lPointIndex"></param>
    /// <returns></returns>
    public static bool IsLShaped(this Polytool polyTool, out int lPointIndex)
    {
        return IsPolygonLShaped(polyTool.ControlPoints, out lPointIndex);
    }
    /// <summary>
    /// Does the Polygon resemble an 'L'?
    /// </summary>
    /// <param name="controlPoints"></param>
    /// <param name="lPointIndex"></param>
    /// <returns></returns>
    public static bool IsPolygonLShaped(this IEnumerable<Vector3> controlPoints, out int lPointIndex)
    {
        Vector3[] controlPointsArray = controlPoints.ToArray();
        lPointIndex = 0;

        if (controlPointsArray.Length != 6)
            return false;

        Vector3[] nextForward = new Vector3[controlPointsArray.Length];
        Vector3[] previousForward = new Vector3[controlPointsArray.Length];
        Vector3[] inbetweenForward = new Vector3[controlPointsArray.Length];

        int count = 0;
        float angle = 0;

        for (int i = 0; i < controlPointsArray.Length; i++)
        {
            int previousPoint = controlPointsArray.GetPreviousControlPoint(i);
            int nextPoint = controlPointsArray.GetNextControlPoint(i);

            nextForward[i] = Vector3Extensions.GetDirectionToTarget(controlPointsArray[i], controlPointsArray[nextPoint]);
            previousForward[i] = Vector3Extensions.GetDirectionToTarget(controlPointsArray[i], controlPointsArray[previousPoint]);
            inbetweenForward[i] = Vector3.Lerp(nextForward[i], previousForward[i], 0.5f);

            Vector3 a = controlPointsArray[i] + inbetweenForward[i];

            if (!controlPointsArray.IsPointInsidePolygon(a))
            {
                count++;
                angle = Vector3.Angle(nextForward[i], previousForward[i]);
                lPointIndex = i;
            }
        }

        if (count == 1 && angle < 100)
        {
            return true;
        }

        return false;
    }


    /// <summary>
    /// Does the Polyshape resemble a 'T'? 
    /// </summary>
    /// <param name="polyTool"></param>
    /// <param name="tPointsIndex"></param>
    /// <returns></returns>
    public static bool IsTShaped(this Polytool polyTool, out int[] tPointsIndex)
    {
        return IsPolygonTShaped(polyTool.ControlPoints, out tPointsIndex);
    }

    public static bool IsPolygonTShaped(this IEnumerable<Vector3> controlPoints, out int[] tPointsIndex)
    {
        Vector3[] controlPointsArray = controlPoints.ToArray();
        tPointsIndex = new int[2];

        if (controlPointsArray.Length != 8)
            return false;

        Vector3[] nextForward = new Vector3[controlPointsArray.Length];
        Vector3[] previousForward = new Vector3[controlPointsArray.Length];
        Vector3[] inbetweenForward = new Vector3[controlPointsArray.Length];

        List<float> angles = new List<float>();
        List<int> indices = new List<int>();

        for (int i = 0; i < controlPointsArray.Length; i++)
        {
            int previousPoint = controlPointsArray.GetPreviousControlPoint(i);
            int nextPoint = controlPointsArray.GetNextControlPoint(i);

            nextForward[i] = Vector3Extensions.GetDirectionToTarget(controlPointsArray[i], controlPointsArray[nextPoint]);
            previousForward[i] = Vector3Extensions.GetDirectionToTarget(controlPointsArray[i], controlPointsArray[previousPoint]);
            inbetweenForward[i] = Vector3.Lerp(nextForward[i], previousForward[i], 0.5f);

            Vector3 a = controlPointsArray[i] + inbetweenForward[i];

            if (!controlPointsArray.IsPointInsidePolygon(a))
            {
                angles.Add(Vector3.Angle(nextForward[i], previousForward[i]));
                indices.Add(i);
            }
        }

        if (indices.Count != 2)
            return false;

        tPointsIndex[0] = indices[0];
        tPointsIndex[1] = indices[1];

        if (angles.Count == 2)
        {
            if (angles[0] < 100 && angles[1] < 100)
            {
                if (Mathf.Abs(indices[0] - indices[1]) == 3)
                {
                    // trying to work out if points are parallel
                    // get forward vector between b and previous point
                    // get forward vector between c and next point

                    Vector3 b = controlPointsArray[indices[0]];
                    Vector3 c = controlPointsArray[indices[1]];

                    int previousPoint = controlPointsArray.GetPreviousControlPoint(indices[0]);

                    Vector3 backwardsVector = (controlPointsArray[previousPoint] - b).normalized;

                    float distance = Vector3.Distance(b, c);

                    Vector3 d = b + (-backwardsVector * distance);

                    float distanceA = Vector3.Distance(c, d);

                    if (distanceA <= 5)
                    {
                        return true;
                    }

                }
            }
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
        return IsPolygonUShaped(polyTool.ControlPoints, out uPointsIndex);
    }

    public static bool IsPolygonUShaped(this IEnumerable<Vector3> controlPoints, out int[] uPointsIndex)
    {
        uPointsIndex = new int[2];

        Vector3[] controlPointsArray = controlPoints.ToArray();

        if (controlPointsArray.Length != 8)
            return false;

        Vector3[] nextForward = new Vector3[controlPointsArray.Length];
        Vector3[] previousForward = new Vector3[controlPointsArray.Length];
        Vector3[] inbetweenForward = new Vector3[controlPointsArray.Length];

        //List<float> angles = new List<float>();
        List<int> indices = new List<int>();

        for (int i = 0; i < controlPointsArray.Length; i++)
        {
            int previousPoint = controlPointsArray.GetPreviousControlPoint(i);
            int nextPoint = controlPointsArray.GetNextControlPoint(i);

            nextForward[i] = Vector3Extensions.GetDirectionToTarget(controlPointsArray[i], controlPointsArray[nextPoint]);
            previousForward[i] = Vector3Extensions.GetDirectionToTarget(controlPointsArray[i], controlPointsArray[previousPoint]);
            inbetweenForward[i] = Vector3.Lerp(nextForward[i], previousForward[i], 0.5f);

            Vector3 a = controlPointsArray[i] + inbetweenForward[i];

            if (!controlPointsArray.IsPointInsidePolygon(a))
            {
                //float angle = Vector3.Angle(nextForward[i], previousForward[i]);
                //Debug.Log(angle);
                //angles.Add(angle);
                indices.Add(i);
            }
        }

        if (indices.Count != 2)
            return false;

        // We know if it's the u shape if the indices points are next to each other.
        int next = controlPointsArray.GetNextControlPoint(indices[0]);

        if (next == indices[1])
        {
            uPointsIndex[0] = indices[0];
            uPointsIndex[1] = indices[1];
            return true;
        }

        return false;
    }

    public static bool IsEShaped(this Polytool polyTool, out int[] ePointsIndex)
    {
        return IsPolygonEShaped(polyTool.ControlPoints, out ePointsIndex);
    }

    public static bool IsPolygonEShaped(this IEnumerable<Vector3> controlPoints, out int[] ePointsIndices)
    {
        int[] indices = controlPoints.GetConcaveIndexPoints();
        ePointsIndices = new int[4];

        Vector3[] controlPointsArray = controlPoints.ToArray();

        if (controlPointsArray.Length != 12 && indices.Length == ePointsIndices.Length)
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

        return true;
    }

    public static bool IsPlusShaped(this Polytool polyTool, out int[] plusPointIndices)
    {
        return IsPolygonPlusShaped(polyTool.ControlPoints, out plusPointIndices);
    }

    public static bool IsPolygonPlusShaped(this IEnumerable<Vector3> controlPoints, out int[] plusPointIndices)
    {
        int[] indices = GetConcaveIndexPoints(controlPoints);
        plusPointIndices = new int[4];

        Vector3[] controlPointsArray = controlPoints.ToArray();
        
        if (controlPointsArray.Length != 12 | indices.Length != plusPointIndices.Length)
            return false;

        //
        if(Mathf.Abs(indices[0] - indices[1]) == 3 &&
           Mathf.Abs(indices[1] - indices[2]) == 3 &&
           Mathf.Abs(indices[2] - indices[3]) == 3 &&
           Mathf.Abs(indices[3] - indices[0]) == 9)
        {
            plusPointIndices[0] = indices[0];
            plusPointIndices[1] = indices[1];
            plusPointIndices[2] = indices[2];
            plusPointIndices[3] = indices[3];
            return true;
        }

        return false;
        
    }

    public static bool IsNShaped(this Polytool polyTool, out int[] nPointIndices)
    {
        return IsPolygonNShaped(polyTool.ControlPoints, out nPointIndices);
    }

    public static bool IsPolygonNShaped(this IEnumerable<Vector3> controlPoints, out int[] nPointIndices)
    {
        int[] indices = GetConcaveIndexPoints(controlPoints);
        nPointIndices = new int[2];

        Vector3[] controlPointsArray = controlPoints.ToArray();

        if (controlPointsArray.Length != 10 | indices.Length != nPointIndices.Length)
            return false;
//
        nPointIndices = indices;
        return true;
    }

    public static bool IsMShaped(this Polytool polyTool, out int[] mPointIndices)
    {
        return IsPolygonMShaped(polyTool.ControlPoints, out mPointIndices);
    }

    public static bool IsPolygonMShaped(this IEnumerable<Vector3> controlPoints, out int[] mPointIndices)
    {
        int[] indices = GetConcaveIndexPoints(controlPoints);
        mPointIndices = new int[3];

        Vector3[] controlPointsArray = controlPoints.ToArray();

        if (controlPointsArray.Length != 12 | indices.Length != mPointIndices.Length)
            return false;

        // Organise the m points.

        for (int i = 0; i< indices.Length; i++)
        {
            int oneNext = controlPoints.GetNextControlPoint(indices[i]);
            int twoNext = controlPoints.GetNextControlPoint(oneNext);

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
            int oneNext = controlPoints.GetNextControlPoint(indices[i]);
            int twoNext = controlPoints.GetNextControlPoint(oneNext);
            int threeNext = controlPoints.GetNextControlPoint(twoNext);
            int fourNext = controlPoints.GetNextControlPoint(threeNext);
            int fiveNext = controlPoints.GetNextControlPoint(fourNext);

            int onePrevious = controlPoints.GetPreviousControlPoint(indices[i]);
            int twoPrevious = controlPoints.GetPreviousControlPoint(onePrevious);
            int threePrevious = controlPoints.GetPreviousControlPoint(twoPrevious);
            int fourPrevious = controlPoints.GetPreviousControlPoint(threePrevious);
            int fivePrevious = controlPoints.GetPreviousControlPoint(fourPrevious);

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

    private static int[] GetConcaveIndexPoints(this IEnumerable<Vector3> controlPoints)
    {
        Vector3[] controlPointsArray = controlPoints.ToArray();
        Vector3[] nextForward = new Vector3[controlPointsArray.Length];
        Vector3[] previousForward = new Vector3[controlPointsArray.Length];
        Vector3[] inbetweenForward = new Vector3[controlPointsArray.Length];

        List<int> indices = new List<int>();

        for (int i = 0; i < controlPointsArray.Length; i++)
        {
            int previousPoint = controlPointsArray.GetPreviousControlPoint(i);
            int nextPoint = controlPointsArray.GetNextControlPoint(i);

            nextForward[i] = Vector3Extensions.GetDirectionToTarget(controlPointsArray[i], controlPointsArray[nextPoint]);
            previousForward[i] = Vector3Extensions.GetDirectionToTarget(controlPointsArray[i], controlPointsArray[previousPoint]);
            inbetweenForward[i] = Vector3.Lerp(nextForward[i], previousForward[i], 0.5f);

            Vector3 a = controlPointsArray[i] + inbetweenForward[i];

            if (!controlPointsArray.IsPointInsidePolygon(a))
            {
                indices.Add(i);
            }
        }

        return indices.ToArray();
    }

    /// <summary>
    /// Returns an index to the next control point.
    /// </summary>
    /// <param name="polyShape"></param>
    /// <param name="currentIndex"></param>
    /// <returns></returns>
    public static int GetNextPoint(this Polytool polyTool, int index)
    {
        return polyTool.ControlPoints.GetNextControlPoint(index);
    }

    public static int GetNextControlPoint(this IEnumerable<Vector3> controlPoints, int index)
    {
        Vector3[] controlPointsArray = controlPoints.ToArray();

        if (index < 0 || index >= controlPointsArray.Length)
            return -1;

        int next = 1;

        if (index == controlPointsArray.Length - 1)
            next = -(controlPointsArray.Length - 1);

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
        return polyTool.ControlPoints.GetPreviousControlPoint(index);
    }

    public static int GetPreviousControlPoint(this IEnumerable<Vector3> controlPoints, int index)
    {
        Vector3[] controlPointsArray = controlPoints.ToArray();

        if (index < 0 || index >= controlPointsArray.Length)
            return -1;

        int previous = -1;

        if (index == 0)
            previous = controlPointsArray.Length - 1;

        return index + previous;
    }

    public static bool IsConcave(this Polytool polyTool, out int[] concavePoints)
    {
        concavePoints = GetConcaveIndexPoints(polyTool.ControlPoints);

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
        return IsPolygonDescribableInOneLine(polyTool.ControlPoints, out oneLine);
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

                if (points.IsPolygonPlusShaped(out int[] plusPointIndices))
                {
                    oneLine = new Vector3[5];

                    for (int i = 0; i < plusPointIndices.Length; i++)
                    {
                        int onePointNext = points.GetNextControlPoint(plusPointIndices[i]);
                        int twoPointNext = points.GetNextControlPoint(onePointNext);
                        oneLine[i] = Vector3.Lerp(points[onePointNext], points[twoPointNext], 0.5f);
                    }

                    Vector3[] plusPoints = new Vector3[] { points[plusPointIndices[0]],
                        points[plusPointIndices[1]],
                        points[plusPointIndices[2]],
                        points[plusPointIndices[3]] };

                    oneLine[4] = ProMaths.Average(plusPoints);
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
}
