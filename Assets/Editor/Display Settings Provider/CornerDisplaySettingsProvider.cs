using OnlyInvalid.CustomVisualElements;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CornerDisplaySettingsProvider : SettingsProvider
{
    public CornerDisplaySettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
    {
    }
    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        var display = DisplayDataSettings.Data;

        Label title = new Label("Corner")
        {
            style =
            {
                fontSize = 20,
                unityFontStyleAndWeight = FontStyle.Bold
            }
        };

        var sides = new SliderIntDisplayDataField(display.Corner.Sides);

        sides.RegisterCallback<ChangeEvent<SliderDisplayData<int>>>(evt => display.Corner.Sides = evt.newValue);

        rootElement.Add(title);
        rootElement.Add(sides);
    }

    [SettingsProvider]
    public static SettingsProvider CreateSettingsProvider()
    {
        return new CornerDisplaySettingsProvider("Procedural Building/Display/Corner", SettingsScope.User);
    }
}