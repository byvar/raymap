using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GenericExport.Model.DataBlocks
{
    public class DataBlock
    {
        public string key;

        public static DataBlock DifferenceDataBlockBetween(DataBlock a, DataBlock b)
        {
            if (a is ConcreteWholeSubmeshInPoseDataBlock && b is ConcreteWholeSubmeshInPoseDataBlock)
            {
                ConcreteWholeSubmeshInPoseDataBlock aa = a as ConcreteWholeSubmeshInPoseDataBlock;
                ConcreteWholeSubmeshInPoseDataBlock bb = b as ConcreteWholeSubmeshInPoseDataBlock;

                var verticesDifference = aa.vertices.Select(x => new ExportVector3(x.x, x.y, x.z)).ToList();

                for (int i = 0; i < bb.vertices.Count; i++)
                {
                    verticesDifference[i] = bb.vertices[i] - aa.vertices[i];
                }

                var result = new ConcreteWholeSubmeshInPoseDataBlock(
                    transform: new ObjectTransform(
                        position: bb.transform.position - aa.transform.position,
                        rotation: ExportQuaternion.RotationDifference(aa.transform.rotation, bb.transform.rotation),
                        scale: bb.transform.scale - aa.transform.scale
                        ),
                    vertices: verticesDifference,
                    triangles: new List<int>()
                    );
                return result;
            }
            else
            {
                throw new InvalidOperationException("Operation implemented only for two ConcreteWholeSubmeshInPoseDataBlock blocks!");
            }
        }
    }
}
