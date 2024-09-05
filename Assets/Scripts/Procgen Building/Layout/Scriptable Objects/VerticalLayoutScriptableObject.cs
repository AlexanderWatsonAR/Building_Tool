using OnlyInvalid.ProcGenBuilding.Layout;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using System.Collections;
using UnityEngine;
using UnityEngine.ProBuilder;

[CreateAssetMenu(fileName = "Vertical Layout", menuName = "Layout/New Vertical Layout ")]
public class VerticalLayoutScriptableObject : LayoutScriptableObject
{
    public override Polygon3D CreateContent()
    {
        VerticalLayoutGroup layout = ProBuilderMesh.Create().gameObject.AddComponent<VerticalLayoutGroup>();
        layout.name = name;

        LayoutGroupData data = new LayoutGroupData();
        data.IsDirty = true;

        foreach (ContentScriptableObject content in m_Contents)
        {
            Polygon3D polygon = content.CreateContent();
            polygon.transform.SetParent(layout.transform, true);
            data.Polygons.Add(polygon);
        }

        layout.Initialize(data);

        return layout;

    }
}