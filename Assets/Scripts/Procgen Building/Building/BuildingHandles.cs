using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class BuildingHandles
{
    // This is just a copy of the probuilder cap func. 
    // Create your own distinct handle cap function.
    public static void TestDotHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
    {
        switch (eventType)
        {
            case EventType.MouseMove:
            case EventType.Layout:
                HandleUtility.AddControl(controlID, HandleUtility.DistanceToRectangle(position, rotation, size));
                break;
            case EventType.Repaint:
                {
                    Vector3 vector = ((Camera.current == null) ? Vector3.right : Camera.current.transform.right) * size;
                    Vector3 vector2 = ((Camera.current == null) ? Vector3.up : Camera.current.transform.up) * size;
                    Color c = Color.white;
                    GL.Begin(7);
                    GL.Color(c);
                    GL.Vertex(position + vector + vector2);
                    GL.Vertex(position + vector - vector2);
                    GL.Vertex(position - vector - vector2);
                    GL.Vertex(position - vector + vector2);
                    GL.End();
                    break;
                }
        }
    }

    public static void SolidCircleHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
    {
        switch (eventType)
        {

            case EventType.MouseMove:
            case EventType.Layout:
                HandleUtility.AddControl(controlID, HandleUtility.DistanceToRectangle(position, rotation, size));
                break;
            case EventType.Repaint:
                {
                    Vector3 normal = rotation * Vector3.forward;
                    Handles.DrawSolidDisc(position, normal, size);
                    break;
                }
        }
    }
}
