using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WindowData
{
    [SerializeField, Range(0, 0.999f)] private float m_Height;
    [SerializeField, Range(0, 0.999f)] private float m_Width;
    [SerializeField, Range(3, 32)] private int m_Sides = 3;
    [SerializeField, Range(1, 10)] private int m_Columns, m_Rows;
    [SerializeField, Range(-180, 180)] private float m_Angle;
    [SerializeField] private bool m_IsSmooth;
    [SerializeField] private bool m_IsActive;
    [SerializeField, Range(1, 10)] private int m_FrameColumns, m_FrameRows;
    [SerializeField] private Vector3 m_FrameScale;
    [SerializeField] private Material m_PaneMaterial;
    [SerializeField] private Material m_FrameMaterial;

    public float Height => m_Height;
    public float Width => m_Width;
    public int Sides => m_Sides;
    public int Columns => m_Columns;
    public int Rows => m_Rows;
    public float Angle => m_Angle;
    public bool IsSmooth => m_IsSmooth;
    public bool IsActive => m_IsActive;
    public int FrameColumns => m_FrameColumns;
    public int FrameRows => m_FrameRows;
    public Vector3 FrameScale => m_FrameScale;
    public Material FrameMaterial => m_FrameMaterial;
    public Material PaneMaterial => m_PaneMaterial;

    public WindowData() : this (0.5f, 0.5f, 3, 1, 1, 0, false, true, 2, 2, Vector3.one * 0.9f, null, null)
    {

    }
    public WindowData(WindowData data) : this (data.Height, data.Width, data.Sides, data.Columns, data.Rows, data.Angle, data.IsSmooth, data.IsActive, data.FrameColumns, data.FrameRows, data.FrameScale, data.PaneMaterial, data.FrameMaterial)
    {

    }
    public WindowData(float height, float width, int sides, int columns, int rows, float angle, bool isSmooth, bool isActive, int frameColumns, int frameRows, Vector3 frameScale, Material paneMaterial, Material frameMaterial)
    {
        m_Height = height;
        m_Width = width;
        m_Sides = sides;
        m_Columns = columns;
        m_Rows = rows;
        m_Angle = angle;
        m_IsSmooth = isSmooth;
        m_IsActive = isActive;
        m_FrameColumns = frameColumns;
        m_FrameRows = frameRows;
        m_FrameScale = frameScale;
        m_PaneMaterial = paneMaterial;
        m_FrameMaterial = frameMaterial;
    }
    public void SetFrameMaterial(Material material)
    {
        m_FrameMaterial = material;
    }
    public void SetPaneMaterial(Material material)
    {
        m_PaneMaterial = material;
    }
}
