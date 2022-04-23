﻿namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class GEO_Vertex : BinarySerializable
	{
		public MTH3D_Vector_PS1_Short Position { get; set; }
		public ushort Ushort_06 { get; set; }
		public RGB888Color Color { get; set; } // Range is 0-2

		public override void SerializeImpl(SerializerObject s)
		{
			Position = s.SerializeObject<MTH3D_Vector_PS1_Short>(Position, name: nameof(Position));
			Ushort_06 = s.Serialize<ushort>(Ushort_06, name: nameof(Ushort_06));
			Color = s.SerializeObject<RGB888Color>(Color, name: nameof(Color));
			s.SerializePadding(1, logIfNotNull: true);
		}
	}
}