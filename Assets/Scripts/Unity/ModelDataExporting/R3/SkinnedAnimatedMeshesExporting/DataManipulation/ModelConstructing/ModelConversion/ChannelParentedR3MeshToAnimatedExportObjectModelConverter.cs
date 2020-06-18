using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Unity.ModelDataExporting.MathDescription;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.MeshGeometryConstructing;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.ModelConversion;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.AnimatedExportObjectModelDescription;
using UnityEngine;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.ModelConversion
{
    public class ChannelParentedR3MeshToAnimatedExportObjectModelConverter : R3MeshToAnimatedExportObjectModelConverterBase
    {
        protected override MeshGeometry DeriveMeshGeometryData(Mesh mesh, Transform[] bones)
        {
            var parentChannel = bones[0];
            var result = new MeshGeometry();
            var verticesList = mesh.vertices.Select(x => new Vector3d(x.x, x.y, x.z)).ToList();
            Dictionary<string, Dictionary<int, float>> bonesWeights = new Dictionary<string, Dictionary<int, float>>();

            Dictionary<int, float> verticesBoneWeights = new Dictionary<int, float>();
            for (int i = 0; i < verticesList.Count; i++)
            {
                verticesBoneWeights.Add(i, 1.0f);
            }

            bonesWeights.Add(parentChannel.name, verticesBoneWeights);

            result.vertices = verticesList;
            result.triangles = MeshDataHelper.GetTrianglesList(mesh.triangles);
            result.bonesWeights = bonesWeights;
            result.normals = MeshDataHelper.GetNormals(mesh.normals);
            result.uvMaps = MeshDataHelper.GetUvMaps(mesh);
            return result;
        }

        protected override Dictionary<string, TransformModel> GetBonesBindPoseTransforms(R3AnimatedMesh r3AnimatedMesh)
        {
            GameObject parentChannelGameObject = r3AnimatedMesh.GetParentChannel();
            var result = new Dictionary<string, TransformModel>();
            result.Add(parentChannelGameObject.name,
                new TransformModel(
                        new Vector3d(0.0f, 0.0f, 0.0f),
                        new MathDescription.Quaternion(1.0f, 0.0f, 0.0f, 0.0f),
                        new Vector3d(1.0f, 1.0f, 1.0f),
                        new Vector3d(0.0f, 0.0f, 0.0f),
                        new MathDescription.Quaternion(1.0f, 0.0f, 0.0f, 0.0f),
                        new Vector3d(1.0f, 1.0f, 1.0f)
                    ));
            return result;
        }

        protected override Transform[] GetBonesTransforms(R3AnimatedMesh r3AnimatedMesh)
        {
            return new Transform[] { r3AnimatedMesh.GetParentChannel().transform };
        }

        protected override Mesh GetMesh(R3AnimatedMesh r3AnimatedMesh)
        {
            return r3AnimatedMesh.GetComponent<MeshFilter>().mesh;
        }
    }
}
