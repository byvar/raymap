using BinarySerializer.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BinarySerializer.Ubisoft.CPA.PS1 {
	public static class HIE_SuperObjectExtensions {
		public static GameObject GetGameObject(this HIE_SuperObject spo) {
			GameObject gao = new GameObject($"{spo.Type} @ {spo.Offset}");
			
			if (FileSystem.mode == FileSystem.Mode.Web) {
				gao.name = $"{spo.Type}";
			}

			/*SuperObjectComponent soc = gao.AddComponent<SuperObjectComponent>();
			gao.layer = LayerMask.NameToLayer("SuperObject");
			soc.soPS1 = spo;*/

			spo.ApplyMatrix(gao);
			/*if (type == Type.IPO) {
				PhysicalObjectComponent poc = gao.AddComponent<PhysicalObjectComponent>();
				LevelHeader h = (Load as R2PS1Loader).levelHeader;
				int ind = (dataIndex >> 1);
				if (ind >= h.geometricObjectsStatic.entries.Length) throw new Exception("IPO SO data index was too high! " + h.geometricObjectsStatic.entries.Length + " - " + dataIndex);
				gao.name = gao.name + " - " + ind;
				GameObject g = h.geometricObjectsStatic.GetGameObject(ind, null, out _);
				if (g != null) {
					poc.visual = g;
					g.transform.SetParent(gao.transform);
					g.transform.localPosition = Vector3.zero;
					g.transform.localRotation = Quaternion.identity;
				}
				if (h.ipoCollision != null && h.ipoCollision.Length > ind) {
					GameObject c = h.ipoCollision[ind].GetGameObject();
					if (c != null) {
						poc.collide = c;
						c.transform.SetParent(gao.transform);
						c.transform.localPosition = Vector3.zero;
						c.transform.localRotation = Quaternion.identity;
					}
				}
				poc.Init(MapLoader.Loader.controller);
			} else if (type == Type.Perso) {
				LevelHeader h = (Load as R2PS1Loader).levelHeader;
				if (dataIndex >= h.persos.Length) throw new Exception("Perso SO data index was too high! " + h.persos.Length + " - " + dataIndex);
				gao.name = gao.name + " - " + dataIndex;
				PS1PersoBehaviour ps1Perso = h.persos[dataIndex].GetGameObject(gao);
				ps1Perso.superObject = this;
			} else if (type == Type.Sector) {
				LevelHeader h = (Load as R2PS1Loader).levelHeader;
				if (dataIndex >= h.sectors.Length) throw new Exception("Sector SO data index was too high! " + h.sectors.Length + " - " + dataIndex);
				gao.name = gao.name + " - " + dataIndex;
				SectorComponent sect = h.sectors[dataIndex].GetGameObject(gao);
			}*/
			if (spo.Children != null) {
				foreach (HIE_SuperObject spoChild in spo.Children) {
					if (spoChild != null) {
						GameObject gao_spoChild = spoChild.GetGameObject();
						if (gao_spoChild != null) {
							//soc.Children.Add(gao_spoChild.GetComponent<SuperObjectComponent>());
							gao_spoChild.transform.SetParent(gao.transform);
							spoChild.ApplyMatrix(gao_spoChild);
						}
					}
				}
			}
			return gao;
		}

		public static void ApplyMatrix(this HIE_SuperObject spo, GameObject gao) {
			if (spo.Type != HIE_ObjectType_98.InstanciatedPhysicalObject) {
				spo.LocalMatrix?.Value?.Apply(gao);
			} else {
				gao.transform.localPosition = Vector3.zero;
				gao.transform.localRotation = Quaternion.identity;
			}
		}
	}
}
