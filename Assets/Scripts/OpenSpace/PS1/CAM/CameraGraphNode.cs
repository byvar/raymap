using OpenSpace.Loader;
using System.Collections.Generic;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class CameraGraphNode : OpenSpaceStruct {
		public Pointer off_previous;
		public Pointer off_next;
		public int x;
		public int y;
		public int z;
		public uint unknown1;
		public uint unknown2;

		// Parsed
		public CameraGraphNode previous;
		public CameraGraphNode next;


		protected override void ReadInternal(Reader reader) {
			off_previous = Pointer.Read(reader);
			off_next = Pointer.Read(reader);
			x = reader.ReadInt32();
			y = reader.ReadInt32();
			z = reader.ReadInt32();
			unknown1 = reader.ReadUInt32();
			unknown2 = reader.ReadUInt32();

			previous = Load.FromOffsetOrRead<CameraGraphNode>(reader, off_previous);
			next = Load.FromOffsetOrRead<CameraGraphNode>(reader, off_next);
		}


		public GameObject GetGameObject(GameObject parent) {
			GameObject gao = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			gao.name = $"Camera graph node";
			MeshRenderer mr = gao.GetComponent<MeshRenderer>();
			gao.transform.SetParent(parent.transform);
			gao.transform.position = new Vector3(x / 256f, z / 256f, y / 256f);
			//gao.transform.localScale = Vector3.one * (type / 256f) * 2; // default Unity sphere radius is 0.5
			//gao.layer = LayerMask.NameToLayer("Collide");

			/*CollideComponent cc = sphere_gao.AddComponent<CollideComponent>();
			cc.collideROM = this;
			cc.type = collideType;
			cc.index = i;*/

			mr.material = new Material(MapLoader.Loader.collideMaterial);
			//mr.material.color = new Color(int_14 / (float)(256*128), int_18 / (float)(256 * 128), int_1C / (float)(256 * 128), 1f);
			return gao;
		}

	}
}
