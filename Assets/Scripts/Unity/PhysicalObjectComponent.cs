using UnityEngine;

public class PhysicalObjectComponent : MonoBehaviour {
	public GameObject visual;
	public GameObject collide;
	private Controller controller;

	public void Init(Controller controller) {
		this.controller = controller;
		UpdateViewCollision(controller.viewCollision);
	}

	public void UpdateViewCollision(bool displayCollision) {
		if (visual != null) {
			visual.SetActive(!displayCollision);
		}
		if (collide != null) {
			collide.SetActive(displayCollision);
		}
	}

	private void Update() {
		if (controller != null) {
			UpdateViewCollision(controller.viewCollision);
		}
	}

	private void OnEnable() {
		if (controller != null) {
			UpdateViewCollision(controller.viewCollision);
		}
	}
}
