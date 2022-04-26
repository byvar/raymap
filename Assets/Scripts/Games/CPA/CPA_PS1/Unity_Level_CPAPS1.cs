using System;
using BinarySerializer.Ubisoft.CPA.PS1;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace Raymap {
	public class Unity_Level_CPAPS1 : Unity_Level {
		public GAM_Level_PS1 LevelData { get; set; }

		// Added by Init
		public List<SuperObjectComponent> SuperObjects { get; set; } = new List<SuperObjectComponent>(); // To use when SuperObjectComponent is added to GetGameObject

		public override string EnvironmentKey => Unity_Environment_CPA.Key;

		public override void Init() {
			// Collect all superobjects
			ProcessSuperObject(LevelData?.GlobalPointerTable?.DynamicWorld, "Dynamic world");
			ProcessSuperObject(LevelData?.GlobalPointerTable?.FatherSector, "Father sector");
			ProcessSuperObject(LevelData?.GlobalPointerTable?.InactiveDynamicWorld, "Inactive dynamic world");

		}

		private void ProcessSuperObject(HIE_SuperObject spo, string name) {
			if(spo == null) return;
			var gao = spo.GetGameObject();
			if(gao != null) gao.name = name;
		}
	}
}
