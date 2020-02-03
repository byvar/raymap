using OpenSpace.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.ROM {
	public abstract class ROMStruct {
		public Pointer Offset { get; protected set; }
		public ushort Index   { get; protected set; }
		public string IndexString { get { return string.Format("{0:X4}", Index); } }

		public void Init(Pointer offset, ushort index) {
			this.Offset = offset;
			this.Index = index;
		}
		protected abstract void ReadInternal(Reader reader);
		public void Read(Reader reader) {
			Pointer.DoAt(ref reader, Offset, () => {
				ReadInternal(reader);
			});
		}

		public static R2ROMLoader Loader {
			get {
				return MapLoader.Loader as R2ROMLoader;
			}
		}
	}
}
