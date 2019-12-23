using Newtonsoft.Json;
using OpenSpace.Animation.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.ComponentLargo {
    public class AnimA3DLargo : OpenSpaceStruct {
		public Pointer off_vectors;
		public Pointer off_quaternions;
		public Pointer off_hierarchies;
		public Pointer off_ntto;
		public Pointer off_onlyFrames;
		public Pointer off_channels;
		public Pointer off_numNTTO;
		public Pointer off_frameVector;
		public Pointer off_frameQuaternion;
		public Pointer off_unknowns;
		public Pointer off_deformations;

		public ushort num_onlyFrames;
		public byte num_channels;
		public AnimVector[] vectors;
		public AnimQuaternion[] quaternions;
		public AnimHierarchy[] hierarchies;
		public AnimNTTO[] ntto;
		public AnimOnlyFrame[] onlyFrames;
		public AnimChannelLargo[] channels;
		public AnimNumOfNTTO[] numOfNTTO;
		public AnimFrameVector[] framevector;
		public AnimFrameQuaternion[] framequaternion;
		public AnimFrameUnknown[] unknowns;
		public AnimDeformation[] deformations;

		public ushort num_vectors;
		public ushort num_quaternions;
		public ushort num_hierarchies;
		public ushort num_ntto;
		public ushort num_numOfNTTO;
		public ushort num_framevectors;
		public ushort num_framequaternions;
		public ushort num_unknowns;
		public ushort num_deformations;

		protected override void ReadInternal(Reader reader) {
			MapLoader l = MapLoader.Loader;

			off_vectors = Pointer.Read(reader);
			off_quaternions = Pointer.Read(reader);
			off_hierarchies = Pointer.Read(reader);
			off_ntto = Pointer.Read(reader);
			off_onlyFrames = Pointer.Read(reader);
			off_channels = Pointer.Read(reader);
			off_numNTTO = Pointer.Read(reader);
			off_frameVector = Pointer.Read(reader);
			off_frameQuaternion = Pointer.Read(reader);
			off_unknowns = Pointer.Read(reader);
			off_deformations = Pointer.Read(reader);

			onlyFrames = l.ReadArray<AnimOnlyFrame>(num_onlyFrames, reader, off_onlyFrames);
			channels = l.ReadArray<AnimChannelLargo>(num_channels, reader, off_channels);

			if (onlyFrames.Length > 0) num_deformations = onlyFrames[0].deformation;
			if (onlyFrames.Length > 0) num_hierarchies = (ushort)onlyFrames.Max(of => of.start_hierarchies_for_frame + of.num_hierarchies_for_frame);
			if (channels.Length > 0) num_unknowns = (ushort)channels.Sum(ch => ch.numFrameUnknowns);
			if (channels.Length > 0) num_framequaternions = (ushort)channels.Sum(ch => ch.numFrameQuaternions);
			if (channels.Length > 0) num_framevectors = (ushort)channels.Sum(ch => ch.numFrameVectors);
			if (onlyFrames.Length > 0 && channels.Length > 0) num_numOfNTTO = (ushort)(channels.Max(ch => ch.numOfNTTO) + onlyFrames.Max(of => of.numOfNTTO) + 1);

			framevector = l.ReadArray<AnimFrameVector>(num_framevectors, reader, off_frameVector);
			framequaternion = l.ReadArray<AnimFrameQuaternion>(num_framequaternions, reader, off_frameQuaternion);
			unknowns = l.ReadArray<AnimFrameUnknown>(num_unknowns, reader, off_unknowns);
			numOfNTTO = l.ReadArray<AnimNumOfNTTO>(num_numOfNTTO, reader, off_numNTTO);
			hierarchies = l.ReadArray<AnimHierarchy>(num_hierarchies, reader, off_hierarchies);
			deformations = l.ReadArray<AnimDeformation>(num_deformations, reader, off_deformations);

			if (framevector.Length > 0) num_vectors = (ushort)(framevector.Max(fv => fv.vector) + 1);
			if (framequaternion.Length > 0) num_quaternions = (ushort)(framequaternion.Max(fq => fq.quaternion) + 1);
			if (numOfNTTO.Length > 0) num_ntto = (ushort)(numOfNTTO.Max(n => n.numOfNTTO) + 1);

			vectors = l.ReadArray<AnimVector>(num_vectors, reader, off_vectors);
			quaternions = l.ReadArray<AnimQuaternion>(num_quaternions, reader, off_quaternions);
			ntto = l.ReadArray<AnimNTTO>(num_ntto, reader, off_ntto);
		}
	}
}
