using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mesh Shape", menuName = "Shape/New Mesh Shape")]
public class MeshShapeSO : ShapeSO
{
    public MeshShape MeshShape => m_Shape as MeshShape;

    public override void Initialize()
    {
        m_Shape = new MeshShape();
    }
}
