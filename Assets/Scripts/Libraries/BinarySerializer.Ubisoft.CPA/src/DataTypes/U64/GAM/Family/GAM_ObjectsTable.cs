using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_ObjectsTable : U64_Struct {
		public U64_ArrayReference<GAM_ObjectsTableEntry> ObjectsTableList { get; set; }
		public ushort PhysicalObjectsCount { get; set; }
		public ushort ZDxUsed { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ObjectsTableList = s.SerializeObject<U64_ArrayReference<GAM_ObjectsTableEntry>>(ObjectsTableList, name: nameof(ObjectsTableList));
			PhysicalObjectsCount = s.Serialize<ushort>(PhysicalObjectsCount, name: nameof(PhysicalObjectsCount));
			ZDxUsed = s.Serialize<ushort>(ZDxUsed, name: nameof(ZDxUsed));
			s.SerializePadding(2, logIfNotNull: true);

			ObjectsTableList?.Resolve(s, PhysicalObjectsCount);
		}
	}
}
