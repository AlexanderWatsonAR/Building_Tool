using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Outer Frame", menuName = "Frame/New Outer Frame")]
public class OuterFrameScriptableObject : FrameScriptableObject
{
    [SerializeField] FrameScriptableObject m_InnerFrame;

}
