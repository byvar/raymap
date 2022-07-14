namespace BinarySerializer.Ubisoft.CPA {
	/// <summary>
	/// Event data in object table
	/// </summary>
	public class A3D_EventInTable : BinarySerializable {
		public uint Event { get; set; }
		public Pointer<SND_BlockEvent> SoundEvent { get; set; }
		public A3D_GenericEvent GenericEvent { get; set; }
		public MEC_RequestType MechanicEvent { get; set; }

		public A3D_EventType Type { get; set; }
		public byte Priority { get; set; }
		public byte FirstCall { get; set; } // Amount of times the animation loops before the event. 255 = never play
		public byte Period { get; set; } // Amount of loops after FirstCall
		public uint BinaryEventId { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Event = s.Serialize<uint>(Event, name: nameof(Event));
			Type = s.Serialize<A3D_EventType>(Type, name: nameof(Type));
			Priority = s.Serialize<byte>(Priority, name: nameof(Priority));
			FirstCall = s.Serialize<byte>(FirstCall, name: nameof(FirstCall));
			Period = s.Serialize<byte>(Period, name: nameof(Period));
			BinaryEventId = s.Serialize<uint>(BinaryEventId, name: nameof(BinaryEventId));

			switch (Type) {
				case A3D_EventType.Generic:
					s.DoAt(Offset, () => {
						GenericEvent = s.Serialize<A3D_GenericEvent>(GenericEvent, name: nameof(GenericEvent));
					});
					break;
				case A3D_EventType.Generate:
					break;
				case A3D_EventType.Mechanic:
					s.DoAt(Offset, () => {
						MechanicEvent = s.Serialize<MEC_RequestType>(MechanicEvent, name: nameof(MechanicEvent));
					});
					break;
				case A3D_EventType.Sound:
					/*s.DoAt(Offset, () => {
						SoundEvent = s.SerializePointer<SND_BlockEvent>(SoundEvent, name: nameof(SoundEvent));
					});*/
					// not saved in binary. for binary the BinaryEventid is used instead
					break;
			}
		}
	}
}