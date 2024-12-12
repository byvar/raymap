﻿namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class AI_Node_String : U64_Struct {
		public U64_String String { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			String = s.SerializeObject<U64_String>(String, name: nameof(String));
		}
	}
}