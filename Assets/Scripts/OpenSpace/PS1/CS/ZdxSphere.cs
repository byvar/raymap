﻿using OpenSpace.Loader;
using System.Collections.Generic;
using UnityEngine;
using CollideType = OpenSpace.Collide.CollideType;

namespace OpenSpace.PS1 {
	public class ZdxSphere : OpenSpaceStruct, IGeometricObjectElementCollide {
		public uint radius;
		public short x;
		public short y;
		public short z;
		public short padding;
		public uint gameMaterial;

		protected override void ReadInternal(Reader reader) {
			radius = reader.ReadUInt32();
			x = reader.ReadInt16();
			y = reader.ReadInt16();
			z = reader.ReadInt16();
			padding = reader.ReadInt16();
			gameMaterial = reader.ReadUInt32();
		}


		public GameObject GetGameObject(GameObject parent, int index, CollideType collideType = CollideType.None) {
			GameObject gao = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			gao.name = "Sphere " + index;
			gao.transform.SetParent(parent.transform);
			MeshRenderer mr = gao.GetComponent<MeshRenderer>();
			gao.transform.localPosition = new Vector3(x, z, y) / R2PS1Loader.CoordinateFactor;
			gao.transform.localScale = Vector3.one * (radius / R2PS1Loader.CoordinateFactor) * 2; // default Unity sphere radius is 0.5
			gao.layer = LayerMask.NameToLayer("Collide");

			BillboardBehaviour bill = gao.AddComponent<BillboardBehaviour>();
			bill.mode = BillboardBehaviour.LookAtMode.CameraPosXYZ;

			GameMaterial gm = (Load as R2PS1Loader).levelHeader.gameMaterials?[gameMaterial];
			CollideMaterial cm = gm?.collideMaterial;
			if (cm != null) {
				mr.material = cm.CreateMaterial();
			} else {
				mr.material = new Material(MapLoader.Loader.collideMaterial);
			}
			if (collideType != CollideType.None) {
				Color col = mr.material.color;
				mr.material = new Material(MapLoader.Loader.collideTransparentMaterial);
				mr.material.color = new Color(col.r, col.g, col.b, col.a * 0.7f);
				switch (collideType) {
					case CollideType.ZDD:
						mr.material.SetTexture("_MainTex", Resources.Load<Texture2D>("Textures/zdd")); break;
					case CollideType.ZDM:
						mr.material.SetTexture("_MainTex", Resources.Load<Texture2D>("Textures/zdm")); break;
					case CollideType.ZDE:
						mr.material.SetTexture("_MainTex", Resources.Load<Texture2D>("Textures/zde")); break;
					case CollideType.ZDR:
						mr.material.SetTexture("_MainTex", Resources.Load<Texture2D>("Textures/zdr")); break;
				}
			}
			CollideComponent cc = gao.AddComponent<CollideComponent>();
			cc.collidePS1 = this;
			cc.type = collideType;

			return gao;
		}

		public GameMaterial GetMaterial(int? index) {
			GameMaterial gm = (Load as R2PS1Loader).levelHeader.gameMaterials?[gameMaterial];
			return gm;
		}
	}
}
