using OnlyInvalid.ProcGenBuilding.Common;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Add Content Menu", menuName = "Menu/New Add Content Menu")]
public class AddContentMenu : BaseMenu
{
    [SerializeField] List<ContentScriptableObject> m_Content;

    [SerializeField, HideInInspector] public UnityEvent<Polygon3D> OnCreatedContent = new UnityEvent<Polygon3D>();

    public override void CreateMenu()
    {
        base.CreateMenu();

        foreach (var content in m_Content)
        {
            m_DropdownMenu.AddItem(content.name, false, () => AddContent(content));
        }
    }

    private void AddContent(ContentScriptableObject content)
    {
        Section section = Selection.activeGameObject.GetComponent<Section>();
        Polygon3D polygon3D = content.CreateContent();
        polygon3D.transform.SetParent(section.transform, true);
        OnCreatedContent.Invoke(polygon3D);
    }
}