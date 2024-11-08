using Assets.Scripts.GenericExport.Manipulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GenericExport.Model.DataBlocks
{
    public class FrameDataBlock
    {
        public Dictionary<string, DataBlock> dataBlocks = new Dictionary<string, DataBlock>();

        public static FrameDataBlock GetConcreteWholeSubmeshesInPoseFrameDataBlock(PersoBehaviour persoBehaviour)
        {
            Dictionary<string, ConcreteWholeSubmeshInPoseDataBlock> wholeSubmeshes = new Dictionary<string, ConcreteWholeSubmeshInPoseDataBlock>();

            foreach (var child in persoBehaviour.GetComponentsInChildren<Transform>())
            {
                if (ObjectDeterminer.IsSubmesh(child))
                {
                    wholeSubmeshes[ObjectDeterminer.GetChainedChannelsKey(child)] =
                        ConcreteWholeSubmeshInPoseDataBlock.FromSubmesh(child);
                }
            }

            var result = new FrameDataBlock();
            result.dataBlocks = wholeSubmeshes.ToDictionary(x => x.Key, x => x.Value as DataBlock);
            return result;
        }

        public static FrameDataBlock DifferenceFrameDataBlockBetween(
            FrameDataBlock a,
            FrameDataBlock b
            )
        {
            foreach (var subobjectA in SubobjectsFetcher.FetchSubobjectsInfo(a))
            {
                foreach (var subobjectB in SubobjectsFetcher.FetchSubobjectsInfo(b))
                {
                    if (SubobjectsComparator.AreSubobjectsCompliant(subobjectA, subobjectB))
                    {
                        var keyChangeIndicator = SubobjectKeyChangeDeterminer.DetermineKeyChange(subobjectA, subobjectB);
                        var verticesDifference = SubobjectsVerticesDifferentiator.Difference(subobjectA, subobjectB);
                    }
                }
            }


            //var commonKeys = a.dataBlocks.Keys.Intersect(b.dataBlocks.Keys).ToList();


            //var result = new FrameDataBlock();

            //foreach (var key in commonKeys)
            //{
            //    var dataBlockA = a.dataBlocks[key];
            //    var dataBlockB = b.dataBlocks[key];

            //    result.dataBlocks[key] = DataBlock.DifferenceDataBlockBetween(dataBlockA, dataBlockB);
            //}

            //return result;
        }

        public static FrameDataBlock GetConsolidated(
            Dictionary<int, FrameDataBlock> currentFrameDataBlocks,
            int currentFrame,
            PersoBehaviour persoBehaviour)
        {
            var dataToBeConsideredNow = FrameDataBlock.GetConcreteWholeSubmeshesInPoseFrameDataBlock(persoBehaviour);

            if (currentFrame == 0)
            {
                return dataToBeConsideredNow;
            }
            else
            {
                var previousFrameDataBlock = currentFrameDataBlocks[currentFrame - 1];
                var differenceFrameDataBlock = FrameDataBlock.DifferenceFrameDataBlockBetween(previousFrameDataBlock, dataToBeConsideredNow);
                return differenceFrameDataBlock;
            }
        }
    }
}
