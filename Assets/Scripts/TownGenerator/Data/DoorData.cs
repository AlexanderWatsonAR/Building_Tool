using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DoorData
{
    // Door
    [SerializeField, HideInInspector] private Vector3[] m_ControlPoints;
    [SerializeField, HideInInspector] private Vector3 m_Forward;
    [SerializeField, HideInInspector] private Vector3 m_Right;
    [SerializeField, Range(0, 0.999f)] private float m_Depth;
    [SerializeField, Range(0, 0.999f)] private float m_Scale;
    [SerializeField, HideInInspector] private Vector3 m_CentrePosition;
    [SerializeField] private TransformPoint m_HingePoint;
    [SerializeField] private Vector3 m_HingePosition;
    [SerializeField] private Vector3 m_HingeOffset;
    [SerializeField] private Vector3 m_HingeEulerAngles;
    [SerializeField, HideInInspector] private float m_Height;
    [SerializeField, HideInInspector] private float m_Width;
    [SerializeField] private Material m_Material;

    public Vector3[] ControlPoints => m_ControlPoints;
    public Vector3 Forward => m_Forward;
    public Vector3 Right => m_Right;
    public float Depth => m_Depth;
    public float Scale => m_Scale;
    public float Height => m_Height;
    public float Width => m_Width;
    public Vector3 Centre => m_CentrePosition;

    public TransformPoint HingePoint
    {
        get
        {
            return m_HingePoint;
        }

        set
        {
            if (m_HingePoint != value)
            {
                m_HingeOffset = Vector3.zero;
            }

            m_HingePoint = value;

            switch (m_HingePoint)
            {
                case TransformPoint.Middle:
                    m_HingePosition = m_CentrePosition;
                    break;
                case TransformPoint.Top:
                    m_HingePosition = m_CentrePosition + (Vector3.up * m_Height * 0.5f);
                    break;
                case TransformPoint.Bottom:
                    m_HingePosition = m_CentrePosition - (Vector3.up * m_Height * 0.5f);
                    break;
                case TransformPoint.Left:
                    m_HingePosition = m_CentrePosition - (m_Right * m_Width * 0.5f);
                    break;
                case TransformPoint.Right:
                    m_HingePosition = m_CentrePosition + (m_Right * m_Width * 0.5f);
                    break;
            }
        }
    }

    public Vector3 HingePosition => m_HingePosition;
    public Vector3 HingeOffset => m_HingeOffset;
    public Vector3 HingeEulerAngles => m_HingeEulerAngles;
    public Material Material => m_Material;

    public DoorData() : this(new Vector3[0], Vector3.zero, Vector3.zero, 0.2f, 0.9f, TransformPoint.Left, Vector3.zero, Vector3.zero, null)
    {

    }

    public DoorData(DoorData data) : this
    (
        data.ControlPoints,
        data.Forward,
        data.Right,
        data.Depth,
        data.Scale,
        data.HingePoint,
        data.HingeOffset,
        data.HingeEulerAngles,
        data.Material
    )
    {
    }

    public DoorData(IEnumerable<Vector3> controlPoints, Vector3 forward, Vector3 right, float depth, float scale, TransformPoint hingePoint, Vector3 hingeOffset, Vector3 hingeEulerAngles, Material material)
    {
        SetControlPoints(controlPoints);
        m_Forward = forward;
        m_Right = right;
        m_Depth = depth;
        m_Scale = scale;

        HingePoint = hingePoint;
        m_HingeOffset = hingeOffset;
        m_HingeEulerAngles = hingeEulerAngles;
        m_Material = material;
    }

    public void SetControlPoints(IEnumerable<Vector3> controlPoints)
    {
        if (controlPoints == null)
            return;

        if (controlPoints.Count() == 0)
            return;

        m_ControlPoints = controlPoints.ToArray();
        m_CentrePosition = UnityEngine.ProBuilder.Math.Average(m_ControlPoints);

        Vector3 min, max;
        Extensions.MinMax(m_ControlPoints, out min, out max);
        m_Height = max.y - min.y;
        m_Width = max.x - min.x + (max.z - min.z);
    }
}
