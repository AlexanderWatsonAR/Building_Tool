using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UHandleUtil = UnityEditor.HandleUtility;
using static UnityEditor.PlayerSettings;
using UnityEditor.PackageManager.UI;

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
    private List<Vector3> m_LocalControlPointPositions;
    [SerializeField] private PolyMode m_PolyMode = PolyMode.Hide;

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
                    m_PolyPath.PolyMode = PolyMode.Hide;
                    SceneView.RepaintAll();
                }
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

        Ray ray = UHandleUtil.GUIPointToWorldRay(currentEvent.mousePosition);
        float size = UHandleUtil.GetHandleSize(ray.GetPoint(1)) * 0.05f;

        bool didHit = Physics.Raycast(ray, out RaycastHit hit);

        if (didHit)
        {
            Vector3 hp = m_Building.transform.InverseTransformPoint(hit.point);

            if (ValidatePoint(hp))
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

        Handles.DotHandleCap(-1, ray.GetPoint(1), Quaternion.identity, size, currentEvent.type);
        Handles.color = Color.white;

        m_MousePosition = ray.GetPoint(1);

        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
        {
            if (didHit)
            {
                Vector3 point = m_Building.transform.InverseTransformPoint(hit.point);

                if (m_PolyPath.ControlPointCount >= 3)
                {
                    float dis = Vector3.Distance(point, m_PolyPath.GetPositionAt(0));

                    if (dis <= 1)
                    {
                        m_PolyMode = PolyMode.Hide;
                        m_PolyPath.PolyMode = PolyMode.Hide;
                        m_PolyPath.ValidateControlPoints();
                        m_Building.Initialize().Build();
                        return;
                    }
                }

                if (ValidatePoint(point))
                {
                    m_PolyPath.AddControlPoint(point);
                    m_IsValidPoint = true;
                }
                else
                {
                    m_IsValidPoint = false;
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
            if (m_IsValidPoint)
            {
                Handles.color = m_CP_Valid;
            }
            else
            {
                Handles.color = m_CP_Invalid;
            }
            Handles.DrawDottedLine(m_LocalControlPointPositions[^1], m_MousePosition, 1);
            Handles.color = Color.white;
        }
    }

    private void Edit()
    {
        if (m_PolyMode != PolyMode.Edit || m_PolyPath.ControlPointCount == 0)
            return;

        DrawHandles();
        ConnectTheDots();

        if (m_PolyPath.ControlPointCount >= 3 && m_PolyMode != PolyMode.Draw)
        {
            Handles.DrawAAPolyLine(m_LocalControlPointPositions[0], m_LocalControlPointPositions[^1]);
        }

    }

    private void DrawHandles()
    {
        m_LocalControlPointPositions = m_PolyPath.LocalPositions(m_Building.transform);
        float y = m_LocalControlPointPositions[0].y;

        for (int i = 0; i < m_LocalControlPointPositions.Count; i++)
        {
            float size = UHandleUtil.GetHandleSize(m_PolyPath.GetPositionAt(i)) * 0.05f;

            Vector3 pos = Handles.FreeMoveHandle(m_LocalControlPointPositions[i], Quaternion.identity, size, Vector3.up, Handles.DotHandleCap);

            // This is for when the user is repositioning the handle in the scene.
            pos = new Vector3(pos.x, y, pos.z);
            Vector3 worldPos = m_Building.transform.TransformPoint(pos);

            if (worldPos != m_PolyPath.GetPositionAt(i))
            {
                m_Building.PolyPath.SetPositionAt(i, worldPos);
            }
        }
    }

    private void ConnectTheDots()
    {
        Handles.DrawAAPolyLine(m_LocalControlPointPositions.ToArray());
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
            float dis = UHandleUtil.DistancePointLine(point, m_PolyPath.GetPositionAt(i), m_PolyPath.GetPositionAt(i + 1));

            if (dis <= 1)
            {
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

    private void OnSelectionChanged()
    {
        Selection.activeGameObject = m_Building.gameObject;

        EditorApplication.delayCall += () =>
        {
            Selection.activeGameObject = m_Building.gameObject;

            if (Selection.activeGameObject != null)
                Debug.Log(Selection.activeGameObject.name);

        };
    }
}
