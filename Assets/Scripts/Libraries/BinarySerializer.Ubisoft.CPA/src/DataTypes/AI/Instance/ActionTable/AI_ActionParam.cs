namespace BinarySerializer.Ubisoft.CPA {
	public class AI_ActionParam : BinarySerializable {
		public AI_GetSetParam[] Parameters { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Parameters = s.SerializeObjectArray<AI_GetSetParam>(Parameters, 8, name: nameof(Parameters));
		}
	}
}
