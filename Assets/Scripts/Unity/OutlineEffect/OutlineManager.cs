using cakeslice;
using UnityEngine;

public class OutlineManager : MonoBehaviour {
	public OutlineEffect outline;
	public Color highlightColor;
	public Color selectColor;
	public float fadeInSpeed = 10f;
	public float fadeOutSpeed = 3f;
	public bool selecting = false;
	public float curLerp = 0f;
	public GameObject Highlight {
		get {
			return outline.Highlight;
		}
		set {
			if (outline.Highlight != value) {
				outline.Highlight = value;
			}
		}
	}

	void Update() {
		if (Highlight == null || !selecting) {
			curLerp = Mathf.Clamp01(curLerp - Time.deltaTime * fadeOutSpeed);
		} else {
			curLerp = Mathf.Clamp01(curLerp + Time.deltaTime * fadeInSpeed);
		}
		outline.lineColor0 = Color.Lerp(highlightColor, selectColor, curLerp);
	}
}