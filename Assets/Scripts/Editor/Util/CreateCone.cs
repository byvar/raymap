using UnityEngine;
using UnityEditor;
using System.Collections;
 
// an Editor method to create a cone primitive (so far no end caps)
// the top center is placed at (0/0/0)
// the bottom center is placed at (0/0/length)
// if either one of the radii is 0, the result will be a cone, otherwise a truncated cone
// note you will get inevitable breaks in the smooth shading at cone tips
// note the resulting mesh will be created as an asset in Assets/Editor
// Author: Wolfram Kresse
public class CreateCone : ScriptableWizard {
 
	public int numVertices = 10;
	public float radiusTop = 0f;
	public float radiusBottom = 1f;
	public float length = 1f;
	public float openingAngle = 0f; // if >0, create a cone with this angle by setting radiusTop to 0, and adjust radiusBottom according to length;
	public bool outside = true;
	public bool inside = false;
	public bool addCollider = false;
 
    [MenuItem ("GameObject/Create Other/Cone")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Create Cone", typeof(CreateCone));
    }
 
	void OnWizardCreate(){
		GameObject newCone=new GameObject("Cone");
		if(openingAngle>0&&openingAngle<180){
			radiusTop=0;
			radiusBottom=length*Mathf.Tan(openingAngle*Mathf.Deg2Rad/2);
		}
		string meshName = newCone.name + numVertices + "v" + radiusTop + "t" + radiusBottom + "b" + length + "l" + length + (outside?"o":"") + (inside?"i":"");
		string meshPrefabPath = "Assets/Editor/" + meshName + ".asset";
		Mesh mesh = (Mesh)AssetDatabase.LoadAssetAtPath(meshPrefabPath, typeof(Mesh));
		if(mesh==null){
			mesh=new Mesh();
			mesh.name=meshName;
			// can't access Camera.current
			//newCone.transform.position = Camera.current.transform.position + Camera.current.transform.forward * 5.0f;
			int multiplier=(outside?1:0)+(inside?1:0);
			int offset=(outside&&inside?2*numVertices:0);
			Vector3[] vertices=new Vector3[2*multiplier*numVertices]; // 0..n-1: top, n..2n-1: bottom
			Vector3[] normals=new Vector3[2*multiplier*numVertices];
			Vector2[] uvs=new Vector2[2*multiplier*numVertices];
			int[] tris;
			float slope=Mathf.Atan((radiusBottom-radiusTop)/length); // (rad difference)/height
			float slopeSin=Mathf.Sin(slope);
			float slopeCos=Mathf.Cos(slope);
			int i;
 
			for(i=0;i<numVertices;i++){
				float angle=2*Mathf.PI*i/numVertices;
				float angleSin=Mathf.Sin(angle);
				float angleCos=Mathf.Cos(angle);
				float angleHalf=2*Mathf.PI*(i+0.5f)/numVertices; // for degenerated normals at cone tips
				float angleHalfSin=Mathf.Sin(angleHalf);
				float angleHalfCos=Mathf.Cos(angleHalf);
 
				vertices[i]=new Vector3(radiusTop*angleCos,radiusTop*angleSin,0);
				vertices[i+numVertices]=new Vector3(radiusBottom*angleCos,radiusBottom*angleSin,length);
 
				if(radiusTop==0)
					normals[i]=new Vector3(angleHalfCos*slopeCos,angleHalfSin*slopeCos,-slopeSin);
				else
					normals[i]=new Vector3(angleCos*slopeCos,angleSin*slopeCos,-slopeSin);
				if(radiusBottom==0)
					normals[i+numVertices]=new Vector3(angleHalfCos*slopeCos,angleHalfSin*slopeCos,-slopeSin);
				else
					normals[i+numVertices]=new Vector3(angleCos*slopeCos,angleSin*slopeCos,-slopeSin);
 
				uvs[i]=new Vector2(1.0f*i/numVertices,1);
				uvs[i+numVertices]=new Vector2(1.0f*i/numVertices,0);
 
				if(outside&&inside){
					// vertices and uvs are identical on inside and outside, so just copy
					vertices[i+2*numVertices]=vertices[i];
					vertices[i+3*numVertices]=vertices[i+numVertices];
					uvs[i+2*numVertices]=uvs[i];
					uvs[i+3*numVertices]=uvs[i+numVertices];
				}
				if(inside){
					// invert normals
					normals[i+offset]=-normals[i];
					normals[i+numVertices+offset]=-normals[i+numVertices];
				}
			}
			mesh.vertices = vertices;
			mesh.normals = normals;		
			mesh.uv = uvs;
 
			// create triangles
			// here we need to take care of point order, depending on inside and outside
			int cnt=0;
			if(radiusTop==0){
				// top cone
				tris=new int[numVertices*3*multiplier];
				if(outside)
					for(i=0;i<numVertices;i++){
						tris[cnt++]=i+numVertices;
						tris[cnt++]=i;
						if(i==numVertices-1)
							tris[cnt++]=numVertices;
						else
							tris[cnt++]=i+1+numVertices;
					}
				if(inside)
					for(i=offset;i<numVertices+offset;i++){
						tris[cnt++]=i;
						tris[cnt++]=i+numVertices;
						if(i==numVertices-1+offset)
							tris[cnt++]=numVertices+offset;
						else
							tris[cnt++]=i+1+numVertices;
					}
			}else if(radiusBottom==0){
				// bottom cone
				tris=new int[numVertices*3*multiplier];
				if(outside)
					for(i=0;i<numVertices;i++){
						tris[cnt++]=i;
						if(i==numVertices-1)
							tris[cnt++]=0;
						else
							tris[cnt++]=i+1;
						tris[cnt++]=i+numVertices;
					}
				if(inside)
					for(i=offset;i<numVertices+offset;i++){
						if(i==numVertices-1+offset)
							tris[cnt++]=offset;
						else
							tris[cnt++]=i+1;
						tris[cnt++]=i;
						tris[cnt++]=i+numVertices;
					}
			}else{
				// truncated cone
				tris=new int[numVertices*6*multiplier];
				if(outside)
					for(i=0;i<numVertices;i++){
						int ip1=i+1;
						if(ip1==numVertices)
							ip1=0;
 
						tris[cnt++]=i;
						tris[cnt++]=ip1;
						tris[cnt++]=i+numVertices;
 
						tris[cnt++]=ip1+numVertices;
						tris[cnt++]=i+numVertices;
						tris[cnt++]=ip1;
					}
				if(inside)
					for(i=offset;i<numVertices+offset;i++){
						int ip1=i+1;
						if(ip1==numVertices+offset)
							ip1=offset;
 
						tris[cnt++]=ip1;
						tris[cnt++]=i;
						tris[cnt++]=i+numVertices;
 
						tris[cnt++]=i+numVertices;
						tris[cnt++]=ip1+numVertices;
						tris[cnt++]=ip1;
					}
			}
			mesh.triangles = tris;		
			AssetDatabase.CreateAsset(mesh, meshPrefabPath);
			AssetDatabase.SaveAssets();
		}
 
		MeshFilter mf=newCone.AddComponent<MeshFilter>();
		mf.mesh = mesh;
 
		newCone.AddComponent<MeshRenderer>();
 
		if(addCollider){
			MeshCollider mc=newCone.AddComponent<MeshCollider>();
			mc.sharedMesh=mf.sharedMesh;
		}
 
        Selection.activeObject = newCone;
	}
}