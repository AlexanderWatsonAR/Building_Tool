using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using HandleUtil = UnityEditor.HandleUtility;
using UnityEngine.Events;
using OnlyInvalid.ProcGenBuilding.Building;


namespace OnlyInvalid.ProcGenBuilding.Common
{
    public abstract class DrawTool : EditorTool
    {
        protected static DrawState m_DrawState;
        static MouseCursor m_Cursor;
        protected static IDrawable m_Drawable;
        static bool m_IsHandleHeld;
        static int m_SelectedHandle = -1;
        static Vector3 m_StartPos, m_EndPos;

        Vector3 m_MousePosition;
        Color m_Valid = Color.green;
        Color m_Invalid = Color.red;

        #region Events
        public static UnityEvent<DrawState> OnStateChange = new UnityEvent<DrawState>();
        public static UnityEvent<Vector3> OnHeldStart = new UnityEvent<Vector3>();
        public static UnityEvent<Vector3> OnHeld = new UnityEvent<Vector3>();
        public static UnityEvent<Vector3> OnHeldEnd = new UnityEvent<Vector3>();
        public static UnityEvent OnDragged = new UnityEvent();
        public static UnityEvent<int> OnHandleSelected = new UnityEvent<int>();
        #endregion

        #region Accessors
        public static DrawState DrawState { get { return m_DrawState; } set { m_DrawState = value; OnStateChange.Invoke(m_DrawState); } }
        public static int SelectedHandle => m_SelectedHandle - 1;
        #endregion

        public override void OnActivated()
        {
            m_Cursor = MouseCursor.ArrowPlus;
            m_Drawable = target as IDrawable;
            m_IsHandleHeld = false;
            m_SelectedHandle = -1;

            OnHeldStart.AddListener((startPos) => { m_StartPos = startPos; });
            OnHeldEnd.AddListener((endPos) =>
            {
                m_EndPos = endPos;

                if (m_StartPos != m_EndPos)
                    OnDragged.Invoke();

            });

            switch (m_DrawState)
            {
                case DrawState.Hide:
                    DrawState = DrawState.Edit;
                    break;
            }
        }
        public override void OnWillBeDeactivated()
        {
            m_Cursor = MouseCursor.Arrow;
            m_Drawable = null;
            DrawState = DrawState.Hide;
            m_IsHandleHeld = false;
            m_SelectedHandle = -1;

            OnHeldStart.RemoveAllListeners();
            OnHeld.RemoveAllListeners();
            OnHeldEnd.RemoveAllListeners();
            OnDragged.RemoveAllListeners();
            OnStateChange.RemoveAllListeners();
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
            if (m_DrawState != DrawState.Draw)
                return;

            Event currentEvent = Event.current;

            Ray ray = HandleUtil.GUIPointToWorldRay(currentEvent.mousePosition);
            float size = HandleUtil.GetHandleSize(ray.GetPoint(1)) * 0.05f;

            bool didHit = m_Drawable.Path.Raycast(ray, out Vector3 hit);

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
            if (m_Drawable == null)
                return;

            if (m_Drawable.Path.PathPointsCount == 0 || m_DrawState != DrawState.Draw)
                return;

            DrawHandles();
            ConnectTheDots();

            if (m_DrawState == DrawState.Draw)
            {
                Vector3[] globalPositions = m_Drawable.Path.Positions;
                Handles.color = m_Drawable.Path.IsPathValid ? m_Valid : m_Invalid;
                Handles.DrawDottedLine(globalPositions[^1], m_MousePosition, 1);
                Handles.color = Color.white;
            }
        }
        void Edit()
        {
            if (m_Drawable == null)
                return;

            if (m_DrawState != DrawState.Edit || m_Drawable.Path.PathPointsCount == 0)
                return;

            Handles.color = m_Drawable.Path.IsPathValid ? Color.white : m_Invalid;

            DrawHandles();
            ConnectTheDots();

        }
        void DrawHandles()
        {
            Vector3[] globalPositions = m_Drawable.Path.Positions;

            for (int i = 0; i < globalPositions.Length; i++)
            {
                float size = HandleUtil.GetHandleSize(m_Drawable.Path.GetPositionAt(i)) * 0.05f;

                Color handleColour = Handles.color;
                Handles.color = m_SelectedHandle == i + 1 ? Color.yellow : handleColour;

                Vector3 globalPoint = Handles.FreeMoveHandle(i + 1, globalPositions[i], size, m_Drawable.Path.ControlPoints[i].Up, BuildingHandles.SolidCircleHandleCap);

                Handles.color = handleColour;

                // How we detect if the control point has changed.
                if (globalPoint != m_Drawable.Path.GetPositionAt(i))
                {
                    Ray ray = HandleUtil.GUIPointToWorldRay(Event.current.mousePosition);

                    Plane plane = new Plane(Vector3.up, Vector3.zero);

                    if (m_Drawable.Path.Raycast(ray, out Vector3 impactPoint))
                    {
                        m_Drawable.Path.SetPositionAt(impactPoint, i);
                    }

                }
            }
        }
        void ConnectTheDots()
        {
            Handles.DrawAAPolyLine(m_Drawable.Path.Positions);
        }
        void OnEnable()
        {
            EditorApplication.update += CheckHandlesHeld;
        }
        void OnDisable()
        {
            EditorApplication.update -= CheckHandlesHeld;
        }
        void CheckHandlesHeld()
        {
            if (m_DrawState != DrawState.Edit)
                return;

            if (m_Drawable == null)
                return;

            Vector3[] globalPositions = m_Drawable.Path.Positions;

            if (GUIUtility.hotControl == 0 && m_IsHandleHeld)
            {
                Vector3 selectedHandlePos = globalPositions[m_SelectedHandle - 1];
                OnHeldEnd.Invoke(selectedHandlePos);
            }

            if (GUIUtility.hotControl > 0 && GUIUtility.hotControl < globalPositions.Length + 1)
            {
                if (!m_IsHandleHeld)
                {
                    OnHeldStart.Invoke(globalPositions[GUIUtility.hotControl - 1]);
                    m_IsHandleHeld = true;
                }

                m_SelectedHandle = GUIUtility.hotControl;
                OnHeld.Invoke(globalPositions[m_SelectedHandle - 1]);
            }
            else
            {
                m_IsHandleHeld = false;
            }

            //if (GUIUtility.hotControl != m_SelectedHandle && m_IsHandleHeld)
            //{
            //    OnHandleSelected.Invoke(m_SelectedHandle-1);
            //}
        }
    }

}
