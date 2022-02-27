namespace BinarySerializer.Ubisoft.CPA.U64 {
    public class A3D_Vector : BinarySerializable {
        public int IntX { get; set; } // Divide by 4096 to get float
        public int IntY { get; set; }
        public int IntZ { get; set; }

        public CPA_Vector3D Vector { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            if (s.GetCPASettings().Platform == Platform.N64) {
                Vector = s.SerializeObject<CPA_Vector3D>(Vector, name: nameof(Vector));
            } else {
                IntX = s.Serialize<int>(IntX, name: nameof(IntX));
                IntY = s.Serialize<int>(IntY, name: nameof(IntY));
                IntZ = s.Serialize<int>(IntZ, name: nameof(IntZ));
            }
        }
    }
}