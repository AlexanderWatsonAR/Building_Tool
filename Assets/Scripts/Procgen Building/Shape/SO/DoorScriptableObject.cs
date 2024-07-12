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
        ProBuilderMesh doorMesh = ProBuilderMesh.Create();
        Door door = doorMesh.gameObject.AddComponent<Door>();
        door.name = name;
        m_DoorData.IsDirty = true;
        door.Initialize(m_DoorData);
        return door;
    }
}
