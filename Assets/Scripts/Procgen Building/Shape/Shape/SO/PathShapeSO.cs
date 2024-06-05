using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Path Shape", menuName = "Shape/New Path Shape")]
public class PathShapeSO : ShapeSO
{
    public PathShape PathShape => m_Shape as PathShape;

    public override void Initialize()
    {
        m_Shape = new PathShape();
    }

}
