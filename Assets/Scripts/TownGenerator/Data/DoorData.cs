using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DoorData
{
    // Doorway
    [SerializeField, Range(0, 0.999f)] private float m_PedimentHeight;
    [SerializeField, Range(0, 0.999f)] private float m_SideWidth;
    [SerializeField, Range(-0.999f, 0.999f)] private float m_SideOffset;
    [SerializeField, Range(1, 10)] private int m_Columns, m_Rows;
    [SerializeField] private float m_ArchHeight;
    [SerializeField] private int m_ArchSides;
    // Door
    [SerializeField] private bool m_IsActive;
    [SerializeField] private Vector3 m_Scale;
    [SerializeField] private TransformPoint m_HingePoint;
    [SerializeField] private Vector3 m_HingeOffset;
    [SerializeField] private Vector3 m_HingeEulerAngles;
    [SerializeField] private Material m_Material;
    // Door Frame
    [SerializeField] private float m_FrameDepth;
    [SerializeField] private float m_FrameInsideScale;

    public float PedimentHeight => m_PedimentHeight;
    public float SideWidth => m_SideWidth;
    public float SideOffset => m_SideOffset;
    public int Columns => m_Columns;
    public int Rows => m_Rows;
    public float ArchHeight => m_ArchHeight;
    public int ArchSides => m_ArchSides;
    public bool IsActive => m_IsActive;
    public Vector3 Scale => m_Scale;
    public TransformPoint HingePoint => m_HingePoint;
    public Vector3 HingeOffset => m_HingeOffset;
    public Vector3 HingeEulerAngles => m_HingeEulerAngles;
    public Material Material => m_Material;
    public float FrameDepth => m_FrameDepth;
    public float FrameInsideScale => m_FrameInsideScale;

    public DoorData() : this(0.75f, 0.5f, 0, 1, 1, 1, 3, true, Vector3.one * 0.9f, TransformPoint.Left, Vector3.zero, Vector3.zero, null, 1,1)
    {

    }

    public DoorData(DoorData data) : this
    (
        data.PedimentHeight,
        data.SideWidth,
        data.SideOffset,
        data.Columns,
        data.Rows,
        data.ArchHeight,
        data.ArchSides,
        data.IsActive,
        data.Scale,
        data.HingePoint,
        data.HingeOffset,
        data.HingeEulerAngles,
        data.Material,
        data.FrameDepth,
        data.FrameInsideScale
    )
    {
    }

    public DoorData(float pedimentHeight, float sideWidth, float sideOffset, int columns, int rows, float archHeight, int archSides, bool isActive, Vector3 scale, TransformPoint hingePoint, Vector3 hingeOffset, Vector3 hingeEulerAngles, Material material, float frameDepth, float frameInsideScale)
    {
        m_PedimentHeight = pedimentHeight;
        m_SideWidth = sideWidth;
        m_SideOffset = sideOffset;
        m_Columns = columns;
        m_Rows = rows;
        m_ArchHeight = archHeight;
        m_ArchSides = archSides;
        m_IsActive = isActive;
        m_Scale = scale;
        m_HingePoint = hingePoint;
        m_HingeOffset = hingeOffset;
        m_HingeEulerAngles = hingeEulerAngles;
        m_Material = material;
        m_FrameDepth = frameDepth;
        m_FrameInsideScale = frameInsideScale;
    }
}
