using BinarySerializer.N64;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GEO_CompressedGraphicsListDS : U64_Struct {
		public int Pre_CompressedSize { get; set; }

		public GEO_GraphicsList GraphicsList { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			// TODO: Use GBA_LZSS encoder here instead
			/*s.DoEncoded(new YAY0Encoder(), () => {
				GraphicsList = s.SerializeObject<GEO_GraphicsList>(GraphicsList, g => g.Pre_Size = s.CurrentLength, name: nameof(GraphicsList));
			});*/
		}
	}
}
