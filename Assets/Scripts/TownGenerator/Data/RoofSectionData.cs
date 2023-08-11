using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoofSectionData
{
    [SerializeField] private WindowData m_WindowData;

    public WindowData WindowData => m_WindowData;

    public RoofSectionData() : this(new WindowData())
    {

    }

    public RoofSectionData(RoofSectionData data) : this(data.WindowData)
    {

    }

    public RoofSectionData(WindowData windowData)
    {
        m_WindowData = windowData;
    }
}
