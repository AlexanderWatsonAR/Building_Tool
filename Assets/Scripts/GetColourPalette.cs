using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetColourPalette : MonoBehaviour
{
    public ColourPalette ColourPalette;

    // Start is called before the first frame update
    void Start()
    {
        ColourPalette.GetColourCoordinates();
    }
}
