using OpenSpace;
using System;
using UnityEngine;

public class SPOBoundingVolume {

    public Pointer offset;

    public enum BoundingVolumeType {
        Sphere, Box
    }

    public BoundingVolumeType type;

    // For Sphere
    public Vector3 sphereCenter;
    public float sphereRadius;

    // For Box
    public Vector3 boxMin;
    public Vector3 boxMax;

    public Vector3 boxCenter; // calculated from boxMin, boxMax
    public Vector3 boxSize; // calculated from boxMin, boxMax

    public SPOBoundingVolume(Pointer offset)
    {
        this.offset = offset;
    }

    // SuperObject BoundingVolume
    public static SPOBoundingVolume Read(EndianBinaryReader reader, Pointer offset, BoundingVolumeType type)
    {
        SPOBoundingVolume volume = new SPOBoundingVolume(offset);

        volume.type = type;

        // Read floats
        float float_1 = reader.ReadSingle();
        float float_2 = reader.ReadSingle();
        float float_3 = reader.ReadSingle();
        float float_4 = reader.ReadSingle();
        float float_5 = reader.ReadSingle();
        float float_6 = reader.ReadSingle();

        if (type == BoundingVolumeType.Box) {
            volume.boxMin = new Vector3(float_1, float_3, float_2);
            volume.boxMax = new Vector3(float_4, float_6, float_5);

            volume.boxSize = volume.boxMax - volume.boxMin;
            volume.boxCenter = volume.boxMin + (volume.boxSize * 0.5f);
        } else if (type == BoundingVolumeType.Sphere) {
            volume.sphereCenter = new Vector3(float_1, float_3, float_2);
            volume.sphereRadius = float_4;
        } else {
            throw new ArgumentException("Type should be Box or Sphere");
        }

        return volume;
    }
}