using OnlyInvalid.ProcGenBuilding.Common;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "Content Menu", menuName = "Menu/New Content Menu")]
public class ContentMenu : BaseMenu
{
    [SerializeField] List<ContentScriptableObject> m_Content;

    public override void CreateMenu()
    {
        base.CreateMenu();

        foreach (var content in m_Content)
        {
            m_DropdownMenu.AddItem(content.name, false, () => AddToSection(content));
        }
    }

    private void AddToSection(ContentScriptableObject content)
    {
        Section section = Selection.activeGameObject.GetComponent<Section>();
        Polygon3D polygon3D = content.Create3DPolygon();
        polygon3D.transform.SetParent(section.transform, true);
        OpeningData windowOpening = new OpeningData(new NPolygon(4), polygon3D);
        section.SectionData.AddOpening(windowOpening);
    }
}
