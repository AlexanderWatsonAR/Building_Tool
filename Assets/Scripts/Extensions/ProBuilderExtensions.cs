using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using ProMaths = UnityEngine.ProBuilder.Math;

public static class ProBuilderExtensions
{
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
        Vector3[] positions = proBuilderMesh.positions.ToArray();
        List<Vector3> selectedVerts = new List<Vector3>();

        foreach (int i in indices)
        {
            selectedVerts.Add(positions[i]);
        }

        int[] indicesArray = indices.ToArray();
        for (int i = 0; i < selectedVerts.Count; i++)
        {
            Vector3 point = selectedVerts[i] - transformPoint;
            Vector3 v = Vector3.Scale(point, scale) + transformPoint;
            Vector3 offset = v - selectedVerts[i];
            proBuilderMesh.TranslateVertices(new int[] { indicesArray[i] }, offset);
        }
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
}
