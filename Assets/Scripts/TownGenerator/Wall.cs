using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder;
using System.Linq;
using UnityEditor;
using System;

public class Wall : MonoBehaviour, IBuildable
{
    [SerializeField] WallData m_Data;
    private List<Vector3[]> m_SubPoints; // Grid points, based on control points, columns & rows.
    private WallSectionData m_SectionData;

    public event Action<WallData> OnDataChange;

    private List<Vector3[]> SubPoints
    {
        get
        {
            if (m_Data.Columns <= 0 && m_Data.Rows <= 0) return null;

            m_SubPoints = MeshMaker.CreateGridFromControlPoints(m_Data.ControlPoints, m_Data.Columns, m_Data.Rows);

            return m_SubPoints;
        }
    }

    public WallData WallData => m_Data;

    public void OnDataChange_Invoke()
    {
        OnDataChange?.Invoke(m_Data);
    }

    public IBuildable Initialize(IData data)
    {
        m_Data = data as WallData;

        Vector3 right = m_Data.ControlPoints[0].DirectionToTarget(m_Data.ControlPoints[3]);
        Vector3 faceNormal = Vector3.Cross(right, Vector3.up);

        Material defaultMat = BuiltinMaterials.defaultMaterial;

        WindowData winData = new WindowData()
        {
            Forward = faceNormal,
            OuterFrameDepth = m_Data.Depth,
            InnerFrameDepth = m_Data.Depth * 0.5f,
            PaneDepth = m_Data.Depth * 0.25f,
            OuterFrameMaterial = defaultMat,
            InnerFrameMaterial = defaultMat,
            PaneMaterial = defaultMat,
            ShuttersMaterial = defaultMat
        };

        DoorData doorData = new DoorData()
        {
            Forward = faceNormal,
            Right = right,
            HingePoint = TransformPoint.Left,
            Material = defaultMat,
        };

        m_SectionData = new WallSectionData()
        {
            WallDepth = m_Data.Depth,
            WindowData = winData,
            DoorData = doorData,
            DoorFrameInsideScale = doorData.Scale,
            DoorFrameDepth = m_Data.Depth * 1.1f
        };


        return this;
    }

    public void Build()
    {
        List<Vector3[]> subPoints = SubPoints;

        transform.DeleteChildren();

        m_Data.Sections = new WallSectionData[m_Data.Columns, m_Data.Rows];

        for (int x = 0; x < m_Data.Columns; x++)
        {
            for (int y = 0; y < m_Data.Rows; y++)
            {
                Vector3 first = subPoints[y][x];
                Vector3 second = subPoints[y + 1][x];
                Vector3 third = subPoints[y + 1][x + 1];
                Vector3 fourth = subPoints[y][x + 1];

                Vector3[] points = new Vector3[] { first, second, third, fourth };

                ProBuilderMesh wallSectionMesh = ProBuilderMesh.Create();
                wallSectionMesh.name = "Wall Section " + x.ToString() + " " + y.ToString();
                wallSectionMesh.GetComponent<Renderer>().sharedMaterial = m_Data.Material;
                wallSectionMesh.transform.SetParent(transform, false);

                WallSectionData wallSectionData = new WallSectionData(m_SectionData) // Pass in generic section data.
                {
                    ControlPoints = points,
                    ID = new Vector2Int(x, y)
                };

                m_Data.Sections[x, y] = wallSectionData;

                WallSection wallSection = wallSectionMesh.AddComponent<WallSection>().Initialize(wallSectionData) as WallSection;
                wallSection.Build();

                wallSection.OnDataChange += (WallSectionData data) => { m_Data.Sections[data.ID.x, data.ID.y] = data;};

            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        //if (m_Points == null)
        //    return;

        //if (m_Rows != 0 && m_Columns != 0)
        //{
        //    List<Vector3[]> subPoints = SubPoints; // cols by rows

        //    for (int i = 0; i < m_Columns; i++)
        //    {
        //        for (int j = 0; j < m_Rows; j++)
        //        {
        //            Vector3 first = subPoints[i + 0][j + 0];
        //            Vector3 second = subPoints[i + 0][j + 1];
        //            Vector3 third = subPoints[i + 1][j + 1];
        //            Vector3 fourth = subPoints[i + 1][j + 0];

        //            Vector3 dir = first.DirectionToTarget(fourth);
        //            Vector3 cross = Vector3.Cross(Vector3.up, dir) * m_Depth;

        //            first += cross;
        //            second += cross;
        //            third += cross;
        //            fourth += cross;

        //            first += transform.position;
        //            second += transform.position;
        //            third += transform.position;
        //            fourth += transform.position;

        //            Handles.DrawAAPolyLine(first, second, third, fourth);
        //            Handles.DrawAAPolyLine(first, fourth);

        //        }
        //    }
        //}
    }
}
