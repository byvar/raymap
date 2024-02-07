using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GenericExport.Model
{
    public class ChannelObject { }
    public class MeshObject 
    {
        public List<Vector3> vertices = new List<Vector3>();
        public List<int> triangles = new List<int>();
    }

    public class ObjectTransform
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
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
            result.transform.position = channelObject.transform.position;
            result.transform.rotation = channelObject.transform.rotation;
            result.transform.scale = channelObject.transform.lossyScale;

            return result;
        }

        public static ExportObject MeshObjectFrom(GameObject meshObject)
        {
            var result = new ExportObject();
            result.name = meshObject.name;

            Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;

            result.mesh = new MeshObject();
            result.mesh.vertices = mesh.vertices.ToList();
            result.mesh.triangles = mesh.triangles.ToList();

            result.transform = new ObjectTransform();
            result.transform.position = meshObject.transform.position;
            result.transform.rotation = meshObject.transform.rotation;
            result.transform.scale = meshObject.transform.lossyScale;

            return result;
        }
    }
}
