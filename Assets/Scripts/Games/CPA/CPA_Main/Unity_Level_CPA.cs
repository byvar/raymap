using System;
using BinarySerializer.Ubisoft.CPA;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace Raymap {
	public class Unity_Level_CPA : Unity_Level {
		public CPA_Globals_SNA LevelData { get; set; }

		// Added by Init
		public List<SuperObjectComponent> SuperObjects { get; set; } = new List<SuperObjectComponent>(); // To use when SuperObjectComponent is added to GetGameObject

		public override string EnvironmentKey => Unity_Environment_CPA.Key;

		public override void Init() {
			// Collect all superobjects
			ProcessSuperObject(LevelData?.GlobalPointers_Level?.ActualWorld, "Actual world");
			ProcessSuperObject(LevelData?.GlobalPointers_Level?.DynamicWorld, "Dynamic world");
			ProcessSuperObject(LevelData?.GlobalPointers_Level?.FatherSector, "Father sector");
			ProcessSuperObject(LevelData?.GlobalPointers_Level?.InactiveDynamicWorld, "Inactive dynamic world");
		}

		private void ProcessSuperObject(HIE_SuperObject spo, string name) {
			if(spo == null) return;
			var gao = spo.GetGameObject();
			if(gao != null) gao.name = name;
		}
	}
}
