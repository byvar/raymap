namespace BinarySerializer.Ubisoft.CPA {
	public class DNM_Dynamics : BinarySerializable {
		public DNM_DynamicsBaseBlock BaseBlock { get; set; }
		public DNM_DynamicsAdvancedBlock AdvancedBlock { get; set; }
		public DNM_DynamicsComplexBlock ComplexBlock { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			BaseBlock = s.SerializeObject<DNM_DynamicsBaseBlock>(BaseBlock, name: nameof(BaseBlock));

			if (BaseBlock.EndFlags.HasFlag(DNM_DynamicsEndFlags.AdvancedSize) || BaseBlock.EndFlags.HasFlag(DNM_DynamicsEndFlags.ComplexSize)) {
				AdvancedBlock = s.SerializeObject<DNM_DynamicsAdvancedBlock>(AdvancedBlock, name: nameof(AdvancedBlock));
			}
			if (BaseBlock.EndFlags.HasFlag(DNM_DynamicsEndFlags.ComplexSize)) {
				ComplexBlock = s.SerializeObject<DNM_DynamicsComplexBlock>(ComplexBlock, name: nameof(ComplexBlock));
			}
		}
	}
}
