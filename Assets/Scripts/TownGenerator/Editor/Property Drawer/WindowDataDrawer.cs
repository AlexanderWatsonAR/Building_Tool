using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Rendering;
using System.Linq;

[CustomPropertyDrawer(typeof(WindowData))]
public class WindowDataDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        VisualElement container = new VisualElement();

        WindowDataSerializedProperties props = new WindowDataSerializedProperties(data);

        IBuildable buildable = data.serializedObject.targetObject as IBuildable;

        #region Declare Fields
        PropertyField activeElementsField = new PropertyField(props.ActiveElements);
        activeElementsField.BindProperty(props.ActiveElements);

        #region Outer Frame
        Foldout outerFrameFoldout = new Foldout() { text = "Outer Frame"};

        PropertyField outerScaleField = new PropertyField(props.OuterFrameScale) { label = "Scale"};
        outerScaleField.BindProperty(props.OuterFrameScale);

        PropertyField outerFrameDepthField = new PropertyField(props.OuterFrameDepth) { label = "Depth" };
        outerFrameDepthField.BindProperty(props.OuterFrameDepth);

        outerFrameFoldout.Add(outerScaleField);
        outerFrameFoldout.Add(outerFrameDepthField);

        #endregion

        #region Inner Frame
        Foldout innerFrameFoldout = new Foldout() { text = "Inner Frame" };

        PropertyField colsField = new PropertyField(props.InnerFrameColumns) { label = "Columns" };
        colsField.BindProperty(props.InnerFrameColumns);

        PropertyField rowsField = new PropertyField(props.InnerFrameRows) { label = "Rows" };
        rowsField.BindProperty(props.InnerFrameRows);

        PropertyField innerFrameScaleField = new PropertyField(props.InnerFrameScale) { label = "Scale" };
        innerFrameScaleField.BindProperty(props.InnerFrameScale);

        PropertyField innerFrameDepthField = new PropertyField(props.InnerFrameDepth) { label = "Depth" };
        innerFrameDepthField.BindProperty(props.InnerFrameDepth);

        innerFrameFoldout.Add(colsField);
        innerFrameFoldout.Add(rowsField);
        innerFrameFoldout.Add(innerFrameScaleField);
        innerFrameFoldout.Add(innerFrameDepthField);

        #endregion

        #region Pane
        Foldout paneFoldout = new Foldout() { text = "Pane" };

        PropertyField paneDepthField = new PropertyField(props.PaneDepth) { label = "Depth" };
        paneDepthField.BindProperty(props.PaneDepth);

        paneFoldout.Add(paneDepthField);

        #endregion

        #region Shutters
        Foldout shuttersFoldout = new Foldout() { text = "Shutters" };

        PropertyField shuttersDepthField = new PropertyField(props.ShuttersDepth) { label = "Depth" };
        shuttersDepthField.BindProperty(props.ShuttersDepth);

        PropertyField shuttersAngleField = new PropertyField(props.ShuttersAngle) { label = "Angle" };
        shuttersAngleField.BindProperty(props.ShuttersAngle);

        shuttersFoldout.Add(shuttersDepthField);
        shuttersFoldout.Add(shuttersAngleField);

        #endregion
        #endregion

        #region Register Value Change Callback
        activeElementsField.RegisterValueChangeCallback(evt => 
        {
            if (evt == null)
                return;

            WindowElement currentlyActive = evt.changedProperty.GetEnumValue<WindowElement>();

            bool isOuterFrameActive = currentlyActive.IsElementActive(WindowElement.OuterFrame);
            bool isInnerFrameActive = currentlyActive.IsElementActive(WindowElement.InnerFrame);
            bool isPaneActive = currentlyActive.IsElementActive(WindowElement.Pane);
            bool areShuttersActive = currentlyActive.IsElementActive(WindowElement.Shutters);

            outerFrameFoldout.SetEnabled(isOuterFrameActive);
            innerFrameFoldout.SetEnabled(isInnerFrameActive);
            paneFoldout.SetEnabled(isPaneActive);
            shuttersFoldout.SetEnabled(areShuttersActive);

            WindowData[] windows = GetWindowDataFromBuildable(buildable);

            bool rebuild = false;

            foreach (WindowData win in windows)
            {
                bool wasOuterFrameActive = win.IsOuterFrameActive;
                bool wasInnerFrameActive = win.IsInnerFrameActive;
                bool wasPaneActive = win.IsPaneActive;
                bool wereShuttersActive = win.AreShuttersActive;

                if (isOuterFrameActive == true && wasOuterFrameActive == false)
                {
                    win.DoesOuterFrameNeedRebuild = true;
                    rebuild = true;
                }
                    
                if (isInnerFrameActive == true && wasInnerFrameActive == false)
                {
                    win.DoesInnerFrameNeedRebuild = true;
                    rebuild = true;
                }
                    
                if (isPaneActive == true && wasPaneActive == false)
                {
                    win.DoesPaneNeedRebuild = true;
                    rebuild = true;
                }
                    
                if (areShuttersActive == true && wereShuttersActive == false)
                {
                    win.DoShuttersNeedRebuild = true;
                    rebuild = true;
                }

                if(buildable is not Window)
                    win.ActiveElements = currentlyActive;
            }

            if(rebuild)
            {
                buildable.Demolish();
                buildable.Build();
            }
        });

        #region Outer Frame
        outerScaleField.RegisterValueChangeCallback(evt => 
        {
            if (evt == null)
                return;

            WindowData[] windows = GetWindowDataFromBuildable(buildable);

            foreach (WindowData win in windows)
            {
                if (win.OuterFrameScale == evt.changedProperty.floatValue && buildable is not Window)
                    continue;

                win.OuterFrameScale = evt.changedProperty.floatValue;
                win.DoesOuterFrameNeedRebuild = true;
                win.DoesInnerFrameNeedRebuild = true;
            }

            Build(buildable);
            return;
            
        });
        outerFrameDepthField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            WindowData[] windows = GetWindowDataFromBuildable(buildable);

            foreach (WindowData win in windows)
            {
                if (win.OuterFrameDepth == evt.changedProperty.floatValue && buildable is not Window)
                    continue;

                win.OuterFrameDepth = evt.changedProperty.floatValue;
                win.DoesOuterFrameNeedRebuild = true;
            }

            Build(buildable);

        });
        #endregion

        #region Inner Frame
        colsField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            WindowData[] windows = GetWindowDataFromBuildable(buildable); 

            foreach (WindowData win in windows)
            {
                if (win.InnerFrameColumns == evt.changedProperty.intValue && buildable is not Window)
                    continue;

                win.InnerFrameColumns = evt.changedProperty.intValue;
                win.InnerFrameHolePoints = Window.CalculateInnerFrame(win);
                win.DoesInnerFrameNeedRebuild = true;
            }

            Build(buildable);
                
        });
        rowsField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            WindowData[] windows = GetWindowDataFromBuildable(buildable);

            foreach (WindowData win in windows)
            {
                if (win.InnerFrameRows == evt.changedProperty.intValue && buildable is not Window)
                    continue;

                win.InnerFrameRows = evt.changedProperty.intValue;
                win.InnerFrameHolePoints = Window.CalculateInnerFrame(win);
                win.DoesInnerFrameNeedRebuild = true;
            }

            Build(buildable);
        });
        innerFrameScaleField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            WindowData[] windows = GetWindowDataFromBuildable(buildable);

            foreach (WindowData win in windows)
            {
                if (win.InnerFrameScale == evt.changedProperty.floatValue && buildable is not Window)
                    continue;

                win.InnerFrameScale = evt.changedProperty.floatValue;
                win.DoesInnerFrameNeedRebuild = true;
            }

            Build(buildable);

        });
        innerFrameDepthField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            WindowData[] windows = GetWindowDataFromBuildable(buildable);

            foreach (WindowData win in windows)
            {
                if (win.InnerFrameDepth == evt.changedProperty.floatValue && buildable is not Window)
                    continue;

                win.InnerFrameDepth = evt.changedProperty.floatValue;
                win.DoesInnerFrameNeedRebuild = true;
            }
            Build(buildable);

        });
        #endregion

        #region Pane
        paneDepthField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            WindowData[] windows = GetWindowDataFromBuildable(buildable);

            foreach (WindowData win in windows)
            {
                if (win.PaneDepth == evt.changedProperty.floatValue && buildable is not Window)
                    continue;

                win.PaneDepth = evt.changedProperty.floatValue;
                win.DoesPaneNeedRebuild = true;
            }

            Build(buildable);
        });
        #endregion

        #region Shutters
        shuttersDepthField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            WindowData[] windows = GetWindowDataFromBuildable(buildable);

            foreach (WindowData win in windows)
            {
                if (win.ShuttersDepth == evt.changedProperty.floatValue && buildable is not Window)
                    continue;

                win.ShuttersDepth = evt.changedProperty.floatValue;
                win.DoShuttersNeedRebuild = true;
            }

            Build(buildable);
        });
        shuttersAngleField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            WindowData[] windows = GetWindowDataFromBuildable(buildable);

            foreach (WindowData win in windows)
            {
                if (win.ShuttersAngle == evt.changedProperty.floatValue && buildable is not Window)
                    continue;

                win.ShuttersAngle = evt.changedProperty.floatValue;
                win.DoShuttersNeedRebuild = true;
            }

            Build(buildable);
        });
        #endregion
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
    /// <summary>
    /// Window data could be attached to different buildable objects.
    /// In some instances, we want to apply window data changes to multiple other
    /// data elements that are contained in those buildable objects.
    /// </summary>
    /// <param name="buildable"></param>
    /// <returns></returns>
    private WindowData[] GetWindowDataFromBuildable(IBuildable buildable)
    {
        WindowData[] dataset = new WindowData[0];

        switch (buildable)
        {
            case Wall:
                {
                    // TODO: instead of the first section index, get the one currently selected in the wall inspector.
                    Wall wall = buildable as Wall;
                    dataset = wall.Data.Sections[0].Windows;
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
                    dataset = new WindowData[] { window.Data };
                }
                break;
        }

        return dataset;
    }

    private void Build(IBuildable buildable)
    {
        switch(buildable)
        {
            case Wall:
                // TODO the the wall section that is selected in the inspector & do the section build case.
                break;
            case WallSection:
                {
                    WallSection section = buildable as WallSection;

                    for (int i = 0; i < section.transform.childCount; i++)
                    {
                        if (section.transform.GetChild(i).TryGetComponent(out Window window))
                        {
                            window.Build();
                        }
                    }
                }
                break;
            case Window:
                buildable.Build();
                break;
        }
    }
}
