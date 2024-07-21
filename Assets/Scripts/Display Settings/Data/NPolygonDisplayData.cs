using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPolygonDisplayData : ScriptableObject
{
    [SerializeField] SliderDisplayData<int> m_Sides;

    public SliderDisplayData<int> Sides { get => m_Sides; set => m_Sides = value; }

    public NPolygonDisplayData Initialize()
    {
        m_Sides = new SliderDisplayData<int>()
        {
            label = "Sides",
            range = new RangeValues<int>(3, 18),
            direction = UnityEngine.UIElements.SliderDirection.Horizontal,
            inverted = false,
            showInputField = true,
        };

        return this;
    }
}
