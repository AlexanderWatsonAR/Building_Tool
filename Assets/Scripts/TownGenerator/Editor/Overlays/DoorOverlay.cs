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

        DoorSerializedProperties props = new DoorSerializedProperties(m_Door);

        Display(root, props, m_Door);

        return root;
    }

    public static void Display(VisualElement root, DoorSerializedProperties props, IBuildable buildable)
    {
        Slider scale = new Slider("Scale") { tooltip = "Scale", lowValue = 0, highValue = 0.999f, value = props.Scale.floatValue };
        scale.BindProperty(props.Scale);
        scale.RegisterValueChangedCallback(evt => { if (evt.newValue != evt.previousValue) { buildable.Build(); } });
        root.Add(scale);

        Foldout hingeFoldout = new Foldout() { text = "Hinge" };
        root.Add(hingeFoldout);

        EnumField position = new EnumField("Position") { tooltip = "Position" };
        position.BindProperty(props.HingePoint);
        position.RegisterValueChangedCallback(evt => { if (evt.newValue != evt.previousValue) { buildable.Build(); } });
        hingeFoldout.Add(position);

        Vector3Field offset = new Vector3Field("Offset") { value = props.HingeOffset.vector3Value, tooltip = "Offset" };
        offset.BindProperty(props.HingeOffset);
        offset.RegisterValueChangedCallback(evt => { if (evt.newValue != evt.previousValue) { buildable.Build(); } });
        hingeFoldout.Add(offset);

        Vector3Field rotation = new Vector3Field("Rotation") { value = props.HingeEulerAngle.vector3Value, tooltip = "Rotation as an euler angle" };
        rotation.BindProperty(props.HingeEulerAngle);
        rotation.RegisterValueChangedCallback(evt => { if (evt.newValue != evt.previousValue) { buildable.Build(); } });
        hingeFoldout.Add(rotation);

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
