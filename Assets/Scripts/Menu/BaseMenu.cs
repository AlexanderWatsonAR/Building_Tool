using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseMenu : ScriptableObject
{
    protected GenericDropdownMenu m_DropdownMenu;
    protected VisualElement m_Root;

    public GenericDropdownMenu Menu => m_DropdownMenu;
    public VisualElement Root => m_Root; 

    public virtual void Initialize(VisualElement root)
    {
        m_Root = root;
    }

    public virtual void CreateMenu()
    {
        m_DropdownMenu = new GenericDropdownMenu();
        m_DropdownMenu.contentContainer.Add(new Label(name) { style = { alignSelf = Align.Center, unityFontStyleAndWeight = FontStyle.Bold } }); // Menu Title
        m_DropdownMenu.AddSeparator("");
    }
}
