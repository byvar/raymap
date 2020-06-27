using OpenSpace.Loader;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class CollideMaterial : ROMStruct {
		public ushort type;
		public ushort identifier;

		protected override void ReadInternal(Reader reader) {
			identifier = reader.ReadUInt16();
			type = reader.ReadUInt16();
		}

		public bool Slide { get { return GetFlag(00); } set { SetFlag(00, value); } }
		public bool Trampoline { get { return GetFlag(01); } set { SetFlag(01, value); } }
		public bool GrabbableLedge { get { return GetFlag(02); } set { SetFlag(02, value); } }
		public bool Wall { get { return GetFlag(03); } set { SetFlag(03, value); } }
		public bool FlagUnknown { get { return GetFlag(04); } set { SetFlag(04, value); } }
		public bool HangableCeiling { get { return GetFlag(05); } set { SetFlag(05, value); } }
		public bool ClimbableWall { get { return GetFlag(06); } set { SetFlag(06, value); } }
		public bool Electric { get { return GetFlag(07); } set { SetFlag(07, value); } }
		public bool LavaDeathWarp { get { return GetFlag(08); } set { SetFlag(08, value); } }
		public bool FallTrigger { get { return GetFlag(09); } set { SetFlag(09, value); } }
		public bool HurtTrigger { get { return GetFlag(10); } set { SetFlag(10, value); } }
		public bool DeathWarp { get { return GetFlag(11); } set { SetFlag(11, value); } }
		public bool FlagUnk2 { get { return GetFlag(12); } set { SetFlag(12, value); } }
		public bool FlagUnk3 { get { return GetFlag(13); } set { SetFlag(13, value); } }
		public bool Water { get { return GetFlag(14); } set { SetFlag(14, value); } }
		public bool NoCollision { get { return GetFlag(15); } set { SetFlag(15, value); } }

		public void SetFlag(int index, bool value) {
			BitArray bitArray = new BitArray(BitConverter.GetBytes(identifier));
			bitArray.Set(index, value);
			ushort[] array = new ushort[1];
			bitArray.CopyTo(array, 0);
			this.identifier = array[0];
		}

		public bool GetFlag(int index) {
			BitArray bitArray = new BitArray(BitConverter.GetBytes(identifier));
			return bitArray.Get(index);
		}

		public void SetMaterial(MeshRenderer mr) {
			mr.material = new Material(MapLoader.Loader.collideMaterial);
			if (NoCollision) {
				mr.material = new Material(MapLoader.Loader.collideTransparentMaterial);
				//mr.material.SetTexture("_MainTex", Util.CreateDummyCheckerTexture());
				mr.material.color = new Color(1, 1, 1, 0.3f); // transparent cyan
			}
			if (Slide) mr.material.color = Color.blue;
			if (Water) {
				mr.material = new Material(MapLoader.Loader.collideTransparentMaterial);
				//mr.material.SetTexture("_MainTex", Util.CreateDummyCheckerTexture());
				mr.material.color = new Color(0, 1, 1, 0.5f); // transparent cyan
			}
			if (ClimbableWall || HangableCeiling) {
				mr.material.color = new Color(244f / 255f, 131f / 255f, 66f / 255f); // ORANGE
			}
			if (LavaDeathWarp || DeathWarp) {
				mr.material.color = Color.red;
			}
			if (HurtTrigger) mr.material.color = new Color(126 / 255f, 2 / 255f, 204 / 255f); // purple
			if (FallTrigger) mr.material.color = Color.black;
			if (Trampoline) mr.material.color = Color.yellow;
			if (Electric) mr.material.color = new Color(219f / 255f, 140 / 255f, 212 / 255f); // pink
			if (Wall) mr.material.color = Color.grey;
			if (GrabbableLedge) mr.material.color = Color.green;
			if (FlagUnknown || FlagUnk2 || FlagUnk3) mr.material.color = new Color(124 / 255f, 68 / 255f, 33 / 255f); // brown
		}
	}
}
