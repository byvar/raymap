using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Unity.AnimationExporting;
using OpenSpace;
using OpenSpace.Object.Properties;
using UnityEngine;

namespace Assets.Scripts.Unity
{  
    public class PersoAnimationsDataExporter
    {
        private PersoBehaviour persoBehaviour;
        private int currentStateIndex;
        private global::OpenSpace.LinkedList<State> persoStates;
        private string[] stateNames;
        private int statesCount;
        public bool isAnimationsExportInProgress;
        private uint previousAnimationFrame;
        private JSONAnimationDataFileWriter jsonAnimationDataFileWriter;
        private AnimationsModelConstructor animationsModelConstructor;

        public PersoAnimationsDataExporter(PersoBehaviour persoBehaviour)
        {
            this.persoBehaviour = persoBehaviour;
            this.currentStateIndex = 0;
            persoStates = persoBehaviour.perso.p3dData.family.states;
            stateNames = persoBehaviour.perso.p3dData.family.states.Select(s => (s == null ? "Null" : s.ToString())).ToArray();
            statesCount = persoBehaviour.perso.p3dData.family.states.Count;
            isAnimationsExportInProgress = false;
        }

        public void InitExportProcess()
        {
            isAnimationsExportInProgress = true;
            persoBehaviour.playAnimationFramesAutomatically = false;
            persoBehaviour.playAnimation = true;
            previousAnimationFrame = 0;
            currentStateIndex = 0;
            jsonAnimationDataFileWriter = new JSONAnimationDataFileWriter();
            animationsModelConstructor = new AnimationsModelConstructor();
        }

        public void UpdateExportProcess()
        {
            if (currentStateIndex < statesCount)
            {
                if (persoBehaviour.currentFrame < previousAnimationFrame)
                {
                    //we looped animation, go to the next one
                    currentStateIndex += 1;
                    persoBehaviour.SetState(currentStateIndex);
                    previousAnimationFrame = 0;
                } else
                {
                    animationsModelConstructor.addAnimationFrameToAnimationClip("Animation " + currentStateIndex.ToString(), persoBehaviour);
                    persoBehaviour.currentFrame += 1;
                    previousAnimationFrame = persoBehaviour.currentFrame;

                    if (persoBehaviour.currentFrame > 500)
                    {
                        if (currentStateIndex < statesCount)
                        {
                            currentStateIndex += 1;
                            persoBehaviour.SetState(currentStateIndex);
                            previousAnimationFrame = 0;
                        } else
                        {
                            EndExportProcess();
                        }                            
                    }
                }                
            } else
            {
                EndExportProcess();
            }
        }

        public void EndExportProcess()
        {
            jsonAnimationDataFileWriter.writeAnimationModel(animationsModelConstructor.getAnimationsModel());
            isAnimationsExportInProgress = false;
        }
    }
}
