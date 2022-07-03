using System;
using System.Collections.Generic;

namespace BinarySerializer.Ubisoft.CPA {
	public abstract class CPA_Globals {
		public Context Context { get; set; }
		public string Map { get; set; }
		public abstract string GameDataDirectory { get; }
		public abstract string LevelsDirectory { get; }

		public CPA_Globals(Context c) {
			Context = c;
			c.StoreObject<CPA_Globals>(ContextKey, this);
		}

		public static string ContextKey => nameof(CPA_Globals);

		public abstract Dictionary<CPA_Path, string> GetPaths(string levelName);
	}
}
