namespace BinarySerializer.N64 {
	public class RSP_Command : BinarySerializable {
		public RSP_CommandType Command { get; set; }
		public RSP_CommandData Data { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			// Just to determine the type
			s.DoAt(Offset, () => {
				Command = s.Serialize<RSP_CommandType>(Command, name: nameof(Command));
			});

			T SerializeData<T>() where T : RSP_CommandData, new() {
				return s.SerializeObject<T>((T)Data, onPreSerialize: cmd => cmd.Command = Command, name: nameof(Data));
			}
			
			Data = Command switch {
				RSP_CommandType.RSP_GBI1_Vtx => SerializeData<RSP_Command_GBI1_Vtx>(),
				RSP_CommandType.RSP_GBI1_ModifyVtx => SerializeData<RSP_Command_GBI1_ModifyVtx>(),
				_ => SerializeData<RSP_Command_Placeholder>(),
			};


			s.Align(8, Offset);
		}
	}
}
