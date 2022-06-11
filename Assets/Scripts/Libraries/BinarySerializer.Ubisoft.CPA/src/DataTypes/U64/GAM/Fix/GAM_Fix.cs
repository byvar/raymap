namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_Fix : U64_Struct {
		public U64_ArrayReference<GAM_LevelsNameList> LevelsNameList { get; set; }
		public U64_Reference<IPT_DscInput> DscInput { get; set; }
		public U64_Reference<IPT_InputLink> InputLink { get; set; } // = 3DOS_EntryActions
		public U64_ArrayReference<MTH3D_Vector> Vector3D { get; set; }
		public U64_ArrayReference<U64_TripledIndex> TripledIndex { get; set; }
		public U64_Reference<GAM_GenericMemory> Memory { get; set; }
		public U64_Reference<GLI_Texture> ShadowTexture { get; set; }
		public U64_Reference<GLI_Texture> LightTexture { get; set; }
		public U64_Reference<GLI_VisualMaterial> VisualMaterialController { get; set; } // "No Controller" material
		public U64_ArrayReference<LST_ReferenceElement<GLI_Texture>> NoCtrlList { get; set; } // "No Controller" textures

		public ushort Vector3DCount { get; set; }
		public ushort TripledIndexCount { get; set; }
		public ushort LevelsCount { get; set; }

		public U64_ArrayReference<MTH3D_Vector> GlobalVector3D { get; set; }
		public U64_ArrayReference<U64_TripledIndex> GlobalTripledIndex { get; set; }
		public ushort GlobalVector3DCount { get; set; }
		public ushort GlobalTripledIndexCount { get; set; }

		public U64_Reference<GLI_CPakFont> PakFont { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			// No resolving in this. Only load what's absolutely necessary, because this is loaded to create the level list.

			LevelsNameList = s.SerializeObject<U64_ArrayReference<GAM_LevelsNameList>>(LevelsNameList, name: nameof(LevelsNameList));
			DscInput = s.SerializeObject<U64_Reference<IPT_DscInput>>(DscInput, name: nameof(DscInput));
			InputLink = s.SerializeObject<U64_Reference<IPT_InputLink>>(InputLink, name: nameof(InputLink));
			Vector3D = s.SerializeObject<U64_ArrayReference<MTH3D_Vector>>(Vector3D, name: nameof(Vector3D));
			TripledIndex = s.SerializeObject<U64_ArrayReference<U64_TripledIndex>>(TripledIndex, name: nameof(TripledIndex));
			Memory = s.SerializeObject<U64_Reference<GAM_GenericMemory>>(Memory, name: nameof(Memory));
			ShadowTexture = s.SerializeObject<U64_Reference<GLI_Texture>>(ShadowTexture, name: nameof(ShadowTexture));
			LightTexture = s.SerializeObject<U64_Reference<GLI_Texture>>(LightTexture, name: nameof(LightTexture));
			VisualMaterialController = s.SerializeObject<U64_Reference<GLI_VisualMaterial>>(VisualMaterialController, name: nameof(VisualMaterialController));
			NoCtrlList = s.SerializeObject<U64_ArrayReference<LST_ReferenceElement<GLI_Texture>>>(NoCtrlList, name: nameof(NoCtrlList));

			Vector3DCount = s.Serialize<ushort>(Vector3DCount, name: nameof(Vector3DCount));
			TripledIndexCount = s.Serialize<ushort>(TripledIndexCount, name: nameof(TripledIndexCount));
			LevelsCount = s.Serialize<ushort>(LevelsCount, name: nameof(LevelsCount));

			GlobalVector3D = s.SerializeObject<U64_ArrayReference<MTH3D_Vector>>(GlobalVector3D, name: nameof(GlobalVector3D));
			GlobalTripledIndex = s.SerializeObject<U64_ArrayReference<U64_TripledIndex>>(GlobalTripledIndex, name: nameof(GlobalTripledIndex));
			GlobalVector3DCount = s.Serialize<ushort>(GlobalVector3DCount, name: nameof(GlobalVector3DCount));
			GlobalTripledIndexCount = s.Serialize<ushort>(GlobalTripledIndexCount, name: nameof(GlobalTripledIndexCount));

			PakFont = s.SerializeObject<U64_Reference<GLI_CPakFont>>(PakFont, name: nameof(PakFont));

			// Resolve level list
			LevelsNameList.Resolve(s, LevelsCount, isInFixFixFat: true);
		}

		public GAM_Fix ResolveLevelReferences(SerializerObject s) {
			// Global references
			GlobalVector3D.Resolve(s, GlobalVector3DCount, isInFixFixFat: true);
			GlobalTripledIndex.Resolve(s, GlobalTripledIndexCount, isInFixFixFat: true);
			// Level specific references
			Vector3D.Resolve(s, Vector3DCount);
			TripledIndex.Resolve(s, TripledIndexCount);
			
			DscInput?.Resolve(s);
			InputLink?.Resolve(s);
			Memory?.Resolve(s);
			ShadowTexture?.Resolve(s);
			LightTexture?.Resolve(s);
			VisualMaterialController?.Resolve(s);
			NoCtrlList?.Resolve(s, 5);
			PakFont?.Resolve(s);

			return this;
		}

		// Index resolve actions
		public static MTH3D_Vector GetVector3DIndex(U64_Index<MTH3D_Vector> index) => index.Context.GetLoader().Fix?.Value?.Vector3D?.Value[index.Index];
		public static MTH3D_Vector GetGlobalVector3DIndex(U64_Index<MTH3D_Vector> index) => index.Context.GetLoader().Fix?.Value?.GlobalVector3D?.Value[index.Index];
	}
}
