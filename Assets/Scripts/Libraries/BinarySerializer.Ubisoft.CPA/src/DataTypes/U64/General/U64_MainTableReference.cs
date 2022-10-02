using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class U64_MainTableReference<T> : BinarySerializable, ISerializerShortLog 
		where T : U64_Struct, new() {
		public ushort Index { get; set; }
		public T Value { get; set; }

		public bool IsNull => Index == 0xFFFF;

		public override void SerializeImpl(SerializerObject s) {
			Index = s.Serialize<ushort>(Index, name: nameof(Index));
		}

		public U64_MainTableReference() {
			Index = 0xFFFF;
		}
		public U64_MainTableReference(Context c, ushort index) {
			Context = c;
			Index = index;
		}

		public U64_MainTableReference<T> Resolve(SerializerObject s, bool isInFixFixFat = false,
			Action<SerializerObject, T> onPreSerialize = null) {
			//Action<SerializerObject, T> onPostSerialize = null) {

			if (IsNull) return this;

			var loader = s.GetLoader();
			ushort index = Index;
			if (isInFixFixFat) index = (ushort)BitHelpers.SetBits64(index, 1, 1, 15);
			var type = U64_StructType_Defines.GetType(typeof(T));
			var name = $"{type.Value}_{index:X4}";

			var mainTable_index = BitHelpers.SetBits64(index, 0, 1, 15);
			var ptr = loader.Data.MainTablesDictionary[type.Value].StructTable[mainTable_index];

			if (ptr != null) {
				s.DoAt(ptr, () => {
					Value = s.SerializeObject<T>(Value, onPreSerialize: t => {
						t.CPA_Index = index;
						onPreSerialize?.Invoke(s, t);
					}, name: $"{type}_{index:X4}");
					//onPostSerialize?.Invoke(s, Value);
				});
			}
			return this;
		}

		public U64_MainTableReference<T> ResolveAs<U>(SerializerObject s, bool isInFixFixFat = false,
			Action<SerializerObject, U> onPreSerialize = null) where U : T, new() {
			//Action<SerializerObject, T> onPostSerialize = null) {

			if (IsNull) return this;

			var loader = s.GetLoader();
			ushort index = Index;
			if (isInFixFixFat) index = (ushort)BitHelpers.SetBits64(index, 1, 1, 15);
			var type = U64_StructType_Defines.GetType(typeof(U));
			var name = $"{type.Value}_{index:X4}";

			var mainTable_index = BitHelpers.SetBits64(index, 0, 1, 15);
			var ptr = loader.Data.MainTablesDictionary[type.Value].StructTable[mainTable_index];

			if (ptr != null) {
				s.DoAt(ptr, () => {
					Value = s.SerializeObject<U>((U)Value, onPreSerialize: t => {
						t.CPA_Index = index;
						onPreSerialize?.Invoke(s, t);
					}, name: $"{type}_{index:X4}");
					//onPostSerialize?.Invoke(s, Value);
				});
			}
			return this;
		}


		public string ShortLog => $"{Index:X4}";
	}
}
