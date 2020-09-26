using OpenSpace.Object;
using UnityEngine;


public class ObjectSelector : MonoBehaviour {
    public Controller controller;
    public CameraComponent cam;
    public BasePersoBehaviour highlightedPerso = null;
    public BasePersoBehaviour selectedPerso = null;
    public CollideComponent highlightedCollision = null;
    public WayPointBehaviour highlightedWayPoint = null;
	private GameObject tooFarLimitDiamond = null;
    public bool IsSelecting { get; private set; }  = false;
    public OutlineManager outline;

    private void HandleCollision() {
        int layerMask = 0;
        if (controller.viewCollision) {
            layerMask |= 1 << LayerMask.NameToLayer("Collide");
        } else {
            layerMask |= 1 << LayerMask.NameToLayer("Visual");
        }
        if (controller.viewGraphs) {
            layerMask |= 1 << LayerMask.NameToLayer("Graph");
        }
        Ray ray = cam.cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore);
		if (hits != null && hits.Length > 0) {
			System.Array.Sort(hits, (x, y) => (x.distance.CompareTo(y.distance)));
            if (controller.showPersos) {
                for (int i = 0; i < hits.Length; i++) {
                    // the object identified by hit.transform was clicked
                    BasePersoBehaviour pb = hits[i].transform.GetComponentInParent<BasePersoBehaviour>();
                    if (pb != null) {
                        highlightedPerso = pb;
                        if (Input.GetMouseButtonDown(0)) {
                            IsSelecting = true;
                        }
                        UpdateHighlight();
                        if (IsSelecting) {
                            if (cam.IsPanningWithThreshold()) {
                                IsSelecting = false;
                            } else if (Input.GetMouseButtonUp(0)) {
                                Select(pb, view: true);
                            }
                        }
                        break;
                    }
                }
            }
            if (controller.viewCollision) {
                for (int i = 0; i < hits.Length; i++) {
                    CollideComponent cc = hits[i].transform.GetComponent<CollideComponent>();
                    if (cc != null) {
                        highlightedCollision = cc;
                        break;
                    }
                }
            }
            if (controller.viewGraphs) {
                for (int i = 0; i < hits.Length; i++) {
                    WayPointBehaviour wp = hits[i].transform.GetComponent<WayPointBehaviour>();
                    if (wp != null) {
                        highlightedWayPoint = wp;
                        break;
                    }
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

    private void UpdateHighlight() {
        outline.Highlight = highlightedPerso != null ? highlightedPerso.gameObject : null;
        outline.selecting = IsSelecting;
    }

    public void Select(BasePersoBehaviour pb, bool view = false) {
        //print(pb.name);
        if (selectedPerso != pb || view) {
#if UNITY_EDITOR
            if (pb?.gameObject!=null)
                UnityEditor.Selection.activeGameObject = pb.gameObject;
#endif
            selectedPerso = pb;
            cam.JumpTo(pb.gameObject);
        }
    }

	public void Select(SuperObjectComponent so) {
		cam.JumpTo(so.gameObject);
	}

    public void Deselect() {
        selectedPerso = null;
    }

    void Update() {
        if (UnitySettings.IsRaymapGame) {
            return;
        }
        highlightedPerso = null;
        UpdateHighlight();
        highlightedCollision = null;
        highlightedWayPoint = null;
		Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
		if (!cam.MouseLookEnabled && !cam.MouseLookRMBEnabled 
			&& controller.LoadState == Controller.State.Finished
			&& screenRect.Contains(Input.mousePosition)) HandleCollision();
		if (controller.LoadState == Controller.State.Finished) UpdateTooFarLimit();
        if (IsSelecting && (!Input.GetMouseButton(0) || highlightedPerso == null)) {
            IsSelecting = false;
        }
    }

	void UpdateTooFarLimit() {
		if (tooFarLimitDiamond == null) InitTooFarLimit();
        PersoBehaviour pb = selectedPerso == null ? null : selectedPerso as PersoBehaviour;
		if (pb != null && controller.viewCollision && pb.perso != null
			&& pb.perso.stdGame != null && pb.perso.stdGame.tooFarLimit > 0) {
			if (!tooFarLimitDiamond.activeSelf) tooFarLimitDiamond.SetActive(true);
			tooFarLimitDiamond.transform.localScale = Vector3.one * pb.perso.stdGame.tooFarLimit;
			tooFarLimitDiamond.transform.localRotation = Quaternion.identity;
			tooFarLimitDiamond.transform.position = selectedPerso.transform.position;
		} else {
			if (tooFarLimitDiamond.activeSelf) tooFarLimitDiamond.SetActive(false);
		}
	}

	void InitTooFarLimit() {
		tooFarLimitDiamond = new GameObject("TooFarLimit");
		tooFarLimitDiamond.transform.localScale = Vector3.one;
		tooFarLimitDiamond.transform.localPosition = Vector3.zero;
		tooFarLimitDiamond.transform.localRotation = Quaternion.identity;
		MeshRenderer meshRenderer = tooFarLimitDiamond.AddComponent<MeshRenderer>();
		MeshFilter meshFilter = tooFarLimitDiamond.AddComponent<MeshFilter>();
		meshFilter.mesh = Resources.Load<Mesh>("diamond");
		meshRenderer.material = controller.collideTransparentMaterial;
		meshRenderer.material.color = new Color(1, 0.5f, 0, 0.5f);
		tooFarLimitDiamond.SetActive(false);
	}
}
