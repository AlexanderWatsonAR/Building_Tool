using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;

public static class BuildingShapeGenerator
{
    private static Extrudable ConstructSingleSupport()
    {
        throw new System.Exception();
    }

    public static void ConstructSingleSupport(this Extrudable extrudable, Vector3 startPosition, Vector3 endPosition, int steps)
    {
        ProBuilderMesh beam = ShapeGenerator.CreateShape(ShapeType.Sprite);
        //beam.GetComponent<MeshRenderer>().sharedMaterial = m_ColourSwatchMaterial;
        beam.transform.SetParent(extrudable.transform, false);
        beam.transform.localPosition = startPosition;
        float distance = Vector3.Distance(startPosition, endPosition);
        beam.transform.forward = Vector3Extensions.GetDirectionToTarget(beam.transform.localPosition, endPosition);
        beam.transform.eulerAngles += (Vector3.right * 90);

        steps = steps == 0 ? Mathf.FloorToInt(distance / 5) : steps;

        //Extrudable extrusion = beam.AddComponent<Extrudable>();
        extrudable.Extrude(beam.faces, ExtrudeMethod.FaceNormal, distance, steps);

        //Vector2 woodOffset = -m_ColourPalette.GetColorCoordinateByName("Wood_3").uv;

        //beam.SetMeshColour(woodOffset);

        beam.ToMesh();
        beam.Refresh();
    }

}
