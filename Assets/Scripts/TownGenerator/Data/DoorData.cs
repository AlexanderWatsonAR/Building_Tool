using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class DoorData : Polygon3DData
{
    [SerializeField] private int m_ID;
    [SerializeField] private DoorElement m_ActiveElements;

    // Door
    [SerializeField, HideInInspector] private Vector3 m_Right;
    [SerializeField, Range(0, 0.999f)] private float m_Scale;
    [SerializeField] private TransformPoint m_HingePoint;
    [SerializeField] private Vector3 m_HingePosition;
    [SerializeField] private Vector3 m_HingeOffset;
    [SerializeField] private Vector3 m_HingeEulerAngles;

    #region Handle
    [SerializeField] private float m_HandleSize;
    [SerializeField, Range(0, 1)] private float m_HandleScale;
    [SerializeField] private Vector3 m_HandlePosition;
    [SerializeField] private TransformPoint m_HandlePoint;
    #endregion

    [SerializeField] private Material m_Material;

    #region Accessors
    public int ID { get { return m_ID; } set { m_ID = value; } }
    public DoorElement ActiveElements { get { return m_ActiveElements; } set { m_ActiveElements = value; } }
    public Vector3 Right { get { return m_Right; } set { m_Right = value; } }
    public float Scale { get { return m_Scale; } set { m_Scale = value; } }
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
        }
    }
    public Vector3 HingePosition => m_HingePosition;
    public Vector3 HingeOffset { get { return m_HingeOffset; } set { m_HingeOffset = value; } }
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

            m_HandlePosition -= Position;
            m_HandlePosition = Vector3.Scale(m_HandlePosition, Vector3.one * m_Scale) + Position;
            m_HandlePosition += Normal * Depth;

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
    #endregion

    // I don't care for these empty constructors that use dummy data.
    public DoorData() : this
    (
        DoorElement.Everything, null, null, Vector3.forward,
        Vector3.right, 1, 1, 1, 1, Vector3.zero, TransformPoint.Left,
        Vector3.zero, Vector3.zero, 0.2f, 1, TransformPoint.Right, null
    )
    {
    }

    public DoorData(DoorData data) : this
    (
        data.ActiveElements,
        data.ControlPoints,
        data.HolePoints,
        data.Normal,
        data.Right,
        data.Height,
        data.Width,
        data.Depth,
        data.Scale,
        data.Position,
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

    public DoorData(DoorElement activeElements, Vector3[] controlPoints, Vector3[][] holePoints, Vector3 normal,
        Vector3 right, float height, float width, float depth, float scale, Vector3 position,
        TransformPoint hingePoint, Vector3 hingeOffset, Vector3 hingeEulerAngles,
        float handleSize, float handleScale, TransformPoint handlePoint, Material material) : base (controlPoints, holePoints, normal, height, width, depth, position)
    {
        m_ActiveElements = activeElements;
        m_Right = right;
        m_Scale = scale;
        HingePoint = hingePoint;
        m_HingeOffset = hingeOffset;
        m_HingeEulerAngles = hingeEulerAngles;
        m_HandleSize = handleSize;
        m_HandleScale = handleScale;
        m_HandlePoint = handlePoint;
        m_Material = material;
    }

    private Vector3 TransformPointToPosition(TransformPoint transformPoint)
    {
        Vector3 position = Position;

        switch (transformPoint)
        {
            case TransformPoint.Middle:
                position = Position;
                break;
            case TransformPoint.Top:
                position = Position + (Vector3.up * Height * 0.5f);
                break;
            case TransformPoint.Bottom:
                position = Position - (Vector3.up * Height * 0.5f);
                break;
            case TransformPoint.Left:
                position = Position - (m_Right * Width * 0.5f);
                break;
            case TransformPoint.Right:
                position = Position + (m_Right * Width * 0.5f);
                break;
        }

        return position;
    }
}
