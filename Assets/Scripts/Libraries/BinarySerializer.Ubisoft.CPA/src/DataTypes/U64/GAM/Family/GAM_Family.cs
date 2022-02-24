using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_Family : U64_Struct {
		public U64_Reference<GAM_StateRef> StateRef { get; set; }
		public U64_Reference<GAM_ObjectsTable> ObjectsTable { get; set; }
		public U64_Reference<U64_BoundingVolume> BoundingVolume { get; set; }
		public ushort ChannelsCount { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			StateRef = s.SerializeObject<U64_Reference<GAM_StateRef>>(StateRef, name: nameof(StateRef))?.Resolve(s);
			ObjectsTable = s.SerializeObject<U64_Reference<GAM_ObjectsTable>>(ObjectsTable, name: nameof(ObjectsTable))?.Resolve(s);
			BoundingVolume = s.SerializeObject<U64_Reference<U64_BoundingVolume>>(BoundingVolume, name: nameof(BoundingVolume))?.Resolve(s);
			ChannelsCount = s.Serialize<ushort>(ChannelsCount, name: nameof(ChannelsCount));
		}
	}
}
