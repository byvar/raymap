using OpenSpace.Object;
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
        public float littleAlpha_fogInfinite;
        public float bigAlpha_fogBlendNear;
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
        public byte sendLightFlag;
        public byte objectLightedFlag;
        public byte paintingLightFlag;
        public byte alphaLightFlag;
        public Vector3 interMinPos;
        public Vector3 exterMinPos;
        public Vector3 interMaxPos;
        public Vector3 exterMaxPos;
        public Vector3 exterCenterPos;
        // ...
        public float attFactor3;
        public float intensityMin_fogBlendFar;
        public float intensityMax;
        public Vector4 background_color;
        public uint createsShadowsOrNot;
        public string name = null;
        public Pointer dimmer;


        [Flags]
        public enum ObjectLightedFlag {
            None = 0,
            Environment = 1,
            Perso = 2
        }

		public enum LightType {
			Unknown = 0,
			Parallel = 1,
			Spherical = 2,
			Hotspot = 3, // R2: Cone
			Ambient = 4,
			ParallelOtherType = 5, // also seems to be the one with exterMinPos & exterMaxPos, so not spherical
			Fog = 6, // Also background color
			ParallelInASphere = 7,
			SphereOtherType = 8 // ignores persos?
		}

        private LightBehaviour light;
        public LightBehaviour Light {
            get {
                if (light == null) {
                    GameObject gao = new GameObject((name == null ? "Light" : name) + " @ " + offset + " | " +
                        "Type: " + type + " - Far: " + far + " - Near: " + near +
                        //" - FogBlendNear: " + bigAlpha_fogBlendNear + " - FogBlendFar: " + intensityMin_fogBlendFar +
                        " - AlphaLightFlag: " + alphaLightFlag +
                        " - PaintingLightFlag: " + paintingLightFlag +
                        " - ObjectLightedFlag: " + objectLightedFlag);
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

        public bool IsObjectLighted(ObjectLightedFlag flags) {
            if (Settings.s.engineVersion == Settings.EngineVersion.Montreal) return true;
            if (flags == ObjectLightedFlag.Environment) return true;
            return ((objectLightedFlag & (int)flags) == (int)flags);
        }
        
        public static LightInfo Read(Reader reader, Pointer offset) {
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
            0x100 size
            example: learn_30 @ 0x38684

            previously thought:
            0x160 size
            example: astro_00 @ 1d30c
            */
            LightInfo l = new LightInfo(offset);
            l.turnedOn = reader.ReadByte();
            l.castShadows = reader.ReadByte();
			if (Settings.s.game == Settings.Game.R2Revolution) {
				l.type = reader.ReadUInt16();
			} else {
				l.giroPhare = reader.ReadByte();
				l.pulse = reader.ReadByte();
				if (Settings.s.platform != Settings.Platform.DC) reader.ReadUInt32();
				l.type = reader.ReadUInt16();
				reader.ReadUInt16();
			}
			l.far = reader.ReadSingle();
			l.near = reader.ReadSingle();
			l.littleAlpha_fogInfinite = reader.ReadSingle();
			l.bigAlpha_fogBlendNear = reader.ReadSingle();
			l.giroStep = reader.ReadSingle();
			l.pulseStep = reader.ReadSingle();
			if (Settings.s.engineVersion == Settings.EngineVersion.R3) {
				l.pulseMaxRange = reader.ReadSingle();
				l.giroAngle = reader.ReadSingle();
				reader.ReadSingle();
			}
			if (Settings.s.platform == Settings.Platform.DC) reader.ReadUInt32();
			l.transMatrix = Matrix.Read(reader, Pointer.Current(reader));
			if (Settings.s.platform != Settings.Platform.DC && Settings.s.game != Settings.Game.R2Revolution) {
                reader.ReadUInt32(); // 0
                reader.ReadUInt32(); // 0
                reader.ReadUInt32(); // 0
                reader.ReadUInt32(); // 0
            }
            if (Settings.s.engineVersion != Settings.EngineVersion.Montreal) {
                if (Settings.s.platform == Settings.Platform.DC) {
					reader.ReadSingle();
                } else if(Settings.s.game != Settings.Game.R2Revolution) {
					reader.ReadUInt32(); // 0
					reader.ReadUInt32(); // 0
				}
                l.color = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                if (Settings.s.engineVersion == Settings.EngineVersion.R3 || Settings.s.game == Settings.Game.R2Revolution) {
                    l.shadowIntensity = reader.ReadSingle(); // 0
                }
                l.sendLightFlag = reader.ReadByte(); // Non-zero: light enabled
                l.objectLightedFlag = reader.ReadByte(); // & 1: Affect IPOs. & 2: Affect Persos. So 3 = affect all
                l.paintingLightFlag = reader.ReadByte();
                l.alphaLightFlag = reader.ReadByte();
                l.interMinPos    = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                l.exterMinPos    = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                l.interMaxPos    = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                l.exterMaxPos    = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                l.exterCenterPos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                l.attFactor3 = reader.ReadSingle();
                l.intensityMin_fogBlendFar = reader.ReadSingle();
                l.intensityMax = reader.ReadSingle();
                l.background_color = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
				if (Settings.s.engineVersion == Settings.EngineVersion.R3 || Settings.s.game == Settings.Game.R2Revolution) {
					l.createsShadowsOrNot = reader.ReadUInt32();
				}
            } else {
                l.paintingLightFlag = (byte)reader.ReadUInt32();
                reader.ReadUInt32();
                l.color = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                l.createsShadowsOrNot = reader.ReadUInt32();
                l.name = reader.ReadString(0x14);
                reader.ReadByte();
                l.sendLightFlag = reader.ReadByte(); // Non-zero: light enabled
                reader.ReadByte();
                reader.ReadByte();
                l.intensityMin_fogBlendFar = reader.ReadSingle();
                l.dimmer = Pointer.Read(reader);
            }

            lo.lights.Add(l);
            return l;
        }

        public void Write(Writer writer) {
            if (light != null && light.IsModified) {
                Pointer.Goto(ref writer, transMatrix.offset);
                transMatrix.Write(writer);
                Pointer.Goto(ref writer, Pointer.Current(writer) + (6 * 4));
                writer.Write(color.x); writer.Write(color.y); writer.Write(color.z); writer.Write(color.w);

                if (Settings.s.engineVersion == Settings.EngineVersion.R3) {
                    writer.Write(shadowIntensity); // 0
                }
                Pointer.Goto(ref writer, Pointer.Current(writer) + 2);
                writer.Write(paintingLightFlag);
                writer.Write(alphaLightFlag);
            }
        }
    }
}
