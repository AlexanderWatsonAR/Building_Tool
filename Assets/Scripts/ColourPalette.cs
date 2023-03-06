using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Scriptable Objects/Colour Palette")]
public class ColourPalette : ScriptableObject
{
    public ColourExtractor ColourExtractor;
    public ColourCoordinate[] colourCoordinates;

    [System.Serializable]
    public class ColourCoordinate
    {
        public Color color;
        public Vector2 uv;
        public string name;
    }

    public void GetColourCoordinates()
    {
        for (int i = 0; i < colourCoordinates.Length; i++)
        {
            if (!ColourExtractor.DoesColourExistInPalette(colourCoordinates[i].color))
                continue;

            colourCoordinates[i].uv = ColourExtractor.GetColourCoordinate(colourCoordinates[i].color).uv;
        }
    }

    public ColourCoordinate GetColorCoordinateByName(string colorName)
    {
        return colourCoordinates.FirstOrDefault(c => c.name == colorName);
    }
}
