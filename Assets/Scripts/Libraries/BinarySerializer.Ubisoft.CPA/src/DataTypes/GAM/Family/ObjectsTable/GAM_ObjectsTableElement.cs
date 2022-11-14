namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_ObjectsTableElement : BinarySerializable {
		public Pointer<MTH3D_Vector> CustomZoom { get; set; }
		public Pointer<IGAM_ObjectsTableTarget> Target { get; set; }
		public uint ChannelIndex { get; set; }
		public GAM_ObjectsTableTargetType TargetType { get; set; }

		// For LipsSync
		public byte? Phoneme { get; set; } // ASCII form: 'A' + Phoneme
		public byte? Intensity { get; set; }
		public byte? Expression { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			CustomZoom = s.SerializePointer<MTH3D_Vector>(CustomZoom, name: nameof(CustomZoom))?.ResolveObject(s);
			Target = s.SerializePointer<IGAM_ObjectsTableTarget>(Target, name: nameof(Target));
			ChannelIndex = s.Serialize<uint>(ChannelIndex, name: nameof(ChannelIndex));
			TargetType = s.Serialize<GAM_ObjectsTableTargetType>(TargetType, name: nameof(TargetType));

			Phoneme = s.SerializeNullable<byte>(Phoneme, name: nameof(Phoneme));
			Intensity = s.SerializeNullable<byte>(Intensity, name: nameof(Intensity));
			Expression = s.SerializeNullable<byte>(Expression, name: nameof(Expression));
			s.Align(4, Offset);

			Target?.ResolveValue(s, (s, value, name) => {
				return TargetType switch {
					GAM_ObjectsTableTargetType.PhysicalObject => SerializeTarget<PO_PhysicalObject>(s, value, name),
					GAM_ObjectsTableTargetType.Event => SerializeTarget<A3D_EventInTable>(s, value, name),
					_ => throw new BinarySerializableException(this, $"Unparsed TargetType {TargetType}")
				};
			});
		}

		private IGAM_ObjectsTableTarget SerializeTarget<T>(SerializerObject s, IGAM_ObjectsTableTarget value, string name)
			where T : BinarySerializable, IGAM_ObjectsTableTarget, new()
			=> s.SerializeObject<T>((T)value, name: name);

	}
}
