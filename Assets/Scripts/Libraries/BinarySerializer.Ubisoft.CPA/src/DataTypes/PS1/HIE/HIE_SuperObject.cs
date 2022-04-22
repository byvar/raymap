namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class HIE_SuperObject : BinarySerializable, LST2_IEntry<HIE_SuperObject>
	{
		public bool Pre_IsDynamic { get; set; }

		public HIE_ObjectType_98 Type { get; set; }
		public int DataIndex { get; set; }
		public LST2_DynamicList<HIE_SuperObject> Children { get; set; }
		public Pointer<HIE_SuperObject> NextBrother { get; set; }
		public Pointer<HIE_SuperObject> PreviousBrother { get; set; }
		public Pointer<HIE_SuperObject> Parent { get; set; }
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

		public Pointer<HIE_SuperObject> LST2_Next => NextBrother;
		public Pointer<HIE_SuperObject> LST2_Previous => PreviousBrother;

		public override void SerializeImpl(SerializerObject s)
		{
			CPA_Settings settings = s.GetCPASettings();

			Type = s.Serialize<HIE_ObjectType_98>(Type, name: nameof(Type));
			DataIndex = s.Serialize<int>(DataIndex, name: nameof(DataIndex));
			Children = s.SerializeObject<LST2_DynamicList<HIE_SuperObject>>(Children, name: nameof(Children));
			NextBrother = s.SerializePointer<HIE_SuperObject>(NextBrother, name: nameof(NextBrother));
			PreviousBrother = s.SerializePointer<HIE_SuperObject>(PreviousBrother, name: nameof(PreviousBrother));
			Parent = s.SerializePointer<HIE_SuperObject>(Parent, name: nameof(Parent));
			LocalMatrix = s.SerializePointer(LocalMatrix, name: nameof(LocalMatrix))?.Resolve(s);

			if (settings.EngineVersion != EngineVersion.RaymanRush_PS1)
			{
				GlobalMatrix = s.SerializePointer(GlobalMatrix, name: nameof(GlobalMatrix))?.Resolve(s);
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

			// Resolve hierarchy
			Children?.Resolve(s, name: nameof(Children));
			if (Parent?.PointerValue == null) {
				NextBrother?.Resolve(s);
				PreviousBrother?.Resolve(s);
			}
			Parent?.Resolve(s);
		}
	}
}