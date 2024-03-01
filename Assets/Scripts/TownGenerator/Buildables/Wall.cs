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
    [SerializeReference] WallData m_Data;
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

        //Vector3 right = m_Data.Start.DirectionToTarget(m_Data.End);
        //Vector3 faceNormal = Vector3.Cross(right, Vector3.up);

        m_Data.SectionData = new WallSectionData
        {
            Depth = m_Data.Depth,
            Window = DefineWindowDefaults(),
            Door = DefineDoorDefaults(),
            DoorFrame = DefineFrameDefaults(),
        };

        return this;
    }

    private WindowData DefineWindowDefaults()
    {
        FrameData outerFrame = new FrameData()
        {
            Depth = m_Data.Depth,
            Normal = m_Data.Normal
            
        };
        GridFrameData innerFrame = new GridFrameData()
        {
            Depth = m_Data.Depth * 0.5f,
            Normal = m_Data.Normal
            
        };
        Polygon3DData pane = new Polygon3DData()
        {
            Depth = m_Data.Depth * 0.25f,
            Normal = m_Data.Normal
        };
        DoorData leftShutter = new DoorData()
        {
            Depth = m_Data.Depth * 0.5f,
            Normal = m_Data.Normal

        };
        DoorData rightShutter = new DoorData()
        {
            Depth = m_Data.Depth * 0.5f,
            Normal = m_Data.Normal
        };
        WindowData winData = new WindowData()
        {
            OuterFrame = outerFrame,
            InnerFrame = innerFrame,
            Pane = pane,
            LeftShutter = leftShutter,
            RightShutter = rightShutter
        };
        return winData;
    }

    private DoorData DefineDoorDefaults()
    {
        DoorData doorData = new DoorData()
        {
            Normal = m_Data.Normal,
            HingePoint = TransformPoint.Left,
        };
        return doorData;
    }

    private FrameData DefineFrameDefaults()
    {
        FrameData frameData = new FrameData()
        {
            Depth = m_Data.Depth * 1.1f,
            Normal = m_Data.Normal
        };
        return frameData;
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

                m_Data.Sections[count] ??= new WallSectionData(m_Data.SectionData)
                {
                    ID = new Vector2Int(x, y),
                    Polygon = new PolygonData(points, m_Data.Normal),
                    Depth = m_Data.Depth,                 
                };

                m_Data.Sections[count].Polygon.ControlPoints = points;
                m_Data.Sections[count].Depth = m_Data.Depth;

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
