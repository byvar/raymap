using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimA3DGeneral : OpenSpaceStruct {
		public bool readFull = false;

        public ushort speed;
        public ushort num_vectors;
        public ushort num_quaternions;
        public ushort num_hierarchies;
        public ushort num_NTTO;
        public ushort num_numNTTO;
        public ushort num_deformations;
        public ushort num_channels;
        public ushort num_onlyFrames;
        public ushort unk_12;
        public ushort unk_14;
        public ushort num_keyframes;
        public ushort num_events;
        public ushort unk_1A;
        public ushort subtractFramesForSpeed;
        public ushort unk_1E;
        public ushort speed2;
        public ushort unk_22_morphs;
        public ushort start_vectors2;
        public ushort start_quaternions2;
        public ushort num_morphData;
        public ushort start_vectors;
        public ushort start_quaternions;
        public ushort start_hierarchies;
        public ushort start_NTTO;
        public ushort start_deformations;
        public ushort start_onlyFrames;
        public ushort start_channels;
        public ushort start_events;
        public ushort start_morphData;

        public AnimVector[] vectors;
        public AnimQuaternion[] quaternions;
        public AnimHierarchy[] hierarchies;
        public AnimNTTO[] ntto;
        public AnimOnlyFrame[] onlyFrames;
        public AnimChannel[] channels;
        public AnimNumOfNTTO[] numOfNTTO;
        public AnimFramesKFIndex[] framesKFIndex;
        public AnimKeyframe[] keyframes;
        public AnimEvent[] events;
        public AnimMorphData[] morphData;
        public AnimDeformation[] deformations;

		protected override void ReadInternal(Reader reader) {
			if (readFull && CPA_Settings.s.game != CPA_Settings.Game.R2Revolution) {
				reader.AutoAligning = true;
			}
			if (CPA_Settings.s.game == CPA_Settings.Game.R2Revolution) {
				ReadRevolution(reader);
			} else if (CPA_Settings.s.engineVersion < CPA_Settings.EngineVersion.R3) {
				/* Each a3d is 0x38 long in R2. 0x34 in TT */
				speed = reader.ReadUInt16();
				num_vectors = reader.ReadUInt16();
				num_quaternions = reader.ReadUInt16();
				num_hierarchies = reader.ReadUInt16();
				num_NTTO = reader.ReadUInt16();
				num_numNTTO = reader.ReadUInt16();
				num_channels = reader.ReadUInt16();
				num_onlyFrames = reader.ReadUInt16();
				num_keyframes = reader.ReadUInt16();
				if (CPA_Settings.s.engineVersion >= CPA_Settings.EngineVersion.R2
					&& CPA_Settings.s.game != CPA_Settings.Game.R2Demo
					&& CPA_Settings.s.game != CPA_Settings.Game.RedPlanet) {
					unk_14 = reader.ReadUInt16();
				}
				num_events = reader.ReadUInt16();
				unk_1A = reader.ReadUInt16(); // vector related
				subtractFramesForSpeed = reader.ReadUInt16();
				unk_1E = reader.ReadUInt16(); // only frames again?
				speed2 = reader.ReadUInt16(); // field0 again?
				unk_22_morphs = reader.ReadUInt16(); // morph count ? (GAM_fn_p_stGetMorphData)
				start_vectors2 = reader.ReadUInt16();
				start_quaternions2 = reader.ReadUInt16();
				num_morphData = reader.ReadUInt16();
				start_vectors = reader.ReadUInt16();
				start_quaternions = reader.ReadUInt16();
				start_hierarchies = reader.ReadUInt16();
				start_NTTO = reader.ReadUInt16();
				start_onlyFrames = reader.ReadUInt16();
				start_channels = reader.ReadUInt16();
				start_events = reader.ReadUInt16();
				start_morphData = reader.ReadUInt16();
				if (CPA_Settings.s.engineVersion >= CPA_Settings.EngineVersion.R2
					&& CPA_Settings.s.game != CPA_Settings.Game.R2Demo
					&& CPA_Settings.s.game != CPA_Settings.Game.RedPlanet) {
					reader.ReadUInt16(); // padding?
				}
				if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.TT) {
					start_vectors2 = 0;
					start_quaternions2 = 0;
					start_vectors = 0;
					start_quaternions = 0;
					start_hierarchies = 0;
					start_NTTO = 0;
					start_onlyFrames = 0;
					start_channels = 0;
					start_events = 0;
					start_morphData = 0;
				}
			} else {
				/* Each a3d is 0x3c long */
				speed = reader.ReadUInt16();
				num_vectors = reader.ReadUInt16();
				num_quaternions = reader.ReadUInt16();
				num_hierarchies = reader.ReadUInt16();
				num_NTTO = reader.ReadUInt16();
				num_numNTTO = reader.ReadUInt16();
				num_deformations = reader.ReadUInt16();
				num_channels = reader.ReadUInt16();
				num_onlyFrames = reader.ReadUInt16();
				unk_12 = reader.ReadUInt16();
				unk_14 = reader.ReadUInt16();
				num_keyframes = reader.ReadUInt16();
				num_events = reader.ReadUInt16();
				unk_1A = reader.ReadUInt16();
				subtractFramesForSpeed = reader.ReadUInt16();
				unk_1E = reader.ReadUInt16();
				speed2 = reader.ReadUInt16();
				unk_22_morphs = reader.ReadUInt16();
				start_vectors2 = reader.ReadUInt16();
				start_quaternions2 = reader.ReadUInt16();
				num_morphData = reader.ReadUInt16();
				start_vectors = reader.ReadUInt16();
				start_quaternions = reader.ReadUInt16();
				start_hierarchies = reader.ReadUInt16();
				start_NTTO = reader.ReadUInt16();
				start_deformations = reader.ReadUInt16();
				start_onlyFrames = reader.ReadUInt16();
				start_channels = reader.ReadUInt16();
				start_events = reader.ReadUInt16();
				start_morphData = reader.ReadUInt16();
			}
			if (readFull && CPA_Settings.s.game != CPA_Settings.Game.R2Revolution) {
				ReadFull(reader);
				reader.AutoAligning = false;
			}
		}

		private void ReadRevolution(Reader reader) {
			MapLoader l = MapLoader.Loader;

			LegacyPointer off_vectors = LegacyPointer.Read(reader);
			LegacyPointer off_quaternions = LegacyPointer.Read(reader);
			LegacyPointer off_hierarchies = LegacyPointer.Read(reader);
			LegacyPointer off_ntto = LegacyPointer.Read(reader);
			LegacyPointer off_onlyFrames = LegacyPointer.Read(reader);
			LegacyPointer off_channels = LegacyPointer.Read(reader);
			LegacyPointer off_numNTTO = LegacyPointer.Read(reader);
			LegacyPointer off_kfIndex = LegacyPointer.Read(reader);
			LegacyPointer off_keyframes = LegacyPointer.Read(reader);
			LegacyPointer off_events = LegacyPointer.Read(reader);
			LegacyPointer off_morphData = LegacyPointer.Read(reader);
			speed = reader.ReadUInt16();
			num_vectors = reader.ReadUInt16();
			num_quaternions = reader.ReadUInt16();
			num_hierarchies = reader.ReadUInt16();
			num_NTTO = reader.ReadUInt16();
			num_numNTTO = reader.ReadUInt16();
			num_channels = reader.ReadUInt16();
			num_onlyFrames = reader.ReadUInt16();
			num_keyframes = reader.ReadUInt16();
			num_events = reader.ReadUInt16();
			reader.ReadUInt16();
			reader.ReadUInt16();
			reader.ReadUInt16();
			reader.ReadUInt16();
			num_morphData = reader.ReadUInt16();

			vectors = l.ReadArray<AnimVector>(num_vectors, reader, off_vectors);
			quaternions = l.ReadArray<AnimQuaternion>(num_quaternions, reader, off_quaternions);
			hierarchies = l.ReadArray<AnimHierarchy>(num_hierarchies, reader, off_hierarchies);
			ntto = l.ReadArray<AnimNTTO>(num_NTTO, reader, off_ntto);
			onlyFrames = l.ReadArray<AnimOnlyFrame>(num_onlyFrames, reader, off_onlyFrames);
			channels = l.ReadArray<AnimChannel>(num_channels, reader, off_channels);
			numOfNTTO = l.ReadArray<AnimNumOfNTTO>(num_numNTTO * num_channels, reader, off_numNTTO); // There's something after this
			framesKFIndex = l.ReadArray<AnimFramesKFIndex>(num_onlyFrames * num_channels, reader, off_kfIndex);
			keyframes = l.ReadArray<AnimKeyframe>(num_keyframes, reader, off_keyframes);
			events = l.ReadArray<AnimEvent>(num_events, reader, off_events);
			morphData = l.ReadArray<AnimMorphData>(num_morphData, reader, off_morphData);
		}


		private void ReadFull(Reader reader) {
			MapLoader l = MapLoader.Loader;

			if (AnimVector.Aligned) reader.AutoAlign(4);
			vectors = l.ReadArray<AnimVector>(num_vectors, reader);
			if (AnimQuaternion.Aligned) reader.AutoAlign(4);
			quaternions = l.ReadArray<AnimQuaternion>(num_quaternions, reader);
			if (AnimHierarchy.Aligned) reader.AutoAlign(4);
			hierarchies = l.ReadArray<AnimHierarchy>(num_hierarchies, reader);
			if (AnimNTTO.Aligned) reader.AutoAlign(4);
			ntto = l.ReadArray<AnimNTTO>(num_NTTO, reader);
			if (AnimOnlyFrame.Aligned) reader.AutoAlign(4);
			onlyFrames = l.ReadArray<AnimOnlyFrame>(num_onlyFrames, reader);
			if (AnimChannel.Aligned) reader.AutoAlign(4);
			channels = l.ReadArray<AnimChannel>(num_channels, reader);
			if (AnimNumOfNTTO.Aligned) reader.AutoAlign(4);
			numOfNTTO = l.ReadArray<AnimNumOfNTTO>(num_numNTTO * num_channels, reader);
			if (AnimFramesKFIndex.Aligned) reader.AutoAlign(4);
			framesKFIndex = l.ReadArray<AnimFramesKFIndex>(num_onlyFrames * num_channels, reader);
			if (AnimKeyframe.Aligned) reader.AutoAlign(4);
			keyframes = l.ReadArray<AnimKeyframe>(num_keyframes, reader);
			if (CPA_Settings.s.engineVersion > CPA_Settings.EngineVersion.TT) {
				if (AnimEvent.Aligned) reader.AutoAlign(4);
				events = l.ReadArray<AnimEvent>(num_events, reader);
				if (AnimMorphData.Aligned) reader.AutoAlign(4);
				//if (a3d.num_morphData > 0) MapLoader.Loader.print("MorphData " + a3d.num_morphData + ": " + Pointer.Current(reader));
				morphData = l.ReadArray<AnimMorphData>(num_morphData, reader);
			}
			/*MapLoader.Loader.print("A3D: " + offset + " - " + Pointer.Current(reader)
				+ " - NN:" + a3d.num_numNTTO
				+ " - CH:" + a3d.num_channels
				+ " - OF:" + a3d.num_onlyFrames
				+ " - KF:" + a3d.num_keyframes
				+ " - KFI: " + a3d.num_onlyFrames * a3d.num_channels
				+ " - V:" + a3d.num_vectors
				+ " - Q:" + a3d.num_quaternions);*/
			if (CPA_Settings.s.hasDeformations) {
				if (AnimDeformation.Aligned) reader.AutoAlign(4);
				deformations = l.ReadArray<AnimDeformation>(num_deformations, reader);
				//reader.Align(AnimDeformation.Size * a3d.deformations.Length, 4);
			}
        }

		/*public static int Size {
            get {
                switch (Settings.s.engineVersion) {
                    case Settings.EngineVersion.R3: return 0x3C;
                    case Settings.EngineVersion.R2:
                        if (Settings.s.game == Settings.Game.R2Demo) {
                            return 0x34;
                        } else {
                            return 0x38;
                        }
                    case Settings.EngineVersion.TT: return 0x34;
                    default: throw new Exception("Anim A3D size not set for this engine version");
                }
            }
        }*/

        public static bool Aligned {
            get { return false; }
        }
    }
}
