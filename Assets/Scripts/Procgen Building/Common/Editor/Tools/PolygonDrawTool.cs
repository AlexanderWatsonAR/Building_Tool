using OnlyInvalid.ProcGenBuilding.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [EditorTool("Edit Polygon Path", typeof(IPolygon))]
    [Icon("Packages/com.unity.probuilder/Content/Icons/Tools/PolyShape/CreatePolyShape.png")]
    public class PolygonDrawTool : DrawTool
    {
        public static UnityEvent OnPolygonComplete = new UnityEvent();

        public override void OnActivated()
        {
            base.OnActivated();

            m_Drawable.Path.OnPointAdded.AddListener(() =>
            {
                if (m_Drawable.Path.PathPointsCount < 4)
                    return;

                // if the point is the same as the first point the polygon is complete.
                if (Vector3.Distance(m_Drawable.Path.GetFirstPosition(), m_Drawable.Path.GetLastPosition()) <= m_Drawable.Path.MinimumPointDistance)
                {
                    m_Drawable.Path.RemoveLastPoint();
                    DrawState = DrawState.Hide;
                    OnPolygonComplete.Invoke();
                    ToolManager.RestorePreviousPersistentTool();
                }
            });
        }
        public override void OnToolGUI(EditorWindow window)
        {
            base.OnToolGUI(window);

            if (m_Drawable == null)
                return;

            if (m_DrawState == DrawState.Draw)
                return;

            Vector3[] globalPositions = m_Drawable.Path.Positions;

            if (globalPositions.Length >= 3)
                Handles.DrawAAPolyLine(globalPositions[0], globalPositions[^1]);
            
        }
    }
}
