using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerDisplayData : BaseDisplayData
{
    [SerializeField] SliderDisplayData<int> m_Sides;

    public SliderDisplayData<int> Sides { get => m_Sides; set => m_Sides = value; }

    public override BaseDisplayData Initialize()
    {
        m_Sides = new SliderDisplayData<int>()
        {
            label = "Sides",
            range = new RangeValues<int>(2, 18),
            direction = UnityEngine.UIElements.SliderDirection.Horizontal,
            inverted = false,
            showInputField = true
        };

        return this;
    }
}
