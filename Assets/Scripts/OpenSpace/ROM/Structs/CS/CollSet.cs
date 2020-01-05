using OpenSpace.Loader;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CollideType = OpenSpace.Collide.CollideType;

namespace OpenSpace.ROM {
	public class CollSet : ROMStruct {
		// Size: 20 = 0x14
		public Dictionary<CollideType, Reference<ZdxList>> zdxList = new Dictionary<CollideType, Reference<ZdxList>>();
		public Dictionary<CollideType, Reference<ActivationList>> activationList = new Dictionary<CollideType, Reference<ActivationList>>();
		public ushort word10; // 0x10
		public byte byte12;
		public byte byte13;

		protected override void ReadInternal(Reader reader) {
			zdxList[CollideType.ZDD] = new Reference<ZdxList>(reader, true);
			zdxList[CollideType.ZDE] = new Reference<ZdxList>(reader, true);
			zdxList[CollideType.ZDM] = new Reference<ZdxList>(reader, true);
			zdxList[CollideType.ZDR] = new Reference<ZdxList>(reader, true);
			activationList[CollideType.ZDD] = new Reference<ActivationList>(reader, true);
			activationList[CollideType.ZDE] = new Reference<ActivationList>(reader, true);
			activationList[CollideType.ZDM] = new Reference<ActivationList>(reader, true);
			activationList[CollideType.ZDR] = new Reference<ActivationList>(reader, true);
			word10 = reader.ReadUInt16();
			byte12 = reader.ReadByte();
			byte13 = reader.ReadByte();
		}
	}
}
