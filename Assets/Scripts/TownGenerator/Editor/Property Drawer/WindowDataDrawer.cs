using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Rendering;
using System.Linq;
using Unity.VisualScripting;

[CustomPropertyDrawer(typeof(WindowData))]
public class WindowDataDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        VisualElement container = new VisualElement();

        WindowDataSerializedProperties props = new WindowDataSerializedProperties(data);

        IBuildable buildable = data.serializedObject.targetObject as IBuildable;

        #region Fields
        PropertyField activeElementsField = new PropertyField(props.ActiveElements);
        Foldout outerFrameFoldout = new Foldout() { text = "Outer Frame" };
        PropertyField outerFrameField = new PropertyField(props.OuterFrame.Data);
        Foldout innerFrameFoldout = new Foldout() { text = "Inner Frame" };
        PropertyField innerFrameField = new PropertyField(props.InnerFrame.Data);
        Foldout paneFoldout = new Foldout() { text = "Pane" };
        PropertyField paneField = new PropertyField(props.Data);
        Foldout shuttersFoldout = new Foldout() { text = "Shutters" };
        // How did this look before?
        PropertyField leftShutter = new PropertyField(props.LeftShutter.Data);
        PropertyField rightShutter = new PropertyField(props.RightShutter.Data);
        #endregion

        #region Bind
        activeElementsField.BindProperty(props.ActiveElements);
        outerFrameField.BindProperty(props.OuterFrame.Data);
        innerFrameField.BindProperty(props.InnerFrame.Data);
        paneField.BindProperty(props.Pane.Data);
        leftShutter.BindProperty(props.LeftShutter.Data);
        rightShutter.BindProperty(props.RightShutter.Data);
        #endregion

        #region Register Value Change Callback
        activeElementsField.RegisterValueChangeCallback(evt => 
        {
            WindowElement currentlyActive = evt.changedProperty.GetEnumValue<WindowElement>();

            bool isOuterFrameActive = currentlyActive.IsElementActive(WindowElement.OuterFrame);
            bool isInnerFrameActive = currentlyActive.IsElementActive(WindowElement.InnerFrame);
            bool isPaneActive = currentlyActive.IsElementActive(WindowElement.Pane);
            bool areShuttersActive = currentlyActive.IsElementActive(WindowElement.Shutters);

            outerFrameFoldout.SetEnabled(isOuterFrameActive);
            innerFrameFoldout.SetEnabled(isInnerFrameActive);
            paneFoldout.SetEnabled(isPaneActive);
            shuttersFoldout.SetEnabled(areShuttersActive);

            WindowData[] windows = GetDataFromBuildable(buildable);

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

            Demolish(buildable);

            if (rebuild)
            {
                Build(buildable);
            }
        });
        #endregion

        #region Add Fields to Container
        outerFrameFoldout.Add(outerFrameField);
        innerFrameFoldout.Add(innerFrameField);
        paneFoldout.Add(paneField);
        shuttersFoldout.Add(leftShutter);
        shuttersFoldout.Add(rightShutter);
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
    private WindowData[] GetDataFromBuildable(IBuildable buildable)
    {
        WindowData[] dataset = new WindowData[0];

        switch (buildable)
        {
            case Wall:
                {
                    // TODO: instead of the first section index, get the one currently selected in the wall inspector.
                    Wall wall = buildable as Wall;
                    dataset = wall.Data.Sections[0].WindowOpening.Windows;
                }
                break;
            case WallSection:
                {
                    WallSection wallSection = buildable as WallSection;
                    dataset = wallSection.Data.WindowOpening.Windows;
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
                    section.BuildChildren();
                }
                break;
            case Window:
                buildable.Build();
                break;
        }
    }

    private void Demolish(IBuildable buildable)
    {
        switch (buildable)
        {
            case Wall:
                // TODO the the wall section that is selected in the inspector & do the section demolish case.
                break;
            case WallSection:
                {
                    WallSection section = buildable as WallSection;

                    for (int i = 0; i < section.transform.childCount; i++)
                    {
                        if (section.transform.GetChild(i).TryGetComponent(out Window window))
                        {
                            window.Demolish();
                        }
                    }
                }
                break;
            case Window:
                buildable.Demolish();
                break;
        }
    }
}
