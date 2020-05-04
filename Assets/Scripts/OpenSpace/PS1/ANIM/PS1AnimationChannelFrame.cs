using OpenSpace.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class PS1AnimationKeyframe : OpenSpaceStruct {
		public ushort flags;
		public short? ntto;
		public ushort? pos;
		public ushort? rot;
		public short? extraDuration;
		public ushort? scl;
		public short? morphNTTO;
		public short? morphProgress;

		protected override void ReadInternal(Reader reader) {
			flags = reader.ReadUInt16();
			if ((flags & ((ushort)AnimationFlags.First)) != 0) {
				ntto = reader.ReadInt16();
			}
			if ((flags & ((ushort)AnimationFlags.Position)) != 0) {
				pos = reader.ReadUInt16();
			}
			if ((flags & ((ushort)AnimationFlags.Rotation)) != 0) {
				rot = reader.ReadUInt16();
			}
			if ((flags & ((ushort)AnimationFlags.Duration)) != 0) {
				extraDuration = reader.ReadInt16();
			}
			if ((flags & ((ushort)AnimationFlags.Scale)) != 0) {
				scl = reader.ReadUInt16();
				R2PS1Loader l = Load as R2PS1Loader;
				if(scl > l.maxScaleVector) l.maxScaleVector = scl.Value;
			}
			if ((flags & ((ushort)AnimationFlags.NTTO)) != 0) {
				morphNTTO = reader.ReadInt16();
				morphProgress = reader.ReadInt16();
			}
		}

		[Flags]
		public enum AnimationFlags : ushort {
			None = 0,
			First = 1 << 0,
			Position = 1 << 1,
			Rotation = 1 << 2,
			Duration = 1 << 3,
			Scale = 1 << 4,
			NTTO = 1 << 5,
		}
	}
}
