using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class U64_Reference<T> : BinarySerializable where T : U64_Struct, new() {
		public ushort Index { get; set; }
		public T Value { get; set; }

		public bool IsNull => Index == 0xFFFF;

		public override void SerializeImpl(SerializerObject s) {
			Index = s.Serialize<ushort>(Index, name: nameof(Index));
		}

		public U64_Reference() {
			Index = 0xFFFF;
		}
		public U64_Reference(Context c, ushort index) {
			Context = c;
			Index = index;
		}

		public U64_Reference<T> Resolve(SerializerObject s, bool isInFixFixFat = false,
			Action<SerializerObject, T> onPreSerialize = null) {
			//Action<SerializerObject, T> onPostSerialize = null) {

			if (IsNull) return this;

			var loader = s.GetLoader();
			ushort index = Index;
			if (isInFixFixFat) index = (ushort)BitHelpers.SetBits64(index, 1, 1, 15);
			var type = U64_StructType_Defines.GetType(typeof(T));
			var name = $"{type.Value}_{index:X4}";

			loader.RequestFile(s, type.Value, index, (ser, configureAction) => {
				Value = ser.SerializeObject<T>(Value, onPreSerialize: t => {
					configureAction(t); onPreSerialize?.Invoke(ser, t);
				}, name: name);
			}, null, name: name);

			/*var ptr = loader.GetStructPointer(typeof(T), index, global: false);

			if (ptr != null) {
				var type = U64_StructType_Defines.TypeMapping[typeof(T)];
				s.DoAt(ptr, () => {
					Value = s.SerializeObject<T>(Value, onPreSerialize: t => {
						t.CPA_Index = index;
						onPreSerialize?.Invoke(s,t);
					}, name: $"{type}_{index:X4}");
					//onPostSerialize?.Invoke(s, Value);
				});
			}*/
			return this;
		}


		public override bool UseShortLog => true;
		public override string ShortLog => $"{Index:X4}";
	}
}
