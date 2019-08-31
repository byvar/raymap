using OpenSpace.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.ROM {
	public class Reference<T> where T : ROMStruct, new() {
		public ushort index;
		public T Value { get; set; }

		public Reference(Reader reader, bool resolve = false, Action<T> onPreRead = null) {
			index = reader.ReadUInt16();
			if (resolve) {
				Resolve(reader, onPreRead: onPreRead);
			}
		}

		public Reference(ushort index, Reader reader = null, bool resolve = false, Action<T> onPreRead = null) {
			this.index = index;
			if (resolve && reader != null) {
				Resolve(reader, onPreRead: onPreRead);
			}
		}

		public Reference<T> Resolve(Reader reader, Action<T> onPreRead = null) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			Value = l.GetOrRead(reader, index, onPreRead: onPreRead);
			return this;
		}
	}
}
