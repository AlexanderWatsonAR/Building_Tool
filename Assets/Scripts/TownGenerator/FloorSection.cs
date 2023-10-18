using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class FloorSection : MonoBehaviour
{
    [SerializeField] private FloorSectionData m_Data;
    [SerializeField] private ProBuilderMesh m_ProBuilderMesh;


    public FloorSection Initialize(FloorSectionData data)
    {
        m_Data = new FloorSectionData(data);
        m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
        return this;
    }

    public FloorSection Build()
    {
        m_ProBuilderMesh.CreateShapeFromPolygon(m_Data.ControlPoints, m_Data.Height, false);
        m_ProBuilderMesh.ToMesh();
        m_ProBuilderMesh.Refresh();

        return this;
    }

}
