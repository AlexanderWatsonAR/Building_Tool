using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UIElements;

public class Window : MonoBehaviour, IBuildable
{
    [SerializeField] private WindowData m_Data;

    [SerializeField] private ProBuilderMesh m_OuterFrame;
    [SerializeField] private ProBuilderMesh m_InnerFrame;
    [SerializeField] private ProBuilderMesh m_Pane;
    [SerializeField] private ProBuilderMesh m_LeftShutter, m_RightShutter;

    private float m_Height, m_Width;
    private Vector3 m_Min, m_Max;
    private Vector3 m_Position;

    public float Height => m_Height;
    public float Width => m_Width;


    public WindowData WindowData => m_Data;

    private Vector3[] ScaledControlPoints
    {
        get
        {
            Vector3[] scaledPoints = new Vector3[m_Data.ControlPoints.Length];

            for(int i = 0; i < scaledPoints.Length; i++)
            {
                Vector3 point = m_Data.ControlPoints[i] - m_Position;
                Vector3 v = Vector3.Scale(point, Vector3.one * m_Data.OuterFrameScale) + m_Position;
                scaledPoints[i] = v;
            }

            return scaledPoints;
        }
    }
    public IBuildable Initialize(IData data)
    {
        m_Data = data as WindowData;

        Extensions.MinMax(m_Data.ControlPoints, out m_Min, out m_Max);
        m_Height = m_Max.y - m_Min.y;
        m_Width = m_Max.x - m_Min.x + (m_Max.z - m_Min.z);
        m_Position = Vector3.Lerp(m_Min, m_Max, 0.5f);

        return this;
    }

    public void SetPosition(Vector3 position)
    {
        if(m_OuterFrame != null)
        {
            m_OuterFrame.transform.position = position;
            m_OuterFrame.LocaliseVertices();
            m_OuterFrame.Refresh();
        }

        if(m_InnerFrame != null)
        {
            m_InnerFrame.transform.position = position;
            m_InnerFrame.LocaliseVertices();
            m_InnerFrame.Refresh();
        }
        if(m_Pane != null)
        {
            m_Pane.transform.position = position;
            m_Pane.LocaliseVertices();
            m_Pane.Refresh();
        }
        if(m_LeftShutter != null)
        {
            m_LeftShutter.transform.position = position;
            m_LeftShutter.LocaliseVertices();
            m_LeftShutter.Refresh();
        }
        if (m_RightShutter != null)
        {
            m_RightShutter.transform.position = position;
            m_RightShutter.LocaliseVertices();
            m_RightShutter.Refresh();
        }
    }

    public void Build()
    {
        transform.DeleteChildren();


        if (m_Data.ActiveElements == WindowElement.Nothing)
            return;

        Vector3[] points = m_Data.IsOuterFrameActive ? ScaledControlPoints : m_Data.ControlPoints;

        if (m_Data.IsOuterFrameActive)
        {
            m_OuterFrame = ProBuilderMesh.Create();
            m_OuterFrame.name = "Outer Frame";
            m_OuterFrame.transform.SetParent(transform, false);
            m_OuterFrame.GetComponent<Renderer>().sharedMaterial = m_Data.OuterFrameMaterial;

            List<IList<Vector3>> holePoints = new();
            holePoints.Add(ScaledControlPoints);
            m_OuterFrame.CreateShapeFromPolygon(m_Data.ControlPoints, m_Data.Forward, holePoints);
            ProBuilderMesh backFrame = Instantiate(m_OuterFrame);
            backFrame.faces[0].Reverse();
            m_OuterFrame.Extrude(m_OuterFrame.faces, ExtrudeMethod.FaceNormal, m_Data.OuterFrameDepth);
            CombineMeshes.Combine(new ProBuilderMesh[] { m_OuterFrame, backFrame }, m_OuterFrame);
            DestroyImmediate(backFrame.gameObject);
            m_OuterFrame.ToMesh();
            m_OuterFrame.Refresh();
        }

        if(m_Data.IsInnerFrameActive)
        {
            // Inner Frame
            m_InnerFrame = MeshMaker.PolyFrameGrid(points, m_Height, m_Width, m_Data.InnerFrameScale, m_Data.InnerFrameColumns, m_Data.InnerFrameRows, m_Position, m_Data.Forward);
            ProBuilderMesh gridCover = Instantiate(m_InnerFrame);
            gridCover.faces[0].Reverse();
            m_InnerFrame.Extrude(m_InnerFrame.faces, ExtrudeMethod.FaceNormal, m_Data.InnerFrameDepth);
            CombineMeshes.Combine(new ProBuilderMesh[] { m_InnerFrame, gridCover }, m_InnerFrame);
            DestroyImmediate(gridCover.gameObject);
            m_InnerFrame.transform.SetParent(transform, false);
            m_InnerFrame.name = "Inner Frame";
            m_InnerFrame.GetComponent<Renderer>().sharedMaterial = m_Data.InnerFrameMaterial;

        }

        if (m_Data.IsPaneActive)
        {
            // Pane
            m_Pane = ProBuilderMesh.Create();
            m_Pane.transform.SetParent(transform, false);
            m_Pane.name = "Pane";
            m_Pane.GetComponent<Renderer>().sharedMaterial = m_Data.PaneMaterial;
            m_Pane.CreateShapeFromPolygon(points, m_Data.Forward);
            ProBuilderMesh backPane = Instantiate(m_Pane);
            backPane.faces[0].Reverse();
            m_Pane.Extrude(m_Pane.faces, ExtrudeMethod.FaceNormal, m_Data.PaneDepth);
            CombineMeshes.Combine(new ProBuilderMesh[] { m_Pane, backPane }, m_Pane);
            DestroyImmediate(backPane.gameObject);
            m_Pane.ToMesh();
            m_Pane.Refresh();
        }

        if(m_Data.AreShuttersActive)
        {
            // Shutters
            List<IList<Vector3>> shutterVertices = MeshMaker.SpiltPolygon(points, m_Width, m_Height, 2, 1, m_Position, m_Data.Forward);

            for (int i = 0; i < shutterVertices.Count; i++)
            {
                IList<Vector3> shutter = shutterVertices[i];

                for (int j = 0; j < shutter.Count; j++)
                {
                    shutter[j] += m_Data.Forward * m_Data.OuterFrameDepth;
                }

                shutterVertices[i] = shutter;
            }

            Vector3 right = Vector3.Cross(m_Data.Forward, Vector3.up);

            DoorData rightShutterData = new DoorData()
            {
                ControlPoints = shutterVertices[0].ToArray(),
                Forward = m_Data.Forward,
                Right = right,
                Depth = m_Data.ShuttersDepth,
                Scale = 1,
                HingePoint = TransformPoint.Right,
                HingeEulerAngles = -Vector3.up * m_Data.ShuttersAngle,
                Material = m_Data.ShuttersMaterial
            };

            DoorData leftShutterData = new DoorData()
            {
                ControlPoints = shutterVertices[1].ToArray(),
                Forward = m_Data.Forward,
                Right = right,
                Depth = m_Data.ShuttersDepth,
                Scale = 1,
                HingePoint = TransformPoint.Left,
                HingeEulerAngles = Vector3.up * m_Data.ShuttersAngle,
                Material = m_Data.ShuttersMaterial
            };

            m_RightShutter = ProBuilderMesh.Create();
            m_RightShutter.name = "Right shutter";
            m_RightShutter.transform.SetParent(transform, false);
            Door rightShutter = m_RightShutter.AddComponent<Door>();
            rightShutter.Initialize(rightShutterData).Build();

            m_LeftShutter = ProBuilderMesh.Create();
            m_LeftShutter.name = "Left shutter";
            m_LeftShutter.transform.SetParent(transform, false);
            Door leftShutter = m_LeftShutter.AddComponent<Door>();
            leftShutter.Initialize(leftShutterData).Build();
        }
    }
}
