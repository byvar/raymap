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
		public bool forceFix;

		public Reference(Reader reader, bool resolve = false, Action<T> onPreRead = null, bool forceFix = false) {
			index = reader.ReadUInt16();
			this.forceFix = forceFix;
			if (resolve) {
				Resolve(reader, onPreRead: onPreRead);
			}
		}

		public Reference(ushort index, Reader reader = null, bool resolve = false, Action<T> onPreRead = null, bool forceFix = false) {
			this.index = index;
			this.forceFix = forceFix;
			if (resolve) {
				Resolve(reader, onPreRead: onPreRead);
			}
		}

		public Reference(ushort index, T value) {
			this.index = index;
			this.Value = value;
		}
		public Reference() {
			this.index = 0xFFFF;
			this.Value = null;
		}

		public Reference<T> Resolve(Reader reader, Action<T> onPreRead = null) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			ushort index = forceFix ? (ushort)(this.index | (ushort)FATEntry.Flag.Fix) : this.index;
			Value = l.GetOrRead(reader, index, onPreRead: onPreRead);
			return this;
		}

		public static implicit operator T(Reference<T> a) {
			return a?.Value;
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
		public bool forceFix;

		public GenericReference(Reader reader, bool resolve = false, bool forceFix = false) {
			index = reader.ReadUInt16();
			type = reader.ReadUInt16();
			this.forceFix = forceFix;
			if (resolve) {
				Resolve(reader);
			}
		}

		public GenericReference(ushort type, ushort index, Reader reader = null, bool resolve = false, bool forceFix = false) {
			this.type = type;
			this.index = index;
			this.forceFix = forceFix;
			if (resolve) {
				Resolve(reader);
			}
		}

		public Type Resolve(Reader reader) {
			if (this.type == 0xFFFF) return null;
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			FATEntry.Type entryType = FATEntry.GetEntryType(this.type);
			System.Type type = null;
			foreach (KeyValuePair<System.Type, FATEntry.Type> typePair in FATEntry.types) {
				if (typePair.Value == entryType) {
					type = typePair.Key;
					break;
				}
			}
			ushort index = forceFix ? (ushort)(this.index | (ushort)FATEntry.Flag.Fix) : this.index;
			switch (entryType) {
				case FATEntry.Type.GeometricElementTriangles:
					Value = l.GetOrRead<GeometricObjectElementTriangles>(reader, index);
					break;
				case FATEntry.Type.GeometricElementSprites:
					Value = l.GetOrRead<GeometricObjectElementSprites>(reader, index);
					break;
				case FATEntry.Type.VisualMaterial:
					Value = l.GetOrRead<VisualMaterial>(reader, index);
					break;
				case FATEntry.Type.GeometricElementCollideTriangles:
					Value = l.GetOrRead<GeometricObjectElementCollideTriangles>(reader, index);
					break;
				case FATEntry.Type.GeometricElementCollideAlignedBoxes:
					Value = l.GetOrRead<GeometricObjectElementCollideAlignedBoxes>(reader, index);
					break;
				case FATEntry.Type.GeometricElementCollideSpheres:
					Value = l.GetOrRead<GeometricObjectElementCollideSpheres>(reader, index);
					break;
				case FATEntry.Type.GameMaterial:
					Value = l.GetOrRead<GameMaterial>(reader, index);
					break;
				case FATEntry.Type.PhysicalObject:
					Value = l.GetOrRead<PhysicalObject>(reader, index);
					break;
				case FATEntry.Type.Sector:
					Value = l.GetOrRead<Sector>(reader, index);
					break;
				default:
					UnityEngine.Debug.LogWarning("GenericReference: Unsupported struct with type " + entryType + "(" + this.type + ")");
					break;
			}
			return type;
		}
	}
}
