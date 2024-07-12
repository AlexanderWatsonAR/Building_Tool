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
        ProBuilderMesh frameMesh = ProBuilderMesh.Create();
        GridFrame gridFrame = frameMesh.gameObject.AddComponent<GridFrame>();
        gridFrame.name = name;
        m_GridFrameData.IsDirty = true;
        m_GridFrameData.IsHoleDirty = true;
        gridFrame.Initialize(m_GridFrameData);
        return gridFrame;
    }
}
