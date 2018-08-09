using UnityEngine;
using System.Collections;
using System;
using OpenSpace.EngineObject;
using System.Collections.Generic;

public class CustomBitsComponent : MonoBehaviour {
    public StandardGame stdGame;
    public bool hasAiCustomBits = false;
    public bool modified = false;
    private BitArray[] bits;

    public void Init() {
        if (hasAiCustomBits) {
            bits = new BitArray[4];
        } else bits = new BitArray[4];
        for (int i = 0; i < bits.Length; i++) {
            bits[i] = new BitArray(BitConverter.GetBytes(GetRawFlags((CustomBitsType)i)));
        }
    }

    public enum CustomBitsType {
        CustomBits = 0,
        CustomBitsInitial,
        AICustomBits,
        AICustomBitsInitial
    }

    public int GetRawFlags(CustomBitsType type) {
        switch (type) {
            case CustomBitsType.CustomBits: return stdGame.customBits;
            case CustomBitsType.AICustomBits: return stdGame.aiCustomBits;
            case CustomBitsType.CustomBitsInitial: return stdGame.customBitsInitial;
            case CustomBitsType.AICustomBitsInitial: return stdGame.aiCustomBitsInitial;
            default: return 0;
        }
    }

    public void SetFlag(CustomBitsType type, int index, bool value) {
        BitArray bitArray = bits[(int)type];
        if (bitArray.Get(index) != value) {
            bitArray.Set(index, value);
            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            SetRawFlags(type, array[0]);
        }
    }

    public bool GetFlag(CustomBitsType type, int index) {
        BitArray bitArray = bits[(int)type];
        return bitArray.Get(index);
    }

    public void SetRawFlags(CustomBitsType type, int rawFlags) {
        switch (type) {
            case CustomBitsType.CustomBits:
                if (stdGame.customBits != rawFlags) {
                    modified = true;
                    stdGame.customBits = rawFlags;
                }
                break;
            case CustomBitsType.AICustomBits:
                if (stdGame.aiCustomBits != rawFlags) {
                    modified = true;
                    stdGame.aiCustomBits = rawFlags;
                }
                break;
            case CustomBitsType.CustomBitsInitial:
                if (stdGame.customBitsInitial != rawFlags) {
                    modified = true;
                    stdGame.customBitsInitial = rawFlags;
                }
                break;
            case CustomBitsType.AICustomBitsInitial:
                if (stdGame.aiCustomBitsInitial != rawFlags) {
                    modified = true;
                    stdGame.aiCustomBitsInitial = rawFlags;
                }
                break;
            default: break;
        }
    }


}
