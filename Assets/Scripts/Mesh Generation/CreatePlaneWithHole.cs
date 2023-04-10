using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using ProMaths = UnityEngine.ProBuilder.Math;

public static class CreatePlaneWithHole
{
    
    public static ProBuilderMesh Create(IEnumerable<Vector3> controlPoints, Vector3 offset, float angle, Vector3 scale, int columns, int rows, bool flipFace = false)
    {
        Vector3[] cps = controlPoints.ToArray();

        if (cps.Length != 4)
            return null;

        if (scale == Vector3.zero || columns == 0 || rows == 0)
        {
            int[] tris = new int[6];
            tris[0] = 0;
            tris[1] = 1;
            tris[2] = 3;
            tris[3] = 3;
            tris[4] = 1;
            tris[5] = 2;

            if (flipFace)
                tris = tris.Reverse().ToArray();

            ProBuilderMesh quad = ProBuilderMesh.Create(cps, new Face[] { new Face(tris) });
            quad.ToMesh();
            quad.Refresh();
            return quad;
        }

        // int numberOfPoints = 2 + (6 * columns);
        int pointsWide =  columns + 1;
        int pointsHigh = rows + 1;

        Vector3[] leftPoints = Vector3Extensions.LerpCollection(cps[0], cps[1], pointsHigh).ToArray(); // row start points
        Vector3[] rightPoints = Vector3Extensions.LerpCollection(cps[3], cps[2], pointsHigh).ToArray(); // row end points

        List<Vector3[]> controlPointsGrid = new List<Vector3[]>();
        List<Vector3[]> holePointsGrid = new List<Vector3[]>();
        List<int[]> indexGrid = new List<int[]>();
        int start = 0;

        for (int i = 0; i < leftPoints.Length; i++)
        {
            controlPointsGrid.Add(Vector3Extensions.LerpCollection(leftPoints[i], rightPoints[i], pointsWide));
            holePointsGrid.Add(Vector3Extensions.LerpCollection(leftPoints[i], rightPoints[i], pointsWide));
            indexGrid.Add(Enumerable.Range(start, leftPoints.Length).ToArray());

            start += leftPoints.Length;
        }
        //Array.Copy(vertices, holeVertices, count);

        // Hole Transformations
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Vector3 bl = controlPointsGrid[i][j];
                Vector3 tl = controlPointsGrid[i + 1][j];
                Vector3 tr = controlPointsGrid[i + 1][j + 1];
                Vector3 br = controlPointsGrid[i][j + 1];

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

                holePointsGrid[i][j] = hole[0];
                holePointsGrid[i + 1][j] = hole[1];
                holePointsGrid[i + 1][j + 1] = hole[2];
                holePointsGrid[i][j + 1] = hole[3];
            }
        }
        {
            //for (int i = 0; i < (vertices.Length - rows - 1); i++)
            //{
            //    if ((i + 1) % columns == 0)
            //        continue;

            //    int a = i;
            //    int b = i + rows;
            //    int c = i + rows + 1;
            //    int d = i + 1;

            //    Vector3[] hole = new Vector3[] { vertices[a] , vertices[b] , vertices[c] , vertices[d] };
            //    Vector3 tPoint = ProMaths.Average(hole);

            //    for (int j = 0; j < hole.Length; j++)
            //    {
            //        // Scale
            //        Vector3 point = hole[j] - tPoint;
            //        Vector3 v3 = Vector3.Scale(point, scale) + tPoint;
            //        hole[j] = v3;
            //    }

            //    holeVertices[a] = hole[0];
            //    holeVertices[b] = hole[1];
            //    holeVertices[c] = hole[2];
            //    holeVertices[d] = hole[3];
            //}
        }

        int count = 0;
        Vector3[] vertices = new Vector3[pointsWide * pointsHigh];
        Vector3[] holeVertices = new Vector3[vertices.Length];

        // 2D to 1D
        for (int i = 0; i < pointsWide; i++)
        {
            for (int j = 0; j < pointsHigh; j++)
            {
                vertices[count] = controlPointsGrid[i][j];
                holeVertices[count] = holePointsGrid[i][j];
                count++;
            }
        }

        Vector3[] allVerts = vertices.Concat(holeVertices).ToArray();

        List<int> triangles = new List<int>();

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                int[] tris = new int[24];

                int cBottomLeft = indexGrid[i][j];
                int cTopLeft = indexGrid[i + 1][j];
                int cTopRight = indexGrid[i + 1][j + 1];
                int cBottomRight = indexGrid[i][j + 1];

                int hBottomLeft = indexGrid[i][j] + count;
                int hTopLeft = indexGrid[i + 1][j] + count;
                int hTopRight = indexGrid[i + 1][j + 1] + count;
                int hBottomRight = indexGrid[i][j + 1] + count;

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
            }
        }

        {
            //for (int i = 0; i < (vertices.Length - rows - 1); i++)
            //{
            //    if ((i + 1) % columns == 0)
            //        continue;

            //    int[] tris = new int[24];

            //    int cBottomLeft = i;
            //    int cTopLeft = i + rows;
            //    int cTopRight = i + rows + 1;
            //    int cBottomRight = i + 1;

            //    int hBottomLeft = cBottomLeft + count;
            //    int hTopLeft = cTopLeft + count;
            //    int hTopRight = cTopRight + count;
            //    int hBottomRight = cBottomRight + count;

            //    // Left
            //    tris[0] = cBottomLeft;
            //    tris[1] = cTopLeft;
            //    tris[2] = hBottomLeft;
            //    tris[3] = hBottomLeft;
            //    tris[4] = cTopLeft;
            //    tris[5] = hTopLeft;
            //    // Top
            //    tris[6] = hTopLeft;
            //    tris[7] = cTopLeft;
            //    tris[8] = cTopRight;
            //    tris[9] = cTopRight;
            //    tris[10] = hTopRight;
            //    tris[11] = hTopLeft;
            //    // Right
            //    tris[12] = cTopRight;
            //    tris[13] = cBottomRight;
            //    tris[14] = hTopRight;
            //    tris[15] = hTopRight;
            //    tris[16] = cBottomRight;
            //    tris[17] = hBottomRight;
            //    // Bottom
            //    tris[18] = hBottomRight;
            //    tris[19] = cBottomRight;
            //    tris[20] = cBottomLeft;
            //    tris[21] = cBottomLeft;
            //    tris[22] = hBottomLeft;
            //    tris[23] = hBottomRight;

            //    triangles.AddRange(tris);
            //}
        }


        if (flipFace)
        {
            triangles = triangles.Reverse<int>().ToList();
        }

        ProBuilderMesh proBuilderMesh = ProBuilderMesh.Create(allVerts, new Face[] { new Face(triangles) });
        proBuilderMesh.ToMesh();
        proBuilderMesh.Refresh();

        return proBuilderMesh;













        //Vector3 transformPoint = ProMaths.Average(cps);
        //Vector3[] holePositions = cps;

        //Vector3 dir = cps[0].GetDirectionToTarget(cps[3]);
        //Vector3 forward = Vector3.Cross(Vector3.up, dir);


        //for (int i = 0; i < holePositions.Length; i++)
        //{
        //    // Scale
        //    Vector3 point = holePositions[i] - transformPoint;
        //    Vector3 v = Vector3.Scale(point, scale) + transformPoint;
        //    holePositions[i] = v;

        //    // Rotation
        //    Vector3 localEulerAngles = forward * angle;
        //    Vector3 v1 = Quaternion.Euler(localEulerAngles) * (holePositions[i] - transformPoint) + transformPoint;
        //    offset = v1 - holePositions[i];
        //    holePositions[i] += offset;

        //    positions[i + 4] = holePositions[i];
        //}

        ////int cBottomLeft = i;
        ////int cTopLeft = i + rows;
        ////int cTopRight = i + rows + 1;
        ////int cBottomRight = i + 1;

        //int cBottomLeft = 0;
        //int cTopLeft = 1;
        //int cTopRight = 2;
        //int cBottomRight = 3;

        //int hBottomLeft = 4;
        //int hTopLeft = 5;
        //int hTopRight = 6;
        //int hBottomRight = 7;

        //List<int> triangles = new List<int>(new int[24]);

        //// Left
        //triangles[0] = cBottomLeft;
        //triangles[1] = cTopLeft;
        //triangles[2] = hBottomLeft;
        //triangles[3] = hBottomLeft;
        //triangles[4] = cTopLeft;
        //triangles[5] = hTopLeft;
        //// Top
        //triangles[6] = hTopLeft;
        //triangles[7] = cTopLeft;
        //triangles[8] = cTopRight;
        //triangles[9] = cTopRight;
        //triangles[10] = hTopRight;
        //triangles[11] = hTopLeft;
        //// Right
        //triangles[12] = cTopRight;
        //triangles[13] = cBottomRight;
        //triangles[14] = hTopRight;
        //triangles[15] = hTopRight;
        //triangles[16] = cBottomRight;
        //triangles[17] = hBottomRight;
        //// Bottom
        //triangles[18] = hBottomRight;
        //triangles[19] = cBottomRight;
        //triangles[20] = cBottomLeft;
        //triangles[21] = cBottomLeft;
        //triangles[22] = hBottomLeft;
        //triangles[23] = hBottomRight;


    }

}
