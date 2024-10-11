using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//public struct TransformData
//{
//    public Vector3 position, eulerAngle, scale;

//    public TransformData(Vector3 position, Vector3 eulerAngle, Vector3 scale)
//    {
//        this.position = position;
//        this.eulerAngle = eulerAngle;
//        this.scale = scale;
//    }
//}

[System.Serializable]
public struct WallPoints
{
    public Vector3 Start;
    public Vector3 End;

    public WallPoints(Vector3 start, Vector3 end)
    {
        Start = start;
        End = end;
    }
}
[System.Serializable]
public struct LerpPoint
{
    public Vector3 Start, End;
    public float T;

    public LerpPoint(Vector3 start, Vector3 end, float t)
    {
        this.Start = start;
        this.End = end;
        this.T = t;
    }

    public LerpPoint(Vector3 start)
    {
        this.Start = start;
        this.End = Vector3.zero;
        this.T = 0;
    }
}
[System.Serializable]
public struct RangeValues<T>
{
    public T lower, upper;

    public RangeValues(T lower, T upper)
    {
        this.lower = lower;
        this.upper = upper;
    }
}

[System.Serializable]
public struct SliderDisplayData<T>
{
    public string label;
    public RangeValues<T> range;
    public bool showInputField;
    public SliderDirection direction;
    public bool inverted;

    public SliderDisplayData(string label, RangeValues<T> range, bool showInputField, SliderDirection direction, bool inverted)
    {
        this.label = label;
        this.range = range;
        this.showInputField = showInputField;
        this.direction = direction;
        this.inverted = inverted;
    }
    public SliderDisplayData(SliderDisplayData<T> data)
    {
        this.label = data.label;
        this.range = data.range;
        this.showInputField = data.showInputField;
        this.direction = data.direction;
        this.inverted = data.inverted;    
    }
}

