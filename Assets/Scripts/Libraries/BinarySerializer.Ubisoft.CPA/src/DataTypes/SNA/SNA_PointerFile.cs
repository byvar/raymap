namespace BinarySerializer.Ubisoft.CPA {
	/// <summary>
	/// Used for small files like GlobalPointerTable, PTX and relocation files
	/// </summary>
	/// <typeparam name="T">Content of file</typeparam>
	public class SNA_PointerFile<T> : BinarySerializable where T : BinarySerializable, new() {
		public T Value { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersion == EngineVersion.TonicTrouble) {
				s.DoEncoded(new SNA_TTWindowEncoder(), () => {
					Value = s.SerializeObject<T>(Value, name: nameof(Value));
				});
			} else {
				Value = s.SerializeObject<T>(Value, name: nameof(Value));
			}
		}
	}
}
