using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Rendering;

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

            WindowElement win = (WindowElement) evt.newValue;

            outerFrameFoldout.SetEnabled(win.IsElementActive(WindowElement.OuterFrame));
            innerFrameFoldout.SetEnabled(win.IsElementActive(WindowElement.InnerFrame));
            paneFoldout.SetEnabled(win.IsElementActive(WindowElement.Pane));
            shuttersFoldout.SetEnabled(win.IsElementActive(WindowElement.Shutters));

            if (evt.newValue == evt.previousValue)
                return;

            if (buildable is WallSection)
            {
                WallSection wallSection = buildable as WallSection;
                foreach (WindowData winData in wallSection.Data.Windows)
                {
                    winData.ActiveElements = win;
                }
            }



            buildable.Build();
        });
        outerScaleField.RegisterValueChangeCallback(evt => 
        {
            if (evt == null)
                return;

            if(buildable is WallSection)
            {
                WallSection wallSection = buildable as WallSection;
                foreach(WindowData winData in wallSection.Data.Windows)
                {
                    winData.OuterFrameScale = evt.changedProperty.floatValue;
                }
            }

            buildable.Build();
        });
        outerFrameDepthField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            if (buildable is WallSection)
            {
                WallSection wallSection = (WallSection)buildable;
                foreach (WindowData winData in wallSection.Data.Windows)
                {
                    winData.OuterFrameDepth = evt.changedProperty.floatValue;
                }
            }

            buildable.Build();
        });
        colsField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            if (buildable is WallSection)
            {
                WallSection wallSection = (WallSection)buildable;
                foreach (WindowData winData in wallSection.Data.Windows)
                {
                    winData.InnerFrameColumns = evt.changedProperty.intValue;
                }
            }

            buildable.Build();
        });
        rowsField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            if (buildable is WallSection)
            {
                WallSection wallSection = (WallSection)buildable;
                foreach (WindowData winData in wallSection.Data.Windows)
                {
                    winData.InnerFrameRows = evt.changedProperty.intValue;
                }
            }

            buildable.Build();
        });
        innerFrameScaleField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            if (buildable is WallSection)
            {
                WallSection wallSection = (WallSection)buildable;
                foreach (WindowData winData in wallSection.Data.Windows)
                {
                    winData.InnerFrameScale = evt.changedProperty.floatValue;
                }
            }

            buildable.Build();
        });
        innerFrameDepthField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            if (buildable is WallSection)
            {
                WallSection wallSection = (WallSection)buildable;
                foreach (WindowData winData in wallSection.Data.Windows)
                {
                    winData.InnerFrameDepth = evt.changedProperty.floatValue;
                }
            }

            buildable.Build();
        });
        paneDepthField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            if (buildable is WallSection)
            {
                WallSection wallSection = (WallSection)buildable;
                foreach (WindowData winData in wallSection.Data.Windows)
                {
                    winData.PaneDepth = evt.changedProperty.floatValue;
                }
            }

            buildable.Build();
        });
        shuttersDepthField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            if (buildable is WallSection)
            {
                WallSection wallSection = (WallSection)buildable;
                foreach (WindowData winData in wallSection.Data.Windows)
                {
                    winData.ShuttersDepth = evt.changedProperty.floatValue;
                }
            }

            buildable.Build();
        });
        shuttersAngleField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            if (buildable is WallSection)
            {
                WallSection wallSection = (WallSection)buildable;
                foreach (WindowData winData in wallSection.Data.Windows)
                {
                    winData.ShuttersAngle = evt.changedProperty.floatValue;
                }
            }

            buildable.Build();
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
}
