namespace BinarySerializer.Ubisoft.CPA {
	/* TODO: Add the following
	 * - R2 iOS
	 * - TT PC final
	 * - R2 3DS
	 * - R2 N64?
	 * - Donald PC
	 * - Donald N64
	 * - Laura & Alex
	 * - Rayman M (check PS2 versions)
	 * - Rayman Arena GC/Xb
	 */

    public abstract class AI_Types {
        //public string[] functionTypes;
        public AI_Keyword[] Keywords { get; set; }
        public AI_Operator[] Operators { get; set; }
		public string[] functionTable { get; set; }
		public string[] procedureTable { get; set; }
		public AI_Condition[] Conditions { get; set; }
		public AI_DsgVarType[] VariableTypes { get; set; }
		public AI_Field[] Fields { get; set; }
		public AI_MetaAction[] MetaActions { get; set; }
		public AI_InterpretType[] InterpretTypes { get; set; }

		public void Init() {
			InitArrays();
			CreateDictionaries();
		}

		protected abstract void InitInterpretTypes();
		protected abstract void InitVariableTypes();
		protected abstract void InitKeywords();
		protected abstract void InitOperators();
		protected abstract void InitConditions();
		protected abstract void InitFields();
		protected abstract void InitMetaActions();
		protected abstract void InitFunctions();
		protected abstract void InitProcedures();

		protected void InitArrays() {
			InitInterpretTypes();
			InitVariableTypes();
			InitKeywords();
			InitOperators();
			InitConditions();
			InitFields();
			InitMetaActions();
			InitFunctions();
			InitProcedures();
		}

		private void CreateDictionaries() {
			// TODO
		}

		public AI_Keyword? GetKeyword(uint index) {
			if(index < Keywords.Length) return Keywords[index];
			return null;
		}
		public AI_Operator? GetOperator(uint index) {
			if (index < Operators.Length) return Operators[index];
			return null;
		}
		public string GetFunction(uint index) {
			if (index < functionTable.Length) return functionTable[index];
			return "";
		}
		public string GetProcedure(uint index) {
			if (index < procedureTable.Length) return procedureTable[index];
			return "";
		}
		public AI_Condition? GetCondition(uint index) {
			if (index < Conditions.Length) return Conditions[index];
			return null;
		}
		public AI_Field? GetField(uint index) {
			if (index < Fields.Length) return Fields[index];
			return null;
		}
		public AI_MetaAction? GetMetaAction(uint index) {
			if (index < MetaActions.Length) return MetaActions[index];
			return null;
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
