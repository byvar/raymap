using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class CameraInfo : ROMStruct {
		// Size: 116
		public Vector3 vec_00;
		public Vector3 vec_0C;
		public Vector3 vec_18;
		public Vector3 vec_24;
		public Vector3 vec_30;
		public Vector4 vec4_3C;
		public Vector2 vec2_4C;
		public Vector4 vec4_54;
		public float flt_64;
		public ushort word_68;
		public ushort word_6A;
		public ushort word_6C;
		public ushort word_6E;
		public ushort word_70;
		public ushort word_72;

		protected override void ReadInternal(Reader reader) {
			//Loader.print("Camera @ " + Offset);
			vec_00 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			vec_0C = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			vec_18 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			vec_24 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			vec_30 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			vec4_3C = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			vec2_4C = new Vector2(reader.ReadSingle(), reader.ReadSingle());
			vec4_54 = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			flt_64 = reader.ReadSingle();
			word_68 = reader.ReadUInt16();
			word_6A = reader.ReadUInt16();
			word_6C = reader.ReadUInt16();
			word_6E = reader.ReadUInt16();
			word_70 = reader.ReadUInt16();
			word_72 = reader.ReadUInt16();
		}
	}
}
