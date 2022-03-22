namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class PLA_State : BinarySerializable
	{
		public Pointer AnimationIndexPointer { get; set; }
		public short ZoneZDM { get; set; }
		public short ZoneZDE { get; set; }
		public short ZoneZDD { get; set; }
		public short ZoneZDR { get; set; }
		public Pointer TransitionsPointer { get; set; }
		public uint TransitionsCount { get; set; }
		public Pointer AutoStatePointer { get; set; }
		public Pointer Pointer_18 { get; set; }
		public byte Byte_1C { get; set; }
		public byte Speed { get; set; }
		public ushort Ushort_1E { get; set; }

		// Serialized from pointers
		public ANIM_AnimationIndex AnimationIndex { get; set; }
		public PLA_StateTransition[] Transitions { get; set; }
		public PLA_State AutoState { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			CPA_Settings settings = s.GetCPASettings();

			AnimationIndexPointer = s.SerializePointer(AnimationIndexPointer, name: nameof(AnimationIndexPointer));
			ZoneZDM = s.Serialize<short>(ZoneZDM, name: nameof(ZoneZDM));
			ZoneZDE = s.Serialize<short>(ZoneZDE, name: nameof(ZoneZDE));
			ZoneZDD = s.Serialize<short>(ZoneZDD, name: nameof(ZoneZDD));
			ZoneZDR = s.Serialize<short>(ZoneZDR, name: nameof(ZoneZDR));
			TransitionsPointer = s.SerializePointer(TransitionsPointer, name: nameof(TransitionsPointer));
			TransitionsCount = s.Serialize<uint>(TransitionsCount, name: nameof(TransitionsCount));
			AutoStatePointer = s.SerializePointer(AutoStatePointer, name: nameof(AutoStatePointer));

			if (settings.EngineVersion != EngineVersion.RaymanRush_PS1)
				Pointer_18 = s.SerializePointer(Pointer_18, name: nameof(Pointer_18));

			Byte_1C = s.Serialize<byte>(Byte_1C, name: nameof(Byte_1C));
			Speed = s.Serialize<byte>(Speed, name: nameof(Speed));
			Ushort_1E = s.Serialize<ushort>(Ushort_1E, name: nameof(Ushort_1E));

			// Serialize data from pointers
			s.DoAt(AnimationIndexPointer, () => 
				AnimationIndex = s.SerializeObject<ANIM_AnimationIndex>(AnimationIndex, name: nameof(AnimationIndex)));
			s.DoAt(TransitionsPointer, () =>
				Transitions = s.SerializeObjectArray<PLA_StateTransition>(Transitions, TransitionsCount, name: nameof(Transitions)));
			s.DoAt(AutoStatePointer, () =>
				AutoState = s.SerializeObject<PLA_State>(AutoState, name: nameof(AutoState)));
		}
	}
}