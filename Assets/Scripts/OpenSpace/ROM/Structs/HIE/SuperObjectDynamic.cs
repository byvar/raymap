using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class SuperObjectDynamic : ROMStruct {
		// Size: 12
		public Reference<Perso> perso;
		public ushort matrixIndex;
		public ushort flags;
		public ushort unkByte; // read as byte?
		public uint unk;
		public SuperObject.Transform transform;

		protected override void ReadInternal(Reader reader) {
			//Loader.print(Pointer.Current(reader));
			perso = new Reference<Perso>(reader);
			matrixIndex = reader.ReadUInt16();
			flags = reader.ReadUInt16();
			unkByte = reader.ReadUInt16();
			unk = reader.ReadUInt32();

			ResolveMatrices(reader);
			perso.Resolve(reader);
		}

		protected void ResolveMatrices(Reader reader) {
			transform = new SuperObject.Transform();
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
					//MapLoader.Loader.print(transform.position);
				}
			}
		}

		protected SuperObject.ROMMatrix ResolveMatrix(Reader reader, ushort index, Short3Array indices, Vector3Array vectors) {
			if (index == 0xFFFF || indices == null || vectors == null) return null;
			Short3Array.Triangle tri = indices.triangles[index];
			return new SuperObject.ROMMatrix() {
				v1 = vectors.vectors[tri.v1],
				v2 = vectors.vectors[tri.v2],
				v3 = vectors.vectors[tri.v3]
			};
		}

		public GameObject GetGameObject() {
			GameObject gao = new GameObject("SOD @ " + Offset);
			if (perso.Value != null) {
				GameObject persoGao = perso.Value.GetGameObject();
				persoGao.transform.SetParent(gao.transform);
			}
			/*if (data.Value != null && data.Value is PhysicalObject) {
				GameObject po = ((PhysicalObject)data.Value).GetGameObject();
				if (po != null) po.transform.SetParent(gao.transform);
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
			}*/
			return gao;
		}
		public void SetTransform(GameObject gao) {
			transform.Apply(gao);
		}
	}
}
