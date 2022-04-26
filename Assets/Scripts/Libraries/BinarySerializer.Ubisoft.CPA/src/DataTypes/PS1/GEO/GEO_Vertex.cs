namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class GEO_Vertex : BinarySerializable
	{
		public MTH3D_Vector_PS1_Short Position { get; set; }
		public RGB888Color Color { get; set; } // Range is 0-2

		public override void SerializeImpl(SerializerObject s)
		{
			Position = s.SerializeObject<MTH3D_Vector_PS1_Short>(Position, name: nameof(Position));
			s.SerializePadding(2, logIfNotNull: true);
			Color = s.SerializeObject<RGB888Color>(Color, name: nameof(Color));
			s.SerializePadding(1, logIfNotNull: true);
		}
	}
}