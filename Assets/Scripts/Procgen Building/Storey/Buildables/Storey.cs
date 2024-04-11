using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using OnlyInvalid.ProcGenBuilding.Common;
using OnlyInvalid.ProcGenBuilding.Wall;
using OnlyInvalid.ProcGenBuilding.Corner;
using OnlyInvalid.ProcGenBuilding.Pillar;
using OnlyInvalid.ProcGenBuilding.Floor;

namespace OnlyInvalid.ProcGenBuilding.Storey
{
    public class Storey : Buildable
    {
        [SerializeReference] StoreyData m_Data;

        [SerializeField] List<Corner.Corner> m_Corners;
        [SerializeField] List<Wall.Wall> m_Walls;
        [SerializeField] List<Pillar.Pillar> m_Pillars;
        [SerializeField] Floor.Floor m_Floor;

        public override Buildable Initialize(DirtyData data)
        {
            m_Data = data as StoreyData;
            m_Corners ??= new List<Corner.Corner>();
            m_Walls ??= new List<Wall.Wall>();
            m_Pillars ??= new List<Pillar.Pillar>();
            return this;
        }

        public override void Build()
        {
            Demolish();

            CreateExternalWalls();
            CreateCorners();
            CreatePillars();
            CreateFloor();

            BuildWalls();
            BuildCorners();
            BuildPillars();
            BuildFloor();
        }
        private void CreatePillars()
        {
            if (!m_Data.ActiveElements.IsElementActive(StoreyElement.Pillars))
                return;

            if (m_Data.Pillars == null || m_Data.Pillars.Length != m_Data.ControlPoints.Length)
            {
                m_Data.Pillars = new PillarData[m_Data.ControlPoints.Length];
            }

            m_Pillars = new List<Pillar.Pillar>();

            GameObject pillars = new GameObject("Pillars");
            pillars.transform.SetParent(transform, false);

            for (int i = 0; i < m_Data.ControlPoints.Length; i++)
            {
                m_Data.Pillars[i] ??= CalculatePillar(i);

                Pillar.Pillar pillar = CreatePillar(m_Data.Pillars[i]);
                pillar.transform.SetParent(pillars.transform, false);
                pillar.transform.localPosition = m_Data.ControlPoints[i].Position;
                int index = m_Data.ControlPoints.GetNext(i);
                pillar.transform.forward = pillar.transform.localPosition.DirectionToTarget(m_Data.ControlPoints[index].Position);

                m_Pillars.Add(pillar);
            }
        }
        private void CreateCorners()
        {
            if (!m_Data.ActiveElements.IsElementActive(StoreyElement.Walls))
                return;

            GameObject corners = new GameObject("Corners");
            corners.transform.SetParent(transform, false);

            if (m_Data.Corners == null || m_Data.Corners.Length != m_Data.ControlPoints.Length)
            {
                m_Data.Corners = new CornerData[m_Data.ControlPoints.Length];
            }

            for (int i = 0; i < m_Data.ControlPoints.Length; i++)
            {
                m_Data.Corners[i] = CalculateCorner(i);
                Corner.Corner corner = CreateCorner(m_Data.Corners[i]);
                corner.transform.SetParent(corners.transform, false);
                m_Corners.Add(corner);
            }
        }
        private void CreateExternalWalls()
        {
            if (!m_Data.ActiveElements.IsElementActive(StoreyElement.Walls))
                return;

            if (m_Data.Walls == null || m_Data.Walls.Length != m_Data.ControlPoints.Length)
            {
                m_Data.Walls = new WallData[m_Data.ControlPoints.Length];
            }

            GameObject walls = new GameObject("Walls");
            walls.transform.SetParent(transform, false);

            // Construct the walls 
            for (int i = 0; i < m_Data.ControlPoints.Length; i++)
            {
                m_Data.Walls[i] ??= CalculateWall(i);

                Wall.Wall wall = CreateWall(m_Data.Walls[i]);
                wall.transform.SetParent(walls.transform, true);

                m_Walls.Add(wall);
            }
        }
        private Wall.Wall CreateWall(WallData data)
        {
            ProBuilderMesh wallMesh = ProBuilderMesh.Create();
            wallMesh.name = "Wall " + data.ID.ToString();
            Wall.Wall wall = wallMesh.AddComponent<Wall.Wall>();
            wall.Initialize(data);
            return wall;
        }
        private Corner.Corner CreateCorner(CornerData data)
        {
            ProBuilderMesh cornerMesh = ProBuilderMesh.Create();
            cornerMesh.name = "Corner " + data.ID.ToString();
            cornerMesh.GetComponent<Renderer>().sharedMaterial = BuiltinMaterials.defaultMaterial;
            Corner.Corner corner = cornerMesh.AddComponent<Corner.Corner>();
            corner.Initialize(data);
            return corner;
        }
        private Pillar.Pillar CreatePillar(PillarData data)
        {
            int i = data.ID;
            ProBuilderMesh pillarMesh = ProBuilderMesh.Create();
            pillarMesh.name = "Pillar " + data.ID.ToString();
            Pillar.Pillar pillar = pillarMesh.AddComponent<Pillar.Pillar>();
            pillar.Initialize(m_Data.Pillars[i]);
            return pillar;
        }
        private void CreateFloor()
        {
            if (!m_Data.ActiveElements.IsElementActive(StoreyElement.Floor))
                return;

            ProBuilderMesh floorMesh = ProBuilderMesh.Create();

            ControlPoint[] points = new ControlPoint[m_Data.ControlPoints.Length];
            Array.Copy(m_Data.ControlPoints, points, points.Length);

            m_Floor = floorMesh.AddComponent<Floor.Floor>().Initialize(new FloorData() { ControlPoints = points }) as Floor.Floor;

            floorMesh.transform.SetParent(transform, false);
            floorMesh.transform.localPosition = Vector3.zero;

        }

        #region Build
        public void BuildCorners()
        {
            m_Corners.BuildCollection();
        }
        public void BuildPillars()
        {
            m_Pillars.BuildCollection();
        }
        public void BuildWalls()
        {
            m_Walls.BuildCollection();
        }
        public void BuildFloor()
        {
            m_Floor.Build();
        }
        #endregion

        #region Calculate
        /// <summary>
        /// Can we migrate all of this to the Corners class?
        /// </summary>
        /// <param name="cornerIndex"></param>
        /// <returns></returns>
        private CornerData CalculateCorner(int cornerIndex)
        {
            bool isConcave = m_Data.ControlPoints.IsConcave(out int[] concavePoints);
            WallData wallData = m_Data.WallData;

            int current = cornerIndex;
            int previous = m_Data.ControlPoints.GetPrevious(current);
            int next = m_Data.ControlPoints.GetNext(current);


            Vector3 dirA = m_Data.Walls[current].StartPosition.DirectionToTarget(m_Data.Walls[current].EndPosition);
            Vector3 crossA = Vector3.Cross(dirA, Vector3.up) * wallData.Depth;

            Vector3 dirB = m_Data.Walls[previous].StartPosition.DirectionToTarget(m_Data.Walls[previous].EndPosition);
            Vector3 crossB = Vector3.Cross(dirB, Vector3.up) * wallData.Depth;

            Vector3 intersection;

            Extensions.DoLinesIntersect(m_Data.Walls[current].StartPosition + crossA, m_Data.Walls[current].EndPosition + crossA, m_Data.Walls[previous].StartPosition + crossB, m_Data.Walls[previous].EndPosition + crossB, out intersection);

            int numberOfSamples = m_Data.CornerData.Sides + 1;

            Vector3[] cornerPoints = new Vector3[] { m_Data.Walls[current].StartPosition, m_Data.Walls[current].StartPosition + crossA, m_Data.Walls[current].StartPosition + crossB, intersection };
            Vector3[] flatPoints = new Vector3[] { cornerPoints[0], cornerPoints[1], cornerPoints[2] };

            bool isInside = isConcave && concavePoints.Any(b => b == current);

            if (isInside)
            {
                Extensions.DoLinesIntersect(m_Data.Walls[current].StartPosition, m_Data.Walls[current].EndPosition, m_Data.Walls[previous].StartPosition, m_Data.Walls[previous].EndPosition, out intersection);
                cornerPoints = new Vector3[] { m_Data.Walls[current].StartPosition, m_Data.Walls[current].StartPosition + crossA, m_Data.Walls[previous].EndPosition, intersection };
                flatPoints = new Vector3[] { m_Data.Walls[current].StartPosition, m_Data.Walls[current].StartPosition + crossA, m_Data.Walls[previous].EndPosition };
            }

            CornerData cornerData = new CornerData(m_Data.CornerData)
            {
                CornerPoints = cornerPoints,
                FlatPoints = flatPoints.SortPointsClockwise().ToArray(),
                ID = current,
                IsInside = isInside,
                Height = m_Data.WallData.Height
            };

            return cornerData;
        }
        private WallData CalculateWall(int wallIndex)
        {
            WallData wallData = m_Data.WallData;
            wallData.Sections = new WallSectionData[wallData.Columns * wallData.Rows];

            m_Data.WallPoints = new WallPoints[m_Data.ControlPoints.Length];

            int current = wallIndex;
            int next = m_Data.ControlPoints.GetNext(current);

            Vector3 nextDir = m_Data.ControlPoints[current].DirectionToTarget(m_Data.ControlPoints[next]);
            Vector3 wallForward = Vector3.Cross(nextDir, Vector3.up) * wallData.Depth;

            ControlPoint start = new ControlPoint(m_Data.ControlPoints[current]);
            ControlPoint end = new ControlPoint(m_Data.ControlPoints[next]);

            bool isConcave = m_Data.ControlPoints.IsConcave(out int[] concavePoints);

            if (isConcave)
            {
                bool conditionA = concavePoints.Any(a => a == next);
                bool conditionB = concavePoints.Any(b => b == current);

                if (conditionA)
                {
                    end.SetForward(-wallForward);
                }
                if (conditionB)
                {
                    start.SetForward(-wallForward);
                }
            }

            WallData data = new WallData(wallData)
            {
                ID = wallIndex,
                Start = start,
                End = end,
                Normal = wallForward.normalized
            };

            return data;
        }
        private PillarData CalculatePillar(int pillarIndex)
        {
            PillarData data = new PillarData(m_Data.Pillar)
            {
                ID = pillarIndex
            };

            return data;
        }
        #endregion

        public override void Demolish()
        {
            transform.DeleteChildren();

            m_Walls?.Clear();
            m_Corners?.Clear();
            m_Pillars?.Clear();


        }
    }
}
