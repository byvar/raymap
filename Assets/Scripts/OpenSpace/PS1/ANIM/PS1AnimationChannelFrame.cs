using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class PS1AnimationChannelFrame : OpenSpaceStruct {
		public ushort flags;
		public ushort? ntto;
		public ushort? pos;
		public ushort? rot;
		public ushort? hierarchy;
		public ushort? scl;
		public ushort? morphNTTO;
		public ushort? morphProgress;

		protected override void ReadInternal(Reader reader) {
			flags = reader.ReadUInt16();
			if ((flags & ((ushort)AnimationFlags.First)) != 0) {
				ntto = reader.ReadUInt16();
			}
			if ((flags & ((ushort)AnimationFlags.Position)) != 0) {
				pos = reader.ReadUInt16();
			}
			if ((flags & ((ushort)AnimationFlags.Rotation)) != 0) {
				rot = reader.ReadUInt16();
			}
			if ((flags & ((ushort)AnimationFlags.Hierarchy)) != 0) {
				hierarchy = reader.ReadUInt16();
			}
			if ((flags & ((ushort)AnimationFlags.Scale)) != 0) {
				scl = reader.ReadUInt16();
			}
			if ((flags & ((ushort)AnimationFlags.NTTO)) != 0) {
				morphNTTO = reader.ReadUInt16();
				morphProgress = reader.ReadUInt16();
			}
		}

		[Flags]
		public enum AnimationFlags : ushort {
			None = 0,
			First = 1 << 0,
			Position = 1 << 1,
			Rotation = 1 << 2,
			Hierarchy = 1 << 3,
			Scale = 1 << 4,
			NTTO = 1 << 5,
		}
	}
}
