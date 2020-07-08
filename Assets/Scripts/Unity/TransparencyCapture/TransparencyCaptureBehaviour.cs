using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class TransparencyCaptureBehaviour : MonoBehaviour
{
    public async UniTask<byte[]> Capture(int width, int height)
    {
        await UniTask.WaitForEndOfFrame();
		//After Unity4,you have to do this function after WaitForEndOfFrame in Coroutine
		//Or you will get the error:"ReadPixels was called to read pixels from system frame buffer, while not inside drawing frame"
		//zzTransparencyCapture.captureScreenshot("capture.png");
		byte[] screenshotBytes = null;
		var lScreenshot = zzTransparencyCapture.captureScreenshot(width, height);
		try {
			screenshotBytes = lScreenshot.EncodeToPNG();
		} finally {
			Object.DestroyImmediate(lScreenshot);
		}
		Debug.Log("Screenshot saved.");
		return screenshotBytes;
	}

    public static Resolution GetCurrentResolution()
    {   
        return new Resolution
        {
            width = Camera.main.pixelWidth,
            height = Camera.main.pixelHeight,
            refreshRate = Screen.currentResolution.refreshRate,
        };
    }

    /*public void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) {
            StartCoroutine(Capture(Screen.width * 4, Screen.height * 4));
    }*/
}