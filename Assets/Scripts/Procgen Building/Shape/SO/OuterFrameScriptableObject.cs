using OnlyInvalid.ProcGenBuilding.Polygon3D;
using OnlyInvalid.ProcGenBuilding.Window;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.ProBuilder;

[CreateAssetMenu(fileName = "Outer Frame", menuName = "Frame/New Outer Frame")]
public class OuterFrameScriptableObject : BaseFrameScriptableObject
{
    [SerializeField] OuterFrameData m_OuterFrameData;
    [SerializeField] ContentScriptableObject m_Content;

    public override Polygon3D CreateContent()
    {
        ProBuilderMesh frameMesh = ProBuilderMesh.Create();
        OuterFrame frame = frameMesh.gameObject.AddComponent<OuterFrame>();
        frame.name = name;
        m_OuterFrameData.IsDirty = true;
        m_OuterFrameData.IsHoleDirty = true;
        m_OuterFrameData.InnerPolygon3D = m_Content.CreateContent();
        m_OuterFrameData.InnerPolygon3D.transform.SetParent(frame.transform, true);
        frame.Initialize(m_OuterFrameData);
        return frame;
    }

}
