using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System.Net;

[CustomPropertyDrawer(typeof(StoreyData))]
public class StoreyDataDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        StoreyData storeyData = data.GetUnderlyingValue() as StoreyData;
        IBuildable buildable = data.serializedObject.targetObject as IBuildable; // what if the gameobject has multiple buildable components?

        // Create property container element.
        var container = new VisualElement();

        Foldout nameFoldout = new Foldout()
        {
            text = storeyData.Name
        };

        SerializedProperty activeStoreyElements = data.FindPropertyRelative("m_ActiveElements");
        EnumFlagsField activeStoreyElementsField = new EnumFlagsField("Active Elements", activeStoreyElements.GetEnumValue<StoreyElement>());
        activeStoreyElementsField.BindProperty(activeStoreyElements);

        #region Wall
        SerializedProperty wallData = data.FindPropertyRelative("m_Wall");
        SerializedProperty wallHeight = wallData.FindPropertyRelative("m_Height");
        SerializedProperty wallDepth = wallData.FindPropertyRelative("m_Depth");

        Foldout wallFoldout = new Foldout()
        {
            text = "Walls"
        };

        PropertyField wallHeightField = new PropertyField(wallHeight);
        wallHeightField.BindProperty(wallHeight);
        wallHeightField.RegisterValueChangeCallback(evt => 
        {
            if (storeyData.Walls == null)
                return;

            for(int i = 0; i < storeyData.Walls.Length; i++)
            {
                storeyData.Walls[i].Height = evt.changedProperty.floatValue;
            }

            buildable.Build();
        });

        PropertyField wallDepthField = new PropertyField(wallDepth);
        wallDepthField.BindProperty(wallDepth);
        wallDepthField.RegisterValueChangeCallback(evt =>
        {
            if (storeyData.Walls == null)
                return;

            for (int i = 0; i < storeyData.Walls.Length; i++)
            {
                storeyData.Walls[i].Depth = evt.changedProperty.floatValue;
            }

            buildable.Build();
        });


        wallFoldout.Add(wallHeightField);
        wallFoldout.Add(wallDepthField);
        #endregion

        #region Corner
        SerializedProperty cornerData = data.FindPropertyRelative("m_Corner");
        SerializedProperty cornerType = cornerData.FindPropertyRelative("m_Type");
        SerializedProperty cornerSides = cornerData.FindPropertyRelative("m_Sides");

        Foldout cornerFoldout = new Foldout() { text = "Corner" };

        EnumField cornerTypeField = new EnumField(cornerType.GetEnumValue<CornerType>());
        PropertyField cornerSidesField = new PropertyField(cornerSides);

        cornerTypeField.BindProperty(cornerType);
        cornerSidesField.BindProperty(cornerSides);

        cornerFoldout.Add(cornerTypeField);
        cornerFoldout.Add(cornerSidesField);

        #endregion

        #region Pillar
        SerializedProperty pillarData = data.FindPropertyRelative("m_Pillar");
        PropertyField pillarField = new PropertyField(pillarData);
        pillarField.BindProperty(pillarData);
        #endregion

        #region Floor
        SerializedProperty floorData = data.FindPropertyRelative("m_Floor");
        PropertyField floorField = new PropertyField(floorData);
        floorField.BindProperty(floorData);
        #endregion

        #region Register Value Changed Callback

        cornerTypeField.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue != null)
            {
                cornerSidesField.SetEnabled((CornerType)evt.newValue == CornerType.Round);
            }
        });

        activeStoreyElementsField.RegisterValueChangedCallback(evt =>
        {
            wallFoldout.SetEnabled(activeStoreyElements.GetEnumValue<StoreyElement>().IsElementActive(StoreyElement.Walls));
            cornerFoldout.SetEnabled(activeStoreyElements.GetEnumValue<StoreyElement>().IsElementActive(StoreyElement.Walls));
            pillarField.SetEnabled(activeStoreyElements.GetEnumValue<StoreyElement>().IsElementActive(StoreyElement.Pillars));
            floorField.SetEnabled(activeStoreyElements.GetEnumValue<StoreyElement>().IsElementActive(StoreyElement.Floor));
        });
        #endregion

        #region Add Fields to Container
        // Add fields to the container.
        container.Add(nameFoldout);
        nameFoldout.Add(activeStoreyElementsField);
        nameFoldout.Add(wallFoldout);
        nameFoldout.Add(cornerFoldout);
        nameFoldout.Add(pillarField);
        nameFoldout.Add(floorField);
        #endregion


        return container;
    }


    //public override void OnGUI(Rect container, SerializedProperty data, GUIContent label)
    //{
    //    SerializedProperty activeElements = data.FindPropertyRelative("m_ActiveElements");

    //    string storeyName = data.FindPropertyRelative("m_Name").stringValue;

    //    #region Wall
    //    SerializedProperty wallData = data.FindPropertyRelative("m_Wall");
    //    #endregion

    //    #region Corner
    //    SerializedProperty cornerData = data.FindPropertyRelative("m_Corner");
    //    SerializedProperty cornerType = cornerData.FindPropertyRelative("m_Type");
    //    SerializedProperty cornerSides = cornerData.FindPropertyRelative("m_Sides");
    //    #endregion

    //    #region Floor
    //    SerializedProperty floorData = data.FindPropertyRelative("m_Floor");
    //    #endregion

    //    #region Pillar
    //    SerializedProperty pillarData = data.FindPropertyRelative("m_Pillar");
    //    #endregion

    //    EditorGUI.BeginProperty(container, new GUIContent("Label"), data);

    //    //m_StoreyFoldout = EditorGUI.BeginFoldoutHeaderGroup(container, m_StoreyFoldout, storeyName);


    //    //EditorGUI.PropertyField(new Rect(new Vector2(container.x, container.y + container.height), container.size), activeElements);

    //    StoreyElement elements = activeElements.GetEnumValue<StoreyElement>();

    //    EditorGUI.BeginDisabledGroup(!elements.IsElementActive(StoreyElement.Walls));

    //    // EditorGUI.PropertyField

    //    EditorGUI.EndDisabledGroup();

    //    EditorGUI.BeginDisabledGroup(!elements.IsElementActive(StoreyElement.Floor));

    //    EditorGUI.EndDisabledGroup();

    //    EditorGUI.BeginDisabledGroup(!elements.IsElementActive(StoreyElement.Pillars));

    //    EditorGUI.EndDisabledGroup();

    //    EditorGUI.EndFoldoutHeaderGroup();

    //    EditorGUI.EndProperty();
    //}
}
