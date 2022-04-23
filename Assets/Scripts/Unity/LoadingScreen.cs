using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {
    public Image loadingPanel;
    public Text loadingText;

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        
    }

    public string LoadingText {
        get { return loadingText.text; }
        set { loadingText.text = value; }
    }

    public Sprite LoadingImage {
        get { return loadingPanel.sprite; }
        set { loadingPanel.sprite = value; }
    }

	public Color LoadingtextColor {
		get { return loadingText.color; }
		set { loadingText.color = value; }
	}

    public bool Active {
        get { return gameObject.activeSelf; }
        set { gameObject.SetActive(value); }
    }
}
