using System;

namespace BinarySerializer.Ubisoft.CPA {
	public class AI_DsgVarValue : BinarySerializable {
		// OnPreSerialize
		public bool Pre_IsArrayEntry { get; set; }
		public AI_DsgVarType Pre_LinkedType { get; set; }

		// Value types
		public bool ValueBoolean { get; set; }
		public sbyte ValueSByte { get; set; }
		public byte ValueUByte { get; set; }
		public short ValueShort { get; set; }
		public ushort ValueUShort { get; set; }
		public byte ValueListMaxSize { get; set; }
		public int ValueInt { get; set; }
		public uint ValueUInt { get; set; }
		public float ValueFloat { get; set; }
		public MTH3D_Vector ValueVector { get; set; }
		public Pointer<WAY_WayPoint> ValueWayPoint { get; set; }
		public Pointer<HIE_SuperObject> ValueSuperObject { get; set; }
		public GAM_EngineObject ValuePerso {
			get => ValueSuperObject?.Value?.LinkedObject?.Value as GAM_EngineObject;
			set => ValueSuperObject = value.StandardGame?.Value?.SuperObject;
		}
		public AI_DsgVarList ValueList { get; set; }
		public Pointer<AI_Comport> ValueComport { get; set; }
		public Pointer<GAM_State> ValueAction { get; set; }
		public uint ValueText { get; set; }
		public Pointer<GMT_GameMaterial> ValueGameMaterial { get; set; }
		public GAM_ActorCapabilities ValueCaps { get; set; }
		public Pointer<WAY_Graph> ValueGraph { get; set; }
		public Pointer<IPT_EntryElement> ValueInput { get; set; }
		public Pointer<GLI_Light> ValueLight { get; set; }
		public Pointer<GLI_Material> ValueVisualMaterial { get; set; }
		public AI_DsgVarValueArray ValueArray { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			SerializeValue(s);
		}

		private void SerializeValue(SerializerObject s) {
			// Serialize differently depending on type
			var nodeType = Pre_LinkedType;
			
			switch (nodeType) {
				case AI_DsgVarType.Boolean:
					ValueBoolean = s.Serialize<bool>(ValueBoolean, name: nameof(ValueBoolean));
					break;
				case AI_DsgVarType.SByte:
					ValueSByte = s.Serialize<sbyte>(ValueSByte, name: nameof(ValueSByte));
					break;
				case AI_DsgVarType.UByte:
					ValueUByte = s.Serialize<byte>(ValueUByte, name: nameof(ValueUByte));
					break;
				case AI_DsgVarType.Short:
					ValueShort = s.Serialize<short>(ValueShort, name: nameof(ValueShort));
					break;
				case AI_DsgVarType.UShort:
					ValueUShort = s.Serialize<ushort>(ValueUShort, name: nameof(ValueUShort));
					break;
				case AI_DsgVarType.Int:
					ValueInt = s.Serialize<int>(ValueInt, name: nameof(ValueInt));
					break;
				case AI_DsgVarType.UInt:
					ValueUInt = s.Serialize<uint>(ValueUInt, name: nameof(ValueUInt));
					break;
				case AI_DsgVarType.Float:
					ValueFloat = s.Serialize<float>(ValueFloat, name: nameof(ValueFloat));
					break;
				case AI_DsgVarType.WayPoint:
					ValueWayPoint = s.SerializePointer<WAY_WayPoint>(ValueWayPoint, name: nameof(ValueWayPoint))?.ResolveObject(s);
					break;
				case AI_DsgVarType.Way:
					throw new NotImplementedException();
				case AI_DsgVarType.Perso:
				case AI_DsgVarType.SuperObject:
					ValueSuperObject = s.SerializePointer<HIE_SuperObject>(ValueSuperObject, name: nameof(ValueSuperObject))?.ResolveObject(s);
					break;
				case AI_DsgVarType.List:
					ValueList = s.SerializeObject<AI_DsgVarList>(ValueList, name: nameof(ValueList));
					break;
				case AI_DsgVarType.Vector:
					ValueVector = s.SerializeObject<MTH3D_Vector>(ValueVector, name: nameof(ValueVector));
					break;
				case AI_DsgVarType.Comport:
					ValueComport = s.SerializePointer<AI_Comport>(ValueComport, name: nameof(ValueComport))?.ResolveObject(s);
					break;
				case AI_DsgVarType.Action:
					ValueAction = s.SerializePointer<GAM_State>(ValueAction, name: nameof(ValueAction))?.ResolveObject(s);
					break;
				case AI_DsgVarType.Text:
					ValueText = s.Serialize<uint>(ValueText, name: nameof(ValueText));
					break;
				case AI_DsgVarType.GameMaterial:
					ValueGameMaterial = s.SerializePointer<GMT_GameMaterial>(ValueGameMaterial, name: nameof(ValueGameMaterial))?.ResolveObject(s);
					break;
				case AI_DsgVarType.Caps:
					ValueCaps = s.Serialize<GAM_ActorCapabilities>(ValueCaps, name: nameof(ValueCaps));
					break;
				case AI_DsgVarType.Graph:
					ValueGraph = s.SerializePointer<WAY_Graph>(ValueGraph, name: nameof(ValueGraph))?.ResolveObject(s);
					break;
				case AI_DsgVarType.Input:
					ValueInput = s.SerializePointer<IPT_EntryElement>(ValueInput, name: nameof(ValueInput))?.ResolveObject(s);
					break;
				case AI_DsgVarType.Light:
					ValueLight = s.SerializePointer<GLI_Light>(ValueLight, name: nameof(ValueLight))?.ResolveObject(s);
					break;
				case AI_DsgVarType.VisualMaterial:
					ValueVisualMaterial = s.SerializePointer<GLI_Material>(ValueVisualMaterial, name: nameof(ValueVisualMaterial))?.ResolveObject(s);
					break;
				case AI_DsgVarType.PersoArray:
				case AI_DsgVarType.VectorArray:
				case AI_DsgVarType.FloatArray:
				case AI_DsgVarType.IntegerArray:
				case AI_DsgVarType.WayPointArray:
				case AI_DsgVarType.TextArray:

				case AI_DsgVarType.GraphArray:
				case AI_DsgVarType.SuperObjectArray:
				case AI_DsgVarType.SOLinksArray:
				case AI_DsgVarType.SoundEventArray:
				case AI_DsgVarType.VisualMatArray:
				// case AI_DsgVarType.TextRefArray: // Doesn't exist?

				case AI_DsgVarType.ActionArray:
				case AI_DsgVarType.Placeholder__UnknownArray:
					ValueArray = s.SerializeObject<AI_DsgVarValueArray>(ValueArray,
						onPreSerialize: a => a.Pre_LinkedType = Pre_LinkedType,
						name: nameof(ValueArray));
					break;
				default:
					throw new BinarySerializableException(this, $"Unimplemented dsgvar type {nodeType}");
			}
		}
	}
}
