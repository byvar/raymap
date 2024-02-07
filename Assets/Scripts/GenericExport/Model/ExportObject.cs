using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GenericExport.Model
{
    public class ExportVector3
    {
        public float x;
        public float y;
        public float z;

        public ExportVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static ExportVector3 FromVector3(Vector3 vec)
        {
            return new ExportVector3(vec.x, vec.y, vec.z);
        }
    }

    public class ExportQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public ExportQuaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static ExportQuaternion FromQuaternion(Quaternion q)
        {
            return new ExportQuaternion(q.x, q.y, q.z, q.w);
        }
    }

    public class ChannelObject { }
    public class MeshObject 
    {
        public List<ExportVector3> vertices = new List<ExportVector3>();
        public List<int> triangles = new List<int>();
    }

    public class ObjectTransform
    {
        public ExportVector3 position;
        public ExportQuaternion rotation;
        public ExportVector3 scale;
    }

    public class ExportObject
    {
        public string name;

        public ChannelObject channel;
        public MeshObject mesh;

        public ObjectTransform transform;

        public static ExportObject ChannelObjectFrom(GameObject channelObject)
        {
            var result = new ExportObject();
            result.name = channelObject.name;
            result.channel = new ChannelObject();

            result.transform = new ObjectTransform();
            result.transform.position = ExportVector3.FromVector3(channelObject.transform.position);
            result.transform.rotation = ExportQuaternion.FromQuaternion(channelObject.transform.rotation);
            result.transform.scale = ExportVector3.FromVector3(channelObject.transform.lossyScale);

            return result;
        }

        public static ExportObject MeshObjectFrom(GameObject meshObject)
        {
            var result = new ExportObject();
            result.name = meshObject.name;

            Mesh mesh = meshObject.GetComponent<SkinnedMeshRenderer>() != null ? 
                meshObject.GetComponent<SkinnedMeshRenderer>().sharedMesh : meshObject.GetComponent<MeshFilter>().mesh;

            result.mesh = new MeshObject();
            result.mesh.vertices = mesh.vertices.Select(x => new ExportVector3(x.x, x.y, x.z)).ToList();
            result.mesh.triangles = mesh.triangles.ToList();

            result.transform = new ObjectTransform();
            result.transform.position = ExportVector3.FromVector3(meshObject.transform.position);
            result.transform.rotation = ExportQuaternion.FromQuaternion(meshObject.transform.rotation);
            result.transform.scale = ExportVector3.FromVector3(meshObject.transform.lossyScale);

            return result;
        }
    }
}
