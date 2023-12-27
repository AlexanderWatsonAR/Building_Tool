using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Unity.VisualScripting;

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

        PropertyField wallDepthField = new PropertyField(wallDepth);
        wallDepthField.BindProperty(wallDepth);

        wallFoldout.Add(wallHeightField);
        wallFoldout.Add(wallDepthField);
        #endregion

        #region Corner
        SerializedProperty cornerData = data.FindPropertyRelative("m_Corner");
        SerializedProperty cornerType = cornerData.FindPropertyRelative("m_Type");
        SerializedProperty cornerSides = cornerData.FindPropertyRelative("m_Sides");

        Foldout cornerFoldout = new Foldout() { text = "Corner" };
        EnumField cornerTypeField = new EnumField(cornerType.GetEnumValue<CornerType>());
        cornerTypeField.BindProperty(cornerType);

        PropertyField cornerSidesField = new PropertyField(cornerSides);
        cornerSidesField.BindProperty(cornerSides);

        wallFoldout.Add(cornerFoldout);
        cornerFoldout.Add(cornerTypeField);
        cornerFoldout.Add(cornerSidesField);

        if (cornerSides.GetEnumValue<CornerType>() != CornerType.Round)
        {
            cornerSidesField.SetEnabled(false);
        }

        #endregion

        #region Pillar
        SerializedProperty pillarData = data.FindPropertyRelative("m_Pillar");
        Foldout pillarFoldout = new Foldout() { text = "Pillars"};

        SerializedProperty pillarHeight = pillarData.FindPropertyRelative("m_Height");
        SerializedProperty pillarWidth = pillarData.FindPropertyRelative("m_Width");
        SerializedProperty pillarDepth = pillarData.FindPropertyRelative("m_Depth");
        SerializedProperty pillarSides = pillarData.FindPropertyRelative("m_Sides");
        SerializedProperty pillarIsSmooth = pillarData.FindPropertyRelative("m_IsSmooth");

        PropertyField pillarHeightField = new PropertyField(pillarHeight);
        pillarHeightField.BindProperty(pillarHeight);

        PropertyField pillarWidthField = new PropertyField(pillarWidth);
        pillarWidthField.BindProperty(pillarWidth);

        PropertyField pillarDepthField = new PropertyField(pillarDepth);
        pillarDepthField.BindProperty(pillarDepth);

        PropertyField pillarSidesField = new PropertyField(pillarSides);
        pillarSidesField.BindProperty(pillarSides);

        PropertyField pillarIsSmoothField = new PropertyField(pillarIsSmooth);
        pillarIsSmoothField.BindProperty(pillarIsSmooth);

        pillarFoldout.Add(pillarHeightField);
        pillarFoldout.Add(pillarWidthField);
        pillarFoldout.Add(pillarDepthField);
        pillarFoldout.Add(pillarSidesField);
        pillarFoldout.Add(pillarIsSmoothField);

        #endregion

        #region Floor
        SerializedProperty floorData = data.FindPropertyRelative("m_Floor");
        Foldout floorFoldout = new Foldout() { text = "Floor" };

        SerializedProperty floorHeight = floorData.FindPropertyRelative("m_Height");
        PropertyField floorHeightField = new PropertyField(floorHeight);
        floorHeightField.BindProperty(floorHeight);

        floorFoldout.Add(floorHeightField);
        #endregion

        #region Register Value Change Callback

        activeStoreyElementsField.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue == null)
                return;

            wallFoldout.SetEnabled(activeStoreyElements.GetEnumValue<StoreyElement>().IsElementActive(StoreyElement.Walls));
            pillarFoldout.SetEnabled(activeStoreyElements.GetEnumValue<StoreyElement>().IsElementActive(StoreyElement.Pillars));
            floorFoldout.SetEnabled(activeStoreyElements.GetEnumValue<StoreyElement>().IsElementActive(StoreyElement.Floor));

            buildable.Build();
        });

        wallHeightField.RegisterValueChangeCallback(evt =>
        {
            if (storeyData.Walls == null)
                return;

            for (int i = 0; i < storeyData.Walls.Length; i++)
            {
                storeyData.Walls[i].Height = evt.changedProperty.floatValue;
            }

            buildable.Build();
        });

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

        cornerTypeField.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue == null)
                return;

            cornerSidesField.SetEnabled((CornerType)evt.newValue == CornerType.Round);
            buildable.Build();
        });

        cornerSidesField.RegisterValueChangeCallback(evt => buildable.Build());

        pillarHeightField.RegisterValueChangeCallback(evt =>
        {
            if (storeyData.Pillars == null)
                return;

            for (int i = 0; i < storeyData.Pillars.Length; i++)
            {
                storeyData.Pillars[i].Height = evt.changedProperty.floatValue;
            }

            buildable.Build();
        });

        pillarDepthField.RegisterValueChangeCallback(evt =>
        {
            if (storeyData.Pillars == null)
                return;

            for (int i = 0; i < storeyData.Pillars.Length; i++)
            {
                storeyData.Pillars[i].Depth = evt.changedProperty.floatValue;
            }

            buildable.Build();
        });

        pillarWidthField.RegisterValueChangeCallback(evt =>
        {
            if (storeyData.Pillars == null)
                return;

            for (int i = 0; i < storeyData.Pillars.Length; i++)
            {
                storeyData.Pillars[i].Width = evt.changedProperty.floatValue;
            }

            buildable.Build();
        });

        pillarSidesField.RegisterValueChangeCallback(evt =>
        {
            if (storeyData.Pillars == null)
                return;

            for (int i = 0; i < storeyData.Pillars.Length; i++)
            {
                storeyData.Pillars[i].Sides = evt.changedProperty.intValue;
            }

            buildable.Build();
        });

        pillarIsSmoothField.RegisterValueChangeCallback(evt =>
        {
            if (storeyData.Pillars == null)
                return;

            if (evt.changedProperty.GetUnderlyingValue() == null)
                return;

            bool rebuild = false;

            for (int i = 0; i < storeyData.Pillars.Length; i++)
            {
                if (storeyData.Pillars[i].IsSmooth != evt.changedProperty.boolValue)
                    rebuild = true;

                storeyData.Pillars[i].IsSmooth = evt.changedProperty.boolValue;
            }

            if(rebuild)
                buildable.Build();
        });

        floorHeightField.RegisterValueChangeCallback(evt =>
        {
            if (storeyData.FloorData == null)
                return;

            buildable.Build();
        });

        #endregion

        #region Add Fields to Container
        container.Add(nameFoldout);
        nameFoldout.Add(activeStoreyElementsField);
        nameFoldout.Add(wallFoldout);
        nameFoldout.Add(pillarFoldout);
        nameFoldout.Add(floorFoldout);
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
