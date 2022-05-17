using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class HIE_SuperObject : U64_Struct {
		public POS_CompletePosition Matrix { get; set; }
		public U64_GenericReference LinkedObject { get; set; }
		public U64_ArrayReference<LST_ReferenceElement<HIE_SuperObject>> Children { get; set; }
		public U64_BoundingVolumeBox BoundingVolume { get; set; }
		public ushort ChildCount { get; set; }
		public ushort TransparencyLevel { get; set; }
		public HIE_SuperObjectFlags Flags { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			Matrix = s.SerializeObject<POS_CompletePosition>(Matrix, name: nameof(Matrix));
			LinkedObject = s.SerializeObject<U64_GenericReference>(LinkedObject, onPreSerialize: o => o.ImmediateSerializeType = U64_GenericReference.ImmediateSerialize.Index, name: nameof(LinkedObject));
			Children = s.SerializeObject<U64_ArrayReference<LST_ReferenceElement<HIE_SuperObject>>>(Children, name: nameof(Children));
			BoundingVolume = s.SerializeObject<U64_BoundingVolumeBox>(BoundingVolume, name: nameof(BoundingVolume));
			LinkedObject.SerializeType(s);
			ChildCount = s.Serialize<ushort>(ChildCount, name: nameof(ChildCount));
			TransparencyLevel = s.Serialize<ushort>(TransparencyLevel, name: nameof(TransparencyLevel));
			s.SerializePadding(2, logIfNotNull: true);
			Flags = s.Serialize<HIE_SuperObjectFlags>(Flags, name: nameof(Flags));

			LinkedObject?.Resolve(s);
			Children?.Resolve(s, ChildCount);
		}
	}

}
