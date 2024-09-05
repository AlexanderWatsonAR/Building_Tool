using OnlyInvalid.ProcGenBuilding.Polygon3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

[CreateAssetMenu(fileName = "Grid Frame", menuName = "Frame/New Grid Frame")]
public class GridFrameScriptableObject : BaseFrameScriptableObject
{
    [SerializeField] GridFrameData m_GridFrameData;

    public override Polygon3D CreateContent()
    {
        GridFrame gridFrame = ProBuilderMesh.Create().gameObject.AddComponent<GridFrame>();
        gridFrame.name = name;

        GridFrameData data = new GridFrameData(m_GridFrameData);
        data.IsDirty = true;
        data.IsHoleDirty = true;

        gridFrame.Initialize(data);
        return gridFrame;
    }
}
