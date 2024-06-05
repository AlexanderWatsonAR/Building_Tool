using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Window", menuName = "Content/New Window")]
public class WindowA : Content
{
    [SerializeField] List<FrameA> m_Frames;
    [SerializeField] Content m_Pane;

}