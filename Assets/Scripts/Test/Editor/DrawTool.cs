using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using HandleUtil = UnityEditor.HandleUtility;
using OnlyInvalid.ProcGenBuilding.Building;
using System.Linq;


[EditorTool("Draw Path", typeof(IDrawable))]
[Icon("Packages/com.unity.probuilder/Content/Icons/Tools/PolyShape/CreatePolyShape.png")]
public class DrawTool : EditorTool
{
    public static DrawState DrawState;

    static MouseCursor m_Cursor;
    static IDrawable m_Drawable;

    Vector3 m_MousePosition;
    Color m_Valid = Color.green;
    Color m_Invalid = Color.red;
    // Vector3[] m_GlobalControlPointPositions;

    //[SerializeField] bool m_IsValidPolygon;
    //[SerializeField] bool m_IsAHandleSelected;
    //[SerializeField] int m_SelectedHandle = -1;

    public static void Activate(DrawState drawState)
    {
        if (Selection.activeGameObject.TryGetComponent(out IDrawable drawable))
        {
            ToolManager.SetActiveTool<DrawTool>();
            DrawState = drawState;
            m_Drawable = drawable;
        }
    }

    public override void OnActivated()
    {
        m_Cursor = MouseCursor.ArrowPlus;
    }

    public override void OnWillBeDeactivated()
    {
        m_Cursor = MouseCursor.Arrow;
    }

    public override void OnToolGUI(EditorWindow window)
    {
        if (window is not SceneView)
            return;

        if (Event.current.type == EventType.Repaint)
        {
            Rect sceneViewRect = window.position;
            sceneViewRect.x = 0;
            sceneViewRect.y = 0;
            EditorGUIUtility.AddCursorRect(sceneViewRect, m_Cursor);
        }

        Input();
        Draw();
        Edit();

    }

    void Input()
    {
        if (DrawState != DrawState.Draw)
            return;

        Event currentEvent = Event.current;

        Ray ray = HandleUtil.GUIPointToWorldRay(currentEvent.mousePosition);
        float size = HandleUtil.GetHandleSize(ray.GetPoint(1)) * 0.05f;

        bool didHit = m_Drawable.Path.Plane.Raycast(ray, out float enter);
        Vector3 hit = ray.GetPoint(enter);

        if (didHit)
        {
            if (m_Drawable.Path.CanPointBeAdded(hit))
            {
                Handles.color = m_Valid;
            }
            else
            {
                Handles.color = m_Invalid;
            }
        }
        else
        {
            Handles.color = m_Invalid;
        }

        Vector3 sceneCamPos = SceneView.lastActiveSceneView.camera.transform.position;
        Vector3 handlePos = ray.GetPoint(1);

        Quaternion handleRot = Quaternion.LookRotation(handlePos.DirectionToTarget(sceneCamPos));

        BuildingHandles.SolidCircleHandleCap(-1, handlePos, handleRot, size, currentEvent.type);

        Handles.color = Color.white;

        m_MousePosition = ray.GetPoint(1);

        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
        {
            if (!didHit)
                return;

            m_Drawable.Path.AddPointToPath(hit);
        }

        if (currentEvent.type == EventType.MouseMove)
            SceneView.RepaintAll();
    }

    void Draw()
    {
        if (m_Drawable.Path.PathPointsCount == 0 || DrawState != DrawState.Draw)
            return;

        DrawHandles();
        ConnectTheDots();

        if (DrawState == DrawState.Draw)
        {
            Vector3[] globalPositions = m_Drawable.Path.Positions;
            Handles.color = m_Drawable.Path.IsPathValid ? m_Valid : m_Invalid;
            Handles.DrawDottedLine(globalPositions[^1], m_MousePosition, 1);
            Handles.color = Color.white;
        }
    }

    void Edit()
    {
        if (DrawState != DrawState.Edit || m_Drawable.Path.PathPointsCount == 0)
            return;

        Handles.color = m_Drawable.Path.IsPathValid ? Color.white : m_Invalid;

        DrawHandles();
        ConnectTheDots();

        if (m_Drawable.Path.PathPointsCount >= 3 && DrawState != DrawState.Draw)
        {
            Vector3[] globalPositions = m_Drawable.Path.Positions;
            Handles.DrawAAPolyLine(globalPositions[0], globalPositions[^1]);
        }

    }

    private void DrawHandles()
    {
        Vector3[] globalPositions = m_Drawable.Path.Positions;

        for (int i = 0; i < globalPositions.Length; i++)
        {
            float size = HandleUtil.GetHandleSize(m_Drawable.Path.GetPositionAt(i)) * 0.05f;

            Vector3 globalPoint = Handles.FreeMoveHandle(i + 1, globalPositions[i], size, Vector3.up, BuildingHandles.SolidCircleHandleCap);

            // How we detect if the control point has changed.
            if (globalPoint != m_Drawable.Path.GetPositionAt(i))
            {
                Ray ray = HandleUtil.GUIPointToWorldRay(Event.current.mousePosition);

                if (m_Drawable.Path.Plane.Raycast(ray, out float enter))
                {
                    Vector3 rayPoint = ray.GetPoint(enter);

                    m_Drawable.Path.SetPositionAt(rayPoint, i);
                }

            }

            //// Highlight the first control point. The first point is important for shape recognition.
            //if (i == 0)
            //{
            //    handleColour = Handles.color;
            //    Handles.color = Color.blue;
            //    Handles.DrawWireDisc(globalPoint, Vector3.up, size * 1.5f, 5f);
            //    Handles.color = handleColour;
            //}
        }

        //if (GUIUtility.hotControl > 0 && GUIUtility.hotControl < m_GlobalControlPointPositions.Length + 1)
        //{
        //    m_SelectedHandle = GUIUtility.hotControl;
        //}

    }
    private void ConnectTheDots()
    {
        Handles.DrawAAPolyLine(m_Drawable.Path.Positions);
    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
        m_Drawable = null;
    }

}
