using Assets.Scripts.GenericExport.Checks;
using Assets.Scripts.GenericExport.Model;
using Assets.Scripts.GenericExport.Model.DataBlocks;
using System.Linq;
using System;
using System.Collections.Generic;
using Assets.Scripts.GenericExport.Model.Differences;
using Assets.Scripts.GenericExport.Manipulation.Differences;

namespace Assets.Scripts.GenericExport.Manipulation
{
    

    public class SubobjectsHistoryBuilder
    {
        public SubobjectsInStatesTrendsInfo trends = new SubobjectsInStatesTrendsInfo();

        public void AddToHistory(
            int stateIndex,
            int frameIndex,
            string subobjectKey, ConcreteWholeSubmeshInPoseDataBlock currentSubobjectFormInHistory)
        {
            throw new NotImplementedException();
            //if (!trends.statesSubobjectsTrends.ContainsKey(stateIndex))
            //{
            //    trends.statesSubobjectsTrends[stateIndex] = new StateSubobjectsTrends();
            //}

            //if (!trends.statesSubobjectsTrends[stateIndex].subobjectsStories.ContainsKey(subobjectKey))
            //{
            //    trends.statesSubobjectsTrends[stateIndex].subobjectsStories[subobjectKey] = new SubobjectHistory();
            //    trends.statesSubobjectsTrends[stateIndex].subobjectsStories[subobjectKey].periodsInHistory.Add(
            //        new SubobjectPeriodInHistory()
            //        {
            //            frameStart = frameIndex,
            //            frameEnd = frameIndex,
            //            periodType = SubobjectHistoryPeriodType.CONCRETE_FORM,
            //            concreteGeometry = new ConcreteGeometry()
            //            {
            //                vertices = currentSubobjectFormInHistory.vertices,
            //                triangles = currentSubobjectFormInHistory.triangles,
            //            }
            //        }
            //    );
            //} else
            //{
            //    EvaluatedGeometry evaluatedGeometry = new EvaluatedGeometry();

            //    for (int i = 0; i < trends.statesSubobjectsTrends[stateIndex].subobjectsStories[subobjectKey].periodsInHistory.Count; i++)
            //    {
            //        evaluatedGeometry = GeometryDifferentiator.Consolidate(
            //            evaluatedGeometry,
            //            trends.statesSubobjectsTrends[stateIndex].subobjectsStories[subobjectKey].periodsInHistory[i]
            //            );
            //    }

            //    List<ExportVector3> newVerticesOffsets = GeometryDifferentiator.Subtract(
            //            currentSubobjectFormInHistory,
            //            evaluatedGeometry
            //        );

            //    var lastPeriodInHistory = trends.statesSubobjectsTrends[stateIndex].subobjectsStories[subobjectKey].periodsInHistory.Last();

            //    if (lastPeriodInHistory.periodType == SubobjectHistoryPeriodType.CONCRETE_FORM)
            //    {
            //        trends.statesSubobjectsTrends[stateIndex].subobjectsStories[subobjectKey].periodsInHistory.Add(
            //            new SubobjectPeriodInHistory()
            //            {
            //                frameStart = frameIndex,
            //                frameEnd = frameIndex,
            //                periodType = SubobjectHistoryPeriodType.LINEAR_MORPH_PROGRESSION,
            //                morphEvolutionGeometry = new MorphEvolutionGeometry()
            //                {
            //                    verticesOffsets = newVerticesOffsets,
            //                }
            //            }
            //        );
            //    } else
            //    {
            //        if (GeometryDifferentiator.AreOffsetsDifferent(newVerticesOffsets, lastPeriodInHistory.morphEvolutionGeometry.verticesOffsets))
            //        {
            //            trends.statesSubobjectsTrends[stateIndex].subobjectsStories[subobjectKey].periodsInHistory.Add(
            //                new SubobjectPeriodInHistory()
            //                {
            //                    frameStart = frameIndex,
            //                    frameEnd = frameIndex,
            //                    periodType = SubobjectHistoryPeriodType.CONCRETE_FORM,
            //                    concreteGeometry = new ConcreteGeometry()
            //                    {
            //                        vertices = currentSubobjectFormInHistory.vertices,
            //                        triangles = currentSubobjectFormInHistory.triangles,
            //                    }
            //                }
            //            );
            //        } else
            //        {
            //            trends.statesSubobjectsTrends[stateIndex].subobjectsStories[subobjectKey].periodsInHistory.Add(
            //                new SubobjectPeriodInHistory()
            //                {
            //                    frameStart = frameIndex,
            //                    frameEnd = frameIndex,
            //                    periodType = SubobjectHistoryPeriodType.LINEAR_MORPH_PROGRESSION,
            //                    morphEvolutionGeometry = new MorphEvolutionGeometry()
            //                    {
            //                        verticesOffsets = newVerticesOffsets,
            //                    }
            //                }
            //        );
            //        }
            //    }
            //}
        }

        public SubobjectsInStatesTrendsInfo Build()
        {
            throw new NotImplementedException();
            //return trends;
        }
    }
}
