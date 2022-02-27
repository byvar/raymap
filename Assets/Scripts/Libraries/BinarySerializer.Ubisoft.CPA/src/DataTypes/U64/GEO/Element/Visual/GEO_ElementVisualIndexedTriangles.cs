using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GEO_ElementVisualIndexedTriangles : U64_Struct {
		public U64_Reference<GLI_VisualMaterial> VisualMaterial { get; set; }
		public U64_Reference<U64_Placeholder> VerticesList { get; set; }
		public U64_Reference<U64_Placeholder> GraphicsList { get; set; }
		public ushort GraphicsListSize { get; set; }
		public ushort VerticesListSize { get; set; }
		public ushort FacesCount { get; set; }
		public ushort VerticesCount { get; set; }
		public U64_Reference<U64_Placeholder> CompressedGraphicsList { get; set; }
		public ushort CompressedGraphicsListSize { get; set; }
		public U64_Reference<U64_Placeholder> GraphicsList3DS { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			VisualMaterial = s.SerializeObject<U64_Reference<GLI_VisualMaterial>>(VisualMaterial, name: nameof(VisualMaterial))?.Resolve(s);
			if (s.GetCPASettings().Platform == Platform.N64) {
				VerticesList = s.SerializeObject<U64_Reference<U64_Placeholder>>(VerticesList, name: nameof(VerticesList));
				GraphicsList = s.SerializeObject<U64_Reference<U64_Placeholder>>(GraphicsList, name: nameof(GraphicsList));
				GraphicsListSize = s.Serialize<ushort>(GraphicsListSize, name: nameof(GraphicsListSize));
				VerticesListSize = s.Serialize<ushort>(VerticesListSize, name: nameof(VerticesListSize));
				FacesCount = s.Serialize<ushort>(FacesCount, name: nameof(FacesCount));
			} else if (s.GetCPASettings().Platform == Platform._3DS) {
				GraphicsList3DS = s.SerializeObject<U64_Reference<U64_Placeholder>>(GraphicsList3DS, name: nameof(GraphicsList3DS));
				FacesCount = s.Serialize<ushort>(FacesCount, name: nameof(FacesCount));
				VerticesCount = s.Serialize<ushort>(VerticesCount, name: nameof(VerticesCount));
			} else if (s.GetCPASettings().Platform == Platform.DS) {
				GraphicsList = s.SerializeObject<U64_Reference<U64_Placeholder>>(GraphicsList, name: nameof(GraphicsList));
				GraphicsListSize = s.Serialize<ushort>(GraphicsListSize, name: nameof(GraphicsListSize));
				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.RaymanRavingRabbids)) {
					CompressedGraphicsList = s.SerializeObject<U64_Reference<U64_Placeholder>>(CompressedGraphicsList, name: nameof(CompressedGraphicsList));
					CompressedGraphicsListSize = s.Serialize<ushort>(CompressedGraphicsListSize, name: nameof(CompressedGraphicsListSize));
				}
			}

			// Resolve
			VisualMaterial?.Resolve(s);
		}
	}

}
