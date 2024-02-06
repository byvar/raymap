using System;
using UnityEngine;

namespace Assets.Scripts.GenericExport.Model
{
    public class ChannelObject { }

    public class ExportObject
    {
        public string name;

        public ChannelObject channel;

        public static ExportObject ChannelObjectFrom(GameObject channelObject)
        {
            var result = new ExportObject();
            result.name = channelObject.name;
            result.channel = new ChannelObject();

            return result;
        }
    }
}
