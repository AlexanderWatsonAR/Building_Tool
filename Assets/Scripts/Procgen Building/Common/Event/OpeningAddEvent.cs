using OnlyInvalid.ProcGenBuilding.Common;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class OpeningAddEvent : EventBase<OpeningAddEvent>
{
    //OpeningData m_Opening;

    //public OpeningData opening { get => m_Opening; set => m_Opening = value; }

    //public static OpeningAddEvent GetPooled(/*OpeningData item*/)
    //{
    //    OpeningAddEvent e = GetPooled();
    //    //e.opening = item;
    //    return e;
    //}

    //protected override void Init()
    //{
    //    base.Init();
    //    bubbles = true;
    //    tricklesDown = true;
    //}
}

public class OpeningEvent : EventBase<OpeningEvent>
{
    public static OpeningEvent GetPooled(string message, VisualElement target)
    {
        OpeningEvent e = GetPooled();
        return e;
    }
}

public class DataEvent : EventBase<DataEvent>
{
    bool m_IsDirty;
    public bool isDirty { get => m_IsDirty; set => m_IsDirty = value; }

    public static DataEvent GetPooled(bool isDirty)
    {
        DataEvent e = GetPooled();
        e.isDirty = isDirty;
        return e;
    }
}