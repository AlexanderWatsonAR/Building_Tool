using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.HighDefinition;

namespace OnlyInvalid.Rendering
{
    public static class BuiltInMaterials
    {
        const string k_Standard = "Standard";
        const string k_URPLit = "Universal Render Pipeline/Lit";
        const string k_HDRPLit = "HDRP/Lit";

        const string k_URPAsset = "UniversalRenderPipelineAsset";
        const string k_HDRPAsset = "HDRenderPipelineAsset";

        public static Material Defualt
        {
            get
            {
                return GraphicsSettings.defaultRenderPipeline.defaultMaterial;
            }
        }

    }

}

