using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_ObjectsTableEntry : U64_Struct {
		public Type TypeOfLinkedObject { get; set; }
		public EntryLinkedObject LinkedObject { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			TypeOfLinkedObject = s.Serialize<Type>(TypeOfLinkedObject, name: nameof(TypeOfLinkedObject));
			s.SerializePadding(2, logIfNotNull: true);
			LinkedObject = TypeOfLinkedObject switch {
				Type.PhysicalObject => s.SerializeObject<PhysicalObject>((PhysicalObject)LinkedObject, name: nameof(LinkedObject)),
				Type.Event => s.SerializeObject<Event>((Event)LinkedObject, name: nameof(LinkedObject)),
				_ => throw new BinarySerializableException(this, $"Invalid {nameof(TypeOfLinkedObject)} value: {TypeOfLinkedObject}")
			};
		}

		public enum Type : ushort {
			PhysicalObject = 0,
			Event = 1
		}

		public abstract class EntryLinkedObject : BinarySerializable { }

		public class PhysicalObject : EntryLinkedObject {
			public U64_GenericReference Entry { get; set; }
			public U64_Index<MTH3D_Vector> CustomZoom { get; set; }
			//public ushort EntryType { get; set; }
			public ushort Flag { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Entry = s.SerializeObject<U64_GenericReference>(Entry,
					onPreSerialize: gr => gr.ImmediateSerializeType = U64_GenericReference.ImmediateSerialize.Index,
					name: nameof(Entry));
				CustomZoom = s.SerializeObject<U64_Index<MTH3D_Vector>>(CustomZoom, name: nameof(CustomZoom))?.SetAction(GAM_Fix.GetGlobalVector3DIndex);
				Entry.SerializeType(s);
				Flag = s.Serialize<ushort>(Flag, name: nameof(Flag));

				Entry.Resolve(s);
			}
		}

		public class Event : EntryLinkedObject {
			public int Target { get; set; } // For sound events: 16 top bits = bank, 16 bottom bits = event pos
			public EventType Type { get; set; }
			public byte FirstCall { get; set; } // 255 = never play
			public byte Period { get; set; }
			public byte Priority { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Target = s.Serialize<int>(Target, name: nameof(Target));
				Type = s.Serialize<EventType>(Type, name: nameof(Type));
				FirstCall = s.Serialize<byte>(FirstCall, name: nameof(FirstCall));
				Period = s.Serialize<byte>(Period, name: nameof(Period));
				Priority = s.Serialize<byte>(Priority, name: nameof(Priority));
			}
			public enum EventType : byte {
				SoundEvent = 0,
				MechanicEvent = 1,
				GenerateEvent = 2,
				GenericEvent = 3
			}
		}
	}
}
