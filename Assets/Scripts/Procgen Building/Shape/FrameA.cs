using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Frame", menuName = "Frame/New Frame")]
public class FrameA : Content
{
    [SerializeField, Range(0, 1)] float m_Scale, m_Depth;
}
