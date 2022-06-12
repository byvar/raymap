using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class U64_ArrayReference<T> : BinarySerializable where T : U64_Struct, new() {
		public ushort Index { get; set; }
		public T[] Value { get; set; }

		public bool IsNull => Index == 0xFFFF;

		public override void SerializeImpl(SerializerObject s) {
			Index = s.Serialize<ushort>(Index, name: nameof(Index));
		}

		public U64_ArrayReference() {
			Index = 0xFFFF;
		}
		public U64_ArrayReference(Context c, ushort index) {
			Context = c;
			Index = index;
		}

		public U64_ArrayReference<T> Resolve(SerializerObject s, 
			long count,
			bool isInFixFixFat = false,
			Action<SerializerObject, T> onPreSerialize = null) {

			if(IsNull) return this;
			if (typeof(T) == typeof(U64_Placeholder)) {
				s.LogWarning("Trying to resolve Placeholder - skipping");
				return this;
			}

			var loader = s.GetLoader();
			ushort index = Index;
			if (isInFixFixFat) index = (ushort)BitHelpers.SetBits64(index, 1, 1, 15);

			var type = U64_StructType_Defines.GetType(typeof(T));
			var name = $"{type.Value}_{index:X4}";

			if (count == 0) {
				Value = s.SerializeObjectArray<T>(Value, 0, name: name);
				return this;
			}

			loader.RequestFile(s, type.Value, index, null, (ser, configureAction) => {
				Value = ser.SerializeObjectArray<T>(Value, count, onPreSerialize: (t, ind_in_array) => {
					configureAction(t, ind_in_array); onPreSerialize?.Invoke(ser, t);
				}, name: name);
			}, arrayCount: (uint)count, name: name);

			/*var ptr = loader.GetStructPointer(typeof(T), index, global: false);

			if (ptr != null) {
				s.DoAt(ptr, () => {
					Value = s.SerializeObjectArray<T>(Value, count, onPreSerialize: (t, ind_in_array) => {
						t.CPA_Index = index;
						t.CPA_ArrayIndex = ind_in_array;
						onPreSerialize?.Invoke(s,t);
					}, name: nameof(Value));
				});
			}*/
			return this;
		}


		public override bool UseShortLog => true;
		public override string ShortLog => $"{Index:X4}";
	}
}
