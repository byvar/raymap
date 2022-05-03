using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_CharacterCineInfo : U64_Struct {
		public MTH3D_Vector ShiftTarget { get; set; }
		public MTH3D_Vector ShiftPosition { get; set; }
		public Speed LinearSpeed { get; set; }
		public Speed AngularSpeed { get; set; }
		public Speed TargetSpeed { get; set; }
		public MinMax Distance { get; set; }
		public MinMax BoundDistance { get; set; }
		public MinMax Z { get; set; }
		public Angle Alpha { get; set; }
		public Angle Theta { get; set; }
		public float Focal { get; set; }
		public ushort DNMFlags { get; set; }
		public ushort AIFlags { get; set; }
		public ushort Viewport { get; set; }
		public ushort Activation { get; set; }
		public short Channel { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ShiftTarget = s.SerializeObject<MTH3D_Vector>(ShiftTarget, name: nameof(ShiftTarget));
			ShiftPosition = s.SerializeObject<MTH3D_Vector>(ShiftPosition, name: nameof(ShiftPosition));
			LinearSpeed = s.SerializeObject<Speed>(LinearSpeed, name: nameof(LinearSpeed));
			AngularSpeed = s.SerializeObject<Speed>(AngularSpeed, name: nameof(AngularSpeed));
			TargetSpeed = s.SerializeObject<Speed>(TargetSpeed, name: nameof(TargetSpeed));
			Distance = s.SerializeObject<MinMax>(Distance, name: nameof(Distance));
			BoundDistance = s.SerializeObject<MinMax>(BoundDistance, name: nameof(BoundDistance));
			Z = s.SerializeObject<MinMax>(Z, name: nameof(Z));
			Alpha = s.SerializeObject<Angle>(Alpha, name: nameof(Alpha));
			Theta = s.SerializeObject<Angle>(Theta, name: nameof(Theta));
			Focal = s.Serialize<float>(Focal, name: nameof(Focal));
			DNMFlags = s.Serialize<ushort>(DNMFlags, name: nameof(DNMFlags));
			AIFlags = s.Serialize<ushort>(AIFlags, name: nameof(AIFlags));
			Viewport = s.Serialize<ushort>(Viewport, name: nameof(Viewport));
			Activation = s.Serialize<ushort>(Activation, name: nameof(Activation));
			Channel = s.Serialize<short>(Channel, name: nameof(Channel));
			s.SerializePadding(2, logIfNotNull: true);
		}

		public class Speed : BinarySerializable {
			public float Value { get; set; }
			public float ValueIncrease { get; set; }
			public float ValueDecrease { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Value = s.Serialize<float>(Value, name: nameof(Value));
				ValueIncrease = s.Serialize<float>(ValueIncrease, name: nameof(ValueIncrease));
				ValueDecrease = s.Serialize<float>(ValueDecrease, name: nameof(ValueDecrease));
			}
		}
		public class MinMax : BinarySerializable {
			public float Min { get; set; }
			public float Max { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Min = s.Serialize<float>(Min, name: nameof(Min));
				Max = s.Serialize<float>(Max, name: nameof(Max));
			}
		}
		public class Angle : BinarySerializable {
			public float Value { get; set; }
			public float ValueShift { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Value = s.Serialize<float>(Value, name: nameof(Value));
				ValueShift = s.Serialize<float>(ValueShift, name: nameof(ValueShift));
			}
		}
	}
}
