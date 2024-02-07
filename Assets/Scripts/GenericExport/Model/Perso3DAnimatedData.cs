using System.Collections.Generic;

namespace Assets.Scripts.GenericExport.Model
{
    public class Perso3DAnimatedData
    {
        public Dictionary<int, Dictionary<int, Perso3DFrameExportData>> states = 
            new Dictionary<int, Dictionary<int, Perso3DFrameExportData>>();
    }
}
