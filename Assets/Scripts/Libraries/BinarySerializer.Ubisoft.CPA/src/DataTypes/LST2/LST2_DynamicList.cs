﻿namespace BinarySerializer.Ubisoft.CPA
{
	public class LST2_DynamicList<T> : LST2_List<T> where T : BinarySerializable, LST2_IEntry<T>, new() {
		public LST2_DynamicList() : base(LST2_ListType.Dynamic) { }

		public new LST2_DynamicList<T> Resolve(SerializerObject s, string name = null) {
			base.Resolve(s, name: name);
			return this;
		}
	}
}