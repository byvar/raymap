using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace BinarySerializer.Unity
{
	// TODO: Rather than using the name to keep track of the value states we could use an index. That way resolved generic pointers will work.
	public class EditorGUISerializer : SerializerObject
	{
		#region Constructor

		public EditorGUISerializer(Context context) : base(context)
		{
			Foldouts = new Dictionary<string, bool>();
			CurrentName = new Stack<string>();
		}

		#endregion

		#region Private Properties

		private Dictionary<string, bool> Foldouts { get; }
		private Stack<string> CurrentName { get; }
		private string GetFullName(string name) => String.Join(".", CurrentName.Append(name));

		#endregion

		#region Public Properties

		public override bool FullSerialize => false;
		public override long CurrentLength => 0;
		public override BinaryFile CurrentBinaryFile => null;
		public override long CurrentFileOffset => 0;
		public override long CurrentAbsoluteOffset => 0;

		#endregion

		#region Private Editor Helpers

		private void DoFoldout(string name, Action action)
		{
			CurrentName.Push(name);

			string fullName = GetFullName(name);

			if (!Foldouts.ContainsKey(fullName))
				Foldouts[fullName] = false;

			Foldouts[fullName] = EditorGUILayout.Foldout(Foldouts[fullName], name ?? DefaultName, true);

			if (Foldouts[fullName])
			{
				Depth++;
				EditorGUI.indentLevel++;

				action();

				Depth--;
				EditorGUI.indentLevel--;
			}

			CurrentName.Pop();
		}

		#endregion

		#region Logging

		public override void Log(string logString, params object[] args)
		{
			//EditorGUILayout.LabelField(logString);
		}

		#endregion

		#region Encoding

		public override void DoEncoded(IStreamEncoder encoder, Action action, Endian? endianness = null, bool allowLocalPointers = false, string filename = null)
		{
			action();
		}

		public override Pointer BeginEncoded(IStreamEncoder encoder, Endian? endianness = null, bool allowLocalPointers = false, string filename = null)
		{
			return null;
		}

		public override void EndEncoded(Pointer endPointer) { }

		#endregion

		#region Positioning

		public override void Goto(Pointer offset) { }

		public override void DoAt(Pointer offset, Action action)
		{
			if (offset == null)
				return;

			DoFoldout(offset.ToString(), action);
		}

		#endregion

		#region Checksum

		public override T SerializeChecksum<T>(T calculatedChecksum, string name = null) => default;

		#endregion

		#region Serialization

		public override T Serialize<T>(T obj, string name = null)
		{
			name ??= DefaultName;

			// Get the type
			Type type = typeof(T);
			TypeCode typeCode = Type.GetTypeCode(type);

			if (type.IsEnum)
			{
				if (type.GetCustomAttributes(false).OfType<FlagsAttribute>().Any())
					return (T)(object)EditorGUILayout.EnumFlagsField(name, (Enum)(object)obj);
				else
					return (T)(object)EditorGUILayout.EnumPopup(name, (Enum)(object)obj);
			}

			switch (typeCode)
			{
				case TypeCode.Boolean:
					return (T)(object)EditorGUILayout.Toggle(name, (bool)(object)obj);

				case TypeCode.SByte:
					return (T)(object)(sbyte)EditorGUILayout.IntField(name, (sbyte)(object)obj);

				case TypeCode.Byte:
					return (T)(object)(byte)EditorGUILayout.IntField(name, (byte)(object)obj);

				case TypeCode.Int16:
					return (T)(object)(short)EditorGUILayout.IntField(name, (short)(object)obj);

				case TypeCode.UInt16:
					return (T)(object)(ushort)EditorGUILayout.IntField(name, (ushort)(object)obj);

				case TypeCode.Int32:
					return (T)(object)EditorGUILayout.IntField(name, (int)(object)obj);

				case TypeCode.UInt32:
					return (T)(object)(uint)EditorGUILayout.LongField(name, (uint)(object)obj);

				case TypeCode.Int64:
					return (T)(object)EditorGUILayout.LongField(name, (long)(object)obj);

				case TypeCode.UInt64:
					return (T)(object)(ulong)EditorGUILayout.LongField(name, (long)(ulong)(object)obj);

				case TypeCode.Single:
					return (T)(object)EditorGUILayout.FloatField(name, (float)(object)obj);

				case TypeCode.Double:
					return (T)(object)EditorGUILayout.DoubleField(name, (double)(object)obj);

				case TypeCode.String:
					return (T)(object)EditorGUILayout.TextField(name, (string)(object)obj);

				case TypeCode.Decimal:
				case TypeCode.Char:
				case TypeCode.DateTime:
				case TypeCode.Empty:
				case TypeCode.DBNull:

				case TypeCode.Object:
					if (type == typeof(UInt24))
					{
						return (T)(object)(UInt24)(uint)EditorGUILayout.LongField(name ?? DefaultName, (uint)(UInt24)(object)obj);
					}
					else if (type == typeof(byte?))
					{
						var b = (byte?)(object)obj;
						byte byteValue;
						bool hasValue = b.HasValue;

						hasValue = EditorGUILayout.Toggle(name, hasValue);

						if (hasValue)
							byteValue = (byte)EditorGUILayout.IntField(name, b ?? 0);
						else
							byteValue = 0;

						if (hasValue)
							return (T)(object)(byte?)byteValue;

						return (T)(object)null;
					}
					else
					{
						throw new NotSupportedException($"The generic type for ('{name}') is not supported");
					}
				default:
					throw new NotSupportedException($"The generic type for ('{name}') is not supported");
			}

		}

		public override T SerializeObject<T>(T obj, Action<T> onPreSerialize = null, string name = null)
		{
			if (obj == null)
				return null;

            name ??= DefaultName;

			// BaseColor is defined in the BinarySerializer base library, so we can add a custom check for that
			if (obj is BaseColor color)
			{
				EditorGUI.BeginChangeCheck();
				Color unityColor = EditorGUILayout.ColorField(name, color.GetColor());

				if (EditorGUI.EndChangeCheck())
				{
					color.Alpha = unityColor.a;
					color.Red = unityColor.r;
					color.Green = unityColor.g;
					color.Blue = unityColor.b;
				}
			}
			else
			{
				EditorGUISerializerConfig config = Context.GetEditorGUISerializerConfig();
				EditorGUISerializerConfig.SerializableObjectHandler handler = config?.GetHandler<T>();

				if (handler != null)
					handler(obj, name);
				else
					DoFoldout(name, () => obj.Serialize(this));
			}

			return obj;
		}

		public override Pointer SerializePointer(Pointer obj, PointerSize size = PointerSize.Pointer32, Pointer anchor = null, bool allowInvalid = false, long? nullValue = null, string name = null)
		{
            name ??= DefaultName;

			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.TextField(name, $"{obj}");
			EditorGUI.EndDisabledGroup();

			return obj;
		}

		public override Pointer<T> SerializePointer<T>(Pointer<T> obj, PointerSize size = PointerSize.Pointer32, Pointer anchor = null,
			bool allowInvalid = false, long? nullValue = null, string name = null)
		{
			name ??= DefaultName;

			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.TextField(name, $"{obj}");
			EditorGUI.EndDisabledGroup();

			return obj;
		}
		
		public override string SerializeString(string obj, long? length = null, Encoding encoding = null, string name = null)
		{
			name ??= DefaultName;
			return EditorGUILayout.TextField(name, obj);
		}

		#endregion

		#region Array Serialization

		public override T[] SerializeArraySize<T, U>(T[] obj, string name = null)
		{
			name ??= DefaultName;

			U size = Serialize<U>(default, name: $"{name}.Length");

			// Convert size to int, slow
			int intSize = (int)Convert.ChangeType(size, typeof(int));

			if (obj == null)
				obj = new T[intSize];
			else if (obj.Length != intSize)
				Array.Resize(ref obj, intSize);

			return obj;
		}

		public override T[] SerializeArray<T>(T[] obj, long count, string name = null)
		{
			name ??= DefaultName;

			if (obj == null) 
				obj = new T[count];

			else if (count != obj.Length) 
				Array.Resize(ref obj, (int)count);

			DoFoldout($"{name}[{obj.Length}]", () =>
			{
				for (int i = 0; i < obj.Length; i++)
					Serialize(obj[i], name: $"{name}[{i}]");
			});

			return obj;
		}

		public override T[] SerializeObjectArray<T>(T[] obj, long count, Action<T, int> onPreSerialize = null, string name = null)
		{
			name ??= DefaultName;

			if (obj == null)
				obj = new T[count];
			else if (count != obj.Length) 
				Array.Resize(ref obj, (int)count);

			DoFoldout($"{name}[{obj.Length}]", () =>
			{
				for (int i = 0; i < obj.Length; i++)
					SerializeObject(obj[i], name: $"{name}[{i}]");
			});

			return obj;
		}

		public override T[] SerializeArrayUntil<T>(T[] obj, Func<T, bool> conditionCheckFunc, Func<T> getLastObjFunc = null, string name = null)
		{
			return SerializeArray<T>(obj, obj.Length, name: name);
		}

		public override T[] SerializeObjectArrayUntil<T>(T[] obj, Func<T, bool> conditionCheckFunc, Func<T> getLastObjFunc = null, Action<T, int> onPreSerialize = null, string name = null)
		{
			return SerializeObjectArray<T>(obj, obj.Length, onPreSerialize: onPreSerialize, name: name);
		}

		public override Pointer[] SerializePointerArray(Pointer[] obj, long count, PointerSize size = PointerSize.Pointer32, Pointer anchor = null, bool allowInvalid = false, long? nullValue = null,
			string name = null)
		{
			name ??= DefaultName;

			if (obj == null)
				obj = new Pointer[count];
			else if (count != obj.Length) 
				Array.Resize(ref obj, (int)count);

			DoFoldout($"{name}[{obj.Length}]", () =>
			{
				for (int i = 0; i < obj.Length; i++)
					SerializePointer(obj[i], size: size, anchor: anchor, allowInvalid: allowInvalid, nullValue: nullValue, name: $"{name}[{i}]");
			});

			return obj;
		}

		public override Pointer<T>[] SerializePointerArray<T>(Pointer<T>[] obj, long count, PointerSize size = PointerSize.Pointer32, Pointer anchor = null,
			bool allowInvalid = false, long? nullValue = null, string name = null)
		{
			name ??= DefaultName;

			if (obj == null) 
				obj = new Pointer<T>[count];
			else if (count != obj.Length) 
				Array.Resize(ref obj, (int)count);

			DoFoldout($"{name}[{obj.Length}]", () =>
			{
				for (int i = 0; i < obj.Length; i++)
					SerializePointer(obj[i], size: size, anchor: anchor, allowInvalid: allowInvalid, nullValue: nullValue, name: $"{name}[{i}]");
			});

			return obj;
		}

		public override string[] SerializeStringArray(string[] obj, long count, int length, Encoding encoding = null, string name = null)
		{
			name ??= DefaultName;

			if (obj == null) 
				obj = new string[count];
			else if (count != obj.Length) 
				Array.Resize(ref obj, (int)count);

			DoFoldout($"{name}[{obj.Length}]", () =>
			{
				for (int i = 0; i < obj.Length; i++)
					SerializeString(obj[i], name: $"{name}[{i}]");
			});

			return obj;
		}

		#endregion

		#region Other Serialization

		public override void DoEndian(Endian endianness, Action action)
		{
			action();
		}

		public override void SerializeBitValues(Action<SerializeBits64> serializeFunc)
		{
			serializeFunc((value, length, name) => Serialize<long>(value, name: name));
		}

		public override void DoBits<T>(Action<BitSerializerObject> serializeFunc)
		{
			serializeFunc(new EditorGUIBitSerializer(this, CurrentPointer));
		}

		#endregion
	}
}