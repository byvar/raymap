using BinarySerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raymap {
	public abstract class Unity_Data {
		public Unity_Level Level { get; protected set; }
		public Context Context => Level.Context;

		protected object _LinkedObject { get; private set; }

		protected void Init(Unity_Level level, object linkedObject) {
			Level = level;
			_LinkedObject = linkedObject;
		}
	}
	public abstract class Unity_Data<T> : Unity_Data {
		public T LinkedObject => (T)_LinkedObject;

		public void Init(Unity_Level level, T linkedObject) {
			base.Init(level, linkedObject);
		}
	}
}
