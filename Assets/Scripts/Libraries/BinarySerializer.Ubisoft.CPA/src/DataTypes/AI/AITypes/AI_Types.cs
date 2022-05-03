namespace BinarySerializer.Ubisoft.CPA {
    public abstract class AI_Types {
        //public string[] functionTypes;
        public string[] keywordTable { get; set; }
        public string[] operatorTable { get; set; }
		public string[] functionTable { get; set; }
		public string[] procedureTable { get; set; }
		public string[] conditionTable { get; set; }
		public AI_DsgVarType[] dsgVarTypeTable { get; set; }
		public string[] fieldTable { get; set; }
		public string[] metaActionTable { get; set; }
		public AI_InterpretType[] nodeTypes { get; set; }

		public void Init() {
			InitArrays();
			CreateDictionaries();
		}

		protected abstract void InitArrays();

		private void CreateDictionaries() {
			// TODO
		}

		public AI_InterpretType GetNodeType(byte functionType) {
            if (functionType < nodeTypes.Length) return nodeTypes[functionType];
            return AI_InterpretType.Unknown;
        }

        public AI_DsgVarType GetDsgVarType(uint type) {
            if (type < dsgVarTypeTable.Length) return dsgVarTypeTable[type];
            return AI_DsgVarType.None;
        }
	}
}
