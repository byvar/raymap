using System.Text;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class FON_StringLength : U64_Struct {
		public U64_String String { get; set; }

		public byte Length {
			get {
				if(String?.Value?.Value?.BinaryValue == null) return 0;
				return String.Value.Value.BinaryValue[0];
			}
		}

		public override void SerializeImpl(SerializerObject s) {
			String = s.SerializeObject<U64_String>(String, onPreSerialize: str => str.Pre_IsBinary = true, name: nameof(String));
		}
	}
}
