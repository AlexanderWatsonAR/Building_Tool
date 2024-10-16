using ClipperLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Floor
{
    public class Floor : Buildable
    {
        [SerializeReference] FloorData m_FloorData;

        [SerializeField] Vector3 m_Position;
        [SerializeField] float m_Size;

        [SerializeField] Vector3[] m_Split;

        public Vector3[] Split => m_Split;

        public override Buildable Initialize(DirtyData data)
        {
            m_FloorData = data as FloorData;
            name = "Floor";
            m_Split = new Vector3[0];
            m_Position = Vector3.zero;
            return this;
        }

        public override void Build()
        {
            transform.DeleteChildren();

            Vector3 min, max;
            Extensions.MinMax(m_FloorData.ControlPoints.GetPositions(), out min, out max);

            float width = max.x - min.x;
            float height = max.z - min.z;

            m_Size = width > height ? width : height;

            m_Position = Vector3.Lerp(min, max, 0.5f);

            IList<IList<Vector3>> floorSection = MeshMaker.SpiltPolygon(m_FloorData.ControlPoints.GetPositions(), m_Size, m_Size, m_FloorData.Columns, m_FloorData.Rows, m_Position, Vector3.up);

            List<Vector3> list = new();

            foreach (List<Vector3> polygon in floorSection)
            {
                list.AddRange(polygon);

                FloorSectionData sectionData = new FloorSectionData(polygon, m_FloorData.Height);
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

        }

        public override void Demolish()
        {

        }

        private void OnDrawGizmosSelected()
        {
            Vector3[] square = new Vector3[]
            {
            m_Position + new Vector3(-m_Size * 0.5f, 0, -m_Size * 0.5f),
            m_Position + new Vector3(-m_Size * 0.5f, 0, m_Size * 0.5f),
            m_Position + new Vector3(m_Size * 0.5f, 0, m_Size * 0.5f),
            m_Position + new Vector3(m_Size * 0.5f, 0, -m_Size * 0.5f)
            };

            //Handles.DrawAAPolyLine(square);
            //Handles.DrawAAPolyLine(square[0], square[3]);

            for (int x = 0; x < m_FloorData.Columns; x++)
            {
                for (int y = 0; y < m_FloorData.Rows; y++)
                {

                }
            }

        }

    }
}
