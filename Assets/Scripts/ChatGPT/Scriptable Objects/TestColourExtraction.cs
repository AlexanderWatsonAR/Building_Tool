using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestColourExtraction : MonoBehaviour
{
    public ColourExtractor ColourExtractor;

    // Start is called before the first frame update
    void Start()
    {
        ColourExtractor.ExtractColours();
    }
}
