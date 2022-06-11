using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_LevelEntry : U64_Struct {
		public U64_Reference<GAM_Character> Character { get; set; }
		public POS_CompletePosition Matrix { get; set; }
		public LevelEntryFlags Flags { get; set; }
		public byte Transparency { get; set; }
		public HIE_SuperObjectFlags CharacterFlags { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Character = s.SerializeObject<U64_Reference<GAM_Character>>(Character, name: nameof(Character))?.Resolve(s);
			Matrix = s.SerializeObject<POS_CompletePosition>(Matrix, name: nameof(Matrix));
			Flags = s.Serialize<LevelEntryFlags>(Flags, name: nameof(Flags));
			Transparency = s.Serialize<byte>(Transparency, name: nameof(Transparency));
			s.SerializePadding(1, logIfNotNull: true);
			CharacterFlags = s.Serialize<HIE_SuperObjectFlags>(CharacterFlags, name: nameof(CharacterFlags));
		}

		[Flags]
		public enum LevelEntryFlags : ushort {
			None = 0,
			PrincipalCharacter = 1,
			CharacterLaunchingSounds = 2,
			CharacterAlwaysActive = 4,
			StandardCamera = 32,
		}
	}
}
