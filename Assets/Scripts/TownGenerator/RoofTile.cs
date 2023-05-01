using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.ProBuilder;

public class RoofTile : MonoBehaviour
{
    [SerializeField, Range(0, 10)] private float m_Height;
    [SerializeField, Range(0, 10)] private float m_Extend;
    [SerializeField] private bool m_FlipFace, m_ExtendHeightBeginning, m_ExtendHeightEnd, m_ExtendWidthBeginning, m_ExtendWidthEnd;
    [SerializeField] private Material m_Material;

    [SerializeField, HideInInspector] Vector3[] m_ControlPoints;

    [SerializeField, HideInInspector] Vector3[] m_ExtendedControlPoints;

    [SerializeField, HideInInspector] ProBuilderMesh m_ProBuilderMesh;

    // Control point indices
    private static readonly int m_BottomLeft = 0;
    private static readonly int m_TopLeft = 1;
    private static readonly int m_TopRight = 2;
    private static readonly int m_BottomRight = 3;

    public void SetControlPoints(IEnumerable<Vector3> controlPoints)
    {
        m_ControlPoints = controlPoints.ToArray();
        m_ExtendedControlPoints = m_ControlPoints;
    }

    private void Reset()
    {
        Initialize();
    }

    public RoofTile Initialize()
    {
        m_Height = 0.2f;
        m_Extend = 0.23f;
        return this;
    }

    public RoofTile Extend(bool heightBeginning, bool heightEnd, bool widthBeginning, bool widthEnd)
    {
        m_ExtendHeightBeginning = heightBeginning;
        m_ExtendHeightEnd = heightEnd;
        m_ExtendWidthBeginning = widthBeginning;
        m_ExtendWidthEnd = widthEnd;

        m_ExtendedControlPoints = m_ControlPoints;

        if (!heightBeginning && !heightEnd && !widthBeginning && !widthEnd)
            return this;

        Vector3 topLeftToBottomLeft = m_ControlPoints[m_TopLeft].GetDirectionToTarget(m_ControlPoints[m_BottomLeft]);
        Vector3 topRightToBottomRight = m_ControlPoints[m_TopRight].GetDirectionToTarget(m_ControlPoints[m_BottomRight]);

        Vector3 topLeftToTopRight = m_ControlPoints[m_TopLeft].GetDirectionToTarget(m_ControlPoints[m_TopRight]);
        Vector3 bottomLeftToBottomRight = m_ControlPoints[m_BottomLeft].GetDirectionToTarget(m_ControlPoints[m_BottomRight]);

        if (heightBeginning)
        {
            m_ExtendedControlPoints[m_TopLeft] += -topLeftToBottomLeft * m_Extend;
            m_ExtendedControlPoints[m_TopRight] += -topRightToBottomRight * m_Extend;
        }

        if (heightEnd)
        {
            m_ExtendedControlPoints[m_BottomLeft] += topLeftToBottomLeft * m_Extend;
            m_ExtendedControlPoints[m_BottomRight] += topRightToBottomRight * m_Extend;
        }

        if (widthBeginning)
        {
            m_ExtendedControlPoints[m_TopLeft] += -topLeftToTopRight * m_Extend;
            m_ExtendedControlPoints[m_BottomLeft] += -bottomLeftToBottomRight * m_Extend;
        }

        if (widthEnd)
        {
            m_ExtendedControlPoints[m_TopRight] += topLeftToTopRight * m_Extend;
            m_ExtendedControlPoints[m_BottomRight] += bottomLeftToBottomRight * m_Extend;
        }
        return this;
    }

    public RoofTile Build()
    {
        m_ProBuilderMesh = MeshMaker.CubeProjection(m_ExtendedControlPoints, m_Height);

        return this;
    }

}
