using OnlyInvalid.ProcGenBuilding.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OnlyInvalid.ProcGenBuilding.Common
{

    [CreateAssetMenu(fileName = "Opening", menuName = "Opening/New Opening")]
    public class OpeningScriptableObject : ScriptableObject
    {
        [SerializeReference] ShapeScriptableObject m_ShapeSO;
        [SerializeField] OpeningData m_Opening;

        public OpeningData Opening => m_Opening;
    }
}