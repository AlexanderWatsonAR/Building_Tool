using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

[Overlay(typeof(SceneView), "Wall Section", true)]
public class WallSectionOverlay : Overlay, ITransientOverlay
{
    [SerializeField] WallSection m_Section;

    public override VisualElement CreatePanelContent()
    {
        if (m_Section == null)
            return new VisualElement();

        VisualElement container = new VisualElement() { name = "Wall Section Container" };

        SerializedObject serializedObject = new SerializedObject(m_Section);

        SerializedProperty data = serializedObject.FindProperty("m_Data");

        PropertyField dataField = new PropertyField(data);
        dataField.BindProperty(data);

        container.Add(dataField);

        return container;

    }

    public bool visible
    {
        get
        {
            if (Selection.activeGameObject != null)
            {
                bool isSection = Selection.activeGameObject.TryGetComponent(out WallSection section);

                if (isSection)
                {
                    m_Section = section;

                }
                return isSection;
            }
            return false;
        }
    }
}
