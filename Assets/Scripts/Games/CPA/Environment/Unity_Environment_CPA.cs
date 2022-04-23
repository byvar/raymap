using UnityEngine;

namespace Raymap {
	public class Unity_Environment_CPA : Unity_Environment {
		public const string Key = "CPA";

		// Set in inspector
		public Material MaterialVisualOpaque;
		public Material MaterialVisualTransparent;
		public Material MaterialVisualLight;
		public Material MaterialCollision;
		public Material MaterialCollisionTransparent;

		public override void Init() {
			// TODO: Initialize monobehaviours etc
			//throw new System.NotImplementedException();
		}

		public override void OnDataLoaded() {
			//throw new System.NotImplementedException();
		}
	}
}
