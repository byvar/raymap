using System.Collections;
using UnityEngine;

public class TransparencyCaptureToFile:MonoBehaviour
{
    public IEnumerator capture()
    {

        yield return new WaitForEndOfFrame();
        //After Unity4,you have to do this function after WaitForEndOfFrame in Coroutine
        //Or you will get the error:"ReadPixels was called to read pixels from system frame buffer, while not inside drawing frame"
        //zzTransparencyCapture.captureScreenshot("capture.png");
		var lScreenshot = zzTransparencyCapture.captureScreenshot(Screen.width * 4, Screen.height * 4);
		try {
			byte[] screenshotBytes = lScreenshot.EncodeToPNG();
			OpenSpace.Util.ByteArrayToFile(OpenSpace.MapLoader.Loader.gameDataBinFolder + "yo.png",screenshotBytes);
		} finally {
			Object.DestroyImmediate(lScreenshot);
		}
		Debug.Log("Screenshot saved.");
	}

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            StartCoroutine(capture());
    }
}