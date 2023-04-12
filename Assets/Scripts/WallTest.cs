using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
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
    private List<Vector3> m_insidePoints;
    private List<Vector3> m_Directions;

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

        Vector3 h = m_Height * Vector3.up;

        m_Directions = new List<Vector3>();
        m_insidePoints = insidePoints;

        // Construct the walls 
        for (int i = 0; i < controlPoints.Count; i++)
        {
            int oneNext = m_Polytool.GetNextPoint(i);
            Vector3 oneNextInside = insidePoints[oneNext];

            Vector3[] points = new Vector3[] { insidePoints[i], insidePoints[i] + h, oneNextInside + h, oneNextInside };

            Vector3 scale = new Vector3(m_HoleWidth, m_HoleHeight, m_HoleWidth);

            ProBuilderMesh outside = CreatePlaneWithHole.Create(points, Vector3.zero, m_HoleRotation, scale, m_HoleColumns, m_HoleRows);
            outside.name = "Wall " + i.ToString();

            ProBuilderMesh inside = CreatePlaneWithHole.Create(points, Vector3.zero, m_HoleRotation, scale, m_HoleColumns, m_HoleRows, true);
            inside.ToMesh();
            inside.Refresh();

            outside.Extrude(new Face[] { outside.faces[0] }, ExtrudeMethod.FaceNormal, m_Depth);
            outside.GetComponent<Renderer>().sharedMaterial = m_Material;
            outside.transform.SetParent(transform, true);

            outside.AddComponent<TheWall>().Init(outside);

            outside.ToMesh();
            outside.Refresh();

            inside.GetComponent<Renderer>().sharedMaterial = m_Material;
            inside.transform.SetParent(outside.transform, true);



            // Add corner posts
            int onePrevious = m_Polytool.GetPreviousPoint(i);
            Vector3 onePreviousInside = insidePoints[onePrevious];

            Vector3 dir = insidePoints[i].GetDirectionToTarget(oneNextInside);
            Vector3 cross = Vector3.Cross(Vector3.up, dir) * m_Depth;

            Vector3 dir1 = insidePoints[i].GetDirectionToTarget(onePreviousInside);
            Vector3 cross1 = Vector3.Cross(dir1, Vector3.up) * m_Depth;

            m_Directions.Add(cross1);

            Vector3 zero = controlPoints[i];
            Vector3 one = insidePoints[i] + cross;
            Vector3 two = insidePoints[i];
            Vector3 three = insidePoints[i] + cross1;

            ProBuilderMesh post = ProBuilderMesh.Create();
            post.name = "Corner";
            post.CreateShapeFromPolygon(new Vector3[] { zero, one, two, three }, m_Height, false);

            post.GetComponent<Renderer>().sharedMaterial = m_Material;
            post.transform.SetParent(outside.transform, true);

            post.ToMesh();
            post.Refresh();



            //proBuilderMesh.AddComponent<MeshCollider>().convex = true;
        }


    }

    private void OnDrawGizmos()
    {
        for(int i = 0; i < m_insidePoints.Count; i++)
        {
            Handles.DoPositionHandle(m_insidePoints[i], Quaternion.LookRotation(m_Directions[i], Vector3.up));
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
