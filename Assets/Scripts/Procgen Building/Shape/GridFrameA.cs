using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Grid Frame", menuName = "Frame/New Grid Frame")]
public class GridFrameA : FrameA
{
    [SerializeField, Range(1, 5)] int m_Columns, m_Rows;

}
