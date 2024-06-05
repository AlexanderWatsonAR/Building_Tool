using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ElementPanel : EditorWindow
{
    [SerializeField] VisualTreeAsset m_Tree;

    public static void ShowWindow()
    {
        GetWindow(typeof(ElementPanel), false, "Element Panel");
    }

    public void CreateGUI()
    {
        rootVisualElement.Add(m_Tree.CloneTree());
    }

}
