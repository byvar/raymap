using Assets.Scripts.GenericExport.Model;
using Assets.Scripts.GenericExport.Model.Compressed;
using Assets.Scripts.GenericExport.Model.DataBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GenericExport.Manipulation.Compression
{
    public static class SubobjectsCompressedPerso3DAnimatedDataTransformer
    {
        public static SubobjectsCompressedPerso3DAnimatedData Transform(Perso3DAnimatedData perso3DAnimatedData)
        {
            var result = new SubobjectsCompressedPerso3DAnimatedData();

            foreach (var state in perso3DAnimatedData.states.OrderBy(x => x.Key))
            {
                result.states[state.Key] = new Dictionary<int, CompressedFrameDataBlock>();
                foreach (var frame in state.Value.OrderBy(x => x.Key))
                {
                    result.states[state.Key][frame.Key] = new CompressedFrameDataBlock();
                    foreach (var subobject in frame.Value.dataBlocks)
                    {
                        var compressedDataBlock = SubobjectCompressedFrameDataBlock.FromConcreteWholeSubmeshInPoseDataBlock(
                            subobject.Value as ConcreteWholeSubmeshInPoseDataBlock);
                        var subobjectGeometryData = SubobjectGeometryData.FromConcreteWholeSubmeshInPoseDataBlock(
                            subobject.Value as ConcreteWholeSubmeshInPoseDataBlock
                            );

                        result.states[state.Key][frame.Key].dataBlocks[subobject.Key] = compressedDataBlock;
                        if (!result.subobjects.ContainsKey(subobjectGeometryData.key))
                        {
                            result.subobjects[subobjectGeometryData.key] = subobjectGeometryData;
                        }
                    }
                }
            }

            return result;
        }
    }
}
