using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class etes : MonoBehaviour
{
    private PolyPath polytool;


    public void DoTest()
    {
        polytool = GetComponent<PolyPath>();

    }

    private void OnDrawGizmos()
    {
        //polytool = GetComponent<PolyPath>();

        //if (polytool.IsDescribableInOneLine(out Vector3[] oneLine))
        //{
        //    foreach(Vector3 line in oneLine)
        //    {
        //        Handles.DoPositionHandle(line, Quaternion.identity);
        //    }
        //}
        
        //if(polytool.IsMShaped(out int[] convexPoints))
        //{
        //    Vector3[] controlPointsArray = polytool.LocalPositions.ToArray();

        //    GUIStyle style = new GUIStyle();
        //    style.fontSize = 18;
        //    for (int i = 0; i < controlPointsArray.Length; i++)
        //    {
        //        for(int j = 0; j < convexPoints.Length; j++)
        //        {
        //            if (i == convexPoints[j])
        //            {
        //                style.normal.textColor = Color.red;
        //                break;
        //            }
        //            else
        //            {
        //                style.normal.textColor = Color.green;
        //            }
        //        }

        //        Handles.Label(polytool.LocalPositions[i], i.ToString(), style);
        //    }

        //}
    }

}
