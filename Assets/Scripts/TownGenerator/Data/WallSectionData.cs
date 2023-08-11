using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WallSectionData
{
    [SerializeField] private WindowData m_WindowData;
    [SerializeField] private DoorData m_DoorData;

    public WindowData WindowData => m_WindowData;
    public DoorData DoorData => m_DoorData;

    public WallSectionData() : this(new WindowData(), new DoorData())
    {

    }
    public WallSectionData(WallSectionData data) : this(data.WindowData, data.DoorData)
    {

    }

    public WallSectionData(WindowData windowData, DoorData doorData)
    {
        m_WindowData = windowData;
        m_DoorData = doorData;
    }
}
