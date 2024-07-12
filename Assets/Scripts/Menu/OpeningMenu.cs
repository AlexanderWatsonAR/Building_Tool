using OnlyInvalid.ProcGenBuilding.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Opening Menu", menuName = "Menu/New Opening Menu")]
public class OpeningMenu : BaseMenu
{
    [SerializeField] List<OpeningScriptableObject> m_Openings;


    public override void CreateMenu()
    {
        base.CreateMenu();

        foreach (var opening in m_Openings)
        {
            m_DropdownMenu.AddItem(opening.name, false, () => { AddToSection(opening); }); // TODO add opening to section
        }
    }
    public void AddToSection(OpeningScriptableObject opening)
    {
        Section section = Selection.activeGameObject.GetComponent<Section>();
        section.SectionData.AddOpening(opening.Opening);

    }

}