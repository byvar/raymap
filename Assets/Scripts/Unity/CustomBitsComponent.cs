using UnityEngine;
using System.Collections;
using System;
using OpenSpace;
using OpenSpace.EngineObject;

public class CustomBitsComponent : MonoBehaviour
{
    public string title = "Custom Bits";
    public int rawFlags;
    public Pointer offset;

    public void SetFlag(int index, bool value)
    {
        BitArray bitArray = new BitArray(BitConverter.GetBytes(this.rawFlags));
        bitArray.Set(index, value);
        int[] array = new int[1];
        bitArray.CopyTo(array, 0);
        this.rawFlags = array[0];
    }

    public bool GetFlag(int index)
    {
        BitArray bitArray = new BitArray(BitConverter.GetBytes(this.rawFlags));
        return bitArray.Get(index);
    }

    public void SetRawFlags(int rawFlags)
    {
        this.rawFlags = rawFlags;
    }
}
