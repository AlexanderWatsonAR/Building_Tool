using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using OnlyInvalid.ProcGenBuilding.Building;
using UnityEditor.EditorTools;

[CustomEditor(typeof(Drawable))]
public class DrawableEditor : Editor
{
    static VisualElement m_Root;

    public override VisualElement CreateInspectorGUI()
    {
        m_Root = new VisualElement();
        DisplayDrawState(DrawState.Hide);
        return m_Root;
    }

    public static void DisplayDrawState(DrawState state)
    {
        m_Root.Clear();

        switch(state)
        {
            case DrawState.Draw:
                {
                    m_Root.Add(new HelpBox("Click to draw points", HelpBoxMessageType.Info));
                }
                break;
            case DrawState.Edit:
                {
                    m_Root.Add(new HelpBox("Edit Mode", HelpBoxMessageType.Info));

                    Button quit_btn = new Button(() =>
                    {
                        ToolManager.RestorePreviousPersistentTool();
                        DisplayDrawState(DrawState.Hide);
                    });

                    quit_btn.text = "Quit Edit";

                    m_Root.Add(quit_btn);
                }
                break;
            case DrawState.Hide:
                {
                    m_Root.Add(new HelpBox("Hide Mode", HelpBoxMessageType.Info));

                    Button edit_btn = new Button(() =>
                    {
                        ToolManager.SetActiveTool<DrawTool>();
                        DisplayDrawState(DrawState.Edit);
                    });

                    edit_btn.text = "Edit Path";

                    m_Root.Add(edit_btn);
                }
                break;
        }
    }

}
