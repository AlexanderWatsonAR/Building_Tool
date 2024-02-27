using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using ProMaths = UnityEngine.ProBuilder.Math;
using Edge = UnityEngine.ProBuilder.Edge;
using System.Diagnostics;

public static class ProBuilderExtensions
{
    /// <summary>
    /// Sometimes the generated promesh has a normal that points in the opposite direction to what is expected.
    /// This checks the normal & flips it if it is incorrect.
    /// Note: This will only work for a polygon.
    /// </summary>
    /// <param name="proBuilderMesh"></param>
    /// <param name="normal"></param>
    public static void MatchFaceToNormal(this ProBuilderMesh proBuilderMesh, Vector3 normal)
    {
        Vector3 aveNormal = ProMaths.Average(proBuilderMesh.GetNormals());

        if (!Vector3Extensions.Approximately(aveNormal, normal, 0.1f))
        {
            proBuilderMesh.faces[0].Reverse();
        }
    }

    /// <summary>
    /// Gives a polygon depth
    /// </summary>
    /// <param name="polygon"></param>
    /// <param name="extrude"></param>
    public static void Solidify(this ProBuilderMesh polygon, float extrude)
    {
        ProBuilderMesh lid = UnityEngine.Object.Instantiate(polygon);
        lid.faces[0].Reverse();
        polygon.Extrude(polygon.faces, ExtrudeMethod.FaceNormal, extrude);
        CombineMeshes.Combine(new ProBuilderMesh[] { polygon, lid }, polygon);
        UnityEngine.Object.DestroyImmediate(lid.gameObject);
        polygon.ToMesh();
        polygon.Refresh();
    }

    public static ActionResult CreateShapeFromPolygon(this ProBuilderMesh proBuilderMesh, IEnumerable<ControlPoint> controlPoints, float extrude, bool flipNormals)
    {
        return proBuilderMesh.CreateShapeFromPolygon(controlPoints.GetPositions(), extrude, flipNormals);
    }

    public static ActionResult CreateShapeFromPolygon(this ProBuilderMesh proBuilderMesh, IList<Vector3> points, Vector3 normal, IList<IList<Vector3>> holePoints = null)
    {
        if (proBuilderMesh == null)
            throw new NullReferenceException();

        ActionResult result = ActionResult.NoSelection;

        if (holePoints != null)
        {
            result = proBuilderMesh.CreateShapeFromPolygon(points, 0, false, holePoints);
        }
        else
        {
            result = proBuilderMesh.CreateShapeFromPolygon(points, 0, false);
        }

        proBuilderMesh.ToMesh();
        proBuilderMesh.Refresh();
        proBuilderMesh.MatchFaceToNormal(normal);
        proBuilderMesh.ToMesh();
        proBuilderMesh.Refresh();

        return result;
    }

    public static IEnumerable<int> GetCoincidentVerticesFromPosition(this ProBuilderMesh proBuilderMesh, Vector3 position, float marginForError = 0.001f)
    {
        Vertex[] vertices = proBuilderMesh.GetVertices();

        int firstIndex = 0;

        for(int i = 0; i < vertices.Length; i++)
        {
            if (Vector3Extensions.Approximately(position, vertices[i].position, marginForError))
            {
                firstIndex = i;
                break;
            }

            if (i == vertices.Length - 1)
                throw new Exception("Position: " + position + " not found.");
        }

        List<int> shared = proBuilderMesh.GetCoincidentVertices(new int[] { firstIndex });

        return shared;
    }

    public static IList<int> GetAllDistinctIndices(this ProBuilderMesh proBuilderMesh)
    {
        List<int> distinctIndices = new();

        foreach(Face f in proBuilderMesh.faces)
        {
            distinctIndices.AddRange(f.distinctIndexes);
        }

        return distinctIndices;
    }

    public static void SetPosition(this ProBuilderMesh proBuilderMesh, int index, Vector3 position)
    {
        List<int> shared = proBuilderMesh.GetCoincidentVertices(new int[] { index });

        if (shared.Count <= 0)
            return;

        Vertex[] vertices = proBuilderMesh.GetVertices();

        for(int i = 0; i < shared.Count; i++)
        {
            vertices[shared[i]].position = position;
        }

        proBuilderMesh.SetVertices(vertices);
        proBuilderMesh.ToMesh();
    }

    /// <summary>
    /// Assumes Pivot location is at the centre.
    /// </summary>
    /// <param name="proBuilderMesh"></param>
    public static void LocaliseVertices(this ProBuilderMesh proBuilderMesh, Vector3? transformPoint = null)
    {
        Transform t = proBuilderMesh.transform;
        Vertex[] vertices = proBuilderMesh.GetVertices();

        if(transformPoint == null)
            transformPoint = ProMaths.Average(proBuilderMesh.positions);

        for (int i = 0; i < vertices.Length; i++)
        {
            // Scale
            Vector3 point = vertices[i].position - transformPoint.Value;
            Vector3 v = Vector3.Scale(point, t.localScale) + transformPoint.Value;
            Vector3 offset = v - vertices[i].position;
            vertices[i].position += offset;

            // Rotation
            Vector3 localEulerAngles = t.localEulerAngles;
            Vector3 v1 = Quaternion.Euler(localEulerAngles) * (vertices[i].position - transformPoint.Value) + transformPoint.Value;
            offset = v1 - vertices[i].position;
            vertices[i].position += offset;

            // Position
            vertices[i].position += t.localPosition;
        }

        proBuilderMesh.SetVertices(vertices);
        proBuilderMesh.ToMesh();

        proBuilderMesh.transform.localPosition = Vector3.zero;
        proBuilderMesh.transform.localEulerAngles = Vector3.zero;
        proBuilderMesh.transform.localScale = Vector3.one;
    }

    public static Vector3[] FaceToVertices(this ProBuilderMesh proBuilderMesh, Face face)
    {
        Vector3[] positions = proBuilderMesh.positions.ToArray();
        Vector3[] verts = new Vector3[face.distinctIndexes.Count];

        for(int i = 0; i < face.distinctIndexes.Count; i++)
        {
            verts[i] = positions[face.distinctIndexes[i]];
        }

        return verts;
    }

    public static void ScaleVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<Face> faces, Vector3 transformPoint, float scale)
    {
        ScaleVertices(proBuilderMesh, faces, transformPoint, Vector3.one * scale);
    }

    public static void ScaleVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<Face> faces, Vector3 transformPoint, Vector3 scale)
    {
        List<int> indices = new List<int>();

        foreach (Face face in faces)
        {
            indices.AddRange(face.distinctIndexes);
        }

        ScaleVertices(proBuilderMesh, indices, transformPoint, scale);
    }

    public static void ScaleVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<Face> faces, TransformPoint transformPoint, float scale)
    {
        ScaleVertices(proBuilderMesh, faces, transformPoint, Vector3.one * scale);
    }

    public static void ScaleVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<Face> faces, TransformPoint transformPoint, Vector3 scale)
    {
        List<int> indices = new List<int>();

        foreach (Face face in faces)
        {
            indices.AddRange(face.distinctIndexes);
        }

        ScaleVertices(proBuilderMesh, indices, transformPoint,  scale);
    }

    public static void ScaleVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<Edge> edges, Vector3 transformPoint, float scale)
    {
        ScaleVertices(proBuilderMesh, edges, transformPoint, Vector3.one * scale);
    }

    public static void ScaleVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<Edge> edges, Vector3 transformPoint, Vector3 scale)
    {
        List<int> indices = new List<int>();

        foreach (Edge edge in edges)
        {
            indices.Add(edge.a);
            indices.Add(edge.b);
        }

        indices = (List<int>) indices.Distinct();

        ScaleVertices(proBuilderMesh, indices, transformPoint, scale);
    }

    public static void ScaleVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<Edge> edges, TransformPoint transformPoint, float scale)
    {
        ScaleVertices(proBuilderMesh, edges, transformPoint, Vector3.one * scale);
    }

    public static void ScaleVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<Edge> edges, TransformPoint transformPoint, Vector3 scale)
    {
        List<int> indices = new List<int>();

        foreach (Edge edge in edges)
        {
            indices.Add(edge.a);
            indices.Add(edge.b);
        }

        indices = (List<int>)indices.Distinct();

        ScaleVertices(proBuilderMesh, indices, transformPoint, scale);
    }

    public static void ScaleVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<int> indices, TransformPoint transformPoint, float scale)
    {
        ScaleVertices(proBuilderMesh, indices, transformPoint, Vector3.one * scale);
    }

    public static void ScaleVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<int> indices, Vector3 transformPoint, float scale)
    {
        ScaleVertices(proBuilderMesh, indices, transformPoint, Vector3.one * scale);
    }
    public static void ScaleVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<int> indices, Vector3 transformPoint, Vector3 scale)
    {
        Vertex[] points = proBuilderMesh.GetVertices();
        List<Vector3> selectedVerts = new List<Vector3>();

        foreach (int i in indices)
        {
            selectedVerts.Add(points[i].position);
        }
        
        int[] indicesArray = indices.ToArray();
        for (int i = 0; i < selectedVerts.Count; i++)
        {
            Vector3 point = selectedVerts[i] - transformPoint;
            Vector3 v = Vector3.Scale(point, scale) + transformPoint;
            points[indicesArray[i]].position = v;

            List<int> shared = proBuilderMesh.GetCoincidentVertices(new int[] { indicesArray[i] });

            for (int j = 0; j < shared.Count; j++)
            {
                points[shared[j]].position = v;
            }
        }

        proBuilderMesh.SetVertices(points);
        proBuilderMesh.ToMesh();
    }

    public static void ScaleVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<int> indices, TransformPoint transformPoint, Vector3 scale)
    {
        Vector3[] positions = proBuilderMesh.positions.ToArray();
        List<Vector3> selectedVerts = new();

        foreach (int i in indices)
        {
            selectedVerts.Add(positions[i]);
        }

        Vector3 scalePoint = transformPoint.PointToVector3(selectedVerts);

        ScaleVertices(proBuilderMesh, indices, scalePoint, scale);
        
    }
    public static void ScaleFace(this ProBuilderMesh proBuilderMesh, Face face, float scaleFactor)
    {
        int[] indices = face.distinctIndexes.ToArray();

        Vector3[] positions = proBuilderMesh.positions.ToArray();
        List<Vector3> selectedVerts = new();

        foreach (int i in indices)
        {
            selectedVerts.Add(positions[i]);
        }

        Vector3 facePosition = proBuilderMesh.FaceCentre(face);
        Vector3 faceNormal = proBuilderMesh.FaceNormal(face);

        for (int i = 0; i < selectedVerts.Count; i++)
        {
            Vector3 vertex = selectedVerts[i];
            float distanceToFace = Vector3.Dot(vertex - facePosition, faceNormal);
            float scale = scaleFactor + distanceToFace;
            Vector3 scaledVertex = facePosition + (vertex - facePosition) * scale;
            Vector3 offset = scaledVertex - vertex;
            proBuilderMesh.TranslateVertices(new int[] { indices[i] }, offset);
        }



    }
    public static void RotateVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<Edge> edges, Vector3 transformPoint, Quaternion rotation)
    {
        RotateVertices(proBuilderMesh, edges, transformPoint, rotation.eulerAngles);
    }

    public static void RotateVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<Edge> edges, Vector3 transformPoint, Vector3 eulerAngles)
    {
        List<int> indices = new List<int>();

        foreach (Edge edge in edges)
        {
            indices.Add(edge.a);
            indices.Add(edge.b);
        }

        indices = (List<int>)indices.Distinct();

        RotateVertices(proBuilderMesh, indices, transformPoint, eulerAngles);
    }

    public static void RotateVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<Edge> edges, TransformPoint transformPoint, Quaternion rotation)
    {
        RotateVertices(proBuilderMesh, edges, transformPoint, rotation.eulerAngles);
    }

    public static void RotateVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<Edge> edges, TransformPoint transformPoint, Vector3 eulerAngles)
    {
        List<int> indices = new List<int>();

        foreach (Edge edge in edges)
        {
            indices.Add(edge.a);
            indices.Add(edge.b);
        }

        indices = (List<int>)indices.Distinct();

        RotateVertices(proBuilderMesh, indices, transformPoint, eulerAngles);
    }

    public static void RotateVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<Face> faces, Vector3 transformPoint, Quaternion rotation)
    {
        RotateVertices(proBuilderMesh, faces, transformPoint, rotation.eulerAngles);
    }

    public static void RotateVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<Face> faces, Vector3 transformPoint, Vector3 eulerAngles)
    {
        List<int> indices = new List<int>();

        foreach (Face face in faces)
        {
            indices.AddRange(face.distinctIndexes);
        }

        RotateVertices(proBuilderMesh, indices, transformPoint, eulerAngles);
    }

    public static void RotateVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<Face> faces, TransformPoint transformPoint, Quaternion rotation)
    {
        RotateVertices(proBuilderMesh, faces, transformPoint, rotation.eulerAngles);
    }

    public static void RotateVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<Face> faces, TransformPoint transformPoint, Vector3 eulerAngles)
    {
        List<int> indices = new List<int>();

        foreach (Face face in faces)
        {
            indices.AddRange(face.distinctIndexes);
        }

        RotateVertices(proBuilderMesh, indices, transformPoint, eulerAngles);
    }

    public static void RotateVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<int> distinctIndices, TransformPoint transformPoint, Vector3 eulerAngles)
    {
        Vector3[] positions = proBuilderMesh.positions.ToArray();
        List<Vector3> selectedVerts = new List<Vector3>();

        foreach (int i in distinctIndices)
        {
            selectedVerts.Add(positions[i]);
        }

        Vector3 rotatePoint = transformPoint.PointToVector3(selectedVerts);

        RotateVertices(proBuilderMesh, distinctIndices, rotatePoint, eulerAngles);
    }

    public static void RotateVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<int> distinctIndices, TransformPoint transformPoint, Quaternion rotation)
    {
        RotateVertices(proBuilderMesh, distinctIndices, transformPoint, rotation.eulerAngles);
    }
    public static void RotateVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<int> distinctIndices, Vector3 transformPoint, Quaternion rotation)
    {
        RotateVertices(proBuilderMesh, distinctIndices, transformPoint, rotation.eulerAngles);
    }
    public static void RotateVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<int> distinctIndices, Vector3 transformPoint, Vector3 eulerAngles)
    {
        Vertex[] points = proBuilderMesh.GetVertices();
        List<Vector3> selectedVerts = new List<Vector3>();

        foreach (int i in distinctIndices)
        {
            selectedVerts.Add(points[i].position);
        }

        int[] indicesArray = distinctIndices.ToArray();
        for (int i = 0; i < selectedVerts.Count; i++)
        {
            Vector3 v = Quaternion.Euler(eulerAngles) * (selectedVerts[i] - transformPoint) + transformPoint;
            List<int> shared = proBuilderMesh.GetCoincidentVertices(new int[] { indicesArray[i] });

            for (int j = 0; j < shared.Count; j++)
            {
                points[shared[j]].position = v;
            }
        }

        proBuilderMesh.SetVertices(points);
        proBuilderMesh.ToMesh();

    }
    /// <summary>
    /// 0 = top left. 1 = top right. 2 = bottom right. 3 = bottom left
    /// </summary>
    /// <param name="face"></param>
    /// <returns></returns>
    public static int[] CornerPositions(this ProBuilderMesh proBuilderMesh, Face face)
    {
        if(face.distinctIndexes.Count < 4)
            return new int[0];

        Vertex[] positions = proBuilderMesh.GetVertices();

        Vector3 first = positions[face.distinctIndexes[0]].position;

        float minY = first.y;
        float maxY = minY;

        for(int i = 0; i < face.distinctIndexes.Count; i++)
        {
            Vector3 point = positions[face.distinctIndexes[i]].position;
            minY = point.y < minY ? point.y : minY;
            maxY = point.y > maxY ? point.y : maxY;
        }

        //Vector3 rightDirection = Vector3.Cross(normal, Vector3.up);
        //Vector3 leftDirection = -rightDirection;
        //Vector3 upDirection = Vector3.up;
        //Vector3 downDirection = Vector3.down;

        Vector3 centre = proBuilderMesh.FaceCentre(face);

        List<int> unorderedCorners = new List<int>();
        float delta = 0.02f;

        for (int i = 0; i < face.distinctIndexes.Count; i++)
        {
            Vertex point = positions[face.distinctIndexes[i]];

            float y = point.position.y;

            if (Mathf.Abs(y - minY) <= delta || Mathf.Abs(y - maxY) <= delta)
            {
                unorderedCorners.Add(face.distinctIndexes[i]);
            }
        }

        return unorderedCorners.ToArray();

        //for(int i = 0; i < unorderedCorners.Count; i++)
        //{
        //    Vertex point = positions[unorderedCorners[i]];

        //    if (point.position.y > centre.y)
        //    {
        //        topCorners.Add(unorderedCorners[i]);
        //    }
        //    else
        //    {
        //        bottomCorners.Add(unorderedCorners[i]);
        //    }
        //}



        //Vector3 topLeft = positions[topCorners[0]].position;
        //Vector3 topRight = positions[topCorners[1]].position;

        //float distance = Vector3.Distance(topLeft, topRight);

        //if (topLeft + rightDirection * distance != topRight)
        //{
        //    topLeft = topRight;
        //    topCorners[0] = topCorners[1];
        //}

        //Vector3 bottomLeft = positions[bottomCorners[0]].position;
        //Vector3 bottomRight = positions[bottomCorners[1]].position;

        //distance = Vector3.Distance(bottomLeft, bottomRight);

        //if (bottomLeft + rightDirection * distance != bottomRight)
        //{
        //    bottomLeft = bottomRight;
        //    bottomCorners[0] = bottomCorners[1];
        //}

    }

    public static Vector3 FaceCentre(this ProBuilderMesh proBuilderMesh, Face face)
    {
        Vector3[] positions = proBuilderMesh.positions.ToArray();
        int[] indices = face.distinctIndexes.ToArray();
        List<Vector3> facePoints = new List<Vector3>();

        for(int i = 0; i < indices.Length; i++)
        {
            facePoints.Add(positions[indices[i]]);
        }

        Vector3 faceCentre = ProMaths.Average(facePoints);

        return faceCentre;
    }

    public static Vector3 FaceNormal(this ProBuilderMesh proBuilderMesh, Face face)
    {
        Vector3[] normals = proBuilderMesh.normals.ToArray();
        int[] indices = face.distinctIndexes.ToArray();
        List<Vector3> facePoints = new List<Vector3>();

        for (int i = 0; i < indices.Length; i++)
        {
            facePoints.Add(normals[indices[i]]);
        }

        Vector3 faceCentre = ProMaths.Average(facePoints);

        return faceCentre;
    }

    public static Vector3[] Positions(this Vertex[] vertices)
    {
        Vector3[] positions = new Vector3[vertices.Length];

        for(int i = 0; i < vertices.Length; i++)
        {
            positions[i] = vertices[i].position;
        }
        return positions;
    }

    public static Vector3[] Normals(this Vertex[] vertices)
    {
        Vector3[] normals = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            normals[i] = vertices[i].normal;
        }
        return normals;
    }

    public static Vector3 CalculateNormal(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // Calculate the cross product of two edges of the surface to find the normal vector
        Vector3 edge1 = p1 - p0;
        Vector3 edge2 = p2 - p0;
        Vector3 normal = Vector3.Cross(edge1, edge2).normalized;

        // Return the normal vector
        return normal;
    }
}
