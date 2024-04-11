using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine.Serialization;
using UnityEditor.Rendering;
using Unity.VisualScripting;

namespace OnlyInvalid.ProcGenBuilding.Wall
{
    [CustomEditor(typeof(Wall))]
    public class WallEditor : Editor
    {
        [SerializeField] private SerializedProperty m_ActiveSection;

        class Section : VisualElement
        {
            private SerializedProperty m_WallSection;
            public SerializedProperty WallSection => m_WallSection;

            public Section(SerializedProperty wallSection, float height, float width, Color? border = null)
            {
                m_WallSection = wallSection;
                Color edge = border.HasValue ? border.Value : Color.white;
                style.width = width;
                style.height = height;

                style.marginRight = 5;
                style.marginBottom = 5;

                style.borderBottomWidth = 1;
                style.borderTopWidth = 1;
                style.borderLeftWidth = 1;
                style.borderRightWidth = 1;
                style.borderBottomColor = edge;
                style.borderTopColor = edge;
                style.borderLeftColor = edge;
                style.borderRightColor = edge;
                style.justifyContent = Justify.Center;
                style.alignContent = Align.Center;
                style.alignItems = Align.Center;

                SerializedProperty wallElement = wallSection.FindPropertyRelative("m_WallElement");
                string text = wallElement.GetEnumName<WallElement>();

                SerializedProperty id = wallSection.FindPropertyRelative("m_ID");

                Label label = new(text);
                Add(label);

                //this.AddManipulator(new Clickable(() =>
                //{

                //    Debug.Log(id.vector2IntValue);
                //}));
            }
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement() { name = "Wall Container" };

            serializedObject.Update();

            Wall wall = (Wall)target;

            SerializedProperty data = serializedObject.FindProperty("m_Data");

            SerializedProperty columns = data.FindPropertyRelative("m_Columns");
            SerializedProperty rows = data.FindPropertyRelative("m_Rows");

            VisualElement selectableBoxContainer = new VisualElement()
            {
                name = "Box Container",
                style =
            {
                height = 200,
                width = 200,
                left = 20,
                top = 1
            }
            };

            VisualElement sectionContainer = new VisualElement() { name = "Section Container" };

            PropertyField columnsField = new PropertyField(columns) { name = "Columns" };
            columnsField.style.minWidth = 300;
            columnsField.BindProperty(columns);
            columnsField.RegisterValueChangeCallback(evt =>
            {
                if (evt == null)
                    return;

                bool rebuild = false;

                if (wall.WallData.Sections == null || wall.WallData.Sections.Length < 1 || wall.WallData.Sections[0] == null || wall.WallData.Sections.Length != wall.WallData.Columns * wall.WallData.Rows)
                    rebuild = true;

                if (!rebuild)
                    return;

                Debug.Log("Sections have been re-initialized");
                wall.WallData.Sections = new WallSectionData[wall.WallData.Columns * wall.WallData.Rows];
                wall.Build();

                data.FindPropertyRelative("m_Sections").SetUnderlyingValue(wall.WallData.Sections);

                selectableBoxContainer.Clear();
                AddGridOfSelectableBoxes(selectableBoxContainer, sectionContainer, columns.intValue, rows.intValue);
                sectionContainer.Clear();
            });

            PropertyField rowsField = new PropertyField(rows) { name = "Rows" };
            rowsField.style.minWidth = 300;
            rowsField.BindProperty(rows);
            rowsField.RegisterValueChangeCallback(evt =>
            {
                if (evt == null)
                    return;

                bool rebuild = false;

                if (wall.WallData.Sections == null || wall.WallData.Sections.Length < 1 || wall.WallData.Sections[0] == null || wall.WallData.Sections.Length != wall.WallData.Columns * wall.WallData.Rows)
                    rebuild = true;

                if (!rebuild)
                    return;

                Debug.Log("Sections have been re-initialized");
                wall.WallData.Sections = new WallSectionData[wall.WallData.Columns * wall.WallData.Rows];
                wall.Build();

                data.FindPropertyRelative("m_Sections").SetUnderlyingValue(wall.WallData.Sections);

                selectableBoxContainer.Clear();
                AddGridOfSelectableBoxes(selectableBoxContainer, sectionContainer, columns.intValue, rows.intValue);
                sectionContainer.Clear();
            });

            VisualElement sliderContainer = new VisualElement() { name = "Sliders" };
            sliderContainer.Add(columnsField);
            sliderContainer.Add(rowsField);

            AddGridOfSelectableBoxes(selectableBoxContainer, sectionContainer, columns.intValue, rows.intValue);

            root.Add(sliderContainer);
            root.Add(selectableBoxContainer);
            root.Add(sectionContainer);


            return root;
        }

        private void AddGridOfSelectableBoxes(VisualElement container, VisualElement sectionContainer, int columns, int rows)
        {
            Wall wall = target as Wall;

            if (wall.WallData == null)
                return;

            if (wall.WallData.Sections == null)
                return;

            if (wall.WallData.Sections.Length == 0)
                return;

            foreach (WallSectionData sectionData in wall.WallData.Sections)
            {
                if (sectionData == null)
                    return;
            }

            float gridSpace = 200;
            float spacing = 5f;
            float borderWidth = 1f;

            float boxWidth = (gridSpace / columns) - spacing;
            float boxHeight = (gridSpace / rows) - spacing;

            VisualElement gridContainer = new VisualElement();
            gridContainer.style.width = gridSpace;
            gridContainer.style.height = gridSpace;
            gridContainer.style.flexDirection = FlexDirection.Column;
            gridContainer.style.flexWrap = Wrap.Wrap;

            SerializedProperty data = serializedObject.FindProperty("m_Data");
            SerializedProperty sections = data.FindPropertyRelative("m_Sections");

            int count = 0;

            for (int x = 0; x < columns; x++)
            {
                VisualElement colContainer = new VisualElement();
                colContainer.style.flexDirection = FlexDirection.Column;

                for (int y = 0; y < rows; y++)
                {
                    Section section = new Section(sections.GetArrayElementAtIndex(count), boxHeight, boxWidth);

                    section.AddManipulator(new Clickable(() =>
                    {
                        AddSection(sectionContainer, section.WallSection);
                    }));

                    colContainer.Add(section);
                    count++;
                }
                gridContainer.Add(colContainer);
            }
            container.Add(gridContainer);
        }

        private void AddSection(VisualElement container, SerializedProperty section)
        {
            container.Clear();

            WallSectionData data = section.GetUnderlyingValue() as WallSectionData;

            Foldout foldout = new Foldout() { text = "Wall Section " + data.ID };

            PropertyField sectionField = new PropertyField(section);
            sectionField.BindProperty(section);

            foldout.Add(sectionField);
            container.Add(foldout);
        }
    }
}
