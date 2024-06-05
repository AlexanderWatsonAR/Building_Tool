using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeshShape : Shape
{
    [SerializeField] Mesh m_Mesh;

    public override Vector3[] ControlPoints()
    {
        return m_Mesh.vertices;
    }
}
