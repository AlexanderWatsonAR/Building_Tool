using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [System.Serializable]
    public class Arch : Shape
    {
        [SerializeField] float m_ArchHeight;
        [SerializeField] int m_ArchSides;
    }
}

