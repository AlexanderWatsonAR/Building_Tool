using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Default Material Palette", menuName = "Polybuilder/ New Material Palette", order = 0)]
public class MaterialPalette : ScriptableObject
{
    [Header("Door")]
    [SerializeField] private Material m_Door;
    [SerializeField] private Material m_DoorFrame;
    [SerializeField] private Material m_DoorHandle;
    [Header("Window")]
    [SerializeField] private Material m_WindowOuterFrame;
    [SerializeField] private Material m_WindowInnerFrame;
    [SerializeField] private Material m_WindowPane;
    [SerializeField] private Material m_WindowShutters;
    [Header("Roof")]
    [SerializeField] private Material m_RoofExterior;
    [SerializeField] private Material m_RoofInterior;
    [Header("Wall")]
    [SerializeField] private Material m_WallExterior;
    [SerializeField] private Material m_WallExteriorCorners;
    [SerializeField] private Material m_WallInterior;
    [SerializeField] private Material m_WallInteriorCorners;

    [SerializeField] private Material m_Pillar;
    [SerializeField] private Material m_Ceiling;
    [SerializeField] private Material m_Floor;

    public Material Door { get { return m_Door; } set { m_Door = value; } }
    public Material DoorFrame { get { return m_DoorFrame; } set { m_DoorFrame = value; } }
    public Material DoorHandle { get { return m_DoorHandle; } set { m_DoorHandle = value; } }
    public Material OuterFrame { get { return m_WindowOuterFrame; } set { m_WindowOuterFrame = value; } }
    public Material InnerFrame { get { return m_WindowInnerFrame; } set { m_WindowInnerFrame = value; } }
    public Material Pane { get { return m_WindowPane; } set { m_WindowPane = value; } }
    public Material Shutters { get { return m_WindowShutters; } set { m_WindowShutters = value; } }
    public Material ExteriorRoof { get { return m_RoofExterior; } set { m_RoofExterior = value; } }
    public Material InteriorRoof { get { return m_RoofInterior; } set { m_RoofInterior = value; } }
    public Material ExteriorWall { get { return m_WallExterior; } set { m_WallExterior = value; } }
    public Material ExteriorWallCorners { get { return m_WallExteriorCorners; } set { m_WallExteriorCorners = value; } }
    public Material InteriorWallCorners { get { return m_WallInteriorCorners; } set { m_WallInteriorCorners = value; } }
    public Material InteriorWall { get { return m_WallInterior; } set { m_WallInterior = value; } }
    public Material Pillar { get { return m_Pillar; } set { m_Pillar = value; } }
    public Material Ceiling { get { return m_Ceiling; } set { m_Ceiling = value; } }
    public Material Floor { get { return m_Floor; } set { m_Floor = value; } }

    /// <summary>
    /// Set default values
    /// </summary>
    /// <returns></returns>
    public MaterialPalette Initialize()
    {
        Material def = BuiltinMaterials.defaultMaterial;

        m_Door = def;
        m_DoorFrame = def;
        m_DoorHandle = def;
        m_WindowOuterFrame = def;
        m_WindowInnerFrame = def;
        m_WindowPane = def;
        m_WindowShutters = def;
        m_RoofExterior = def;
        m_RoofInterior = def;
        m_WallExterior = def;
        m_WallExteriorCorners = def;
        m_WallInterior = def;
        m_Pillar = def;
        m_Ceiling = def;
        m_Floor = def;

        return this;
    }

}
