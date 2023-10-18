using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

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

    // Handle
    [SerializeField] private float m_HandleSize;
    [SerializeField, Range(0, 1)] private float m_HandleScale;
    [SerializeField] private Vector3 m_HandlePosition;
    [SerializeField] private TransformPoint m_HandlePoint;

    [SerializeField] private Material m_Material;

    public Vector3[] ControlPoints{ get{ return m_ControlPoints;} set{ SetControlPoints(value); }}
    public Vector3 Forward { get { return m_Forward; } set { m_Forward = value; } }
    public Vector3 Right { get { return m_Right; } set { m_Right = value; } }
    public float Depth { get { return m_Depth; } set { m_Depth = value; } }
    public float Scale { get { return m_Scale; } set { m_Scale = value; } }
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

            m_HingePosition = TransformPointToPosition(m_HingePoint);

            //switch (m_HingePoint)
            //{
            //    case TransformPoint.Middle:
            //        m_HingePosition = m_CentrePosition;
            //        break;
            //    case TransformPoint.Top:
            //        m_HingePosition = m_CentrePosition + (Vector3.up * m_Height * 0.5f);
            //        break;
            //    case TransformPoint.Bottom:
            //        m_HingePosition = m_CentrePosition - (Vector3.up * m_Height * 0.5f);
            //        break;
            //    case TransformPoint.Left:
            //        m_HingePosition = m_CentrePosition - (m_Right * m_Width * 0.5f);
            //        break;
            //    case TransformPoint.Right:
            //        m_HingePosition = m_CentrePosition + (m_Right * m_Width * 0.5f);
            //        break;
            //}
        }
    }

    

    public Vector3 HingePosition => m_HingePosition;
    public Vector3 HingeOffset => m_HingeOffset;
    public Vector3 HingeEulerAngles { get { return m_HingeEulerAngles; } set { m_HingeEulerAngles = value; } }

    public float HandleSize { get { return m_HandleSize; } set{ m_HandleSize = value; } }
    public float HandleScale { get { return m_HandleScale; } set{ m_HandleScale = value; } }
    public TransformPoint HandlePoint
    {
        get
        {
            return m_HandlePoint;
        }
        set
        {
            m_HandlePoint = value;
            m_HandlePosition = TransformPointToPosition(m_HandlePoint);

            m_HandlePosition -= m_CentrePosition;
            m_HandlePosition = Vector3.Scale(m_HandlePosition, Vector3.one * m_Scale) + m_CentrePosition;
            m_HandlePosition += m_Forward * m_Depth;

            float size = m_HandleSize * m_HandleScale;

            switch(m_HandlePoint)
            {
                case TransformPoint.Middle:
                    break;
                case TransformPoint.Top:
                    m_HandlePosition +=  Vector3.up * -size;
                    break;
                case TransformPoint.Bottom:
                    m_HandlePosition += Vector3.up * size;
                    break;
                case TransformPoint.Left:
                    m_HandlePosition += m_Right * size;
                    break;
                case TransformPoint.Right:
                    m_HandlePosition += m_Right * -size;
                    break;
            }
        }
    }
    public Vector3 HandlePosition => m_HandlePosition;
    public Material Material { get { return m_Material; } set { m_Material = value; } }

    public DoorData() : this(new Vector3[0], Vector3.zero, Vector3.zero, 0.2f, 0.9f, TransformPoint.Left, Vector3.zero, Vector3.zero, 0.2f, 1, TransformPoint.Right, null)
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
        data.HandleSize,
        data.HandleScale,
        data.HandlePoint,
        data.Material
    )
    {
    }

    public DoorData(IEnumerable<Vector3> controlPoints, Vector3 forward, Vector3 right, float depth, float scale, TransformPoint hingePoint, Vector3 hingeOffset, Vector3 hingeEulerAngles, float handleSize, float handleScale, TransformPoint handlePoint, Material material)
    {
        SetControlPoints(controlPoints);
        m_Forward = forward;
        m_Right = right;
        m_Depth = depth;
        m_Scale = scale;

        HingePoint = hingePoint;
        m_HingeOffset = hingeOffset;
        m_HingeEulerAngles = hingeEulerAngles;

        m_HandleSize = handleSize;
        m_HandleScale = handleScale;
        m_HandlePoint = handlePoint;

        m_Material = material;
    }

    private void SetControlPoints(IEnumerable<Vector3> controlPoints)
    {
        if (controlPoints == null)
            return;

        if (controlPoints.Count() == 0)
            return;

        m_ControlPoints = controlPoints.ToArray();

        Vector3 min, max;
        Extensions.MinMax(m_ControlPoints, out min, out max);
        m_Height = max.y - min.y;
        m_Width = max.x - min.x + (max.z - min.z);

        m_CentrePosition = Vector3.Lerp(min, max, 0.5f);
        m_HandleSize = Height * m_Scale * 0.05f;
        SetHandlePosition();
        
    }

    private void SetHandlePosition()
    {
        HandlePoint = m_HandlePoint;
    }

    private Vector3 TransformPointToPosition(TransformPoint transformPoint)
    {
        Vector3 position = m_CentrePosition;

        switch (transformPoint)
        {
            case TransformPoint.Middle:
                position = m_CentrePosition;
                break;
            case TransformPoint.Top:
                position = m_CentrePosition + (Vector3.up * m_Height * 0.5f);
                break;
            case TransformPoint.Bottom:
                position = m_CentrePosition - (Vector3.up * m_Height * 0.5f);
                break;
            case TransformPoint.Left:
                position = m_CentrePosition - (m_Right * m_Width * 0.5f);
                break;
            case TransformPoint.Right:
                position = m_CentrePosition + (m_Right * m_Width * 0.5f);
                break;
        }

        return position;
    }
}
