using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static UnityEditor.PlayerSettings;

// Add Poly path stuff
[CustomEditor(typeof(Building))]
public class BuildingEditor : Editor
{
    private Building m_Building;
    private PolyPath m_PolyPath;
    private SerializedProperty m_PolyPathProp;
    private Vector3 m_MousePosition;
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
                EditorGUILayout.HelpBox("Move points to update the poly building's shape", MessageType.Info);
                break;
            case PolyMode.Show:
                EditorGUILayout.HelpBox("Show", MessageType.Info);
                break;
            case PolyMode.Hide:
                EditorGUILayout.HelpBox("Hide", MessageType.Info);
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

    private void OnSceneGUI()
    {
        Input();
        Draw();
    }

    private void Input()
    {
        if (m_PolyMode != PolyMode.Draw)
            return;

        Event currentEvent = Event.current;
        
        Ray ray = UnityEditor.HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);

        float size = UnityEditor.HandleUtility.GetHandleSize(ray.origin) * 0.175f;

        Handles.DotHandleCap(-1, ray.GetPoint(1), Quaternion.identity, size, currentEvent.type);

        m_MousePosition = ray.GetPoint(1);

        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 point = m_Building.transform.InverseTransformPoint(hit.point);

                if(m_PolyPath.ControlPointCount >= 3)
                {
                    float dis = Vector3.Distance(point, m_PolyPath.GetPositionAt(0));

                    if(dis <= 1)
                    {
                        m_PolyMode = PolyMode.Edit;
                        m_PolyPath.PolyMode = PolyMode.Edit;
                        m_PolyPath.ReverseControlPoints();
                        m_Building.Initialize().Build();
                        return;
                    }
                }

                m_PolyPath.AddControlPoint(point);
            }
        }

        if (currentEvent.type == EventType.MouseMove)
            SceneView.RepaintAll();
    }

    private void Draw()
    {
        if (m_PolyPath.ControlPointCount == 0 || m_PolyMode == PolyMode.Hide)
            return;

        List<Vector3> localPositions = m_PolyPath.LocalPositions(m_Building.transform);
        float y = localPositions[0].y;

        for (int i = 0; i < localPositions.Count; i++)
        {
            float size = UnityEditor.HandleUtility.GetHandleSize(m_PolyPath.GetPositionAt(i)) * 0.05f;

            Vector3 pos = Handles.FreeMoveHandle(localPositions[i], Quaternion.identity, size, Vector3.up, Handles.DotHandleCap);
            pos = new Vector3(pos.x, y, pos.z);
            Vector3 worldPos = m_Building.transform.TransformPoint(pos);

            if(worldPos != m_PolyPath.GetPositionAt(i))
            {
                m_Building.PolyPath.SetPositionAt(i, worldPos);
            }  
        }

        if (m_PolyPath.ControlPointCount <= 1)
            return;

        if(m_PolyMode == PolyMode.Draw && m_PolyPath.ControlPointCount > 0)
        {
            Handles.DrawDottedLine(localPositions[^1], m_MousePosition, 1);
        }

        Handles.DrawAAPolyLine(localPositions.ToArray());

        if (m_PolyPath.ControlPointCount >= 3 && m_PolyMode != PolyMode.Draw)
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
