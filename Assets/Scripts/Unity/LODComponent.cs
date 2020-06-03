using OpenSpace.Visual;
using System.Linq;
using UnityEngine;

public class LODComponent : MonoBehaviour {
	public VisualSetLOD[] visualSet;
	public GameObject[] gameObjects;

	private void Update() {
		float bestLOD = visualSet.Min(v => v.LODdistance);
		foreach (VisualSetLOD lod in visualSet) {
			if (lod.obj.Gao != null && lod.LODdistance != bestLOD) lod.obj.Gao.SetActive(false);
		}
	}
}
