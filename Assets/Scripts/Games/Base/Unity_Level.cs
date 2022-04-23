using BinarySerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raymap {
	public abstract class Unity_Level {
		public Context Context { get; protected set; }

		public abstract string EnvironmentKey { get; }
		public abstract void Init();

		public void Register(Context context) {
			Context = context;
			Context.StoreObject<Unity_Level>(ContextKey, this);
		}

		public static string ContextKey => nameof(Unity_Level);
	}
}
