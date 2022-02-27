using Cysharp.Threading.Tasks;
using System.Diagnostics;

namespace BinarySerializer.Unity {
	public class TimeController {

        private static readonly Stopwatch stopwatch = new Stopwatch();

        public static async UniTask WaitFrame() {
            //await UniTask.NextFrame();
            await UniTask.Yield();
            //await UniTask.WaitForEndOfFrame();

            if (stopwatch.IsRunning) stopwatch.Restart();
        }

        public static void StartStopwatch() {
            stopwatch.Start();
        }
        public static void StopStopwatch() {
            if (stopwatch.IsRunning) stopwatch.Stop();
        }

        public static async UniTask WaitIfNecessary() {
            if (!stopwatch.IsRunning) stopwatch.Start();
            if (stopwatch.ElapsedMilliseconds > 16)
                await WaitFrame();
        }
    }
}
