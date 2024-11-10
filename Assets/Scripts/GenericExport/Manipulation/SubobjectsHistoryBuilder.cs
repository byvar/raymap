using Assets.Scripts.GenericExport.Checks;
using Assets.Scripts.GenericExport.Model;
using Assets.Scripts.GenericExport.Model.DataBlocks;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GenericExport.Manipulation
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
    }

    public class SubobjectsHistoryBuilder
    {
        public SubobjectsInStatesTrendsInfo trends = new SubobjectsInStatesTrendsInfo();

        public void AddToHistory(
            int stateIndex,
            int frameIndex,
            string subobjectKey, ConcreteWholeSubmeshInPoseDataBlock currentSubobjectFormInHistory)
        {
            if (!trends.statesSubobjectsTrends.ContainsKey(stateIndex))
            {
                trends.statesSubobjectsTrends[stateIndex] = new StateSubobjectsTrends();
            }

            if (!trends.statesSubobjectsTrends[stateIndex].subobjectsStories.ContainsKey(subobjectKey))
            {
                trends.statesSubobjectsTrends[stateIndex].subobjectsStories[subobjectKey] = new SubobjectHistory();
                trends.statesSubobjectsTrends[stateIndex].subobjectsStories[subobjectKey].periodsInHistory.Add(
                    new SubobjectPeriodInHistory()
                    {
                        frameStart = frameIndex,
                        frameEnd = frameIndex,
                        periodType = SubobjectHistoryPeriodType.CONCRETE_FORM,
                        concreteGeometry = new ConcreteGeometry()
                        {
                            vertices = currentSubobjectFormInHistory.vertices,
                            triangles = currentSubobjectFormInHistory.triangles,
                        }
                    }
                );
            } else
            {
                trends.statesSubobjectsTrends[stateIndex].subobjectsStories[subobjectKey].periodsInHistory.Add(
                    new SubobjectPeriodInHistory()
                    {
                        frameStart = frameIndex,
                        frameEnd = frameIndex,
                        periodType = SubobjectHistoryPeriodType.LINEAR_MORPH_PROGRESSION,
                        morphEvolutionGeometry = new MorphEvolutionGeometry()
                        {
                            verticesOffsets = GeometryDifferentiator.Subtract(
                                currentSubobjectFormInHistory,
                                trends.statesSubobjectsTrends[stateIndex].subobjectsStories[subobjectKey].periodsInHistory[0]
                            )
                        }
                    }
                );

                //List<ExportVector3> currentDifference = null;

                //for (int i = 2; i < trends.statesSubobjectsTrends[stateIndex].subobjectsStories[subobjectKey].periodsInHistory.Count; i++)
                //{
                //    List<ExportVector3> newDifference = GeometryDifferentiator.SubtractMorphEvolutionPeriods(
                //        trends.statesSubobjectsTrends[stateIndex].subobjectsStories[subobjectKey].periodsInHistory[i],
                //        trends.statesSubobjectsTrends[stateIndex].subobjectsStories[subobjectKey].periodsInHistory[i - 1]
                //        );

                //    //if (currentDifference != null && GeometryDifferentiator.AreOffsetsDifferent(currentDifference, newDifference))
                //    //{
                //    //    throw new ModelCheckFailedException("");
                //    //}

                //    currentDifference = newDifference;
                //}

                //var previousSubobjectMorphEvolutionInfo =
                //    SubobjectMorphEvolutionInfoFetcher.FetchMorphEvolutionInfo(
                //        0,
                //        trends.statesSubobjectsTrends[stateIndex].subobjectsStories[subobjectKey].periodsInHistory,
                //        trends.statesSubobjectsTrends[stateIndex].subobjectsStories[subobjectKey].periodsInHistory[0],
                //        currentSubobjectFormInHistory
                //    );

                //for (int i = 1; i < trends.statesSubobjectsTrends[stateIndex].subobjectsStories[subobjectKey].periodsInHistory.Count; i++)
                //{
                //    var subobjectMorphEvolutionInfo =
                //        SubobjectMorphEvolutionInfoFetcher.FetchMorphEvolutionInfo(
                //            i,
                //            trends.statesSubobjectsTrends[stateIndex].subobjectsStories[subobjectKey].periodsInHistory,
                //            trends.statesSubobjectsTrends[stateIndex].subobjectsStories[subobjectKey].periodsInHistory[i],
                //            currentSubobjectFormInHistory
                //        );

                //    if (!subobjectMorphEvolutionInfo.IsConformantTo(previousSubobjectMorphEvolutionInfo))
                //    {
                //        throw new ModelCheckFailedException("");
                //    }
                //}
                //trends.statesSubobjectsTrends[stateIndex].subobjectsStories[subobjectKey].periodsInHistory.Add(
                //    new SubobjectPeriodInHistory()
                //    {
                //        frameStart = frameIndex,
                //        frameEnd = frameIndex,
                //        periodType = SubobjectHistoryPeriodType.LINEAR_MORPH_PROGRESSION,
                //    }
                //);
            }
        }

        public SubobjectsInStatesTrendsInfo Build()
        {
            return trends;
        }
    }
}
