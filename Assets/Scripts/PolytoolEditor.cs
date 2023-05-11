using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.EditorCoroutines.Editor;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;
using UnityEditor.ProBuilder;
using UnityEditor.EditorTools;

[CustomEditor(typeof(Polytool))]
public class PolytoolEditor : Editor
{
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Polytool polytool;
    [SerializeField] private bool isDrawingPrevious;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();

        polytool = (Polytool)target;


        EditorGUILayout.BeginHorizontal();
        isDrawingPrevious = serializedObject.FindProperty("m_IsDrawing").boolValue;

        if (GUILayout.Button("Start Draw"))
        {
            Selection.selectionChanged += OnSelectionChanged;
            Debug.Log("Start Draw");
            serializedObject.FindProperty("m_IsDrawing").boolValue = true;
        }

        if (GUILayout.Button("Stop Draw"))
        {
            Selection.selectionChanged -= OnSelectionChanged;
            Debug.Log("Stop Draw");
            polytool.CalculateForwards();
            serializedObject.FindProperty("m_IsDrawing").boolValue = false;
        }
        
        EditorGUILayout.EndHorizontal();
        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        serializedObject.Update();
        bool isDrawing = serializedObject.FindProperty("m_IsDrawing").boolValue;

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
            polytool = (Polytool)target;

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                polytool.AddControlPoint(hit.point);
            }
        }

        if (currentEvent.type == EventType.MouseMove)
            SceneView.RepaintAll();

    }

    private void OnSelectionChanged()
    {
        Selection.activeGameObject = polytool.gameObject;

        EditorApplication.delayCall += () =>
        {
            Selection.activeGameObject = polytool.gameObject;

            if (Selection.activeGameObject != null)
                Debug.Log(Selection.activeGameObject.name);

        };
    }
}
