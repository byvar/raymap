using System;
using UnityEngine;

namespace Assets.Scripts.GenericExport.Model
{
    public class ChannelObject { }
    public class MeshObject { }

    public class ExportObject
    {
        public string name;

        public ChannelObject channel;
        public MeshObject mesh;

        public static ExportObject ChannelObjectFrom(GameObject channelObject)
        {
            var result = new ExportObject();
            result.name = channelObject.name;
            result.channel = new ChannelObject();

            return result;
        }

        public static ExportObject MeshObjectFrom(GameObject meshObject)
        {
            var result = new ExportObject();
            result.name = meshObject.name;
            result.mesh = new MeshObject();

            return result;
        }
    }
}
