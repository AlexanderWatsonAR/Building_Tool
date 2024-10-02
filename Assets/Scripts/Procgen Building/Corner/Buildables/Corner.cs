using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Corner
{
    public class Corner : Polygon3D.Polygon3D
    {
        public CornerData CornerData => m_Data as CornerData;
    }
}
