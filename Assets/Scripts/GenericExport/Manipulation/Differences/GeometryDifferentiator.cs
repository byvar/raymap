using Assets.Scripts.GenericExport.Model.DataBlocks;
using Assets.Scripts.GenericExport.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GenericExport.Model.Differences;

namespace Assets.Scripts.GenericExport.Manipulation.Differences
{
    public static class GeometryDifferentiator
    {
        public static List<ExportVector3> Subtract(
            ConcreteWholeSubmeshInPoseDataBlock currentSubobjectFormInHistory, SubobjectPeriodInHistory subobjectPeriodInHistory)
        {
            var result = new List<ExportVector3>();

            for (int i = 0; i < currentSubobjectFormInHistory.vertices.Count; i++)
            {
                result.Add(currentSubobjectFormInHistory.vertices[i] - subobjectPeriodInHistory.concreteGeometry.vertices[i]);
            }

            return result;
        }

        public static List<ExportVector3> SubtractMorphEvolutionPeriods(
            SubobjectPeriodInHistory subobjectPeriodInHistory1, SubobjectPeriodInHistory subobjectPeriodInHistory2)
        {
            var result = new List<ExportVector3>();

            for (int i = 0; i < subobjectPeriodInHistory1.morphEvolutionGeometry.verticesOffsets.Count; i++)
            {
                result.Add(subobjectPeriodInHistory1.morphEvolutionGeometry.verticesOffsets[i] - subobjectPeriodInHistory2.morphEvolutionGeometry.verticesOffsets[i]);
            }

            return result;
        }

        public static bool AreOffsetsDifferent(List<ExportVector3> verticesOffsetsA, List<ExportVector3> verticesOffsetsB)
        {
            var result = false;

            for (int i = 0; i < verticesOffsetsA.Count; i++)
            {
                if ((verticesOffsetsA[i] - verticesOffsetsB[i]).magnitude > 0.000001f)
                {
                    return true;
                }
            }

            return result;
        }

        public static EvaluatedGeometry Consolidate(
            EvaluatedGeometry evaluatedGeometry, SubobjectPeriodInHistory subobjectPeriodInHistory)
        {
            if (subobjectPeriodInHistory.periodType == SubobjectHistoryPeriodType.CONCRETE_FORM)
            {
                return new EvaluatedGeometry()
                {
                    vertices = subobjectPeriodInHistory.concreteGeometry.vertices,
                    triangles = subobjectPeriodInHistory.concreteGeometry.triangles
                };
            } else
            {
                return new EvaluatedGeometry()
                {
                    vertices = VerticesAdder.Add(evaluatedGeometry.vertices, subobjectPeriodInHistory.morphEvolutionGeometry.verticesOffsets),
                    triangles = evaluatedGeometry.triangles
                };
            }
        }
    }

    public static class VerticesAdder
    {
        public static List<ExportVector3> Add(List<ExportVector3> vertices, List<ExportVector3> verticesOffsets)
        {
            var result = new List<ExportVector3>();

            for (int i = 0; i < vertices.Count; i++)
            {
                result.Add(vertices[i] + verticesOffsets[i]);
            }

            return result;
        }
    }
}
