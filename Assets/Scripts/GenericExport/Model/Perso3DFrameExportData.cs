using Assets.Scripts.GenericExport.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GenericExport.Model
{
    public class Perso3DFrameExportData
    {
        public ObjectTransform persoTransform = new ObjectTransform ();
        public Tree<string, ExportObject> frameHierarchyTree = new Tree<string, ExportObject>();
    }
}
