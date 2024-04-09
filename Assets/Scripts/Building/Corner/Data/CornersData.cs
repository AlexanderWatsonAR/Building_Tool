using UnityEngine;

[System.Serializable]
public class CornersData : DirtyData
{
    [SerializeField] CornerData m_Corner;
    [SerializeField] CornerData[] m_Corners;

    public CornerData[] Corners { get { return m_Corners; } set { m_Corners = value; } }
    public CornerData Corner { get { return m_Corner; } set { m_Corner = value; } }

    public CornersData(CornerData[] corners, CornerData corner)
    {
        m_Corners = corners;
        m_Corner = corner;
    }

    public CornersData(CornersData data) : this(data.Corners, data.Corner)
    {

    }

}
