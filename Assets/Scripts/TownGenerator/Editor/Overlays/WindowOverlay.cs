using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Overlays;
using UnityEditor.Rendering;
using Unity.VisualScripting;

[Overlay(typeof(SceneView), "Window", true)]
public class WindowOverlay : Overlay, ITransientOverlay
{
    [SerializeField] Window m_Window;

    public override VisualElement CreatePanelContent()
    {
        var root = new VisualElement() { name = "Window Root" };

        SerializedObject serializedObject = new SerializedObject(m_Window);

        SerializedProperty data = serializedObject.FindProperty("m_Data");

        PropertyField dataField = new PropertyField(data);
        dataField.BindProperty(data);

        root.Add(dataField);

        //WindowSerializedProperties props = new WindowSerializedProperties(m_Window);

        //Display(root, props, m_Window, this);

        return root;
    }

    public static void Display(VisualElement root, WindowDataSerializedProperties props, IBuildable buildable, Overlay overlay)
    {
        //WindowData winData = (WindowData)props.Data.GetUnderlyingValue();

        //EnumFlagsField activeWinElements = new EnumFlagsField() { tooltip = "Active Elements" };
        //activeWinElements.BindProperty(props.ActiveElements);
        //activeWinElements.RegisterValueChangedCallback((ChangeEvent<System.Enum> evt) => { if (evt.newValue != evt.previousValue) { buildable.Build(); overlay.displayed = false; } overlay.displayed = true; });
        //root.Add(activeWinElements);

        //#region Outer Frame
        //Foldout outerFrameFold = new Foldout() { text = "Outer Frame" };
        //outerFrameFold.SetEnabled(winData.IsOuterFrameActive);
        //root.Add(outerFrameFold);

        //Slider outerFScale = new Slider() { tooltip = "Scale", lowValue = 0, highValue = 0.999f, value = winData.OuterFrameScale };
        //outerFScale.BindProperty(props.OuterFrameScale);
        //outerFScale.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { buildable.Build(); } });
        //outerFrameFold.Add(outerFScale);

        //Slider outerFDepth = new Slider() { tooltip = "Depth", lowValue = 0, highValue = 0.999f, value = winData.OuterFrameDepth };
        //outerFDepth.BindProperty(props.OuterFrameDepth);
        //outerFDepth.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { buildable.Build(); } });
        //outerFrameFold.Add(outerFDepth);
        //#endregion

        //#region Inner Frame
        //Foldout innerFrameFold = new Foldout() { text = "Inner Frame" };
        //innerFrameFold.SetEnabled(winData.IsInnerFrameActive);
        //root.Add(innerFrameFold);

        //SliderInt innerFCols = new SliderInt() { tooltip = "Columns", lowValue = 1, highValue = 5, value = winData.InnerFrameColumns };
        //innerFCols.BindProperty(props.InnerFrameColumns);
        //innerFCols.RegisterValueChangedCallback((ChangeEvent<int> evt) => { if (evt.newValue != evt.previousValue) { buildable.Build(); } });
        //innerFrameFold.Add(innerFCols);

        //SliderInt innerFRows = new SliderInt() { tooltip = "Rows", lowValue = 1, highValue = 5, value = winData.InnerFrameRows };
        //innerFRows.BindProperty(props.InnerFrameRows);
        //innerFRows.RegisterValueChangedCallback((ChangeEvent<int> evt) => { if (evt.newValue != evt.previousValue) { buildable.Build(); } });
        //innerFrameFold.Add(innerFRows);

        //Slider innerFScale = new Slider() { tooltip = "Scale", lowValue = 0, highValue = 0.999f, value = winData.InnerFrameScale };
        //innerFScale.BindProperty(props.InnerFrameScale);
        //innerFScale.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { buildable.Build(); } });
        //innerFrameFold.Add(innerFScale);

        //Slider innerFDepth = new Slider() { tooltip = "Depth", lowValue = 0, highValue = 0.999f, value = winData.InnerFrameDepth };
        //innerFDepth.BindProperty(props.InnerFrameDepth);
        //innerFDepth.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { buildable.Build(); } });
        //innerFrameFold.Add(innerFDepth);
        //#endregion

        //#region Pane
        //Foldout paneFold = new Foldout() { text = "Pane" };
        //paneFold.SetEnabled(winData.IsPaneActive);
        //root.Add(paneFold);

        //Slider paneDepth = new Slider() { tooltip = "Depth", lowValue = 0, highValue = 0.999f, value = winData.PaneDepth };
        //paneDepth.BindProperty(props.PaneDepth);
        //paneDepth.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { buildable.Build(); } });
        //paneFold.Add(paneDepth);
        //#endregion

        //#region Shutters
        //Foldout shuttersFold = new Foldout() { text = "Shutters" };
        //shuttersFold.SetEnabled(winData.AreShuttersActive);
        //root.Add(shuttersFold);

        //Slider shuttersDepth = new Slider() { tooltip = "Depth", lowValue = 0, highValue = 0.999f, value = winData.ShuttersDepth };
        //shuttersDepth.BindProperty(props.ShuttersDepth);
        //shuttersDepth.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { buildable.Build(); } });
        //shuttersFold.Add(shuttersDepth);

        //Slider shuttersAngle = new Slider() { tooltip = "Angle", lowValue = 0, highValue = 180f, value = winData.ShuttersAngle };
        //shuttersAngle.BindProperty(props.ShuttersAngle);
        //shuttersAngle.RegisterValueChangedCallback((ChangeEvent<float> evt) => { if (evt.newValue != evt.previousValue) { buildable.Build(); } });
        //shuttersFold.Add(shuttersAngle);
       // #endregion
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
