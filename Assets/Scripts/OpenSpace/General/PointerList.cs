using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace {
	public class PointerList<T> : OpenSpaceStruct where T : OpenSpaceStruct, new() {
		public uint length;
		public Pointer<T>[] pointers;
		public Action<T> onPreRead = null;

		public T this[int index] { get => pointers[index]; set => pointers[index] = value; }

		public int Count => pointers.Length;

		public bool Contains(T item) {
			return pointers.Any(p => p.Value == item);
		}

		public IEnumerator<T> GetEnumerator() {
			return pointers.Select(p => p.Value).GetEnumerator();
		}

		protected override void ReadInternal(Reader reader) {
			pointers = new Pointer<T>[length];
			for(int i = 0; i < length; i++) {
				pointers[i] = new Pointer<T>(reader, true, onPreRead: onPreRead);
			}
		}
	}
}
