using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[System.Serializable]
public class DoorData : Polygon3DData
{
    [SerializeField] int m_ID;
    [SerializeField] DoorElement m_ActiveElements;

    // Door
    [SerializeField] TransformData m_HingeData;

    #region Handle
    [SerializeField] float m_HandleSize;
    [SerializeField, Range(0, 1)] float m_HandleScale;
    [SerializeField] Vector3 m_HandlePosition;
    [SerializeField] RelativePosition m_HandlePoint;
    #endregion

    [SerializeField] Material m_Material;

    #region Accessors
    public int ID { get { return m_ID; } set { m_ID = value; } }
    public DoorElement ActiveElements { get { return m_ActiveElements; } set { m_ActiveElements = value; } }
    public float HandleSize { get { return m_HandleSize; } set{ m_HandleSize = value; } }
    public float HandleScale { get { return m_HandleScale; } set{ m_HandleScale = value; } }
    public TransformData HingeData { get { return m_HingeData; } set { m_HingeData = value; } }
    public Vector3 HandlePosition => m_HandlePosition;
    public Material Material { get { return m_Material; } set { m_Material = value; } }
    #endregion

    // I don't care for these empty constructors that use dummy data.
    public DoorData() : this
    (
        DoorElement.Everything, null, null, Vector3.forward,
        Vector3.right, 1, 1, 1, Vector3.zero, null, null
    )
    {
    }

    public DoorData(DoorData data) : this
    (
        data.ActiveElements,
        data.Polygon,
        data.Holes,
        data.Normal,
        data.Up,
        data.Height,
        data.Width,
        data.Depth,
        data.Position,
        data.HingeData,
        data.Material
    )
    {
    }

    public DoorData(DoorElement activeElements, PolygonData polygon, PolygonData[] holes, Vector3 normal,
        Vector3 up, float height, float width, float depth, Vector3 position,TransformData hingeData, Material material) : base (polygon, holes, normal, up, height, width, depth, position)
    {
        m_ActiveElements = activeElements;
        m_HingeData = hingeData;
        m_Material = material;
    }

    public new object Clone()
    {
        DoorData copy = base.Clone() as DoorData;
        copy.HingeData = new TransformData(HingeData);
        return copy;
    }
}
