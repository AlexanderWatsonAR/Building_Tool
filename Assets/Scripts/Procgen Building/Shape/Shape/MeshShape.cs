using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeshShape : Shape
{
    [SerializeField] Mesh m_Mesh;

    public Mesh Mesh { get => m_Mesh; set => m_Mesh = value; }

    public MeshShape(Mesh mesh)
    {
        m_Mesh = mesh;
    }

    public override Vector3[] ControlPoints()
    {
        m_Mesh ??= new Mesh();

        if (m_Mesh.vertices == null || m_Mesh.vertices.Length < 3)
        {
            m_Mesh.vertices = MeshMaker.Square();
            m_Mesh.normals = new Vector3[] { Vector3.forward };
            m_Mesh.name = "Square";
        }

        return m_Mesh.vertices;
    }
}
