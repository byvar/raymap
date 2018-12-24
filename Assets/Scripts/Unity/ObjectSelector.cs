using OpenSpace.Object;
using UnityEngine;


public class ObjectSelector : MonoBehaviour {
    public Controller controller;
    public CameraComponent cam;
    public PersoBehaviour highlightedPerso = null;
    public PersoBehaviour selectedPerso = null;

    private void HandleCollision() {
        int layerMask = 0;
        if (controller.viewCollision) {
            layerMask |= 1 << LayerMask.NameToLayer("Collide");
        } else {
            layerMask |= 1 << LayerMask.NameToLayer("Visual");
        }
        Ray ray = cam.cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore);
		if (hits != null && hits.Length > 0) {
			System.Array.Sort(hits, (x, y) => (x.distance.CompareTo(y.distance)));
			for (int i = 0; i < hits.Length; i++) {
                // the object identified by hit.transform was clicked
                PersoBehaviour pb = hits[i].transform.GetComponentInParent<PersoBehaviour>();
                if (pb != null) {
                    highlightedPerso = pb;
                    if (Input.GetMouseButtonDown(0)) {
                        Select(pb, view: true);
                    }
                    return;
                }
            }
            /*SectorComponent sector = hit.transform.GetComponentInParent<SectorComponent>();
            if (sector != null) {
                JumpTo(sector.gameObject);
                return;
            }*/
        }
        /*if (Input.GetMouseButtonDown(0)) { // if left button pressed...
            if (Physics.Raycast(ray, out hit)) {
                // the object identified by hit.transform was clicked
                PersoBehaviour pb = hit.transform.GetComponentInParent<PersoBehaviour>();
                if (pb != null) {
                    JumpTo(pb.gameObject);
                    return;
                }
                SectorComponent sector = hit.transform.GetComponentInParent<SectorComponent>();
                if (sector != null) {
                    JumpTo(sector.gameObject);
                    return;
                }
            }
        }*/
    }

    public void Select(PersoBehaviour pb, bool view = false) {
        //print(pb.name);
        if (selectedPerso != pb || view) {
            selectedPerso = pb;
            cam.JumpTo(pb.gameObject);
        }
    }

	public void Select(SuperObject so) {
		cam.JumpTo(so.Gao);
	}

    public void Deselect() {
        selectedPerso = null;
    }

    void Update() {
        highlightedPerso = null;
		Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
		if (!cam.mouseLookEnabled
			&& controller.LoadState == Controller.State.Finished
			&& screenRect.Contains(Input.mousePosition)) HandleCollision();
    }
}
