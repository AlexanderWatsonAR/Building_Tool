using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Rendering;
using OnlyInvalid.ProcGenBuilding.Storey;
using OnlyInvalid.ProcGenBuilding.Wall;

namespace OnlyInvalid.ProcGenBuilding.Building
{
    [CustomEditor(typeof(BuildingScriptableObject))]
    public class BuildingScriptableObjectEditor : Editor
    {
        // What is the purpose of this editor?
        // This could be useful for debugging purposes.
        // Check if data is being serialized by reference.

        internal class GridVisualElement : HorizontalVisualElement
        {
            Label m_Columns, m_Rows, m_Size;

            public GridVisualElement(int columns, int rows) : base()
            {
                m_Columns = new Label("Columns: " + columns.ToString());
                m_Rows = new Label("Rows: " + rows.ToString());
                m_Size = new Label("Size: " + (columns * rows).ToString());

                this.Add(m_Columns);
                this.Add(m_Rows);
                this.Add(m_Size);
            }
        }

        internal class SizeVisualElement : HorizontalVisualElement
        {
            Label m_Height, m_Width;

            public SizeVisualElement(float height, float width) : base()
            {
                m_Height = new Label("Height: " + height.ToString());
                m_Width = new Label("Width: " + width.ToString());

                this.Add(m_Height);
                this.Add(m_Width);
            }
        }

        internal class HorizontalVisualElement : VisualElement
        {
            public HorizontalVisualElement() : base()
            {
                this.style.flexDirection = FlexDirection.Row;
            }
        }

        BuildingDataSerializedProperties m_Props;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            serializedObject.Update();

            SerializedProperty data = serializedObject.FindProperty("m_Data");

            m_Props = new BuildingDataSerializedProperties(data);

            Label numberOfStoreys = new Label("Storey Count: " + m_Props.Storeys.Count);

            root.Add(numberOfStoreys);

            foreach (var storey in m_Props.Storeys)
            {
                Foldout storeyFoldout = new Foldout() { text = "Storey " + storey.ID.intValue };
                storeyFoldout.Add(new Label("Name: " + storey.Name.stringValue));

                VisualElement walls = DefineWallsField(storey);
                storeyFoldout.Add(walls);
                root.Add(storeyFoldout);
            }
            return root;
        }

        private VisualElement DefineWallsField(StoreyDataSerializedProperties storey)
        {
            Foldout wallsFoldout = new Foldout() { text = "Walls" };

            wallsFoldout.Add(new Label("Height: " + storey.Wall.Height.floatValue));
            wallsFoldout.Add(new Label("Depth: " + storey.Wall.Depth.floatValue));

            foreach (var wall in storey.Walls)
            {
                Foldout wallFoldout = new Foldout() { text = "Wall " + wall.ID.intValue };

                GridVisualElement grid = new GridVisualElement(wall.Columns.intValue, wall.Rows.intValue);

                //horizontalContainer.Add(new Label("Grid Size: " + wall.Columns.intValue * wall.Rows.intValue));
                //horizontalContainer.Add(new Label("Columns: " + wall.Columns.intValue));
                //horizontalContainer.Add(new Label("Rows: " + wall.Rows.intValue));


                //wallFoldout.Add(horizontalContainer);
                wallFoldout.Add(grid);
                wallFoldout.Add(DefineWallSectionsFields(wall));
                wallsFoldout.Add(wallFoldout);
            }

            return wallsFoldout;
        }

        private VisualElement DefineWallSectionsFields(WallDataSerializedProperties wall)
        {
            VisualElement sections = new VisualElement();

            foreach (var section in wall.Sections)
            {
                Foldout sectionFoldout = new Foldout { text = "Wall Section" + section.ID.vector2IntValue };

                WallElement activeElement = section.WallElement.GetEnumValue<WallElement>();

                sectionFoldout.Add(new Label("Wall Element: " + activeElement));

                switch (activeElement)
                {
                    case WallElement.Wall:

                        break;
                    case WallElement.Doorway:
                        {
                            var doorway = section.Doorway;
                            sectionFoldout.Add(new GridVisualElement(doorway.Columns.intValue, doorway.Rows.intValue));
                            sectionFoldout.Add(new SizeVisualElement(doorway.Height.floatValue, doorway.Width.floatValue));
                        }
                        break;
                    case WallElement.Archway:
                        {
                            var archway = section.Archway;

                            PropertyField archwayField = new PropertyField(archway.Data);

                            foreach (var door in archway.Doors)
                            {
                                PropertyField doorField = new PropertyField(door.Data);
                            }
                        }
                        break;
                    case WallElement.Window:
                        {
                            var windowOpening = section.WindowOpening;
                            sectionFoldout.Add(new GridVisualElement(windowOpening.Columns.intValue, windowOpening.Rows.intValue));

                            SizeVisualElement shape = new SizeVisualElement(windowOpening.Height.floatValue, windowOpening.Width.floatValue);
                            shape.Add(new Label("Sides: " + windowOpening.Sides.intValue));
                            shape.Add(new Label("Angle: " + windowOpening.Angle.floatValue));
                            sectionFoldout.Add(shape);

                            foreach (var window in windowOpening.Windows)
                            {
                                Foldout windowFoldout = new Foldout() { text = "Window " + window.ID.intValue };

                                HorizontalVisualElement activeElementsCon = new();


                                HorizontalVisualElement outerFrameCon = new();

                                var outerFrame = window.OuterFrame;

                                outerFrameCon.Add(new Label("Outer Frame"));
                                outerFrameCon.Add(new Label("Depth: " + outerFrame.Depth.floatValue.ToString()));
                                outerFrameCon.Add(new Label("Scale: " + outerFrame.Scale.floatValue.ToString()));

                                HorizontalVisualElement innerFrameCon = new();

                                var innerFrame = window.InnerFrame;

                                innerFrameCon.Add(new Label("Inner Frame"));
                                innerFrameCon.Add(new GridVisualElement(innerFrame.Columns.intValue, innerFrame.Rows.intValue));
                                innerFrameCon.Add(new Label("Depth: " + innerFrame.Depth.floatValue.ToString()));
                                innerFrameCon.Add(new Label("Scale: " + innerFrame.Scale.floatValue.ToString()));

                                HorizontalVisualElement paneCon = new();
                                paneCon.Add(new Label("Pane: "));
                                paneCon.Add(new Label("Depth: " + window.Pane.Depth.floatValue));

                                windowFoldout.Add(outerFrameCon);
                                windowFoldout.Add(innerFrameCon);
                                windowFoldout.Add(paneCon);
                                sectionFoldout.Add(windowFoldout);

                            }
                        }
                        break;
                    case WallElement.Extension:
                        break;
                    case WallElement.Empty:
                        break;
                }

                sections.Add(sectionFoldout);
            }

            return sections;
        }

    }
}
