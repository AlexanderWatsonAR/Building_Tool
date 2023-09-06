using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using static UnityEngine.EventSystems.EventTrigger;
using ProMaths = UnityEngine.ProBuilder.Math;

public static class MeshMaker
{
    private static readonly int[] m_QuadTriangles = new int[]
    {
        0, 1, 3, 3, 1, 2
    };

    private static readonly int[] m_CubeTriangles = new int[]
    {
         0, 4, 1, 1, 4, 5, // Front
         2, 6, 7, 7, 3, 2, // Back
         4, 7, 5, 5, 7, 6, // Top
         0, 3, 7, 7, 4, 0, // Left
         1, 5, 2, 2, 5, 6, // Right
         0, 2, 3, 1, 2, 0  // Bottom
    };

    private static readonly int[] m_ProjectedCubeTriangles = new int[]
    {
         5, 4, 0, 0, 1, 5, // Front
         2, 6, 7, 7, 3, 2, // Back
         4, 5, 7, 7, 6, 4, // Top
         3, 7, 5, 5, 1, 3, // Left
         0, 4, 6, 6, 2, 0, // Right
         1, 0, 2, 2, 3, 1  // Bottom
    };

    private static readonly Face[] m_CubeFaces = new Face[]
    {
        new Face(new int[] { 0, 4, 1, 1, 4, 5 }), // Front
        new Face(new int[] { 2, 6, 7, 7, 3, 2 }), // Back
        new Face(new int[] { 4, 7, 5, 5, 7, 6 }), // Top
        new Face(new int[] { 0, 3, 7, 7, 4, 0 }), // Left
        new Face(new int[] { 1, 5, 2, 2, 5, 6 }), // Right
        new Face(new int[] { 0, 2, 3, 1, 2, 0 })  // Bottom
    };

    public static ProBuilderMesh Cube(IEnumerable<Vector3> controlPoints, float height, bool flipFace = false)
    {
        // Control Points: 0 = Bottom Left, 1 = Top Left, 2 = Top Right, 3 = Bottom Right

        Vector3[] points = controlPoints.ToArray();
        if (controlPoints.ToArray().Length != 4)
            return null;

        Vector3 dir = points[0].DirectionToTarget(points[3]);

        if (points[0] == points[3])
        {
            dir = points[1].DirectionToTarget(points[2]);
        }

        Vector3 forward = Vector3.Cross(Vector3.up, dir) * height;

        Vector3[] vertices = new Vector3[8];

        // Bottom Points
        vertices[0] = points[0] + forward;
        vertices[1] = points[3] + forward;
        vertices[2] = points[3];
        vertices[3] = points[0];

        // Top Points
        vertices[4] = points[1] + forward;
        vertices[5] = points[2] + forward;
        vertices[6] = points[2];
        vertices[7] = points[1];

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = m_CubeTriangles;

        GameObject cube = new GameObject();
        cube.AddComponent<MeshFilter>().sharedMesh = mesh;

        new MeshImporter(cube).Import();

        return cube.GetComponent<ProBuilderMesh>();
    }

    public static ProBuilderMesh CubeProjection(Vector3 start, Vector3 end, Vector3 size)
    {
        Vector3 forward = start.DirectionToTarget(end);
        Vector3 right = Vector3.Cross(Vector3.up, forward) * size.x;

        Vector3[] vertices = new Vector3[8];

        // Bottom Points
        vertices[0] = start + right;
        vertices[1] = end + right;
        vertices[2] = end - right;
        vertices[3] = start - right;
        // Top Points
        vertices[4] = vertices[0] + (Vector3.up * size.y);
        vertices[5] = new Vector3(vertices[1].x + (forward.x * size.y), vertices[1].y, vertices[1].z + (forward.z * size.y));
        vertices[6] = new Vector3(vertices[2].x + (forward.x * size.y), vertices[2].y, vertices[2].z + (forward.z * size.y));
        vertices[7] = vertices[3] + (Vector3.up * size.y);

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = m_ProjectedCubeTriangles;

        GameObject cubeProjection = new GameObject();
        cubeProjection.AddComponent<MeshFilter>().sharedMesh = mesh;

        new MeshImporter(cubeProjection).Import();

        return cubeProjection.GetComponent<ProBuilderMesh>();

    }

    public static ProBuilderMesh CubeProjection(IEnumerable<Vector3> controlPoints, float height, bool isInside = true)
    {
        // Control Points: 0 = Bottom Left, 1 = Top Left, 2 = Top Right, 3 = Bottom Right

        Vector3[] points = controlPoints.ToArray();
        Vector3[] vertices = new Vector3[8];
        Vector3 up = Vector3.up * height;

        Vector3 scale = new Vector3(1, 0, 1);

        Vector3 dirA = points[1].DirectionToTarget(points[0]);
        Vector3 a = dirA * height;
        a.Scale(scale);
        Vector3 a1 = points[0].DirectionToTarget(points[0] + a) * height;

        Vector3 dirB = points[2].DirectionToTarget(points[3]);
        Vector3 b = dirB * height;
        b.Scale(scale);
        Vector3 b1 = points[3].DirectionToTarget(points[3] + b) * height;

        // Bottom Points
        vertices[0] = points[0] + a1;
        vertices[1] = points[3] + b1;
        vertices[2] = points[3];
        vertices[3] = points[0];

        if (!isInside) // projects the new vertices inside instead of out.
        {
            // Bottom Points
            vertices[0] = points[0];
            vertices[1] = points[3];
            vertices[2] = points[3] - b1;
            vertices[3] = points[0] - a1;
        }

        // Top Points
        vertices[4] = points[1] + up;
        vertices[5] = points[2] + up;
        vertices[6] = points[2];
        vertices[7] = points[1];

        int[] tris = m_CubeTriangles.Clone() as int[];

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = tris;

        GameObject cubeProjection = new GameObject();
        cubeProjection.AddComponent<MeshFilter>().sharedMesh = mesh;

        new MeshImporter(cubeProjection).Import();

        ProBuilderMesh projection = cubeProjection.GetComponent<ProBuilderMesh>();
        projection.ToMesh();
        projection.Refresh();

        return projection;
    }

    public static Vector3[] ProjectedCubeVertices(IEnumerable<Vector3> controlPoints, float extrude, bool isInside = true)
    {
        // Control Points: 0 = Bottom Left, 1 = Top Left, 2 = Top Right, 3 = Bottom Right

        Vector3[] points = controlPoints.ToArray();

        Vector3 scale = new Vector3(1, 0, 1);

        Vector3 dirA = points[1].DirectionToTarget(points[0]);
        Vector3 a = dirA * extrude;
        a.Scale(scale);
        Vector3 a1 = points[0].DirectionToTarget(points[0] + a) * extrude;

        Vector3 dirB = points[2].DirectionToTarget(points[3]);
        Vector3 b = dirB * extrude;
        b.Scale(scale);
        Vector3 b1 = points[3].DirectionToTarget(points[3] + b) * extrude;


        points[0] = points[0] + a1;
        points[1] = points[1] + Vector3.up * extrude;
        points[2] = points[2] + Vector3.up * extrude;
        points[3] = points[3] + b1;

        return points;
    }

    public static ProBuilderMesh RoofTileMesh(IEnumerable<Vector3> controlPoints, /*int columns, int rows, int sides,*/ float extrude, bool flipFace = false)
    {
        Vector3[] points = controlPoints.ToArray();


        // Calculate the bottom verts.
        Vector3 scale = new Vector3(1, 0, 1);

        Vector3 dirA = points[1].DirectionToTarget(points[0]);
        Vector3 a = dirA * extrude;
        a.Scale(scale);
        Vector3 a1 = points[0].DirectionToTarget(points[0] + a) * extrude;

        Vector3 dirB = points[2].DirectionToTarget(points[3]);
        Vector3 b = dirB * extrude;
        b.Scale(scale);
        Vector3 b1 = points[3].DirectionToTarget(points[3] + b) * extrude;

        Vector3 c = Vector3.Lerp(points[0], points[1], 0.5f);
        Vector3 d = Vector3.Lerp(points[0] + a1, points[1] + (Vector3.up * extrude), 0.5f);

        float distance = Vector3.Distance(c, d);

        if (points[1] == points[2])
        {
            points = new Vector3[] { points[0], points[1], points[3] };
        }

        ProBuilderMesh mesh = ProBuilderMesh.Create();
        mesh.CreateShapeFromPolygon(points, distance, false);
        mesh.ToMesh();


        Vector3[] topPoints = new Vector3[points.Length];
        Array.Copy(points, topPoints, points.Length);

        Vector3 normal = mesh.GetVertices(mesh.faces[0].distinctIndexes)[0].normal;

        for (int i = 0; i < topPoints.Length; i++)
        {
            topPoints[i] += normal * distance;
        }

        List<List<int>> sharedIndices = new (topPoints.Length);

        Vertex[] vertices = mesh.GetVertices();
        
        for(int i = 0; i < topPoints.Length; i++)
        {
            sharedIndices.Add(new List<int>());

            for (int j = 0; j < vertices.Length; j++)
            {
                if (Vector3Extensions.Approximately(topPoints[i], vertices[j].position, 0.001f))
                {
                    sharedIndices[i].Add(j);
                }
            }
        }

        for(int i = 0; i < sharedIndices[0].Count; i++)
        {
            vertices[sharedIndices[0][i]].position = points[0] + a1;
        }

        for (int i = 0; i < sharedIndices[1].Count; i++)
        {
            vertices[sharedIndices[1][i]].position = points[1] + (Vector3.up * extrude);
        }

        if(points.Length == 3)
        {
            for (int i = 0; i < sharedIndices[2].Count; i++)
            {
                vertices[sharedIndices[2][i]].position = points[2] + b1;
            }
        }
        else
        {
            for (int i = 0; i < sharedIndices[2].Count; i++)
            {
                vertices[sharedIndices[2][i]].position = points[2] + (Vector3.up * extrude);
            }

            for (int i = 0; i < sharedIndices[3].Count; i++)
            {
                vertices[sharedIndices[3][i]].position = points[3] + b1;
            }
        }

        mesh.SetVertices(vertices);
        mesh.ToMesh();

        mesh.Refresh();
        return mesh;
    }

    public static ProBuilderMesh Quad(IEnumerable<Vector3> controlPoints, bool flipFace = false)
    {
        if (controlPoints.ToArray().Length != 4)
            return null;

        int[] tris = m_QuadTriangles.Clone() as int[];

        if (flipFace)
            tris = tris.Reverse().ToArray();

        ProBuilderMesh quad = ProBuilderMesh.Create(controlPoints, new Face[] { new Face(tris) });
        quad.ToMesh();
        quad.Refresh();
        return quad;
    }
    // Remove?
    public static ProBuilderMesh ProjectedCubeGrid(IEnumerable<Vector3> controlPoints, float height, float width, int numberOfPCubes)
    {
        Vector3[] points = controlPoints.ToArray();

        Vector3 forwardA = points[0].DirectionToTarget(points[1]);
        Vector3 forwardB = points[2].DirectionToTarget(points[3]);
        Vector3 cross = Vector3.Cross(Vector3.up, forwardB);

        // Bottom Points
        Vector3[] a = Vector3Extensions.LerpCollection(points[0], points[1], numberOfPCubes);
        Vector3[] b = Vector3Extensions.LerpCollection(points[0] + (forwardA * width), points[1] + (forwardA * width), numberOfPCubes);
        Vector3[] c = Vector3Extensions.LerpCollection(points[2], points[3], numberOfPCubes);
        Vector3[] d = Vector3Extensions.LerpCollection(points[2] + (forwardB * width), points[3] + (forwardB * width), numberOfPCubes);

        // Top Points
        Vector3[] e = Vector3Extensions.LerpCollection(points[0] + (Vector3.up * height), points[1] + (Vector3.up * height), numberOfPCubes);
        Vector3[] f = Vector3Extensions.LerpCollection(points[0] + (Vector3.up * height) + (forwardA * width), points[1] + (forwardA * width) + (Vector3.up * height), numberOfPCubes);
        Vector3[] g = Vector3Extensions.LerpCollection(points[2] + (cross * height), points[3] + (cross * height), numberOfPCubes);
        Vector3[] h = Vector3Extensions.LerpCollection(points[2] + (forwardB * width) + (cross * height), points[3] + (forwardB * width) + (cross * height), numberOfPCubes);

        List<Vector3> positions = new List<Vector3>();
        List<int> triangles = new List<int>();
        int vertCount = 0;

        for (int i = 0; i < numberOfPCubes; i++)
        {
            positions.Add(a[i]);
            positions.Add(b[i]);
            positions.Add(c[i]);
            positions.Add(d[i]);
            positions.Add(e[i]);
            positions.Add(f[i]);
            positions.Add(g[i]);
            positions.Add(h[i]);

            int[] tris = m_ProjectedCubeTriangles.Clone() as int[];

            for (int j = 0; j < tris.Length; j++)
            {
                tris[j] += vertCount;
            }

            triangles.AddRange(tris);

            vertCount += 8;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = positions.ToArray();
        mesh.triangles = triangles.ToArray();

        GameObject projectedCube = new GameObject();
        projectedCube.AddComponent<MeshFilter>().sharedMesh = mesh;
        projectedCube.AddComponent<MeshRenderer>();

        new MeshImporter(projectedCube).Import();

        ProBuilderMesh proMesh = projectedCube.GetComponent<ProBuilderMesh>();
        proMesh.ToMesh();
        proMesh.Refresh();

        return proMesh;

    }
    // Remove?
    public static ProBuilderMesh[] MultiCubes(IEnumerable<Vector3> topPoints, IEnumerable<Vector3> bottomPoints, int numberOfCubes, float width, bool flipFace = false)
    {
        Vector3[] top = topPoints.ToArray();
        Vector3[] bottom = bottomPoints.ToArray();

        Vector3[] topA = Vector3Extensions.LerpCollection(top[0], top[1], numberOfCubes);
        Vector3[] topB = Vector3Extensions.LerpCollection(top[2], top[3], numberOfCubes);

        Vector3[] bottomA = Vector3Extensions.LerpCollection(bottom[0], bottom[3], numberOfCubes);

        Vector3 dir = bottom[0].DirectionToTarget(bottom[3]);
        Vector3 cross = Vector3.Cross(Vector3.up, dir) * width;

        Vector3[] bottomB = Vector3Extensions.LerpCollection(bottom[0] + cross, bottom[3] + cross, numberOfCubes);

        ProBuilderMesh[] cubes = new ProBuilderMesh[numberOfCubes];

        for (int i = 0; i < numberOfCubes; i++)
        {
            cubes[i] = Cube(new Vector3[] { topA[i], topB[i], bottomA[i], bottomB[i] }, width, flipFace);
        }

        return cubes;
    }

    /// <summary>
    /// Control points should be 4 points. Ordered as following, 0 = bottom left, 1 = top left, 2 = top right, 3 = bottom right.
    /// </summary>
    /// <param name="controlPoints"></param>
    /// <param name="columns"></param>
    /// <param name="rows"></param>
    /// <returns></returns>
    public static List<Vector3[]> CreateGridFromControlPoints(IEnumerable<Vector3> controlPoints, int columns, int rows)
    {
        Vector3[] points = controlPoints.ToArray();

        List<Vector3[]> controlPointsGrid = new List<Vector3[]>();

        int pointsWide = columns + 1;
        int pointsHigh = rows + 1;

        Vector3[] leftPoints = Vector3Extensions.LerpCollection(points[0], points[1], pointsHigh).ToArray(); // row start points
        Vector3[] rightPoints = Vector3Extensions.LerpCollection(points[3], points[2], pointsHigh).ToArray(); // row end points

        for (int i = 0; i < leftPoints.Length; i++)
        {
            controlPointsGrid.Add(Vector3Extensions.LerpCollection(leftPoints[i], rightPoints[i], pointsWide));
        }

        return controlPointsGrid;
    }

    /// <summary>
    /// Changes control points to a rectangular shape.
    /// Assumes points are ordered.
    /// </summary>
    /// <param name="controlPoints"></param>
    /// <returns></returns>
    private static Vector3[] RectanglePoints(IEnumerable<Vector3> controlPoints)
    {
        if (controlPoints.Count() != 4)
            return new Vector3[0];

        Vector3[] points = controlPoints.ToArray();
        Vector3 centre = ProMaths.Average(points);

        //Vector3 faceNormal = points.CalculatePolygonFaceNormal();

        Vector3 up = Vector3.Lerp(points[0], points[3], 0.5f).DirectionToTarget(Vector3.Lerp(points[1], points[2], 0.5f));
        Vector3 faceNormal = Vector3.Cross(up, points[0].DirectionToTarget(points[3]));

        Quaternion rotation = Quaternion.FromToRotation(-faceNormal, Vector3.forward);
        
        for(int i = 0; i < points.Length; i++)
        {
            // Rotate to align with XY plane
            Vector3 euler = rotation.eulerAngles;
            Vector3 v = Quaternion.Euler(euler) * (points[i] - centre) + centre;
            points[i] = v;
        }

        float minY = points[0].y <= points[3].y ? points[0].y : points[3].y;

        Vector3 bottomLeft = points[0].x < points[1].x ? new Vector3(points[1].x, points[0].y, points[0].z) : points[0];
        Vector3 topLeft = points[1].x < points[0].x ? new Vector3(points[0].x, points[1].y, points[1].z) : points[1];
        Vector3 topRight = points[2].x < points[3].x ? new Vector3(points[3].x, points[2].y, points[2].z) : points[2];
        Vector3 bottomRight = points[3].x < points[2].x ? new Vector3(points[2].x, points[3].y, points[3].z) : points[3];

        Vector3[] rectangle = new Vector3[] {bottomLeft, topLeft, topRight, bottomRight };
        Quaternion cpRotation = Quaternion.FromToRotation(Vector3.forward, -faceNormal);

        for (int i = 0; i < rectangle.Length; i++)
        {
            // Rotate to align to control points
            Vector3 euler = cpRotation.eulerAngles;
            Vector3 v = Quaternion.Euler(euler) * (rectangle[i] - centre) + centre;
            rectangle[i] = v;
        }


        return rectangle;
    }

    private static Vector3[] RectanglePoints1(IEnumerable<Vector3> controlPoints)
    {
        if (controlPoints.Count() != 4)
            return new Vector3[0];

        Vector3[] points = controlPoints.ToArray();

        float lengthA = Vector3.Distance(points[0], points[3]); // bottom line;
        float lengthB = Vector3.Distance(points[1], points[2]); // top line;


        return new Vector3[0];
    }

    public static ProBuilderMesh ArchedDoorHoleGrid(IEnumerable<Vector3> controlPoints, float width, int columns, int rows, float height, float archHeight, int archSides, out List<List<Vector3>> holeVertices, Vector3? positionOffset = null, bool flipFace = false)
    {
        Vector3[] points = controlPoints.ToArray();

        holeVertices = new List<List<Vector3>>();

        if (points.Length != 4)
            return null;

        if (width == 0 || height == 0 || columns == 0 || rows == 0)
        {
            return Quad(controlPoints, flipFace);
        }

        List<Vector3[]> controlPointsGrid = CreateGridFromControlPoints(points, columns, rows);

        IList<IList<Vector3>> holePoints = new List<IList<Vector3>>();

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Vector3 bl = controlPointsGrid[j][i];
                Vector3 tl = controlPointsGrid[j + 1][i];
                Vector3 tr = controlPointsGrid[j + 1][i + 1];
                Vector3 br = controlPointsGrid[j][i + 1];

                Vector3[] quadVerts = new Vector3[] { bl, tl, tr, br };
                Vector3 up = bl.DirectionToTarget(tl);
                Vector3 right = bl.DirectionToTarget(br);
                Vector3 faceNormal = Vector3.Cross(up, right);

                Vector3 position = ProMaths.Average(quadVerts);

                Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, -faceNormal);

                float actualWidth = Vector3.Distance(bl, br);
                float actualHeight = Vector3.Distance(bl, tl);
                float hWidth = actualWidth * 0.5f;
                float hHeight = actualHeight * 0.5f;
                float qHeight = hHeight * 0.5f;

                if (positionOffset.HasValue)
                    position += hWidth * positionOffset.Value.x * right;

                Vector3 scalePosition = position + Vector3.up * (hHeight * -0.999f);
                Vector3 scale = new Vector3(width, height);

                Vector3 archHighPoint = new Vector3(0, Mathf.Lerp(qHeight, hHeight, archHeight));

                List<Vector3> holeVerts = new();

                //float y = Mathf.Lerp(hHeight, qHeight, archHeight);

                holeVerts.Add(new Vector3(-hWidth, -hHeight));
                holeVerts.AddRange(Vector3Extensions.QuadraticLerpCollection(new Vector3(-hWidth, qHeight), archHighPoint, new Vector3(hWidth, qHeight), archSides + 1));
                holeVerts.Add(new Vector3(hWidth, -hHeight));


                for (int k = 0; k < holeVerts.Count; k++)
                {
                    // Position
                    holeVerts[k] += position;

                    // Scale 
                    Vector3 point = holeVerts[k] - scalePosition;
                    Vector3 v2 = Vector3.Scale(point, scale) + scalePosition;
                    holeVerts[k] = v2;

                    // Rotate to align with control points
                    Vector3 euler = rotation.eulerAngles;
                    Vector3 v = Quaternion.Euler(euler) * (holeVerts[k] - position) + position;
                    holeVerts[k] = v;
                }

                for (int k = 0; k < quadVerts.Length; k++)
                {
                    // Scale quad
                    Vector3 point = quadVerts[k] - position;
                    Vector3 v = Vector3.Scale(point, Vector3.one * 0.999f) + position;
                    quadVerts[k] = v;
                }

                holeVerts = ConstrainPolygonToQuad(quadVerts, holeVerts.ToArray()).ToList();
                holePoints.Add(holeVerts.ToList());
            }
        }

        ProBuilderMesh mesh = ProBuilderMesh.Create();

        Vector3 normal = Vector3.Cross(points[0].DirectionToTarget(points[1]), points[0].DirectionToTarget(points[3]));

        if (flipFace)
            normal = -normal;

        mesh.CreateShapeFromPolygon(controlPoints.ToList(), 0, flipFace, holePoints);
        mesh.ToMesh();
        mesh.Refresh();

        Vector3[] normals = mesh.GetNormals();

        // The promesh normals should be the same (or roughly the same) as the calculated normal.
        if (!Vector3Extensions.Approximately(normals[0], normal, 0.1f))
        {
            mesh.faces[0].Reverse();
        }

        //Smoothing.ApplySmoothingGroups(mesh, faces, 360 / sides);
        mesh.ToMesh();
        mesh.Refresh();

        foreach (IList<Vector3> hole in holePoints)
        {
            holeVertices.Add(hole.ToList());
        }

        return mesh;
    }

    public static ProBuilderMesh NPolyHoleGrid(IEnumerable<Vector3> controlPoints, Vector3 scale, int columns, int rows, int sides, float angle, out List<List<Vector3>> holeVertices, bool flipFace = false)
    {
        return NPolyHoleGrid(controlPoints, scale, columns, rows, sides, angle, out holeVertices, null, null, flipFace);
    }

    public static ProBuilderMesh NPolyHoleGrid(IEnumerable<Vector3> controlPoints, Vector3 scale, int columns, int rows, int sides, float angle, out List<List<Vector3>> holeVertices, Vector3? positionOffset = null, Vector3? scalePointOffset = null, bool flipFace = false)
    {
        //Vector3[] points = controlPoints.ToArray();

        Vector3[] points = RectanglePoints(controlPoints);
        holeVertices = new List<List<Vector3>>();

        if (points.Length != 4)
            return null;

        if (scale == Vector3.zero || columns == 0 || rows == 0)
        {
            return Quad(controlPoints, flipFace);
        }

        List<Vector3[]> controlPointsGrid = CreateGridFromControlPoints(points, columns, rows);

        IList<IList<Vector3>> holePoints = new List<IList<Vector3>>();

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Vector3 bl = controlPointsGrid[j][i];
                Vector3 tl = controlPointsGrid[j + 1][i];
                Vector3 tr = controlPointsGrid[j + 1][i + 1];
                Vector3 br = controlPointsGrid[j][i + 1];

                Vector3[] quadVerts = new Vector3[] { bl, tl, tr, br };
                Vector3 up = bl.DirectionToTarget(tl);
                Vector3 right = bl.DirectionToTarget(br);
                Vector3 faceNormal = Vector3.Cross(up, right);

                Vector3 position = ProMaths.Average(quadVerts);

                Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, -faceNormal);

                float width = Vector3.Distance(bl, br);
                float height = Vector3.Distance(bl, tl);
                float hWidth = width * 0.5f;
                float hHeight = height * 0.5f;

                if (positionOffset.HasValue)
                    position += hWidth * positionOffset.Value.x * right;

                Vector3 scalePosition = position;

                if(scalePointOffset.HasValue)
                    scalePosition += new Vector3(hWidth * scalePointOffset.Value.x, hHeight * scalePointOffset.Value.y);

                Vector3[] holeVerts = new Vector3[4];

                if (sides == 4)
                {
                    holeVerts[0] = new Vector3(-hWidth, -hHeight);
                    holeVerts[1] = new Vector3(-hWidth, hHeight);
                    holeVerts[2] = new Vector3(hWidth, hHeight);
                    holeVerts[3] = new Vector3(hWidth, -hHeight);
                }
                else
                {
                    holeVerts = CreateNPolygon(sides, hWidth, hHeight);
                }
                
                for (int k = 0; k < holeVerts.Length; k++)
                {
                    // Position
                    holeVerts[k] += position;

                    // Scale 
                    Vector3 point = holeVerts[k] - scalePosition;
                    Vector3 v2 = Vector3.Scale(point, scale) + scalePosition;
                    holeVerts[k] = v2;

                    // Rotate
                    Vector3 euler = Quaternion.AngleAxis(angle, Vector3.forward).eulerAngles;
                    Vector3 v3 = Quaternion.Euler(euler) * (holeVerts[k] - position) + position;
                    holeVerts[k] = v3;

                    // Rotate to align with control points
                    euler = rotation.eulerAngles;
                    Vector3 v = Quaternion.Euler(euler) * (holeVerts[k] - position) + position;
                    holeVerts[k] = v;
                }

                for(int k = 0; k < quadVerts.Length; k++)
                {
                    // Scale quad
                    Vector3 point = quadVerts[k] - position;
                    Vector3 v = Vector3.Scale(point, Vector3.one * 0.999f) + position;
                    quadVerts[k] = v;
                }

                holeVerts = ConstrainPolygonToQuad(quadVerts, holeVerts);
                holePoints.Add(holeVerts.ToList());
            }
        }

        ProBuilderMesh mesh = ProBuilderMesh.Create();

        Vector3 normal = Vector3.Cross(points[0].DirectionToTarget(points[1]), points[0].DirectionToTarget(points[3]));

        if (flipFace)
            normal = -normal;

        mesh.CreateShapeFromPolygon(controlPoints.ToList(), 0, flipFace, holePoints);
        mesh.ToMesh();
        mesh.Refresh();

        Vector3[] normals = mesh.GetNormals();

        // The promesh normals should be the same (or roughly the same) as the calculated normal.
        if (!Vector3Extensions.Approximately(normals[0], normal, 0.1f))
        {
            mesh.faces[0].Reverse();
        }

        mesh.ToMesh();
        mesh.Refresh();

        foreach(IList<Vector3> hole in holePoints)
        {
            holeVertices.Add(hole.ToList());
        }

        return mesh;
    }
    /// <summary>
    /// Creates an 'n' polygon on the XY plane.
    /// Face normal vector will be 0,0,1.
    /// </summary>
    /// <param name="sides"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public static Vector3[] CreateNPolygon(int sides, float width, float height)
    {
        float angle = 360f / sides;

        Vector3[] vertices = new Vector3[sides];

        for (int i = 0; i < sides; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * (angle * i)) * width;
            float y = Mathf.Cos(Mathf.Deg2Rad * (angle * i)) * height;

            vertices[i] = new Vector3(x, y, 0);
        }

        return vertices;
    }

    /// <summary>
    /// Returns the constrained polygon.
    /// </summary>
    /// <param name="quad"></param>
    /// <param name="polygon"></param>
    /// <returns></returns>
    private static Vector3[] ConstrainPolygonToQuad(Vector3[] quad, Vector3[] polygon)
    {
        Vector3 min, max;

        MinMax(quad, out min, out max);

        for(int i = 0; i < polygon.Length; i++)
        {
            // Constrain the vertex position within the quad boundaries
            polygon[i] = new Vector3(Mathf.Clamp(polygon[i].x, min.x, max.x), Mathf.Clamp(polygon[i].y, min.y, max.y), Mathf.Clamp(polygon[i].z, min.z, max.z));
        }

        return polygon;
    }

    public static void MinMax(IEnumerable<Vector3> points, out Vector3 min, out Vector3 max)
    {
        Vector3[] vectors = points.ToArray();

        min = vectors[0];
        max = vectors[0];

        for (int i = 1; i < vectors.Length; i++)
        {
            if (min.x > vectors[i].x)
                min.x = vectors[i].x;
            if (min.y > vectors[i].y)
                min.y = vectors[i].y;
            if (min.z > vectors[i].z)
                min.z = vectors[i].z;

            if (max.x < vectors[i].x)
                max.x = vectors[i].x;
            if (max.y < vectors[i].y)
                max.y = vectors[i].y;
            if (max.z < vectors[i].z)
                max.z = vectors[i].z;
        }
    }

    public static ProBuilderMesh PolyFrameGrid(IEnumerable<Vector3> polyPoints, float polyHeight, float polyWidth, Vector3 scale, int columns, int rows, bool flipFace = false)
    {
        Vector3[] points = polyPoints.ToArray();
        Vector3 normal = points.CalculatePolygonFaceNormal();
        Vector3 position = ProMaths.Average(points);

        Vector3[] pointsCopy = new Vector3[points.Length];
        Array.Copy(points, pointsCopy, points.Length);

        Quaternion xZRotation = Quaternion.FromToRotation(normal, Vector3.up);

        for (int i = 0; i < pointsCopy.Length; i++)
        {
            Vector3 euler = xZRotation.eulerAngles;
            Vector3 v = Quaternion.Euler(euler) * (pointsCopy[i] - position) + position;
            pointsCopy[i] = v;
        }

        // Create Grid.
        float cellWidth = polyWidth / columns;
        float cellHeight = polyHeight / rows;

        int pointsWide = columns + 1;
        int pointsHigh = rows + 1;

        float xOffset = columns  * cellWidth * 0.5f;
        float yOffset = rows * cellHeight * 0.5f;

        Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, normal);
        Vector3[,] grid = new Vector3[pointsWide, pointsHigh];

        Quaternion endRotation = Quaternion.FromToRotation(Vector3.up, normal);

        for (int x = 0; x < pointsWide; x++)
        {
            for(int y = 0; y < pointsHigh; y++)
            {
                Vector3 point = new Vector3(x * cellWidth - xOffset, y * cellHeight - yOffset) + position;
                Vector3 firstEuler = rotation.eulerAngles;
                Vector3 v = Quaternion.Euler(firstEuler) * (point - position) + position;
                Vector3 secondEuler = xZRotation.eulerAngles;
                Vector3 v1 = Quaternion.Euler(secondEuler) * (v - position) + position;
                grid[x, y] = v1;
            }
        }

        List<IList<Vector3>> holePoints = new();

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3 bl = grid[x, y];
                Vector3 tl = grid[x, y + 1];
                Vector3 tr = grid[x + 1, y + 1];
                Vector3 br = grid[x + 1, y];

                Vector3[] quad = new Vector3[] { bl, tl, tr, br };
                List<Vector3> hole = new List<Vector3>();

                for( int i = 0; i < quad.Length; i++)
                {
                    int next = quad.GetNextControlPoint(i);

                    if (pointsCopy.IsPointInsidePolygon(quad[i]))
                    {
                        hole.Add(quad[i]);
                    }

                    if (SearchPolygonForIntersections(pointsCopy, quad[i], quad[next], out Vector3[] interPoints))
                    {
                        hole.AddRange(interPoints);
                    }
                }

                if (hole.Count == 0)
                    continue;

                for (int i = 0; i < pointsCopy.Length; i++)
                {
                    float distA = UnityEditor.HandleUtility.DistancePointLine(pointsCopy[i], bl, tl);
                    float distB = UnityEditor.HandleUtility.DistancePointLine(pointsCopy[i], tl, tr);
                    float distC = UnityEditor.HandleUtility.DistancePointLine(pointsCopy[i], tr, br);
                    float distD = UnityEditor.HandleUtility.DistancePointLine(pointsCopy[i], br, bl);

                    if (quad.IsPointInsidePolygon(pointsCopy[i]) ||
                        distA == 0 || distB == 0 || distC == 0 || distD == 0)
                    {
                        hole.Add(pointsCopy[i]);
                    }
                }

                hole = hole.Distinct(0.001f).ToList();

                if (hole.Count <= 2)
                    continue;

                hole = hole.SortPointsClockwise().ToList();

                Vector3 holeCentre = ProMaths.Average(hole);

                for (int i = 0; i < hole.Count(); i++)
                {
                    Vector3 point = hole[i] - holeCentre;
                    Vector3 v = Vector3.Scale(point, scale) + holeCentre;
                    hole[i] = v;
                }

                for (int i = 0; i < hole.Count(); i++)
                {
                    Vector3 point = hole[i] - position;
                    Vector3 euler = endRotation.eulerAngles;
                    Vector3 v = Quaternion.Euler(euler) * point + position;
                    hole[i] = v;
                }

                holePoints.Add(hole);
            }
        }

        ProBuilderMesh proBuilderMesh = ProBuilderMesh.Create();
        proBuilderMesh.CreateShapeFromPolygon(points, 0, flipFace, holePoints);
        proBuilderMesh.ToMesh();
        proBuilderMesh.Refresh();

        Vector3 aveNormal = ProMaths.Average(proBuilderMesh.GetNormals());

        if (!Vector3Extensions.Approximately(aveNormal, normal, 0.1f))
        {
            proBuilderMesh.faces[0].Reverse();
        }

        proBuilderMesh.ToMesh();
        proBuilderMesh.Refresh();

        return proBuilderMesh;
    }

    private static bool SearchPolygonForIntersections(IEnumerable<Vector3> polygon, Vector3 line1Start, Vector3 line1End, out Vector3[] intersectionPoints)
    {
        Vector3[] points = polygon.ToArray();
        intersectionPoints = new Vector3[0];
        List<Vector3> listOfIntersections = new();
        bool anyIntersectionsFound = false;

        for(int i = 0; i < points.Length; i++)
        {
            int next = points.GetNextControlPoint(i);

            // Fix: This may interect multiple lines of the polygon.
            if (Extensions.DoLinesIntersect(line1Start, line1End, points[i], points[next], out Vector3 intersection, false))
            {
                listOfIntersections.Add(intersection);
                anyIntersectionsFound = true;
            }
        }

        intersectionPoints = listOfIntersections.ToArray();

        return anyIntersectionsFound;
    }

    public static ProBuilderMesh HoleGrid(IEnumerable<Vector3> controlPoints, Vector3 scale, int columns, int rows, out List<Vector3[]> holeGridControlPoints, bool flipFace = false)
    {
        Vector3[] cps = controlPoints.ToArray();
        holeGridControlPoints = new();

        if (cps.Length != 4)
            return null;

        if (scale == Vector3.zero || columns == 0 || rows == 0)
        {
            return Quad(controlPoints, flipFace);
        }

        int pointsWide = columns + 1;
        int pointsHigh = rows + 1;

        Vector3[] leftPoints = Vector3Extensions.LerpCollection(cps[0], cps[1], pointsHigh).ToArray(); // row start points
        Vector3[] rightPoints = Vector3Extensions.LerpCollection(cps[3], cps[2], pointsHigh).ToArray(); // row end points

        List<Vector3[]> controlPointsGrid = new List<Vector3[]>();
        List<Vector3[]> holePointsGrid = new List<Vector3[]>();
        List<int[]> cIndexGrid = new List<int[]>();

        for (int i = 0; i < leftPoints.Length; i++)
        {
            controlPointsGrid.Add(Vector3Extensions.LerpCollection(leftPoints[i], rightPoints[i], pointsWide));

            cIndexGrid.Add(new int[pointsWide]);
            cIndexGrid[i][0] = i;

            for (int j = 1; j < cIndexGrid[0].Length; j++)
            {
                cIndexGrid[i][j] = cIndexGrid[i][j - 1] + pointsHigh;
            }
        }

        // Hole Transformations
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {

                Vector3 bl = controlPointsGrid[j][i];
                Vector3 tl = controlPointsGrid[j + 1][i];
                Vector3 tr = controlPointsGrid[j + 1][i + 1];
                Vector3 br = controlPointsGrid[j][i + 1];

                Vector3 bottomLeft = new Vector3(bl.x, bl.y, bl.z);
                Vector3 topLeft = new Vector3(tl.x, tl.y, tl.z);
                Vector3 topRight = new Vector3(tr.x, tr.y, tr.z);
                Vector3 bottomRight = new Vector3(br.x, br.y, br.z);

                Vector3[] hole = new Vector3[] { bottomLeft, topLeft, topRight, bottomRight };
                Vector3 centre = ProMaths.Average(hole);

                for (int k = 0; k < hole.Length; k++)
                {
                    // Scale
                    Vector3 point = hole[k] - centre;
                    Vector3 v = Vector3.Scale(point, scale) + centre;
                    hole[k] = v;
                }

                holePointsGrid.Add(hole);
            }
        }

        int count = 0;
        Vector3[] vertices = new Vector3[pointsWide * pointsHigh];
        Vector3[] holeVertices = new Vector3[holePointsGrid.Count * holePointsGrid[0].Length];

        // 2D to 1D
        for (int i = 0; i < pointsWide; i++)
        {
            for (int j = 0; j < pointsHigh; j++)
            {
                vertices[count] = controlPointsGrid[j][i];
                count++;
            }
        }

        List<int[]> hIndexGrid = new List<int[]>();

        int hCount = 0;

        for (int i = 0; i < holePointsGrid.Count; i++)
        {
            hIndexGrid.Add(Enumerable.Range(count, holePointsGrid[i].Length).ToArray());

            for (int j = 0; j < holePointsGrid[i].Length; j++)
            {
                holeVertices[hCount] = holePointsGrid[i][j];
                hCount++;
                count++;
            }
        }

        Vector3[] allVerts = vertices.Concat(holeVertices).ToArray();

        List<int> triangles = new List<int>();

        List<Face> faces = new List<Face>();

        hCount = 0;

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                int[] tris = new int[24];

                int cBottomLeft = cIndexGrid[j][i];
                int cTopLeft = cIndexGrid[j + 1][i];
                int cTopRight = cIndexGrid[j + 1][i + 1];
                int cBottomRight = cIndexGrid[j][i + 1];

                int[] hIndices = hIndexGrid[hCount];

                int hBottomLeft = hIndices[0];
                int hTopLeft = hIndices[1];
                int hTopRight = hIndices[2];
                int hBottomRight = hIndices[3];

                // Faces
                // Left
                tris[0] = cBottomLeft;
                tris[1] = cTopLeft;
                tris[2] = hBottomLeft;
                tris[3] = hBottomLeft;
                tris[4] = cTopLeft;
                tris[5] = hTopLeft;
                // Top
                tris[6] = hTopLeft;
                tris[7] = cTopLeft;
                tris[8] = cTopRight;
                tris[9] = cTopRight;
                tris[10] = hTopRight;
                tris[11] = hTopLeft;
                // Right
                tris[12] = cTopRight;
                tris[13] = cBottomRight;
                tris[14] = hTopRight;
                tris[15] = hTopRight;
                tris[16] = cBottomRight;
                tris[17] = hBottomRight;
                // Bottom
                tris[18] = hBottomRight;
                tris[19] = cBottomRight;
                tris[20] = cBottomLeft;
                tris[21] = cBottomLeft;
                tris[22] = hBottomLeft;
                tris[23] = hBottomRight;

                triangles.AddRange(tris);
                faces.Add(new Face(tris));

                hCount++;
            }
        }

        if (flipFace)
        {
            triangles = triangles.Reverse<int>().ToList();
        }

        holeGridControlPoints = holePointsGrid;

        ProBuilderMesh proBuilderMesh = ProBuilderMesh.Create(allVerts, new Face[] { new Face(triangles) });
        proBuilderMesh.ToMesh();
        proBuilderMesh.Refresh();

        return proBuilderMesh;
    }


    public static ProBuilderMesh DoorGrid(IEnumerable<Vector3> controlPoints, Vector3 scale, int columns, int rows, float sideOffset, out List<Vector3[]> doorGridControlPoints, bool flipFace = false)
    {
        Vector3[] cps = controlPoints.ToArray();

        doorGridControlPoints = new();

        if (cps.Length != 4)
            return null;

        if (scale == Vector3.zero || columns == 0 || rows == 0)
        {
            return Quad(controlPoints, flipFace);
        }

        int pointsWide = columns + 1;
        int pointsHigh = rows + 1;

        Vector3[] leftPoints = Vector3Extensions.LerpCollection(cps[0], cps[1], pointsHigh).ToArray(); // row start points
        Vector3[] rightPoints = Vector3Extensions.LerpCollection(cps[3], cps[2], pointsHigh).ToArray(); // row end points

        List<Vector3[]> controlPointsGrid = new List<Vector3[]>();
        List<Vector3[]> holePointsGrid = new List<Vector3[]>();
        List<int[]> cIndexGrid = new List<int[]>();

        for (int i = 0; i < leftPoints.Length; i++)
        {
            controlPointsGrid.Add(Vector3Extensions.LerpCollection(leftPoints[i], rightPoints[i], pointsWide));

            cIndexGrid.Add(new int[pointsWide]);
            cIndexGrid[i][0] = i;

            for (int j = 1; j < cIndexGrid[0].Length; j++)
            {
                cIndexGrid[i][j] = cIndexGrid[i][j - 1] + pointsHigh;
            }
        }

        // Hole Transformations
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Vector3 bl = controlPointsGrid[j][i];
                Vector3 tl = controlPointsGrid[j + 1][i];
                Vector3 tr = controlPointsGrid[j + 1][i + 1];
                Vector3 br = controlPointsGrid[j][i + 1];

                Vector3 bottomLeft = new Vector3(bl.x, bl.y, bl.z);
                Vector3 topLeft = new Vector3(tl.x, tl.y, tl.z);
                Vector3 topRight = new Vector3(tr.x, tr.y, tr.z);
                Vector3 bottomRight = new Vector3(br.x, br.y, br.z);

                Vector3[] bottomPoints = new Vector3[] { bottomLeft, bottomRight };
                Vector3[] topPoints = new Vector3[] { topLeft, topRight };
                Vector3 bottomCentre = ProMaths.Average(bottomPoints);

                Vector3 bottomScale = new Vector3(scale.x, 0, scale.z);

                for (int k = 0; k < bottomPoints.Length; k++)
                {
                    // Scale
                    Vector3 point = bottomPoints[k] - bottomCentre;
                    Vector3 v = Vector3.Scale(point, bottomScale) + bottomCentre;
                    bottomPoints[k] = v;
                }

                for (int k = 0; k < topPoints.Length; k++)
                {
                    // Scale
                    Vector3 point = topPoints[k] - bottomCentre;
                    Vector3 v = Vector3.Scale(point, scale) + bottomCentre;
                    topPoints[k] = v;
                }

                float width = Vector3.Distance(bottomPoints[0], bottomPoints[1]);
                float height = Vector3.Distance(bottomPoints[0], topPoints[0]);
                Vector3 dir = bottomPoints[0].DirectionToTarget(bottomPoints[1]);

                if (sideOffset > 0)
                {
                    bottomPoints[0] = Vector3.Lerp(bottomPoints[0], bl, sideOffset);
                    bottomPoints[1] = bottomPoints[0] + (dir * width);
                }
                else
                {
                    sideOffset = -sideOffset;
                    bottomPoints[1] = Vector3.Lerp(bottomPoints[1], br, sideOffset);
                    bottomPoints[0] = bottomPoints[1] + (-dir * width);
                    sideOffset = -sideOffset;
                }

                topPoints[0] = bottomPoints[0] + (Vector3.up * height);
                topPoints[1] = bottomPoints[1] + (Vector3.up * height);

                Vector3[] hole = new Vector3[] { bottomPoints[0], topPoints[0], topPoints[1], bottomPoints[1] };

                holePointsGrid.Add(hole);
            }
        }

        int count = 0;
        Vector3[] vertices = new Vector3[pointsWide * pointsHigh];
        Vector3[] holeVertices = new Vector3[holePointsGrid.Count * holePointsGrid[0].Length];

        // 2D to 1D
        for (int i = 0; i < pointsWide; i++)
        {
            for (int j = 0; j < pointsHigh; j++)
            {
                vertices[count] = controlPointsGrid[j][i];
                count++;
            }
        }

        List<int[]> hIndexGrid = new List<int[]>();

        int hCount = 0;

        for (int i = 0; i < holePointsGrid.Count; i++)
        {
            hIndexGrid.Add(Enumerable.Range(count, holePointsGrid[i].Length).ToArray());

            for (int j = 0; j < holePointsGrid[i].Length; j++)
            {
                holeVertices[hCount] = holePointsGrid[i][j];
                hCount++;
                count++;
            }
        }

        Vector3[] allVerts = vertices.Concat(holeVertices).ToArray();

        List<int> triangles = new List<int>();

        List<Face> faces = new List<Face>();

        hCount = 0;

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                int[] tris = new int[18];

                int cBottomLeft = cIndexGrid[j][i];
                int cTopLeft = cIndexGrid[j + 1][i];
                int cTopRight = cIndexGrid[j + 1][i + 1];
                int cBottomRight = cIndexGrid[j][i + 1];

                int[] hIndices = hIndexGrid[hCount];

                int hBottomLeft = hIndices[0];
                int hTopLeft = hIndices[1];
                int hTopRight = hIndices[2];
                int hBottomRight = hIndices[3];

                // Faces
                // Left
                tris[0] = cBottomLeft;
                tris[1] = cTopLeft;
                tris[2] = hBottomLeft;
                tris[3] = hBottomLeft;
                tris[4] = cTopLeft;
                tris[5] = hTopLeft;
                // Top
                tris[6] = hTopLeft;
                tris[7] = cTopLeft;
                tris[8] = cTopRight;
                tris[9] = cTopRight;
                tris[10] = hTopRight;
                tris[11] = hTopLeft;
                // Right
                tris[12] = cTopRight;
                tris[13] = cBottomRight;
                tris[14] = hTopRight;
                tris[15] = hTopRight;
                tris[16] = cBottomRight;
                tris[17] = hBottomRight;

                triangles.AddRange(tris);
                faces.Add(new Face(tris));

                hCount++;
            }
        }

        if (flipFace)
        {
            triangles = triangles.Reverse<int>().ToList();
        }

        ProBuilderMesh proBuilderMesh = ProBuilderMesh.Create(allVerts, new Face[] { new Face(triangles) });
        proBuilderMesh.ToMesh();
        proBuilderMesh.Refresh();

        doorGridControlPoints = holePointsGrid;

        return proBuilderMesh;
    }
}
