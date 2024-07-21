using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridFrameDisplayData : ScriptableObject
{
    [SerializeField] SliderDisplayData<int> m_Columns, m_Rows;
    [SerializeField] SliderDisplayData<float> m_Depth, m_Scale;

    public SliderDisplayData<int> Columns { get => m_Columns; set => m_Columns = value; }
    public SliderDisplayData<int> Rows { get => m_Rows; set => m_Rows = value; }
    public SliderDisplayData<float> Depth { get => m_Depth; set => m_Depth = value; }
    public SliderDisplayData<float> Scale { get => m_Scale; set => m_Scale = value; }

    public GridFrameDisplayData Initialize()
    {
        m_Columns = new SliderDisplayData<int>()
        {
            label = "Columns",
            range = new RangeValues<int>(1, 5),
            direction = UnityEngine.UIElements.SliderDirection.Horizontal,
            inverted = false,
            showInputField = true
        };
        m_Rows = new SliderDisplayData<int>()
        {
            label = "Rows",
            range = new RangeValues<int>(1, 5),
            direction = UnityEngine.UIElements.SliderDirection.Horizontal,
            inverted = false,
            showInputField = true
        };
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