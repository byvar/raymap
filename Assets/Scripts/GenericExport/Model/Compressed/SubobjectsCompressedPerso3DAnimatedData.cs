using Assets.Scripts.GenericExport.Model.DataBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GenericExport.Model.Compressed
{
    public class SubobjectCompressedFrameDataBlock
    {
        public string key;
        public ObjectTransform transform;
        public string geometryDataReference;

        public static SubobjectCompressedFrameDataBlock FromConcreteWholeSubmeshInPoseDataBlock(ConcreteWholeSubmeshInPoseDataBlock concreteWholeSubmeshInPoseDataBlock)
        {
            return new SubobjectCompressedFrameDataBlock()
            {
                key = concreteWholeSubmeshInPoseDataBlock.key,
                transform = concreteWholeSubmeshInPoseDataBlock.transform,
                geometryDataReference = concreteWholeSubmeshInPoseDataBlock.GetGeometryDataHash()
            };
        }
    }

    public class SubobjectGeometryData
    {
        public string key;
        public List<ExportVector3> vertices = new List<ExportVector3>();
        public List<int> triangles = new List<int>();

        public static SubobjectGeometryData FromConcreteWholeSubmeshInPoseDataBlock(ConcreteWholeSubmeshInPoseDataBlock concreteWholeSubmeshInPoseDataBlock)
        {
            return new SubobjectGeometryData()
            {
                vertices = concreteWholeSubmeshInPoseDataBlock.vertices,
                triangles = concreteWholeSubmeshInPoseDataBlock.triangles,
                key = concreteWholeSubmeshInPoseDataBlock.GetGeometryDataHash()
            };
        }
    }

    public class CompressedFrameDataBlock
    {
        public Dictionary<string, SubobjectCompressedFrameDataBlock> dataBlocks = new Dictionary<string, SubobjectCompressedFrameDataBlock> ();
    }

    public class SubobjectsCompressedPerso3DAnimatedData
    {
        public Dictionary<string, SubobjectGeometryData> subobjects = new Dictionary<string, SubobjectGeometryData>();
        public Dictionary<int, Dictionary<int, CompressedFrameDataBlock>> states = 
            new Dictionary<int, Dictionary<int, CompressedFrameDataBlock>>();
    }
}
