using OpenSpace;
using OpenSpace.Animation;
using OpenSpace.Cinematics;
using OpenSpace.Object;
using OpenSpace.Visual;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CinematicSwitcher : MonoBehaviour {
	bool loaded = false;
	CinematicsManager cinematicsManager;
	Cinematic cinematic;
	
	public string[] cinematicNames = { "Null" };
	int currentCinematic = 0;
	public int cinematicIndex = 0;

	public PersoBehaviour[] actors;

	// Use this for initialization
	void Start() {

	}

	public void Init() {
		cinematicsManager = MapLoader.Loader.cinematicsManager;
		Array.Resize(ref cinematicNames, cinematicsManager.cinematics.Count + 1);
		Array.Copy(cinematicsManager.cinematics.Select(c => (c == null ? "Null" : c.name)).ToArray(), 0, cinematicNames, 1, cinematicsManager.cinematics.Count);
		if (currentCinematic == -1) currentCinematic = 0;
		cinematicIndex = currentCinematic;
		loaded = true;
	}

	// Update is called once per frame
	void Update() {
		if (loaded) {
			if (cinematicIndex != currentCinematic) {
				currentCinematic = cinematicIndex;
				SetCinematic(currentCinematic);
			}
		}
	}

	public void SetCinematic(int index) {
		if (index < 0 || index > cinematicsManager.cinematics.Count) return;
		cinematicIndex = index;
		currentCinematic = index;
		index--;

		actors = new PersoBehaviour[0];
		if (cinematic != null) {
			foreach (CinematicActor actor in cinematic.actors) {
				PersoBehaviour pb = actor.perso.Gao.GetComponent<PersoBehaviour>();
				actor.cineState.anim_ref = AnimationReference.FromOffset(actor.cineState.off_anim_ref);
				pb.SetState(actor.waitState.index);
			}
			cinematic = null;
		}
		if(index != -1) {
			cinematic = cinematicsManager.cinematics[index];
			actors = cinematic.actors.Select(a => a.perso.Gao.GetComponent<PersoBehaviour>()).ToArray();
			foreach (CinematicActor actor in cinematic.actors) {
				PersoBehaviour pb = actor.perso.Gao.GetComponent<PersoBehaviour>();
				actor.cineState.anim_ref = actor.anim_ref;
				actor.cineState.speed = actor.cineStateSpeed;
				pb.SetState(actor.cineState.index);
			}
		}
	}
}