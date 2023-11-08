using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Overlays;
using UnityEngine.Rendering;

[Overlay(typeof(SceneView), "Window", true)]
public class WindowOverlay : Overlay, ITransientOverlay
{
    [SerializeField] Window m_Window;

    public override VisualElement CreatePanelContent()
    {
        throw new System.NotImplementedException();
    }

    public static void DisplayWindow(VisualElement root, WindowSerializedProperties props, IBuildingComponent buildComponent)
    {
        //EnumFlagsField activeWinElements = new EnumFlagsField() { tooltip = "Active Elements" };
        //activeWinElements.BindProperty(props.ActiveElements);
        //activeWinElements.RegisterValueChangedCallback((ChangeEvent<System.Enum> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); displayed = false; } displayed = true; });
        //root.Add(activeWinElements);

        //#region Outer Frame
        //Foldout outerFrameFold = new Foldout() { text = "Outer Frame" };
        //outerFrameFold.SetEnabled(winData.IsOuterFrameActive);
        //root.Add(outerFrameFold);

        //Slider outerFScale = new Slider() { tooltip = "Scale", lowValue = 0, highValue = 0.999f, value = winData.OuterFrameScale };
        //outerFScale.BindProperty(m_SerialProps.WindowOuterFrameScale);
        //outerFScale.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
        //outerFrameFold.Add(outerFScale);

        //Slider outerFDepth = new Slider() { tooltip = "Depth", lowValue = 0, highValue = 0.999f, value = winData.OuterFrameDepth };
        //outerFDepth.BindProperty(m_SerialProps.WindowOuterFrameDepth);
        //outerFDepth.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
        //outerFrameFold.Add(outerFDepth);
        //#endregion

        //#region Inner Frame
        //Foldout innerFrameFold = new Foldout() { text = "Inner Frame" };
        //innerFrameFold.SetEnabled(winData.IsInnerFrameActive);
        //root.Add(innerFrameFold);

        //SliderInt innerFCols = new SliderInt() { tooltip = "Columns", lowValue = 1, highValue = 5, value = winData.InnerFrameColumns };
        //innerFCols.BindProperty(m_SerialProps.WindowInnerFrameColumns);
        //innerFCols.RegisterValueChangedCallback((ChangeEvent<int> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
        //innerFrameFold.Add(innerFCols);

        //SliderInt innerFRows = new SliderInt() { tooltip = "Rows", lowValue = 1, highValue = 5, value = winData.InnerFrameRows };
        //innerFRows.BindProperty(m_SerialProps.WindowInnerFrameRows);
        //innerFRows.RegisterValueChangedCallback((ChangeEvent<int> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
        //innerFrameFold.Add(innerFRows);

        //Slider innerFScale = new Slider() { tooltip = "Scale", lowValue = 0, highValue = 0.999f, value = winData.InnerFrameScale };
        //innerFScale.BindProperty(m_SerialProps.WindowInnerFrameScale);
        //innerFScale.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
        //innerFrameFold.Add(innerFScale);

        //Slider innerFDepth = new Slider() { tooltip = "Depth", lowValue = 0, highValue = 0.999f, value = winData.InnerFrameDepth };
        //innerFDepth.BindProperty(m_SerialProps.WindowInnerFrameDepth);
        //innerFDepth.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
        //innerFrameFold.Add(innerFDepth);
        //#endregion

        //#region Pane
        //Foldout paneFold = new Foldout() { text = "Pane" };
        //paneFold.SetEnabled(winData.IsPaneActive);
        //root.Add(paneFold);

        //Slider paneDepth = new Slider() { tooltip = "Depth", lowValue = 0, highValue = 0.999f, value = winData.PaneDepth };
        //paneDepth.BindProperty(m_SerialProps.WindowPaneDepth);
        //paneDepth.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
        //paneFold.Add(paneDepth);
        //#endregion

        //#region Shutters
        //Foldout shuttersFold = new Foldout() { text = "Shutters" };
        //shuttersFold.SetEnabled(winData.AreShuttersActive);
        //root.Add(shuttersFold);

        //Slider shuttersDepth = new Slider() { tooltip = "Depth", lowValue = 0, highValue = 0.999f, value = winData.ShuttersDepth };
        //shuttersDepth.BindProperty(m_SerialProps.WindowShuttersDepth);
        //shuttersDepth.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
        //shuttersFold.Add(shuttersDepth);

        //Slider shuttersAngle = new Slider() { tooltip = "Angle", lowValue = 0, highValue = 180f, value = winData.ShuttersAngle };
        //shuttersAngle.BindProperty(m_SerialProps.WindowShuttersAngle);
        //shuttersAngle.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { m_Section.Build(); } });
        //shuttersFold.Add(shuttersAngle);
        //#endregion
    }

    public bool visible
    {
        get
        {
            if (Selection.activeGameObject != null)
            {
                bool isWindow = Selection.activeGameObject.TryGetComponent(out Window window);

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
