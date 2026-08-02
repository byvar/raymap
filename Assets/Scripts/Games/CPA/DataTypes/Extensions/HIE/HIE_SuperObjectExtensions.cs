using BinarySerializer.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BinarySerializer.Ubisoft.CPA {
	public static class HIE_SuperObjectExtensions {
		public static GameObject GetGameObject(this HIE_SuperObject spo) {
			GameObject gao = new GameObject($"{spo.LinkedObjectType} @ {spo.Offset}");
			gao.AddBinarySerializableData(spo);

			if (FileSystem.mode == FileSystem.Mode.Web) {
				gao.name = $"{spo.LinkedObjectType}";
			}

			/*SuperObjectComponent soc = gao.AddComponent<SuperObjectComponent>();
			gao.layer = LayerMask.NameToLayer("SuperObject");
			soc.soPS1 = spo;*/

			spo.ApplyMatrix(gao);
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
			if (spo.LinkedObjectType != HIE_ObjectType.IPO) {
				spo.LocalMatrix?.Value?.Apply(gao);
			} else {
				gao.transform.localPosition = Vector3.zero;
				gao.transform.localRotation = Quaternion.identity;
			}
		}
	}
}
