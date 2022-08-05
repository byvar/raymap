using System;

namespace BinarySerializer.Ubisoft.CPA {
	public class HIE_SuperObject : BinarySerializable, ILST2_Child<HIE_SuperObject, HIE_SuperObject> {
		public HIE_ObjectType LinkedObjectType { get; set; }
		public HIE_ObjectType_98 LinkedObjectType_98 { get; set; }
		public Pointer<IHIE_LinkedObject> LinkedObject { get; set; }
		public LST2_DynamicParentElement<HIE_SuperObject, HIE_SuperObject> Children { get; set; }
		public LST2_DynamicChildElement<HIE_SuperObject, HIE_SuperObject> ListElement { get; set; }
		public Pointer<MAT_Transformation> LocalMatrix { get; set; }
		public Pointer<MAT_Transformation> GlobalMatrix { get; set; }
		public int LastComputeFrame { get; set; }
		public GLI_DrawMask SuperObjectDrawMask { get; set; }
		public HIE_SuperObjectFlags Flags { get; set; }
		public Pointer<ICOL_BoundingVolume> BoundingVolume { get; set; }
		public Pointer<COL_BoundingSphere> VisualBoundingVolume { get; set; }
		public Pointer<ICOL_BoundingVolume> CollideBoundingVolume { get; set; }
		public float Transparency { get; set; }
		public MTH3D_Vector SemiLookAtVector { get; set; }

		public uint R2iOSUnknown { get; set; }

		// CPA_3
		public GLI_RGBA8888Color OutlineColor { get; set; }
		public int DisplayPriority { get; set; }
		// Internal Light
		public int IL_Status { get; set; }
		public MTH3D_Vector AmbientColor { get; set; }
		public MTH3D_Vector ParallelDirection { get; set; }
		public MTH3D_Vector ParallelColor { get; set; }

		public byte SuperImposedOnWhichViewport { get; set; }
		public bool IsSPO { get; set; }
		public byte Transition { get; set; }

		// LST2 Implementation
		public Pointer<HIE_SuperObject> LST2_Parent => ListElement?.Father;
		public Pointer<HIE_SuperObject> LST2_Next => ListElement?.LST2_Next;
		public Pointer<HIE_SuperObject> LST2_Previous => ListElement?.LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				LinkedObjectType = s.Serialize<HIE_ObjectType>(LinkedObjectType, name: nameof(LinkedObjectType));
			} else {
				LinkedObjectType_98 = s.Serialize<HIE_ObjectType_98>(LinkedObjectType_98, name: nameof(LinkedObjectType_98));
			}
			LinkedObject = s.SerializePointer<IHIE_LinkedObject>(LinkedObject, name: nameof(LinkedObject))
				?.ResolveValue(s, SerializeLinkedObject());
			Children = s.SerializeObject<LST2_DynamicParentElement<HIE_SuperObject, HIE_SuperObject>>(Children, name: nameof(Children))?.Resolve(s, name: nameof(Children));
			ListElement = s.SerializeObject<LST2_DynamicChildElement<HIE_SuperObject, HIE_SuperObject>>(ListElement, name: nameof(ListElement))?.Resolve(s);
			LocalMatrix = s.SerializePointer<MAT_Transformation>(LocalMatrix, name: nameof(LocalMatrix))?.ResolveObject(s);
			GlobalMatrix = s.SerializePointer<MAT_Transformation>(GlobalMatrix, name: nameof(GlobalMatrix))?.ResolveObject(s);
			LastComputeFrame = s.Serialize<int>(LastComputeFrame, name: nameof(LastComputeFrame));
			SuperObjectDrawMask = s.Serialize<GLI_DrawMask>(SuperObjectDrawMask, name: nameof(SuperObjectDrawMask));
			Flags = s.Serialize<HIE_SuperObjectFlags>(Flags, name: nameof(Flags));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				VisualBoundingVolume = s.SerializePointer<COL_BoundingSphere>(VisualBoundingVolume, name: nameof(VisualBoundingVolume))?.ResolveObject(s);
				CollideBoundingVolume = s.SerializePointer<ICOL_BoundingVolume>(CollideBoundingVolume, name: nameof(CollideBoundingVolume))
					?.ResolveValue(s, SerializeBoundingVolume());

				SemiLookAtVector = s.SerializeObject<MTH3D_Vector>(SemiLookAtVector, name: nameof(SemiLookAtVector));
				Transparency = s.Serialize<float>(Transparency, name: nameof(Transparency));

				OutlineColor = s.SerializeObject<GLI_RGBA8888Color>(OutlineColor, name: nameof(OutlineColor));
				DisplayPriority = s.Serialize<int>(DisplayPriority, name: nameof(DisplayPriority));

				IL_Status = s.Serialize<int>(IL_Status, name: nameof(IL_Status));
				AmbientColor = s.SerializeObject<MTH3D_Vector>(AmbientColor, name: nameof(AmbientColor));
				ParallelDirection = s.SerializeObject<MTH3D_Vector>(ParallelDirection, name: nameof(ParallelDirection));
				ParallelColor = s.SerializeObject<MTH3D_Vector>(ParallelColor, name: nameof(ParallelColor));

				SuperImposedOnWhichViewport = s.Serialize<byte>(SuperImposedOnWhichViewport, name: nameof(SuperImposedOnWhichViewport));
				IsSPO = s.Serialize<bool>(IsSPO, name: nameof(IsSPO));
				Transition = s.Serialize<byte>(Transition, name: nameof(Transition));
				s.Align(4, Offset);
			} else {
				BoundingVolume = s.SerializePointer<ICOL_BoundingVolume>(BoundingVolume, name: nameof(BoundingVolume))
					?.ResolveValue(s, SerializeBoundingVolume());

				Transparency = s.Serialize<float>(Transparency, name: nameof(Transparency));
				SemiLookAtVector = s.SerializeObject<MTH3D_Vector>(SemiLookAtVector, name: nameof(SemiLookAtVector));

				if(s.GetCPASettings().Platform == Platform.iOS)
					R2iOSUnknown = s.Serialize<uint>(R2iOSUnknown, name: nameof(R2iOSUnknown));
			}
		}

		private PointerFunctions.SerializeFunction<IHIE_LinkedObject> SerializeLinkedObject(Action<IHIE_LinkedObject> onPreSerialize = null) {
			return (s, value, name) => {
				T SerializeObject<T>() where T : BinarySerializable, IHIE_LinkedObject, new() {
					return s.SerializeObject<T>((T)value, onPreSerialize: onPreSerialize, name: name);
				}

				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
					return LinkedObjectType switch {
						HIE_ObjectType.SuperObject => null,
						HIE_ObjectType.Actor => SerializeObject<GAM_EngineObject>(),
						HIE_ObjectType.Sector => SerializeObject<SCT_Sector>(),
						HIE_ObjectType.PO => SerializeObject<PO_PhysicalObject>(),
						HIE_ObjectType.PO_Mirror => SerializeObject<PO_PhysicalObject>(),
						HIE_ObjectType.IPO => SerializeObject<IPO_InstanciatedPhysicalObject>(),
						HIE_ObjectType.IPO_Mirror => SerializeObject<IPO_InstanciatedPhysicalObject>(),
						HIE_ObjectType.SpecialEffect => SerializeObject<GEO_GeometricObject>(),
						HIE_ObjectType.NoAction => null,
						HIE_ObjectType.Mirror => SerializeObject<GEO_GeometricObject>(),
						HIE_ObjectType.EDT_Geometric => SerializeObject<GEO_GeometricObject>(),
						_ => throw new BinarySerializableException(this, $"Unsupported LinkedObjectType {LinkedObjectType}")
					};
				} else {
					return LinkedObjectType_98 switch {
						HIE_ObjectType_98.SuperObject => null,
						HIE_ObjectType_98.Character => SerializeObject<GAM_EngineObject>(),
						HIE_ObjectType_98.Sector => SerializeObject<SCT_Sector>(),
						HIE_ObjectType_98.InstanciatedPhysicalObject => SerializeObject<IPO_InstanciatedPhysicalObject>(),
						_ => throw new BinarySerializableException(this, $"Unsupported LinkedObjectType {LinkedObjectType_98}")
					};
				}
			};
		}

		private PointerFunctions.SerializeFunction<ICOL_BoundingVolume> SerializeBoundingVolume(Action<ICOL_BoundingVolume> onPreSerialize = null) {
			return (s, value, name) => {
				T SerializeObject<T>() where T : BinarySerializable, ICOL_BoundingVolume, new() {
					return s.SerializeObject<T>((T)value, onPreSerialize: onPreSerialize, name: name);
				}

				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
					if (Flags.HasFlag(HIE_SuperObjectFlags.TypeOfBoundingVolume)) {
						return SerializeObject<COL_BoundingSphere>();
					} else {
						return SerializeObject<COL_ParallelBox>();
					}
				} else {
					return SerializeObject<COL_CollideObject>();
				}
			};
		}
	}
}
