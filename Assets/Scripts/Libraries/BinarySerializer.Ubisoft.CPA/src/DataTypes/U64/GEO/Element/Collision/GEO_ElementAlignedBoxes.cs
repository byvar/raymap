using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GEO_ElementAlignedBoxes : U64_Struct {
		public LST_List<GEO_AlignedBox> Boxes { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Boxes = s.SerializeObject<LST_List<GEO_AlignedBox>>(Boxes, name: nameof(Boxes))?.Resolve(s);
		}
	}
}
