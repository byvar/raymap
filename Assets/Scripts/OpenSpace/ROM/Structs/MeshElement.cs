using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class MeshElement : ROMStruct {
		public Reference<VisualMaterial> visualMaterial;
		public Reference<MeshElementTriangles> triangles;
		public ushort sz_triangles;
		public ushort num_uvs;
		
        protected override void ReadInternal(Reader reader) {
			visualMaterial = new Reference<VisualMaterial>(reader, true);
			if (Settings.s.platform == Settings.Platform.N64) {
				triangles = new Reference<MeshElementTriangles>(reader);
				reader.ReadUInt16();
				sz_triangles = reader.ReadUInt16();
				reader.ReadUInt16();
				reader.ReadUInt16();
			} else if (Settings.s.platform == Settings.Platform.DS) {
				triangles = new Reference<MeshElementTriangles>(reader);
				sz_triangles = reader.ReadUInt16();
			} else if (Settings.s.platform == Settings.Platform._3DS) {
				triangles = new Reference<MeshElementTriangles>(reader);
				sz_triangles = reader.ReadUInt16();
				num_uvs = reader.ReadUInt16();
			}
			triangles.Resolve(reader, (t) => { t.length = sz_triangles; t.num_uvs = num_uvs; });
			
		}
    }
}
