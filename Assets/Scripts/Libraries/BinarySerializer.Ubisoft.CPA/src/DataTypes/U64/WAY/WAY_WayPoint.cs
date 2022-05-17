namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class WAY_WayPoint : U64_Struct {
		public short Radius { get; set; }
		public MTH3D_ShortVector Vertex { get; set; }
		public U64_Reference<GAM_Character> Father { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Radius = s.Serialize<short>(Radius, name: nameof(Radius));
			Vertex = s.SerializeObject<MTH3D_ShortVector>(Vertex, name: nameof(Vertex));
			Father = s.SerializeObject<U64_Reference<GAM_Character>>(Father, name: nameof(Father))?.Resolve(s);
		}
	}
}
