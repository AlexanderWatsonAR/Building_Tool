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
    [SerializeField] private WallSection m_Section;
    [SerializeField] private WallSectionSerializedProperties m_SerialProps;

    public override VisualElement CreatePanelContent()
    {
        if (m_Section == null)
            return new VisualElement();

        m_SerialProps = new WallSectionSerializedProperties(m_Section);

        var root = new VisualElement() { name = "Wall Section Root" };

        EnumField wallElementField = new EnumField() { tooltip = "Section" };
        wallElementField.BindProperty(m_SerialProps.WallElement);
        wallElementField.RegisterValueChangedCallback((ChangeEvent<System.Enum> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); displayed = false; } displayed = true; }); // Toggling display off & on triggers the panel to redraw. // Bug: this doesn't work if the panel has been collapsed.
        root.Add(wallElementField);

        WallElement wallElement = m_Section.WallElement;

        if (collapsed)
            return root;

        switch (wallElement)
        {
            case WallElement.Doorway:
                {
                    Foldout shapeFold = new Foldout() { text = "Shape" };
                    root.Add(shapeFold);

                    Slider sideOffset = new Slider() { label = "Offset", lowValue = -0.999f, highValue = 0.999f, value = m_Section.Data.SideOffset };
                    sideOffset.BindProperty(m_SerialProps.DoorOffset);
                    sideOffset.RegisterValueChangedCallback(evt => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
                    shapeFold.Add(sideOffset);

                    Slider height = new Slider() { label = "Height", lowValue = 0, highValue = 0.999f, value = m_Section.Data.PedimentHeight };
                    height.BindProperty(m_SerialProps.DoorHeight);
                    height.RegisterValueChangedCallback(evt => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
                    shapeFold.Add(height);

                    Slider width = new Slider() { label = "Width", lowValue = 0, highValue = 0.999f, value = m_Section.Data.SideWidth };
                    width.BindProperty(m_SerialProps.DoorWidth);
                    width.RegisterValueChangedCallback(evt => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
                    shapeFold.Add(width);

                    Toggle isDoorActive = new Toggle() { text = "Is Active", value = m_SerialProps.DoorActive.boolValue };
                    isDoorActive.BindProperty(m_SerialProps.DoorActive);
                    isDoorActive.RegisterValueChangedCallback(evt => { if (evt.newValue != evt.previousValue) { m_Section.Build(); displayed = false; } displayed = true; });
                    root.Add(isDoorActive);

                    Foldout doorFoldout = new Foldout() { text = "Door" };
                    doorFoldout.SetEnabled(m_SerialProps.DoorActive.boolValue);
                    root.Add(doorFoldout);

                    DoorOverlay.Display(doorFoldout, m_SerialProps.DoorSerializedProperties, m_Section);
                }
                break;

            case WallElement.Window:
                {
                    WindowData winData = m_Section.Data.WindowData;

                    Foldout shapeFold = new Foldout() { text = "Shape" };
                    root.Add(shapeFold);

                    SliderInt sides = new SliderInt() { tooltip = "Sides", lowValue = 3, highValue = 16, value = m_Section.Data.WindowSides };
                    sides.BindProperty(m_SerialProps.WindowSides);
                    sides.RegisterValueChangedCallback((ChangeEvent<int> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
                    shapeFold.Add(sides);

                    Slider height = new Slider() { tooltip = "Height", lowValue = 0, highValue = 0.999f, value = m_Section.Data.WindowHeight };
                    height.BindProperty(m_SerialProps.WindowHeight);
                    height.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
                    shapeFold.Add(height);

                    Slider width = new Slider() { tooltip = "Width", lowValue = 0, highValue = 0.999f, value = m_Section.Data.WindowWidth };
                    width.BindProperty(m_SerialProps.WindowWidth);
                    width.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
                    shapeFold.Add(width);

                    Slider angle = new Slider() { tooltip = "Angle", lowValue = -180, highValue = 180, value = m_Section.Data.WindowAngle };
                    angle.BindProperty(m_SerialProps.WindowAngle);
                    angle.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });

                    shapeFold.Add(angle);

                    WindowOverlay.Display(root, m_SerialProps.WindowSerializedProperties, m_Section, this);
                }
                break;

        }

        
        return root;

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
