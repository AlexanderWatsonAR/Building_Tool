using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using OnlyInvalid.ProcGenBuilding.Common;
using UnityEngine.ProBuilder;
using Unity.VisualScripting;
using static UnityEditor.PlayerSettings;
using UnityEngine.UIElements;

public class WallA : Polygon3D
{
    public WallAData WallAData => m_Data as WallAData;

    public override Buildable Initialize(DirtyData data)
    {
        base.Initialize(data);

        //OpeningAData opening = new OpeningAData(new Square(PolygonMaker.PivotPoint.BottomCentre), Vector3.up * 0.25f, Vector3.zero, Vector3.one * 0.5f);

        //ProBuilderMesh frameMesh = ProBuilderMesh.Create();
        //Frame frame = frameMesh.AddComponent<Frame>();
        //frame.transform.SetParent(transform, false);
        //FrameData frameData = new FrameData(new Square(), new List<Polygon2DData>(), 0.95f, 0.05f, Vector3.zero, Vector3.zero, Vector3.one);
        //frame.Initialize(frameData);
        //frame.FrameData.IsDirty = true;
        //frame.Build();




        //Vector3 pos = this.Position + opening.Position + Vector3.zero;
        //Vector3 eulerAngle = (this.Rotation * opening.Rotation * Quaternion.identity).eulerAngles;
        //Vector3 scale = Vector3.Scale(Vector3.Scale(this.Scale, opening.Scale), Vector3.one);

        //frame.transform.SetParent()

        //FrameData data = new FrameData(new Square(), new List<Polygon2DData>(), 0.95f, 0.05f, pos, eulerAngle, scale);
        //data.IsDirty = true;
        //frame.Initialize(data);

        //opening.Content = frame;

        //WallAData.AddToInterior(opening);

        return this;
    }

    public override void Build()
    {
        if (!WallAData.IsDirty)
            return;

        if (WallAData.HasInterior)
        {
            foreach(OpeningAData opening in WallAData.Openings)
            {
                opening.Content.transform.SetParent(this.transform, false);
            }
        }


        base.Build();

        //if (!m_Data.IsDirty)
        //    return;

        // base.Build();

        //Vector3[] controlPoints = WallAData.ControlPoints;

        //for(int i = 0; i < controlPoints.Length; i++)
        //{
        //    controlPoints[i] += Vector3.forward * transform.localScale.z * 0.5f;
        //}

        //m_ProBuilderMesh.CreateShapeFromPolygon(controlPoints, Polygon2DData.Normal(), Polygon2DData.Holes);
        //m_ProBuilderMesh.Solidify(Polygon3DData.Depth);
        //Polygon2DData.IsDirty = false;

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