using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class FloorSection : MonoBehaviour, IBuildable
{
    [SerializeField] private FloorSectionData m_Data;
    [SerializeField] private ProBuilderMesh m_ProBuilderMesh;


    public IBuildable Initialize(IData data)
    {
        m_Data = data as FloorSectionData;
        m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
        return this;
    }

    public void Build()
    {
        m_ProBuilderMesh.CreateShapeFromPolygon(m_Data.ControlPoints, m_Data.Height, false);
        m_ProBuilderMesh.ToMesh();
        m_ProBuilderMesh.Refresh();
    }
    public void Demolish()
    {

    }


}
