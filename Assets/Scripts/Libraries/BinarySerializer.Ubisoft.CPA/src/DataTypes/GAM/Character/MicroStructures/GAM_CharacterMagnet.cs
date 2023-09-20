namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_CharacterMagnet : BinarySerializable {
		public const int MaxModifiedObjectsCount = 20;

		public float Far { get; set; }
		public float Near { get; set; }
		public float Strength { get; set; }
		public MGT_MagnetStatus Status { get; set; }

		public MTH3D_Vector Position { get; set; }
		public uint Duration { get; set; } // In milliseconds

		public short MaxIndexOfMagnetModification { get; set; }
		public Pointer<MGT_MagnetModification>[] MagnetModifications { get; set; }

		public MGT_MagnetModifiedFlags ModifiedFlags { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			Far = s.Serialize<float>(Far, name: nameof(Far));
			Near = s.Serialize<float>(Near, name: nameof(Near));
			Strength = s.Serialize<float>(Strength, name: nameof(Strength));
			Status = s.Serialize<MGT_MagnetStatus>(Status, name: nameof(Status));

			Position = s.SerializeObject<MTH3D_Vector>(Position, name: nameof(Position));
			Duration = s.Serialize<uint>(Duration, name: nameof(Duration));

			MaxIndexOfMagnetModification = s.Serialize<short>(MaxIndexOfMagnetModification, name: nameof(MaxIndexOfMagnetModification));
			MagnetModifications = s.SerializePointerArray<MGT_MagnetModification>(MagnetModifications, MaxModifiedObjectsCount, name: nameof(MagnetModifications))?.ResolveObject(s);

			ModifiedFlags = s.Serialize<MGT_MagnetModifiedFlags>(ModifiedFlags, name: nameof(ModifiedFlags));
		}
	}
}
