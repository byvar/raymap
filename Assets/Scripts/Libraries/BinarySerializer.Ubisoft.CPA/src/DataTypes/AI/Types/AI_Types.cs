namespace BinarySerializer.Ubisoft.CPA {
	/* TODO: Add the following
	 * - TT PC final
	 * - R2 3DS
	 * - R2 N64: there are some DS exclusive functions, conditions etc.
	 * - RRR DS proto
	 * - RRR DS final
	 * - Donald PC
	 * - Donald N64
	 * - Laura & Alex
	 * - Rayman M PS2 demo
	 * - Rayman Arena GC/Xb
	 * - Dinosaur PS2
	 */

    public abstract class AI_Types {
        //public string[] functionTypes;
        public AI_Keyword[] Keywords { get; set; }
        public AI_Operator[] Operators { get; set; }
		public AI_Function[] Functions { get; set; }
		public AI_Procedure[] Procedures { get; set; }
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
		public AI_Function? GetFunction(uint index) {
			if (index < Functions.Length) return Functions[index];
			return null;
		}
		public AI_Procedure? GetProcedure(uint index) {
			if (index < Procedures.Length) return Procedures[index];
			return null;
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
