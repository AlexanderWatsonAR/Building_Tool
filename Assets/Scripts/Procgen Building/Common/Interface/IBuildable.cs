
using UnityEngine;
using UnityEngine.Events;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    public interface IBuildable
    {
        IBuildable Initialize(DirtyData data);
        void Build();
        void Demolish();

    }
}
