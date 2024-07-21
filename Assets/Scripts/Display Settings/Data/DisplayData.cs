using UnityEngine;

[CreateAssetMenu(fileName = "Display Data", menuName = "Display Data / Display Data")]
public class DisplayData : ScriptableObject
{
    [SerializeField] NPolygonDisplayData m_NPolygon;
    [SerializeField] ArchDisplayData m_Arch;
    [SerializeField] FrameDisplayData m_Frame;
    [SerializeField] GridFrameDisplayData m_GridFrame;
    [SerializeField] PillarDisplayData m_Pillar;

    public NPolygonDisplayData NPolygon
    {
        get
        {
            return m_NPolygon = m_NPolygon != null ? m_NPolygon : CreateInstance<NPolygonDisplayData>().Initialize();
        }
    }
    public ArchDisplayData Arch
    {
        get
        {
            return m_Arch = m_Arch != null ? m_Arch : CreateInstance<ArchDisplayData>().Initialize();
        }
    }
    public FrameDisplayData Frame
    {
        get
        {
            return m_Frame = m_Frame != null ? m_Frame : CreateInstance<FrameDisplayData>().Initialize();
        }
    }
    public GridFrameDisplayData GridFrame
    {
        get
        {
            return m_GridFrame = m_GridFrame != null ? m_GridFrame : CreateInstance<GridFrameDisplayData>().Initialize();
        }
    }
    public PillarDisplayData Pillar
    {
        get
        {
            return m_Pillar = m_Pillar != null ? m_Pillar : CreateInstance<PillarDisplayData>().Initialize();
        }
    }


    public DisplayData Initialize()
    {
        m_NPolygon = NPolygon;
        m_Arch = Arch;
        m_Frame = Frame;
        m_GridFrame = GridFrame;
        return this;
    }
}

