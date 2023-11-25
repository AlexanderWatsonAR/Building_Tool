using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HandleUtil = UnityEditor.HandleUtility;
using UnityEditor.ProBuilder;
using UnityEngine.Rendering;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

// Add Poly path stuff
[CustomEditor(typeof(Building))]
public class BuildingEditor : Editor
{
    private Building m_Building;
    private PolyPath m_Path;
    private SerializedProperty m_Data;
    private Vector3 m_MousePosition;
    private Color m_CP_Valid = Color.green;
    private Color m_CP_Invalid = Color.red;
    private bool m_IsValidPoint;
    MouseCursor m_MouseCursor;
    private Vector3[] m_GlobalControlPointPositions;

    [SerializeField] private bool m_IsValidPolygon;
    [SerializeField] private bool m_IsAHandleSelected;
    [SerializeField] private int m_SelectedHandle = -1;

    //public override void OnInspectorGUI()
    //{
    //    serializedObject.Update();

    //    m_PolyMode = m_PolyPath.PolyMode;

    //    switch (m_PolyMode)
    //    {
    //        case PolyMode.Draw:
    //            EditorGUILayout.HelpBox("Click to draw points", MessageType.Info);
    //            break;
    //        case PolyMode.Edit:
    //            if (GUILayout.Button("Quit Edit"))
    //            {
    //                QuitEdit();
    //                SceneView.RepaintAll();
    //            }

    //            EditorGUI.BeginDisabledGroup(m_SelectedHandle == -1);

    //            if (GUILayout.Button("Remove Point"))
    //            {
    //                m_Building.Data.Path.RemoveControlPointAt(m_SelectedHandle-1);
    //                if(m_Building.Data.Path.IsValidPath())
    //                {
    //                    m_Building.Build();
    //                }
    //                SceneView.RepaintAll();
    //            }

    //            EditorGUI.EndDisabledGroup();

    //            EditorGUILayout.HelpBox("Move points to update the poly building's shape", MessageType.Info);
    //            break;
    //        case PolyMode.Hide:
    //            if (GUILayout.Button("Edit Poly Building"))
    //            {
    //                m_PolyPath.PolyMode = PolyMode.Edit;
    //                SceneView.RepaintAll();
    //            }
    //            EditorGUILayout.HelpBox("Editing the shape of the building will erase changes made to the building.", MessageType.Warning);

    //            //SerializedProperty storeys = m_Data.FindPropertyRelative("m_Storeys");

    //            //EditorGUILayout.PropertyField(storeys);

    //            break;
    //    }

    //    EditorGUI.BeginDisabledGroup(!m_Building.isActiveAndEnabled);

    //    EditorGUILayout.BeginHorizontal();

    //    if (GUILayout.Button("Build"))
    //    {
    //        m_Building.Build();
    //    }

    //    if (GUILayout.Button("Reset"))
    //    {
    //        m_Building.RevertBuilding();
    //    }

    //    EditorGUILayout.EndHorizontal();

    //    EditorGUI.EndDisabledGroup();

    //    if(serializedObject.ApplyModifiedProperties())
    //    {
    //        m_Building.Data.AssignStoreyID();
    //        m_Building.Build();
    //    }
    //}

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement container = new VisualElement();

        VisualElement polymodeContainer = new VisualElement();

        DisplayMessages(polymodeContainer);

        m_Path.OnPolyModeChanged += (pathMode) => DisplayMessages(polymodeContainer);

        VisualElement horizontalWrapper = new VisualElement()
        {
            style =
            {
                flexDirection = FlexDirection.Row,
                flexGrow = 1
            }
        };

        Button build_btn = new Button(() => m_Building.Build())
        {
            text = "Build"
        };
        Button reset_btn = new Button(() => m_Building.Build())
        {
            text = "Reset"
        };

        horizontalWrapper.Add(build_btn);
        horizontalWrapper.Add(reset_btn);

        container.Add(polymodeContainer);
        container.Add(horizontalWrapper);

        return container;
    }

    private void DisplayMessages(VisualElement element)
    {
        element.Clear();

        switch (m_Path.PolyMode)
        {
            case PolyMode.Draw:
                element.Add(new HelpBox("Click to draw points", HelpBoxMessageType.Info));
                break;
            case PolyMode.Edit:
                {
                    Button quit_btn = new Button(() => { QuitEdit(); SceneView.RepaintAll(); });
                    quit_btn.text = "Quit Edit";

                    Button remove_btn = new Button(() => 
                    {
                        m_Building.Data.Path.RemoveControlPointAt(m_SelectedHandle - 1);

                        if (m_Building.Data.Path.IsValidPath())
                        {
                            m_Building.Build();
                        }

                        SceneView.RepaintAll();
                    });
                    remove_btn.text = "Remove Point";
                    remove_btn.SetEnabled(m_SelectedHandle == -1);

                    element.Add(quit_btn);
                    element.Add(remove_btn);
                    element.Add(new HelpBox("Move points to update the poly building's shape", HelpBoxMessageType.Info));
                }
                break;
            case PolyMode.Hide:
                {
                    Button edit_btn = new Button(() =>
                    {
                        m_Path.PolyMode = PolyMode.Edit;
                        SceneView.RepaintAll();
                    });
                    edit_btn.text = "Edit Building Path";

                    element.Add(edit_btn);
                    element.Add(new HelpBox("Editing the shape of the building will erase changes made to the building", HelpBoxMessageType.Warning));

                    SerializedProperty storeys = m_Data.FindPropertyRelative("m_Storeys");
                    PropertyField storeysField = new PropertyField(storeys);
                    storeysField.BindProperty(storeys);

                    element.Add(storeysField);
                }
                break;
        }
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
        if (m_Path.PolyMode != PolyMode.Draw)
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
                if (m_Path.ControlPointCount >= 3)
                {
                    float dis = Vector3.Distance(hit, m_Path.GetPositionAt(0));

                    if (dis <= 1)
                    {
                        m_Path.PolyMode = PolyMode.Hide;
                        if(m_Path.IsValidPath())
                        {
                            m_Path.CalculateForwards();
                            m_Building.Initialize(new BuildingData(m_Path)).Build();
                        }
                        
                        
                        return;
                    }
                }

                if (m_IsValidPoint)
                {
                    m_Path.AddControlPoint(hit);
                }
            }
        }

        if (currentEvent.type == EventType.MouseMove)
            SceneView.RepaintAll();
    }

    private void Draw()
    {
        if (m_Path.ControlPointCount == 0 || m_Path.PolyMode != PolyMode.Draw)
            return;

        DrawHandles();
        ConnectTheDots();

        if (m_Path.PolyMode == PolyMode.Draw)
        {
            Handles.color = m_IsValidPoint ? m_CP_Valid : m_CP_Invalid;
            Handles.DrawDottedLine(m_GlobalControlPointPositions[^1], m_MousePosition, 1);
            Handles.color = Color.white;
        }
    }

    private void Edit()
    {
        if (m_Path.PolyMode != PolyMode.Edit || m_Path.ControlPointCount == 0)
            return;

        Handles.color = m_Building.Data.Path.IsPathValid ? Color.white : m_CP_Invalid;
        

        DrawHandles();
        ConnectTheDots();

        if (m_Path.ControlPointCount >= 3 && m_Path.PolyMode != PolyMode.Draw)
        {
            Handles.DrawAAPolyLine(m_GlobalControlPointPositions[0], m_GlobalControlPointPositions[^1]);
        }

    }

    private void DrawHandles()
    {
        Vector3[] controlPoints = m_Building.Data.Path.ControlPoints.GetPositions();
        m_GlobalControlPointPositions = m_Building.transform.TransformPoints(controlPoints).ToArray();

        Vector3 centre = UnityEngine.ProBuilder.Math.Average(m_GlobalControlPointPositions);

        for (int i = 0; i < m_GlobalControlPointPositions.Length; i++)
        {
            float size = HandleUtil.GetHandleSize(m_Path.GetPositionAt(i)) * 0.05f;

            Color handleColour = Handles.color;

            Handles.color = m_SelectedHandle == i+1 ? Color.yellow : handleColour;
            Vector3 globalPoint = Handles.FreeMoveHandle(i+1, m_GlobalControlPointPositions[i], Quaternion.identity, size, Vector3.up, BuildingHandles.TestSolidCircleHandleCap);

            Handles.color = handleColour;
            Vector3 localPoint = m_Building.transform.InverseTransformPoint(globalPoint);


            // How we detect if the control point has changed.
            if (localPoint != m_Path.GetPositionAt(i))
            {
                // Map the free move handle position to the XZ plane.
                Plane plane = new Plane(Vector3.up, Vector3.zero); // XZ plane (upward)
                Ray ray = HandleUtil.GUIPointToWorldRay(Event.current.mousePosition);

                if (plane.Raycast(ray, out float enter))
                {
                    Vector3 rayPoint = ray.GetPoint(enter);
                    Vector3 localRay = m_Building.transform.InverseTransformPoint(rayPoint);

                    m_Building.Data.Path.SetPositionAt(i, rayPoint);
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
        if (m_Path.ControlPointCount == 0)
            return true;

        if (m_Path.ControlPointCount >= 3)
        {
            if (Vector3.Distance(point, m_Path.GetPositionAt(0)) <= 1)
            {
                return true;
            }
        }

        for (int i = 0; i < m_Path.ControlPointCount; i++)
        {
            float dis = Vector3.Distance(point, m_Path.GetPositionAt(i));

            if (dis <= 1)
            {
                return false;
            }
        }

        for (int i = 0; i < m_Path.ControlPointCount - 1; i++)
        {
            float dis = HandleUtil.DistancePointLine(point, m_Path.GetPositionAt(i), m_Path.GetPositionAt(i + 1));

            if (dis <= 1)
            {
                return false;
            }
        }

        for (int i = 0; i < m_Path.ControlPointCount; i++)
        {
            int next = m_Path.ControlPoints.GetNext(i);

            if (Extensions.DoLinesIntersect(m_Path.GetLastPosition(), point, m_Path.GetPositionAt(i), m_Path.GetPositionAt(next), out Vector3 intersection, false))
            {
                if (intersection == m_Path.GetLastPosition())
                    return true;

                return false;
            }
        }

        return true;
    }

    private void OnEnable()
    {
        m_Building = (Building)target;
        m_Path = m_Building.Data.Path;
        m_Data = serializedObject.FindProperty("m_Data");
    }
    private void OnDisable()
    {
        QuitEdit();

    }
    private void QuitEdit()
    {
        m_Path.PolyMode = PolyMode.Hide;
        m_SelectedHandle = -1;
    }
}
