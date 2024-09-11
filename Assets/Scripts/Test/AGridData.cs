using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGridData : Polygon3DAData
{
    [SerializeField] Vector2Int m_Dimensions;
    [SerializeField] Vector2 m_GridScale;

    public Vector2Int Dimensions { get => m_Dimensions; set => m_Dimensions = value; }
    public Vector2 Scale { get => m_GridScale; set => m_GridScale = value; }

    public AGridData() :base()
    {
        m_Dimensions = Vector2Int.one;
        m_GridScale = Vector2.one * 0.9f;
    }

}
