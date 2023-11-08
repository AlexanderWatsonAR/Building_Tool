using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HandleUtil = UnityEditor.HandleUtility;
using UnityEditor.ProBuilder;
using UnityEngine.Rendering;
using System.Linq;

// Add Poly path stuff
[CustomEditor(typeof(Building))]
public class BuildingEditor : Editor
{
    private Building m_Building;
    private PolyPath m_PolyPath;
    private SerializedProperty m_PolyPathProp;
    private Vector3 m_MousePosition;
    private Color m_CP_Valid = Color.green;
    private Color m_CP_Invalid = Color.red;
    private bool m_IsValidPoint;
    MouseCursor m_MouseCursor;
    private Vector3[] m_GlobalControlPointPositions;
    [SerializeField] private PolyMode m_PolyMode = PolyMode.Hide;

    [SerializeField] private bool m_IsValidPolygon;
    [SerializeField] private bool m_IsAHandleSelected;
    [SerializeField] private int m_SelectedHandle = -1;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        m_PolyMode = m_PolyPath.PolyMode;

        switch (m_PolyMode)
        {
            case PolyMode.Draw:
                EditorGUILayout.HelpBox("Click to draw points", MessageType.Info);
                break;
            case PolyMode.Edit:
                if (GUILayout.Button("Quit Edit"))
                {
                    QuitEdit();
                    SceneView.RepaintAll();
                }

                EditorGUI.BeginDisabledGroup(m_SelectedHandle == -1);

                if (GUILayout.Button("Remove Point"))
                {
                    m_Building.PolyPath.RemoveControlPointAt(m_SelectedHandle-1);
                    if(m_Building.PolyPath.IsValidPath())
                    {
                        m_Building.Rebuild();
                    }
                    SceneView.RepaintAll();
                }

                EditorGUI.EndDisabledGroup();

                EditorGUILayout.HelpBox("Move points to update the poly building's shape", MessageType.Info);
                break;
            case PolyMode.Hide:
                if (GUILayout.Button("Edit Poly Building"))
                {
                    m_PolyPath.PolyMode = PolyMode.Edit;
                    SceneView.RepaintAll();
                }
                EditorGUILayout.HelpBox("Editing the shape of the building will erase changes made to the building.", MessageType.Warning);

                break;
        }

        EditorGUI.BeginDisabledGroup(!m_Building.isActiveAndEnabled);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Build"))
        {
            m_Building.Build();
        }

        if (GUILayout.Button("Reset"))
        {
            m_Building.RevertBuilding();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUI.EndDisabledGroup();

        serializedObject.ApplyModifiedProperties();
    }

    // Should I migrate this stuff to the building tool editor?
    private void OnSceneGUI()
    {
        Input();
        Draw();
        Edit();
    }

    private void Input()
    {
        if (m_PolyMode != PolyMode.Draw)
            return;

        Event currentEvent = Event.current;

        Ray ray = HandleUtil.GUIPointToWorldRay(currentEvent.mousePosition);
        float size = HandleUtil.GetHandleSize(ray.GetPoint(1)) * 0.05f;

        // Map the free move handle position to the XZ plane.
        Plane plane = new Plane(Vector3.up, Vector3.zero); // XZ plane (upward)
        bool didHit = plane.Raycast(ray, out float enter);
        Vector3 hit = ray.GetPoint(enter);
        //bool didHit = Physics.Raycast(ray, out RaycastHit hit);

        if (didHit)
        {
            if (ValidatePoint(hit))
            {
                Handles.color = m_CP_Valid;
                m_IsValidPoint = true;
            }
            else
            {
                Handles.color = m_CP_Invalid;
                m_IsValidPoint = false;
            }
        }
        else
        {
            Handles.color = m_CP_Invalid;
            m_IsValidPoint = false;
        }

        Vector3 sceneCamPos = SceneView.lastActiveSceneView.camera.transform.position;
        Vector3 handlePos = ray.GetPoint(1);

        Quaternion handleRot = Quaternion.LookRotation(handlePos.DirectionToTarget(sceneCamPos));

        BuildingHandles.TestSolidCircleHandleCap(-1, handlePos, handleRot, size, currentEvent.type);

        Handles.color = Color.white;

        m_MousePosition = ray.GetPoint(1);

        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
        {
            if (didHit)
            {
                if (m_PolyPath.ControlPointCount >= 3)
                {
                    float dis = Vector3.Distance(hit, m_PolyPath.GetPositionAt(0));

                    if (dis <= 1)
                    {
                        m_PolyMode = PolyMode.Hide;
                        m_PolyPath.PolyMode = PolyMode.Hide;
                        m_PolyPath.IsValidPath();
                        m_Building.Initialize().Build();
                        
                        return;
                    }
                }

                if (m_IsValidPoint)
                {
                    m_PolyPath.AddControlPoint(hit);
                }
            }
        }

        if (currentEvent.type == EventType.MouseMove)
            SceneView.RepaintAll();
    }

    private void Draw()
    {
        if (m_PolyPath.ControlPointCount == 0 || m_PolyMode != PolyMode.Draw)
            return;

        DrawHandles();
        ConnectTheDots();

        if (m_PolyMode == PolyMode.Draw)
        {
            Handles.color = m_IsValidPoint ? m_CP_Valid : m_CP_Invalid;
            Handles.DrawDottedLine(m_GlobalControlPointPositions[^1], m_MousePosition, 1);
            Handles.color = Color.white;
        }
    }

    private void Edit()
    {
        if (m_PolyMode != PolyMode.Edit || m_PolyPath.ControlPointCount == 0)
            return;

        Handles.color = m_Building.PolyPath.IsPathValid ? Color.white : m_CP_Invalid;
        

        DrawHandles();
        ConnectTheDots();

        if (m_PolyPath.ControlPointCount >= 3 && m_PolyMode != PolyMode.Draw)
        {
            Handles.DrawAAPolyLine(m_GlobalControlPointPositions[0], m_GlobalControlPointPositions[^1]);
        }

    }

    private void DrawHandles()
    {
        Vector3[] controlPoints = m_Building.ControlPoints.GetPositions();
        m_GlobalControlPointPositions = m_Building.transform.TransformPoints(controlPoints).ToArray();

        Vector3 centre = UnityEngine.ProBuilder.Math.Average(m_GlobalControlPointPositions);

        for (int i = 0; i < m_GlobalControlPointPositions.Length; i++)
        {
            float size = HandleUtil.GetHandleSize(m_PolyPath.GetPositionAt(i)) * 0.05f;

            Color handleColour = Handles.color;

            Handles.color = m_SelectedHandle == i+1 ? Color.yellow : handleColour;
            Vector3 globalPoint = Handles.FreeMoveHandle(i+1, m_GlobalControlPointPositions[i], Quaternion.identity, size, Vector3.up, BuildingHandles.TestSolidCircleHandleCap);

            Handles.color = handleColour;
            Vector3 localPoint = m_Building.transform.InverseTransformPoint(globalPoint);


            // How we detect if the control point has changed.
            if (localPoint != m_PolyPath.GetPositionAt(i))
            {
                // Map the free move handle position to the XZ plane.
                Plane plane = new Plane(Vector3.up, Vector3.zero); // XZ plane (upward)
                Ray ray = HandleUtil.GUIPointToWorldRay(Event.current.mousePosition);

                if (plane.Raycast(ray, out float enter))
                {
                    Vector3 rayPoint = ray.GetPoint(enter);
                    Vector3 localRay = m_Building.transform.InverseTransformPoint(rayPoint);

                    m_Building.PolyPath.SetPositionAt(i, rayPoint);
                   // Debug.Log("Handle: " + i + " Position: " + localRay);
                }

            }

            // Highlight the first control point. The first point is important for shape recognition.
            if (i == 0)
            {
                handleColour = Handles.color;
                Handles.color = Color.blue;
                Handles.DrawWireDisc(globalPoint, Vector3.up, size*1.5f, 5f);
                Handles.color = handleColour;
            }
        }

        //if (m_PolyMode != PolyMode.Edit)
        //    return;

        //Vector3 closestPoint = HandleUtil.ClosestPointToPolyLine(m_GlobalControlPointPositions);

        //Handles.DotHandleCap(-1, closestPoint, Quaternion.identity, 0.1f, Event.current.type);

       // m_IsAHandleSelected = GUIUtility.hotControl > 0 && GUIUtility.hotControl < m_GlobalControlPointPositions.Length +1? true : false;

        if(GUIUtility.hotControl > 0 && GUIUtility.hotControl < m_GlobalControlPointPositions.Length + 1)
        {
            m_SelectedHandle = GUIUtility.hotControl;
        }

    }

    private void ConnectTheDots()
    {
        Handles.DrawAAPolyLine(m_GlobalControlPointPositions.ToArray());
    }

    private bool ValidatePoint(Vector3 point)
    {
        if (m_PolyPath.ControlPointCount == 0)
            return true;

        if (m_PolyPath.ControlPointCount >= 3)
        {
            if (Vector3.Distance(point, m_PolyPath.GetPositionAt(0)) <= 1)
            {
                return true;
            }
        }

        for (int i = 0; i < m_PolyPath.ControlPointCount; i++)
        {
            float dis = Vector3.Distance(point, m_PolyPath.GetPositionAt(i));

            if (dis <= 1)
            {
                return false;
            }
        }

        for (int i = 0; i < m_PolyPath.ControlPointCount - 1; i++)
        {
            float dis = HandleUtil.DistancePointLine(point, m_PolyPath.GetPositionAt(i), m_PolyPath.GetPositionAt(i + 1));

            if (dis <= 1)
            {
                return false;
            }
        }

        for (int i = 0; i < m_PolyPath.ControlPointCount; i++)
        {
            int next = m_PolyPath.ControlPoints.GetNext(i);

            if (Extensions.DoLinesIntersect(m_PolyPath.GetLastPosition(), point, m_PolyPath.GetPositionAt(i), m_PolyPath.GetPositionAt(next), out Vector3 intersection, false))
            {
                if (intersection == m_PolyPath.GetLastPosition())
                    return true;

                return false;
            }
        }

        return true;
    }

    private void OnEnable()
    {
        m_Building = (Building)target;
        m_PolyPath = m_Building.PolyPath;
        m_PolyPathProp = serializedObject.FindProperty("m_BuildingPolyPath");
    }
    private void OnDisable()
    {
        QuitEdit();

    }

    private void QuitEdit()
    {
        m_PolyPath.PolyMode = PolyMode.Hide;
        m_SelectedHandle = -1;
    }

    //private void OnSelectionChanged()
    //{
    //    Selection.activeGameObject = m_Building.gameObject;

    //    EditorApplication.delayCall += () =>
    //    {
    //        Selection.activeGameObject = m_Building.gameObject;

    //        if (Selection.activeGameObject != null)
    //            Debug.Log(Selection.activeGameObject.name);

    //    };
    //}
}
