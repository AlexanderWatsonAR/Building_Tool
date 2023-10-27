using ClipperLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Floor : MonoBehaviour
{
    [SerializeField] private FloorData m_Data;

    [SerializeField] private Vector3[] m_Split;

    public Vector3[] Split => m_Split;

    public Floor Initialize(FloorData data)
    {
        m_Data = data;
        m_Split = new Vector3[0];
        return this;
    }

    public Floor Build()
    {
        transform.DeleteChildren();

        Vector3 min, max;
        Extensions.MinMax(m_Data.ControlPoints.GetPositions(), out min, out max);

        float width = max.x - min.x;
        float height = max.z - min.z;

        float size = width > height ? width : height;

        Vector3 pos = Vector3.Lerp(min, max, 0.5f);

        List<IList<Vector3>> floorSection = MeshMaker.SpiltPolygon(m_Data.ControlPoints.GetPositions(), size, size, m_Data.Columns, m_Data.Rows, pos, Vector3.up);

        List<Vector3> list = new();

        foreach (List<Vector3> polygon in floorSection)
        {
            list.AddRange(polygon);

            FloorSectionData sectionData = new FloorSectionData(polygon, m_Data.Height);
            ProBuilderMesh sectionMesh = ProBuilderMesh.Create();
            sectionMesh.name = "Floor Section";
            sectionMesh.transform.SetParent(transform, true);
            sectionMesh.AddComponent<FloorSection>().Initialize(sectionData).Build();
            sectionMesh.GetComponent<Renderer>().sharedMaterial = BuiltinMaterials.defaultMaterial;
        }

        m_Split = list.Distinct().ToArray();

        // TODO: We want a way for the user to insert a hole into the floor.
        // The hole should be modifiable, so that the user can adjust its size, shape & position.
        // The hole should also be bound to the inside of the polygon.

        // Idea: Floor Sections! Spilt the polygon with mesh maker function.
        // each floor section can have a hole

        return this;
    }

}
