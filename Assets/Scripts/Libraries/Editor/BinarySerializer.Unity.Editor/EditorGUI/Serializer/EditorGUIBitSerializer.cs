using System;

namespace BinarySerializer.Unity.Editor
{
	public class EditorGUIBitSerializer : BitSerializerObject {
		public new EditorGUISerializer SerializerObject => (EditorGUISerializer)base.SerializerObject;

		public EditorGUIBitSerializer(SerializerObject serializerObject, Pointer valueOffset)
			: base(serializerObject, valueOffset, null, 0) { }

		public override T SerializeBits<T>(T value, int length, SignedNumberRepresentation sign = SignedNumberRepresentation.Unsigned, string name = null)
		{
			T t = SerializerObject.Serialize<T>(value, name);
			Position += length;
			return t;
		}

		public override T? SerializeNullableBits<T>(T? value, int length, string name = null) {
			T? t = SerializerObject.SerializeNullable<T>(value, name);
			Position += length;
			return t;
		}

		public override T SerializeObject<T>(T obj, Action<T> onPreSerialize = null, string name = null) 
		{
			SerializerObject.DoFoldout(name, () => {
				if (obj == null) obj = new T();
				obj.SerializeImpl(this);
			});

			return obj;
		}
	}
}