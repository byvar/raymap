using OpenSpace.Loader;
using System.Collections.Generic;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class CameraModifier : OpenSpaceStruct {
		public uint type;
		public int int_04;
		public int int_08;
		public int int_0C;
		public int int_10;
		public int int_14;
		public int int_18;
		public int int_1C;
		public Pointer off_20; // if type == 9: a linkedlist. otherwise: a SO pointer
		public int x2;
		public int y2;
		public int z2;
		public int int_30;
		public int int_34;
		public int x_7;
		public int y_7;
		public int z_7;
		public int int_44;
		public int x;
		public int y;
		public int z;
		public int int_54;
		public int int_58;
		public int int_5C;
		public int int_60;
		public int int_64;

		// Parsed
		public CameraGraph graph;

		protected override void ReadInternal(Reader reader) {
			type = reader.ReadUInt32();
			int_04 = reader.ReadInt32();
			int_08 = reader.ReadInt32();
			int_0C = reader.ReadInt32();
			int_10 = reader.ReadInt32();
			int_14 = reader.ReadInt32();
			int_18 = reader.ReadInt32();
			int_1C = reader.ReadInt32();
			off_20 = Pointer.Read(reader);
			x2 = reader.ReadInt32();
			y2 = reader.ReadInt32();
			z2 = reader.ReadInt32();
			int_30 = reader.ReadInt32();
			int_34 = reader.ReadInt32();
			x_7 = reader.ReadInt32();
			y_7 = reader.ReadInt32();
			z_7 = reader.ReadInt32();
			int_44 = reader.ReadInt32();
			x = reader.ReadInt32();
			y = reader.ReadInt32();
			z = reader.ReadInt32();
			int_54 = reader.ReadInt32();
			int_58 = reader.ReadInt32();
			int_5C = reader.ReadInt32();
			int_60 = reader.ReadInt32();
			int_64 = reader.ReadInt32();

			if (type == 9 && off_20 != null) {
				graph = Load.FromOffsetOrRead<CameraGraph>(reader, off_20);
			}
		}


		public GameObject GetGameObject(CameraModifierVolume volume) {
			GameObject gao = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			gao.name = $"Camera Modifier Type {type} @ {Offset} {Size} - {off_20}";
			MeshRenderer mr = gao.GetComponent<MeshRenderer>();
			gao.transform.localPosition = Position;
			//gao.transform.localScale = Vector3.one * (type / 256f) * 2; // default Unity sphere radius is 0.5
			//gao.layer = LayerMask.NameToLayer("Collide");

			GameObject camGao = volume?.GetGameObject(gao);

			/*CollideComponent cc = sphere_gao.AddComponent<CollideComponent>();
			cc.collideROM = this;
			cc.type = collideType;
			cc.index = i;*/

			if (graph != null) {
				graph.GetGameObject(gao);
			}

			mr.material = new Material(MapLoader.Loader.collideMaterial);
			mr.material.color = new Color(int_14 / (float)(256*128), int_18 / (float)(256 * 128), int_1C / (float)(256 * 128), 1f);
			return gao;
		}

		Vector3 Position {
			get {
				var factor = R2PS1Loader.CoordinateFactor;
				if (type == 6 || type == 9) {
					return new Vector3(x, z, y) / factor;
				} else if (type == 7) {
					return new Vector3(x_7, z_7, y_7) / factor;
				}
				return new Vector3(x, z, y) / factor;
			}
		}

	}
}
