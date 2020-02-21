using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.Visual.PS2Optimized {
	public class PS2OptimizedSDCStructure : OpenSpaceStruct {
		public uint flags;
		public uint num_elements;
		public Pointer off_visualMaterials;
		public Pointer off_elements;
		public Pointer off_uint1;
		public Pointer off_uint2;
		public Pointer off_unk; // Seems like an empty buffer? Some 2's in it
		public uint unk1;
		public uint unk2;
		public uint isSinus;

		// Parsed
		public Pointer[] off_visualMaterials_array;
		public Pointer[] off_elements_array;
		public PS2OptimizedSDCStructureElement[] elements;
		public VisualMaterial[] visualMaterials;
		public uint[] uint1;
		public uint[] num_triangles;

		protected override void ReadInternal(Reader reader) {
			flags = reader.ReadUInt32();
			//UnityEngine.Debug.LogWarning("GEO: " + Offset + " - " + flags);
			num_elements = reader.ReadUInt32();
			off_visualMaterials = Pointer.Read(reader);
			Pointer.DoAt(ref reader, off_visualMaterials, () => {
				off_visualMaterials_array = new Pointer[num_elements];
				visualMaterials = new VisualMaterial[num_elements];
				for (int i = 0; i < num_elements; i++) {
					off_visualMaterials_array[i] = Pointer.Read(reader);
					visualMaterials[i] = VisualMaterial.FromOffsetOrRead(off_visualMaterials_array[i], reader);
				}
			});
			off_elements = Pointer.Read(reader);
			off_uint1 = Pointer.Read(reader);
			off_uint2 = Pointer.Read(reader);
			off_unk = Pointer.Read(reader);
			unk1 = reader.ReadUInt32();
			unk2 = reader.ReadUInt32();

			Pointer.DoAt(ref reader, off_uint1, () => {
				uint1 = new uint[num_elements];
				for (int i = 0; i < num_elements; i++) {
					uint1[i] = reader.ReadUInt32();
				}
			});
			Pointer.DoAt(ref reader, off_uint2, () => {
				num_triangles = new uint[num_elements];
				for (int i = 0; i < num_elements; i++) {
					num_triangles[i] = reader.ReadUInt32();
				}
			});
			Pointer.DoAt(ref reader, off_elements, () => {
				off_elements_array = new Pointer[num_elements];
				for (int i = 0; i < num_elements; i++) {
					off_elements_array[i] = Pointer.Read(reader);
				}
			});

			elements = new PS2OptimizedSDCStructureElement[num_elements];
			for (int i = 0; i < num_elements; i++) {
				Pointer.DoAt(ref reader, off_elements_array[i], () => {
					elements[i] = new PS2OptimizedSDCStructureElement(this, i);
					elements[i].Read(reader);
				});
			}
		}

		public uint Type {
			get {
				return flags & 0xF;
			}
		}
	}
}
