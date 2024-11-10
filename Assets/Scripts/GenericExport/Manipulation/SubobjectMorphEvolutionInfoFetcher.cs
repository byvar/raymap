using Assets.Scripts.GenericExport.Model;
using Assets.Scripts.GenericExport.Model.DataBlocks;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GenericExport.Manipulation
{
    public class SubobjectMorphEvolutionInfo
    {
        public List<ExportVector3> verticesOffsets = new List<ExportVector3>();

        public bool IsConformantTo(SubobjectMorphEvolutionInfo previousSubobjectMorphEvolutionInfo)
        {
            throw new NotImplementedException();
        }
    }

    public static class VerticesDifferentiator
    {
        public static List<ExportVector3> SubtractVertices(List<ExportVector3> verticesA, List<ExportVector3> verticesB)
        {
            throw new NotImplementedException();
        }
    }

    public class SubobjectMorphEvolutionInfoFetcher
    {
        public static SubobjectMorphEvolutionInfo FetchMorphEvolutionInfo(
            int periodIndex,
            List<SubobjectPeriodInHistory> allPeriodsInHistory,
            SubobjectPeriodInHistory subobjectPeriodInHistory, ConcreteWholeSubmeshInPoseDataBlock currentSubobjectFormInHistory)
        {
            throw new NotImplementedException();
            //if (subobjectPeriodInHistory.periodType == SubobjectHistoryPeriodType.CONCRETE_FORM)
            //{
            //    return new SubobjectMorphEvolutionInfo()
            //    {
            //        verticesOffsets = VerticesDifferentiator.SubtractVertices(
            //            currentSubobjectFormInHistory.vertices,
            //            subobjectPeriodInHistory.concreteGeometry.vertices
            //            )
            //    };
            //} else
            //{
            //    for (int i = 0; i < allPeriodsInHistory.Count; i++)
            //    {

            //    }
            //}
        }
    }
}
