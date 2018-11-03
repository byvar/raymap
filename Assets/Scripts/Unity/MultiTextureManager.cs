using OpenSpace;
using OpenSpace.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MultiTextureManager : MonoBehaviour {
	public Controller cont;

    public void Update() {
		if(cont.playTextureAnimations) {
			if (cont.LoadState == Controller.State.Finished && MapLoader.Loader.visualMaterials != null) {
				for (int i = 0; i < MapLoader.Loader.visualMaterials.Count; i++) {
					VisualMaterial vm = MapLoader.Loader.visualMaterials[i];
					if (vm.animTextures.Count > 0 && !vm.IsLockedAnimatedTexture) {
						vm.currentAnimTexture %= vm.animTextures.Count;
						vm.animTextures[vm.currentAnimTexture].currentTime += Time.deltaTime /2f;
						if (vm.animTextures[vm.currentAnimTexture].currentTime > vm.animTextures[vm.currentAnimTexture].time) {
							//float rest = vm.animTextures[vm.currentAnimTexture].currentTime - vm.animTextures[vm.currentAnimTexture].time;
							vm.animTextures[vm.currentAnimTexture].currentTime = 0;
							vm.currentAnimTexture = (vm.currentAnimTexture + 1) % vm.animTextures.Count;
							vm.animTextures[vm.currentAnimTexture].currentTime = 0;
						}
					}
				}
			}
		}
    }
}
