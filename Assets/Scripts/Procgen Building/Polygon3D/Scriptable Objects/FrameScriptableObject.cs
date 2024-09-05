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
        Frame frame = ProBuilderMesh.Create().gameObject.AddComponent<Frame>();
        frame.name = name;

        FrameData data = new FrameData(m_FrameData);
        data.IsDirty = true;
        data.IsHoleDirty = true;

        frame.Initialize(data);
        return frame;
    }

}
