using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class SuperObject : ROMStruct {
		// Size: 20
		public ushort matrixIndex; // 0, references global indices. check load_so_matrices.
		public ushort dataIndex; // 2
		public Reference<SuperObjectArray> children; // 4
		public Reference<CompressedVector3Array> comprVec3Array; // 6. Size: 12, so 2 vector3s
		public ushort type; // 8
		public ushort num_children; // 10
		public ushort unk; // 12
		public ushort unk2;
		public uint flags; // 16

		public GenericReference data;
		public Transform transform;

		protected override void ReadInternal(Reader reader) {
			matrixIndex = reader.ReadUInt16();
			dataIndex = reader.ReadUInt16();
			children = new Reference<SuperObjectArray>(reader, false);
			comprVec3Array = new Reference<CompressedVector3Array>(reader, true, v => v.length = 2);
			type = reader.ReadUInt16();
			num_children = reader.ReadUInt16();
			unk = reader.ReadUInt16();
			unk2 = reader.ReadUInt16();
			flags = reader.ReadUInt32();

			ResolveMatrices(reader);
			children.Resolve(reader, soa => soa.length = num_children);
			data = new GenericReference(type, dataIndex, reader, true);
		}

		protected void ResolveMatrices(Reader reader) {
			transform = new Transform();
			if (matrixIndex == 0xFFFF) return;
			LevelHeader lh = Loader.Get<LevelHeader>((ushort)(Loader.CurrentLevel | (ushort)FATEntry.Flag.Fix));
			if (lh != null) {
				Short3Array indices = lh.indices.Value;
				Vector3Array vectors = lh.vectors.Value;
				Short3Array.Triangle tri = lh.indices.Value.triangles[matrixIndex];
				transform.matrix1 = ResolveMatrix(reader, tri.v1, indices, vectors);
				transform.matrix2 = ResolveMatrix(reader, tri.v2, indices, vectors);
				if (tri.v3 != 0xFFFF) {
					transform.position = vectors.vectors[tri.v3];
					MapLoader.Loader.print(transform.position);
				}
			}
		}

		protected ROMMatrix ResolveMatrix(Reader reader, ushort index, Short3Array indices, Vector3Array vectors) {
			if (index == 0xFFFF || indices == null || vectors == null) return null;
			Short3Array.Triangle tri = indices.triangles[index];
			Vector3[] matrixVecs = new Vector3[3];
			matrixVecs[0] = vectors.vectors[tri.v1];
			matrixVecs[1] = vectors.vectors[tri.v2];
			matrixVecs[2] = vectors.vectors[tri.v3];
			for (int i = 0; i < 3; i++) {
				MapLoader.Loader.print(matrixVecs[i]);
			}
			return null;
		}

		public GameObject GetGameObject() {
			GameObject gao = new GameObject("SO @ " + Offset + " - " + type);
			if (data.Value != null && data.Value is PhysicalObject) {
				GameObject po = ((PhysicalObject)data.Value).GetGameObject();
				if(po !=  null) po.transform.SetParent(gao.transform);
			}
			if (children.Value != null) {
				foreach (Reference<SuperObject> so in children.Value.superObjects) {
					if (so.Value != null) {
						GameObject soGao = so.Value.GetGameObject();
						if (soGao != null) {
							soGao.transform.SetParent(gao.transform);
							so.Value.SetTransform(soGao);
						}
					}
				}
			}
			return gao;
		}
		public void SetTransform(GameObject gao) {
			transform.Apply(gao);
		}

		public class ROMMatrix {
			public Vector3? v1;
			public Vector3? v2;
			public Vector3? v3;
		}
		public class Transform {
			public ROMMatrix matrix1;
			public ROMMatrix matrix2;
			public Vector3? position;

			public Matrix Matrix {
				get {
					Matrix4x4 mat;
					if (position.HasValue) {
						mat = Matrix4x4.Translate(position.Value);
					} else {
						mat = Matrix4x4.identity;
					}
					return new Matrix(null, 0, mat, Vector4.one);
				}
			}
			public void Apply(GameObject gao) {
				Matrix mat = Matrix;
				gao.transform.localPosition = mat.GetPosition(convertAxes: true);
				gao.transform.localRotation = mat.GetRotation(convertAxes: true);
				gao.transform.localScale = mat.GetScale(convertAxes: true);
			}
		}
	}
}
