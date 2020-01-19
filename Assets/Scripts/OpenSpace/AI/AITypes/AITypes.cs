using OpenSpace.Object;
using OpenSpace.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using OpenSpace.Object.Properties;

namespace OpenSpace.AI {
    public partial class AITypes {
        //public string[] functionTypes;
        public string[] keywordTable;
        public string[] operatorTable;
        public string[] functionTable;
        public string[] procedureTable;
        public string[] conditionTable;
        public DsgVarInfoEntry.DsgVarType[] dsgVarTypeTable;
        public string[] fieldTable;
        public string[] metaActionTable;
        public ScriptNode.NodeType[] nodeTypes;
        
        public ScriptNode.NodeType GetNodeType(byte functionType) {
            if (functionType < nodeTypes.Length) return nodeTypes[functionType];
            return ScriptNode.NodeType.Unknown;
        }

        public DsgVarInfoEntry.DsgVarType GetDsgVarType(uint type) {
            if (type < dsgVarTypeTable.Length) return dsgVarTypeTable[type];
            return DsgVarInfoEntry.DsgVarType.None;
        }

	}
}
