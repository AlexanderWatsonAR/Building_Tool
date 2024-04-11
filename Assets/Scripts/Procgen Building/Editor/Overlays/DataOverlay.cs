using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Overlays;
using OnlyInvalid.ProcGenBuilding.Common;
using OnlyInvalid.ProcGenBuilding.Building;
using OnlyInvalid.ProcGenBuilding.Window;
using OnlyInvalid.ProcGenBuilding.Storey;
using OnlyInvalid.ProcGenBuilding.Pillar;
using OnlyInvalid.ProcGenBuilding.Floor;
using OnlyInvalid.ProcGenBuilding.Wall;
using OnlyInvalid.ProcGenBuilding.Roof;
using OnlyInvalid.ProcGenBuilding.Door;

public class DataOverlay : Overlay
{
    protected VisualElement m_Root;
    protected SerializedObject m_SerializedObject;
    protected SerializedProperty m_Data;
    protected PropertyField m_DataField;
    protected GameObject m_GameObject;

    public override VisualElement CreatePanelContent()
    {
        m_Root = new VisualElement();

        // Should I make IBuildable a public class
        IBuildable buildable = m_GameObject.GetComponent<IBuildable>();

        switch(buildable) // this switch is a lil silly
        {
            case Building:
                Building building = buildable as Building;
                m_SerializedObject = new SerializedObject(building);
                break;
            case Storey:
                Storey storey = buildable as Storey;
                m_SerializedObject = new SerializedObject(storey);
                break;
            case Walls:
                Walls walls = buildable as Walls;
                m_SerializedObject = new SerializedObject(walls);
                break;
            case Wall:
                Wall wall = buildable as Wall;
                m_SerializedObject = new SerializedObject(wall);
                break;
            case WallSection:
                WallSection wallSection = buildable as WallSection;
                m_SerializedObject = new SerializedObject(wallSection);
                break;
            case Window:
                Window window = buildable as Window;
                m_SerializedObject = new SerializedObject(window);
                break;
            case Door:
                Door door = buildable as Door;
                m_SerializedObject = new SerializedObject(door);
                break;
            case Floor:
                Floor floor = buildable as Floor;
                m_SerializedObject = new SerializedObject(floor);
                break;
            case FloorSection:
                FloorSection floorSection = buildable as FloorSection;
                m_SerializedObject = new SerializedObject(floorSection);
                break;
            case Roof:
                Roof roof = buildable as Roof;
                m_SerializedObject = new SerializedObject(roof);
                break;
            case RoofTile:
                RoofTile roofTile = buildable as RoofTile;
                m_SerializedObject = new SerializedObject(roofTile);
                break;
            case RoofSection:
                RoofSection roofSection = buildable as RoofSection;
                m_SerializedObject = new SerializedObject(roofSection);
                break;
            case Pillars:
                Pillars pillars = buildable as Pillars;
                m_SerializedObject = new SerializedObject(pillars);
                break;
            case Pillar:
                Pillar pillar = buildable as Pillar;
                m_SerializedObject = new SerializedObject(pillar);
                break;
        }

        m_Data = m_SerializedObject.FindProperty("m_Data");
        m_DataField = new PropertyField(m_Data);
        m_DataField.BindProperty(m_Data);
        return m_Root;
    }
}
