using UnityEngine;

public class Pane : Polygon3D
{
    [SerializeReference] PaneData m_Data;

    public PaneData Data => m_Data;

    public override IBuildable Initialize(IData data)
    {
        m_Data = data as PaneData;
        return base.Initialize(data);
    }

    public override void Build()
    {
        base.Build();
    }
}
