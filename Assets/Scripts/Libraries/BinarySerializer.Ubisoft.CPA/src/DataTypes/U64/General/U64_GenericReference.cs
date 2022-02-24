using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class U64_GenericReference : BinarySerializable {
		public ushort Index { get; set; }
		public ushort Type { get; set; }

		/// <summary>
		/// Some GenericReferences aren't structured like (Index,Type) one of them has to be serialized differently.
		/// Use these flags to control what is serialized first, then use the SerializeIndex/Type functions later.
		/// </summary>
		public ImmediateSerialize ImmediateSerializeType { get; set; } = ImmediateSerialize.All;

		public U64_Struct Value { get; set; }

		public bool IsNull => Index == 0xFFFF;

		public override void SerializeImpl(SerializerObject s) {
			if (ImmediateSerializeType.HasFlag(ImmediateSerialize.Index)) SerializeIndex(s);
			if (ImmediateSerializeType.HasFlag(ImmediateSerialize.Type)) SerializeType(s);
		}

		public void SerializeIndex(SerializerObject s) {
			Index = s.Serialize<ushort>(Index, name: nameof(Index));
		}
		public void SerializeType(SerializerObject s) {
			Type = s.Serialize<ushort>(Type, name: nameof(Type));
		}

		public U64_GenericReference() {
			Index = 0xFFFF;
			Type = 0xFFFF;
		}
		public U64_GenericReference(Context c, ushort index, ushort type) {
			Context = c;
			Index = index;
			Type = type;
		}

		[Flags]
		public enum ImmediateSerialize {
			None = 0,
			Index = 1 << 0,
			Type = 1 << 1,
			All = Index | Type,
		}

		public U64_GenericReference Resolve(SerializerObject s, bool isInFixFixFat = false,
			Action<SerializerObject, U64_Struct> onPreSerialize = null) {
			//Action<SerializerObject, T> onPostSerialize = null) {

			if (IsNull) return this;

			var loader = s.GetLoader();
			ushort index = Index;
			if (isInFixFixFat) index = (ushort)BitHelpers.SetBits(index, 1, 1, 15);
			var type = U64_StructType_Defines.GetType(s.Context, Type);
			if(!type.HasValue)
				throw new BinarySerializableException(this, $"Cannot resolve type index {Type}");
			var name = $"{type}_{index:X4}";

			loader.RequestFile(s, type.Value, index, (ser, configureAction) => {
				SerializeValue(ser, type.Value, onPreSerialize: t => {
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

		private void SerializeValue(SerializerObject s, U64_StructType type, Action<U64_Struct> onPreSerialize = null, string name = null) {
			U64_Struct Serialize<T>() where T : U64_Struct, new() => s.SerializeObject<T>((T)Value, onPreSerialize: onPreSerialize, name: name);
			Value = type switch {
				U64_StructType.IdCardBase => Serialize<MEC_IdCardBase>(),
				U64_StructType.IdCardCamera => Serialize<MEC_IdCardCamera>(),
				_ => throw new BinarySerializableException(this, $"Type {type} is not implemented in {GetType()}")
			};
		}


		public override bool UseShortLog => true;
		public override string ShortLog => $"{Index:X4}";
	}
}
