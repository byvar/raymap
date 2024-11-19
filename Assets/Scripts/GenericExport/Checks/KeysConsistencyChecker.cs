using Assets.Scripts.GenericExport.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GenericExport.Checks
{


    public static class KeysConsistencyChecker
    {
        public static void CheckForKeysConsistency(Perso3DAnimatedData perso3DAnimatedData)
        {
            int keysCount = 0;
            bool keysCountSet = false;

            foreach (var state in perso3DAnimatedData.states)
            {
                foreach (var frame in state.Value)
                {
                    if (keysCount != frame.Value.dataBlocks.Count && keysCountSet)
                    {
                        throw new ModelCheckFailedException(
                            $"Keys consistency check failed in state {state.Key} frame {frame.Key}"
                        );
                    }

                    keysCount = frame.Value.dataBlocks.Count;
                    keysCountSet = true;
                }
            }
        }
    }
}
