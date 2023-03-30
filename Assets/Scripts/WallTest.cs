using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Rendering;
using ProMaths = UnityEngine.ProBuilder.Math;

public class WallTest : MonoBehaviour
{
    [SerializeField] private Material m_Material;
    [SerializeField, Range(0, 1)] private float m_Width;
    [SerializeField] private float m_Height;
    private Polytool m_Polytool;
    private List<IList<Vector3>> m_Holes;

    public float Width => m_Width;
    public float Height => m_Height;

    public void CreateWallOutline()
    {
        Deconstruct();
        m_Polytool = GetComponent<Polytool>();

        List<Vector3> controlPoints = new List<Vector3>();

        foreach(Vector3 point in m_Polytool.ControlPoints)
        {
            controlPoints.Add(point);
        }

        List<Vector3> insidePoints = new List<Vector3>();

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
            }

            insidePoints.Add(pos);
        }

        m_Holes = new List<IList<Vector3>>();
        m_Holes.Add(insidePoints);

        float scale = m_Width - 1;

        Vector3 transformPoint = ProMaths.Average(insidePoints);

        for (int i = 0; i < insidePoints.Count; i++)
        {
            Vector3 point = insidePoints[i] - transformPoint;
            Vector3 v = (point * scale) + transformPoint;
            insidePoints[i] = point + v;
        }

        ProBuilderMesh proBuilderMesh = ProBuilderMesh.Create();
        proBuilderMesh.CreateShapeFromPolygon(controlPoints, m_Height, false, m_Holes);

        proBuilderMesh.GetComponent<Renderer>().sharedMaterial = m_Material;
        proBuilderMesh.transform.SetParent(transform, true);

        proBuilderMesh.ToMesh();
        proBuilderMesh.Refresh();

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
