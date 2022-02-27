namespace BinarySerializer.Ubisoft.CPA.U64 {
    public class GLI_TextureListEntry : U64_Struct {
        public U64_Reference<GLI_Texture> Texture { get; set; }
        public ushort Time { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            Texture = s.SerializeObject<U64_Reference<GLI_Texture>>(Texture, name: nameof(Texture))?.Resolve(s);
            Time = s.Serialize<ushort>(Time, name: nameof(Time));
        }
    }

}
