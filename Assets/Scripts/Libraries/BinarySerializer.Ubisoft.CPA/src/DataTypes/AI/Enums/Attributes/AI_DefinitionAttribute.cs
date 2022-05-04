using System;

namespace BinarySerializer.Ubisoft.CPA {
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class AI_DefinitionAttribute : Attribute {
		public string ScriptName { get; }
		public string English { get; }
		public string French { get; }
		public string Type { get; }
		public AI_DefinitionAttribute(
			string SCR = null,
			string EN = null,
			string FR = null,
			string ED = null,
			string Type = null
		) {
			ScriptName = SCR;
			if (ED != null) {
				English = ED;
				French = ED;
			} else {
				English = EN;
				French = FR;
			}
			this.Type = Type;
		}
	}
}