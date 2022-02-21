using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class U64_Reference<T> : BinarySerializable where T : U64_Struct, new() {
		public ushort Index { get; set; }
		public T Value { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Index = s.Serialize<ushort>(Index, name: nameof(Index));
		}

		public U64_Reference() { }
		public U64_Reference(Context c, ushort index) {
			Context = c;
			Index = index;
		}

		public U64_Reference<T> Resolve(SerializerObject s, bool isInFixFixFat = false,
			Action<SerializerObject, T> onPreSerialize = null,
			Action<SerializerObject, T> onPostSerialize = null) {

			var loader = s.GetLoader();
			ushort index = Index;
			if (isInFixFixFat) index = (ushort)BitHelpers.SetBits(index, 1, 1, 15);

			var ptr = loader.GetStructPointer(typeof(T), index, global: false);

			if (ptr != null) {
				s.DoAt(ptr, () => {
					Value = s.SerializeObject<T>(Value, onPreSerialize: t => {
						t.CPA_Index = index;
						onPreSerialize?.Invoke(s,t);
					}, name: nameof(Value));
					onPostSerialize?.Invoke(s, Value);
				});
			}
			return this;
		}


		public override bool UseShortLog => true;
		public override string ShortLog => $"{Index:X4}";
	}
}
