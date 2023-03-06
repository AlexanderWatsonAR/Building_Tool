using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

public static class PolyShapeExtensions
{
    /// <summary>
    /// Do the control points move clockwise?
    /// </summary>
    /// <param name="shape"></param>
    /// <returns></returns>
    public static bool IsClockwise(this PolyShape shape)
    {
        return IsClockwise(shape.controlPoints);
    }
    /// <summary>
    /// Do the control points move clockwise?
    /// </summary>
    /// <param name="polygonControlPoints"></param>
    /// <returns></returns>
    public static bool IsClockwise(IEnumerable<Vector3> polygonControlPoints)
    {
        Vector3[] points = polygonControlPoints.ToArray();
        float temp = 0;
        bool isClockwise = false;

        for (int i = 0; i < points.Length; i++)
        {
            if (i != points.Length - 1)
            {
                float mulA = points[i].x * points[i + 1].z;
                float mulB = points[i + 1].x * points[i].z;
                temp = temp + (mulA - mulB);
            }
            else
            {
                float mulA = points[i].x * points[i].z;
                float mulB = points[0].x * points[i].z;
                temp = temp + (mulA - mulB);
            }
        }
        temp /= 2;

        isClockwise = temp < 0 ? false : true;

        return isClockwise;
    }

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
    public static bool IsPointInside(this PolyShape polyShape, Vector3 point)
    {
        return IsPointInsidePolygon(polyShape.controlPoints, point);
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
    /// <param name="polyShape"></param>
    /// <param name="lPointIndex"></param>
    /// <returns></returns>
    public static bool IsLShaped(this PolyShape polyShape, out int lPointIndex)
    {
        return IsPolygonLShaped(polyShape.controlPoints, out lPointIndex);
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
    /// <param name="polyShape"></param>
    /// <param name="tPointsIndex"></param>
    /// <returns></returns>
    public static bool IsTShaped(this PolyShape polyShape, out int[] tPointsIndex)
    {
        return IsPolygonTShaped(polyShape.controlPoints, out tPointsIndex);
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
    /// <param name="polyShape"></param>
    /// <param name="uPointsIndex"></param>
    /// <returns></returns>
    public static bool IsUShaped(this PolyShape polyShape, out int[] uPointsIndex)
    {
        return IsPolygonUShaped(polyShape.controlPoints, out uPointsIndex);
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

        int next = controlPointsArray.GetNextControlPoint(indices[0]);

        if (next == indices[1])
        {
            uPointsIndex[0] = indices[0];
            uPointsIndex[1] = indices[1];
            return true;
        }

        return false;
    }

    /// <summary>
    /// Returns an index to the next control point.
    /// </summary>
    /// <param name="polyShape"></param>
    /// <param name="currentIndex"></param>
    /// <returns></returns>
    public static int GetNextPoint(this PolyShape polyShape, int index)
    {
        return polyShape.controlPoints.GetNextControlPoint(index);
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
    /// <param name="polyShape"></param>
    /// <param name="currentIndex"></param>
    /// <returns></returns>
    public static int GetPreviousPoint(this PolyShape polyShape, int index)
    {
        return polyShape.controlPoints.GetPreviousControlPoint(index);
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

    /// <summary>
    /// If true, out returns a 1D representation of the polyshape.
    /// </summary>
    /// <param name="polyShape"></param>
    /// <param name="oneLine"></param>
    /// <returns></returns>
    public static bool IsDescribableInOneLine(this PolyShape polyShape, out Vector3[] oneLine)
    {
        return IsPolygonDescribableInOneLine(polyShape.controlPoints, out oneLine);
    }

    public static bool IsPolygonDescribableInOneLine(this IEnumerable<Vector3> controlPoints, out Vector3[] oneLine)
    {
        Vector3[] controlPointsArray = controlPoints.ToArray();
        oneLine = new Vector3[0];

        if (controlPointsArray.Length % 2 != 0)
            return false;

        switch (controlPointsArray.Length)
        {
            case 4:
                oneLine = new Vector3[2];
                Vector3 a = Vector3.Lerp(controlPointsArray[0], controlPointsArray[1], 0.5f);
                Vector3 b = Vector3.Lerp(controlPointsArray[2], controlPointsArray[3], 0.5f);
                oneLine[0] = a;
                oneLine[1] = b;

                return true;
            case 6:
                oneLine = new Vector3[3];
                int index;

                if (controlPointsArray.IsPolygonLShaped(out index))
                {
                    Vector3 c, d, e;

                    int onePointNext = controlPointsArray.GetNextControlPoint(index);
                    int twoPointNext = controlPointsArray.GetNextControlPoint(onePointNext);
                    int threePointNext = controlPointsArray.GetNextControlPoint(twoPointNext);

                    int onePointPrevious = controlPointsArray.GetPreviousControlPoint(index);
                    int twoPointPrevious = controlPointsArray.GetPreviousControlPoint(onePointPrevious);

                    c = Vector3.Lerp(controlPointsArray[onePointNext], controlPointsArray[twoPointNext], 0.5f);
                    d = Vector3.Lerp(controlPointsArray[index], controlPointsArray[threePointNext], 0.5f);
                    e = Vector3.Lerp(controlPointsArray[onePointPrevious], controlPointsArray[twoPointPrevious], 0.5f);

                    oneLine[0] = c;
                    oneLine[1] = d;
                    oneLine[2] = e;
                    return true;
                }
                break;
            case 8:
                oneLine = new Vector3[4];
                int[] indices;

                Vector3 start = Vector3.zero, second = Vector3.zero, third = Vector3.zero, last = Vector3.zero;

                if (controlPointsArray.IsPolygonTShaped(out indices))
                {
                    int onePointNext = controlPointsArray.GetNextControlPoint(indices[0]);
                    int twoPointNext = controlPointsArray.GetNextControlPoint(onePointNext);

                    int onePointPrevious = controlPointsArray.GetPreviousControlPoint(indices[0]);
                    int twoPointPrevious = controlPointsArray.GetPreviousControlPoint(onePointPrevious);

                    start = Vector3.Lerp(controlPointsArray[onePointNext], controlPointsArray[twoPointNext], 0.5f);
                    second = Vector3.Lerp(controlPointsArray[onePointPrevious], controlPointsArray[twoPointPrevious], 0.5f);

                    onePointNext = controlPointsArray.GetNextControlPoint(indices[1]);
                    twoPointNext = controlPointsArray.GetNextControlPoint(onePointNext);

                    third = Vector3.Lerp(controlPointsArray[onePointNext], controlPointsArray[twoPointNext], 0.5f);
                    last = Vector3.Lerp(second, third, 0.5f);
                }
                if (controlPointsArray.IsPolygonUShaped(out indices))
                {
                    int onePointPrevious = controlPointsArray.GetPreviousControlPoint(indices[0]);
                    int twoPointPrevious = controlPointsArray.GetPreviousControlPoint(onePointPrevious);
                    int threePointPrevious = controlPointsArray.GetPreviousControlPoint(twoPointPrevious);

                    int onePointNext = controlPointsArray.GetNextControlPoint(indices[1]);
                    int twoPointNext = controlPointsArray.GetNextControlPoint(onePointNext);
                    int threePointNext = controlPointsArray.GetNextControlPoint(twoPointNext);

                    start = Vector3.Lerp(controlPointsArray[onePointPrevious], controlPointsArray[twoPointPrevious], 0.5f);
                    second = Vector3.Lerp(controlPointsArray[indices[0]], controlPointsArray[threePointPrevious], 0.5f);
                    third = Vector3.Lerp(controlPointsArray[indices[1]], controlPointsArray[threePointNext], 0.5f);
                    last = Vector3.Lerp(controlPointsArray[onePointNext], controlPointsArray[twoPointNext], 0.5f);
                }

                oneLine[0] = start;
                oneLine[1] = second;
                oneLine[2] = third;
                oneLine[3] = last;
                return true;
        }

        return false;
    }
}
