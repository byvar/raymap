using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class LightInfo : ROMStruct {
		// size: 104
		public float near;
		public float far;
		public float fogBlendFar;
		public float fogBlendNear;
		public Vector4 backgroundColor;
		public Vector4 color;
		public ushort objectLightedFlag;
		public ushort lightProperties;
		public Vector3 vec3_34;
		public Vector3 vec3_40;
		public Vector3 vec3_4C;
		public Vector3 vec3_58;
		public ushort transformIndex;
		public ushort flags;

		public ROMTransform transform;

		protected override void ReadInternal(Reader reader) {
			near = reader.ReadSingle();
			far = reader.ReadSingle();
			fogBlendFar = reader.ReadSingle();
			fogBlendNear = reader.ReadSingle();
			backgroundColor = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			color = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()); // looks like color
			objectLightedFlag = reader.ReadUInt16();
			lightProperties = reader.ReadUInt16();
			vec3_34 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			vec3_40 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			vec3_4C = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			vec3_58 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			transformIndex = reader.ReadUInt16();
			flags = reader.ReadUInt16();

			transform = new ROMTransform(transformIndex);
		}

		public byte Type {
			get {
				return (byte)(flags & 0xF);
			}
		}
		public bool IsActive {
			get {
				return (flags & 0x8000) != 0;
			}
		}

		public int PaintingLightFlag {
			get {
				return (lightProperties & 0x1);
			}
		}
		public int AlphaLightFlag {
			get {
				return (lightProperties & 0x2); // should be 2 or 0, so no shifts necessary
			}
		}

		public GameObject GetGameObject() {
			GameObject gao = new GameObject("LightInfo @ " + Offset);
			return gao;
		}
	}
}
