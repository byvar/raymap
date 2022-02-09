using BinarySerializer;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raymap {
	public class LegacyGameManager : BaseGameManager {
		public override MapTreeNode[] GetLevels(MapViewerSettings settings) {
			throw new NotImplementedException();
		}

		public override UniTask<Unity_Level> LoadAsync(Context context) {
			throw new NotImplementedException();
		}
	}
}
