using OnlyInvalid.ProcGenBuilding.Common;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;


public class Polygon3DA : Buildable
{
    [SerializeField, HideInInspector] protected ProBuilderMesh m_ProBuilderMesh;

    public Polygon3DAData Polygon3DData => m_Data as Polygon3DAData;

    public override void Build()
    {
        if (!m_Data.IsDirty)
            return;

        IList<IList<Vector3>> holePoints = Polygon3DData.Holes;
        m_ProBuilderMesh.CreateShapeFromPolygon(Polygon3DData.ControlPoints, Polygon3DData.Normal, holePoints);
        m_ProBuilderMesh.Solidify(Polygon3DData.Depth);
        Polygon3DData.IsDirty = false;
    }
}

[System.Serializable]
public class Polygon3DAData : DirtyData
{
    #region Members
    #region Transform
    // These transforms may not be needed. Could we just use the transform component?

    [SerializeField] Vector3 m_Position, m_EulerAngle, m_Scale;
    [SerializeField] Quaternion m_Rotation;
    #endregion
    // Here we are assuming the exterior shape has a size of 1 and are polygons orientated on the xy plane (normal 0,0,1)
    [SerializeReference] Shape m_ExteriorShape;
    [SerializeReference] List<Shape> m_InteriorShapes;

    [SerializeField] float m_Depth;
    #endregion

    #region Accessors
    public Shape ExteriorShape => m_ExteriorShape;
    public Vector3[] LocalControlPoints => m_ExteriorShape.ControlPoints();
    public Vector3[] ControlPoints
    {
        get
        {
            Vector3[] controlPoints = m_ExteriorShape.ControlPoints();
            Matrix4x4 trs = Matrix4x4.TRS(m_Position, Rotation, m_Scale);

            for(int i = 0; i < controlPoints.Length; i++)
            {
                controlPoints[i] = trs.MultiplyPoint3x4(controlPoints[i]);
            }

            return controlPoints;
        }
    }
    public IList<IList<Vector3>> LocalHoles => (IList<IList<Vector3>>) m_InteriorShapes.Select(hole => hole.ControlPoints().ToList());
    public IList<IList<Vector3>> Holes
    {
        get
        {
            IList<IList<Vector3>> holes = new List<IList<Vector3>>();

            if (m_InteriorShapes == null)
                return null;

            Matrix4x4 trs = Matrix4x4.TRS(m_Position, Rotation, m_Scale);

            foreach (var shape in m_InteriorShapes)
            {
                IList<Vector3> hole = shape.ControlPoints();

                for (int i = 0; i < hole.Count; i++)
                {
                    hole[i] = trs.MultiplyPoint3x4(hole[i]);
                }

                holes.Add(hole);
            }

            return holes;
        }
    }
    public Vector3 Normal
    {
        get
        {
            return m_Rotation * Vector3.forward;
        }
    }
    public float Depth { get => m_Depth; set => m_Depth = value; }
    public Quaternion Rotation
    {
        get
        {
            return Quaternion.Euler(m_EulerAngle);
        }
    }

    #endregion

    public Polygon3DAData()
    {
        m_Position = Vector3.zero;
        m_EulerAngle = Vector3.zero;
        m_Rotation = Quaternion.identity;
        m_Scale = Vector3.one;

        m_ExteriorShape = new Square();
        m_InteriorShapes = new List<Shape>();

        m_Depth = 1;
    }


    public void AddInteriorShape(Shape shape)
    {
        m_InteriorShapes.Add(shape);
    }

    public void ClearInterior()
    {
        m_InteriorShapes?.Clear();
    }



}


