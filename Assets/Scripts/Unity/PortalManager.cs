using OpenSpace;
using OpenSpace.Object;
using OpenSpace.Visual;
using OpenSpace.Waypoints;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PortalManager : MonoBehaviour {
	public Controller controller;
	bool loaded = false;
	public List<Portal> portals = new List<Portal>();
	public float m_ClipPlaneOffset = 0.07f;

	// Use this for initialization
	void Start() {

    }

    // Update is called once per frame
    void Update() {
		if (loaded) {
			/*foreach (Portal p in portals) {
				// TODO: don't rotate cameras. Reimplement mirror based on http://wiki.unity3d.com/index.php/MirrorReflection4
				if (p.mirrorGeometricObject != null) { // It's a mirror
					Vector3 mainCamPos = Camera.main.transform.position;
					Quaternion mainCamRot = Camera.main.transform.rotation;
					Vector3 normal = p.mirrorGeometricObject.normals[0];
					Vector3 direction = p.mirrorGeometricObject.vertices[0] - mainCamPos;
					Vector3 reflected = Vector3.Reflect(direction, normal);
					//p.camera.transform.position = p.mirrorGeometricObject.vertices[0] + reflected;
					p.camera.transform.localRotation = Quaternion.LookRotation(reflected, Vector3.up);
				}
			}*/
			/*foreach (Portal p in portals) {
				UpdatePortal(p);
			}*/
			//Vector3.Reflect(originalObject.position, Vector3.right);
		}
    }

	void UpdatePortal(Portal p) {
		if (p.geometricObject != null) { // It's a mirror
			Camera cam = Camera.main;
			Camera reflectionCamera = p.camera;
			Vector3 pos = p.geometricObject.vertices[0];
			Vector3 normal = p.geometricObject.normals[0];
			// Render reflection
			// Reflect camera around reflection plane
			float d = -Vector3.Dot(normal, pos) - m_ClipPlaneOffset;
			Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

			Matrix4x4 reflection = Matrix4x4.zero;
			CalculateReflectionMatrix(ref reflection, reflectionPlane);
			Vector3 oldpos = cam.transform.position;
			Vector3 newpos = reflection.MultiplyPoint(oldpos);
			reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;
			// Setup oblique projection matrix so that near plane is our reflection
			// plane. This way we clip everything below/above it for free.
			Vector4 clipPlane = CameraSpacePlane(reflectionCamera, pos, normal, 1.0f);
			//Matrix4x4 projection = cam.projectionMatrix;
			Matrix4x4 projection = cam.CalculateObliqueMatrix(clipPlane);
			reflectionCamera.projectionMatrix = projection;

			//reflectionCamera.cullingMask = ~(1 << 4) & m_ReflectLayers.value; // never render water layer
			//reflectionCamera.targetTexture = m_ReflectionTexture;
			//GL.SetRevertBackfacing(true);
			GL.invertCulling = true;
			reflectionCamera.transform.position = newpos;
			Vector3 euler = cam.transform.eulerAngles;
			reflectionCamera.transform.eulerAngles = new Vector3(0, euler.y, euler.z);
			reflectionCamera.Render();
			GL.invertCulling = false;
			reflectionCamera.transform.position = oldpos;
		}
	}

	public void Init() {
		if (MapLoader.Loader is OpenSpace.Loader.R3Loader) {
			MapLoader l = MapLoader.Loader;
			foreach (SuperObject so in l.superObjects) {
				if (so.type == SuperObject.Type.IPO_2) {
					//Debug.LogWarning("TYPE 2 " + so.Gao.name);
					Portal portal = new Portal() {
						containerSO = so,
						containerIPO = so.data as IPO
					};
					portal.cameraSO = SuperObject.FromOffset(portal.containerIPO.off_portalCamera);
					MeshObject geo = portal.containerIPO.data.visualSet[0].obj as MeshObject;
					portal.geometricObject = geo;
					if (geo != null) {
						MeshElement el = geo.subblocks[0] as MeshElement;
						portal.meshElement = el;
						portal.material = el.Gao.GetComponent<Renderer>().sharedMaterial;
						//Debug.LogWarning(so.type + " - " + portal.containerIPO.offset + " - " + portal.containerIPO.off_portalCamera);
						if (portal.cameraSO != null) {
							GameObject camGao = new GameObject("Portal Camera - " + portal.containerIPO.Gao.name + " - " + portal.cameraSO.Gao.name);
							Camera camera = camGao.AddComponent<Camera>();
							camGao.transform.position = portal.cameraSO.matrix.GetPosition(true);
							camGao.transform.rotation = portal.cameraSO.matrix.GetRotation(true) * Quaternion.Euler(-180, 0, 0);
							camGao.transform.localScale = portal.cameraSO.matrix.GetScale(true);
							camera.fieldOfView = Camera.main.fieldOfView;
							camera.enabled = false;
							camGao.transform.SetParent(transform);
							portal.camera = camera;
							portals.Add(portal);
						} else {
							// it's a mirror
							portal.isMirror = true;
							GameObject camGao = new GameObject("Mirror Camera - " + portal.containerIPO.Gao.name);
							Camera camera = camGao.AddComponent<Camera>();
							camGao.transform.position = geo.vertices[0];
							/*camGao.transform.rotation = portal.cameraSO.matrix.GetRotation(true) * Quaternion.Euler(-180, 0, 0);
							camGao.transform.localScale = portal.cameraSO.matrix.GetScale(true);*/
							camera.fieldOfView = Camera.main.fieldOfView;
							camera.enabled = false;
							camGao.transform.SetParent(transform);
							portal.camera = camera;
							portals.Add(portal);
						}

						el.Gao.layer = LayerMask.NameToLayer("VisualMirror");
						PortalBehaviour pb = el.Gao.AddComponent<PortalBehaviour>();
						pb.m_ReflectLayers = (1 << LayerMask.NameToLayer("Visual")) | (1 << LayerMask.NameToLayer("VisualOnlyInMirror"));
						pb.portal = portal;
						pb.textureIndex = portal.material.GetInt("_NumTextures");
						portal.material.SetInt("_NumTextures", pb.textureIndex + 1);
					}
				}
			}
		}
		loaded = true;
	}

	// Given position/normal of the plane, calculates plane in camera space.
	private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign) {
		Vector3 offsetPos = pos + normal * m_ClipPlaneOffset;
		Matrix4x4 m = cam.worldToCameraMatrix;
		Vector3 cpos = m.MultiplyPoint(offsetPos);
		Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
	}

	// Calculates reflection matrix around the given plane
	private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane) {
		reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
		reflectionMat.m01 = (-2F * plane[0] * plane[1]);
		reflectionMat.m02 = (-2F * plane[0] * plane[2]);
		reflectionMat.m03 = (-2F * plane[3] * plane[0]);

		reflectionMat.m10 = (-2F * plane[1] * plane[0]);
		reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
		reflectionMat.m12 = (-2F * plane[1] * plane[2]);
		reflectionMat.m13 = (-2F * plane[3] * plane[1]);

		reflectionMat.m20 = (-2F * plane[2] * plane[0]);
		reflectionMat.m21 = (-2F * plane[2] * plane[1]);
		reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
		reflectionMat.m23 = (-2F * plane[3] * plane[2]);

		reflectionMat.m30 = 0F;
		reflectionMat.m31 = 0F;
		reflectionMat.m32 = 0F;
		reflectionMat.m33 = 1F;
	}

	public class Portal {
		public SuperObject containerSO;
		public IPO containerIPO;
		public SuperObject cameraSO;
		public MeshObject geometricObject;
		public Camera camera;
		public RenderTexture rt;
		public bool isMirror;
		public Material material;
		public MeshElement meshElement;
	}
}
