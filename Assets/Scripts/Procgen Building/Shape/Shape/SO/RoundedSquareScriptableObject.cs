using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rounded Square", menuName = "Shape/Calculated Shape/New Rounded Square")]
public class RoundedSquareScriptableObject : ShapeScriptableObject
{
    public RoundedSquare RoundedSquare => m_Shape as RoundedSquare;

    public override void Initialize()
    {
        m_Shape = new RoundedSquare();
    }
}
