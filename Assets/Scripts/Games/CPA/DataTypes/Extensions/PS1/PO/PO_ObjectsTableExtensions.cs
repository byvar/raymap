using BinarySerializer.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BinarySerializer.Ubisoft.CPA.PS1 {
	public static class PO_ObjectsTableExtensions {
		public static GameObject GetGameObject(this PO_ObjectsTable poTable, int i, CS_PhysicalObjectCollisionMapping[] collision, out GameObject[] bones, int? morphI = null, float morphProgress = 0f) {
			bones = null;
			if (i < 0 || i >= poTable.Entries.Length) return null;
			return poTable.Entries[i]?.GetGameObject(
				collision,
				out bones,
				morphPO: ((morphI != null && morphI.Value >= 0 && morphI.Value < poTable.Entries.Length) ? poTable.Entries[morphI.Value] : null),
				morphProgress: morphProgress);
		}

		public static GameObject GetGameObject(this PO_PhysicalObject po, CS_PhysicalObjectCollisionMapping[] collision, out GameObject[] bones, PO_PhysicalObject morphPO = null, float morphProgress = 0f) {
			bones = null;
			GameObject gao = po.GeometricObject?.GetGameObject(out bones, morphObject: morphPO?.GeometricObject, morphProgress: morphProgress);
			if (gao != null) {
				GameObject wrapper = new GameObject(gao.name + " - Wrapper");
				if (morphProgress > 0 || morphPO != null) {
					gao.name += " - Morph Progress: " + morphProgress;
				}
				gao.transform.SetParent(wrapper.transform);
				gao.transform.localPosition = Vector3.zero;
				gao.transform.localRotation = Quaternion.identity;
				gao.transform.localScale = Vector3.one;
				PhysicalObjectComponent poc = wrapper.AddComponent<PhysicalObjectComponent>();
				poc.visual = gao;
				if (collision != null) {
					CS_PhysicalObjectCollisionMapping cm = collision.FirstOrDefault(c => c.POListEntryPointer == po.Offset);
					if (cm != null && cm.Collision != null) {
						GameObject cgao = cm.Collision.GetGameObject();
						cgao.transform.SetParent(wrapper.transform);
						cgao.transform.localPosition = Vector3.zero;
						cgao.transform.localRotation = Quaternion.identity;
						cgao.transform.localScale = Vector3.one;
						poc.collide = cgao;
					}
				}
				//poc.Init(OpenSpace.MapLoader.Loader.controller);
				return wrapper;
			} else return gao;
		}
	}
}
