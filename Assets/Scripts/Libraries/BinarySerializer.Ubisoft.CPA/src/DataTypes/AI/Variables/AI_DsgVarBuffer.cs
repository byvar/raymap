namespace BinarySerializer.Ubisoft.CPA {
	public class AI_DsgVarBuffer : BinarySerializable {
		public AI_DsgVar Pre_DsgVar { get; set; }
		public bool Pre_Read { get; set; } = true;

		public AI_DsgVarValue[] Values { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (Pre_Read) {
				if (Values == null)
					Values = new AI_DsgVarValue[Pre_DsgVar.VariablesCount];
				if (Values.Length != Pre_DsgVar.VariablesCount) {
					var values = Values;
					System.Array.Resize(ref values, (int)Pre_DsgVar.VariablesCount);
					Values = values;
				}
				for (int i = 0; i < Values.Length; i++) {
					var info = Pre_DsgVar.Variables.Value[i];
					s.DoAt(Offset + info.OffsetInMemory, () => {
						Values[i] = s.SerializeObject<AI_DsgVarValue>(Values[i],
							onPreSerialize: v => v.Pre_LinkedType = info.LinkedType,
							name: $"{nameof(Values)}[{i}]");
					});
				}
			}
		}
	}
}
