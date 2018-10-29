using UnityEngine;

// Very simple smooth mouselook modifier for the MainCamera in Unity
// by Francis R. Griffiths-Keam - www.runningdimensions.com
// http://forum.unity3d.com/threads/a-free-simple-smooth-mouselook.73117/


public class ObjectSelector : MonoBehaviour {
    public Controller controller;
    public CameraComponent cam;
    public PersoBehaviour currentPerso;

    private void HandleCollision() {
        int layerMask = 0;
        if (controller.viewCollision) {
            layerMask |= 1 << LayerMask.NameToLayer("Collide");
        } else {
            layerMask |= 1 << LayerMask.NameToLayer("Visual");
        }
        Ray ray = cam.cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000f, layerMask, QueryTriggerInteraction.Ignore);
        if (hits != null && hits.Length > 0) {
            for (int i = 0; i < hits.Length; i++) {
                // the object identified by hit.transform was clicked
                PersoBehaviour pb = hits[i].transform.GetComponentInParent<PersoBehaviour>();
                if (pb != null) {
                    currentPerso = pb;
                    if (Input.GetMouseButtonDown(0)) {
                        //print(pb.name);
                        cam.JumpTo(pb.gameObject);
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

    void Update() {
        currentPerso = null;
        if(!cam.mouseLookEnabled) HandleCollision();
    }
}
