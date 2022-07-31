namespace BinarySerializer.Ubisoft.CPA {
	public class PO_PhysicalCollideSet : BinarySerializable {
		public Pointer<COL_CollideObject> ZDM { get; set; }
		public Pointer<COL_CollideObject> ZDD { get; set; }
		public Pointer<COL_CollideObject> ZDE { get; set; }
		public Pointer<COL_CollideObject> ZDR { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ZDM = s.SerializePointer<COL_CollideObject>(ZDM, name: nameof(ZDM))?.ResolveObject(s);
			ZDD = s.SerializePointer<COL_CollideObject>(ZDD, name: nameof(ZDD))?.ResolveObject(s);
			ZDE = s.SerializePointer<COL_CollideObject>(ZDE, name: nameof(ZDE))?.ResolveObject(s);
			ZDR = s.SerializePointer<COL_CollideObject>(ZDR, name: nameof(ZDR))?.ResolveObject(s);
		}
	}
}
