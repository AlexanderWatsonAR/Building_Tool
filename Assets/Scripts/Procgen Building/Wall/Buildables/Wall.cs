using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder;
using System.Linq;
using UnityEditor;
using System;
using OnlyInvalid.ProcGenBuilding.Common;
using OnlyInvalid.ProcGenBuilding.Window;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using OnlyInvalid.ProcGenBuilding.Door;

namespace OnlyInvalid.ProcGenBuilding.Wall
{
    public class Wall : Buildable
    {
        [SerializeReference] WallData m_WallData;

        [SerializeField] List<Section> m_Sections;

        List<Vector3[]> m_SubPoints; // Grid points, based on control points, columns & rows.

        private List<Vector3[]> SubPoints
        {
            get
            {
                if (m_WallData.Columns <= 0 && m_WallData.Rows <= 0) return null;

                Vector3 bottomLeft = m_WallData.StartPosition;
                Vector3 bottomRight = m_WallData.EndPosition;
                Vector3 h = Vector3.up * m_WallData.Height;

                Vector3 topLeft = m_WallData.IsTriangle ? Vector3.Lerp(bottomLeft + h, bottomRight + h, 0.5f) : bottomLeft + h;
                Vector3 topRight = m_WallData.IsTriangle ? Vector3.Lerp(bottomLeft + h, bottomRight + h, 0.5f) : bottomRight + h;

                m_SubPoints = MeshMaker.CreateGridFromControlPoints(new Vector3[] { bottomLeft, topLeft, topRight, bottomRight }, m_WallData.Columns, m_WallData.Rows);

                return m_SubPoints;
            }
        }

        public WallData WallData => m_WallData;

        public override Buildable Initialize(DirtyData data)
        {
            base.Initialize(data);
            m_WallData = data as WallData;

            //Vector3 right = m_Data.Start.DirectionToTarget(m_Data.End);
            //Vector3 faceNormal = Vector3.Cross(right, Vector3.up);

            m_WallData.SectionData = new SectionData
            {
                Depth = m_WallData.Depth,
                //Window = DefineWindowDefaults(),
                //Door = DefineDoorDefaults(),
                //DoorFrame = DefineFrameDefaults(),
                Normal = m_WallData.Normal
            };

            return this;
        }

        private WindowData DefineWindowDefaults()
        {
            OuterFrameData outerFrame = new OuterFrameData()
            {
                Depth = m_WallData.Depth,
                Normal = m_WallData.Normal

            };
            InnerFrameData innerFrame = new InnerFrameData()
            {
                Depth = m_WallData.Depth * 0.5f,
                Normal = m_WallData.Normal

            };
            PaneData pane = new PaneData()
            {
                Depth = m_WallData.Depth * 0.25f,
                Holes = new PolygonData[0],
                Normal = m_WallData.Normal
            };
            DoorData leftShutter = new DoorData()
            {
                Depth = m_WallData.Depth * 0.5f,
                Normal = m_WallData.Normal

            };
            DoorData rightShutter = new DoorData()
            {
                Depth = m_WallData.Depth * 0.5f,
                Normal = m_WallData.Normal
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
                Normal = m_WallData.Normal,
                Hinge = new TransformData()
                {
                    RelativePosition = RelativePosition.Left
                }
            };
            return doorData;
        }
        private FrameData DefineFrameDefaults()
        {
            FrameData frameData = new FrameData()
            {
                Depth = m_WallData.Depth * 1.1f,
                Normal = m_WallData.Normal
            };
            return frameData;
        }

        public override void Build()
        {
            Demolish();

            if (m_Data.IsDirty)
                CreateWallSections();

            m_Sections.BuildCollection();
        }

        private Section CreateWallSection(SectionData data)
        {
            ProBuilderMesh wallSectionMesh = ProBuilderMesh.Create();
            wallSectionMesh.name = "Wall Section " + data.ID.ToString();
            wallSectionMesh.GetComponent<Renderer>().sharedMaterial = m_WallData.Material;
            wallSectionMesh.transform.SetParent(transform, false);
            Section wallSection = wallSectionMesh.AddComponent<Section>();
            wallSection.Initialize(m_WallData.Sections[data.ID]);
            wallSection.AddListener(dirtyData =>
            {
                SectionData wallSectionData = dirtyData as SectionData;
                m_WallData.Sections[wallSectionData.ID] = wallSectionData;
                m_OnDataChanged.Invoke(m_WallData);
            });
            return wallSection;
        }

        private void CreateWallSections()
        {
            m_Sections ??= new List<Section>();

            List<Vector3[]> subPoints = SubPoints;

            m_WallData.Sections ??= new SectionData[m_WallData.Columns * m_WallData.Rows];

            int count = 0;

            for (int x = 0; x < m_WallData.Columns; x++)
            {
                for (int y = 0; y < m_WallData.Rows; y++)
                {
                    Vector3 first = subPoints[y][x];
                    Vector3 second = subPoints[y + 1][x];
                    Vector3 third = subPoints[y + 1][x + 1];
                    Vector3 fourth = subPoints[y][x + 1];

                    Vector3[] points = new Vector3[] { first, second, third, fourth };

                    m_WallData.Sections[count] ??= new SectionData(m_WallData.SectionData)
                    {
                        ID = count,
                        Polygon = new PolygonData(points, m_WallData.Normal),
                        Normal = m_WallData.Normal,
                        Depth = m_WallData.Depth,
                        IsDirty = true
                    };

                    m_WallData.Sections[count].Polygon = new PolygonData(points, m_WallData.Normal);
                    m_WallData.Sections[count].Depth = m_WallData.Depth;

                    Section section = CreateWallSection(m_WallData.Sections[count]);
                    m_Sections.Add(section);

                    count++;
                }
            }
        }


        public override void Demolish()
        {
            if (!m_Data.IsDirty)
                return;

            transform.DeleteChildren();
        }
    }
}
