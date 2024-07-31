using OnlyInvalid.CustomVisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ShapeDisplaySettingsProvider : SettingsProvider
{
    public ShapeDisplaySettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
    {
    }
    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        var display = DisplayDataSettings.Data;

        Label title = new Label("Shape")
        {
            style =
            {
                fontSize = 20,
                unityFontStyleAndWeight = FontStyle.Bold
            }
        };

        var nPolygon = display.NPolygon;
        var arch = display.Arch;

        Label nPolygonLabel = new Label("N-Polygon")
        {
            style =
            {
                fontSize = 16,
                unityFontStyleAndWeight = FontStyle.Bold
            }
        };
        Label archLabel = new Label("Arch")
        {
            style =
            {
                fontSize = 16,
                unityFontStyleAndWeight = FontStyle.Bold
            }
        };
        var nPolygonSides = new SliderIntDisplayDataField(nPolygon.Sides);
        var archSides = new SliderIntDisplayDataField(arch.Sides);

        nPolygonSides.RegisterCallback<ChangeEvent<SliderDisplayData<int>>>(evt => nPolygon.Sides = evt.newValue);
        archSides.RegisterCallback<ChangeEvent<SliderDisplayData<int>>>(evt => arch.Sides = evt.newValue);

        rootElement.Add(title);
        rootElement.Add(nPolygonLabel);
        rootElement.Add(nPolygonSides);
        rootElement.Add(archLabel);
        rootElement.Add(archSides);

    }

    [SettingsProvider]
    public static SettingsProvider CreateSettingsProvider()
    {
        return new ShapeDisplaySettingsProvider("Procedural Building/Display/Shape", SettingsScope.User);
    }

}
