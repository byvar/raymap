using OpenSpace;
using OpenSpace.Visual;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// From http://wiki.unity3d.com/index.php/MirrorReflection4
public class PortalBehaviour : MonoBehaviour {
	public PortalManager.Portal portal;
	public int textureIndex;

	public int m_TextureSize = 512;
	public float m_ClipPlaneOffset = 0.07f;

	public LayerMask m_ReflectLayers = -1;

	private RenderTexture m_ReflectionTexture = null;
	private int m_OldReflectionTextureSize = 0;

	private static bool s_InsideRendering = false;

	// This is called when it's known that the object will be rendered by some
	// camera. We render reflections and do other updates here.
	// Because the script executes in edit mode, reflections for the scene view
	// camera will just work!
	public void OnWillRenderObject() {
		if (portal.isMirror) { // It's a mirror
			var rend = GetComponent<Renderer>();
			if (!enabled || !rend || !rend.sharedMaterial || !rend.enabled)
				return;

			Camera cam = Camera.current;
			if (!cam)
				return;

			// Safeguard from recursive reflections.        
			if (s_InsideRendering)
				return;
			s_InsideRendering = true;

			Camera reflectionCamera = portal.camera;
			CreateMirrorObjects();

			// find out the reflection plane: position and normal in world space
			Vector3 pos = portal.geometricObject.vertices[0];
			Vector3 normal = portal.geometricObject.normals[0];

			UpdateCameraModes(cam, reflectionCamera);

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

			reflectionCamera.cullingMask = ~(1 << 4) & m_ReflectLayers.value; // never render water layer
			reflectionCamera.targetTexture = m_ReflectionTexture;
			GL.invertCulling = true;
			reflectionCamera.transform.position = newpos;
			Vector3 euler = cam.transform.eulerAngles;
			reflectionCamera.transform.eulerAngles = new Vector3(0, euler.y, euler.z);
			try {
				reflectionCamera.Render();
			} catch {
			}
			reflectionCamera.transform.position = oldpos;
			GL.invertCulling = false;
			//Material[] materials = rend.sharedMaterials;
			if (portal.material != null) {
				if (portal.material.HasProperty("_Tex" + textureIndex)) {
					portal.material.SetTexture("_Tex" + textureIndex, m_ReflectionTexture);
					Vector4 ogParams = portal.material.GetVector("_Tex" + textureIndex + "Params");
					portal.material.SetVector(
						"_Tex" + textureIndex + "Params",
						new Vector4(ogParams.x, ogParams.y, 1, ogParams.w));
				}
			}

			s_InsideRendering = false;
		} else if(portal.cameraSO != null) {
			var rend = GetComponent<Renderer>();
			if (!enabled || !rend || !rend.sharedMaterial || !rend.enabled)
				return;

			Camera cam = Camera.current;
			if (!cam)
				return;

			// Safeguard from recursive reflections.        
			if (s_InsideRendering)
				return;
			s_InsideRendering = true;

			Camera reflectionCamera = portal.camera;
			CreateMirrorObjects();

			// find out the reflection plane: position and normal in world space
			Vector3 pos = portal.geometricObject.vertices[0];
			Vector3 normal = portal.geometricObject.normals[0];

			UpdateCameraModes(cam, reflectionCamera);

			// Render reflection
			// Reflect camera around reflection plane
			/*float d = -Vector3.Dot(normal, pos) - m_ClipPlaneOffset;
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
			reflectionCamera.projectionMatrix = projection;*/

			reflectionCamera.cullingMask = ~(1 << 4) & m_ReflectLayers.value; // never render water layer
			reflectionCamera.targetTexture = m_ReflectionTexture;

			try {
				reflectionCamera.Render();
			} catch {
			}

			//Material[] materials = rend.sharedMaterials;
			if (portal.material != null) {
				if (portal.material.HasProperty("_Tex" + textureIndex)) {
					portal.material.SetTexture("_Tex" + textureIndex, m_ReflectionTexture);
					Vector4 ogParams = portal.material.GetVector("_Tex" + textureIndex + "Params");
					portal.material.SetVector(
						"_Tex" + textureIndex + "Params",
						new Vector4(ogParams.x, ogParams.y, 2, ogParams.w));
				}
			}

			s_InsideRendering = false;
		}
	}


	// Cleanup all the objects we possibly have created
	void OnDisable() {
		if (m_ReflectionTexture) {
			DestroyImmediate(m_ReflectionTexture);
			m_ReflectionTexture = null;
		}
	}


	private void UpdateCameraModes(Camera src, Camera dest) {
		if (dest == null)
			return;
		// set camera to clear the same way as current camera, but adjust view space when below 16:9.
		dest.clearFlags = src.clearFlags;
		dest.backgroundColor = src.backgroundColor;
		// update other values to match current camera.
		// even if we are supplying custom camera&projection matrices,
		// some of values are used elsewhere (e.g. skybox uses far plane)
		dest.farClipPlane = src.farClipPlane;
		dest.nearClipPlane = src.nearClipPlane;
		dest.orthographic = src.orthographic;
		dest.fieldOfView = (float)Math.Atan((float)Math.Tan(src.fieldOfView * (float)Math.PI / 360f) / (src.aspect < 16f / 9f ? src.aspect : 16f / 9f) * 16f / 9f);
		dest.aspect = src.aspect;
		dest.orthographicSize = src.orthographicSize / (src.aspect < 16f / 9f ? src.aspect : 16f / 9f) * 16f / 9f);
	}

	// On-demand create any objects we need
	private void CreateMirrorObjects() {
		// Reflection render texture
		if (!m_ReflectionTexture || m_OldReflectionTextureSize != m_TextureSize) {
			if (m_ReflectionTexture)
				DestroyImmediate(m_ReflectionTexture);
			m_ReflectionTexture = new RenderTexture(m_TextureSize, m_TextureSize, 16);
			m_ReflectionTexture.name = "__MirrorReflection" + GetInstanceID();
			m_ReflectionTexture.isPowerOfTwo = true;
			m_ReflectionTexture.hideFlags = HideFlags.DontSave;
			m_OldReflectionTextureSize = m_TextureSize;
		}
	}

	// Extended sign: returns -1, 0 or 1 based on sign of a
	private static float sgn(float a) {
		if (a > 0.0f) return 1.0f;
		if (a < 0.0f) return -1.0f;
		return 0.0f;
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
}
