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
    [SerializeReference] private WallData m_Data;
    private List<Vector3[]> m_SubPoints; // Grid points, based on control points, columns & rows.

    private List<Vector3[]> SubPoints
    {
        get
        {
            if (m_Data.Columns <= 0 && m_Data.Rows <= 0) return null;

            Vector3 bottomLeft = m_Data.StartPosition;
            Vector3 bottomRight = m_Data.EndPosition;
            Vector3 h = Vector3.up * m_Data.Height;

            Vector3 topLeft = m_Data.IsTriangle ? Vector3.Lerp(bottomLeft + h, bottomRight + h, 0.5f): bottomLeft + h;
            Vector3 topRight = m_Data.IsTriangle ? Vector3.Lerp(bottomLeft + h, bottomRight + h, 0.5f) : bottomRight + h;

            m_SubPoints = MeshMaker.CreateGridFromControlPoints(new Vector3[] {bottomLeft, topLeft, topRight, bottomRight}, m_Data.Columns, m_Data.Rows);

            return m_SubPoints;
        }
    }

    public WallData Data => m_Data;

    public IBuildable Initialize(IData data)
    {
        m_Data = data as WallData;

        Vector3 right = m_Data.Start.DirectionToTarget(m_Data.End);
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

        m_Data.SectionData = new WallSectionData()
        {
            WallDepth = m_Data.Depth,
            FaceNormal = faceNormal,
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

        m_Data.Sections ??= new WallSectionData[m_Data.Columns * m_Data.Rows];

        int count = 0;

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

                try
                {
                    m_Data.Sections[count] ??= new WallSectionData(m_Data.SectionData)
                    {
                        ID = new Vector2Int(x, y)
                    };
                }
                catch (IndexOutOfRangeException)
                {
                    Debug.Break();
                }
                

                m_Data.Sections[count].ControlPoints = points;
                m_Data.Sections[count].WallDepth = m_Data.Depth;

                WallSection wallSection = wallSectionMesh.AddComponent<WallSection>().Initialize(m_Data.Sections[count]) as WallSection;
                wallSection.Build();
                count++;
            }
        }
    }

    public void Demolish()
    {

    }
}
