using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExtensionData : OpeningData
{
    [SerializeField, Range(1, 10)] float m_Distance;

    public float Distance { get { return m_Distance; } set { m_Distance = value; } }

    public ExtensionData() : this (4)
    {

    }
    public ExtensionData(float distance) : base()
    {
        m_Distance = distance;
    }

    public ExtensionData(float height, float width, int columns, int rows, float distance) : base (height, width, columns, rows)
    {
        m_Distance = distance;
    }

    public ExtensionData(ExtensionData data) : this (data.Height, data.Width, data.Columns, data.Rows, data.Distance)
    {

    }

}
