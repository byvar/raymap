namespace BinarySerializer.Ubisoft.CPA {
    public abstract class AI_Types {
        //public string[] functionTypes;
        public AI_Keyword[] Keywords { get; set; }
        public string[] operatorTable { get; set; }
		public string[] functionTable { get; set; }
		public string[] procedureTable { get; set; }
		public string[] conditionTable { get; set; }
		public AI_DsgVarType[] VariableTypes { get; set; }
		public string[] fieldTable { get; set; }
		public string[] metaActionTable { get; set; }
		public AI_InterpretType[] InterpretTypes { get; set; }

		public void Init() {
			InitArrays();
			CreateDictionaries();
		}

		protected abstract void InitArrays();

		private void CreateDictionaries() {
			// TODO
		}

		public AI_Keyword? GetKeyword(uint index) {
			if(index < Keywords.Length) return Keywords[index];
			return null;
		}
		public string GetOperator(uint index) {
			if (index < operatorTable.Length) return operatorTable[index];
			return "";
		}
		public string GetFunction(uint index) {
			if (index < functionTable.Length) return functionTable[index];
			return "";
		}
		public string GetProcedure(uint index) {
			if (index < procedureTable.Length) return procedureTable[index];
			return "";
		}
		public string GetCondition(uint index) {
			if (index < conditionTable.Length) return conditionTable[index];
			return "";
		}
		public string GetField(uint index) {
			if (index < fieldTable.Length) return fieldTable[index];
			return "";
		}
		public string GetMetaAction(uint index) {
			if (index < metaActionTable.Length) return metaActionTable[index];
			return "";
		}

		public AI_InterpretType GetNodeType(byte functionType) {
            if (functionType < InterpretTypes.Length) return InterpretTypes[functionType];
            return AI_InterpretType.Unknown;
        }

        public AI_DsgVarType GetDsgVarType(uint type) {
            if (type < VariableTypes.Length) return VariableTypes[type];
            return AI_DsgVarType.None;
        }
	}
}
