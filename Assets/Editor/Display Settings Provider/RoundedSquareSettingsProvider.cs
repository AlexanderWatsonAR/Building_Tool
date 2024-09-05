using OnlyInvalid.CustomVisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UIElements;

public class RoundedSquareSettingsProvider : SettingsProvider
{
    public RoundedSquareSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
    {

    }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        var display = DisplayDataSettings.Data;

        Label title = new Label("Rounded Square")
        {
            style =
            {
                fontSize = 20,
                unityFontStyleAndWeight = FontStyle.Bold
            }
        };

        var roundedSquare = display.RoundedSquare;

        var sides = new SliderIntDisplayDataField(roundedSquare.Sides);
        var curveSize = new SliderDisplayDataField(roundedSquare.CurveSize);

        sides.RegisterCallback<ChangeEvent<SliderDisplayData<int>>>(evt => roundedSquare.Sides = evt.newValue);
        curveSize.RegisterCallback<ChangeEvent<SliderDisplayData<float>>>(evt => roundedSquare.CurveSize = evt.newValue);

        rootElement.Add(title);
        rootElement.Add(sides);
        rootElement.Add(curveSize);
    }


    [SettingsProvider]
    public static SettingsProvider CreateSettingsProvider()
    {
        return new RoundedSquareSettingsProvider("Procedural Building/Display/Rounded Square", SettingsScope.User);
    }
}
