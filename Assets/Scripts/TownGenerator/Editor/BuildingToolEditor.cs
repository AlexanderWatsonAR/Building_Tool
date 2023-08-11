using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEditor.ProBuilder;
using UnityEditor;
using UnityEditor.EditorTools;

[EditorTool("Edit Poly Building", typeof(Building))]
public class BuildingToolEditor : EditorTool
{
    MouseCursor m_MouseCursor;
    static GUIContent m_IconContent;

    /// <summary>
    /// Gets the icon and tooltip for the BuildingTool.
    /// </summary>
    public override GUIContent toolbarIcon
    {
        get
        {
            if (m_IconContent == null)
                m_IconContent = new GUIContent()
                {
                    image = EditorGUIUtility.IconContent("Packages/com.unity.probuilder/Content/Icons/Tools/PolyShape/CreatePolyShape.png").image,
                    text = "Create Poly Building",
                    tooltip = "Create Poly Building"
                };
            return m_IconContent;
        }
    }

    public override void OnToolGUI(EditorWindow window)
    {
        if (!(window is SceneView))
            return;

        Building building = (Building)target;
        PolyMode mode = building.PolyPath.PolyMode;

        switch(mode)
        {
            case PolyMode.Draw:
                m_MouseCursor = MouseCursor.ArrowPlus;
                break;
            default:
                m_MouseCursor = MouseCursor.Arrow;
            break;
        }

        if (Event.current.type == EventType.Repaint)
        {
            Rect sceneViewRect = window.position;
            sceneViewRect.x = 0;
            sceneViewRect.y = 0;
            EditorGUIUtility.AddCursorRect(sceneViewRect, m_MouseCursor);
        }
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

}
