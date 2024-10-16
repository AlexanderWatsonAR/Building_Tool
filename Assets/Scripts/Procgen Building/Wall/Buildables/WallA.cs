using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Polygon3D;

public class WallA : Polygon3D
{
    public WallAData WallAData => m_Data as WallAData;

    public override void Build()
    {
        base.Build();

        // When building the content, we want to apply the parents transform properties as well.
        // apply the wall transform and the opening transform.

        //WallAData.Openings[0].Content.Build();

        //if (WallAData.Openings == null)
        //    return;

        //foreach (OpeningAData opening in WallAData.Openings)
        //{
        //    if (opening.Content == null)
        //        continue;

        //    opening.Content.Build();
        //}
    }

}