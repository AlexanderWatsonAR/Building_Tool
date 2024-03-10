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
                    text = "Edit Poly Building",
                    tooltip = "Edit Poly Building"
                };
            return m_IconContent;
        }
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
            EditorGUIUtility.AddCursorRect(sceneViewRect, m_MouseCursor);
        }
    }
    public override void OnActivated()
    {
        Building building = (Building)target;
        building.Data.Path.PolyMode = PolyMode.Edit;
        m_MouseCursor = MouseCursor.ArrowPlus;
    }
    public override void OnWillBeDeactivated()
    {
        Building building = (Building)target;
        building.Data.Path.PolyMode = PolyMode.Hide;
        m_MouseCursor = MouseCursor.Arrow;
    }
    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        
    }

}
