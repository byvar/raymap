using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class LST_Ref<T> : U64_Struct where T : U64_Struct, new() {
		public U64_Reference<T> Entry { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Entry = s.SerializeObject<U64_Reference<T>>(Entry, name: nameof(Entry))?.Resolve(s);
		}
	}
}
