using OnlyInvalid.ProcGenBuilding.Polygon3D;
using OnlyInvalid.ProcGenBuilding.Window;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.ProBuilder;

[CreateAssetMenu(fileName = "Outer Frame", menuName = "Frame/New Outer Frame")]
public class OuterFrameScriptableObject : ContentScriptableObject
{
    [SerializeField] OuterFrameData m_OuterFrameData;
    [SerializeField] FrameScriptableObject m_InnerFrame;

    public override Polygon3D Create3DPolygon()
    {
        ProBuilderMesh frameMesh = ProBuilderMesh.Create();
        OuterFrame frame = frameMesh.gameObject.AddComponent<OuterFrame>();
        frame.name = name;
        m_OuterFrameData.IsDirty = true;
        m_OuterFrameData.IsHoleDirty = true;
        frame.Initialize(m_OuterFrameData);
        return frame;
    }

}
