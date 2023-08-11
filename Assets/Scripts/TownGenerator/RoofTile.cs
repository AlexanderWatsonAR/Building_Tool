using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using Unity.VisualScripting;
using UnityEditor;

public class RoofTile : MonoBehaviour
{
    [SerializeField, Range(0, 10)] private float m_Height;
    [SerializeField, Range(0, 10)] private float m_Extend;
    [SerializeField] private bool m_FlipFace, m_ExtendHeightBeginning, m_ExtendHeightEnd, m_ExtendWidthBeginning, m_ExtendWidthEnd;
    [SerializeField] private Material m_Material;

    [SerializeField, HideInInspector] Vector3[] m_ControlPoints;
    [SerializeField, HideInInspector] Vector3[] m_ExtendedPositions;
    [SerializeField, HideInInspector] private List<Vector3[]> m_SubPoints;
    [SerializeField, Range(1, 10)] private int m_Columns, m_Rows;
    //[SerializeField, HideInInspector] ProBuilderMesh m_ProBuilderMesh;
    [SerializeField, HideInInspector] bool m_IsInside;

    // Control point indices
    private static readonly int m_BottomLeft = 0;
    private static readonly int m_TopLeft = 1;
    private static readonly int m_TopRight = 2;
    private static readonly int m_BottomRight = 3;

    public bool ExtendHeightBeginning => m_ExtendHeightBeginning;
    public bool ExtendHeightEnd => m_ExtendHeightEnd;
    public bool ExtendWidthBeginning => m_ExtendWidthBeginning;
    public bool ExtendWidthEnd => m_ExtendWidthEnd;

    private List<Vector3[]> SubPoints
    {
        get
        {
            if (m_Columns <= 0 && m_Rows <= 0) return null;

            m_SubPoints = MeshMaker.CreateGridFromControlPoints(m_ExtendedPositions, m_Columns, m_Rows);

            return m_SubPoints;
        }
    }

    public void SetControlPoints(IEnumerable<Vector3> controlPoints)
    {
        m_ControlPoints = controlPoints.ToArray();
        m_ExtendedPositions = m_ControlPoints.Clone() as Vector3[];
    }

    private void Reset()
    {
        Initialize(0.25f, 0.25f, true);
    }

    public void SetMaterial(Material material)
    {
        m_Material = material;
    }

    public RoofTile Initialize(float height, float extend, bool isInside = true)
    {
        m_Columns = 1;
        m_Rows = 1;
        m_Height = height;
        m_Extend = extend;
        m_IsInside = isInside;
        
        //m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
        return this;
    }

    public RoofTile Extend(bool heightBeginning = false, bool heightEnd = true, bool widthBeginning = true, bool widthEnd = true)
    {
        m_ExtendHeightBeginning = heightBeginning;
        m_ExtendHeightEnd = heightEnd;
        m_ExtendWidthBeginning = widthBeginning;
        m_ExtendWidthEnd = widthEnd;

        m_ExtendedPositions = m_ControlPoints.Clone() as Vector3[];

        if (!heightBeginning && !heightEnd && !widthBeginning && !widthEnd)
            return this;

        Vector3 topLeftToBottomLeft = m_ControlPoints[m_TopLeft].DirectionToTarget(m_ControlPoints[m_BottomLeft]);
        Vector3 topRightToBottomRight = m_ControlPoints[m_TopRight].DirectionToTarget(m_ControlPoints[m_BottomRight]);

        Vector3 topLeftToTopRight = m_ControlPoints[m_TopLeft].DirectionToTarget(m_ControlPoints[m_TopRight]);
        Vector3 bottomLeftToBottomRight = m_ControlPoints[m_BottomLeft].DirectionToTarget(m_ControlPoints[m_BottomRight]);

        if (m_ExtendHeightBeginning)
        {
            m_ExtendedPositions[m_TopLeft] += -topLeftToBottomLeft * m_Extend;
            m_ExtendedPositions[m_TopRight] += -topRightToBottomRight * m_Extend;
        }

        if (m_ExtendHeightEnd)
        {
            m_ExtendedPositions[m_BottomLeft] += topLeftToBottomLeft * m_Extend;
            m_ExtendedPositions[m_BottomRight] += topRightToBottomRight * m_Extend;
        }

        if (m_ExtendWidthBeginning)
        {
            m_ExtendedPositions[m_TopLeft] += -topLeftToTopRight * m_Extend;
            m_ExtendedPositions[m_BottomLeft] += -bottomLeftToBottomRight * m_Extend;
        }

        if (m_ExtendWidthEnd)
        {
            m_ExtendedPositions[m_TopRight] += topLeftToTopRight * m_Extend;
            m_ExtendedPositions[m_BottomRight] += bottomLeftToBottomRight * m_Extend;
        }
        return this;
    }

    public RoofTile Build()
    {
        List<Vector3[]> bottomPoints = SubPoints;

        Vector3[] projectedVerts = MeshMaker.ProjectedCubeVertices(m_ExtendedPositions, m_Height);
        Vector3 midPointA = Vector3.Lerp(m_ExtendedPositions[0], m_ExtendedPositions[1], 0.5f);
        Vector3 midPointB = Vector3.Lerp(projectedVerts[0], projectedVerts[1], 0.5f);
        float distance = Vector3.Distance(midPointA, midPointB);

        List<Vector3[]> topPoints = MeshMaker.CreateGridFromControlPoints(projectedVerts, m_Columns, m_Rows);

        transform.DeleteChildren();

        for (int i = 0; i < m_Columns; i++)
        {
            for (int j = 0; j < m_Rows; j++)
            {
                Vector3 bl = bottomPoints[j][i];
                Vector3 tl = bottomPoints[j + 1][i];
                Vector3 tr = bottomPoints[j + 1][i + 1];
                Vector3 br = bottomPoints[j][i + 1];

                Vector3[] controlPoints = new Vector3[] { bl, tl, tr, br };

                Vector3 topBL = topPoints[j][i];
                Vector3 topTL = topPoints[j + 1][i];
                Vector3 topTR = topPoints[j + 1][i + 1];
                Vector3 topBR = topPoints[j][i + 1];

                Vector3[] points = new Vector3[] { topBL, topTL, topTR, topBR };

                ProBuilderMesh roofSection = ProBuilderMesh.Create();
                roofSection.name = "Roof Section " + j.ToString() + " " + i.ToString();
                roofSection.GetComponent<Renderer>().sharedMaterial = m_Material;

                roofSection.transform.SetParent(transform, false);
                roofSection.AddComponent<RoofSection>().Initialize(controlPoints, distance).SetTopPoints(points).Build();
            }
        }

        return this;
    }

    private void OnDrawGizmosSelected()
    {
        if (m_ControlPoints == null)
            return;

        if (m_Columns == 0 || m_Rows == 0)
            return;

        List<Vector3[]> subPoints = SubPoints;

        if (m_Rows != 0 && m_Columns != 0)
        {
            for (int i = 0; i < m_Columns; i++)
            {
                for (int j = 0; j < m_Rows; j++)
                {
                    Vector3 bl = subPoints[j + 0][i + 0];
                    Vector3 tl = subPoints[j + 0][i + 1];
                    Vector3 tr = subPoints[j + 1][i + 1];
                    Vector3 br = subPoints[j + 1][i + 0];

                    Vector3 dir = bl.DirectionToTarget(br);
                    Vector3 cross = Vector3.Cross(bl.DirectionToTarget(tl), dir) * m_Height;

                    bl += cross;
                    tl += cross;
                    tr += cross;
                    br += cross;

                    bl += transform.position;
                    tl += transform.position;
                    tr += transform.position;
                    br += transform.position;

                    Handles.DrawAAPolyLine(bl, tl, tr, br);
                    Handles.DrawAAPolyLine(bl, br);

                }
            }
        }
    }

}
