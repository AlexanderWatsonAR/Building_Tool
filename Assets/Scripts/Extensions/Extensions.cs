using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder.Shapes;

public static class Extensions
{
    public static float PolygonLength(this IEnumerable<Vector3> controlPoints)
    {
        float distance = 0;

        Vector3[] points = controlPoints.ToArray();

        for(int i = 0; i < points.Length; i++)
        {
            int next = points.GetNextControlPoint(i);
            distance += Vector3.Distance(points[i], points[next]);
        }
        
        return distance;
    }

    /// <summary>
    /// 2D line intersection on the XZ plane
    /// </summary>
    /// <param name="line1Start"></param>
    /// <param name="line1End"></param>
    /// <param name="line2Start"></param>
    /// <param name="line2End"></param>
    /// <param name="intersection"></param>
    /// <returns></returns>
    public static bool DoLinesIntersect(Vector3 line1Start, Vector3 line1End, Vector3 line2Start, Vector3 line2End, out Vector3 intersection)
    {
        intersection = Vector3.zero;
        float denominator = ((line1End.x - line1Start.x) * (line2End.z - line2Start.z)) - ((line1End.z - line1Start.z) * (line2End.x - line2Start.x));

        if (denominator == 0f)
        {
            // The lines are parallel or coincident, so no intersection point exists
            return false;
        }

        float ua = (((line2End.x - line2Start.x) * (line1Start.z - line2Start.z)) - ((line2End.z - line2Start.z) * (line1Start.x - line2Start.x))) / denominator;
        float ub = (((line1End.x - line1Start.x) * (line1Start.z - line2Start.z)) - ((line1End.y - line1Start.z) * (line1Start.x - line2Start.x))) / denominator;

        if (ua >= 0f && ua <= 1f && ub >= 0f && ub <= 1f)
        {
            // Intersection point lies within both line segments
            float intersectionX = line1Start.x + (ua * (line1End.x - line1Start.x));
            float intersectionZ = line1Start.z + (ua * (line1End.z - line1Start.z));
            intersection = new Vector3(intersectionX, line1Start.y, intersectionZ);
            return true;
        }

        // The intersection point is outside the range of at least one of the line segments
        return false;
    }

    public static void DeleteChildren(this Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (Application.isEditor)
            {
                UnityEngine.Object.DestroyImmediate(child);
            }
            else
            {
                UnityEngine.Object.Destroy(child);
            }

        }

        if (transform.childCount > 0)
        {
            DeleteChildren(transform);
        }
    }

    public static Vector3 CheckAxis(this Axis direction)
    {
        switch (direction)
        {
            case Axis.Z:
                return Vector3.forward;
            case Axis.MinusZ:
                return Vector3.back;
            case Axis.X:
                return Vector3.right;
            case Axis.MinusX:
                return Vector3.left;
            case Axis.Y:
                return Vector3.up;
            case Axis.MinusY:
                return Vector3.down;
        }
        return Vector3.zero;
    }

    // Applies a Bezier curve to an extruding object.
    // returns the positions of the extrusions. Positions are global.
    public static Vector3[] Extrude(this ProBuilderMesh proBuilderMesh, IEnumerable<Face> faces, ExtrudeMethod method, float distance, AnimationCurve curve, Axis curveAxis)
    {
        Vector3 axis = curveAxis.CheckAxis();
        Vector3[] facePositions = new Vector3[2];
        facePositions[0] = proBuilderMesh.transform.TransformPoint(Vector3.zero);
        //Vector2 uvOffset = Vector2.zero;

        //if(proBuilderMesh.GetComponent<MeshFilter>() != null)
        //{
        //    uvOffset = -proBuilderMesh.GetComponent<MeshFilter>().sharedMesh.uv[0];
        //}

        //AutoUnwrapSettings settings = new AutoUnwrapSettings();
        //settings.scale = Vector2.zero;
        //settings.offset = uvOffset;

        if (curve == null)
        {
            proBuilderMesh.Extrude(faces, method, distance);

            foreach (Face f in proBuilderMesh.faces)
            {
                f.uv.Reset();
                f.manualUV = true;
            }

            proBuilderMesh.ToMesh();
            proBuilderMesh.Refresh();

            Vector3[] vertices = proBuilderMesh.VerticesInWorldSpace();
            Vector3 facePos = Vector3.zero;
            for (int i = 0; i < proBuilderMesh.faces[0].distinctIndexes.Count; i++)
            {
                facePos += vertices[proBuilderMesh.faces[0].distinctIndexes[i]];
            }
            facePos /= proBuilderMesh.faces[0].distinctIndexes.Count;
            facePositions[1] = facePos;

            return facePositions;
        }
        if (curve.length <= 1 || axis == Vector3.zero)
        {
            proBuilderMesh.Extrude(faces, method, distance);

            foreach (Face f in proBuilderMesh.faces)
            {
                f.uv.Reset();
                f.manualUV = true;
            }

            proBuilderMesh.ToMesh();
            proBuilderMesh.Refresh();

            Vector3[] vertices = proBuilderMesh.VerticesInWorldSpace();
            Vector3 facePos = Vector3.zero;
            for (int i = 0; i < proBuilderMesh.faces[0].distinctIndexes.Count; i++)
            {
                facePos += vertices[proBuilderMesh.faces[0].distinctIndexes[i]];
            }
            facePos /= proBuilderMesh.faces[0].distinctIndexes.Count;
            facePositions[1] = facePos;

            return facePositions;
        }

        // Faces being extruded follow an animation curve.

        float totalEvaluation = 0;
        float previousEvalution = 0;

        //Vector3 normal = Vector3.forward * distance;
        //normal.Normalize();
        //float unit = normal.z;
        //float extrude = unit;

        int targetNumberOfExtrusions = Mathf.FloorToInt(distance);
        float unit = distance / (float)targetNumberOfExtrusions;

        //if (targetNumberOfExtrusions > 0)
        //{
        //    unit = distance / targetNumberOfExtrusions;
        //}

        List<Vector3> facePosList = new List<Vector3>();
        facePosList.Add(proBuilderMesh.transform.TransformPoint(Vector3.zero));

        //for (float i = 0; i <= distance; i += unit)
        for(int i = 0; i < targetNumberOfExtrusions; i++)
        {
            //if (i + unit >= distance)
            //{
            //    extrude = distance - i;
            //}

            proBuilderMesh.Extrude(faces, method, unit);

            foreach (Face f in proBuilderMesh.faces)
            {
                f.uv.Reset();
                f.manualUV = true;
            }

            proBuilderMesh.ToMesh();
            proBuilderMesh.Refresh();

            float t = i / distance;

            float evalution = curve.Evaluate(t);

            Vector3 offset = Vector3.zero;
            offset -= axis * totalEvaluation;
            offset += axis * evalution;

            proBuilderMesh.TranslateVertices(faces.ToArray()[0].distinctIndexes, offset);

            totalEvaluation += evalution - previousEvalution;

            previousEvalution = evalution;

            Vector3[] vertices = proBuilderMesh.VerticesInWorldSpace();
            Vector3 facePos = Vector3.zero;

            for (int j = 0; j < proBuilderMesh.faces[0].distinctIndexes.Count; j++)
            {
                facePos += vertices[proBuilderMesh.faces[0].distinctIndexes[j]];
            }

            facePos /= proBuilderMesh.faces[0].distinctIndexes.Count;

            facePosList.Add(facePos);
        }
        return facePosList.ToArray();
    }

    public static Vector3[] Extrude(this ProBuilderMesh proBuilderMesh, IEnumerable<Face> faces, ExtrudeMethod method, float distance, AnimationCurve curve, Axis curveAxis, int targetNumberOfExtrusions)
    {
        Vector3 axis = curveAxis.CheckAxis();
        Vector3[] facePositions = new Vector3[2];
        facePositions[0] = proBuilderMesh.transform.TransformPoint(Vector3.zero);

        if (curve == null)
        {
            proBuilderMesh.Extrude(faces, method, distance);

            foreach (Face f in proBuilderMesh.faces)
            {
                f.uv.Reset();
                f.manualUV = true;
            }

            proBuilderMesh.ToMesh();
            proBuilderMesh.Refresh();

            Vector3[] vertices = proBuilderMesh.VerticesInWorldSpace();
            Vector3 facePos = Vector3.zero;
            for (int i = 0; i < proBuilderMesh.faces[0].distinctIndexes.Count; i++)
            {
                facePos += vertices[proBuilderMesh.faces[0].distinctIndexes[i]];
            }
            facePos /= proBuilderMesh.faces[0].distinctIndexes.Count;
            facePositions[1] = facePos;

            return facePositions;
        }
        if (curve.length <= 1 || axis == Vector3.zero)
        {
            proBuilderMesh.Extrude(faces, method, distance);

            foreach (Face f in proBuilderMesh.faces)
            {
                f.uv.Reset();
                f.manualUV = true;
            }

            proBuilderMesh.ToMesh();
            proBuilderMesh.Refresh();

            Vector3[] vertices = proBuilderMesh.VerticesInWorldSpace();
            Vector3 facePos = Vector3.zero;
            for (int i = 0; i < proBuilderMesh.faces[0].distinctIndexes.Count; i++)
            {
                facePos += vertices[proBuilderMesh.faces[0].distinctIndexes[i]];
            }
            facePos /= proBuilderMesh.faces[0].distinctIndexes.Count;
            facePositions[1] = facePos;

            return facePositions;
        }

        // Faces being extruded follow an animation curve.

        float totalEvaluation = 0;
        float previousEvalution = 0;

        float unit = 0;

        if (targetNumberOfExtrusions > 0)
        {
            unit = distance / (float)targetNumberOfExtrusions;
        }

        List<Vector3> facePosList = new List<Vector3>();
        facePosList.Add(proBuilderMesh.transform.TransformPoint(Vector3.zero));

        int count = 2;

        List<Face> topFaces = new List<Face>();


        // Doesn't follow the curve correctly.
        // Is there a way to curve after extrusion?
        for (int i = 0; i < targetNumberOfExtrusions; i++)
        {
            proBuilderMesh.Extrude(faces, method, unit);

            foreach (Face f in proBuilderMesh.faces)
            {
                f.uv.Reset();
                f.manualUV = true;
            }

            topFaces.Add(proBuilderMesh.faces[count]);

            count += 4;

            proBuilderMesh.ToMesh();
            proBuilderMesh.Refresh();

            float t = i / (float)targetNumberOfExtrusions;

            float evalution = curve.Evaluate(t);

            Vector3 offset = Vector3.zero;
            offset -= axis * totalEvaluation;
            offset += axis * evalution;

            proBuilderMesh.TranslateVertices(faces.ToArray()[0].distinctIndexes, offset);

            totalEvaluation += evalution - previousEvalution;

            previousEvalution = evalution;

            Vector3[] vertices = proBuilderMesh.VerticesInWorldSpace();
            Vector3 facePos = Vector3.zero;

            // Calc Face Centre.
            for (int j = 0; j < proBuilderMesh.faces[0].distinctIndexes.Count; j++)
            {
                facePos += vertices[proBuilderMesh.faces[0].distinctIndexes[j]];
            }
            facePos /= proBuilderMesh.faces[0].distinctIndexes.Count;

            facePosList.Add(facePos);
        }

        // Creates a bevel block chain.
        //foreach(Face f in topFaces)
        //{
        //    proBuilderMesh.Extrude(new Face[] {f}, ExtrudeMethod.FaceNormal, 0.25f);

        //    Bevel.BevelEdges(proBuilderMesh, f.edges, 0.25f);

        //    proBuilderMesh.ToMesh();
        //    proBuilderMesh.Refresh();
        //}


        //Debug.Log(count);

        return facePosList.ToArray();
    }

    /// <summary>
    /// Returns indices for each point of extrusion.
    /// </summary>
    /// <param name="proBuilderMesh"></param>
    /// <param name="faces"></param>
    /// <param name="method"></param>
    /// <param name="distance"></param>
    /// <param name="targetNumberOfExtrusions"></param>
    /// <returns></returns>
    public static List<int[]> Extrude(this ProBuilderMesh proBuilderMesh, IEnumerable<Face> faces, ExtrudeMethod method, float distance, int targetNumberOfExtrusions)
    {
        targetNumberOfExtrusions = targetNumberOfExtrusions <= 0 ? Mathf.FloorToInt(distance) : targetNumberOfExtrusions;

        float unit = distance / (float)targetNumberOfExtrusions;

        //List<Face> hypeFace = new List<Face>();
        List<int[]> indices = new List<int[]>();

        for (int i = 0; i < targetNumberOfExtrusions; i++)
        {
            Face[] theFaces = proBuilderMesh.Extrude(faces, method, unit);

            List<UnityEngine.ProBuilder.Edge> edges = new List<UnityEngine.ProBuilder.Edge>();

            foreach (Face face in theFaces)
            {
                edges.Add(face.edges.ToArray()[0]); // Magic
            }

            proBuilderMesh.SetSelectedEdges(edges);

            indices.Add(proBuilderMesh.selectedVertices.ToArray());

            proBuilderMesh.ToMesh();
            proBuilderMesh.Refresh();
        }

        return indices;
    }

    public static void NotchEdge(this ProBuilderMesh proBuilderMesh, UnityEngine.ProBuilder.Edge edge, float size)
    {
        IList<UnityEngine.ProBuilder.Edge> edgeList = new List<UnityEngine.ProBuilder.Edge>();
        edgeList.Add(edge);

        //Face bevelFace = Bevel.BevelEdges(proBuilderMesh, edgeList, size)[0];

        Vector3[] positions = proBuilderMesh.positions.ToArray();

        Vector3 a = positions[edge.a];
        Vector3 b = positions[edge.b];
        Vector3 dir = a.DirectionToTarget(b);

        Vector3 scale = Vector3.zero;

        //proBuilderMesh.ScaleVertices(bevelFace.indexes, )

        //bevelFace.indexes

        throw new NotImplementedException();
    }

    /// <summary>
    /// Colours the mesh by single coordinate on the trimsheet.
    /// </summary>
    public static void SetMeshColour(this ProBuilderMesh proBuilderMesh, Vector2 offset)
    {
        AutoUnwrapSettings settings = new AutoUnwrapSettings();
        settings.scale = Vector2.zero;
        settings.offset = offset;

        foreach (Face f in proBuilderMesh.faces)
        {
            f.manualUV = false;
            f.uv = settings;
        }

        proBuilderMesh.ToMesh();
        proBuilderMesh.Refresh();
    }

    /// <summary>
    /// Calculates where the transform point is in relation to the vertices.
    /// </summary>
    /// <param name="transformPoint"></param>
    /// <param name="vertices"></param>
    /// <returns></returns>
    public static Vector3 PointToVector3(this TransformPoint transformPoint, IEnumerable<Vector3> vertices)
    {
        Vector3 pivotPoint = Vector3.zero;

        Vector3[] distinctVerts = vertices.Distinct().ToArray();

        // ToDO: Organise verts like a polygon.

        //Vector3[] controlPoints = Extensions.CreatePolygon(distinctVerts);
        Vector3 faceNormal = Extensions.CalculatePolygonFaceNormal(distinctVerts);
        Vector3 planeAxis = Vector3.Cross(faceNormal, Vector3.up);

        if (planeAxis == Vector3.zero)
        {
            planeAxis = Vector3.Cross(faceNormal, Vector3.right);
        }

        List<Vector3> leftVertices = new List<Vector3>();
        List<Vector3> rightVertices = new List<Vector3>();
        List<Vector3> topVertices = new List<Vector3>();
        List<Vector3> bottomVertices = new List<Vector3>();

        Vector3 centroid = UnityEngine.ProBuilder.Math.Average(distinctVerts);

        // Find the vertices that are to the left, right, top & bottom of the plane
        for (int i = 0; i < distinctVerts.Length; i++)
        {
            Vector3 direction = distinctVerts[i] - centroid;
            float product = Vector3.Dot(distinctVerts[i] - centroid, planeAxis);

            if (product < 0)
            {
                leftVertices.Add(distinctVerts[i]);
            }
            else if (product > 0)
            {
                rightVertices.Add(distinctVerts[i]);
            }

            if(direction.y > 0)
            {
                topVertices.Add(distinctVerts[i]);
            }
            else if (direction.y < 0)
            {
                bottomVertices.Add(distinctVerts[i]);
            }

        }

        switch (transformPoint)
        {
            case TransformPoint.Top:
                pivotPoint = UnityEngine.ProBuilder.Math.Average(topVertices);
                break;
            case TransformPoint.Left:
                pivotPoint = UnityEngine.ProBuilder.Math.Average(leftVertices);
                break;
            case TransformPoint.Right:
                pivotPoint = UnityEngine.ProBuilder.Math.Average(rightVertices);
                break;
            case TransformPoint.Middle:
                pivotPoint = centroid;
                break;
            case TransformPoint.Bottom:
                pivotPoint = UnityEngine.ProBuilder.Math.Average(bottomVertices);
                break;
        }

        return pivotPoint;
    }
    /// <summary>
    /// Transforms positions from local space to world space.
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="positions"></param>
    /// <returns></returns>
    public static IEnumerable<Vector3> TransformPoints(this Transform transform, IEnumerable<Vector3> positions)
    {
        List<Vector3> globalPoints = new List<Vector3>();

        foreach(Vector3 point in positions)
        {
            globalPoints.Add(transform.TransformPoint(point));
        }

        return globalPoints;
    }
    /// <summary>
    /// Transforms positions from world space to local space
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="positions"></param>
    /// <returns></returns>
    public static IEnumerable<Vector3> InverseTransformPoints(this Transform transform, IEnumerable<Vector3> positions)
    {
        List<Vector3> localPoints = new List<Vector3>();

        foreach (Vector3 point in positions)
        {
            localPoints.Add(transform.InverseTransformPoint(point));
        }

        return localPoints;
    }

    /// <summary>
    /// Assumes polygon is convex. This is for a face with more than 3 points.
    /// </summary>
    /// <param name="vertices"></param>
    /// <returns></returns>
    public static Vector3 CalculatePolygonFaceNormal(this IEnumerable<Vector3> vertices)
    {
        Vector3[] verts = vertices.ToArray();
        Vector3 faceNormal = Vector3.zero;
        Vector3 normal = Vector3.zero;

        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 current = verts[i];
            Vector3 next = verts[(i + 1) % verts.Length];

            normal.x += (current.y - next.y) * (current.z + next.z);
            normal.y += (current.z - next.z) * (current.x + next.x);
            normal.z += (current.x - next.x) * (current.y + next.y);
        }

        faceNormal = normal.normalized;
        return faceNormal;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="vertices"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static Vector3 CalculateTriangleNormal(this IEnumerable<Vector3> vertices)
    { 
        Vector3[] verticesArray = vertices.ToArray();

        if (verticesArray.Length < 3)
            throw new Exception("Vertices count is not 3");

        Vector3 A = verticesArray[0];
        Vector3 B = verticesArray[1];
        Vector3 C = verticesArray[2];

        Vector3 AB = B - A;
        Vector3 AC = C - A;

        Vector3 triangleNormal = Vector3.Cross(AB, AC).normalized;
        return triangleNormal;
    }
    /// <summary>
    /// This orders the vertices like a polygon. With the points moving clockwise.
    /// </summary>
    /// <param name="vertices"></param>
    /// <returns></returns>
    public static Vector3[] CreatePolygon(this IEnumerable<Vector3> vertices)
    {
        Vector3 normal = CalculateTriangleNormal(vertices);

        if (Extensions.IsPolygonClockwise(vertices, normal))
        {
            return vertices.ToArray();
        }

        List<Vector3> verts = vertices.ToList();
        verts.Add(verts[0]);

        Vector3 centroid = UnityEngine.ProBuilder.Math.Average(verts);

        Vector3[] orderedVerts = new Vector3[verts.Count];
        Array.Copy(verts.ToArray(), orderedVerts, verts.Count);

        // Sort the vertices in clockwise order
        // Assumes polygon is convex & closed.
        Array.Sort(orderedVerts, (p1, p2) =>
        {
            return (Vector3.SignedAngle(normal, p1 - centroid, Vector3.up) - Vector3.SignedAngle(normal, p2 - centroid, Vector3.up)).CompareTo(0);
        });

        if (!Extensions.IsPolygonClockwise(vertices, normal))
        {
            Debug.Log("Polygon is not clockwise");
        }

        return orderedVerts;
    }

    /// <summary>
    /// Assumes polygon is aligned to the 'Z' plane.
    /// points need to be organised either clockwise or anti-clockwise for this to work.
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    public static bool IsPolygonClockwise(IEnumerable<Vector3> points)
    {
        Vector3[] pointsArray = points.ToArray();

        float sum = 0;
        for (int i = 0; i < pointsArray.Length; i++)
        {
            Vector3 current = pointsArray[i];
            Vector3 next = pointsArray[(i + 1) % pointsArray.Length];
            sum += (next.x - current.x) * (next.y + current.y);
        }
        return sum > 0;
    }

    /// <summary>
    /// This is used when you don't know what plane the aligned to.
    /// </summary>
    /// <param name="points"></param>
    /// <param name="normal"></param>
    /// <returns></returns>
    public static bool IsPolygonClockwise(IEnumerable<Vector3> points, Vector3 normal)
    {
        Vector3[] pointsArray = points.ToArray();
        Vector3 up = Vector3.up;
        float sum = 0;

        for (int i = 0; i < pointsArray.Length; i++)
        {
            Vector3 current = pointsArray[i];
            Vector3 next = pointsArray[(i + 1) % pointsArray.Length];
            Vector3 edge = next - current;
            Vector3 cross = Vector3.Cross(edge, normal);
            float dot = Vector3.Dot(cross, up);
            sum += dot * edge.magnitude;
        }
        return sum > 0;
    }

    public static Vector3[][] Reverse(this Vector3[][] arrayOfArrays)
    {
        for (int i = 0; i < arrayOfArrays.Length; i++)
        {
            arrayOfArrays[i] = arrayOfArrays[i].Reverse().ToArray();
        }
        return arrayOfArrays;
    }
}
