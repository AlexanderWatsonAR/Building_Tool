using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Overlays;
using UnityEditor.Rendering;
using Unity.VisualScripting;

namespace OnlyInvalid.ProcGenBuilding.Window
{

    [Overlay(typeof(SceneView), nameof(Window), true)]
    public class WindowOverlay : Overlay, ITransientOverlay
    {
        [SerializeField] Window m_Window;

        public override VisualElement CreatePanelContent()
        {
            VisualElement root = new VisualElement() { name = nameof(Window) + "_Root" };
            SerializedObject serializedObject = new SerializedObject(m_Window);
            SerializedProperty data = serializedObject.FindProperty("m_Data");
            PropertyField dataField = new PropertyField(data);
            dataField.BindProperty(data);
            root.Add(dataField);
            return root;
        }

        public bool visible
        {
            get
            {
                if (Selection.activeGameObject != null)
                {
                    GameObject gameObject = Selection.activeGameObject;
                    bool isWindow = gameObject.TryGetComponent(out Window window);

                    if (isWindow)
                    {
                        m_Window = window;

                    }
                    return isWindow;
                }
                return false;
            }
        }

    }
}
