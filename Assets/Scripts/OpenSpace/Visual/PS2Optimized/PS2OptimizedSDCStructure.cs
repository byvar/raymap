using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.Visual.PS2Optimized {
	public class PS2OptimizedSDCStructure : OpenSpaceStruct {
		public uint flags;
		public uint num_elements;
		public LegacyPointer off_visualMaterials;
		public LegacyPointer off_elements;
		public LegacyPointer off_uint1;
		public LegacyPointer off_num_triangles;
		public LegacyPointer off_unk; // Seems like an empty buffer? Some 2's in it
		public uint unk1;
		public uint unk2;
		public uint isSinus;

		// Parsed
		public LegacyPointer[] off_visualMaterials_array;
		public LegacyPointer[] off_elements_array;
		public PS2OptimizedSDCStructureElement[] elements;
		public VisualMaterial[] visualMaterials;
		public uint[] uint1;
		public uint[] num_triangles;

		protected override void ReadInternal(Reader reader) {
			flags = reader.ReadUInt32();
			//UnityEngine.Debug.LogWarning("GEO: " + Offset + " - " + flags);
			num_elements = reader.ReadUInt32();
			off_visualMaterials = LegacyPointer.Read(reader);
			LegacyPointer.DoAt(ref reader, off_visualMaterials, () => {
				off_visualMaterials_array = new LegacyPointer[num_elements];
				visualMaterials = new VisualMaterial[num_elements];
				for (int i = 0; i < num_elements; i++) {
					off_visualMaterials_array[i] = LegacyPointer.Read(reader);
					visualMaterials[i] = VisualMaterial.FromOffsetOrRead(off_visualMaterials_array[i], reader);
				}
			});
			off_elements = LegacyPointer.Read(reader);
			off_uint1 = LegacyPointer.Read(reader);
			off_num_triangles = LegacyPointer.Read(reader);
			off_unk = LegacyPointer.Read(reader);
			unk1 = reader.ReadUInt32();
			unk2 = reader.ReadUInt32();

			LegacyPointer.DoAt(ref reader, off_uint1, () => {
				uint1 = new uint[num_elements];
				for (int i = 0; i < num_elements; i++) {
					uint1[i] = reader.ReadUInt32();
				}
			});
			LegacyPointer.DoAt(ref reader, off_num_triangles, () => {
				num_triangles = new uint[num_elements];
				for (int i = 0; i < num_elements; i++) {
					num_triangles[i] = reader.ReadUInt32();
				}
			});
			LegacyPointer.DoAt(ref reader, off_elements, () => {
				off_elements_array = new LegacyPointer[num_elements];
				for (int i = 0; i < num_elements; i++) {
					off_elements_array[i] = LegacyPointer.Read(reader);
				}
			});

			elements = new PS2OptimizedSDCStructureElement[num_elements];
			for (int i = 0; i < num_elements; i++) {
				LegacyPointer.DoAt(ref reader, off_elements_array[i], () => {
					elements[i] = new PS2OptimizedSDCStructureElement(this, i);
					elements[i].Read(reader);
				});
			}
		}

		public uint Type {
			get {
				return flags & 0xF;
			}
			/* Seems to be based on GL types (but only a little):
			#define GL_POINTS                         0x0000
			#define GL_LINES                          0x0001
			#define GL_LINE_LOOP                      0x0002
			#define GL_LINE_STRIP                     0x0003
			#define GL_TRIANGLES                      0x0004
			#define GL_TRIANGLE_STRIP                 0x0005
			#define GL_TRIANGLE_FAN                   0x0006*/
		}
	}
}
