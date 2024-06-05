using OnlyInvalid.ProcGenBuilding.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OnlyInvalid.ProcGenBuilding.Common
{

    [CreateAssetMenu(fileName = "Opening", menuName = "Opening/New Opening")]
    public class OpeningSO : ScriptableObject
    {
        [SerializeReference] ShapeSO m_ShapeSO;
        [SerializeField] OpeningData m_Opening;

        public OpeningData Opening => m_Opening;
    }
}