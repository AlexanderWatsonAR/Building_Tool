using JetBrains.Annotations;
using OnlyInvalid.ProcGenBuilding.Door;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

[CreateAssetMenu(fileName = "Door", menuName = "Doors/New Door")]
public class DoorScriptableObject : ContentScriptableObject
{
    [SerializeField] DoorData m_DoorData;

    public override Polygon3D CreateContent()
    {
        Door door = ProBuilderMesh.Create().gameObject.AddComponent<Door>();
        door.name = name;

        DoorData data = new DoorData(m_DoorData);
        data.IsDirty = true;

        door.Initialize(m_DoorData);
        return door;
    }
}
