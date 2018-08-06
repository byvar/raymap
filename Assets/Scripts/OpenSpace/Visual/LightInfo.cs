using OpenSpace.EngineObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Visual {
    public class LightInfo : IEquatable<LightInfo> {
        public Pointer offset;
        public List<Sector> containingSectors;

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
        public Matrix transMatrix;
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
                    Quaternion rot = transMatrix.GetRotation(convertAxes: true) * Quaternion.Euler(-90, 0,0);
                    Vector3 scale = transMatrix.GetScale(convertAxes: true);
                    gao.transform.localPosition = pos;
                    gao.transform.localRotation = rot;
                    gao.transform.localScale = scale;
                    light = gao.AddComponent<LightBehaviour>();
                    light.li = this;
                }
                return light;
            }
        }

        public LightInfo(Pointer offset) {
            this.offset = offset;
            containingSectors = new List<Sector>();
        }

        public override bool Equals(System.Object obj) {
            return obj is LightInfo && this == (LightInfo)obj;
        }
        public override int GetHashCode() {
            return offset.GetHashCode();
        }

        public bool Equals(LightInfo other) {
            return this == (LightInfo)other;
        }

        public static bool operator ==(LightInfo x, LightInfo y) {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.offset == y.offset;
        }
        public static bool operator !=(LightInfo x, LightInfo y) {
            return !(x == y);
        }


        public static LightInfo Read(EndianBinaryReader reader, Pointer offset) {
            MapLoader lo = MapLoader.Loader;
            LightInfo parsedLight = lo.lights.FirstOrDefault(li => li.offset == offset);
            if (parsedLight != null) return parsedLight;
            //lo.print("Light offset; " + offset);
            /* For Rayman 3:
            light struct sz = 0x110
            transformation matrix starts at 0x34 and is 0x50 long
            color is at + 156(decimal) or 0x9c and is 4x 4 bytes, but it's out of range?
            can be negative to create shadows maybe?
            at + 252(or 0xfc) is another color
            perso also has light sometimes (at offset + 28)*/

            /* For R2:
            0x160 size
            example: astro_00 @ 1d30c
            */
            LightInfo l = new LightInfo(offset);
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
            if (Settings.s.engineMode == Settings.EngineMode.R3) {
                l.pulseMaxRange = reader.ReadSingle();
                l.giroAngle = reader.ReadSingle();
                reader.ReadSingle();
            }
            l.transMatrix = Matrix.Read(reader, Pointer.Current(reader));
            reader.ReadUInt32(); // 0
            reader.ReadUInt32(); // 0
            reader.ReadUInt32(); // 0
            reader.ReadUInt32(); // 0
            reader.ReadUInt32(); // 0
            reader.ReadUInt32(); // 0
            l.color = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            if (Settings.s.engineMode == Settings.EngineMode.R3) {
                l.shadowIntensity = reader.ReadSingle(); // 0
            }
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
