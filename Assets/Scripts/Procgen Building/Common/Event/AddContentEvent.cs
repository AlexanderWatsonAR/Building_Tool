using OnlyInvalid.ProcGenBuilding.Polygon3D;
using System.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;

public class AddContentEvent : EventBase<AddContentEvent>
{
    Polygon3D m_Content;

    public Polygon3D Content { get => m_Content; set => m_Content = value; }

    public static AddContentEvent GetPooled(Polygon3D content)
    {
        AddContentEvent e = GetPooled();
        e.pooled = content;
        return e;
    }


}