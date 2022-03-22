namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class CS_ActivationZone : BinarySerializable
	{
		public uint ActivationsCount { get; set; }
		public Pointer ActivationsPointer { get; set; }

		// Serialized from pointers
		public uint[] Activations { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			ActivationsCount = s.Serialize<uint>(ActivationsCount, name: nameof(ActivationsCount));
			ActivationsPointer = s.SerializePointer(ActivationsPointer, name: nameof(ActivationsPointer));

			// Serialize data from pointers
			s.DoAt(ActivationsPointer, () => 
				Activations = s.SerializeArray<uint>(Activations, ActivationsCount, name: nameof(Activations)));
		}
	}
}