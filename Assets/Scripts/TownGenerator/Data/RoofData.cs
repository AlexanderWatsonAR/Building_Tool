using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class RoofData : DirtyData
{
    #region Member Variable
    [SerializeField, HideInInspector] private ControlPoint[] m_ControlPoints;
    [SerializeField] private bool m_IsActive; // delete?
    [SerializeField] private RoofType m_RoofType;

    [SerializeField] private RoofTileData m_RoofTileData;

    // Notes
    // Class for grouping together common sets of roof data values
    // What would we call the classes?
    // We could rename this class 'variable roof data' and make a seperate abstract roof class.
    // The other roof class would contain generic roof data. 
    // E.G. roof tile array, height. Then create seperate classes that inherit from roof.
    // MansardData : RoofData

    [SerializeField] Gable m_Gable;
    [SerializeField] Mansard m_Mansard;
    [SerializeField] RoofShapeData m_Pyramid;

    [SerializeField, HideInInspector] private RoofTileData[] m_PyramidRoofTiles;
    [SerializeField, HideInInspector] private RoofTileData[] m_GableRoofTiles;
    [SerializeField, HideInInspector] private RoofTileData[] m_MansardRoofTiles;
    [SerializeField, HideInInspector] private WallData[] m_Walls;

    [SerializeField, Range(-10, 10)] private float m_MansardHeight;
    [SerializeField, Range(0, 1)] private float m_MansardScale;
    [SerializeField, Range(-10, 10)] private float m_PyramidHeight;
    [SerializeField, Range(-10, 10)] private float m_GableHeight;
    [SerializeField, Range(0, 1)] private float m_GableScale;
    [SerializeField] private bool m_IsFlipped; // For M shaped
    [SerializeField] private bool m_IsOpen;

    [SerializeField, HideInInspector] private OneLineShape m_OneLineShape;
    [SerializeField, HideInInspector] private int m_ShapeIndex; // Relative start index used for the gable data indices.
    [SerializeField, HideInInspector] private Vector3[] m_OneLine;
    [SerializeField, HideInInspector] private ControlPoint[] m_PathPoints;
    [SerializeField, HideInInspector] private GableData m_GableData;
    [SerializeField, HideInInspector] private int[] m_AvailableFrames;
    #endregion

    #region Accessors
    public OneLineShape OneLineShape { get { return m_OneLineShape; } set { m_OneLineShape = value; } }
    public int ShapeIndex => m_ShapeIndex;
    public Vector3[] OneLine => m_OneLine;
    public ControlPoint[] ScaledControlPoints => ScaleControlPoints();
    public ControlPoint[] ControlPoints { get { return m_ControlPoints; } set { m_ControlPoints = value; } }
    public RoofTileData[] PyramidTiles { get { return m_PyramidRoofTiles; } set { m_PyramidRoofTiles = value; } }
    public RoofTileData[] GableTiles { get { return m_GableRoofTiles; } set { m_GableRoofTiles = value; } }
    public RoofTileData[] MansardTiles { get { return m_MansardRoofTiles; } set { m_MansardRoofTiles = value; } }
    public RoofTileData TileData { get { return m_RoofTileData; } set { m_RoofTileData = value; } }
    public ControlPoint[] PathPoints => m_PathPoints;
    public float MansardScale => m_MansardScale;
    public float MansardHeight => m_MansardHeight;
    public float PyramidHeight => m_PyramidHeight;
    public float GableHeight => m_GableHeight;
    public bool IsFlipped => m_IsFlipped;
    public bool IsOpen { get { return m_IsOpen; } set { m_IsOpen = value; } }
    public bool IsActive => m_IsActive;
    public RoofType RoofType { get { return m_RoofType; } set { m_RoofType = value; } }
    public float GableScale => m_GableScale;
    public GableData GableData => m_GableData;
    public WallData[] Walls { get { return m_Walls; } set { m_Walls = value; } }
    public bool IsGable
    {
        get
        {
            return m_RoofType == RoofType.Gable | m_RoofType == RoofType.Dormer | m_RoofType == RoofType.MShaped;
        }

    }
    public bool IsHip
    {
        get
        {
            return m_RoofType == RoofType.Dormer | m_RoofType == RoofType.PyramidHip;
        }
    }
    public int[] AvailableFrames
    {
        get
        {
            if (m_AvailableFrames == null || m_AvailableFrames.Length == 0)
                m_AvailableFrames = AvailableRoofFrames();

            return m_AvailableFrames;
        }
    }
    #endregion 

    public RoofData() : this (new ControlPoint[0], new RoofTileData(), null, null, null, RoofType.Mansard, 1, 0.2f, 1, 1, 0.75f, false, false, true )
    {

    }
    public RoofData(RoofData data) : this (data.ControlPoints, data.TileData, data.MansardTiles, data.PyramidTiles, data.GableTiles, data.RoofType, data.MansardHeight, data.MansardScale, data.PyramidHeight, data.GableHeight, data.GableScale, data.IsOpen, data.IsFlipped, data.IsActive )
    {

    }
    public RoofData(ControlPoint[] controlPoints, RoofTileData roofTileData, RoofTileData[] mansardTiles, RoofTileData[] pyramidTiles, RoofTileData[] gableTiles, RoofType type, float mansardHeight, float mansardScale, float pyramidHeight, float gableHeight, float gableScale, bool isOpen, bool isFlipped, bool isActive)
    {
        m_ControlPoints = controlPoints == null ? new ControlPoint[0] : controlPoints;
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
        m_MansardRoofTiles = mansardTiles;
        m_PyramidRoofTiles = pyramidTiles;
        m_GableRoofTiles = gableTiles;
    }

    /// <summary>
    /// returns control points modified by the mansard scale & height.
    /// if the current roof type is not equal to Dormer or Pyramid hip it will returnt the original cps.
    /// </summary>
    /// <returns></returns>
    private ControlPoint[] ScaleControlPoints()
    {
        ControlPoint[] scaledControlPoints = PolygonRecognition.Clone(m_ControlPoints);

        if(IsHip | m_RoofType == RoofType.Mansard)
        {
            scaledControlPoints = scaledControlPoints.ScalePolygon(m_MansardScale, true);
        }

        for(int i = 0; i < scaledControlPoints.Length; i++)
        {
            scaledControlPoints[i] += Vector3.up * m_MansardHeight;
        }

        return scaledControlPoints;
    }

    private int[] AvailableRoofFrames()
    {
        if (m_ControlPoints == null | m_ControlPoints.Length == 0)
            return new int[0];

        int Gable = (int)RoofType.Gable;
        int Mansard = (int)RoofType.Mansard;
        int Dormer = (int)RoofType.Dormer;
        int MShaped = (int)RoofType.MShaped;
        int Pyramid = (int)RoofType.Pyramid;
        int PyramidHip = (int)RoofType.PyramidHip;

        m_PathPoints = m_ControlPoints.FindPathPoints().ToArray();

        if (m_PathPoints.Count() == 4)
        {
            m_ControlPoints.IsDescribableInOneLine(out m_OneLine, out m_OneLineShape, out m_ShapeIndex);
            
            if(!m_OneLineShape.GetGableData(out m_GableData))
            {
                m_OneLineShape.CalculateGableData(m_ControlPoints.GetPositions(), m_OneLine, out m_GableData);
            }

            return new int[] { Gable, Mansard, Dormer, MShaped, Pyramid, PyramidHip };    
        }

        if (m_ControlPoints.IsDescribableInOneLine(out m_OneLine, out m_OneLineShape, out m_ShapeIndex))
        {
            if (!m_OneLineShape.GetGableData(out m_GableData))
            {
                m_OneLineShape.CalculateGableData(m_ControlPoints.GetPositions(), m_OneLine, out m_GableData);
            }

            if (m_ControlPoints.IsPointInside(m_ControlPoints.Centre()))
            {
                return new int[] { Gable, Mansard, Dormer, Pyramid, PyramidHip };
            }
            return new int[] { Gable, Mansard, Dormer };
        }

        if (m_ControlPoints.IsPointInside(m_ControlPoints.Centre()))
        {
            return new int[] { Mansard, Pyramid, PyramidHip };
        }

        return new int[] { Mansard };
    }

    public WallData GetWallByID(int ID)
    {
        for(int i = 0; i < m_Walls.Length; i++)
        {
            if (m_Walls[i].ID == ID)
                return m_Walls[i];
        }
        return null;
    }

    public bool IsConvex
    {
        get
        {
            return !m_ControlPoints.IsConcave(out _);
        }
    }
}
