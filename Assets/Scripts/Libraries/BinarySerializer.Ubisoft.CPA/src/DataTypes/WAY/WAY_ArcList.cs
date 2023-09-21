namespace BinarySerializer.Ubisoft.CPA {
	public class WAY_ArcList : BinarySerializable {
		public LST2_DynamicList<WAY_Arc> Arcs { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Arcs = s.SerializeObject<LST2_DynamicList<WAY_Arc>>(Arcs, name: nameof(Arcs))?.Resolve(s, name: nameof(Arcs));
		}
	}
}
