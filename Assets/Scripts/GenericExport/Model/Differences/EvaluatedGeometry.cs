using Assets.Scripts.GenericExport.Model.DataBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GenericExport.Model.Differences
{
    public class EvaluatedGeometry
    {
        public List<ExportVector3> vertices = new List<ExportVector3>();
        public List<int> triangles = new List<int>();
    }
}
