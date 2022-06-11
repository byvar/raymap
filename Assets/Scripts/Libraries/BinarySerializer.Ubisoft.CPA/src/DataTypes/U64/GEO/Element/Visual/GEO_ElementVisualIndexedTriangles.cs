using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GEO_ElementVisualIndexedTriangles : U64_Struct {
		public U64_Reference<GLI_VisualMaterial> VisualMaterial { get; set; }
		public U64_Reference<GEO_VerticesList> VerticesList { get; set; }
		public U64_Reference<GEO_GraphicsList> GraphicsList { get; set; }
		public ushort GraphicsListSize { get; set; }
		public ushort VerticesListSize { get; set; }
		public ushort FacesCount { get; set; }
		public ushort VerticesCount { get; set; }
		public U64_Reference<GEO_CompressedGraphicsListDS> CompressedGraphicsList { get; set; }
		public ushort CompressedGraphicsListSize { get; set; }
		public U64_Reference<GEO_GraphicsList3DS> GraphicsList3DS { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			VisualMaterial = s.SerializeObject<U64_Reference<GLI_VisualMaterial>>(VisualMaterial, name: nameof(VisualMaterial))?.Resolve(s);
			if (s.GetCPASettings().Platform == Platform.N64) {
				GraphicsList = s.SerializeObject<U64_Reference<GEO_GraphicsList>>(GraphicsList, name: nameof(GraphicsList));
				VerticesList = s.SerializeObject<U64_Reference<GEO_VerticesList>>(VerticesList, name: nameof(VerticesList));
				GraphicsListSize = s.Serialize<ushort>(GraphicsListSize, name: nameof(GraphicsListSize));
				VerticesListSize = s.Serialize<ushort>(VerticesListSize, name: nameof(VerticesListSize));
				FacesCount = s.Serialize<ushort>(FacesCount, name: nameof(FacesCount));
			} else if (s.GetCPASettings().Platform == Platform._3DS) {
				GraphicsList3DS = s.SerializeObject<U64_Reference<GEO_GraphicsList3DS>>(GraphicsList3DS, name: nameof(GraphicsList3DS));
				FacesCount = s.Serialize<ushort>(FacesCount, name: nameof(FacesCount));
				VerticesCount = s.Serialize<ushort>(VerticesCount, name: nameof(VerticesCount));
			} else if (s.GetCPASettings().Platform == Platform.DS) {
				GraphicsList = s.SerializeObject<U64_Reference<GEO_GraphicsList>>(GraphicsList, name: nameof(GraphicsList));
				GraphicsListSize = s.Serialize<ushort>(GraphicsListSize, name: nameof(GraphicsListSize));
				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman4DS)) {
					CompressedGraphicsList = s.SerializeObject<U64_Reference<GEO_CompressedGraphicsListDS>>(CompressedGraphicsList, name: nameof(CompressedGraphicsList));
					CompressedGraphicsListSize = s.Serialize<ushort>(CompressedGraphicsListSize, name: nameof(CompressedGraphicsListSize));
				}
			}

			// Resolve
			VisualMaterial?.Resolve(s);
			VerticesList?.Resolve(s, onPreSerialize: (_, v) => v.Pre_Size = VerticesListSize);
			GraphicsList3DS?.Resolve(s, onPreSerialize: (_, g) => {
				g.Pre_VerticesCount = VerticesCount;
				g.Pre_FacesCount = FacesCount;
			});
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman4DS))
				GraphicsList?.Resolve(s, onPreSerialize: (_, g) => g.Pre_Size = GraphicsListSize);
			CompressedGraphicsList?.Resolve(s, onPreSerialize: (_, g) => g.Pre_CompressedSize = CompressedGraphicsListSize);
		}
	}

}
