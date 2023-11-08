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
        // Toggling display off & on triggers the panel to redraw.
        wallElementField.RegisterValueChangedCallback((ChangeEvent<System.Enum> evt) => { if (evt.newValue != evt.previousValue) { displayed = false; } displayed = true; });
        root.Add(wallElementField);

        WallElement wallElement = m_Section.WallElement;

        switch (wallElement)
        {
            // PropertyField

            case WallElement.Window:
                {
                    WindowData winData = m_Section.WindowData;

                    Foldout shapeFold = new Foldout() { text = "Shape" };
                    root.Add(shapeFold);

                    SliderInt sides = new SliderInt() {tooltip = "Sides", lowValue = 3, highValue = 16, value = m_Section.WindowSides };
                    sides.BindProperty(m_SerialProps.WindowSides);
                    sides.RegisterValueChangedCallback((ChangeEvent<int> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
                    shapeFold.Add(sides);

                    Slider height = new Slider() {tooltip = "Height", lowValue = 0, highValue = 0.999f, value = m_Section.WindowHeight };
                    height.BindProperty(m_SerialProps.WindowHeight);
                    height.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
                    shapeFold.Add(height);

                    Slider width = new Slider() { tooltip = "Width", lowValue = 0, highValue = 0.999f, value = m_Section.WindowWidth };
                    width.BindProperty(m_SerialProps.WindowWidth);
                    width.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
                    shapeFold.Add(width);

                    Slider angle = new Slider() { tooltip = "Angle", lowValue = -180, highValue = 180, value = m_Section.WindowAngle };
                    angle.BindProperty(m_SerialProps.WindowAngle);
                    angle.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
                    
                    shapeFold.Add(angle);

                    EnumFlagsField activeWinElements = new EnumFlagsField() { tooltip = "Active Elements" };
                    activeWinElements.BindProperty(m_SerialProps.WindowActiveElements);
                    activeWinElements.RegisterValueChangedCallback((ChangeEvent<System.Enum> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); displayed = false; } displayed = true; });
                    root.Add(activeWinElements);

                    #region Outer Frame
                    Foldout outerFrameFold = new Foldout() { text = "Outer Frame" };
                    outerFrameFold.SetEnabled(winData.IsOuterFrameActive);
                    root.Add(outerFrameFold);

                    Slider outerFScale = new Slider() { tooltip = "Scale", lowValue = 0, highValue = 0.999f, value = winData.OuterFrameScale };
                    outerFScale.BindProperty(m_SerialProps.WindowOuterFrameScale);
                    outerFScale.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
                    outerFrameFold.Add(outerFScale);

                    Slider outerFDepth = new Slider() { tooltip = "Depth", lowValue = 0, highValue = 0.999f, value = winData.OuterFrameDepth };
                    outerFDepth.BindProperty(m_SerialProps.WindowOuterFrameDepth);
                    outerFDepth.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
                    outerFrameFold.Add(outerFDepth);
                    #endregion

                    #region Inner Frame
                    Foldout innerFrameFold = new Foldout() { text = "Inner Frame" };
                    innerFrameFold.SetEnabled(winData.IsInnerFrameActive);
                    root.Add(innerFrameFold);

                    SliderInt innerFCols = new SliderInt() { tooltip = "Columns", lowValue = 1, highValue = 5, value = winData.InnerFrameColumns };
                    innerFCols.BindProperty(m_SerialProps.WindowInnerFrameColumns);
                    innerFCols.RegisterValueChangedCallback((ChangeEvent<int> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
                    innerFrameFold.Add(innerFCols);

                    SliderInt innerFRows = new SliderInt() { tooltip = "Rows", lowValue = 1, highValue = 5, value = winData.InnerFrameRows };
                    innerFRows.BindProperty(m_SerialProps.WindowInnerFrameRows);
                    innerFRows.RegisterValueChangedCallback((ChangeEvent<int> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
                    innerFrameFold.Add(innerFRows);

                    Slider innerFScale = new Slider() { tooltip = "Scale", lowValue = 0, highValue = 0.999f, value = winData.InnerFrameScale };
                    innerFScale.BindProperty(m_SerialProps.WindowInnerFrameScale);
                    innerFScale.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
                    innerFrameFold.Add(innerFScale);

                    Slider innerFDepth = new Slider() { tooltip = "Depth", lowValue = 0, highValue = 0.999f, value = winData.InnerFrameDepth };
                    innerFDepth.BindProperty(m_SerialProps.WindowInnerFrameDepth);
                    innerFDepth.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
                    innerFrameFold.Add(innerFDepth);
                    #endregion

                    #region Pane
                    Foldout paneFold = new Foldout() { text = "Pane" };
                    paneFold.SetEnabled(winData.IsPaneActive);
                    root.Add(paneFold);

                    Slider paneDepth = new Slider() { tooltip = "Depth", lowValue = 0, highValue = 0.999f, value = winData.PaneDepth };
                    paneDepth.BindProperty(m_SerialProps.WindowPaneDepth);
                    paneDepth.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
                    paneFold.Add(paneDepth);
                    #endregion

                    #region Shutters
                    Foldout shuttersFold = new Foldout() { text = "Shutters" };
                    shuttersFold.SetEnabled(winData.AreShuttersActive);
                    root.Add(shuttersFold);

                    Slider shuttersDepth = new Slider() { tooltip = "Depth", lowValue = 0, highValue = 0.999f, value = winData.ShuttersDepth };
                    shuttersDepth.BindProperty(m_SerialProps.WindowShuttersDepth);
                    shuttersDepth.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
                    shuttersFold.Add(shuttersDepth);

                    Slider shuttersAngle = new Slider() { tooltip = "Angle", lowValue = 0, highValue = 180f, value = winData.ShuttersAngle };
                    shuttersAngle.BindProperty(m_SerialProps.WindowShuttersAngle);
                    shuttersAngle.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
                    shuttersFold.Add(shuttersAngle);
                    #endregion
          

                }
                break;

        }

        return root;

    }

    public override void OnCreated()
    {
        base.OnCreated();
    }

    public bool visible
    {
        get
        {
            if (Selection.activeGameObject != null)
            {
                bool isSection = Selection.activeGameObject.TryGetComponent(out WallSection section);

                if(isSection)
                {
                    m_Section = section;
                   
                }
                return isSection;
            }
            return false;
        }
    }
}
