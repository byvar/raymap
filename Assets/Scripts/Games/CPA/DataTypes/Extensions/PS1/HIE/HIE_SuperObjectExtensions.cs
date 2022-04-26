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
			gao.AddBinarySerializableData(spo);

			if (FileSystem.mode == FileSystem.Mode.Web) {
				gao.name = $"{spo.Type}";
			}

			/*SuperObjectComponent soc = gao.AddComponent<SuperObjectComponent>();
			gao.layer = LayerMask.NameToLayer("SuperObject");
			soc.soPS1 = spo;*/

			spo.ApplyMatrix(gao);
			GAM_GlobalPointerTable gpt = spo.Context.GetLevel().GlobalPointerTable;
			if (spo.Type == HIE_ObjectType_98.InstanciatedPhysicalObject) {
				PhysicalObjectComponent poc = gao.AddComponent<PhysicalObjectComponent>();
				int ind = (spo.DataIndex >> 1);
				if (ind >= gpt.StaticGeometricObjects.Entries.Length) throw new Exception("IPO SPO data index was too high! " + gpt.StaticGeometricObjects.Entries.Length + " - " + spo.DataIndex);
				gao.name = gao.name + " - " + ind;
				GameObject g = gpt.StaticGeometricObjects.GetGameObject(ind, null, out _);
				if (g != null) {
					poc.visual = g;
					g.transform.SetParent(gao.transform, false);
				}
				if (gpt.IpoCollision != null && gpt.IpoCollision.Length > ind) {
					GameObject c = gpt.IpoCollision[ind].GetGameObject();
					if (c != null) {
						poc.collide = c;
						c.transform.SetParent(gao.transform, false);					}
				}
				//poc.Init(OpenSpace.MapLoader.Loader.controller);
			} else if (spo.Type == HIE_ObjectType_98.Character) {
				PERSO_Perso p = (PERSO_Perso)spo.LinkedObject;
				if (spo.DataIndex >= gpt.Persos.Length) throw new Exception("Perso SPO data index was too high! " + gpt.Persos.Length + " - " + spo.DataIndex);
				gao.name = gao.name + " - " + spo.DataIndex;
				/*PS1PersoBehaviour ps1Perso = h.persos[dataIndex].GetGameObject(gao);
				ps1Perso.superObject = this;*/
			} else if (spo.Type == HIE_ObjectType_98.Sector) {
				SECT_Sector s = (SECT_Sector)spo.LinkedObject;
				if (spo.DataIndex >= gpt.Sectors.Length) throw new Exception("Sector SPO data index was too high! " + gpt.Sectors.Length + " - " + spo.DataIndex);
				gao.name = gao.name + " - " + spo.DataIndex;
				//SectorComponent sect = h.sectors[dataIndex].GetGameObject(gao);
			}
			if (spo.Children != null) {
				foreach (HIE_SuperObject spoChild in spo.Children) {
					if (spoChild != null) {
						GameObject gao_spoChild = spoChild.GetGameObject();
						if (gao_spoChild != null) {
							//soc.Children.Add(gao_spoChild.GetComponent<SuperObjectComponent>());
							gao_spoChild.transform.SetParent(gao.transform, false);
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
