using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    public class R3Light : IEquatable<R3Light> {
        public R3Pointer offset;
        public List<R3Sector> containingSectors;

        public byte turnedOn;
        public byte castShadows;
        public byte giroPhare;
        public byte pulse;
        // ...
        public ushort type;
        // ...
        public float far;
        public float near;
        public float littleAlpha;
        public float bigAlpha;
        public float giroStep;
        public float pulseStep;
        public float pulseMaxRange;
        public float giroAngle;
        // ...
        public R3Matrix transMatrix;
        // ...
        public Vector4 color;
        public float shadowIntensity;
        // ...
        public byte paintingLightFlag;
        public byte alphaLightFlag;
        public Vector3 interMinPos;
        public Vector3 exterMinPos;
        public Vector3 interMaxPos;
        public Vector3 exterMaxPos;
        // ...
        public float attFactor3;
        public float intensityMin;
        public float intensityMax;
        public Vector4 background_color;
        public uint createsShadowsOrNot;

        private LightBehaviour light;
        public LightBehaviour Light {
            get {
                if (light == null) {
                    GameObject gao = new GameObject("Light @ " + String.Format("0x{0:X}", offset.offset) + " | " +
                        "Type: " + type + " - Far: " + far + " - Near: " + near);
                    Vector3 pos = transMatrix.GetPosition(convertAxes: true);
                    Quaternion rot = transMatrix.GetRotation(convertAxes: true);
                    Vector3 scale = transMatrix.GetScale(convertAxes: true);
                    gao.transform.localPosition = pos;
                    gao.transform.localRotation = rot;
                    gao.transform.localScale = scale;
                    light = gao.AddComponent<LightBehaviour>();
                    light.r3l = this;
                }
                return light;
            }
        }

        public R3Light(R3Pointer offset) {
            this.offset = offset;
            containingSectors = new List<R3Sector>();
        }

        public override bool Equals(System.Object obj) {
            return obj is R3Light && this == (R3Light)obj;
        }
        public override int GetHashCode() {
            return offset.GetHashCode();
        }

        public bool Equals(R3Light other) {
            return this == (R3Light)other;
        }

        public static bool operator ==(R3Light x, R3Light y) {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.offset == y.offset;
        }
        public static bool operator !=(R3Light x, R3Light y) {
            return !(x == y);
        }


        public static R3Light Read(EndianBinaryReader reader, R3Pointer offset) {
            R3Loader lo = R3Loader.Loader;
            R3Light parsedLight = lo.lights.FirstOrDefault(li => li.offset == offset);
            if (parsedLight != null) return parsedLight;

            /* light struct sz = 0x110
            transformation matrix starts at 0x34 and is 0x50 long
            color is at + 156(decimal) or 0x9c and is 4x 4 bytes, but it's out of range?
            can be negative to create shadows maybe?
            at + 252(or 0xfc) is another color
            perso also has light sometimes (at offset + 28)*/
            R3Light l = new R3Light(offset);
            l.turnedOn = reader.ReadByte();
            l.castShadows = reader.ReadByte();
            l.giroPhare = reader.ReadByte();
            l.pulse = reader.ReadByte();
            reader.ReadUInt32();
            l.type = reader.ReadUInt16();
            reader.ReadUInt16();
            l.far = reader.ReadSingle();
            l.near = reader.ReadSingle();
            l.littleAlpha = reader.ReadSingle();
            l.bigAlpha = reader.ReadSingle();
            l.giroStep = reader.ReadSingle();
            l.pulseStep = reader.ReadSingle();
            l.pulseMaxRange = reader.ReadSingle();
            l.giroAngle = reader.ReadSingle();
            reader.ReadSingle();
            l.transMatrix = R3Matrix.Read(reader, R3Pointer.Current(reader));
            reader.ReadUInt32(); // 0
            reader.ReadUInt32(); // 0
            reader.ReadUInt32(); // 0
            reader.ReadUInt32(); // 0
            reader.ReadUInt32(); // 0
            reader.ReadUInt32(); // 0
            l.color = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            l.shadowIntensity = reader.ReadSingle(); // 0
            reader.ReadByte();
            reader.ReadByte();
            l.paintingLightFlag = reader.ReadByte();
            l.alphaLightFlag = reader.ReadByte();
            l.interMinPos = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            l.exterMinPos = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            l.interMaxPos = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            l.exterMaxPos = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            l.attFactor3 = reader.ReadSingle();
            l.intensityMin = reader.ReadSingle();
            l.intensityMax = reader.ReadSingle();
            l.background_color = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            l.createsShadowsOrNot = reader.ReadUInt32();

            lo.lights.Add(l);
            return l;
        }
    }
}
