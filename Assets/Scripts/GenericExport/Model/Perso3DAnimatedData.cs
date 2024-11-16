using Assets.Scripts.GenericExport.Model.DataBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GenericExport.Model
{
    public class Perso3DAnimatedData
    {
        public Dictionary<int, Dictionary<int, FrameDataBlock>> states = new Dictionary<int, Dictionary<int, FrameDataBlock>>();
    }
}
