namespace BinarySerializer.Nintendo.DS {
	public class DS3D_Command : BinarySerializable {
		public DS3D_CommandType Type { get; set; }

		public DS3D_CommandData Data { get; set; }

		public override void SerializeImpl(SerializerObject s) {
            Type = s.Serialize<DS3D_CommandType>(Type, name: nameof(Type));
        }

		public void SerializeData(SerializerObject s) {
			T SerializeCommandData<T>() where T : DS3D_CommandData, new() {
				return s.SerializeObject<T>((T)Data, onPreSerialize: t => t.Command = this, name: nameof(Data));
			}

			Data = Type switch {
				DS3D_CommandType.None => null,
				DS3D_CommandType.BEGIN_VTXS => SerializeCommandData<DS3D_Command_Begin_Vtxs>(),
				DS3D_CommandType.END_VTXS => null,
				DS3D_CommandType.TEXCOORD => SerializeCommandData<DS3D_Command_TexCoord>(),
				DS3D_CommandType.NORMAL => SerializeCommandData<DS3D_Command_Normal>(),
				DS3D_CommandType.COLOR => SerializeCommandData<DS3D_Command_Color>(),
				DS3D_CommandType.VTX_16 => SerializeCommandData<DS3D_Command_Vtx_16>(),
				DS3D_CommandType.VTX_XY => SerializeCommandData<DS3D_Command_Vtx_XY>(),
				DS3D_CommandType.VTX_XZ => SerializeCommandData<DS3D_Command_Vtx_XZ>(),
				DS3D_CommandType.VTX_YZ => SerializeCommandData<DS3D_Command_Vtx_YZ>(),
				_ => throw new BinarySerializableException(this, $"Unparsed DS3D command type: {Type}")
			};
		}
		public override bool UseShortLog => true;
		public override string ToString() => $"{GetType()}({Type})";
	}
}
