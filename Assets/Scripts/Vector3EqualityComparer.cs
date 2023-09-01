using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Vector3EqualityComparer : IEqualityComparer<Vector3>
{
    private float m_Tolerance;

    public Vector3EqualityComparer(float tolerance = 0.001f)
    {
        m_Tolerance = tolerance;
    }
    public bool Equals(Vector3 v1, Vector3 v2)
    {
        bool result =  Vector3.Distance(v1, v2) < m_Tolerance;
        //Debug.Log($"Comparing {v1} and {v2}. Result: {result}");
        return result;
    }
    public int GetHashCode(Vector3 vector)
    {
        return vector.GetHashCode();
    }

}