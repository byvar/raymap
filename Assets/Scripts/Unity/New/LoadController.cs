using BinarySerializer;
using BinarySerializer.Unity;
using Cysharp.Threading.Tasks;
using Raymap;
using System.Diagnostics;
using UnityEngine;

public class LoadController : MonoBehaviour {
	// Set in Unity inspector
	public GeneralController GeneralController;
	public LoadingScreen LoadingScreen;

	// The context, to reuse when writing
	private Context SerializeContext { get; set; }

    public Unity_Level Level { get; private set; }
	public Unity_Environment Environment { get; private set; }

	private void Awake() {
        Application.logMessageReceived += Log;
        if (Application.platform == RuntimePlatform.WebGLPlayer) {
            UnityEngine.Debug.unityLogger.filterLogType = LogType.Assert;
        }

        // Make sure filesystem is set before checking here
        UnitySettings.ConfigureFileSystem();
    }


    async UniTaskVoid Start() {
        // TODO: Temporary. Call LoadLevelAsync from Controller later.
        await LoadLevelAsync();
    }


    public async UniTask LoadLevelAsync() {
        // Create timer
        var loadTimer = new Stopwatch();
        TimeController.StartStopwatch();
        loadTimer.Start();

        // Start loading screen
        LoadingScreen.Active = true;
        GlobalLoadState.LoadState = GlobalLoadState.State.Loading;
        GlobalLoadState.DetailedState = "Starting...";

        // Create the context
        SerializeContext = new MapViewerContext(UnitySettings.GetGameSettings);
        var manager = UnitySettings.GetGameManager;

        // Make sure all the necessary files are downloaded
        GlobalLoadState.LoadState = GlobalLoadState.State.LoadingFiles;
        await manager.LoadFilesAsync(SerializeContext);
        await TimeController.WaitIfNecessary();

        using (SerializeContext) {
            // Init editor data
            //await LevelEditorData.InitAsync(context.GetR1Settings());
            //await TimeController.WaitIfNecessary();

            // Load the level
            GlobalLoadState.LoadState = GlobalLoadState.State.Loading;
            Level = await manager.LoadAsync(SerializeContext);

			// Store Unity_Level in context
			Level.Register(SerializeContext);
		}

		await TimeController.WaitIfNecessary();
		if (GlobalLoadState.LoadState == GlobalLoadState.State.Error) return;

		GlobalLoadState.LoadState = GlobalLoadState.State.Initializing;
		await TimeController.WaitIfNecessary();

		// Select an "Environment" based on the Unity_Level.
		// This environment will be a prefab or will create all the necessary controllers/monobehaviours.
		// And those will be initialized by the environment itself.
		// This way CPA and Jade for example can use completely different code without getting in each other's way.
		Environment = GeneralController.EnvironmentController.InitializeEnvironment(Level.EnvironmentKey);
		Environment.Register(SerializeContext);
		Environment.Init();

		// Initialize level
		Level.Init();

		Environment.OnDataLoaded();

		await TimeController.WaitIfNecessary();
        if (GlobalLoadState.LoadState == GlobalLoadState.State.Error) return;

        GlobalLoadState.DetailedState = "Finished";
        GlobalLoadState.LoadState = GlobalLoadState.State.Finished;
        LoadingScreen.Active = false;

        TimeController.StopStopwatch();
        loadTimer.Stop();

        UnityEngine.Debug.Log($"Loaded in {loadTimer.ElapsedMilliseconds}ms");
    }

    public void OnDestroy() {
        Level = null;
        SerializeContext?.Dispose();
        SerializeContext = null;
    }

	#region Loading screen
	private void Update() {
        if (LoadingScreen.Active) {
            if (GlobalLoadState.LoadState == GlobalLoadState.State.Error) {
                LoadingScreen.LoadingText = GlobalLoadState.DetailedState;
                LoadingScreen.LoadingtextColor = Color.red;
            } else {
                LoadingScreen.LoadingText = GlobalLoadState.DetailedState;
                LoadingScreen.LoadingtextColor = Color.white;
            }
        }
    }

    public void Log(string condition, string stacktrace, LogType type) {
        switch (type) {
            case LogType.Exception:
            case LogType.Error:
                if (GlobalLoadState.LoadState != GlobalLoadState.State.Finished) {
                    // Allowed exceptions
                    if (condition.Contains("cleaning the mesh failed")) break;
                    if (condition.Contains("desc.isValid() failed!")) break;

                    // Go to error state
                    GlobalLoadState.LoadState = GlobalLoadState.State.Error;
                    if (LoadingScreen.Active) {
                        GlobalLoadState.DetailedState = condition;
                    }
                }
                break;
        }
    }
	#endregion
}
