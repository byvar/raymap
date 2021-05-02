using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using OpenSpace;
using OpenSpace.Object;
using UnityEngine;

public class RaymanTrail : MonoBehaviour
{
    //public LineRenderer Line;
    public LineRenderer LinePrefab;
    
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();

    public bool Recording { get; set; }

    public bool ShowAll
    {
        get => _showAll;
        set
        {
            linesDirty = true;
            _showAll = value;
        }
    }

    private bool _showAll;
    public float PlaybackStage = 1;

    public Attempt AttemptToShow => Data.Attempts.Count > AttemptToShowNumber ? Data.Attempts[AttemptToShowNumber] : null;

    public int AttemptToShowNumber
    {
        get => _attemptToShowNumber;
        set
        {
            linesDirty = true;
            if (AttemptCount == 0) {
                _attemptToShowNumber = 0;
                return;
            }
            if (value < 0) {
                _attemptToShowNumber = 0;
            } else if (value >= AttemptCount) {
                _attemptToShowNumber = AttemptCount - 1;
            } else {
                _attemptToShowNumber = value;
            }
        }
    }

    public int AttemptCount => Data.Attempts?.Count ?? 0;

    private int _attemptToShowNumber;

    private const float maxDist = 10.0f;
    private const float minDist = 0.01f;

    private bool linesDirty = true;

    public class AttemptData
    {
        public List<Attempt> Attempts = new List<Attempt>();
    }

    public AttemptData Data;

    public class Attempt
    {
        public class FrameData
        {
            public FrameData(Vector3 position, Quaternion rotation, int animationIndex, int animationFrame)
            {
                Position = position;
                Rotation = rotation;
                AnimationIndex = animationIndex;
                AnimationFrame = animationFrame;
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
        }

        public List<FrameData> Frames = new List<FrameData>();
    }

    private SuperObject Rayman;
    private PersoBehaviour RaymanPersoBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        Data = new AttemptData();
    }

    private void UpdateLivePreview()
    {
        Reader reader = MapLoader.Loader.livePreviewReader;

        Pointer.Goto(ref reader, Rayman.off_staticMatrix);
        Rayman.matrix = Matrix.Read(MapLoader.Loader.livePreviewReader, Rayman.off_staticMatrix);
        if (Rayman.data != null && Rayman.data.Gao != null) {
            Rayman.data.Gao.transform.position = Rayman.matrix.GetPosition(convertAxes: true);
            Rayman.data.Gao.transform.rotation = Rayman.matrix.GetRotation(convertAxes: true);
            Rayman.data.Gao.transform.localScale = Rayman.matrix.GetScale(convertAxes: true);

            var perso = Rayman.data as Perso;

            PersoBehaviour pb = RaymanPersoBehaviour;
            if (pb != null) {

                Pointer.Goto(ref reader, perso.p3dData.offset);
                perso.p3dData.UpdateCurrentState(reader);

                // State offset changed?
                if (perso.p3dData.stateCurrent != null) {
                    pb.SetState(perso.p3dData.stateCurrent.index);
                    pb.autoNextState = true;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GetRaymanGAO()) {
            return;
        }

        if (Recording) {

            if (MapLoader.Loader.livePreviewReader != null) {
                UpdateLivePreview();
            }

            RaymanPersoBehaviour.playAnimation = true;

            linesDirty = true;

            if (Data.Attempts.Count == 0) {
                Data.Attempts.Add(new Attempt());
            }

            var currentAttempt = Data.Attempts[Data.Attempts.Count - 1];

            if (currentAttempt.Frames.Count > 0 &&
                Vector3.Distance(currentAttempt.Frames.Last().Position, Rayman.Gao.transform.position) > maxDist) {

                currentAttempt = new Attempt();
                Data.Attempts.Add(currentAttempt);
            }

            if (currentAttempt.Frames.Count == 0 ||
                Vector3.Distance(currentAttempt.Frames.Last().Position, Rayman.Gao.transform.position) > minDist) {

                var raymanPos = Rayman.Gao.transform.position;
                if (IsValidPos(raymanPos)) {

                    currentAttempt.Frames.Add(new Attempt.FrameData(
                        Rayman.Gao.transform.position,
                        Rayman.Gao.transform.rotation,
                        RaymanPersoBehaviour.currentState,
                        (int)RaymanPersoBehaviour.currentFrame));
                }
            }
        } else {

            if (RaymanPersoBehaviour != null) {
                RaymanPersoBehaviour.playAnimation = false;
            }

            if (AttemptToShow != null) {
                var index = Mathf.RoundToInt(PlaybackStage * (AttemptToShow.Frames.Count - 1));
                var frame = AttemptToShow.Frames[index];
                UpdateRaymanPos(frame.Position, frame.Rotation, frame.AnimationIndex, frame.AnimationFrame);
            }

            if (linesDirty) {

                ResetLines();

                foreach (var a in Data.Attempts) {

                    System.Random r = new System.Random(a.Frames.Count);
                    if (a == AttemptToShow || ShowAll) {

                        var line = Instantiate(LinePrefab);
                        line.transform.SetParent(this.gameObject.transform, false);
                        line.transform.localPosition = Vector3.zero;

                        var randomColor = Color.HSVToRGB((float)r.NextDouble(), 0.5f+((float)r.NextDouble()*0.5f), 1.0f);
                        line.startColor = line.endColor = randomColor;
                        lineRenderers.Add(line);

                        line.positionCount = a.Frames.Count;
                        line.SetPositions(a.Frames.Select(f=>f.Position).ToArray());
                    }
                }

                linesDirty = false;

            }
        }
    }

    private bool IsValidPos(Vector3 pos)
    {
        float limit = 100000;
        return Mathf.Abs(pos.x) < limit && Mathf.Abs(pos.y) < limit && Mathf.Abs(pos.z) < limit;
    }

    private void UpdateRaymanPos(Vector3 pos, Quaternion rotation, int animIndex, int animFrame)
    {
        Rayman.Gao.transform.position = pos;
        Rayman.Gao.transform.rotation = rotation;
        RaymanPersoBehaviour.SetState(animIndex);
        RaymanPersoBehaviour.currentFrame = (uint)animFrame;
    }

    private void ResetLines()
    {
        foreach (var l in lineRenderers) {
            Destroy(l.gameObject);
        }

        lineRenderers.Clear();
    }

    private bool GetRaymanGAO()
    {
        if (Rayman == null || RaymanPersoBehaviour == null) {
            Rayman = MapLoader.Loader?.persos?.FirstOrDefault(p => p.SuperObject.Gao.name.Contains("YLT_RaymanModel"))?.SuperObject;
            if (Rayman != null) {
                RaymanPersoBehaviour = Rayman.Gao.GetComponent<PersoBehaviour>();
            }
        }

        return Rayman != null;
    }

    public void Reset()
    {
        Data.Attempts.Clear();
        ResetLines();
        linesDirty = true;
    }

    public void NextFrame()
    {
        ChangeFrame(1);
    }

    public void PreviousFrame()
    {
        ChangeFrame(-1);
    }

    private void ChangeFrame(float change)
    {
        PlaybackStage = PlaybackStage + change * (1f / AttemptToShow.Frames.Count);
    }
}
