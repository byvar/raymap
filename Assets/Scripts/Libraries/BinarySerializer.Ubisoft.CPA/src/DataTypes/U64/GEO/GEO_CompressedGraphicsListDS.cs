using BinarySerializer.Nintendo;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GEO_CompressedGraphicsListDS : U64_Struct {
		public int Pre_CompressedSize { get; set; }

		public GEO_GraphicsList GraphicsList { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			s.DoEncoded(new GBA_LZSSEncoder(), () => {
				GraphicsList = s.SerializeObject<GEO_GraphicsList>(GraphicsList, g => g.Pre_Size = s.CurrentLength, name: nameof(GraphicsList));
			});
		}
	}
}
