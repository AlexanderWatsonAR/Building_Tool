using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class RoundedSquareDisplayData : BaseDisplayData
{
    [SerializeField] SliderDisplayData<int> m_Sides;
    [SerializeField] SliderDisplayData<float> m_CurveSize;

    public SliderDisplayData<int> Sides { get => m_Sides; set => m_Sides = value; }
    public SliderDisplayData<float> CurveSize { get => m_CurveSize; set => m_CurveSize = value; }

    public override BaseDisplayData Initialize()
    {
        m_Sides = new SliderDisplayData<int>()
        {
            label = "Sides",
            range = new RangeValues<int>(1, 25),
            direction = UnityEngine.UIElements.SliderDirection.Horizontal,
            inverted = false,
            showInputField = true
        };
        m_CurveSize = new SliderDisplayData<float>()
        {
            label = "Curve Size",
            range = new RangeValues<float>(0, 0.5f),
            direction = UnityEngine.UIElements.SliderDirection.Horizontal,
            inverted = false,
            showInputField = true
        };
        return this;
    }
}