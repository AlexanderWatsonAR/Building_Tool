using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameDisplayData : ScriptableObject
{
    [SerializeField] SliderDisplayData<float> m_Depth, m_Scale;

    public SliderDisplayData<float> Depth { get => m_Depth; set => m_Depth = value; }
    public SliderDisplayData<float> Scale { get => m_Scale; set => m_Scale = value; }

    public FrameDisplayData Initialize()
    {
        m_Depth = new SliderDisplayData<float>()
        {
            label = "Depth",
            range = new RangeValues<float>(0, 1),
            direction = UnityEngine.UIElements.SliderDirection.Horizontal,
            inverted = false,
            showInputField = true
        };
        m_Scale = new SliderDisplayData<float>()
        {
            label = "Scale",
            range = new RangeValues<float>(0, 1),
            direction = UnityEngine.UIElements.SliderDirection.Horizontal,
            inverted = false,
            showInputField = true
        };

        return this;
    }
}