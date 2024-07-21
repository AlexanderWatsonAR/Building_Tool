using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchDisplayData : ScriptableObject
{
    [SerializeField] SliderDisplayData<int> m_Sides;
    [SerializeField] string m_ArchHeightLabel, m_BaseHeightLabel;

    public SliderDisplayData<int> Sides { get => m_Sides; set => m_Sides = value; }
    public string ArchHeightLabel => m_ArchHeightLabel;
    public string BaseHeightLabel => m_BaseHeightLabel;

    public ArchDisplayData Initialize()
    {
        m_Sides = new SliderDisplayData<int>()
        {
            label = "Sides",
            range = new RangeValues<int>(3, 18),
            direction = UnityEngine.UIElements.SliderDirection.Horizontal,
            inverted = false,
            showInputField = true
        };
        m_ArchHeightLabel = "Arch Height";
        m_BaseHeightLabel = "Base Height";
        return this;
    }

}