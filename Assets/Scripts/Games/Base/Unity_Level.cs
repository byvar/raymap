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

		protected Dictionary<object, Unity_Data> UnityDataDictionary { get; set; } = new Dictionary<object, Unity_Data>();
		public T GetUnityData<T,U>(U obj) where T : Unity_Data<U>, new() {
			if (!UnityDataDictionary.ContainsKey(obj)) {
				var data = new T();
				data.Init(this, obj);
				UnityDataDictionary[obj] = data;
			}
			return (T)UnityDataDictionary[obj];
		}
	}
}
