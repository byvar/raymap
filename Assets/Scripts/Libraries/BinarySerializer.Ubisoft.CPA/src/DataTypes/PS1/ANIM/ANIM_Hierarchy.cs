namespace BinarySerializer.Ubisoft.CPA.PS1 {
	public class ANIM_Hierarchy : BinarySerializable {
		public int Child { get; set; }
		public int Parent { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Child = s.Serialize<int>(Child, name: nameof(Child));
			Parent = s.Serialize<int>(Parent, name: nameof(Parent));
		}
	}
}