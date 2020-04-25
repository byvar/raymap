using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class PolygonList : OpenSpaceStruct {
		public ushort type;
		public ushort ushort_02;
		public Pointer off_polygons;
		public uint length;

		public IPS1Polygon[] polygons;

		protected override void ReadInternal(Reader reader) {
			type = reader.ReadUInt16();
			ushort_02 = reader.ReadUInt16();
			off_polygons = Pointer.Read(reader);
			Pointer.DoAt(ref reader, off_polygons, () => {
				length = reader.ReadUInt32();
				switch (type) {
					case 1:
						polygons = Load.ReadArray<QuadLOD>(length, reader);
						break;
					case 3:
						polygons = Load.ReadArray<TriangleNoTexture>(length, reader);
						break;
					case 4:
						polygons = Load.ReadArray<QuadNoTexture>(length, reader);
						break;
					case 5:
						polygons = Load.ReadArray<Triangle>(length, reader);
						break;
					case 6:
						polygons = Load.ReadArray<Quad>(length, reader);
						break;
					default:
						UnityEngine.Debug.LogWarning(type + " - " + Offset);
						break;
				}
			});
		}
	}
}
