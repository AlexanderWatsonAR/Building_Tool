using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Rendering;

[CustomEditor(typeof(BuildingScriptableObject))]
public class BuildingScriptableObjectEditor : Editor
{
    // What is the purpose of this editor?
    // This could be useful for debugging purposes.
    // Check if data is being serialized by reference.

    BuildingDataSerializedProperties m_Props;

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();

        serializedObject.Update();

        SerializedProperty data = serializedObject.FindProperty("m_Data");

        m_Props = new BuildingDataSerializedProperties(data);

        Label numberOfStoreys = new Label("Storey Count: " + m_Props.Storeys.Count);

        root.Add(numberOfStoreys);

        foreach(var storey in m_Props.Storeys)
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

            VisualElement horizontalContainer = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                }
            };

            horizontalContainer.Add(new Label("Columns: " + wall.Columns.intValue));
            horizontalContainer.Add(new Label("Rows: " + wall.Rows.intValue));
            horizontalContainer.Add(new Label("Size: " + wall.Columns.intValue * wall.Rows.intValue));

            wallFoldout.Add(horizontalContainer);
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

                        PropertyField doorwayField = new PropertyField(doorway.Data);

                        foreach (var door in doorway.Doors)
                        {
                            Foldout doorFoldout = new Foldout() { text = "Door" };
                            PropertyField doorField = new PropertyField(door.Data);
                            doorFoldout.Add(doorField);
                            sectionFoldout.Add(doorFoldout);
                        }
                    }
                    break;
                case WallElement.Archway:
                    {
                        var archway = section.Archway;

                        // TODO make a archway prop drawer.
                        PropertyField archwayField = new PropertyField(archway.Data);

                        foreach (var door in archway.Doors)
                        {
                            PropertyField doorField = new PropertyField(door.Data);
                        }
                    }
                    break;
                case WallElement.Window:
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
