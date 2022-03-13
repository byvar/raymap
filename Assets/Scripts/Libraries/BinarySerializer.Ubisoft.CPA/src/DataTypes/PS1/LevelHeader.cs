namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class LevelHeader : BinarySerializable
	{
		public byte[] UnknownBytes1 { get; set; }
		public uint GeometricObjectsDynamicCount_Cine { get; set; }
		public byte[] UnknownBytes2 { get; set; }
		public Pointer DynamicWorldPointer { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			CPA_Settings settings = s.GetCPASettings();

			if (settings.Mode == CPA_GameMode.RaymanRushPS1)
			{
				UnknownBytes1 = s.SerializeArray<byte>(UnknownBytes1, 0x40, name: nameof(UnknownBytes1));
			}
			else if (settings.Mode == CPA_GameMode.DonaldDuckPS1)
			{
				UnknownBytes1 = s.SerializeArray<byte>(UnknownBytes1, 0x58, name: nameof(UnknownBytes1));
			}
			else if (settings.Mode == CPA_GameMode.VIPPS1)
			{
				UnknownBytes1 = s.SerializeArray<byte>(UnknownBytes1, 0x28, name: nameof(UnknownBytes1));
			}
			else if (settings.Mode == CPA_GameMode.JungleBookPS1)
			{
				UnknownBytes1 = s.SerializeArray<byte>(UnknownBytes1, 0xEC, name: nameof(UnknownBytes1));
			}
			else
			{
				UnknownBytes1 = s.SerializeArray<byte>(UnknownBytes1, 0xCC, name: nameof(UnknownBytes1));
				GeometricObjectsDynamicCount_Cine = s.Serialize<uint>(GeometricObjectsDynamicCount_Cine, name: nameof(GeometricObjectsDynamicCount_Cine));
				UnknownBytes2 = s.SerializeArray<byte>(UnknownBytes2, 0x20, name: nameof(UnknownBytes2));
			}

			DynamicWorldPointer = s.SerializePointer(DynamicWorldPointer, name: nameof(DynamicWorldPointer));
		}
	}
}