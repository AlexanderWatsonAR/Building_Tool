using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class RoofTile : MonoBehaviour
{
    [SerializeField, Range(0, 10)] private float m_Height;
    [SerializeField, Range(0, 10)] private float m_Extend;
    [SerializeField] private bool m_FlipFace, m_ExtendHeightBeginning, m_ExtendHeightEnd, m_ExtendWidthBeginning, m_ExtendWidthEnd;
    [SerializeField] private Material m_Material;

    [SerializeField, HideInInspector] Vector3[] m_ControlPoints;
    [SerializeField, HideInInspector] Vector3[] m_ExtendedPositions;
    [SerializeField, HideInInspector] ProBuilderMesh m_ProBuilderMesh;

    // Control point indices
    private static readonly int m_BottomLeft = 0;
    private static readonly int m_TopLeft = 1;
    private static readonly int m_TopRight = 2;
    private static readonly int m_BottomRight = 3;

    public void SetControlPoints(IEnumerable<Vector3> controlPoints)
    {
        m_ControlPoints = controlPoints.ToArray();
        m_ExtendedPositions = m_ControlPoints.Clone() as Vector3[];
    }

    private void Reset()
    {
        Initialize(0.25f, 0.25f);
    }

    public RoofTile Initialize(float height, float extend)
    {
        m_Height = height;
        m_Extend = extend;
        m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
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
        ProBuilderMesh mesh = MeshMaker.CubeProjection(m_ExtendedPositions, m_Height);

        m_ProBuilderMesh.RebuildWithPositionsAndFaces(mesh.positions, mesh.faces);
        m_ProBuilderMesh.ToMesh();
        m_ProBuilderMesh.Refresh();
        DestroyImmediate(mesh.gameObject);

        return this;
    }

}
