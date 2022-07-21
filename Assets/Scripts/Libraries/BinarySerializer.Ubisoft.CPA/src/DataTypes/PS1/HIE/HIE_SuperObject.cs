﻿using System;

namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class HIE_SuperObject : BinarySerializable, ILST2_Child<HIE_SuperObject, HIE_SuperObject>
	{
		public bool Pre_IsDynamic { get; set; }

		public HIE_ObjectType_98 Type { get; set; }
		public int DataIndex { get; set; }
		public LST2_DynamicParentElement<HIE_SuperObject, HIE_SuperObject> Children { get; set; }
		public LST2_DynamicChildElement<HIE_SuperObject, HIE_SuperObject> ListElement { get; set; }
		public Pointer<MAT_Transformation> LocalMatrix { get; set; }
		public Pointer<MAT_Transformation> GlobalMatrix { get; set; }
		public short Short_28 { get; set; }
		public short Short_2A { get; set; }
		public short Short_2C { get; set; }
		public short Short_2E { get; set; }
		public short Short_30 { get; set; }
		public short Short_32 { get; set; }
		public short Short_34 { get; set; }
		public short Short_36 { get; set; }
		public uint Uint_38 { get; set; }
		public Pointer Pointer_38 { get; set; }
		public short Short_3C { get; set; }
		public short Short_3E { get; set; }
		public short Short_40 { get; set; }
		public short Short_42 { get; set; }
		public short Short_44 { get; set; }
		public short Short_46 { get; set; }
		public short Short_48 { get; set; }
		public short Short_4A { get; set; }

		// LST2 Implementation
		public Pointer<HIE_SuperObject> LST2_Parent => ListElement?.Father;
		public Pointer<HIE_SuperObject> LST2_Next => ListElement?.LST2_Next;
		public Pointer<HIE_SuperObject> LST2_Previous => ListElement?.LST2_Previous;

		public override void SerializeImpl(SerializerObject s)
		{
			CPA_Settings settings = s.GetCPASettings();

			Type = s.Serialize<HIE_ObjectType_98>(Type, name: nameof(Type));
			DataIndex = s.Serialize<int>(DataIndex, name: nameof(DataIndex));
			Children = s.SerializeObject<LST2_DynamicParentElement<HIE_SuperObject, HIE_SuperObject>>(Children, name: nameof(Children))?.Resolve(s, name: nameof(Children));
			ListElement = s.SerializeObject<LST2_DynamicChildElement<HIE_SuperObject, HIE_SuperObject>>(ListElement, name: nameof(ListElement))?.Resolve(s);
			LocalMatrix = s.SerializePointer(LocalMatrix, name: nameof(LocalMatrix))?.ResolveObject(s);

			if (settings.EngineVersion != EngineVersion.RaymanRush_PS1)
			{
				GlobalMatrix = s.SerializePointer(GlobalMatrix, name: nameof(GlobalMatrix))?.ResolveObject(s);
				Short_28 = s.Serialize<short>(Short_28, name: nameof(Short_28));
				Short_2A = s.Serialize<short>(Short_2A, name: nameof(Short_2A));
				Short_2C = s.Serialize<short>(Short_2C, name: nameof(Short_2C));
				Short_2E = s.Serialize<short>(Short_2E, name: nameof(Short_2E));
				Short_30 = s.Serialize<short>(Short_30, name: nameof(Short_30));
				Short_32 = s.Serialize<short>(Short_32, name: nameof(Short_32));
				Short_34 = s.Serialize<short>(Short_34, name: nameof(Short_34));
				Short_36 = s.Serialize<short>(Short_36, name: nameof(Short_36));

				//if (Pre_IsDynamic)
					Uint_38 = s.Serialize<uint>(Uint_38, name: nameof(Uint_38));
				/*else
					Pointer_38 = s.SerializePointer(Pointer_38, name: nameof(Pointer_38));*/

				Short_3C = s.Serialize<short>(Short_3C, name: nameof(Short_3C));
				Short_3E = s.Serialize<short>(Short_3E, name: nameof(Short_3E));
				Short_40 = s.Serialize<short>(Short_40, name: nameof(Short_40));
				Short_42 = s.Serialize<short>(Short_42, name: nameof(Short_42));
				Short_44 = s.Serialize<short>(Short_44, name: nameof(Short_44));
				Short_46 = s.Serialize<short>(Short_46, name: nameof(Short_46));
				Short_48 = s.Serialize<short>(Short_48, name: nameof(Short_48));
				Short_4A = s.Serialize<short>(Short_4A, name: nameof(Short_4A));
			}
		}

		public BinarySerializable LinkedObject {
			get {
				GAM_GlobalPointerTable gpt = Context.GetLevel().GlobalPointerTable;
				if (Type == HIE_ObjectType_98.InstanciatedPhysicalObject) {
					if ((DataIndex >> 1) >= gpt.StaticGeometricObjects.Entries.Length) throw new Exception("IPO SO data index was too high! " + gpt.StaticGeometricObjects.Entries.Length + " - " + DataIndex);
					return gpt.StaticGeometricObjects.Entries[DataIndex >> 1].GeometricObject;
				} else if (Type == HIE_ObjectType_98.Character) {
					if (DataIndex >= gpt.Persos.Length) throw new Exception("Perso SO data index was too high! " + gpt.Persos.Length + " - " + DataIndex);
					return gpt.Persos[DataIndex];
				} else if (Type == HIE_ObjectType_98.Sector) {
					if (DataIndex >= gpt.Sectors.Length) throw new Exception("Sector SO data index was too high! " + gpt.Sectors.Length + " - " + DataIndex);
					return gpt.Sectors[DataIndex];
				}
				return null;
			}
		}
	}
}