namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_Character3dData : BinarySerializable {
		public Pointer<GAM_State> InitialState { get; set; }
		public Pointer<GAM_State> CurrentState { get; set; }
		public Pointer<GAM_State> FirstStateOfAction { get; set; }

		public Pointer<GAM_ObjectsTable> InitialObjectsTable { get; set; }
		public Pointer<GAM_ObjectsTable> CurrentObjectsTable { get; set; }

		public Pointer<GAM_Family> Family { get; set; }

		public MAT_Transformation GLIObjectMatrix { get; set; }
		public Pointer<MAT_Transformation> GLIObjectAbsoluteMatrix { get; set; }

		// Current animation
		public FrameIndex CurrentFrame { get; set; }
		public byte RepeatAnimation { get; set; }
		public byte NextEvent { get; set; }
		public Pointer<byte[]> EventActivation { get; set; } // Size: current state -> anim -> events count
		public Pointer<A3D_Hierarchy[]> CurrentHierarchyCouples { get; set; }
		public ushort CurrentHierarchyCouplesCount { get; set; }
		public ushort Elements3dCount { get; set; }

		public Pointer<GAM_State> StateInLastFrame { get; set; }
		public Pointer<GAM_State> WantedState { get; set; }
		public FrameIndex ForcedFrame { get; set; }
		public bool FlagEndState { get; set; }
		public bool FlagEndOfAnim { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			InitialState = s.SerializePointer<GAM_State>(InitialState, name: nameof(InitialState))?.ResolveObject(s);
			CurrentState = s.SerializePointer<GAM_State>(CurrentState, name: nameof(CurrentState))?.ResolveObject(s);
			FirstStateOfAction = s.SerializePointer<GAM_State>(FirstStateOfAction, name: nameof(FirstStateOfAction))?.ResolveObject(s);

			InitialObjectsTable = s.SerializePointer<GAM_ObjectsTable>(InitialObjectsTable, name: nameof(InitialObjectsTable))?.ResolveObject(s);
			CurrentObjectsTable = s.SerializePointer<GAM_ObjectsTable>(CurrentObjectsTable, name: nameof(CurrentObjectsTable))?.ResolveObject(s);

			Family = s.SerializePointer<GAM_Family>(Family, name: nameof(Family))?.ResolveObject(s);

			GLIObjectMatrix = s.SerializeObject<MAT_Transformation>(GLIObjectMatrix, name: nameof(GLIObjectMatrix));
			GLIObjectAbsoluteMatrix = s.SerializePointer<MAT_Transformation>(GLIObjectAbsoluteMatrix, name: nameof(GLIObjectAbsoluteMatrix))?.ResolveObject(s);

			CurrentFrame = s.SerializeObject<FrameIndex>(CurrentFrame, name: nameof(CurrentFrame));
			RepeatAnimation = s.Serialize<byte>(RepeatAnimation, name: nameof(RepeatAnimation));
			NextEvent = s.Serialize<byte>(NextEvent, name: nameof(NextEvent));
			s.Align(4, Offset);
			EventActivation = s.SerializePointer<byte[]>(EventActivation, name: nameof(EventActivation));

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				CurrentHierarchyCouples = s.SerializePointer<A3D_Hierarchy[]>(CurrentHierarchyCouples, name: nameof(CurrentHierarchyCouples));
				CurrentHierarchyCouplesCount = s.Serialize<ushort>(CurrentHierarchyCouplesCount, name: nameof(CurrentHierarchyCouplesCount));
				CurrentHierarchyCouples?.ResolveObjectArray(s, CurrentHierarchyCouplesCount);
			} else {
				throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented for CPA_1");
			}
			Elements3dCount = s.Serialize<ushort>(Elements3dCount, name: nameof(Elements3dCount));
			s.Align(4, Offset);

			StateInLastFrame = s.SerializePointer<GAM_State>(StateInLastFrame, name: nameof(StateInLastFrame))?.ResolveObject(s);
			WantedState = s.SerializePointer<GAM_State>(WantedState, name: nameof(WantedState))?.ResolveObject(s);
			ForcedFrame = s.SerializeObject<FrameIndex>(ForcedFrame, name: nameof(ForcedFrame));
			FlagEndState = s.Serialize<bool>(FlagEndState, name: nameof(FlagEndState));
			FlagEndOfAnim = s.Serialize<bool>(FlagEndOfAnim, name: nameof(FlagEndOfAnim));
			s.Align(4, Offset);

			throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");
		}

		public class FrameIndex : BinarySerializable {
			public byte Frame_CPA1 {
				get => (byte)Frame_CPA2;
				set => Frame_CPA2 = value;
			}
			public ushort Frame_CPA2 { get; set; }
			public float Frame_CPA3 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
					Frame_CPA3 = s.Serialize<float>(Frame_CPA3, name: nameof(Frame_CPA3));
				} else if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
					Frame_CPA2 = s.Serialize<ushort>(Frame_CPA2, name: nameof(Frame_CPA2));
				} else {
					Frame_CPA1 = s.Serialize<byte>(Frame_CPA1, name: nameof(Frame_CPA1));
				}
			}
		}
	}
}
