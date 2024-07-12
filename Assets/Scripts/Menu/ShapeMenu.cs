using OnlyInvalid.ProcGenBuilding.Common;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Shape Menu", menuName = "Menu/New Shape Menu")]
public class ShapeMenu : BaseMenu
{
    [SerializeField] List<ShapeScriptableObject> m_Shapes;

    public override void CreateMenu()
    {
        base.CreateMenu();

        foreach(var shape in m_Shapes)
        {
            m_DropdownMenu.AddItem(shape.name, false, () => { AddToSection(shape); });
        }
    }

    private void AddToSection(ShapeScriptableObject shape)
    {
        Section section = Selection.activeGameObject.GetComponent<Section>();
        section.SectionData.AddOpening(new OpeningData(shape.Shape));
    }

}
