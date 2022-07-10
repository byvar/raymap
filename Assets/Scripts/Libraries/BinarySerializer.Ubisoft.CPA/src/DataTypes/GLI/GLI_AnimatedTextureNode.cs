namespace BinarySerializer.Ubisoft.CPA {
	public class GLI_AnimatedTextureNode : BinarySerializable {
		public Pointer<GLI_Texture> Texture { get; set; }
		public float DisplayTime { get; set; }
		public Pointer<GLI_AnimatedTextureNode> NextDisplayNode { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Texture = s.SerializePointer<GLI_Texture>(Texture, name: nameof(Texture))?.ResolveObject(s);
			DisplayTime = s.Serialize<float>(DisplayTime, name: nameof(DisplayTime));
			NextDisplayNode = s.SerializePointer<GLI_AnimatedTextureNode>(NextDisplayNode, name: nameof(NextDisplayNode))?.ResolveObject(s);
		}
	}
}
