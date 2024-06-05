using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Frame", menuName = "Frame/New Frame")]
public class FrameSO : ContentSO
{
    [SerializeField, Range(0, 1)] float m_Scale, m_Depth;
}
