namespace BinarySerializer.Ubisoft.CPA {
	// Find: (NU_)?M_DEFINE_[^ \t\(]*[ \t]*\([ \t]*e[^_]*_([^ \t,]*)[ \t]*,[ \t]*([^ \t,]*)[ \t]*,[ \t]*([^ \t,]*)[ \t]*,[ \t]*fn_.*
	// Replace: \[Definition\(SCR: \4, ED: \3\)] \2,
	public enum AI_Operator {
		#region DefOper
		//********************************************************************************************************************************************************************************************************************************************************
		// Math operator 
		[AI_Definition(SCR: "Operator_Plus", ED: "+")] ScalarPlusScalar,
		[AI_Definition(SCR: "Operator_Minus", ED: "-")] ScalarMinusScalar,
		[AI_Definition(SCR: "Operator_Mul", ED: "*")] ScalarMulScalar,
		[AI_Definition(SCR: "Operator_Div", ED: "/")] ScalarDivScalar,
		[AI_Definition(SCR: "Operator_UnaryMinus", ED: "_")] ScalarUnaryMinus,
		//********************************************************************************************************************************************************************************************************************************************************
		// Affect operator 
		[AI_Definition(SCR: "Operator_PlusAffect", ED: "+=")] PlusAffect,
		[AI_Definition(SCR: "Operator_MinusAffect", ED: "-=")] MinusAffect,
		[AI_Definition(SCR: "Operator_MulAffect", ED: "*=")] MulAffect,
		[AI_Definition(SCR: "Operator_DivAffect", ED: "/=")] DivAffect,
		[AI_Definition(SCR: "Operator_PlusPlusAffect", ED: "++")] PlusPlusAffect,
		[AI_Definition(SCR: "Operator_MinusMinusAffect", ED: "--")] MinusMinusAffect,
		[AI_Definition(SCR: "Operator_Affect", ED: ":=")] Affect,
		//********************************************************************************************************************************************************************************************************************************************************
		// Dot operator 
		[AI_Definition(SCR: "Operator_Dot", ED: ".")] Dot,
		//********************************************************************************************************************************************************************************************************************************************************
		// Vector Dot operator 
		[AI_Definition(SCR: ".X", ED: ".X")] GetVectorX,
		[AI_Definition(SCR: ".Y", ED: ".Y")] GetVectorY,
		[AI_Definition(SCR: ".Z", ED: ".Z")] GetVectorZ,
		//********************************************************************************************************************************************************************************************************************************************************
		// Vector operator 
		[AI_Definition(SCR: "Operator_VectorPlusVector", ED: "v+")] VectorPlusVector,
		[AI_Definition(SCR: "Operator_VectorMinusVector", ED: "v-")] VectorMinusVector,
		[AI_Definition(SCR: "Operator_VectorMulScalar", ED: "v*")] VectorMulScalar,
		[AI_Definition(SCR: "Operator_VectorDivScalar", ED: "v/")] VectorDivScalar,
		[AI_Definition(SCR: "Operator_VectorUnaryMinus", ED: "v_")] VectorUnaryMinus,
		//********************************************************************************************************************************************************************************************************************************************************
		// Vector Affect operator 
		[AI_Definition(SCR: ".X:=", ED: ".X:=")] SetVectorX,
		[AI_Definition(SCR: ".Y:=", ED: ".Y:=")] SetVectorY,
		[AI_Definition(SCR: ".Z:=", ED: ".Z:=")] SetVectorZ,
		//********************************************************************************************************************************************************************************************************************************************************
		// Ultra Operator 
		[AI_Definition(SCR: "Operator_Ultra", ED: "..")] Ultra,
		//********************************************************************************************************************************************************************************************************************************************************
		// Model Cast Operator 
		[AI_Definition(SCR: "Operator_ModelCast", ED: "@")] ModelCast,
		//********************************************************************************************************************************************************************************************************************************************************
		// Array Operator 
		[AI_Definition(SCR: "Operator_Array", ED: "[")] Array,
		//********************************************************************************************************************************************************************************************************************************************************
		// Affect Array Operator 
		[AI_Definition(SCR: "Operator_AffectArray", ED: ":={")] AffectArray,
		#endregion

		#region R3 PS2
		[AI_Definition(SCR: "Operator_Modulo", ED: "%")] ScalarModulo,
		#endregion
	}
}