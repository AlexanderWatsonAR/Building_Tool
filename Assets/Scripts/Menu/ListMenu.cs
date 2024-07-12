using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "List Menu", menuName = "Menu/New List Menu")]
public class ListMenu : BaseMenu
{
    [SerializeField] List<BaseMenu> m_Menus;

    public override void CreateMenu()
    {
        base.CreateMenu();

        foreach (var menu in m_Menus)
        {
            menu.Initialize(m_Root);
            menu.CreateMenu();
            m_DropdownMenu.AddItem(menu.name, false, () => menu.Menu.DropDown(m_Root.worldBound, m_Root.contentContainer));
        }
    }
}
