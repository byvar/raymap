using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Unity.AnimationExporting;
using Assets.Scripts.Unity.AnimationExporting.DataManipulation;
using OpenSpace;
using OpenSpace.Object.Properties;
using UnityEngine;

namespace Assets.Scripts.Unity
{  
    public class PersoAnimationsDataExporter
    {
        private PersoAnimationStatesDataManipulator persoAnimationStatesDataManipulator;
        private JSONAnimationDataFileWriter jsonAnimationDataFileWriter;

        public PersoAnimationsDataExporter(PersoBehaviour persoBehaviour)
        {
            this.persoAnimationStatesDataManipulator = new PersoAnimationStatesDataManipulator(persoBehaviour);
        }

        public void ExportPersoStatesAnimations()
        {
            AnimationsModel animationsModel = new AnimationsModel();
            foreach (var animationClip in persoAnimationStatesDataManipulator.IterateAnimationClips())
            {
                foreach (var animationKeyframe in animationClip.IterateKeyframes())
                {
                    animationsModel.addAnimationFrameModelToAnimationClip(
                        animationClip.Name, animationKeyframe.GetAnimationFrameModel(), animationKeyframe.FrameNumber);
                }
            }
            jsonAnimationDataFileWriter.writeAnimationModel(animationsModel);
        }
    }
}
