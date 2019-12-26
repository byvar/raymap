using OpenSpace;
using OpenSpace.Loader;
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
	public List<MultiTextureMaterial> materials = new List<MultiTextureMaterial>();

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
		if (cont.LoadState == Controller.State.Finished) {
			/*if(MapLoader.Loader.visualMaterials != null) {
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
			}*/
			for (int i = 0; i < materials.Count; i++) {
				MultiTextureMaterial mtm = materials[i];
				if (mtm != null) {
					if (mtm.visMat != null) {
						VisualMaterial vm = mtm.visMat;
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
					} else if (mtm.visMatROM != null) {
						OpenSpace.ROM.VisualMaterial vm = mtm.visMatROM;
						if (vm.num_textures > 0) {// && !vm.IsLockedAnimatedTexture) {
							mtm.CurrentTextureROM %= vm.num_textures;
							mtm.CurrentTextureROMTime += updateCounter * modifier;
							float animTime = vm.textures.Value.vmTex[mtm.CurrentTextureROM].time / 30f;
							while (mtm.CurrentTextureROMTime > animTime) {
								float rest = mtm.CurrentTextureROMTime - animTime;
								//mtm.CurrentTextureROMTime = 0;
								if (animTime <= 0) break;
								mtm.CurrentTextureROM = (mtm.CurrentTextureROM + 1) % vm.num_textures;
								mtm.CurrentTextureROMTime = rest;
								animTime = vm.textures.Value.vmTex[mtm.CurrentTextureROM].time / 30f;
							}
						}
					}
				}
			}
		}
	}

	public void Register(MultiTextureMaterial mat) {
		materials.Add(mat);
	}
}
