using UnityEngine;

public class PhysicalObjectComponent : MonoBehaviour {
	public GameObject visual;
	public GameObject collide;

	public void Switch(bool displayCollision) {
		if (visual != null) {
			visual.SetActive(!displayCollision);
		}
		if (collide != null) {
			collide.SetActive(displayCollision);
		}
	}
}
