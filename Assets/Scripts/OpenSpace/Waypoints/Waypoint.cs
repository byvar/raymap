using UnityEngine;
using UnityEditor;
using OpenSpace;

public class WayPoint {

    public Pointer offset;
    public Vector3 position;

    public WayPoint(Pointer offset)
    {
        this.offset = offset;
    }

    public static WayPoint Read(EndianBinaryReader reader, Pointer offset)
    {
        WayPoint wp = new WayPoint(offset);

        float x = reader.ReadSingle();
        float y = reader.ReadSingle();
        float z = reader.ReadSingle();
        wp.position = new Vector3(x, y, z);

        return wp;
    }
}