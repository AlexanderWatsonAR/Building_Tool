using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoofData: IData
{
    [SerializeField, HideInInspector] private ControlPoint[] m_ControlPoints;
    [SerializeField, HideInInspector] private ControlPoint[] m_OriginalControlPoints; // Could possible get rid of this?
    [SerializeField] private bool m_IsActive;
    [SerializeField] private RoofType m_RoofType;

    [SerializeField] private RoofTileData m_RoofTileData;

    [SerializeField, Range(-10, 10)] private float m_MansardHeight;
    [SerializeField, Range(0, 2)] private float m_MansardScale;
    [SerializeField, Range(-10, 10)] private float m_PyramidHeight;
    [SerializeField, Range(-10, 10)] private float m_GableHeight;
    [SerializeField, Range(0, 1)] private float m_GableScale;
    [SerializeField] private bool m_IsFlipped; // For M shaped
    [SerializeField] private bool m_IsOpen;

    public ControlPoint[] OriginalControlPoints => m_OriginalControlPoints;
    public ControlPoint[] ControlPoints { get { return m_ControlPoints; } set { m_ControlPoints = value; } }
    public RoofTileData TileData { get { return m_RoofTileData; } set { m_RoofTileData = value; } }
    public float MansardScale => m_MansardScale;
    public float MansardHeight => m_MansardHeight;
    public float PyramidHeight => m_PyramidHeight;
    public float GableHeight => m_GableHeight;
    public bool IsFlipped => m_IsFlipped;
    public bool IsOpen => m_IsOpen;
    public bool IsActive => m_IsActive;
    public RoofType RoofType => m_RoofType;
    public float GableScale => m_GableScale;


    public RoofData() : this (new ControlPoint[0], new RoofTileData(), RoofType.Mansard, 1, 1, 1, 1, 0.75f, false, false, true )
    {

    }
    public RoofData(RoofData data) : this (data.ControlPoints, data.TileData, data.RoofType, data.MansardHeight, data.MansardScale, data.PyramidHeight, data.GableHeight, data.GableScale, data.IsOpen, data.IsFlipped, data.IsActive )
    {

    }
    public RoofData(ControlPoint[] controlPoints, RoofTileData roofTileData, RoofType type, float mansardHeight, float mansardScale, float pyramidHeight, float gableHeight, float gableScale, bool isOpen, bool isFlipped, bool isActive)
    {
        m_ControlPoints = controlPoints == null ? new ControlPoint[0] : controlPoints;
        m_OriginalControlPoints = m_ControlPoints;
        m_RoofTileData = roofTileData;
        m_RoofType = type;
        m_MansardHeight = mansardHeight;
        m_MansardScale = mansardScale;
        m_PyramidHeight = pyramidHeight;
        m_GableHeight = gableHeight;
        m_GableScale = gableScale;
        m_IsFlipped = isFlipped;
        m_IsOpen = isOpen;
        m_IsActive = isActive;
    }

    public bool IsGable
    {
        get
        {
            return m_RoofType == RoofType.Gable || m_RoofType == RoofType.Dormer || m_RoofType == RoofType.MShaped;
        }
        
    }
}
