using Assets.Scripts.GenericExport.Model;
using Assets.Scripts.GenericExport.Model.DataBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GenericExport.Manipulation
{
    public static class SubobjectsTrendsWithinStateObtainer
    {
        public static SubobjectsInStatesTrendsInfo ObtainSubobjectsTrendsWithinStateInfo(Perso3DAnimatedData perso3DAnimatedData)
        {
            var subobjectsHistoryBuilder = new SubobjectsHistoryBuilder();

            foreach (var state in perso3DAnimatedData.states)
            {
                foreach (var frame in state.Value)
                {
                    foreach (var subobject in frame.Value.dataBlocks)
                    {
                        subobjectsHistoryBuilder
                            .AddToHistory(
                                state.Key,
                                frame.Key,
                                subobject.Key,
                                subobject.Value as ConcreteWholeSubmeshInPoseDataBlock
                            );
                    }
                }
            }

            return subobjectsHistoryBuilder.Build();
        }
    }
}
