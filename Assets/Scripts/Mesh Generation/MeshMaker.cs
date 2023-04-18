using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using ProMaths = UnityEngine.ProBuilder.Math;

public static class MeshMaker
{
    public static ProBuilderMesh Cube(IEnumerable<Vector3> controlPoints, float depth, bool flipFace = false)
    {
        // TODO: Add other faces.
        // Issue: faces with shared triangles see to mess up.

        Vector3[] points = controlPoints.ToArray();
        if (controlPoints.ToArray().Length != 4)
            return null;

        Vector3 forward = points[0].GetDirectionToTarget(points[3]);
        Vector3 right = Vector3.Cross(Vector3.up, forward) * depth;

        Vector3[] vertices = new Vector3[8];

        vertices[0] = points[0] + right;
        vertices[1] = points[3] + right;
        vertices[2] = points[3];
        vertices[3] = points[0];

        vertices[4] = points[1] + right;
        vertices[5] = points[2] + right;
        vertices[6] = points[2];
        vertices[7] = points[1];

        List<SharedVertex> shared = new List<SharedVertex>();
        

        Face[] faces = new Face[] { new Face(new int[] { 0, 4, 1, 1, 4, 5 }), // Front
                                    new Face(new int[] { 2, 6, 7, 7, 3, 2 })}; // Back

        ProBuilderMesh cube = ProBuilderMesh.Create(vertices, faces);

        cube.ToMesh();
        cube.Refresh();

        return cube;
    }

    public static ProBuilderMesh Quad (IEnumerable<Vector3> controlPoints, bool flipFace = false)
    {
        if (controlPoints.ToArray().Length != 4)
            return null;

        int[] tris = new int[6];
        tris[0] = 0;
        tris[1] = 1;
        tris[2] = 3;
        tris[3] = 3;
        tris[4] = 1;
        tris[5] = 2;

        if (flipFace)
            tris = tris.Reverse().ToArray();

        ProBuilderMesh quad = ProBuilderMesh.Create(controlPoints, new Face[] { new Face(tris) });
        quad.ToMesh();
        quad.Refresh();
        return quad;
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
                //hIndexGrid.Add()
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

}
