using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    public class Frame : BaseFrame
    {
        public FrameData FrameData => m_Data as FrameData;

        protected override void CalculateInside()
        {
        }
    }
}
