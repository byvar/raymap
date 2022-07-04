namespace BinarySerializer.Ubisoft.CPA {
	public class IPT_KeyAndPadDefineArray : BinarySerializable {
		public IPT_KeyAndPadDefine[] Keys { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Keys = s.SerializeObjectArrayUntil<IPT_KeyAndPadDefine>(Keys,
				k => k.IsInvalid, getLastObjFunc: () => new IPT_KeyAndPadDefine(),
				name: nameof(Keys));
		}
	}
}
