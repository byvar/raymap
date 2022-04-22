using System;
using BinarySerializer.Ubisoft.CPA.PS1;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raymap {
	public class Unity_Level_CPAPS1 : Unity_Level {
		public LevelHeader LevelHeader { get; set; }

		// Added by Init
		public List<HIE_SuperObject> SuperObjects { get; set; } = new List<HIE_SuperObject>();

		public override void Init() {
			// Collect all superobjects
			ProcessSuperObject(LevelHeader?.DynamicWorld, "Dynamic world");
			ProcessSuperObject(LevelHeader?.FatherSector, "Father sector");
			ProcessSuperObject(LevelHeader?.InactiveDynamicWorld, "Inactive dynamic world");

		}

		private void ProcessSuperObject(HIE_SuperObject spo, string name) {
			if(spo == null) return;
			if(!SuperObjects.Contains(spo)) SuperObjects.Add(spo);
			spo.GetGameObject();
		}
	}
}
