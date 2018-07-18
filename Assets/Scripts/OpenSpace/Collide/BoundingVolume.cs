using OpenSpace;
using System;
using UnityEngine;

public class BoundingVolume {
    public Pointer offset;

    public enum Type {
        Sphere, Box
    }

    public Type type;

    // For Sphere
    public Vector3 sphereCenter;
    public float sphereRadius;

    // For Box
    public Vector3 boxMin;
    public Vector3 boxMax;

    public Vector3 boxCenter; // calculated from boxMin, boxMax
    public Vector3 boxSize; // calculated from boxMin, boxMax

    public BoundingVolume(Pointer offset) {
        this.offset = offset;
    }

    public bool ContainsPoint(Vector3 pos) {
        switch (type) {
            case Type.Box:
                return pos.x >= boxMin.x && pos.x <= boxMax.x
                && pos.y >= boxMin.y && pos.y <= boxMax.y
                && pos.z >= boxMin.z && pos.z <= boxMax.z;
            case Type.Sphere:
                return Vector3.Distance(pos, sphereCenter) <= sphereRadius;
            default:
                throw new ArgumentException("Type should be Box or Sphere");
        }
    }

    // SuperObject BoundingVolume
    public static BoundingVolume Read(EndianBinaryReader reader, Pointer offset, Type type) {
        BoundingVolume volume = new BoundingVolume(offset);

        volume.type = type;

        // Read floats
        float float_1 = reader.ReadSingle();
        float float_2 = reader.ReadSingle();
        float float_3 = reader.ReadSingle();
        float float_4 = reader.ReadSingle();
        float float_5 = reader.ReadSingle();
        float float_6 = reader.ReadSingle();

        if (type == Type.Box) {
            volume.boxMin = new Vector3(float_1, float_3, float_2);
            volume.boxMax = new Vector3(float_4, float_6, float_5);

            volume.boxSize = volume.boxMax - volume.boxMin;
            volume.boxCenter = volume.boxMin + (volume.boxSize * 0.5f);
        } else if (type == Type.Sphere) {
            volume.sphereCenter = new Vector3(float_1, float_3, float_2);
            volume.sphereRadius = float_4;
        } else {
            throw new ArgumentException("Type should be Box or Sphere");
        }

        return volume;
    }
}