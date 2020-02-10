using OpenSpace.Loader;
using System.Linq;
using UnityEngine;
using DsgVarType = OpenSpace.AI.DsgVarInfoEntry.DsgVarType;

namespace OpenSpace.ROM {
	public class DsgVarValue {
		public ushort index_of_info;
		public ushort type;
		public ushort param;

		// Custom
		public DsgVarType dsgVarType;
		public Pointer offset;
		public int index;

		// Parsed param
		public sbyte paramByte;
		public byte paramUByte;
		public short paramShort;
		public Reference<DsgVarEntry> paramEntry;
		public Reference<Dsg_Int> paramInt;
		public Reference<Dsg_UInt> paramUInt;
		public Reference<Dsg_Real> paramFloat;
		public Reference<Dsg_Vector3> paramVector3;
		public Reference<WayPoint> paramWaypoint;
		public Reference<Graph> paramGraph;
		public Reference<Comport> paramComport;
		public Reference<State> paramState;
		public Reference<GameMaterial> paramGameMaterial;
		public Reference<Perso> paramPerso;
		public AI.DsgVarValue.List paramList;
		public Reference<Dsg_UInt> paramCaps;

		public DsgVarValue(ushort index_of_info, ushort type) {
			this.index_of_info = index_of_info;
			this.type = type;
		}
		public void Read(Reader reader) {
			offset = Pointer.Current(reader);
			param = reader.ReadUInt16();

			// Read different types of param
			Pointer.Goto(ref reader, offset); paramByte = reader.ReadSByte();
			Pointer.Goto(ref reader, offset); paramUByte = reader.ReadByte();
			Pointer.Goto(ref reader, offset); paramShort = reader.ReadInt16();

			Parse(reader);
		}

		private void Parse(Reader reader) {
			ushort typeNumber = type;
			ushort usedParam = param;
			dsgVarType = DsgVarType.None;
			index = index_of_info;
			if ((type & ((ushort)FATEntry.Flag.Fix)) == (ushort)FATEntry.Flag.Fix) {
				paramEntry = new Reference<DsgVarEntry>(param, reader, true);
				typeNumber = (ushort)(typeNumber & 0x7FFF);
				index = paramEntry.Value.index_of_entry;
				usedParam = paramEntry.Value.param;
			}
			if (Settings.s.aiTypes != null) dsgVarType = Settings.s.aiTypes.GetDsgVarType(typeNumber);
			if (paramEntry == null) {
				// If paramEntry exists, all params <= 4 bytes, so until float, should be read from paramEntry.
				// eg. instead of using paramUInt, you use paramEntry.Value.paramUInt
				// If paramEntry doesn't exist, all params <= 2 bytes should be read from the dsgvar info
				switch (dsgVarType) {
					case DsgVarType.UInt:
						paramUInt = new Reference<Dsg_UInt>(usedParam, reader, true);
						break;
					case DsgVarType.Int:
						paramInt = new Reference<Dsg_Int>(usedParam, reader, true);
						break;
					case DsgVarType.Float:
						paramFloat = new Reference<Dsg_Real>(usedParam, reader, true);
						break;
				}
			}
			switch (dsgVarType) {
				case DsgVarType.Vector:
					paramVector3 = new Reference<Dsg_Vector3>(usedParam, reader, true);
					break;
				case DsgVarType.Graph:
					paramGraph = new Reference<Graph>(usedParam, reader, true);
					break;
				case DsgVarType.WayPoint:
					paramWaypoint = new Reference<WayPoint>(usedParam, reader, true);
					break;
				case DsgVarType.Comport:
					paramComport = new Reference<Comport>(usedParam, reader, true);
					break;
				case DsgVarType.Action:
					paramState = new Reference<State>(usedParam, reader, true);
					break;
				case DsgVarType.GameMaterial:
					paramGameMaterial = new Reference<GameMaterial>(usedParam, reader, true);
					break;
				case DsgVarType.Perso:
					paramPerso = new Reference<Perso>(usedParam, reader, true);
					break;
				case DsgVarType.List:
					paramList = new AI.DsgVarValue.List();
					paramList.curLength = 0;
					paramList.maxLength = (byte)param;
					paramList.list = new AI.DsgVarValue.List.Entry[paramList.maxLength];
					break;
				case DsgVarType.Caps:
					// Always uses the default param in the info
					paramCaps = new Reference<Dsg_UInt>(param, reader, true);
					break;
			}
		}

		// Getters for different types
		public sbyte ValueByte {
			get {
				if (paramEntry == null) {
					return paramByte;
				} else {
					return paramEntry.Value.paramByte;
				}
			}
		}
		public byte ValueUByte {
			get {
				if (paramEntry == null) {
					return paramUByte;
				} else {
					return paramEntry.Value.paramUByte;
				}
			}
		}
		public short ValueShort {
			get {
				if (paramEntry == null) {
					return paramShort;
				} else {
					return paramEntry.Value.paramShort;
				}
			}
		}
		public ushort ValueUShort {
			get {
				if (paramEntry == null) {
					return param;
				} else {
					return paramEntry.Value.param;
				}
			}
		}
		public int ValueInt {
			get {
				if (paramEntry == null) {
					return paramInt.Value?.value ?? 0;
				} else {
					return paramEntry.Value.paramInt;
				}
			}
		}
		public uint ValueUInt {
			get {
				if (paramEntry == null) {
					return paramUInt.Value?.value ?? 0;
				} else {
					return paramEntry.Value.paramUInt;
				}
			}
		}
		public float ValueFloat {
			get {
				if (paramEntry == null) {
					return paramFloat.Value?.value ?? 0f;
				} else {
					return paramEntry.Value.paramFloat;
				}
			}
		}
		public Vector3 ParamVector {
			get {
				return paramVector3.Value?.value ?? Vector3.zero;
			}
		}
		public WayPoint ParamWayPoint {
			get { return paramWaypoint.Value; }
		}
		public Graph ParamGraph {
			get { return paramGraph.Value; }
		}
		public Comport ParamComport {
			get { return paramComport.Value; }
		}
		public State ParamAction {
			get { return paramState.Value; }
		}
		public GameMaterial ParamGameMaterial {
			get { return paramGameMaterial.Value; }
		}
		public Perso ParamPerso {
			get { return paramPerso.Value; }
		}
		public int ParamText {
			get { return param; }
		}
		public uint ParamCaps {
			get { return paramCaps.Value?.value ?? 0; }
		}
	}
}
