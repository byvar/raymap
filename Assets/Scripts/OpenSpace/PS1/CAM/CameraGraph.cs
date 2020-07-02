using OpenSpace.Loader;
using System.Collections.Generic;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class CameraGraph : OpenSpaceStruct {
		public uint uint_00;
		public Pointer off_current;
		public Pointer off_last;
		public Pointer off_first;
		public uint flags;

		// Parsed
		public CameraGraphNode current;
		public CameraGraphNode first;
		public CameraGraphNode last;


		protected override void ReadInternal(Reader reader) {
			uint_00 = reader.ReadUInt32();
			off_current = Pointer.Read(reader);
			off_last = Pointer.Read(reader);
			off_first = Pointer.Read(reader);
			flags = reader.ReadUInt32();

			current = Load.FromOffsetOrRead<CameraGraphNode>(reader, off_current);
			first = Load.FromOffsetOrRead<CameraGraphNode>(reader, off_first);
			last = Load.FromOffsetOrRead<CameraGraphNode>(reader, off_last);
		}


		public GameObject GetGameObject(GameObject parent) {
			GameObject gao = new GameObject("Graph @ " + Offset);
			gao.transform.SetParent(parent.transform);
			gao.transform.localPosition = Vector3.zero;
			CameraGraphNode curNode = first;
			while (curNode != null) {
				GameObject nodeGao = curNode.GetGameObject(gao);
				curNode = curNode.next;
			}
			MeshRenderer mr = gao.GetComponent<MeshRenderer>();
			//gao.transform.localScale = Vector3.one * (type / 256f) * 2; // default Unity sphere radius is 0.5
			return gao;
		}

	}
}
