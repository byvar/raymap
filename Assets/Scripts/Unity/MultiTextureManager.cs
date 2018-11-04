using OpenSpace;
using OpenSpace.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MultiTextureManager : MonoBehaviour {
	public Controller cont;
	public float modifier = 0.75f;
	public float animationSpeed = 60f; // Force 60fps w/ frameskip
	private float updateCounter = 0f;

	public void Update() {
		if(cont.playTextureAnimations) {
			updateCounter += Time.deltaTime;
			if (updateCounter >= 1/animationSpeed) {
				UpdateAnimations();
				updateCounter = 0;
			}
		}
    }

	void UpdateAnimations() {
		if (cont.LoadState == Controller.State.Finished && MapLoader.Loader.visualMaterials != null) {
			for (int i = 0; i < MapLoader.Loader.visualMaterials.Count; i++) {
				VisualMaterial vm = MapLoader.Loader.visualMaterials[i];
				if (vm.animTextures.Count > 0 && !vm.IsLockedAnimatedTexture) {
					vm.currentAnimTexture %= vm.animTextures.Count;
					vm.animTextures[vm.currentAnimTexture].currentTime += updateCounter * modifier;
					while (vm.animTextures[vm.currentAnimTexture].currentTime > vm.animTextures[vm.currentAnimTexture].time) {
						float rest = vm.animTextures[vm.currentAnimTexture].currentTime - vm.animTextures[vm.currentAnimTexture].time;
						vm.animTextures[vm.currentAnimTexture].currentTime = 0;
						if (vm.animTextures[vm.currentAnimTexture].time <= 0) break;
						vm.currentAnimTexture = (vm.currentAnimTexture + 1) % vm.animTextures.Count;
						vm.animTextures[vm.currentAnimTexture].currentTime = rest;
					}
				}
			}
		}
	}
}
