namespace BinarySerializer.Ubisoft.CPA {
	public class FIL_FileNameList : BinarySerializable {
		public LST2_DynamicList<FIL_FileNameListElement> FileNameList { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			FileNameList = s.SerializeObject<LST2_DynamicList<FIL_FileNameListElement>>(FileNameList, name: nameof(FileNameList))
				?.Resolve(s, name: nameof(FileNameList));
		}
	}
}
