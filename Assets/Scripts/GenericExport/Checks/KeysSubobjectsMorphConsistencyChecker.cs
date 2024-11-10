using Assets.Scripts.GenericExport.Model;
using Assets.Scripts.GenericExport.Model.DataBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GenericExport.Checks
{
    public class GeometryData
    {
        public List<ExportVector3> vertices;
        public List<int> triangles;

        public bool IsConformantTo(ConcreteWholeSubmeshInPoseDataBlock encounteredSubobject)
        {
            if (triangles.Count != encounteredSubobject.triangles.Count)
            {
                return false;
            }

            bool trianglesCompliance = true;

            for (int i = 0; i < triangles.Count; i++)
            {
                if (triangles[i] != encounteredSubobject.triangles[i]) { trianglesCompliance = false; break; }
            }

            return vertices.Count == (encounteredSubobject.vertices.Count) && trianglesCompliance;
        }
    }

    public static class KeysSubobjectsMorphConsistencyChecker
    {
        public static void CheckForKeysSubobjectsMorphConsistency(Perso3DAnimatedData perso3DAnimatedData)
        {
            var keyToGeometryDataDictionary = new Dictionary<string, GeometryData>();

            foreach (var state in perso3DAnimatedData.states)
            {
                foreach (var frame in state.Value)
                {
                    foreach (var subobject in frame.Value.dataBlocks)
                    {
                        if (!keyToGeometryDataDictionary.ContainsKey(subobject.Key))
                        {
                            var encounteredSubobject = subobject.Value as ConcreteWholeSubmeshInPoseDataBlock;
                            keyToGeometryDataDictionary.Add(
                                subobject.Key,
                                new GeometryData() { 
                                    vertices = encounteredSubobject.vertices,
                                    triangles = encounteredSubobject.triangles 
                                });
                        } else
                        {
                            var encounteredSubobject = subobject.Value as ConcreteWholeSubmeshInPoseDataBlock;
                            var storedGeometryData = keyToGeometryDataDictionary[subobject.Key];

                            if (!storedGeometryData.IsConformantTo(encounteredSubobject))
                            {
                                throw new ModelCheckFailedException(
                                    $"Keys subobjects morph consistency check failed for state={state.Key}, frame={frame.Key}, key={subobject.Key}"
                                );
                            }
                        }
                    }
                    
                }
            }
        }
    }
}
