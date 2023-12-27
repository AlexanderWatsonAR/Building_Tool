using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Rendering;
using System.Net.Http.Headers;

[CustomPropertyDrawer(typeof(WindowData))]
public class WindowDataDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        VisualElement container = new VisualElement();

        IBuildable buildable = data.serializedObject.targetObject as IBuildable;

        SerializedProperty activeElements = data.FindPropertyRelative("m_ActiveElements");

        EnumFlagsField activeElementsField = new EnumFlagsField(activeElements.GetEnumValue<WindowElement>());
        activeElementsField.BindProperty(activeElements);

        #region Outer Frame
        SerializedProperty outerScale = data.FindPropertyRelative("m_OuterFrameScale");
        SerializedProperty outerFrameDepth = data.FindPropertyRelative("m_OuterFrameDepth");

        Foldout outerFrameFoldout = new Foldout() { text = "Outer Frame"};

        PropertyField outerScaleField = new PropertyField(outerScale) { label = "Scale"};
        outerScaleField.BindProperty(outerScale);

        PropertyField outerFrameDepthField = new PropertyField(outerFrameDepth) { label = "Depth" };
        outerFrameDepthField.BindProperty(outerFrameDepth);

        outerFrameFoldout.Add(outerScaleField);
        outerFrameFoldout.Add(outerFrameDepthField);

        #endregion

        #region Inner Frame

        SerializedProperty cols = data.FindPropertyRelative("m_InnerFrameColumns");
        SerializedProperty rows = data.FindPropertyRelative("m_InnerFrameRows");
        SerializedProperty innerFrameScale = data.FindPropertyRelative("m_InnerFrameScale");
        SerializedProperty innerFrameDepth = data.FindPropertyRelative("m_InnerFrameDepth");

        Foldout innerFrameFoldout = new Foldout() { text = "Inner Frame" };

        PropertyField colsField = new PropertyField(cols) { label = "Columns" };
        colsField.BindProperty(cols);

        PropertyField rowsField = new PropertyField(rows) { label = "Rows" };
        rowsField.BindProperty(rows);

        PropertyField innerFrameScaleField = new PropertyField(innerFrameScale) { label = "Scale" };
        innerFrameScaleField.BindProperty(innerFrameScale);

        PropertyField innerFrameDepthField = new PropertyField(innerFrameDepth) { label = "Depth" };
        innerFrameDepthField.BindProperty(innerFrameDepth);

        innerFrameFoldout.Add(colsField);
        innerFrameFoldout.Add(rowsField);
        innerFrameFoldout.Add(innerFrameScaleField);
        innerFrameFoldout.Add(innerFrameDepthField);

        #endregion

        #region Pane
        SerializedProperty paneDepth = data.FindPropertyRelative("m_PaneDepth");

        Foldout paneFoldout = new Foldout() { text = "Pane" };

        PropertyField paneDepthField = new PropertyField(paneDepth) { label = "Depth" };
        paneDepthField.BindProperty(paneDepth);

        paneFoldout.Add(paneDepthField);

        #endregion

        #region Shutters
        SerializedProperty shuttersDepth = data.FindPropertyRelative("m_ShuttersDepth");
        SerializedProperty shuttersAngle = data.FindPropertyRelative("m_ShuttersAngle");

        Foldout shuttersFoldout = new Foldout() { text = "Shutters" };

        PropertyField shuttersDepthField = new PropertyField(shuttersDepth) { label = "Depth" };
        shuttersDepthField.BindProperty(shuttersDepth);

        PropertyField shuttersAngleField = new PropertyField(shuttersAngle) { label = "Angle" };
        shuttersAngleField.BindProperty(shuttersAngle);

        shuttersFoldout.Add(shuttersDepthField);
        shuttersFoldout.Add(shuttersAngleField);

        #endregion

        #region Register Value Change Callback
        activeElementsField.RegisterValueChangedCallback(evt => 
        {
            if (evt == null)
                return;

            if (evt.newValue == null)
                return;

            WindowElement currentlyActive = (WindowElement) evt.newValue;

            bool isOuterFrameActive = currentlyActive.IsElementActive(WindowElement.OuterFrame);
            bool isInnerFrameActive = currentlyActive.IsElementActive(WindowElement.InnerFrame);
            bool isPaneActive = currentlyActive.IsElementActive(WindowElement.Pane);
            bool areShuttersActive = currentlyActive.IsElementActive(WindowElement.Shutters);

            outerFrameFoldout.SetEnabled(isOuterFrameActive);
            innerFrameFoldout.SetEnabled(isInnerFrameActive);
            paneFoldout.SetEnabled(isPaneActive);
            shuttersFoldout.SetEnabled(areShuttersActive);

            if (evt.newValue == evt.previousValue)
                return;

            WindowData[] windows = GetWindowDataFromBuildable(buildable);

            foreach (WindowData win in windows)
            {
                bool wasOuterFrameActive = win.IsOuterFrameActive;
                bool wasInnerFrameActive = win.IsInnerFrameActive;
                bool wasPaneActive = win.IsPaneActive;
                bool wereShuttersActive = win.AreShuttersActive;

                if (isOuterFrameActive == true && wasOuterFrameActive == false)
                    win.DoesOuterFrameNeedRebuild = true;
                if (isInnerFrameActive == true && wasInnerFrameActive == false)
                    win.DoesInnerFrameNeedRebuild = true;
                if (isPaneActive == true && wasPaneActive == false)
                    win.DoesPaneNeedRebuild = true;
                if (areShuttersActive == true && wereShuttersActive == false)
                    win.DoShuttersNeedRebuild = true;

                if(buildable is not Window)
                    win.ActiveElements = currentlyActive;
            }

            if(isOuterFrameActive || isInnerFrameActive || isPaneActive || areShuttersActive)
            {
                buildable.Demolish();
                buildable.Build();
            }
        });
        outerScaleField.RegisterValueChangeCallback(evt => 
        {
            if (evt == null)
                return;

            bool rebuild = false;

            WindowData[] windows = GetWindowDataFromBuildable(buildable);

            foreach (WindowData win in windows)
            {
                if (win.OuterFrameScale == evt.changedProperty.floatValue && buildable is not Window)
                    continue;

                win.OuterFrameScale = evt.changedProperty.floatValue;
                win.DoesOuterFrameNeedRebuild = true;
                rebuild = true;
            }

            if (rebuild)
            {
                buildable.Demolish();
                buildable.Build();
            }
        });
        outerFrameDepthField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            bool rebuild = false;

            WindowData[] windows = GetWindowDataFromBuildable(buildable);

            foreach (WindowData win in windows)
            {
                if (win.OuterFrameDepth == evt.changedProperty.floatValue && buildable is not Window)
                    continue;

                win.OuterFrameDepth = evt.changedProperty.floatValue;
                win.DoesOuterFrameNeedRebuild = true;
                rebuild = true;
            }

            if (rebuild)
            {
                buildable.Demolish();
                buildable.Build();
            }
        });
        colsField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            bool rebuild = false;

            WindowData[] windows = GetWindowDataFromBuildable(buildable); 

            foreach (WindowData win in windows)
            {
                if (win.InnerFrameColumns == evt.changedProperty.intValue && buildable is not Window)
                    continue;

                win.InnerFrameColumns = evt.changedProperty.intValue;
                win.InnerFrameHolePoints = Window.CalculateInnerFrame(win);
                win.DoesInnerFrameNeedRebuild = true;
                rebuild = true;
            }

            if(rebuild)
            {
                buildable.Demolish();
                buildable.Build();
            }
                
        });
        rowsField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            bool rebuild = false;

            WindowData[] windows = GetWindowDataFromBuildable(buildable);

            foreach (WindowData win in windows)
            {
                if (win.InnerFrameRows == evt.changedProperty.intValue && buildable is not Window)
                    continue;

                win.InnerFrameRows = evt.changedProperty.intValue;
                win.InnerFrameHolePoints = Window.CalculateInnerFrame(win);
                win.DoesInnerFrameNeedRebuild = true;
                rebuild = true;
            }

            if (rebuild)
            {
                buildable.Demolish();
                buildable.Build();
            }
        });
        innerFrameScaleField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            bool rebuild = false;

            WindowData[] windows = GetWindowDataFromBuildable(buildable);

            foreach (WindowData win in windows)
            {
                if (win.InnerFrameScale == evt.changedProperty.floatValue && buildable is not Window)
                    continue;

                win.InnerFrameScale = evt.changedProperty.floatValue;
                win.DoesInnerFrameNeedRebuild = true;
                rebuild = true;
            }

            if (rebuild)
            {
                buildable.Demolish();
                buildable.Build();
            }
        });
        innerFrameDepthField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            bool rebuild = false;

            WindowData[] windows = GetWindowDataFromBuildable(buildable);

            foreach (WindowData win in windows)
            {
                if (win.InnerFrameDepth == evt.changedProperty.floatValue && buildable is not Window)
                    continue;

                win.InnerFrameDepth = evt.changedProperty.floatValue;
                win.DoesInnerFrameNeedRebuild = true;
                rebuild = true;
            }

            if (rebuild)
            {
                buildable.Demolish();
                buildable.Build();
            }
        });
        paneDepthField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            bool rebuild = false;

            WindowData[] windows = GetWindowDataFromBuildable(buildable);

            foreach (WindowData win in windows)
            {
                if (win.PaneDepth == evt.changedProperty.floatValue && buildable is not Window)
                    continue;

                win.PaneDepth = evt.changedProperty.floatValue;
                win.DoesPaneNeedRebuild = true;
                rebuild = true;
            }

            if (rebuild)
            {
                buildable.Demolish();
                buildable.Build();
            }
        });
        shuttersDepthField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            bool rebuild = false;

            WindowData[] windows = GetWindowDataFromBuildable(buildable);

            foreach (WindowData win in windows)
            {
                if (win.ShuttersDepth == evt.changedProperty.floatValue && buildable is not Window)
                    continue;

                win.ShuttersDepth = evt.changedProperty.floatValue;
                win.DoShuttersNeedRebuild = true;
                rebuild = true;
            }

            if (rebuild)
            {
                buildable.Demolish();
                buildable.Build();
            }
        });
        shuttersAngleField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            bool rebuild = false;

            WindowData[] windows = GetWindowDataFromBuildable(buildable);

            foreach (WindowData win in windows)
            {
                if (win.ShuttersAngle == evt.changedProperty.floatValue && buildable is not Window)
                    continue;

                win.ShuttersAngle = evt.changedProperty.floatValue;
                win.DoShuttersNeedRebuild = true;
                rebuild = true;
            }

            if (rebuild)
            {
                buildable.Demolish();
                buildable.Build();
            }
        });
        #endregion

        #region Add Fields to Container
        container.Add(activeElementsField);
        container.Add(outerFrameFoldout);
        container.Add(innerFrameFoldout);
        container.Add(paneFoldout);
        container.Add(shuttersFoldout);
        #endregion

        return container;
    }

    private WindowData[] GetWindowDataFromBuildable(IBuildable buildable)
    {
        WindowData[] dataset = new WindowData[0];

        switch (buildable)
        {
            case Wall:
                {
                    Wall wall = buildable as Wall;
                    dataset = new WindowData[1];
                    dataset[0] = wall.Data.Sections[0, 0].WindowData; // TODO: Replace 0,0 with the actively selected section
                }
                break;
            case WallSection:
                {
                    WallSection wallSection = buildable as WallSection;
                    dataset = wallSection.Data.Windows;
                }
                break;
            case Window:
                {
                    Window window = buildable as Window;
                    dataset = new WindowData[1];
                    dataset[0] = window.Data;
                }
                break;
        }

        return dataset;
    }
}
