using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.AnimatedExportObjectModelDescription;
using UnityEngine;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing
{
    public class ArmatureModelConstructor
    {
        public ArmatureModel ConstructFrom(Transform[] bones)
        {
            var result = new ArmatureModel();
            foreach (var boneTransform in bones)
            {
                var channelGameObject = GetParentChannelFor(boneTransform);
            }
            return result;
        }

        private object GetParentChannelFor(Transform boneTransform)
        {
            throw new NotImplementedException();
        }
    }
}
