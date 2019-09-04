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

		public Reference(ushort index, T value) {
			this.index = index;
			this.Value = value;
		}

		public Reference<T> Resolve(Reader reader, Action<T> onPreRead = null) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			Value = l.GetOrRead(reader, index, onPreRead: onPreRead);
			return this;
		}

		public static implicit operator T(Reference<T> a) {
			return a.Value;
		}
		public static implicit operator Reference<T>(T t) {
			if (t == null) {
				return new Reference<T>(0xFFFF, null);
			} else {
				return new Reference<T>(t.Index, t);
			}
		}
	}


	public class GenericReference {
		public ushort index;
		public ushort type;
		public ROMStruct Value;

		public GenericReference(Reader reader, bool resolve = false) {
			index = reader.ReadUInt16();
			type = reader.ReadUInt16();
			if (resolve) {
				Resolve(reader);
			}
		}

		public GenericReference(ushort type, ushort index, Reader reader = null, bool resolve = false) {
			this.type = type;
			this.index = index;
			if (resolve && reader != null) {
				Resolve(reader);
			}
		}

		public Type Resolve(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			FATEntry.Type entryType = FATEntry.GetEntryType(this.type);
			System.Type type = null;
			foreach (KeyValuePair<System.Type, FATEntry.Type> typePair in FATEntry.types) {
				if (typePair.Value == entryType) {
					type = typePair.Key;
					break;
				}
			}
			switch (entryType) {
				case FATEntry.Type.MeshElement:
					Value = l.GetOrRead<MeshElement>(reader, index);
					break;
			}
			return type;
		}
	}
}
