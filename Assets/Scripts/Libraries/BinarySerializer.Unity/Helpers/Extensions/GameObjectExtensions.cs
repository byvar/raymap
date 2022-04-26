using System;
using UnityEngine;

namespace BinarySerializer.Unity
{
	public static class GameObjectExtensions
	{
		public static T AddComponent<T>(this GameObject obj, Action<T> configureAction)
			where T : Component
		{
			T comp = obj.AddComponent<T>();
			configureAction(comp);
			return comp;
		}

		public static void AddBinarySerializableData(this GameObject obj, BinarySerializable data)
		{
			obj.AddComponent<BinarySerializableDataComponent>(x => x.Data = data);
		}
	}
}