using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Add Poly path stuff
[CustomEditor(typeof(Building))]
public class BuildingEditor : Editor
{
    private Building m_Building;
    private PolyPath m_PolyPath;
    private SerializedProperty m_PolyPathProp;
    private bool isDrawingPrevious;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        EditorGUILayout.BeginHorizontal();
        isDrawingPrevious = m_PolyPath.IsDrawing;

        if (GUILayout.Button("Start Draw"))
        {
            Selection.selectionChanged += OnSelectionChanged;
            Debug.Log("Start Draw");
            m_PolyPath.IsDrawing = true;
        }

        if (GUILayout.Button("Stop Draw"))
        {
            Selection.selectionChanged -= OnSelectionChanged;
            Debug.Log("Stop Draw");
            m_PolyPath.CalculateForwards();
            m_PolyPath.IsDrawing = false;
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        if(GUILayout.Button("Build"))
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

    private void OnSceneGUI()
    {
        Input();
        Draw();
    }

    private void Input()
    {
        bool isDrawing = m_PolyPath.IsDrawing;

        if (isDrawingPrevious && !isDrawing)
        {
            Selection.selectionChanged -= OnSelectionChanged;
            Debug.Log("Drawing Stopped");
        }

        if (!isDrawing)
            return;

        Event currentEvent = Event.current;

        Ray ray = UnityEditor.HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);

        Handles.DotHandleCap(-1, ray.GetPoint(1), Quaternion.identity, 0.01f, currentEvent.type);

        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 point = m_Building.transform.InverseTransformPoint(hit.point);
                m_PolyPath.AddControlPoint(point);
            }
        }

        if (currentEvent.type == EventType.MouseMove)
            SceneView.RepaintAll();
    }

    private void Draw()
    {
        if (m_PolyPath.ControlPointCount == 0)
            return;

        List<Vector3> localPositions = m_PolyPath.LocalPositions(m_Building.transform);

        for (int i = 0; i < localPositions.Count; i++)
        {
            float size = UnityEditor.HandleUtility.GetHandleSize(m_PolyPath.ControlPoints[i].Position) * 0.04f;

            Handles.FreeMoveHandle(localPositions[i], Quaternion.identity, size, Vector3.up, Handles.DotHandleCap);
        }

        if (m_PolyPath.ControlPointCount <= 1)
            return;

        Handles.DrawAAPolyLine(localPositions.ToArray());

        if (m_PolyPath.ControlPointCount >= 3)
        {
            Handles.DrawAAPolyLine(localPositions[0], localPositions[^1]);
        }

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
