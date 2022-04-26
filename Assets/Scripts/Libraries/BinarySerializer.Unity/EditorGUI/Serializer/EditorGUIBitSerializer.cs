﻿namespace BinarySerializer.Unity
{
	public class EditorGUIBitSerializer : BitSerializerObject
	{
		public EditorGUIBitSerializer(SerializerObject serializerObject, Pointer valueOffset)
			: base(serializerObject, valueOffset, null, 0) { }

		public override T SerializeBits<T>(T value, int length, string name = null)
		{
			T t = SerializerObject.Serialize<T>(value, name);
			Position += length;
			return t;
		}
	}
}