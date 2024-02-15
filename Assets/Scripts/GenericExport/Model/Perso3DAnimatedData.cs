using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GenericExport.Model
{
    public static class ObjectDeterminer
    {
        public static bool IsSubmesh(Transform transform)
        {
            return (transform.GetComponent<SkinnedMeshRenderer>() != null ||
                transform.GetComponent<MeshFilter>() != null) && 
                transform.GetComponent<Renderer>()?.enabled == true;
        }

        public static bool IsChannel(Transform transform)
        {
            return transform.name.Contains("Channel");
        }

        public static string GetChainedChannelsKey(Transform submesh)
        {
            var baseT = submesh;
            string result = submesh.name;
            while (baseT.GetComponent<PersoBehaviour>() == null)
            {
                if (IsChannel(baseT))
                {
                    result = baseT.name + ">" + result;
                }
                baseT = baseT.parent;
            }
            return result;
        }
    }

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
            } else
            {
                throw new InvalidOperationException("Operation implemented only for two ConcreteWholeSubmeshInPoseDataBlock blocks!");
            }
        }
    }

    public class ExportVector3
    {
        public float x;
        public float y;
        public float z;

        public ExportVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static ExportVector3 FromVector3(Vector3 vec)
        {
            return new ExportVector3(
                x: vec.x,
                y: vec.y,
                z: vec.z
            );
        }

        public static ExportVector3 operator-(ExportVector3 v1, ExportVector3 v2)
        {
            return new ExportVector3(
                x: v1.x - v2.x,
                y: v1.y - v2.y,
                z: v1.z - v2.z
                );
        }
    }

    public class ExportQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public ExportQuaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static ExportQuaternion FromQuaternion(Quaternion quat)
        {
            return new ExportQuaternion(
                x: quat.x,
                y: quat.y,
                z: quat.z,
                w: quat.w
                );
        }

        public static ExportQuaternion RotationDifference(ExportQuaternion rotation1, ExportQuaternion rotation2)
        {
            return rotation1;
        }
    }

    public class ObjectTransform
    {
        public ExportVector3 position;
        public ExportQuaternion rotation;
        public ExportVector3 scale;

        public ObjectTransform(ExportVector3 position, ExportQuaternion rotation, ExportVector3 scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
    }

    public class ConcreteWholeSubmeshInPoseDataBlock : DataBlock
    {
        public ObjectTransform transform;
        public List<ExportVector3> vertices = new List<ExportVector3>();
        public List<int> triangles = new List<int>();

        public ConcreteWholeSubmeshInPoseDataBlock(
            ObjectTransform transform, List<ExportVector3> vertices, List<int> triangles)
        {
            this.transform = transform;
            this.vertices = vertices;
            this.triangles = triangles;
        }

        public static ConcreteWholeSubmeshInPoseDataBlock FromSubmesh(Transform child)
        {
            Mesh mesh = child.GetComponent<SkinnedMeshRenderer>() != null ?
                child.GetComponent<SkinnedMeshRenderer>().sharedMesh : child.GetComponent<MeshFilter>().mesh;

            return new ConcreteWholeSubmeshInPoseDataBlock(
                transform: new ObjectTransform(
                    position: ExportVector3.FromVector3(child.position),
                    rotation: ExportQuaternion.FromQuaternion(child.rotation),
                    scale: ExportVector3.FromVector3(child.lossyScale)
                    ),
                vertices: mesh.vertices.Select(x => new ExportVector3(x.x, x.y, x.z)).ToList(),
                triangles: mesh.triangles.ToList()
            );
        }
    }

    public class SubmeshDifferenceDataBlock : ConcreteWholeSubmeshInPoseDataBlock {
        public SubmeshDifferenceDataBlock(
            ObjectTransform transform, List<ExportVector3> vertices, List<int> triangles) : base(transform, vertices, triangles)
        {}
    }

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
            var commonKeys = a.dataBlocks.Keys.Intersect(b.dataBlocks.Keys).ToList();


            var result = new FrameDataBlock();

            foreach (var key in commonKeys)
            {
                var dataBlockA = a.dataBlocks[key];
                var dataBlockB = b.dataBlocks[key];

                result.dataBlocks[key] = DataBlock.DifferenceDataBlockBetween(dataBlockA, dataBlockB);
            }

            return result;
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
            } else
            {
                var previousFrameDataBlock = currentFrameDataBlocks[currentFrame - 1];
                var differenceFrameDataBlock = FrameDataBlock.DifferenceFrameDataBlockBetween(previousFrameDataBlock, dataToBeConsideredNow);
                return differenceFrameDataBlock;
            }
        }
    }

    public class Perso3DAnimatedData
    {
        public Dictionary<int, Dictionary<int, FrameDataBlock>> states = new Dictionary<int, Dictionary<int, FrameDataBlock>>();
    }
}
