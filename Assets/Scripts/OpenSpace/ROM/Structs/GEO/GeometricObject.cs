using OpenSpace.Loader;
using System;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class GeometricObject : ROMStruct {
		public float scaleFactor;
		public float factor_1;
		public Reference<CompressedVector3Array> verticesCollide;
		public Reference<CompressedVector3Array> verticesVisual;
		public Reference<CompressedVector3Array> normals;
		public Reference<GeometricElementListCollide> elementsCollide;
		public Reference<GeometricElementListVisual> elementsVisual;
		public ushort num_verticesCollide;
		public ushort num_verticesVisual;
		public ushort num_elementsCollide;
		public ushort num_elementsVisual;
		public ushort unk0;
		public ushort unk1;
		public ushort unk2;
		public ushort unk3;

		public float ScaleFactor {
			get {
				if (scaleFactor != 0f) return scaleFactor;
				return 1f;
			}
		}

		protected override void ReadInternal(Reader reader) {
			scaleFactor = reader.ReadSingle();
			factor_1 = reader.ReadSingle();
			if (Settings.s.platform == Settings.Platform._3DS) {
				verticesCollide = new Reference<CompressedVector3Array>(reader);
				verticesVisual = new Reference<CompressedVector3Array>(reader);
			}
			normals = new Reference<CompressedVector3Array>(reader);
			elementsCollide = new Reference<GeometricElementListCollide>(reader);
			elementsVisual = new Reference<GeometricElementListVisual>(reader);
			if (Settings.s.platform == Settings.Platform._3DS) {
				num_verticesCollide = reader.ReadUInt16();
			}
			num_verticesVisual = reader.ReadUInt16();
			num_elementsCollide = reader.ReadUInt16();
			num_elementsVisual = reader.ReadUInt16();
			unk0 = reader.ReadUInt16();
			unk1 = reader.ReadUInt16();
			unk2 = reader.ReadUInt16();
			unk3 = reader.ReadUInt16();

			//MapLoader.Loader.print("Vertices: " + num_vectors_1 + " or " + string.Format("{0:X4}", num_vectors_1));
			if (Settings.s.platform != Settings.Platform._3DS) {
				num_elementsCollide = num_verticesVisual;
				/*verticesCollide = normals;
				verticesVisual = normals;*/
			}

			verticesCollide?.Resolve(reader, v => { v.length = num_verticesCollide; });
			verticesVisual?.Resolve(reader, v => { v.length = num_verticesVisual; });
			normals.Resolve(reader, v => { v.length = num_verticesVisual; });
			elementsCollide.Resolve(reader, v => { v.length = num_elementsCollide; });
			elementsVisual.Resolve(reader, v => { v.length = num_elementsVisual; });
		}

		public GameObject GetGameObject(Type type) {
			GameObject gao = new GameObject("GeometricObject @ " + Offset);
			gao.name = gao.name + " - S0:" + scaleFactor
										+ " - S1:" + factor_1
										+ " - U0:" + string.Format("{0:X4}", unk0)
										+ " - U1:" + string.Format("{0:X4}", unk1)
										+ " - U2:" + string.Format("{0:X4}", unk2)
										+ " - U3:" + string.Format("{0:X4}", unk3);
			MapLoader l = MapLoader.Loader;
			if (elementsVisual.Value != null) {
				//gao.transform.position = new Vector3(UnityEngine.Random.Range(-100f, 100f), UnityEngine.Random.Range(-100f, 100f), UnityEngine.Random.Range(-100f, 100f));
				foreach (GeometricElementListVisual.GeometricElementListEntry entry in elementsVisual.Value.elements) {
					if (entry.element.Value == null) {
						l.print(entry.element.type);
					}
					if (entry.element.Value != null) {
						if (entry.element.Value is GeometricElementTriangles) {
							GeometricElementTriangles el = entry.element.Value as GeometricElementTriangles;
							GameObject child = el.GetGameObject(type, this);
							if (child != null) {
								child.transform.SetParent(gao.transform);
								child.transform.localPosition = Vector3.zero;
							}
						}
					}
				}
			}
			return gao;
		}

		public enum Type {
			Visual,
			Collide
		}
    }
}
