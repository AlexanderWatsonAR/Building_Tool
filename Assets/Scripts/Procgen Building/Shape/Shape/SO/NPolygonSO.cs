using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[CreateAssetMenu(fileName = "Polygon", menuName ="Shape/Calculated Shape/New Polygon")]
public class NPolygonSO : ShapeSO
{
    public NPolygon NPolygon => m_Shape as NPolygon;

    public override void Initialize()
    {
        m_Shape = new NPolygon();
    }

}
