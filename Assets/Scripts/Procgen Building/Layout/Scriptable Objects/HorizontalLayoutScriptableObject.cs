using OnlyInvalid.ProcGenBuilding.Layout;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

[CreateAssetMenu(fileName ="Horizontal Layout", menuName = "Layout/New Horizontal Layout ")]
public class HorizontalLayoutScriptableObject : LayoutScriptableObject
{
    public override Polygon3D CreateContent()
    {
        ProBuilderMesh layoutMesh = ProBuilderMesh.Create();
        layoutMesh.name = name;
        HorizontalLayoutGroup layout = layoutMesh.gameObject.AddComponent<HorizontalLayoutGroup>();
        m_LayoutGroupData.IsDirty = true;

        foreach(ContentScriptableObject content in m_Contents)
        {
            Polygon3D polygon = content.CreateContent();
            polygon.transform.SetParent(layout.transform, true);
            m_LayoutGroupData.Polygons.Add(polygon);
        }

        layout.Initialize(m_LayoutGroupData);

        return layout;

    }
}
