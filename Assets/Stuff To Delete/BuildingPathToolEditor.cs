//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.ProBuilder;
//using UnityEngine.ProBuilder.MeshOperations;
//using UnityEditor.ProBuilder;
using UnityEditor;
using UnityEditor.EditorTools;
//using System.Linq;
//using HandleUtil = UnityEditor.HandleUtility;
//using ToolManager = UnityEditor.EditorTools.ToolManager;
//using Unity.VisualScripting;

namespace OnlyInvalid.ProcGenBuilding.Building
{

    //[EditorTool("Edit Building Path", typeof(Building))]
    //public class BuildingPathToolEditor
    //{

    //    // TODO: Come up with a series of attainable goals for completing this project & releasing it to the Unity asset store.
    //}
}

//namespace OnlyInvalid.ProcGenBuilding.Building
//{
//    [EditorTool("Edit Building Path", typeof(Building))]
//    public class BuildingPathToolEditor : EditorTool
//    {
//        MouseCursor m_Cursor;

//        Building m_Building;
//        PolyPath m_Path;
//        Vector3 m_MousePosition;
//        Color m_CP_Valid = Color.green;
//        Color m_CP_Invalid = Color.red;
//        bool m_IsValidPoint;
//        Vector3[] m_GlobalControlPointPositions;

//        static GUIContent m_IconContent;

//        [SerializeField] bool m_IsValidPolygon;
//        [SerializeField] bool m_IsAHandleSelected;
//        [SerializeField] int m_SelectedHandle = -1;

//        public override GUIContent toolbarIcon
//        {
//            get
//            {
//                if (m_IconContent == null)
//                    m_IconContent = new GUIContent()
//                    {
//                        image = EditorGUIUtility.IconContent("Packages/com.unity.probuilder/Content/Icons/Tools/PolyShape/CreatePolyShape.png").image,
//                        text = "Edit Poly Building",
//                        tooltip = "Edit Poly Building"
//                    };
//                return m_IconContent;
//            }
//        }

//        public override void OnToolGUI(EditorWindow window)
//        {
//            if (window is not SceneView)
//                return;

//            if (Event.current.type == EventType.Repaint)
//            {
//                Rect sceneViewRect = window.position;
//                sceneViewRect.x = 0;
//                sceneViewRect.y = 0;
//                EditorGUIUtility.AddCursorRect(sceneViewRect, m_Cursor);
//            }

//            Input();
//            Draw();
//            Edit();
//        }

//        public override void OnActivated()
//        {
//            Building building = target as Building;
//            BuildingData data = building.Data as BuildingData;
//            data.Path.PolyMode = PolyMode.Edit;
//            m_Cursor = MouseCursor.ArrowPlus;
//        }
//        public override void OnWillBeDeactivated()
//        {
//            Building building = target as Building;
//            BuildingData data = building.Data as BuildingData;
//            data.Path.PolyMode = PolyMode.Hide;
//            m_Cursor = MouseCursor.Arrow;
//        }
//        private void Input()
//        {
//            if (m_Path.PolyMode != PolyMode.Draw)
//                return;

//            Event currentEvent = Event.current;

//            Ray ray = HandleUtil.GUIPointToWorldRay(currentEvent.mousePosition);
//            float size = HandleUtil.GetHandleSize(ray.GetPoint(1)) * 0.05f;

//            // Map the free move handle position to the XZ plane.
//            Plane plane = new Plane(Vector3.up, Vector3.zero); // XZ plane (upward)
//            bool didHit = plane.Raycast(ray, out float enter);
//            Vector3 hit = ray.GetPoint(enter);

//            if (didHit)
//            {
//                if (ValidatePoint(hit))
//                {
//                    Handles.color = m_CP_Valid;
//                    m_IsValidPoint = true;
//                }
//                else
//                {
//                    Handles.color = m_CP_Invalid;
//                    m_IsValidPoint = false;
//                }
//            }
//            else
//            {
//                Handles.color = m_CP_Invalid;
//                m_IsValidPoint = false;
//            }

//            Vector3 sceneCamPos = SceneView.lastActiveSceneView.camera.transform.position;
//            Vector3 handlePos = ray.GetPoint(1);

//            Quaternion handleRot = Quaternion.LookRotation(handlePos.DirectionToTarget(sceneCamPos));

//            BuildingHandles.SolidCircleHandleCap(-1, handlePos, handleRot, size, currentEvent.type);

//            Handles.color = Color.white;

//            m_MousePosition = ray.GetPoint(1);

//            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
//            {
//                if (!didHit)
//                    return;

//                if (m_Path.ControlPointCount >= 3)
//                {
//                    float dis = Vector3.Distance(hit, m_Path.GetPositionAt(0));

//                    if (dis <= 1 && m_Path.IsValidPath())
//                    {
//                        m_Path.CalculateForwards();
//                        m_Building.AddStorey("Ground");
//                        m_Building.InitializeRoof();
//                        m_Building.Build();
//                        m_Path.PolyMode = PolyMode.Hide;
//                    }
//                }

//                if (m_IsValidPoint)
//                {
//                    m_Path.AddControlPoint(hit);
//                }

//            }

//            if (currentEvent.type == EventType.MouseMove)
//                SceneView.RepaintAll();
//        }
//        private void Draw()
//        {
//            if (m_Path.ControlPointCount == 0 || m_Path.PolyMode != PolyMode.Draw)
//                return;

//            DrawHandles();
//            ConnectTheDots();

//            if (m_Path.PolyMode == PolyMode.Draw)
//            {
//                Handles.color = m_IsValidPoint ? m_CP_Valid : m_CP_Invalid;
//                Handles.DrawDottedLine(m_GlobalControlPointPositions[^1], m_MousePosition, 1);
//                Handles.color = Color.white;
//            }
//        }
//        private void Edit()
//        {
//            if (m_Path.PolyMode != PolyMode.Edit || m_Path.ControlPointCount == 0)
//                return;

//            BuildingData buildingData = m_Building.Data as BuildingData;

//            Handles.color = buildingData.Path.IsPathValid ? Color.white : m_CP_Invalid;

//            DrawHandles();
//            ConnectTheDots();

//            if (m_Path.ControlPointCount >= 3 && m_Path.PolyMode != PolyMode.Draw)
//            {
//                Handles.DrawAAPolyLine(m_GlobalControlPointPositions[0], m_GlobalControlPointPositions[^1]);
//            }

//        }
//        private void DrawHandles()
//        {
//            BuildingData buildingData = m_Building.Data as BuildingData;

//            Vector3[] controlPoints = buildingData.Path.ControlPoints.GetPositions();
//            m_GlobalControlPointPositions = m_Building.transform.TransformPoints(controlPoints, 0).ToArray();

//            Vector3 centre = UnityEngine.ProBuilder.Math.Average(m_GlobalControlPointPositions);

//            for (int i = 0; i < m_GlobalControlPointPositions.Length; i++)
//            {
//                float size = HandleUtil.GetHandleSize(m_Path.GetPositionAt(i)) * 0.05f;

//                Color handleColour = Handles.color;

//                Handles.color = m_SelectedHandle == i + 1 ? Color.yellow : handleColour;

//                Vector3 globalPoint = Handles.FreeMoveHandle(i + 1, m_GlobalControlPointPositions[i], size, Vector3.up, BuildingHandles.SolidCircleHandleCap);

//                Handles.color = handleColour;
//                Vector3 localPoint = m_Building.transform.InverseTransformPoint(globalPoint);


//                // How we detect if the control point has changed.
//                if (localPoint != m_Path.GetPositionAt(i))
//                {
//                    // Map the free move handle position to the XZ plane.
//                    Plane plane = new Plane(Vector3.up, Vector3.zero); // XZ plane (upward)
//                    Ray ray = HandleUtil.GUIPointToWorldRay(Event.current.mousePosition);

//                    if (plane.Raycast(ray, out float enter))
//                    {
//                        Vector3 rayPoint = ray.GetPoint(enter);
//                        Vector3 localRay = m_Building.transform.InverseTransformPoint(rayPoint);

//                        buildingData.Path.SetPositionAt(i, rayPoint);
//                        // Debug.Log("Handle: " + i + " Position: " + localRay);
//                    }

//                }

//                // Highlight the first control point. The first point is important for shape recognition.
//                if (i == 0)
//                {
//                    handleColour = Handles.color;
//                    Handles.color = Color.blue;
//                    Handles.DrawWireDisc(globalPoint, Vector3.up, size * 1.5f, 5f);
//                    Handles.color = handleColour;
//                }
//            }

//            if (GUIUtility.hotControl > 0 && GUIUtility.hotControl < m_GlobalControlPointPositions.Length + 1)
//            {
//                m_SelectedHandle = GUIUtility.hotControl;
//            }

//        }
//        private bool ValidatePoint(Vector3 point)
//        {
//            if (m_Path.ControlPointCount == 0)
//                return true;

//            if (m_Path.ControlPointCount >= 3)
//            {
//                if (Vector3.Distance(point, m_Path.GetPositionAt(0)) <= 1)
//                {
//                    return true;
//                }
//            }

//            for (int i = 0; i < m_Path.ControlPointCount; i++)
//            {
//                float dis = Vector3.Distance(point, m_Path.GetPositionAt(i));

//                if (dis <= 1)
//                {
//                    return false;
//                }
//            }

//            for (int i = 0; i < m_Path.ControlPointCount - 1; i++)
//            {
//                float dis = HandleUtil.DistancePointLine(point, m_Path.GetPositionAt(i), m_Path.GetPositionAt(i + 1));

//                if (dis <= 1)
//                {
//                    return false;
//                }
//            }

//            for (int i = 0; i < m_Path.ControlPointCount; i++)
//            {
//                int next = m_Path.ControlPoints.GetNext(i);

//                if (Extensions.DoLinesIntersect(m_Path.GetLastPosition(), point, m_Path.GetPositionAt(i), m_Path.GetPositionAt(next), out Vector3 intersection, false))
//                {
//                    if (intersection == m_Path.GetLastPosition())
//                        return true;

//                    return false;
//                }
//            }

//            return true;
//        }
//        private void ConnectTheDots()
//        {
//            Handles.DrawAAPolyLine(m_GlobalControlPointPositions.ToArray());
//        }
//        private void OnEnable()
//        {
//            m_Building = target as Building;
//            m_Path = m_Building.BuildingData.Path;
//            ToolManager.SetActiveTool<BuildingPathToolEditor>();
//        }
//        private void OnDisable()
//        {
//            QuitEdit();
//        }
//        private void QuitEdit()
//        {
//            m_Path.PolyMode = PolyMode.Hide;
//            m_SelectedHandle = -1;
//        }

//    }
//}
