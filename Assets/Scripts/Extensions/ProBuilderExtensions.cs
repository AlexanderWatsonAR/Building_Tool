using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using ProMaths = UnityEngine.ProBuilder.Math;
using Edge = UnityEngine.ProBuilder.Edge;

public static class ProBuilderExtensions
{
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
        proBuilderMesh.Refresh();

        proBuilderMesh.transform.localPosition = Vector3.zero;
        proBuilderMesh.transform.localEulerAngles = Vector3.zero;
        proBuilderMesh.transform.localScale = Vector3.one;
    }

    public static Vector3[] GetDistinctVerts(this ProBuilderMesh proBuilderMesh, Face face)
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
        proBuilderMesh.Refresh();
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

    public static void RotateVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<int> indices, TransformPoint transformPoint, Vector3 eulerAngles)
    {
        Vector3[] positions = proBuilderMesh.positions.ToArray();
        List<Vector3> selectedVerts = new List<Vector3>();

        foreach (int i in indices)
        {
            selectedVerts.Add(positions[i]);
        }

        Vector3 rotatePoint = transformPoint.PointToVector3(selectedVerts);

        RotateVertices(proBuilderMesh, indices, rotatePoint, eulerAngles);
    }

    public static void RotateVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<int> indices, TransformPoint transformPoint, Quaternion rotation)
    {
        RotateVertices(proBuilderMesh, indices, transformPoint, rotation.eulerAngles);
    }
    public static void RotateVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<int> indices, Vector3 transformPoint, Quaternion rotation)
    {
        RotateVertices(proBuilderMesh, indices, transformPoint, rotation.eulerAngles);
    }
    public static void RotateVertices(this ProBuilderMesh proBuilderMesh, IEnumerable<int> indices, Vector3 transformPoint, Vector3 eulerAngles)
    {
        Vector3[] positions = proBuilderMesh.positions.ToArray();
        List<Vector3> selectedVerts = new List<Vector3>();

        foreach (int i in indices)
        {
            selectedVerts.Add(positions[i]);
        }

        int[] indicesArray = indices.ToArray();
        for (int i = 0; i < selectedVerts.Count; i++)
        {
            Vector3 v = Quaternion.Euler(eulerAngles) * (selectedVerts[i] - transformPoint) + transformPoint;
            Vector3 offset = v - selectedVerts[i];
            proBuilderMesh.TranslateVertices(new int[] { indicesArray[i] }, offset);
        }
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
