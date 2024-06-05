using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Arch", menuName = "Shape/Calculated Shape/New Arch")]
public class ArchSO : ShapeSO
{
    public Arch Arch => m_Shape as Arch;

    public override void Initialize()
    {
        m_Shape = new Arch(0.75f, 5);
    }

}
