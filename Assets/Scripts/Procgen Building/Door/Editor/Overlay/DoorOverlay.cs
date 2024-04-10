using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Overlays;
using Unity.VisualScripting;

[Overlay(typeof(SceneView), "Door", true)]
public class DoorOverlay : Overlay, ITransientOverlay
{
    [SerializeField] Door m_Door;

    public override VisualElement CreatePanelContent()
    {
        if (m_Door == null)
            return null;

        var root = new VisualElement() { name = "Door Root" };

        SerializedObject serializedObject = new SerializedObject(m_Door);

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
                bool isDoor = Selection.activeGameObject.TryGetComponent(out Door door);

                if (isDoor)
                {
                    m_Door = door;

                }
                return isDoor;
            }
            return false;
        }
    }

}
