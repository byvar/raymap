using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Unity.AnimationExporting
{
    class AnimationsModelConstructor
    {
        AnimationsModel animationsModel = new AnimationsModel();

        private bool isChannelObj(GameObject obj)
        {
            return obj.name.StartsWith("Channel");
        }

        private AnimationFrameModel traverseRecursivelyChildrenChannels(GameObject obj, AnimationFrameModel animationFrameModel)
        {
            if (obj == null)
            {
                return animationFrameModel;
            }

            foreach (Transform child in obj.transform)
            {
                if (child == null || !isChannelObj(child.gameObject))
                    continue;

                GameObject channelBone = getChannelBone(child.gameObject);

                animationFrameModel.addNode(isChannelObj(obj) ? getChannelBone(obj).name : null, channelBone.name,
                    child.gameObject.transform.position.x,
                    child.gameObject.transform.position.y,
                    child.gameObject.transform.position.z,
                    child.gameObject.transform.rotation.x,
                    child.gameObject.transform.rotation.y,
                    child.gameObject.transform.rotation.z,
                    child.gameObject.transform.lossyScale.x,
                    child.gameObject.transform.lossyScale.y,
                    child.gameObject.transform.lossyScale.z);
                traverseRecursivelyChildrenChannels(child.gameObject, animationFrameModel);
            }
            return animationFrameModel;
        }

        private GameObject getChannelBone(GameObject obj)
        {
            foreach (Transform child in obj.transform)
            {
                if (child == null || isChannelObj(child.gameObject))
                {
                    continue;
                }
                if (child.gameObject.name.StartsWith("Bone"))
                {
                    return child.gameObject;
                } else
                {
                    return getChannelBone(child.gameObject);
                }
            }
            throw new KeyNotFoundException("Bone for channel not found!");
        }

        public void addAnimationFrameToAnimationClip(string animationClipName, PersoBehaviour persoBehaviour)
        {
            var animationFrameModel = traverseRecursivelyChildrenChannels(persoBehaviour.gameObject, new AnimationFrameModel());
            animationsModel.addAnimationFrameModelToAnimationClip(animationClipName, animationFrameModel);
        }

        public AnimationsModel getAnimationsModel()
        {
            return animationsModel;
        }
    }
}
