namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class CS_ActivationList : BinarySerializable
	{
		public uint ActivationZonesCount { get; set; }
		public Pointer ActivationZonesPointer { get; set; }

		// Serialized from pointers
		public CS_ActivationZone[] ActivationZones { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			ActivationZonesCount = s.Serialize<uint>(ActivationZonesCount, name: nameof(ActivationZonesCount));
			ActivationZonesPointer = s.SerializePointer(ActivationZonesPointer, name: nameof(ActivationZonesPointer));

			// Serialize data from pointers
			s.DoAt(ActivationZonesPointer, () => 
				ActivationZones = s.SerializeObjectArray<CS_ActivationZone>(ActivationZones, ActivationZonesCount, name: nameof(ActivationZones)));
		}
	}
}