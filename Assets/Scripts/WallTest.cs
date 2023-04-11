using System;
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
    [SerializeField] private Material m_Material;
    [SerializeField, Range(0, 1)] private float m_Depth;
    [SerializeField, Range(1, 100)]private float m_Height;
    [SerializeField, Range(0, 1)] private float m_HoleHeight;
    [SerializeField, Range(0, 1)] private float m_HoleWidth;
    [SerializeField, Range(-25, 25)] private float m_HoleRotation;
    [SerializeField, Range(0, 10)] private int m_HoleRows, m_HoleColumns;

    private List<TheWall> m_Walls;
    private Polytool m_Polytool;
    private List<IList<Vector3>> m_Holes;

    public float Depth => m_Depth;
    public float Height => m_Height;
    public float HoleHeight => m_HoleHeight;
    public float HoleWidth => m_HoleWidth;
    public float HoleRotation => m_HoleRotation;
    public int HoleRows => m_HoleRows;
    public int HoleColumns => m_HoleColumns;

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
            float w = Mathf.Lerp(-1, 1, m_Depth);

            insidePoints[i] += scalingDirections[i] * w;
        }

        //m_Holes = new List<IList<Vector3>>();

        //for (int i = 0; i < controlPoints.Count; i++) 
        //{
        //    List<Vector3> holePoints = new List<Vector3>(new Vector3[4]);
        //    int next = m_Polytool.GetNextPoint(i);

        //    Vector3 bottomHeight = Vector3.up * (m_Height * 0.5f);
        //    Vector3 topHeight = Vector3.up * (m_Height * 0.75f);

        //    Vector3 topLeft = Vector3.Lerp(insidePoints[i], insidePoints[next], 0.5f);
        //    Vector3 topRight = Vector3.Lerp(insidePoints[i], insidePoints[next], 0.6f);
        //    Vector3 bottomRight = topRight;
        //    Vector3 bottomLeft = topLeft;

        //    topLeft += bottomHeight;
        //    topRight += bottomHeight;
        //    bottomRight += topHeight;
        //    bottomLeft += topHeight;

        //    holePoints[0] = bottomLeft;
        //    holePoints[1] = topLeft;
        //    holePoints[2] = topRight;
        //    holePoints[3] = bottomRight;

        //    m_Holes.Add(holePoints);
        //}


        Vector3 h = m_Height * Vector3.up;

        // Construct the walls 
        for (int i = 0; i < controlPoints.Count; i++)
        {
            // Something to think about.
            // How to constrain the hole points to within the bounds of the wall.

            Vector3 nextInside = insidePoints[m_Polytool.GetNextPoint(i)];

            Vector3[] points = new Vector3[] { insidePoints[i], insidePoints[i] + h, nextInside + h, nextInside };

            Vector3 scale = new Vector3(m_HoleWidth, m_HoleHeight, m_HoleWidth);

            ProBuilderMesh proBuilderMesh = CreatePlaneWithHole.Create(points, Vector3.zero, m_HoleRotation, scale, m_HoleColumns, m_HoleRows);
            proBuilderMesh.name = "Wall " + i.ToString();

            ProBuilderMesh inside = CreatePlaneWithHole.Create(points, Vector3.zero, m_HoleRotation, scale, m_HoleColumns, m_HoleRows, true);
            inside.ToMesh();
            inside.Refresh();

            proBuilderMesh.Extrude(new Face[] { proBuilderMesh.faces[0] }, ExtrudeMethod.FaceNormal, m_Depth);

            if(i == 0)
                proBuilderMesh.SetSelectedFaces(new Face[] { proBuilderMesh.faces[0] });

            Vertex[] positions = proBuilderMesh.GetVertices();
            Vector3 nextControlPoint = controlPoints[m_Polytool.GetNextPoint(i)];
            Vector3[] outsidePoints = new Vector3[] { controlPoints[i], controlPoints[i] + h, nextControlPoint + h, nextControlPoint };

            if (false)
            {
                int[] cornerIndices = proBuilderMesh.CornerPositions(proBuilderMesh.faces[0]);

                for (int j = 0; j < cornerIndices.Length; j++)
                {
                    int index = cornerIndices[j];
                    Vector3 a = positions[index].position;

                    int realIndex = 0;
                    float distance = Vector3.Distance(a, outsidePoints[realIndex]);

                    for (int k = 0; k < outsidePoints.Length; k++)
                    {
                        float tempDistance = Vector3.Distance(a, outsidePoints[k]);

                        if (tempDistance <= distance)
                        {
                            distance = tempDistance;
                            realIndex = k;
                        }
                    }

                    positions[index].position = outsidePoints[realIndex];

                    List<int> shared = proBuilderMesh.GetCoincidentVertices(new int[] { index });

                    for (int k = 0; k < shared.Count; k++)
                    {
                        positions[shared[k]].position = outsidePoints[realIndex];
                    }

                }
            }

            proBuilderMesh.GetComponent<Renderer>().sharedMaterial = m_Material;
            proBuilderMesh.transform.SetParent(transform, true);

            List<Vector3> firstFacePoints = new List<Vector3>();

            int[] indices = proBuilderMesh.faces[0].distinctIndexes.ToArray();
            Array.Sort(indices);

            for (int j = 0; j < indices.Length; j++)
            {
                firstFacePoints.Add(positions[indices[j]].position);
            }

            proBuilderMesh.AddComponent<TheWall>().Init(proBuilderMesh);

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
