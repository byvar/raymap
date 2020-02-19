using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.Visual.PS2Optimized {
	public class PS2OptimizedGeometricObject : OpenSpaceStruct {
		public uint dword_00;
		public uint num_elements;
		public Pointer off_visualMaterialsArray;
		public Pointer<PointerList<PS2OptimizedGeometricObjectElementTriangles>> elements;
		public Pointer off_uint1Array;
		public Pointer off_uint2Array;
		public Pointer off_unk; // Seems like an empty buffer? Some 2's in it
		public uint unk1;
		public uint unk2;

		// Parsed
		public Pointer[] off_visualMaterials;
		public VisualMaterial[] visualMaterials;
		public uint[] uint1;
		public uint[] uint2;

		protected override void ReadInternal(Reader reader) {
			Load.print("GEO: " + Offset);
			dword_00 = reader.ReadUInt32();
			UnityEngine.Debug.LogWarning(dword_00);
			num_elements = reader.ReadUInt32();
			off_visualMaterialsArray = Pointer.Read(reader);
			Pointer.DoAt(ref reader, off_visualMaterialsArray, () => {
				off_visualMaterials = new Pointer[num_elements];
				visualMaterials = new VisualMaterial[num_elements];
				for (int i = 0; i < num_elements; i++) {
					off_visualMaterials[i] = Pointer.Read(reader);
					visualMaterials[i] = VisualMaterial.FromOffsetOrRead(off_visualMaterials[i], reader);
				}
			});
			elements = new Pointer<PointerList<PS2OptimizedGeometricObjectElementTriangles>>(
				reader, true, onPreRead: a => a.length = num_elements);
			off_uint1Array = Pointer.Read(reader);
			off_uint2Array = Pointer.Read(reader);
			off_unk = Pointer.Read(reader);
			unk1 = reader.ReadUInt32();
			unk2 = reader.ReadUInt32();

			Pointer.DoAt(ref reader, off_uint1Array, () => {
				uint1 = new uint[num_elements];
				for (int i = 0; i < num_elements; i++) {
					uint1[i] = reader.ReadUInt32();
				}
			});
			Pointer.DoAt(ref reader, off_uint2Array, () => {
				uint2 = new uint[num_elements];
				for (int i = 0; i < num_elements; i++) {
					uint2[i] = reader.ReadUInt32();
				}
			});
		}
	}
}
