using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
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

    public static ProBuilderMesh CubeProjection(IEnumerable<Vector3> controlPoints, float height, bool isOutside = true)
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

        if (!isOutside) // projects the new vertices inside instead of out.
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

    public static ProBuilderMesh Quad (IEnumerable<Vector3> controlPoints, bool flipFace = false)
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
        Vector3[] h = Vector3Extensions.LerpCollection(points[2] + (forwardB * width) + (cross * height), points[3] + (forwardB * width) + (cross * height) , numberOfPCubes);

        List<Vector3> positions = new List<Vector3>();
        List<int> triangles = new List<int>();
        int vertCount = 0;

        for(int i = 0; i < numberOfPCubes; i++)
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

            for(int j = 0; j < tris.Length; j++)
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

    public static ProBuilderMesh HoleGrid(IEnumerable<Vector3> controlPoints, Vector3 offset, float angle, Vector3 scale, int columns, int rows, bool flipFace = false)
    {
        Vector3[] cps = controlPoints.ToArray();

        if (cps.Length != 4)
            return null;

        if (scale == Vector3.zero || columns == 0 || rows == 0)
        {
            return Quad(controlPoints, flipFace);
        }

        int pointsWide =  columns + 1;
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
                cIndexGrid[i][j] = cIndexGrid[i][j-1] + pointsHigh;
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

            for(int j = 0; j < holePointsGrid[i].Length; j++)
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

        ProBuilderMesh proBuilderMesh = ProBuilderMesh.Create(allVerts, new Face[] { new Face(triangles) });
        proBuilderMesh.ToMesh();
        proBuilderMesh.Refresh();

        return proBuilderMesh;
    }


    public static ProBuilderMesh DoorGrid(IEnumerable<Vector3> controlPoints, Vector3 scale, int columns, int rows, out List<Vector3[]> doorGridControlPoints, bool flipFace = false)
    {
        Vector3[] cps = controlPoints.ToArray();

        doorGridControlPoints = new List<Vector3[]>();

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
