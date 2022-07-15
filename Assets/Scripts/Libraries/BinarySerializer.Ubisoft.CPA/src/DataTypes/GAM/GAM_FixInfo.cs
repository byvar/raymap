namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_FixInfo : BinarySerializable {
		public uint ObjectsInFixCount { get; set; }
		public GAM_ObjectInFixInfo[] ObjectsInFix { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ObjectsInFixCount = s.Serialize<uint>(ObjectsInFixCount, name: nameof(ObjectsInFixCount));
			ObjectsInFix = s.SerializeObjectArray<GAM_ObjectInFixInfo>(ObjectsInFix, ObjectsInFixCount, name: nameof(ObjectsInFix));

			foreach(var o in ObjectsInFix) o.Apply(s);
		}
	}
}
