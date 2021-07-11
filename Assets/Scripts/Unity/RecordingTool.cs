using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using OpenSpace;
using OpenSpace.AI;
using OpenSpace.FileFormat;
using OpenSpace.Object;
using OpenSpace.Object.Properties;
using UnityEngine;
using SuperObject = OpenSpace.Object.SuperObject;

public class RecordingTool : MonoBehaviour
{
    public enum RecordingState
    {
        Idle,
        Recording,
        Playback
    }

    public RecordingState State
    {
        get;
        private set;
    }

    public float CurrentTime
    {
        get => _currentTime;
        set
        {
            _currentTime = Mathf.Clamp(value, 0, Data?.Duration ?? 0);
        }
    }

    private float _currentTime;

    private float LastRecordingTime = 0.0f;

    public RecordingData.Frame CurrentFrame => Data.Frames.First(f => f.Time >= CurrentTime);
    public RecordingData.Frame LastFrame(float currentFrameTime) => Data.Frames.LastOrDefault(f => f.Time < currentFrameTime);

    public bool AlignCameraToGame;
    public bool InterpolateTransforms = true;

    public bool IsPlaying { get; private set; }
    public float PlaybackSpeed = 1.0f;
    public float MaxRecordingFps = 60.0f;

    public RecordingData Data;

    public GameObject RecordingSpawnablesRoot;
    public GameObject PlaybackSpawnablesRoot;

    public class RecordingData
    {
        public struct Frame
        {
            public float Time;
            public FrameData Data;

            public Frame(float time, FrameData data)
            {
                Time = time;
                Data = data;
            }
        }

        public class FrameData
        {
            public Dictionary<string, PersoData> PersoData { get; private set; }

            public FrameData(Dictionary<string, PersoData> data)
            {
                PersoData = data;
            }
        }

        public class PersoData
        {
            public PersoData(Vector3 position, Quaternion rotation, int animationIndex, int animationFrame, int ruleIndex, int reflexIndex)
            {
                Position = position;
                Rotation = rotation;
                AnimationIndex = animationIndex;
                AnimationFrame = animationFrame;
                RuleIndex = ruleIndex;
                ReflexIndex = reflexIndex;
            }

            public float posX;
            public float posY;
            public float posZ;

            public float rotX;
            public float rotY;
            public float rotZ;
            public float rotW;

            [JsonIgnore]
            public Vector3 Position
            {
                get => new Vector3(posX, posY, posZ);
                set {
                    posX = value.x;
                    posY = value.y;
                    posZ = value.z;
                }
            }

            [JsonIgnore]
            public Quaternion Rotation
            {
                get => new Quaternion(rotX, rotY, rotZ, rotW);
                set
                {
                    rotX = value.x;
                    rotY = value.y;
                    rotZ = value.z;
                    rotW = value.w;
                }
            }
            public int AnimationIndex;
            public int AnimationFrame;

            public int RuleIndex;
            public int ReflexIndex;
        }

        // (frame, (objName, data))
        public List<Frame> Frames = new List<Frame>();
        public List<string> PersoNames { get; private set; }
        public float Duration => Frames.LastOrDefault().Time;

        public RecordingData(MapLoader loader)
        {
            PersoNames = loader.persos.Select(p => p.namePerso).Where(n=>!string.IsNullOrWhiteSpace(n)).ToList();
        }
    }

    private Dictionary<string, PersoBehaviour> nameToPersoMap;

    private void UpdateLivePreview( PersoBehaviour pb, Reader reader)
    {
        var spo = pb.perso.SuperObject;

        pb.IsEnabled = true;

        if (spo != null) {
            // Update gameobject being active
            MapLoader.Loader.controller.UpdatePersoActive(pb);

            Pointer.Goto(ref reader, pb.perso.off_stdGame);
            // Update stdgame
            pb.perso.stdGame = StandardGame.Read(reader, pb.perso.off_stdGame);
            // If stdgame says the object is inactive, deactivate gao too (ignore camera)
            if (pb.perso.off_camera==null && !pb.perso.stdGame.IsActive()) {
                pb.gameObject.SetActive(false);
            }

            if (pb.gameObject.activeSelf) {
                var mind = pb.brain?.mind?.mind;
                if (mind != null) {
                    Pointer.DoAt(ref reader, mind.Offset, () => { mind.UpdateCurrentBehaviors(reader); }); // TODO: add a toggle to not record behaviors
                }
            }

            Pointer.Goto(ref reader, spo.off_staticMatrix);
            spo.matrix = Matrix.Read(MapLoader.Loader.livePreviewReader, spo.off_staticMatrix);
            if (spo.data != null && spo.data.Gao != null) {

                var gao = spo.data.Gao;

                gao.transform.position = spo.matrix.GetPosition(convertAxes: true);
                gao.transform.rotation = spo.matrix.GetRotation(convertAxes: true);
                gao.transform.localScale = spo.matrix.GetScale(convertAxes: true);

                var perso = pb.perso;

                Pointer.Goto(ref reader, perso.p3dData.offset);
                perso.p3dData.UpdateCurrentState(reader);

                // State offset changed?
                if (perso.p3dData.stateCurrent != null) {
                    if (pb.stateIndex != perso.p3dData.stateCurrent.index) {
                        pb.SetState(perso.p3dData.stateCurrent.index);
                        pb.autoNextState = false;
                    }
                }

            }
        }
    }

    public void StartRecording()
    {
        if (RecordingSpawnablesRoot == null) {
            RecordingSpawnablesRoot = new GameObject("Recording Spawnables");
        }

        SpawnedAlwaysObjects = new Dictionary<Pointer, PersoBehaviour>();

        if (State != RecordingState.Recording) {
            Data = new RecordingData(MapLoader.Loader);
            State = RecordingState.Recording;

            nameToPersoMap = new Dictionary<string, PersoBehaviour>();
            foreach (var n in Data.PersoNames) {
                nameToPersoMap.Add(n, MapLoader.Loader.persos.First(p=>string.Equals(p.namePerso,n)).Gao.GetComponent<PersoBehaviour>());
            }
        }
    }

    public void StopRecording()
    {
        if (State == RecordingState.Recording) {

            State = RecordingState.Idle;
            StartPlayback();
        }
    }

    private void StartPlayback()
    {
        if (PlaybackSpawnablesRoot == null) {
            PlaybackSpawnablesRoot = new GameObject("Playback Spawnables");
        }

        if (State != RecordingState.Playback && Data != null) {
            CurrentTime = 0.0f;

            State = RecordingState.Playback;
        }
    }

    // Update is called once per frame
    void Update()
    {

        switch (State) {
            case RecordingState.Recording: HandleStateRecording(); break;
            case RecordingState.Playback: HandleStatePlayback(); break;
        }
    }

    private void HandleStatePlayback()
    {
        var currentFrame = CurrentFrame;
        var lastFrame = LastFrame(currentFrame.Time);

        if (IsPlaying) {
            CurrentTime += Time.deltaTime * PlaybackSpeed;
        }

        foreach (var kv in nameToPersoMap) {
            var pb = kv.Value;
            pb.playAnimation = IsPlaying;
            pb.AlwaysPlayAnimation = true;
            pb.IsEnabled = true;

            var data = currentFrame.Data.PersoData[kv.Key];
            var lastData = lastFrame.Data?.PersoData?[kv.Key];

            pb.gameObject.SetActive(data != null);
            if (data != null) {

                float interpFactor = (CurrentTime - lastFrame.Time) / (currentFrame.Time - lastFrame.Time);

                var interpPosition = lastData != null ? Vector3.Lerp(lastData.Position, data.Position, interpFactor) : data.Position;
                var interpRotation = lastData != null ? Quaternion.Lerp(lastData.Rotation, data.Rotation, interpFactor) : data.Rotation;

                UpdatePerso(pb, interpPosition, interpRotation, data.AnimationIndex, data.AnimationFrame,
                        data.RuleIndex, data.ReflexIndex);
            }
        }
    }

    private void HandleStateRecording()
    {
        if (Time.time < LastRecordingTime + (1.0f/MaxRecordingFps)) {
            return;
        }

        LastRecordingTime = Time.time;

        var livePreviewReader = MapLoader.Loader.livePreviewReader;

        ReadAlways(livePreviewReader);

        var data = new Dictionary<string, RecordingData.PersoData>();

        foreach (var kv in nameToPersoMap) {
            var pb = kv.Value;
            pb.playAnimation = true;
            pb.AlwaysPlayAnimation = true;

            if (livePreviewReader != null) {

                UpdateLivePreview(pb, livePreviewReader);
            }

            var newData = GetFrameData(livePreviewReader, kv.Value);
            data.Add(kv.Key, newData);

            if (newData != null) {
                UpdatePerso(pb, newData.Position, newData.Rotation, newData.AnimationIndex, newData.AnimationFrame, newData.RuleIndex, newData.ReflexIndex);
            }
        }

        foreach (var alw in SpawnedAlwaysObjects) {
            alw.Value.IsAlways = true;
            UpdateLivePreview(alw.Value, livePreviewReader);
        }

        Data.Frames.Add(new RecordingData.Frame(CurrentTime, new RecordingData.FrameData(data)));

        _currentTime += Time.deltaTime;
    }

    private RecordingData.PersoData GetFrameData(Reader reader, PersoBehaviour pb)
    {
        if (!pb.gameObject.activeSelf) {
            return null;
        }

        int ruleIndex = -1;
        int reflexIndex = -1;

        var mind = pb.brain?.mind?.mind;

        if (mind != null) {
            if (pb.brain?.Intelligence != null && mind.intelligenceNormal?.comport != null) {

                var ruleIntelligence =
                    pb.brain.Intelligence.FirstOrDefault(i => i.Offset == mind.intelligenceNormal.comport.Offset);
                ruleIndex = pb.brain.Intelligence.IndexOf(ruleIntelligence);
            }

            if (pb.brain?.Reflex != null && mind.intelligenceReflex?.comport != null) {

                var reflexIntelligence =
                    pb.brain.Reflex.FirstOrDefault(i => i.Offset == mind.intelligenceReflex.comport.Offset);
                reflexIndex = pb.brain.Reflex.IndexOf(reflexIntelligence);
            }
        }

        var gao = pb.gameObject;
        return new RecordingData.PersoData(
            gao.transform.position,
            gao.transform.rotation,
            pb.currentState,
            (int)pb.currentFrame,
            ruleIndex,
            reflexIndex);
    }

    private bool IsValidPos(Vector3 pos)
    {
        float limit = 100000;
        return Mathf.Abs(pos.x) < limit && Mathf.Abs(pos.y) < limit && Mathf.Abs(pos.z) < limit;
    }

    private void UpdatePerso(PersoBehaviour pb, Vector3 pos, Quaternion rotation, int animIndex, int animFrame, int ruleIndex, int reflexIndex)
    {
        pb.gameObject.transform.position = pos;
        pb.gameObject.transform.rotation = rotation;
        pb.SetState(animIndex);
        pb.currentFrame = (uint)animFrame;

        if (ruleIndex >= 0 || reflexIndex >= 0) {
            if (pb.brain?.mind?.mind != null) {

                var l = MapLoader.Loader;

                var mind = pb.brain.mind.mind;
                if (ruleIndex >= 0) {
                    mind.intelligenceNormal.comport = l.FromOffset<Behavior>(pb.brain.Intelligence[ruleIndex].Offset);
                }
                if (reflexIndex >= 0) {
                    mind.intelligenceReflex.comport = l.FromOffset<Behavior>(pb.brain.Reflex[reflexIndex].Offset);
                }
            }
        }

        if (AlignCameraToGame && string.Equals(pb.NameModel.ToLower(), "stdcam")) {
            Camera.main.transform.position = pb.gameObject.transform.position;
            Camera.main.transform.rotation = pb.gameObject.transform.rotation * Quaternion.Euler(0, 180, 0);
        }
    }

    public void NextFrame()
    {
        if (Data != null) {

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (CurrentTime == Data.Frames.LastOrDefault().Time) {
                return;
            }

            CurrentTime = Data.Frames.FirstOrDefault(f => f.Time > CurrentTime).Time;
        }
    }

    public void PreviousFrame()
    {
        if (Data != null) {

            if (CurrentTime == 0) {
                return;
            }

            CurrentTime = Data.Frames.LastOrDefault(f => f.Time < CurrentTime).Time;
        }
    }

    public void Reset()
    {
        State = RecordingState.Idle;
        CurrentTime = 0;
        Data = null;
    }

    public void TogglePlaying()
    {
        IsPlaying = !IsPlaying;
    }

    public void SeekBackwards()
    {
        CurrentTime -= 0.5f;
    }

    public void SeekForwards()
    {
        CurrentTime += 0.5f;
    }

    public Dictionary<Pointer, PersoBehaviour> SpawnedAlwaysObjects;

    public void ReadAlways(Reader reader)
    {
        MemoryFile mem = (MemoryFile)MapLoader.Loader.files_array[0];
        Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["always"], mem));
        var num_always = reader.ReadUInt32();
        var spawnablePersos = OpenSpace.LinkedList<Perso>.ReadHeader(reader, Pointer.Current(reader), LinkedList.Type.Double);
        var currentPointer = Pointer.Current(reader);
        var off_alwaysSPOs = Pointer.Read(reader);
        Pointer.Goto(ref reader, off_alwaysSPOs);

        if (SuperObject.FromOffset(off_alwaysSPOs) != null) {
            MapLoader.Loader.superObjects.Remove(SuperObject.FromOffset(off_alwaysSPOs));
        }

        var off_alwaysSpoArray = off_alwaysSPOs;

        List<Pointer> ObjectsToRemove = SpawnedAlwaysObjects.Keys.ToList();

        for (int i = 0; i < num_always; i++) {

            int spoSize = Settings.s.game == Settings.Game.R3 ? 0x80 : 0x48; // 0x80 for r3

            var off_alwaysSPO = off_alwaysSpoArray + (i * spoSize);

            Pointer.Goto(ref reader, off_alwaysSPO + 0x1c); // make sure spo has a parent, otherwise they're not in the world
            var parent = Pointer.Read(reader);

            if (parent != null) {
                Pointer.Goto(ref reader, off_alwaysSPO + 4); // read linkedObject (data)
                var data = Pointer.Read(reader);

                if (data != null) {
                    Pointer.Goto(ref reader, data + 4); // stdGame
                    var stdGamePtr = Pointer.Read(reader);

                    if (stdGamePtr != null) {

                        if (!SpawnedAlwaysObjects.ContainsKey(stdGamePtr)) {

                            MapLoader.Loader.superObjects.Remove(SuperObject.FromOffset(off_alwaysSPO));
                            Pointer.Goto(ref reader, off_alwaysSPO);
                            var spo = SuperObject.Read(reader, off_alwaysSPO);

                            var pb = spo.Gao.AddComponent<PersoBehaviour>();
                            pb.perso = spo.data as Perso;
                            pb.sector = null;
                            pb.controller = MapLoader.Loader.controller;
                            pb.IsEnabled = true;
                            pb.Init();

                            SpawnedAlwaysObjects.Add(stdGamePtr, pb);

                            spo.Gao.transform.SetParent(RecordingSpawnablesRoot.transform, false);

                        } else {
                            ObjectsToRemove.Remove(stdGamePtr);
                        }
                    }
                }
            }
        }

        foreach (var stdGame in ObjectsToRemove) {
            Destroy(SpawnedAlwaysObjects[stdGame].gameObject);
            SpawnedAlwaysObjects.Remove(stdGame);
        }

        /*if (SpawnedAlwaysObjects.Count != RecordingSpawnablesRoot.transform.childCount) {
            Debug.Log($"Update Spawnables, {SpawnedAlwaysObjects.Count} != {RecordingSpawnablesRoot.transform.childCount}");
            foreach (Transform t in RecordingSpawnablesRoot.transform) {
                Destroy(t.gameObject);    
            }

            foreach (var spo in SpawnedAlwaysObjects) {
                spo.Value.
            }
        }*/

    }

}
