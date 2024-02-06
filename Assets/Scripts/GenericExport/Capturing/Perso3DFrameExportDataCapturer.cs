using Assets.Scripts.GenericExport.Model;
using System;
using UnityEngine;

namespace Assets.Scripts.GenericExport.Capturing
{
    public class Perso3DFrameExportDataCapturer
    {
        public static Perso3DFrameExportData Capture3DFrameExportData(PersoBehaviour persoBehaviour)
        {
            var result = new Perso3DFrameExportData();

            foreach (Tuple<GameObject, GameObject> parentChildFrameChannels in PersoTraverser.GetChannelsHierarchy(persoBehaviour))
            {
                result.frameHierarchyTree.Add(
                    parentKey: parentChildFrameChannels.Item1?.name,
                    key: parentChildFrameChannels.Item2.name,
                    value: ExportObject.ChannelObjectFrom(parentChildFrameChannels.Item2)
                );      
            }

            foreach (Tuple<GameObject, GameObject> parentChildChannelMesh in PersoTraverser.GetChannelsMeshesHierarchy(persoBehaviour))
            {
                result.frameHierarchyTree.Add(
                    parentKey: parentChildChannelMesh.Item1?.name,
                    key: parentChildChannelMesh.Item2.name,
                    value: ExportObject.MeshObjectFrom(parentChildChannelMesh.Item2)
                );
            }

            throw new NotImplementedException();
        }
    }
}
