using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using static Codice.Client.BaseCommands.BranchExplorer.Layout.BrExLayout;
using System;
using static UnityEngine.Rendering.CoreUtils;
using UnityEngine.Serialization;

[CustomEditor(typeof(Wall))]
public class WallEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement container = new VisualElement() { name = "Wall Container"};

        serializedObject.Update();

        Wall wall = (Wall)target;

        SerializedProperty data = serializedObject.FindProperty("m_Data");

        SerializedProperty columns = data.FindPropertyRelative("m_Columns");
        SerializedProperty rows = data.FindPropertyRelative("m_Rows");

        VisualElement selectableBoxContainer = new VisualElement();
        selectableBoxContainer.style.height = 200;
        selectableBoxContainer.style.width = 200;
        selectableBoxContainer.style.left = 20;
        selectableBoxContainer.style.top = 1;

        PropertyField columnsField = new PropertyField(columns);
        columnsField.style.minWidth = 300;
        columnsField.BindProperty(columns);
        columnsField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            bool rebuild = false;

            if(wall.Data.Sections == null || wall.Data.Sections.Length < 1 || wall.Data.Sections[0] == null || wall.Data.Sections.Length != wall.Data.Columns * wall.Data.Rows)
                rebuild = true;

            if (!rebuild)
                return;

            Debug.Log("Sections have been re-initialized");
            wall.Data.Sections = new WallSectionData[wall.Data.Columns * wall.Data.Rows];
            wall.Build();

            selectableBoxContainer.Clear();
            AddGridOfSelectableBoxes(selectableBoxContainer, columns.intValue, rows.intValue);
        });

        PropertyField rowsField = new PropertyField(rows);
        rowsField.style.minWidth = 300;
        rowsField.BindProperty(rows);
        rowsField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            bool rebuild = false;

            if (wall.Data.Sections == null || wall.Data.Sections.Length < 1 || wall.Data.Sections[0] == null || wall.Data.Sections.Length != wall.Data.Columns * wall.Data.Rows)
                rebuild = true;

            if (!rebuild)
                return;

            Debug.Log("Sections have been re-initialized");
            wall.Data.Sections = new WallSectionData[wall.Data.Columns * wall.Data.Rows];
            wall.Build();

            selectableBoxContainer.Clear();
            AddGridOfSelectableBoxes(selectableBoxContainer, columns.intValue, rows.intValue);
        });

        VisualElement sliderContainer = new VisualElement();
        sliderContainer.style.height = 200;

        sliderContainer.Add(columnsField);
        sliderContainer.Add(rowsField);

        AddGridOfSelectableBoxes(selectableBoxContainer, columns.intValue, rows.intValue);

        VisualElement wallBox = new VisualElement();
        wallBox.style.height = 200;
        wallBox.style.left = 0;
        wallBox.style.right = 0;
        wallBox.style.flexDirection = FlexDirection.Row;
        wallBox.Add(sliderContainer);
        wallBox.Add(selectableBoxContainer);

        container.Add(wallBox);

        return container;
    }

    private void AddGridOfSelectableBoxes(VisualElement root, int columns, int rows)
    {
        Wall wall = target as Wall;

        if (wall.Data == null)
            return;

        if (wall.Data.Sections == null)
            return;

        if (wall.Data.Sections.Length == 0)
            return;

        foreach(WallSectionData sectionData in wall.Data.Sections)
        {
            if (sectionData == null)
                return;
        }

        // Define the grid parameters
        float gridSpace = 200;
        float spacing = 5f;
        float borderWidth = 1f;

        // Calculate the width and height of the grid container
        float boxWidth = (gridSpace / columns) - spacing;
        float boxHeight = (gridSpace / rows) - spacing;

        // Create a grid container
        VisualElement gridContainer = new VisualElement();
        gridContainer.style.width = gridSpace;
        gridContainer.style.height = gridSpace;
        gridContainer.style.flexDirection = FlexDirection.Column;
        gridContainer.style.flexWrap = Wrap.Wrap;

        

        // Create and add selectable boxes to the grid
        for (int x = 0; x < columns; x++)
        {
            VisualElement colContainer = new VisualElement();
            colContainer.style.flexDirection = FlexDirection.Column;

            for (int y = 0; y < rows; y++)
            {
                // Create a selectable box
                VisualElement box = new VisualElement();
                box.style.width = boxWidth;
                box.style.height = boxHeight;
                box.style.marginRight = spacing;
                box.style.marginBottom = spacing;

                // Add a border to the box
                box.style.borderBottomWidth = borderWidth;
                box.style.borderTopWidth = borderWidth;
                box.style.borderLeftWidth = borderWidth;
                box.style.borderRightWidth = borderWidth;
                box.style.borderBottomColor = Color.white;
                box.style.borderTopColor = Color.white;
                box.style.borderLeftColor = Color.white;
                box.style.borderRightColor = Color.white;
                box.style.alignContent = Align.Center;
                box.style.alignItems = Align.Center;

                Label label = new Label();

                // Add a click event handler (for example)
                box.AddManipulator(new Clickable(() =>
                {
                    SerializedProperty mdata = serializedObject.FindProperty("m_Data");
                    SerializedProperty sections = mdata.FindPropertyRelative("m_Sections");
                    SerializedProperty columnArrayElement = sections.GetArrayElementAtIndex(x);
                    SerializedProperty sectionElement = columnArrayElement.GetArrayElementAtIndex(y);

                    PropertyField sectionField = new PropertyField(sectionElement);
                    sectionField.BindProperty(sectionElement);
                    
                    root.Add(sectionField);
                }));

                //if (wall != null && wall.Data != null && wall.Data.Sections != null)
                //{
                //    try
                //    {
                //        WallSectionData data = wall.Data.Sections[x,y];
                //    }
                //    catch(IndexOutOfRangeException ex)
                //    {
                //        Debug.Log("Cols " + x + " Rows " + y);
                //    }
                //}

                box.Add(label);

                // Add the box to the grid container
                colContainer.Add(box);
            }
            gridContainer.Add(colContainer);
        }

        // Add the grid container to the root
        root.Add(gridContainer);
    }
}
