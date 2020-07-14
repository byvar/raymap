using OpenSpace;
using OpenSpace.Animation;
using OpenSpace.Cinematics;
using OpenSpace.Loader;
using OpenSpace.Object;
using OpenSpace.PS1;
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
	
	public string[] CinematicNames { get; private set; } = { "Null" };
	int currentCinematic = 0;
	public int CinematicIndex { get; set; } = 0;

	public PersoBehaviour[] actors;

	// PS1
	public uint currentFrame = 0;
	public bool playAnimation = true;
	public float animationSpeed = 30f;
	private float updateCounter = 0f;
	OpenSpace.PS1.PS1Stream[] ps1Streams;
	Dictionary<int, List<GameObject>> objectPool = new Dictionary<int, List<GameObject>>();
	List<GameObject> allNTTO = new List<GameObject>();
	List<GameObject> currentNTTO = new List<GameObject>();

	// Use this for initialization
	void Start() {

	}

	public void Init() {
		controller = MapLoader.Loader.controller;
		if (MapLoader.Loader is R2PS1Loader) {
			R2PS1Loader l = MapLoader.Loader as R2PS1Loader;
			ps1Streams = l.streams;
			string[] cinematicNames = CinematicNames;
			Array.Resize(ref cinematicNames, ps1Streams.Length + 1);
			CinematicNames = cinematicNames;

			for (int i = 0; i < ps1Streams.Length; i++) {
				CinematicNames[i + 1] = "Stream " + i;
				PS1GameInfo game = PS1GameInfo.Games[Settings.s.mode];
				if (game != null && game.cines !=null && game.cines.ContainsKey(l.lvlName)) {
					if (game.cines[l.lvlName].Length > i) {
						CinematicNames[i + 1] += ": " + game.cines[l.lvlName][i];
					}
				}
				if (l.levelHeader.initialStreamID == i) {
					CinematicNames[i + 1] += " (intro)";
				}
			}
		} else {
			cinematicsManager = MapLoader.Loader.cinematicsManager;
			string[] cinematicNames = CinematicNames;
			Array.Resize(ref cinematicNames, cinematicsManager.cinematics.Count + 1);
			CinematicNames = cinematicNames;
			Array.Copy(cinematicsManager.cinematics.Select(c => (c == null ? "Null" : c.name)).ToArray(), 0, CinematicNames, 1, cinematicsManager.cinematics.Count);
		}
		if (currentCinematic == -1) currentCinematic = 0;
		CinematicIndex = currentCinematic;
		loaded = true;
	}

	// Update is called once per frame
	void Update() {
		if (loaded) {
			if (CinematicIndex != currentCinematic) {
				currentCinematic = CinematicIndex;
				SetCinematic(currentCinematic);
			}
			if (ps1Streams != null && currentCinematic > 0) {
				if (Camera.main.orthographic) {
					CinematicIndex = 0;
					return;
				}
				UpdatePS1Stream();
			}
		}
	}

	public void SetCinematic(int index) {
		if (ps1Streams != null) {
			DeinitPS1Stream();
			if (index < 0 || index > ps1Streams.Length) return;
			CinematicIndex = index;
			currentCinematic = index;
			index--;
			InitPS1Stream();
			UpdatePS1StreamFrame();
		} else {
			if (index < 0 || index > cinematicsManager.cinematics.Count) return;
			CinematicIndex = index;
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

	public void InitPS1Stream() {
		DeinitPS1Stream();
		currentFrame = 0;
		if (currentCinematic > 0 && ps1Streams != null) {
			Camera cam = Camera.main;
			if (cam.orthographic) {
				controller.ApplyCameraSettings(controller.CameraSettings[WebJSON.CameraPos.Initial]);
			}
			OpenSpace.PS1.PS1Stream s = ps1Streams[currentCinematic - 1];
			R2PS1Loader l = MapLoader.Loader as R2PS1Loader;
			l.gao_dynamicWorld.SetActive(false);
			foreach (PS1StreamFrame f in s.frames) {
				Dictionary<int, int> nttoForFrame = new Dictionary<int, int>();
				for(int i = 0; i < f.num_channels; i++) {
					PS1StreamFrameChannel c = f.channels[i];
					int ntto = c.NTTO;
					/*if (c.HasFlag(PS1StreamFrameChannel.StreamFlags.Parent)) {
						Debug.Log(string.Format("{0:X4}", c.flags));
					}*/
					if (ntto >= 0) {
						/*if (nttoPerChannel[i] == null) nttoPerChannel[i] = new Dictionary<int, GameObject>();

						if (!nttoPerChannel[i].ContainsKey(ntto)) {
							GameObject gao = l.levelHeader.geometricObjectsDynamic.GetGameObject(ntto);
							if (gao == null) gao = new GameObject("Empty 2");
							gao.name = string.Format("{0:X4}", c.flags) + " - " + gao.name;
							gao.transform.SetParent(transform);
							gao.SetActive(false);
						}*/
						if (!nttoForFrame.ContainsKey(ntto)) nttoForFrame[ntto] = 0;
						nttoForFrame[ntto]++;
						if (!objectPool.ContainsKey(ntto) || objectPool[ntto].Count < nttoForFrame[ntto]) {
							GameObject gao = l.levelHeader.geometricObjectsDynamic.GetGameObject(ntto, null, out _);
							//if (gao == null) gao = new GameObject("Empty 2");
							if (gao != null) {
								gao.name = string.Format("{0:X4}", c.flags) + " - " + gao.name;
								gao.transform.SetParent(transform);
								gao.SetActive(false);
								allNTTO.Add(gao);
								if (!objectPool.ContainsKey(ntto)) {
									objectPool[ntto] = new List<GameObject>();
								}
								objectPool[ntto].Add(gao);
							}
						}
					}
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

	private void DeinitPS1Stream() {
		foreach (GameObject gao in allNTTO) {
			Destroy(gao);
		}
		allNTTO.Clear();
		currentNTTO.Clear();
		objectPool.Clear();
		R2PS1Loader l = MapLoader.Loader as R2PS1Loader;
		l.gao_dynamicWorld.SetActive(true);
	}

	private void ClearCurrentNTTO() {
		foreach (GameObject gao in currentNTTO) {
			if (gao != null) gao.SetActive(false);
		}
		currentNTTO.Clear();
	}

	public void UpdatePS1StreamFrame() {
		ClearCurrentNTTO();
		if (Camera.main.orthographic) {
			CinematicIndex = 0;
			return;
		}
		if (ps1Streams != null && currentCinematic > 0) {
			OpenSpace.PS1.PS1Stream s = ps1Streams[currentCinematic - 1];
			OpenSpace.PS1.PS1StreamFrame f = s.frames.FirstOrDefault(fr => fr.num_frame == currentFrame);
			if (f != null) {
				Dictionary<int, int> nttoForFrame = new Dictionary<int, int>();
				R2PS1Loader l = MapLoader.Loader as R2PS1Loader;
				Camera cam = Camera.main;
				for (int i = 0; i < f.channels.Length; i++) {
					OpenSpace.PS1.PS1StreamFrameChannel c = f.channels[i];
					if (c.HasFlag(OpenSpace.PS1.PS1StreamFrameChannel.StreamFlags.Camera)) {
						cam.transform.position = c.GetPosition();
						cam.transform.rotation = c.quaternion * Quaternion.Euler(0, 180, 0);
					} else {
						GameObject gao = null;
						int ntto = c.NTTO;
						if (ntto >= 0) {
							if (!nttoForFrame.ContainsKey(ntto)) nttoForFrame[ntto] = 0;
							if (objectPool.ContainsKey(ntto)) {
								gao = objectPool[ntto][nttoForFrame[ntto]];
							}
							if (gao != null) {
								gao.SetActive(true);
								currentNTTO.Add(gao);
								nttoForFrame[ntto]++;
							}
						}
						/*if (c.NTTO > 0) {
							gao = l.levelHeader.geometricObjectsDynamic.GetGameObject(c.NTTO);
							if (gao == null) gao = new GameObject("Empty 2");
						} else {
							gao = new GameObject("Empty");
						}*/

						/*if (c.HasFlag(PS1StreamFrameChannel.StreamFlags.Parent)) {
							Debug.Log(string.Format("{0:X4}", c.flags));
						}*/
						if (gao != null) {
							Vector3 scale = new Vector3(1f, 1f, 1f);
							gao.transform.localPosition = cam.transform.localPosition + c.GetPosition();
							gao.transform.localRotation = c.quaternion;
							if (c.HasFlag(OpenSpace.PS1.PS1StreamFrameChannel.StreamFlags.Scale)) {
								scale = c.GetScale(0x1000);
								//gao.name = string.Format("{0:X4}", c.sx) + string.Format("{0:X4}", c.sy) + string.Format("{0:X4}", c.sz) + " - " + gao.name;
							}
							if (c.HasFlag(OpenSpace.PS1.PS1StreamFrameChannel.StreamFlags.FlipX)) {
								gao.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
							} else {
								gao.transform.localScale = scale;
							}
							//gao.transform.localScale = c.GetScale();
							currentNTTO.Add(gao);
						}
					}
				}
			}
		}
	}
}