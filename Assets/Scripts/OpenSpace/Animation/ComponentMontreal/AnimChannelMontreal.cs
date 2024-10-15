using Newtonsoft.Json;
using OpenSpace.Animation.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.ComponentMontreal {
    public class AnimChannelMontreal : OpenSpaceStruct {
        public LegacyPointer off_matrix;
        public uint isIdentity = 0;
        public byte objectIndex;
        public byte unk1;
        public short unk2;
        public short unk3;
        public byte unkByte1; // object index?
        public byte unkByte2;
        public uint unkUint;
        public Matrix matrix = null;

		protected override void ReadInternal(Reader reader) {
			off_matrix = LegacyPointer.GetPointerAtOffset(Offset);
			isIdentity = reader.ReadUInt32(); // if this is 1, don't check the pointer
			objectIndex = reader.ReadByte();
			unk1 = reader.ReadByte();
			unk2 = reader.ReadInt16();
			if (Legacy_Settings.s.game != Legacy_Settings.Game.R2Beta) {
				unk3 = reader.ReadInt16();
				unkByte1 = reader.ReadByte();
				unkByte2 = reader.ReadByte();
				unkUint = reader.ReadUInt32();
			}

			// Read compressed matrix
			if (isIdentity != 1 && isIdentity != 0) {
				LegacyPointer.DoAt(ref reader, off_matrix, () => {
					matrix = Matrix.ReadCompressed(reader, off_matrix);
				});
			}
		}
	}
}
