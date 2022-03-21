using System;

namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class ANIM_AnimationKeyframe : BinarySerializable
	{
		public AnimationFlags Flags { get; set; }
		public short NTTO { get; set; }
		public ushort Position { get; set; }
		public ushort Rotation { get; set; }
		public short ExtraDuration { get; set; }
		public ushort Scale { get; set; }
		public short MorphNTTO { get; set; }
		public short MorphProgress { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			Flags = s.Serialize<AnimationFlags>(Flags, name: nameof(Flags));

			if (Flags.HasFlag(AnimationFlags.NTTO))
				NTTO = s.Serialize<short>(NTTO, name: nameof(NTTO));

			if (Flags.HasFlag(AnimationFlags.Position))
				Position = s.Serialize<ushort>(Position, name: nameof(Position));

			if (Flags.HasFlag(AnimationFlags.Rotation))
				Rotation = s.Serialize<ushort>(Rotation, name: nameof(Rotation));

			if (Flags.HasFlag(AnimationFlags.Duration))
				ExtraDuration = s.Serialize<short>(ExtraDuration, name: nameof(ExtraDuration));

			if (Flags.HasFlag(AnimationFlags.Scale))
				Scale = s.Serialize<ushort>(Scale, name: nameof(Scale));

			if (Flags.HasFlag(AnimationFlags.Morph))
			{
				MorphNTTO = s.Serialize<short>(MorphNTTO, name: nameof(MorphNTTO));
				MorphProgress = s.Serialize<short>(MorphProgress, name: nameof(MorphProgress));
			}
		}

		[Flags]
		public enum AnimationFlags : ushort
		{
			None = 0,
			NTTO = 1 << 0,
			Position = 1 << 1,
			Rotation = 1 << 2,
			Duration = 1 << 3,
			Scale = 1 << 4,
			Morph = 1 << 5,
			FlipX = 1 << 6,
		}
	}
}