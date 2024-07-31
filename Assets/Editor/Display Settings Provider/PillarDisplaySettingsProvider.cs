using OnlyInvalid.CustomVisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PillarDisplaySettingsProvider : SettingsProvider
{
    public PillarDisplaySettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
    {
    }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        var display = DisplayDataSettings.Data;

        Label title = new Label("Pillar")
        {
            style =
            {
                fontSize = 20,
                unityFontStyleAndWeight = FontStyle.Bold
            }
        };

        var sides = new SliderIntDisplayDataField(display.Pillar.Sides);

        sides.RegisterCallback<ChangeEvent<SliderDisplayData<int>>>(evt => display.Pillar.Sides = evt.newValue);

        rootElement.Add(title);
        rootElement.Add(sides);
    }

    [SettingsProvider]
    public static SettingsProvider CreateSettingsProvider()
    {
        return new PillarDisplaySettingsProvider("Procedural Building/Display/Pillar", SettingsScope.User);
    }

}
