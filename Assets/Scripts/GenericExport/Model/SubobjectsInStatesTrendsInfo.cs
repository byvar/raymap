using Assets.Scripts.GenericExport.Model.DataBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GenericExport.Model
{
    public enum SubobjectHistoryPeriodType
    {
        CONCRETE_FORM,
        LINEAR_MORPH_PROGRESSION,
    }

    public class ConcreteGeometry
    {
        public List<ExportVector3> vertices = new List<ExportVector3>();
        public List<int> triangles = new List<int>();
    }

    public class MorphEvolutionGeometry
    {
        public List<ExportVector3> verticesOffsets = new List<ExportVector3>();
    }

    public class SubobjectPeriodInHistory
    {
        public int frameStart;
        public int frameEnd;

        public SubobjectHistoryPeriodType periodType;

        public ConcreteGeometry concreteGeometry;
        public MorphEvolutionGeometry morphEvolutionGeometry;
    }

    public class SubobjectHistory
    {
        public List<SubobjectPeriodInHistory> periodsInHistory = new List<SubobjectPeriodInHistory>();
    }

    public class StateSubobjectsTrends
    {
        public Dictionary<string, SubobjectHistory> subobjectsStories = new Dictionary<string, SubobjectHistory>();
    }

    public class SubobjectsInStatesTrendsInfo
    {
        public Dictionary<int, StateSubobjectsTrends> statesSubobjectsTrends = new Dictionary<int, StateSubobjectsTrends>();
    }
}
