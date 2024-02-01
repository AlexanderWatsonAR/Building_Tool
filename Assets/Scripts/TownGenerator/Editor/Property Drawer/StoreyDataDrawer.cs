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
{//
    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        StoreyDataSerializedProperties props = new StoreyDataSerializedProperties(data);
        StoreyData storeyData = data.GetUnderlyingValue() as StoreyData;
        IBuildable buildable = data.serializedObject.targetObject as IBuildable; // what if the gameObject has multiple buildable components?

        VisualElement container = new VisualElement();

        if (storeyData.Corners == null || storeyData.Walls == null || storeyData.Pillars == null)
            return container;

        #region Define Visual Elements
        Foldout nameFoldout = new Foldout() { text = storeyData.Name };

        EnumFlagsField activeStoreyElementsField = new EnumFlagsField("Active Elements", props.ActiveElements.GetEnumValue<StoreyElement>());
        activeStoreyElementsField.BindProperty(props.ActiveElements);
        
        #region Wall
        Foldout wallFoldout = new Foldout() { text = "Walls" };

        PropertyField wallHeightField = new PropertyField(props.Wall.Height);
        wallHeightField.BindProperty(props.Wall.Height);
        wallHeightField.SetEnabled(buildable is Building);

        PropertyField wallDepthField = new PropertyField(props.Wall.Depth);
        wallDepthField.BindProperty(props.Wall.Depth);
        #endregion

        #region Corner

        Foldout cornerFoldout = new Foldout() { text = "Corner" };
        EnumField cornerTypeField = new EnumField(props.Corner.Type.GetEnumValue<CornerType>());
        cornerTypeField.BindProperty(props.Corner.Type);

        PropertyField cornerSidesField = new PropertyField(props.Corner.Sides);
        cornerSidesField.BindProperty(props.Corner.Sides);

        if (props.Corner.Type.GetEnumValue<CornerType>() != CornerType.Round)
        {
            cornerSidesField.SetEnabled(false);
        }
        #endregion

        #region Pillar
        Foldout pillarFoldout = new Foldout() { text = "Pillars"};

        PropertyField pillarHeightField = new PropertyField(props.Pillar.Height);
        pillarHeightField.BindProperty(props.Pillar.Height);

        PropertyField pillarWidthField = new PropertyField(props.Pillar.Width);
        pillarWidthField.BindProperty(props.Pillar.Width);

        PropertyField pillarDepthField = new PropertyField(props.Pillar.Depth);
        pillarDepthField.BindProperty(props.Pillar.Depth);

        PropertyField pillarSidesField = new PropertyField(props.Pillar.Sides);
        pillarSidesField.BindProperty(props.Pillar.Sides);

        PropertyField pillarIsSmoothField = new PropertyField(props.Pillar.IsSmooth);
        pillarIsSmoothField.BindProperty(props.Pillar.IsSmooth);
        #endregion

        #region Floor
        Foldout floorFoldout = new Foldout() { text = "Floor" };

        PropertyField floorHeightField = new PropertyField(props.Floor.Height);
        floorHeightField.BindProperty(props.Floor.Height);

        #endregion

        #endregion

        #region Register Value Change Callback
        activeStoreyElementsField.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue == null)
                return;

            if (evt.previousValue == evt.newValue)
                return;

            bool isWallActive = props.ActiveElements.GetEnumValue<StoreyElement>().IsElementActive(StoreyElement.Walls);
            bool isPillarActive = props.ActiveElements.GetEnumValue<StoreyElement>().IsElementActive(StoreyElement.Pillars);
            bool isFloorActive = props.ActiveElements.GetEnumValue<StoreyElement>().IsElementActive(StoreyElement.Floor);

            if (isWallActive == wallFoldout.enabledSelf &&
               isPillarActive == pillarFoldout.enabledSelf &&
               isFloorActive == floorFoldout.enabledSelf)
                return;

            wallFoldout.SetEnabled(isWallActive);
            pillarFoldout.SetEnabled(isPillarActive);
            floorFoldout.SetEnabled(isFloorActive);

            buildable.Build();
        });

        #region Wall
        wallHeightField.RegisterValueChangeCallback(evt =>
        {
            if (storeyData.Walls == null)
                return;

            bool rebuild = false;

            for (int i = 0; i < storeyData.Walls.Length; i++)
            {
                if(storeyData.Walls[i].Height != evt.changedProperty.floatValue)
                    rebuild = true;

                storeyData.Walls[i].Height = evt.changedProperty.floatValue;
            }

            if(rebuild)
                buildable.Build();

        });
        wallDepthField.RegisterValueChangeCallback(evt =>
        {
            if (storeyData.Walls == null)
                return;

            bool rebuild = false;

            for (int i = 0; i < storeyData.Walls.Length; i++)
            {
                if(storeyData.Walls[i].Depth != evt.changedProperty.floatValue)
                    rebuild = true;

                storeyData.Walls[i].Depth = evt.changedProperty.floatValue;
            }

            if(rebuild)
                buildable.Build();
        });
        #endregion

        #region Corner
        cornerTypeField.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue == null)
                return;

            if (evt.previousValue == evt.newValue)
                return;

            cornerSidesField.SetEnabled((CornerType)evt.newValue == CornerType.Round);
            buildable.Build();
        });
        cornerSidesField.RegisterValueChangeCallback(evt =>
        {
            if (evt == null)
                return;

            bool rebuild = false;

            for(int i = 0; i < storeyData.Corners.Length; i++)
            {
                if (storeyData.Corners[i].Sides != evt.changedProperty.intValue)
                    rebuild = true;

                storeyData.Corners[i].Sides = evt.changedProperty.intValue;
            }

            if(rebuild)
                buildable.Build();
        });
        #endregion

        #region Pillar
        pillarHeightField.RegisterValueChangeCallback(evt =>
        {
            if (storeyData.Pillars == null)
                return;

            bool rebuild = false;

            for (int i = 0; i < storeyData.Pillars.Length; i++)
            {
                if (storeyData.Pillars[i].Height != evt.changedProperty.floatValue)
                    rebuild = true;

                storeyData.Pillars[i].Height = evt.changedProperty.floatValue;
            }

            if(rebuild)
                buildable.Build();
        });
        pillarDepthField.RegisterValueChangeCallback(evt =>
        {
            if (storeyData.Pillars == null)
                return;

            bool rebuild = false;

            for (int i = 0; i < storeyData.Pillars.Length; i++)
            {
                if (storeyData.Pillars[i].Depth != evt.changedProperty.floatValue)
                    rebuild = true;

                storeyData.Pillars[i].Depth = evt.changedProperty.floatValue;
            }

            if(rebuild)
                buildable.Build();
        });
        pillarWidthField.RegisterValueChangeCallback(evt =>
        {
            if (storeyData.Pillars == null)
                return;

            bool rebuild = false;

            for (int i = 0; i < storeyData.Pillars.Length; i++)
            {
                if (storeyData.Pillars[i].Width != evt.changedProperty.floatValue)
                    rebuild = true;

                storeyData.Pillars[i].Width = evt.changedProperty.floatValue;
            }

            if(rebuild)
                buildable.Build();
        });
        pillarSidesField.RegisterValueChangeCallback(evt =>
        {
            if (storeyData.Pillars == null)
                return;

            bool rebuild = false;

            for (int i = 0; i < storeyData.Pillars.Length; i++)
            {
                if (storeyData.Pillars[i].Sides != evt.changedProperty.intValue)
                    rebuild = true;

                storeyData.Pillars[i].Sides = evt.changedProperty.intValue;
            }

            if(rebuild)
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
        #endregion

        #region Floor
        floorHeightField.RegisterValueChangeCallback(evt =>
        {
            if (storeyData.FloorData == null)
                return;

            bool rebuild = false;

            if (storeyData.FloorData.Height != evt.changedProperty.floatValue)
                rebuild = true;

            storeyData.FloorData.Height = evt.changedProperty.floatValue;

            if(rebuild)
                buildable.Build();
        });
        #endregion
        #endregion

        #region Add Fields to Container
        container.Add(nameFoldout);
        wallFoldout.Add(wallHeightField);
        wallFoldout.Add(wallDepthField);
        wallFoldout.Add(cornerFoldout);
        cornerFoldout.Add(cornerTypeField);
        cornerFoldout.Add(cornerSidesField);
        pillarFoldout.Add(pillarHeightField);
        pillarFoldout.Add(pillarWidthField);
        pillarFoldout.Add(pillarDepthField);
        pillarFoldout.Add(pillarSidesField);
        pillarFoldout.Add(pillarIsSmoothField);
        floorFoldout.Add(floorHeightField);
        nameFoldout.Add(activeStoreyElementsField);
        nameFoldout.Add(wallFoldout);
        nameFoldout.Add(pillarFoldout);
        nameFoldout.Add(floorFoldout);
        #endregion

        return container;
    }
}
