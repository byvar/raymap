using System;
using System.Collections.Generic;
using UnityEditor;

namespace BinarySerializer.Unity.Editor {
	public class EditorGUISerializerConfig
	{
		// Singleton
		private static EditorGUISerializerConfig _Instance { get; set; }
		public static EditorGUISerializerConfig Instance {
			get {
				if (_Instance == null) _Instance = new EditorGUISerializerConfig();
				return _Instance;
			}
		}

		private readonly Dictionary<Type, SerializableObjectHandler> _handlers = new Dictionary<Type, SerializableObjectHandler>();

		public void AddObjectHandler<T>(SerializableObjectHandler<T> handler)
		{
			_handlers[typeof(T)] = (obj, name) => handler((T)obj, name);
		}

		public void AddObjectHandler<T, U>(Func<T, string, U> guiHandler, Action<T, U> applyAction)
		{
			_handlers[typeof(T)] = (obj, name) =>
			{
				EditorGUI.BeginChangeCheck();
				U value = guiHandler((T)obj, name);

				if (EditorGUI.EndChangeCheck())
					applyAction((T)obj, value);
			};
		}

		public SerializableObjectHandler GetHandler<T>() => 
			_handlers.TryGetValue(typeof(T), out SerializableObjectHandler handler) ? handler : null;

		public delegate void SerializableObjectHandler(object obj, string name);
		public delegate void SerializableObjectHandler<in T>(T obj, string name);
	}
}