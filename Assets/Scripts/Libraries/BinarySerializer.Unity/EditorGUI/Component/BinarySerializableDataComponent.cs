using UnityEngine;

namespace BinarySerializer.Unity
{
	/// <summary>
	/// A component for allowing <see cref="BinarySerializable"/> data to be viewed and edited
	/// </summary>
	public class BinarySerializableDataComponent : MonoBehaviour
	{
		public BinarySerializable Data { get; set; }
	}
}