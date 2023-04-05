using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Rendering;
using ProMaths = UnityEngine.ProBuilder.Math;

public class WallTest : MonoBehaviour
{
    [SerializeField] private Polytool m_HolePolytool;
    [SerializeField] private Material m_Material;
    [SerializeField, Range(0, 1)] private float m_Width;
    [SerializeField] private float m_Height;

    private List<TheWall> m_Walls;
    private Polytool m_Polytool;
    private List<IList<Vector3>> m_Holes;

    public float Width => m_Width;
    public float Height => m_Height;

    public void CreateWallOutline()
    {
        Deconstruct();
        m_Polytool = GetComponent<Polytool>();

        List<Vector3> controlPoints = new List<Vector3>();

        foreach (Vector3 point in m_Polytool.ControlPoints)
        {
            controlPoints.Add(point);
        }

        List<Vector3> insidePoints = new List<Vector3>();
        List<Vector3> scalingDirections = new List<Vector3>();

        for (int i = 0; i < controlPoints.Count; i++)
        {
            int previousPoint = controlPoints.GetPreviousControlPoint(i);
            int nextPoint = controlPoints.GetNextControlPoint(i);

            Vector3 a = controlPoints[i].GetDirectionToTarget(controlPoints[nextPoint]);
            Vector3 b = controlPoints[i].GetDirectionToTarget(controlPoints[previousPoint]);
            Vector3 c = Vector3.Lerp(a, b, 0.5f);

            Vector3 pos = controlPoints[i] + c;

            if (!controlPoints.IsPointInsidePolygon(pos))
            {
                pos = controlPoints[i] - c;
                scalingDirections.Add(-c);
            }
            else
            {
                scalingDirections.Add(c);
            }

            insidePoints.Add(pos);
        }

        for (int i = 0; i < insidePoints.Count; i++)
        {
            float w = Mathf.Lerp(-1, 1, m_Width);

            insidePoints[i] += scalingDirections[i] * w;
        }

        m_Holes = new List<IList<Vector3>>();

        for (int i = 0; i < controlPoints.Count; i++) 
        {
            List<Vector3> holePoints = new List<Vector3>(new Vector3[4]);
            int next = m_Polytool.GetNextPoint(i);

            Vector3 bottomHeight = Vector3.up * (m_Height * 0.5f);
            Vector3 topHeight = Vector3.up * (m_Height * 0.75f);

            Vector3 topLeft = Vector3.Lerp(insidePoints[i], insidePoints[next], 0.5f);
            Vector3 topRight = Vector3.Lerp(insidePoints[i], insidePoints[next], 0.6f);
            Vector3 bottomRight = topRight;
            Vector3 bottomLeft = topLeft;

            topLeft += bottomHeight;
            topRight += bottomHeight;
            bottomRight += topHeight;
            bottomLeft += topHeight;

            holePoints[0] = bottomLeft;
            holePoints[1] = topLeft;
            holePoints[2] = topRight;
            holePoints[3] = bottomRight;

            m_Holes.Add(holePoints);
        }

        int[] triangles = new int[24];
        
        // Left
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 4;
        triangles[3] = 4;
        triangles[4] = 5;
        triangles[5] = 0;
        // Bottom
        triangles[6] = 0;
        triangles[7] = 5;
        triangles[8] = 6;
        triangles[9] = 6;
        triangles[10] = 3;
        triangles[11] = 0;
        // Right
        triangles[12] = 6;
        triangles[13] = 7;
        triangles[14] = 2;
        triangles[15] = 2;
        triangles[16] = 3;
        triangles[17] = 6;
        // Top
        triangles[18] = 4;
        triangles[19] = 1;
        triangles[20] = 2;
        triangles[21] = 2;
        triangles[22] = 7;
        triangles[23] = 4;

        Vector3 h = m_Height * Vector3.up;

        // Construct the walls 
        for (int i = 0; i < controlPoints.Count; i++)
        {
            Vector3 next = insidePoints[m_Polytool.GetNextPoint(i)];

            List<Vector3> points = new List<Vector3>() { insidePoints[i], insidePoints[i] + h, next + h, next };

            foreach(Vector3 point in m_Holes[i])
            {
                points.Add(point);
            }

            Vector3 dirA = insidePoints[0].GetDirectionToTarget(insidePoints[3]);
            Vector3 dirB = insidePoints[1].GetDirectionToTarget(insidePoints[2]);
            Vector3 dir = Vector3.Lerp(dirA, dirB, 0.5f);
            Vector3 centre = (insidePoints[0] + insidePoints[3]) / 2;
            Vector3 forward = Vector3.Cross(Vector3.up, dir);

            ProBuilderMesh proBuilderMesh = ProBuilderMesh.Create(points, new Face[] { new Face(triangles) });
            proBuilderMesh.name = "Wall " + i.ToString();

            ProBuilderMesh inside = ProBuilderMesh.Create(points, new Face[] { new Face(triangles.Reverse()) });
            inside.ToMesh();
            inside.Refresh();

            proBuilderMesh.Extrude(new Face[] { proBuilderMesh.faces[0] }, ExtrudeMethod.FaceNormal, m_Width);

            //proBuilderMesh.CreateShapeFromPolygon(points, 1, false, holes);

            if(i == 0)
                proBuilderMesh.SetSelectedFaces(new Face[] { proBuilderMesh.faces[0] });

            Vertex[] positions = proBuilderMesh.GetVertices();
            Vector3 nextControlPoint = controlPoints[m_Polytool.GetNextPoint(i)];
            Vector3[] realPoints = new Vector3[] { controlPoints[i], controlPoints[i] + h, nextControlPoint + h, nextControlPoint };

            //if (i == 50)
            {

                int[] cornerIndices = proBuilderMesh.CornerPositions(proBuilderMesh.faces[0]);

                for (int j = 0; j < cornerIndices.Length; j++)
                {
                    int index = cornerIndices[j];
                    Vector3 a = positions[index].position;

                    int realIndex = 0;
                    float distance = Vector3.Distance(a, realPoints[realIndex]);

                    for (int k = 0; k < realPoints.Length; k++)
                    {
                        float tempDistance = Vector3.Distance(a, realPoints[k]);

                        if (tempDistance <= distance)
                        {
                            distance = tempDistance;
                            realIndex = k;
                        }
                    }

                    positions[index].position = realPoints[realIndex];

                    List<int> shared = proBuilderMesh.GetCoincidentVertices(new int[] { index });

                    for (int k = 0; k < shared.Count; k++)
                    {
                        positions[shared[k]].position = realPoints[realIndex];
                    }

                }
            }

            proBuilderMesh.GetComponent<Renderer>().sharedMaterial = m_Material;
            proBuilderMesh.transform.SetParent(transform, true);
            proBuilderMesh.AddComponent<TheWall>().Init(points, 1, m_Holes[i].ToList());

            proBuilderMesh.SetVertices(positions);
            proBuilderMesh.ToMesh();
            proBuilderMesh.Refresh();

            inside.GetComponent<Renderer>().sharedMaterial = m_Material;
            inside.transform.SetParent(proBuilderMesh.transform, true);

            //proBuilderMesh.AddComponent<MeshCollider>().convex = true;
        }


    }

    private void Deconstruct()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (Application.isEditor)
            {
                DestroyImmediate(child);
            }
            else
            {
                Destroy(child);
            }

        }

        if (transform.childCount > 0)
        {
            Deconstruct();
        }
    }
}
