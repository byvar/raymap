using OpenSpace;
using OpenSpace.Animation;
using OpenSpace.Cinematics;
using OpenSpace.Loader;
using OpenSpace.Object;
using OpenSpace.Visual;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CinematicSwitcher : MonoBehaviour {
	bool loaded = false;
	Controller controller;
	CinematicsManager cinematicsManager;
	Cinematic cinematic;
	
	public string[] cinematicNames = { "Null" };
	int currentCinematic = 0;
	public int cinematicIndex = 0;

	public PersoBehaviour[] actors;

	// PS1
	public uint currentFrame = 0;
	public bool playAnimation = true;
	public float animationSpeed = 30f;
	private float updateCounter = 0f;
	OpenSpace.PS1.PS1Stream[] ps1Streams;

	// Use this for initialization
	void Start() {

	}

	public void Init() {
		controller = MapLoader.Loader.controller;
		if (MapLoader.Loader is R2PS1Loader) {
			R2PS1Loader l = MapLoader.Loader as R2PS1Loader;
			ps1Streams = l.streams;
			Array.Resize(ref cinematicNames, ps1Streams.Length + 1);

			for (int i = 0; i < ps1Streams.Length; i++) {
				cinematicNames[i + 1] = "Stream " + i;
				if (l.levelHeader.initialStreamID == i) {
					cinematicNames[i + 1] = "Stream " + i + " (intro)";
				}
			}
		} else {
			cinematicsManager = MapLoader.Loader.cinematicsManager;
			Array.Resize(ref cinematicNames, cinematicsManager.cinematics.Count + 1);
			Array.Copy(cinematicsManager.cinematics.Select(c => (c == null ? "Null" : c.name)).ToArray(), 0, cinematicNames, 1, cinematicsManager.cinematics.Count);
		}
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
			if (ps1Streams != null && currentCinematic > 0) {
				UpdatePS1Stream();
			}
		}
	}

	public void SetCinematic(int index) {
		if (ps1Streams != null) {
			if (index < 0 || index > ps1Streams.Length) return;
			cinematicIndex = index;
			currentCinematic = index;
			index--;
			currentFrame = 0;
			UpdatePS1StreamFrame();
		} else {
			if (index < 0 || index > cinematicsManager.cinematics.Count) return;
			cinematicIndex = index;
			currentCinematic = index;
			index--;

			actors = new PersoBehaviour[0];
			if (cinematic != null) {
				foreach (CinematicActor actor in cinematic.actors) {
					PersoBehaviour pb = actor.perso.Gao.GetComponent<PersoBehaviour>();
					actor.cineState.anim_ref = MapLoader.Loader.FromOffset<AnimationReference>(actor.cineState.off_anim_ref);
					pb.SetState(actor.waitState.index);
				}
				cinematic = null;
			}
			if (index != -1) {
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

	public void UpdatePS1Stream() {
		if (controller.playAnimations && playAnimation && currentCinematic > 0 && ps1Streams != null) {
			updateCounter += Time.deltaTime * animationSpeed;
			// If the camera is not inside a sector, animations will only update 1 out of 2 times (w/ frameskip) to avoid lag
			if (updateCounter >= 1f) {
				uint passedFrames = (uint)Mathf.FloorToInt(updateCounter);
				updateCounter %= 1;
				currentFrame += passedFrames;
				OpenSpace.PS1.PS1Stream s = ps1Streams[currentCinematic - 1];
				if (s != null && currentFrame >= s.num_frames) {
					currentFrame = (uint)(currentFrame % s.num_frames);
					UpdatePS1StreamFrame();
				} else {
					UpdatePS1StreamFrame();
				}
			}
		}
	}

	public void UpdatePS1StreamFrame() {
		if (ps1Streams != null && currentCinematic > 0) {
			OpenSpace.PS1.PS1Stream s = ps1Streams[currentCinematic - 1];
			OpenSpace.PS1.PS1StreamFrame f = s.frames.LastOrDefault(fr => fr.num_frame >= 0 && fr.num_frame <= currentFrame);
			if (f != null) {
				for (int i = 0; i < f.channels.Length; i++) {
					OpenSpace.PS1.PS1StreamFrameChannel c = f.channels[i];
					if (c.HasFlag(OpenSpace.PS1.PS1StreamFrameChannel.StreamFlags.Camera)) {
						Camera cam = Camera.main;
						cam.transform.position = c.GetPosition();
						cam.transform.rotation = c.quaternion * Quaternion.Euler(0,180,0);
					}
				}
			}
		}
	}
}