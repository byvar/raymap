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

            foreach (Tuple<GameObject, GameObject> parentChildFrameObjects in PersoTraverser.GetChannelsHierarchy(persoBehaviour))
            {
                result.frameHierarchyTree.Add(parentChildFrameObjects.Item1?.name, parentChildFrameObjects.Item2.name);      
            }

            throw new InvalidOperationException();
        }
    }
}
