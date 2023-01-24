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
		public ushort ElementsCount { get; set; }

		public Pointer<GAM_State> StateInLastFrame { get; set; }
		public Pointer<GAM_State> WantedState { get; set; }
		public FrameIndex ForcedFrame { get; set; }
		public bool FlagEndState { get; set; }
		public bool FlagEndOfAnim { get; set; }

		public Pointer<A3D_Runtime_Channel[]> Channels { get; set; }
		public uint ChannelsCount { get; set; }
		public Pointer<A3D_Runtime_Channel> FirstActiveChannel { get; set; }
		public A3D_Runtime_Frame Frame { get; set; }

		public Pointer<LST2_DynamicList<A3D_Runtime_MorphChannel>> MorphChannels { get; set; }

		public uint StartTime { get; set; }
		public uint TimeDelay { get; set; }
		public uint TimePreviousFrame { get; set; }
		public FrameIndex LastFrame { get; set; }
		public bool IsStateJustModified { get; set; }
		public bool SkipCurrentFrame { get; set; }

		public Pointer<GMT_GameMaterial> ShadowGameMaterial { get; set; }
		public Pointer<GLI_Material> ShadowVisualMaterial { get; set; }
		public Pointer<GLI_Texture> ShadowTexture { get; set; }
		public MTH2D_Vector ShadowScale { get; set; }
		public short ShadowQuality { get; set; }
		public ushort EngineFramesSinceLastMechEvent { get; set; }
		public byte FrameRate { get; set; }
		public bool FlagModifState { get; set; }

		public GLI_DrawMask DrawMaskInit { get; set; }
		public GLI_DrawMask DrawMask { get; set; }

		public int LastComputeFrame { get; set; }
		public MTH3D_Vector LastEventGlobalPosition { get; set; }
		public MAT_Transformation LastEventGlobalTransformation { get; set; }

		public bool IsAnimMatrixChanged { get; set; }
		public byte UserEventFlags { get; set; }
		public byte BrainComputationFrequency { get; set; }
		public sbyte BrainCounter { get; set; }
		public ushort MainBrainCounter { get; set; }
		public byte Transparency { get; set; }

		public bool DiscreetSpeed { get; set; }
		public MTH3D_Vector LightDirectionForDynamicShadow { get; set; }
		public byte IsShadowClosed { get; set; }
		public byte ShadowRecLevel { get; set; }

		public byte LightComputationFrequency { get; set; }
		public sbyte LightCounter { get; set; }

		public MTH3D_Vector ShadowDeformationVector { get; set; }
		public float ShadowHeight { get; set; }

		public float DistNoInterpol { get; set; }
		public float DistNoMorphing { get; set; }
		public float DistNoDEnv { get; set; }
		public LST2_DynamicList<GAM_SubAnimationInUse> SubAnimsInUse { get; set; }
		public Pointer<GAM_Placeholder> CurrentCineActor { get; set; }
		public Pointer<GLI_Material>[] SkinMaterials { get; set; }
		public uint SkinMaterialsCount { get; set; }
		public short[] SkinIndices { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			InitialState = s.SerializePointer<GAM_State>(InitialState, name: nameof(InitialState))?.ResolveObject(s);
			CurrentState = s.SerializePointer<GAM_State>(CurrentState, name: nameof(CurrentState))?.ResolveObject(s);
			FirstStateOfAction = s.SerializePointer<GAM_State>(FirstStateOfAction, name: nameof(FirstStateOfAction))?.ResolveObject(s);

			InitialObjectsTable = s.SerializePointer<GAM_ObjectsTable>(InitialObjectsTable, name: nameof(InitialObjectsTable))?.ResolveObject(s);
			CurrentObjectsTable = s.SerializePointer<GAM_ObjectsTable>(CurrentObjectsTable, name: nameof(CurrentObjectsTable))?.ResolveObject(s);

			Family = s.SerializePointer<GAM_Family>(Family, name: nameof(Family))?.ResolveObject(s);

			if (s.GetCPASettings().IsPressDemo) {
				GLIObjectAbsoluteMatrix = s.SerializePointer<MAT_Transformation>(GLIObjectAbsoluteMatrix, name: nameof(GLIObjectAbsoluteMatrix))?.ResolveObject(s);
				GLIObjectMatrix = s.SerializeObject<MAT_Transformation>(GLIObjectMatrix, name: nameof(GLIObjectMatrix));
			} else {
				GLIObjectMatrix = s.SerializeObject<MAT_Transformation>(GLIObjectMatrix, name: nameof(GLIObjectMatrix));
				GLIObjectAbsoluteMatrix = s.SerializePointer<MAT_Transformation>(GLIObjectAbsoluteMatrix, name: nameof(GLIObjectAbsoluteMatrix))?.ResolveObject(s);
			}
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
			ElementsCount = s.Serialize<ushort>(ElementsCount, name: nameof(ElementsCount));
			s.Align(4, Offset);

			StateInLastFrame = s.SerializePointer<GAM_State>(StateInLastFrame, name: nameof(StateInLastFrame))?.ResolveObject(s);
			WantedState = s.SerializePointer<GAM_State>(WantedState, name: nameof(WantedState))?.ResolveObject(s);
			ForcedFrame = s.SerializeObject<FrameIndex>(ForcedFrame, name: nameof(ForcedFrame));
			FlagEndState = s.Serialize<bool>(FlagEndState, name: nameof(FlagEndState));
			FlagEndOfAnim = s.Serialize<bool>(FlagEndOfAnim, name: nameof(FlagEndOfAnim));
			s.Align(4, Offset);

			Channels = s.SerializePointer<A3D_Runtime_Channel[]>(Channels, name: nameof(Channels)); // C(hannel)SOList
			ChannelsCount = s.Serialize<uint>(ChannelsCount, name: nameof(ChannelsCount));
			Channels?.ResolveObjectArray(s, ChannelsCount);
			FirstActiveChannel = s.SerializePointer<A3D_Runtime_Channel>(FirstActiveChannel, name: nameof(FirstActiveChannel));
			Frame = s.SerializeObject<A3D_Runtime_Frame>(Frame, onPreSerialize: f => f.Pre_ElementsCount = ElementsCount, name: nameof(Frame));

			MorphChannels = s.SerializePointer<LST2_DynamicList<A3D_Runtime_MorphChannel>>(MorphChannels, name: nameof(MorphChannels))
				?.ResolveObject(s);
			MorphChannels?.Value?.Resolve(s, name: nameof(MorphChannels));

			StartTime = s.Serialize<uint>(StartTime, name: nameof(StartTime));
			TimeDelay = s.Serialize<uint>(TimeDelay, name: nameof(TimeDelay));
			TimePreviousFrame = s.Serialize<uint>(TimePreviousFrame, name: nameof(TimePreviousFrame));
			LastFrame = s.SerializeObject<FrameIndex>(LastFrame, onPreSerialize: f => f.Pre_IsLastFrame = true, name: nameof(LastFrame));
			IsStateJustModified = s.Serialize<bool>(IsStateJustModified, name: nameof(IsStateJustModified));
			SkipCurrentFrame = s.Serialize<bool>(SkipCurrentFrame, name: nameof(SkipCurrentFrame));
			s.Align(4, Offset);

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)
				|| s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2Revolution)) {
				ShadowVisualMaterial = s.SerializePointer<GLI_Material>(ShadowVisualMaterial, name: nameof(ShadowVisualMaterial))?.ResolveObject(s);
			} else {
				ShadowGameMaterial = s.SerializePointer<GMT_GameMaterial>(ShadowGameMaterial, name: nameof(ShadowGameMaterial))?.ResolveObject(s);
			}
			ShadowTexture = s.SerializePointer<GLI_Texture>(ShadowTexture, name: nameof(ShadowTexture))?.ResolveObject(s);
			ShadowScale = s.SerializeObject<MTH2D_Vector>(ShadowScale, name: nameof(ShadowScale));
			ShadowQuality = s.Serialize<short>(ShadowQuality, name: nameof(ShadowQuality));

			EngineFramesSinceLastMechEvent = s.Serialize<ushort>(EngineFramesSinceLastMechEvent, name: nameof(EngineFramesSinceLastMechEvent));
			FrameRate = s.Serialize<byte>(FrameRate, name: nameof(FrameRate));
			FlagModifState = s.Serialize<bool>(FlagModifState, name: nameof(FlagModifState));
			s.Align(4, Offset);

			DrawMaskInit = s.Serialize<GLI_DrawMask>(DrawMaskInit, name: nameof(DrawMaskInit));
			DrawMask = s.Serialize<GLI_DrawMask>(DrawMask, name: nameof(DrawMask));
			LastComputeFrame = s.Serialize<int>(LastComputeFrame, name: nameof(LastComputeFrame));

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				LastEventGlobalTransformation = s.SerializeObject<MAT_Transformation>(LastEventGlobalTransformation, name: nameof(LastEventGlobalTransformation));
			} else {
				LastEventGlobalPosition = s.SerializeObject<MTH3D_Vector>(LastEventGlobalPosition, name: nameof(LastEventGlobalPosition));
			}

			IsAnimMatrixChanged = s.Serialize<bool>(IsAnimMatrixChanged, name: nameof(IsAnimMatrixChanged));
			UserEventFlags = s.Serialize<byte>(UserEventFlags, name: nameof(UserEventFlags));
			BrainComputationFrequency = s.Serialize<byte>(BrainComputationFrequency, name: nameof(BrainComputationFrequency));
			BrainCounter = s.Serialize<sbyte>(BrainCounter, name: nameof(BrainCounter));
			MainBrainCounter = s.Serialize<ushort>(MainBrainCounter, name: nameof(MainBrainCounter));
			Transparency = s.Serialize<byte>(Transparency, name: nameof(Transparency));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				DiscreetSpeed = s.Serialize<bool>(DiscreetSpeed, name: nameof(DiscreetSpeed));
				s.Align(4, Offset);
				LightDirectionForDynamicShadow = s.SerializeObject<MTH3D_Vector>(LightDirectionForDynamicShadow, name: nameof(LightDirectionForDynamicShadow));
				IsShadowClosed = s.Serialize<byte>(IsShadowClosed, name: nameof(IsShadowClosed));
				ShadowRecLevel = s.Serialize<byte>(ShadowRecLevel, name: nameof(ShadowRecLevel));
			}

			LightComputationFrequency = s.Serialize<byte>(LightComputationFrequency, name: nameof(LightComputationFrequency));
			LightCounter = s.Serialize<sbyte>(LightCounter, name: nameof(LightCounter));

			s.Align(4, Offset);
			ShadowDeformationVector = s.SerializeObject<MTH3D_Vector>(ShadowDeformationVector, name: nameof(ShadowDeformationVector));
			ShadowHeight = s.Serialize<float>(ShadowHeight, name: nameof(ShadowHeight));

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				DistNoInterpol = s.Serialize<float>(DistNoInterpol, name: nameof(DistNoInterpol));
				DistNoMorphing = s.Serialize<float>(DistNoMorphing, name: nameof(DistNoMorphing));
				DistNoDEnv = s.Serialize<float>(DistNoDEnv, name: nameof(DistNoDEnv));
				SubAnimsInUse = s.SerializeObject<LST2_DynamicList<GAM_SubAnimationInUse>>(SubAnimsInUse, name: nameof(SubAnimsInUse))?.Resolve(s, name: nameof(SubAnimsInUse));
				CurrentCineActor = s.SerializePointer<GAM_Placeholder>(CurrentCineActor, name: nameof(CurrentCineActor))?.ResolveObject(s);
				SkinMaterials = s.SerializePointerArray<GLI_Material>(SkinMaterials, 4, name: nameof(SkinMaterials));
				SkinMaterialsCount = s.Serialize<uint>(SkinMaterialsCount, name: nameof(SkinMaterialsCount));
				SkinIndices = s.SerializeArray<short>(SkinIndices, 4, name: nameof(SkinIndices));
			}
		}

		public class FrameIndex : BinarySerializable {
			public byte Frame_CPA1 {
				get => (byte)Frame_CPA2;
				set => Frame_CPA2 = value;
			}
			public ushort Frame_CPA2 { get; set; }
			public float Frame_CPA3 { get; set; }

			public bool Pre_IsLastFrame { get; set; } = false;

			public override void SerializeImpl(SerializerObject s) {
				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
					Frame_CPA3 = s.Serialize<float>(Frame_CPA3, name: nameof(Frame_CPA3));
				} else if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2) || Pre_IsLastFrame) {
					Frame_CPA2 = s.Serialize<ushort>(Frame_CPA2, name: nameof(Frame_CPA2));
				} else {
					Frame_CPA1 = s.Serialize<byte>(Frame_CPA1, name: nameof(Frame_CPA1));
				}
			}
		}
	}
}
