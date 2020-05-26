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
			if (HasFlag(AnimationFlags.NTTO)) {
				ntto = reader.ReadInt16();
			}
			if (HasFlag(AnimationFlags.Position)) {
				pos = reader.ReadUInt16();
			}
			if (HasFlag(AnimationFlags.Rotation)) {
				rot = reader.ReadUInt16();
			}
			if (HasFlag(AnimationFlags.Duration)) {
				extraDuration = reader.ReadInt16();
			}
			if (HasFlag(AnimationFlags.Scale)) {
				scl = reader.ReadUInt16();
			}
			if (HasFlag(AnimationFlags.Morph)) {
				morphNTTO = reader.ReadInt16();
				morphProgress = reader.ReadInt16();
			}
		}

		[Flags]
		public enum AnimationFlags : ushort {
			None = 0,
			NTTO = 1 << 0,
			Position = 1 << 1,
			Rotation = 1 << 2,
			Duration = 1 << 3,
			Scale = 1 << 4,
			Morph = 1 << 5,

			FlipX = 1 << 6
		}

		public bool HasFlag(AnimationFlags flags) {
			return (this.flags & (ushort)flags) == (ushort)flags;
		}
	}
}
