using Newtonsoft.Json;
using OpenSpace.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Visual {
    public class LightInfo : OpenSpaceStruct {
        [JsonIgnore]
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

        public LightBehaviour light; // very dirty

        public LightInfo() : base() {
            containingSectors = new List<Sector>();
        }

        public bool IsObjectLighted(ObjectLightedFlag flags) {
            if (Settings.s.engineVersion == Settings.EngineVersion.Montreal) return true;
            if (Settings.s.platform != Settings.Platform.PS2 || Settings.s.engineVersion < Settings.EngineVersion.R3) {
                return true;
                //if (flags == ObjectLightedFlag.Environment) return true;
            }
            return ((objectLightedFlag & (int)flags) == (int)flags);
        }

        public void Write(Writer writer) {
            if (light != null && light.IsModified) {
                Pointer.Goto(ref writer, transMatrix.offset);
                transMatrix.Write(writer);
                Pointer.Goto(ref writer, Pointer.Current(writer) + (6 * 4));
                writer.Write(color.x); writer.Write(color.y); writer.Write(color.z); writer.Write(color.w);

                if (Settings.s.engineVersion != Settings.EngineVersion.Montreal) {
                    if (Settings.s.engineVersion == Settings.EngineVersion.R3) {
                        writer.Write(shadowIntensity); // 0
                    }
                    Pointer.Goto(ref writer, Pointer.Current(writer) + 2);
                    writer.Write(paintingLightFlag);
                    writer.Write(alphaLightFlag);
                }
            }
        }

        protected override void ReadInternal(Reader reader) {
            Load.lights.Add(this);

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

            turnedOn = reader.ReadByte();
            castShadows = reader.ReadByte();
            if (Settings.s.game == Settings.Game.R2Revolution || Settings.s.game == Settings.Game.LargoWinch) {
                type = reader.ReadUInt16();
            } else {
                giroPhare = reader.ReadByte();
                pulse = reader.ReadByte();
                if (Settings.s.platform != Settings.Platform.DC) reader.ReadUInt32();
                type = reader.ReadUInt16();
                reader.ReadUInt16();
            }
            far = reader.ReadSingle();
            near = reader.ReadSingle();
            littleAlpha_fogInfinite = reader.ReadSingle();
            bigAlpha_fogBlendNear = reader.ReadSingle();
            if (Settings.s.game == Settings.Game.LargoWinch) reader.ReadSingle();
            giroStep = reader.ReadSingle();
            pulseStep = reader.ReadSingle();
            if (Settings.s.engineVersion == Settings.EngineVersion.R3 && Settings.s.game != Settings.Game.LargoWinch) {
                pulseMaxRange = reader.ReadSingle();
                giroAngle = reader.ReadSingle();
                reader.ReadSingle();
            }
            if (Settings.s.platform == Settings.Platform.DC) reader.ReadUInt32();
            transMatrix = Matrix.Read(reader, Pointer.Current(reader));
            if (Settings.s.platform != Settings.Platform.PS2 && Settings.s.platform != Settings.Platform.DC && Settings.s.game != Settings.Game.R2Revolution && Settings.s.game != Settings.Game.LargoWinch) {
                reader.ReadUInt32(); // 0
                reader.ReadUInt32(); // 0
                reader.ReadUInt32(); // 0
                reader.ReadUInt32(); // 0
            }
            if (Settings.s.engineVersion != Settings.EngineVersion.Montreal) {
                if (Settings.s.platform == Settings.Platform.DC) {
                    reader.ReadSingle();
                } else if (Settings.s.game != Settings.Game.R2Revolution
                    && Settings.s.game != Settings.Game.LargoWinch
                    && Settings.s.platform != Settings.Platform.PS2) {
                    reader.ReadUInt32(); // 0
                    reader.ReadUInt32(); // 0
                }
                //lo.print("LIGHT " + Pointer.Current(reader));
                color = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                if (Settings.s.engineVersion == Settings.EngineVersion.R3 && Settings.s.platform == Settings.Platform.PS2) {
                    reader.ReadBytes(0x20);
                }
                if (Settings.s.engineVersion == Settings.EngineVersion.R3 || Settings.s.game == Settings.Game.R2Revolution) {
                    shadowIntensity = reader.ReadSingle(); // 0
                }
                if (Settings.s.game == Settings.Game.Dinosaur) {
                    sendLightFlag = reader.ReadByte(); // Non-zero: light enabled
                    objectLightedFlag = reader.ReadByte(); // & 1: Affect IPOs. & 2: Affect Persos. So 3 = affect all
                    alphaLightFlag = reader.ReadByte();
                    paintingLightFlag = reader.ReadByte();
                } else if (Settings.s.game == Settings.Game.LargoWinch) {
                    sendLightFlag = reader.ReadByte(); // Non-zero: light enabled
                    paintingLightFlag = reader.ReadByte();
                    alphaLightFlag = reader.ReadByte();
                    objectLightedFlag = reader.ReadByte(); // & 1: Affect IPOs. & 2: Affect Persos. So 3 = affect all
                } else {
                    sendLightFlag = reader.ReadByte(); // Non-zero: light enabled
                    objectLightedFlag = reader.ReadByte(); // & 1: Affect IPOs. & 2: Affect Persos. So 3 = affect all
                    paintingLightFlag = reader.ReadByte();
                    alphaLightFlag = reader.ReadByte();
                }
                interMinPos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                exterMinPos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                interMaxPos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                exterMaxPos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                exterCenterPos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                attFactor3 = reader.ReadSingle();
                intensityMin_fogBlendFar = reader.ReadSingle();
                intensityMax = reader.ReadSingle();
                background_color = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                if ((Settings.s.engineVersion == Settings.EngineVersion.R3 && Settings.s.game != Settings.Game.Dinosaur && Settings.s.game != Settings.Game.LargoWinch) || Settings.s.game == Settings.Game.R2Revolution) {
                    createsShadowsOrNot = reader.ReadUInt32();
                }
                if (Settings.s.engineVersion == Settings.EngineVersion.R3 && Settings.s.platform == Settings.Platform.PS2) {
                    reader.ReadBytes(0xC);
                }
            } else {
                paintingLightFlag = (byte)reader.ReadUInt32();
                reader.ReadUInt32();
                color = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                createsShadowsOrNot = reader.ReadUInt32();
                name = reader.ReadString(0x14);
                reader.ReadByte();
                sendLightFlag = reader.ReadByte(); // Non-zero: light enabled
                reader.ReadByte();
                reader.ReadByte();
                intensityMin_fogBlendFar = reader.ReadSingle();
                dimmer = Pointer.Read(reader);
            }
        }
    }
}
