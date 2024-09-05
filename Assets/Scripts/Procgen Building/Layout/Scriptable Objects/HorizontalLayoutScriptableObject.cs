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
        HorizontalLayoutGroup layout = ProBuilderMesh.Create().gameObject.AddComponent<HorizontalLayoutGroup>();
        layout.name = name;

        LayoutGroupData data = new LayoutGroupData();
        data.IsDirty = true;

        foreach(ContentScriptableObject content in m_Contents)
        {
            Polygon3D polygon = content.CreateContent();
            polygon.transform.SetParent(layout.transform, true);
            data.Polygons.Add(polygon);
        }

        layout.Initialize(data);

        return layout;

    }
}
