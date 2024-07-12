using OnlyInvalid.ProcGenBuilding.Common;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using UnityEngine;
using UnityEngine.ProBuilder;

[CreateAssetMenu(fileName = "Frame", menuName = "Frame/New Frame")]
public class FrameScriptableObject : BaseFrameScriptableObject
{
    [SerializeField] FrameData m_FrameData;

    public override Polygon3D CreateContent()
    {
        ProBuilderMesh frameMesh = ProBuilderMesh.Create();
        Frame frame = frameMesh.gameObject.AddComponent<Frame>();
        frame.name = name;
        m_FrameData.IsDirty = true;
        m_FrameData.IsHoleDirty = true;
        frame.Initialize(m_FrameData);
        return frame;
    }

}
