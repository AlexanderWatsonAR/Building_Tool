using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoofSectionData : IData
{
    [SerializeField] private RoofElement m_RoofElement;
    [SerializeField, HideInInspector] private Vector3[] m_ControlPoints;
    [SerializeField, HideInInspector] private Vector3[] m_TopPoints;
    [SerializeField, HideInInspector] private float m_SectionHeight;

    #region Window Hole
    [SerializeField, Range(0, 0.999f)] private float m_WindowHeight;
    [SerializeField, Range(0, 0.999f)] private float m_WindowWidth;
    [SerializeField, Range(3, 32)] private int m_WindowSides = 3;
    [SerializeField, Range(1, 10)] private int m_WindowColumns, m_WindowRows;
    #endregion

    [SerializeField] private WindowData m_WindowData;

    public Vector3[] ControlPoints { get { return m_ControlPoints; } set { m_ControlPoints = value; } }
    public Vector3[] TopPoints { get { return m_TopPoints; } set { m_TopPoints = value; } }
    public float SectionHeight { get { return m_SectionHeight; } set { m_SectionHeight = value; } }

    public WindowData WindowData => m_WindowData;
    public RoofElement RoofElement => m_RoofElement;
    public float WindowHeight => m_WindowHeight;
    public float WindowWidth => m_WindowWidth;
    public int WindowSides => m_WindowSides;
    public int WindowColumns => m_WindowColumns;
    public int WindowRows => m_WindowRows;


    public RoofSectionData() : this(new WindowData(), RoofElement.Tile, new Vector3[0], new Vector3[0], 0.25f, 0.5f, 0.5f, 4, 1, 1)
    {

    }

    public RoofSectionData(RoofSectionData data) : this(data.WindowData, data.RoofElement, data.ControlPoints, data.TopPoints, data.SectionHeight, data.WindowHeight, data.WindowWidth, data.WindowSides, data.WindowColumns, data.WindowRows)
    {

    }

    public RoofSectionData(WindowData windowData, RoofElement roofElement, Vector3[] controlPoints, Vector3[] topPoints, float sectionHeight, float windowHeight, float windowWidth, int windowSides, int windowColumns, int windowRows)
    {
        m_WindowData = windowData;
        m_RoofElement = roofElement;
        m_ControlPoints = controlPoints == null? new Vector3[0] : controlPoints;
        m_TopPoints = topPoints == null? new Vector3[0] : topPoints;
        m_SectionHeight = sectionHeight;
        m_WindowHeight = windowHeight;
        m_WindowWidth = windowWidth;
        m_WindowSides = windowSides;
        m_WindowColumns = windowColumns;
        m_WindowRows = windowRows;

    }
}
