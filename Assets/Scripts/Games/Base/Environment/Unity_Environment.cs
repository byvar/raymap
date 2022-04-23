using BinarySerializer;
using UnityEngine;

namespace Raymap {
	public abstract class Unity_Environment : MonoBehaviour {
		public Context Context { get; private set; }

		public abstract void Init();
		public abstract void OnDataLoaded();


		public void Register(Context context) {
			Context = context;
			Context.StoreObject<Unity_Environment>(ContextKey, this);
		}

		public static string ContextKey => nameof(Unity_Environment);
	}
}
