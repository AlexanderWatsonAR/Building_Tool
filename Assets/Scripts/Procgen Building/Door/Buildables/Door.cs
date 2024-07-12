using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Door
{
    public class Door : Polygon3D.Polygon3D
    {
        //[SerializeField] private ProBuilderMesh m_DoorHandleMesh;
        [SerializeReference] DoorData m_DoorData;

        public override Buildable Initialize(DirtyData data)
        {
            base.Initialize(data);
            m_DoorData = data as DoorData;


            //m_DoorHandleMesh = ProBuilderMesh.Create();
            //m_DoorHandleMesh.transform.SetParent(transform, false);
            //m_DoorHandleMesh.GetComponent<Renderer>().material = BuiltinMaterials.defaultMaterial;
            //m_DoorHandleMesh.name = "Handle";

            return this;
        }

        public override void Build()
        {
            if (!m_DoorData.IsDirty)
                return;

            if (!m_DoorData.ActiveElements.IsElementActive(DoorElement.Door))
                return;

            base.Build();

            // Scale
            m_ProBuilderMesh.transform.localScale = m_DoorData.Hinge.Scale;
            m_ProBuilderMesh.LocaliseVertices();

            // Rotate
            m_ProBuilderMesh.transform.localEulerAngles = m_DoorData.Hinge.EulerAngle;
            m_DoorData.Hinge.AbsolutePosition = m_DoorData.CalculateRelativePosition(m_DoorData.Hinge.RelativePosition);
            m_ProBuilderMesh.LocaliseVertices(m_DoorData.Hinge.AbsolutePosition + m_DoorData.Hinge.PositionOffset);
            m_ProBuilderMesh.Refresh();

            if (!m_DoorData.ActiveElements.IsElementActive(DoorElement.Handle))
                return;

            #region Handle
            // Handle
            //float size = m_Data.HandleSize * m_Data.HandleScale;
            //Vector3[] points = MeshMaker.CreateNPolygon(8, size, size);
            //Vector3 position = ProMaths.Average(points);

            //// This is aligning the handle points with the door
            //Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, m_Data.Forward);

            //for(int i  = 0; i < points.Length; i++)
            //{
            //    Vector3 v = Quaternion.Euler(rotation.eulerAngles) * (points[i] - position) + position;
            //    points[i] = v;
            //}

            //m_DoorHandleMesh.CreateShapeFromPolygon(points, m_Data.Forward);
            //m_DoorHandleMesh.Extrude(m_DoorHandleMesh.faces, ExtrudeMethod.FaceNormal, 0.1f);
            //m_DoorHandleMesh.ToMesh();
            //m_DoorHandleMesh.Refresh();

            //IList<Edge> edgeList = m_DoorHandleMesh.faces[0].edges.ToList();

            //Bevel.BevelEdges(m_DoorHandleMesh, edgeList, 0.1f);

            //m_DoorHandleMesh.ToMesh();
            //m_DoorHandleMesh.Refresh();

            //m_DoorHandleMesh.transform.localPosition = m_Data.HandlePosition;
            ////m_DoorHandleMesh.transform.localPosition = m_Data.Centre + (m_Data.Forward * m_Data.Depth);
            //m_DoorHandleMesh.LocaliseVertices();
            //m_DoorHandleMesh.Refresh();
            //m_DoorHandleMesh.transform.localEulerAngles = m_Data.HingeEulerAngles;
            //m_DoorHandleMesh.LocaliseVertices(m_Data.HingePosition + m_Data.HingeOffset);
            //m_DoorHandleMesh.Refresh();
            #endregion
        }

    }
}