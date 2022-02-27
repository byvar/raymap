namespace BinarySerializer.Ubisoft.CPA.U64 {
    public class GLI_Texture : U64_Struct {
        public U64_Reference<GLI_BitmapInfo> BitmapInfo { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            BitmapInfo = s.SerializeObject<U64_Reference<GLI_BitmapInfo>>(BitmapInfo, name: nameof(BitmapInfo))?.Resolve(s);
        }
    }

}
