using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UIElements;
using ProMaths = UnityEngine.ProBuilder.Math;

public class Door : MonoBehaviour
{
    [SerializeField] private Vector3[] m_ControlPoints;

    [SerializeField] private ProBuilderMesh m_ProBuilderMesh;
    [SerializeField] private float m_Depth;
    [SerializeField] private Vector3 m_Centre;
    [SerializeField] private Vector3 m_Forward;
    
    [SerializeField] private Material m_Material;
    [SerializeField] private Vector3 m_Scale;
    [SerializeField] private TransformPoint m_HingePoint;
    [SerializeField] private Vector3 m_HingePosition;
    [SerializeField] private Vector3 m_HingeOffset;
    [SerializeField] private Vector3 m_HingeEulerAngles;

    public Vector3 HingeOffset => m_HingeOffset;

    public TransformPoint HingePoint
    {
        get
        {
            return m_HingePoint;
        }

        set
        {
            if(m_HingePoint != value)
            {
                m_HingeOffset = Vector3.zero;
            }
            
            m_HingePoint = value;

            switch (m_HingePoint)
            {
                case TransformPoint.Middle:
                    m_HingePosition = m_Centre;
                    break;
                case TransformPoint.Top:
                    m_HingePosition = Vector3.Lerp(m_ControlPoints[1], m_ControlPoints[2], 0.5f) + (m_Forward * (m_Depth * 0.5f));
                    break;
                case TransformPoint.Bottom:
                    m_HingePosition = Vector3.Lerp(m_ControlPoints[0], m_ControlPoints[3], 0.5f) + (m_Forward * (m_Depth * 0.5f));
                    break;
                case TransformPoint.Left:
                    m_HingePosition = Vector3.Lerp(m_ControlPoints[2], m_ControlPoints[3], 0.5f) + (m_Forward * (m_Depth * 0.5f));
                    break;
                case TransformPoint.Right:
                    m_HingePosition = Vector3.Lerp(m_ControlPoints[0], m_ControlPoints[1], 0.5f) + (m_Forward * (m_Depth * 0.5f));
                    break;
            }

            
        }
    }

    public Door SetMaterial(Material material)
    {
        m_Material = material;
        GetComponent<Renderer>().material = m_Material;
        return this;
    }

    public Door SetHingeEulerAngles(Vector3 eulerAngles)
    {
        m_HingeEulerAngles = eulerAngles;
        return this;
    }

    public Door SetHingeOffset(Vector3 offset)
    {
        m_HingeOffset = offset;
        return this;
    }

    public Door Initialize(IEnumerable<Vector3> controlPoints, float depth, Vector3 scale)
    {
        m_ControlPoints = controlPoints == null ? m_ControlPoints : controlPoints.ToArray();
        m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
        m_Depth = depth;

        Vector3 a = m_ControlPoints[0];
        Vector3 b = m_ControlPoints[3];
        Vector3 dir = a.DirectionToTarget(b);
        Vector3 forward = Vector3.Cross(Vector3.up, dir);
        Vector3 c = ProMaths.Average(m_ControlPoints);
        m_Centre = c + (forward * (depth * 0.5f));
        m_Scale = scale;
        m_Forward = forward;

        m_HingeEulerAngles = Vector3.zero;
        HingePoint = TransformPoint.Left;
        return this;
    }



    public Door Build()
    {
        Rebuild(MeshMaker.Cube(m_ControlPoints, m_Depth));

        // Scale
        m_ProBuilderMesh.transform.localScale = m_Scale;
        m_ProBuilderMesh.LocaliseVertices();
        // Rotate
        m_ProBuilderMesh.transform.localEulerAngles = m_HingeEulerAngles;
        m_ProBuilderMesh.LocaliseVertices(m_HingePosition + m_HingeOffset);
        return this;
    }


    private void Rebuild(ProBuilderMesh mesh)
    {
        m_ProBuilderMesh.RebuildWithPositionsAndFaces(mesh.positions, mesh.faces);
        m_ProBuilderMesh.ToMesh();
        m_ProBuilderMesh.Refresh();
        DestroyImmediate(mesh.gameObject);
    }

}
