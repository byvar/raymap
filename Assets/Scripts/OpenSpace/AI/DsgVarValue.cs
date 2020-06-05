using OpenSpace.Input;
using OpenSpace.Object;
using OpenSpace.Object.Properties;
using OpenSpace.Visual;
using OpenSpace.Waypoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class DsgVarValue {
        public DsgVarInfoEntry.DsgVarType type;
		public Pointer offset;
        public DsgMem dsgMem;

		// Array data
		public uint arrayTypeNumber;
		public DsgVarInfoEntry.DsgVarType arrayType = DsgVarInfoEntry.DsgVarType.None;
        public byte arrayLength;
		public DsgVarValue[] valueArray;

		// Possible values
		public Pointer valuePointer;

		public bool valueBool;
        public sbyte valueByte;
        public byte valueUByte;
        public short valueShort;
        public ushort valueUShort;
        public int valueInt;
        public uint valueUInt;
        public float valueFloat;
        public Vector3 valueVector;
		public List valueList;
        public ObjectList valueObjectList;
        public Behavior valueComport;
        public State valueAction;
        public uint valueCaps;
        public EntryAction valueInput;
        public uint valueSoundEvent;
        public LightInfo valueLight;
        public GameMaterial valueGameMaterial;
        public VisualMaterial valueVisualMaterial;
        public Perso valuePerso;
        public WayPoint valueWayPoint;
        public Graph valueGraph;
        public int valueText;
        public SuperObject valueSuperObject;

        public uint valueSOLinks;
		public uint valueWay;

		public DsgVarValue(DsgVarInfoEntry.DsgVarType type, DsgMem dsgMem) {
			this.type = type;
            this.dsgMem = dsgMem;
		}

        public void Read(Reader reader) {
			offset = Pointer.Current(reader);
			switch (type) {
				case DsgVarInfoEntry.DsgVarType.Boolean:
					valueBool = reader.ReadBoolean(); break;
				case DsgVarInfoEntry.DsgVarType.Byte:
					valueByte = reader.ReadSByte(); break;
				case DsgVarInfoEntry.DsgVarType.UByte:
					valueUByte = reader.ReadByte(); break;
				case DsgVarInfoEntry.DsgVarType.Float:
					valueFloat = reader.ReadSingle(); break;
				case DsgVarInfoEntry.DsgVarType.Int:
					valueInt = reader.ReadInt32(); break;
				case DsgVarInfoEntry.DsgVarType.UInt:
					valueUInt = reader.ReadUInt32(); break;
				case DsgVarInfoEntry.DsgVarType.Short:
					valueShort = reader.ReadInt16(); break;
				case DsgVarInfoEntry.DsgVarType.UShort:
					valueUShort = reader.ReadUInt16(); break;
				case DsgVarInfoEntry.DsgVarType.Vector:
					float x = reader.ReadSingle();
					float y = reader.ReadSingle();
					float z = reader.ReadSingle();
					valueVector = new Vector3(x, y, z);
					break;
				case DsgVarInfoEntry.DsgVarType.Text:
					valueText = reader.ReadInt32(); break;
				case DsgVarInfoEntry.DsgVarType.Graph:
					valuePointer = Pointer.Read(reader);
					valueGraph = Graph.FromOffsetOrRead(valuePointer, reader);
					break;
				case DsgVarInfoEntry.DsgVarType.WayPoint:
					valuePointer = Pointer.Read(reader);
					valueWayPoint = WayPoint.FromOffsetOrRead(valuePointer, reader);
					break;
				case DsgVarInfoEntry.DsgVarType.GameMaterial:
					valuePointer = Pointer.Read(reader);
					valueGameMaterial = GameMaterial.FromOffsetOrRead(valuePointer, reader);
					break;
				case DsgVarInfoEntry.DsgVarType.VisualMaterial:
					valuePointer = Pointer.Read(reader);
					valueVisualMaterial = VisualMaterial.FromOffsetOrRead(valuePointer, reader);
					break;
				case DsgVarInfoEntry.DsgVarType.ObjectList:
					valuePointer = Pointer.Read(reader);
					valueObjectList = ObjectList.FromOffsetOrRead(valuePointer, reader);
					break;
				case DsgVarInfoEntry.DsgVarType.List:
					valueList = new List();
					valueList.Read(reader);
					break;
				case DsgVarInfoEntry.DsgVarType.Light:
					valuePointer = Pointer.Read(reader);
					valueLight = MapLoader.Loader.FromOffsetOrRead<LightInfo>(reader, valuePointer);
					break;
				case DsgVarInfoEntry.DsgVarType.Comport:
					valuePointer = Pointer.Read(reader);
					valueComport = MapLoader.Loader.FromOffsetOrRead<Behavior>(reader, valuePointer);
					break;
				case DsgVarInfoEntry.DsgVarType.Input:
					valuePointer = Pointer.Read(reader);
					valueInput = EntryAction.FromOffsetOrRead(valuePointer, reader);
					break;

				// Fill these in after loading
				case DsgVarInfoEntry.DsgVarType.Perso:
					valuePointer = Pointer.Read(reader);
					// Don't fill in perso yet
					MapLoader.Loader.onPostLoad.Add(InitPostLoad);
					break;
				case DsgVarInfoEntry.DsgVarType.Action:
					valuePointer = Pointer.Read(reader);
					// Don't fill in state yet
					MapLoader.Loader.onPostLoad.Add(InitPostLoad);
					break;
				case DsgVarInfoEntry.DsgVarType.SuperObject:
					valuePointer = Pointer.Read(reader);
					// Don't fill in SO yet
					MapLoader.Loader.onPostLoad.Add(InitPostLoad);
					break;

				// TODO: Figure these out
				case DsgVarInfoEntry.DsgVarType.Caps:
					valueCaps = reader.ReadUInt32(); break;
				case DsgVarInfoEntry.DsgVarType.SOLinks:
					valueSOLinks = reader.ReadUInt32(); break;
				case DsgVarInfoEntry.DsgVarType.SoundEvent:
					valueSoundEvent = reader.ReadUInt32(); break;
				case DsgVarInfoEntry.DsgVarType.Way:
					valueWay = reader.ReadUInt32(); break;

				// Arrays
				case DsgVarInfoEntry.DsgVarType.ActionArray:
				case DsgVarInfoEntry.DsgVarType.FloatArray:
				case DsgVarInfoEntry.DsgVarType.IntegerArray:
				case DsgVarInfoEntry.DsgVarType.PersoArray:
				case DsgVarInfoEntry.DsgVarType.SoundEventArray:
				case DsgVarInfoEntry.DsgVarType.SuperObjectArray:
				case DsgVarInfoEntry.DsgVarType.TextArray:
				case DsgVarInfoEntry.DsgVarType.TextRefArray:
				case DsgVarInfoEntry.DsgVarType.VectorArray:
				case DsgVarInfoEntry.DsgVarType.WayPointArray:
				case DsgVarInfoEntry.DsgVarType.GraphArray:
				case DsgVarInfoEntry.DsgVarType.Array11:
				case DsgVarInfoEntry.DsgVarType.Array9:
					ReadArray(reader); break;
			}
		}

		public void InitPostLoad() {
			switch (type) {
				// Fill these in after loading
				case DsgVarInfoEntry.DsgVarType.Perso:
					SuperObject so = SuperObject.FromOffset(valuePointer);
					if (so != null) {
						valuePerso = so.data as Perso;
					}
					break;
				case DsgVarInfoEntry.DsgVarType.Action:
					valueAction = State.FromOffset(valuePointer);
					break;
				case DsgVarInfoEntry.DsgVarType.SuperObject:
					valueSuperObject = SuperObject.FromOffset(valuePointer);
					break;
			}

            if (dsgMem != null) {
                RegisterReferences(dsgMem);
            }
        }

		public override string ToString() {
			switch (type) {
				case DsgVarInfoEntry.DsgVarType.Boolean:
					return valueBool.ToString();
				case DsgVarInfoEntry.DsgVarType.Byte:
					return valueByte.ToString();
				case DsgVarInfoEntry.DsgVarType.UByte:
					return valueUByte.ToString();
				case DsgVarInfoEntry.DsgVarType.Short:
					return valueShort.ToString();
				case DsgVarInfoEntry.DsgVarType.UShort:
					return valueUShort.ToString();
				case DsgVarInfoEntry.DsgVarType.Int:
					return valueInt.ToString();
				case DsgVarInfoEntry.DsgVarType.UInt:
					return valueUInt.ToString();
				case DsgVarInfoEntry.DsgVarType.Float:
					return valueFloat.ToString();
				case DsgVarInfoEntry.DsgVarType.Vector:
					return "new Vector3(" + valueVector.x + ", " + valueVector.y + ", " + valueVector.z + ")";
				case DsgVarInfoEntry.DsgVarType.Text:
					return "TextRef(" + valueText + ")";
				case DsgVarInfoEntry.DsgVarType.Graph:
					return valueGraph?.ToString();
				case DsgVarInfoEntry.DsgVarType.WayPoint:
					return valueWayPoint?.ToString();
				case DsgVarInfoEntry.DsgVarType.GameMaterial:
					return valueGameMaterial?.ToString();
				case DsgVarInfoEntry.DsgVarType.VisualMaterial:
					return valueVisualMaterial?.ToString();
				case DsgVarInfoEntry.DsgVarType.ObjectList:
					return valueObjectList?.ToString();
				case DsgVarInfoEntry.DsgVarType.List:
					return valueList?.ToString();
				case DsgVarInfoEntry.DsgVarType.Light:
					return valueLight?.ToString();
				case DsgVarInfoEntry.DsgVarType.Comport:
					return valueComport?.ShortName;
				case DsgVarInfoEntry.DsgVarType.Input:
					return valueInput?.ToString();
				case DsgVarInfoEntry.DsgVarType.Perso:
					return valuePerso != null ? ("Perso.GetByName(" + valuePerso.namePerso + ")") : "null";
				case DsgVarInfoEntry.DsgVarType.Action:
					return valueAction?.ShortName;
				case DsgVarInfoEntry.DsgVarType.SuperObject:
					return valueSuperObject != null ? ("GameObject.GetByName(" + valueSuperObject.Gao.name + ")") : "null";
				case DsgVarInfoEntry.DsgVarType.Caps:
					return valueCaps.ToString();
				case DsgVarInfoEntry.DsgVarType.SOLinks:
					return valueSOLinks.ToString();
				case DsgVarInfoEntry.DsgVarType.SoundEvent:
					return valueSoundEvent.ToString();
				case DsgVarInfoEntry.DsgVarType.Way:
					return valueWay.ToString();

				// Arrays
				case DsgVarInfoEntry.DsgVarType.ActionArray:
				case DsgVarInfoEntry.DsgVarType.FloatArray:
				case DsgVarInfoEntry.DsgVarType.IntegerArray:
				case DsgVarInfoEntry.DsgVarType.PersoArray:
				case DsgVarInfoEntry.DsgVarType.SoundEventArray:
				case DsgVarInfoEntry.DsgVarType.SuperObjectArray:
				case DsgVarInfoEntry.DsgVarType.TextArray:
				case DsgVarInfoEntry.DsgVarType.TextRefArray:
				case DsgVarInfoEntry.DsgVarType.VectorArray:
				case DsgVarInfoEntry.DsgVarType.WayPointArray:
				case DsgVarInfoEntry.DsgVarType.GraphArray:
				case DsgVarInfoEntry.DsgVarType.Array11:
				case DsgVarInfoEntry.DsgVarType.Array9:
					return "new " + type + "[" + arrayLength + "]";
			}
			return null;
		}

		public void Write(Writer writer) {
			Pointer.Goto(ref writer, offset);
			switch (type) {
				case DsgVarInfoEntry.DsgVarType.Boolean:
					writer.Write(valueBool); break;
				case DsgVarInfoEntry.DsgVarType.Byte:
					writer.Write(valueByte); break;
				case DsgVarInfoEntry.DsgVarType.UByte:
					writer.Write(valueUByte); break;
				case DsgVarInfoEntry.DsgVarType.Float:
					writer.Write(valueFloat); break;
				case DsgVarInfoEntry.DsgVarType.Int:
					writer.Write(valueInt); break;
				case DsgVarInfoEntry.DsgVarType.UInt:
					writer.Write(valueUInt); break;
				case DsgVarInfoEntry.DsgVarType.Short:
					writer.Write(valueShort); break;
				case DsgVarInfoEntry.DsgVarType.UShort:
					writer.Write(valueUShort); break;
				case DsgVarInfoEntry.DsgVarType.Vector:
					writer.Write(valueVector.x);
					writer.Write(valueVector.y);
					writer.Write(valueVector.z);
					break;
				case DsgVarInfoEntry.DsgVarType.Text:
					writer.Write(valueText); break;
				case DsgVarInfoEntry.DsgVarType.Graph:
					Pointer.Write(writer, valuePointer); break;
				case DsgVarInfoEntry.DsgVarType.WayPoint:
					Pointer.Write(writer, valuePointer); break;
				case DsgVarInfoEntry.DsgVarType.GameMaterial:
					Pointer.Write(writer, valuePointer); break;
				case DsgVarInfoEntry.DsgVarType.VisualMaterial:
					Pointer.Write(writer, valuePointer); break;
				case DsgVarInfoEntry.DsgVarType.ObjectList:
					Pointer.Write(writer, valuePointer); break;
				case DsgVarInfoEntry.DsgVarType.List:
					valueList?.Write(writer); break;
				case DsgVarInfoEntry.DsgVarType.Light:
					Pointer.Write(writer, valuePointer); break;
				case DsgVarInfoEntry.DsgVarType.Comport:
					Pointer.Write(writer, valuePointer); break;
				case DsgVarInfoEntry.DsgVarType.Input:
					Pointer.Write(writer, valuePointer); break;
				case DsgVarInfoEntry.DsgVarType.Perso:
					Pointer.Write(writer, valuePointer); break;
				case DsgVarInfoEntry.DsgVarType.Action:
					Pointer.Write(writer, valuePointer); break;
				case DsgVarInfoEntry.DsgVarType.SuperObject:
					Pointer.Write(writer, valuePointer); break;

				// TODO: Figure these out
				case DsgVarInfoEntry.DsgVarType.Caps:
					writer.Write(valueCaps); break;
				case DsgVarInfoEntry.DsgVarType.SOLinks:
					writer.Write(valueSOLinks); break;
				case DsgVarInfoEntry.DsgVarType.SoundEvent:
					writer.Write(valueSoundEvent); break;
				case DsgVarInfoEntry.DsgVarType.Way:
					writer.Write(valueWay); break;

				// Arrays
				case DsgVarInfoEntry.DsgVarType.ActionArray:
				case DsgVarInfoEntry.DsgVarType.FloatArray:
				case DsgVarInfoEntry.DsgVarType.IntegerArray:
				case DsgVarInfoEntry.DsgVarType.PersoArray:
				case DsgVarInfoEntry.DsgVarType.SoundEventArray:
				case DsgVarInfoEntry.DsgVarType.SuperObjectArray:
				case DsgVarInfoEntry.DsgVarType.TextArray:
				case DsgVarInfoEntry.DsgVarType.TextRefArray:
				case DsgVarInfoEntry.DsgVarType.VectorArray:
				case DsgVarInfoEntry.DsgVarType.WayPointArray:
				case DsgVarInfoEntry.DsgVarType.GraphArray:
				case DsgVarInfoEntry.DsgVarType.Array11:
				case DsgVarInfoEntry.DsgVarType.Array9:
					WriteArray(writer); break;
			}
		}
		public void ReadArray(Reader reader) {
			if (Settings.s.game == Settings.Game.R2Revolution) {
				reader.ReadUInt32();
				arrayTypeNumber = reader.ReadByte();
				arrayLength = reader.ReadByte();
				reader.ReadBytes(2); // padding
			} else {
				arrayTypeNumber = reader.ReadUInt32();
				arrayLength = reader.ReadByte();
				reader.ReadBytes(3); // padding
			}
			arrayType = Settings.s.aiTypes.GetDsgVarType(arrayTypeNumber);
			if (DsgVarInfoEntry.GetDsgVarTypeFromArrayType(type) != arrayType) {
				Debug.LogWarning(currentbuf + " - " + type + " - " + arrayTypeNumber + " - " + arrayType + " - " + arrayLength + " - " + Pointer.Current(reader));
			}
			if (valueArray == null || arrayLength != valueArray.Length) {
				valueArray = new DsgVarValue[arrayLength];
				for (int i = 0; i < arrayLength; i++) {
					valueArray[i] = new DsgVarValue(arrayType, dsgMem);
				}
			}
			for (uint i = 0; i < arrayLength; i++) {
				valueArray[i].Read(reader);
			}
		}

        public List<SearchableString> GetSearchableString(Perso perso, int dsgVarNum)
        {

            string locationString = "DsgVar_" + dsgVarNum;

            List<SearchableString> results = new List<SearchableString>();

            switch(type) {
                case DsgVarInfoEntry.DsgVarType.Text:
                    results.Add(new SearchableString(MapLoader.Loader.localization.GetTextForHandleAndLanguageID(valueText, 0), perso.Gao, locationString));
                    break;
                case DsgVarInfoEntry.DsgVarType.TextArray:
                case DsgVarInfoEntry.DsgVarType.TextRefArray:

                    foreach (var item in valueArray) {
                        results.AddRange(item.GetSearchableString(perso, dsgVarNum));
                    }
                    break;
                default:break;
            }

            return results;

        }

        public void WriteArray(Writer writer) {
			if (Settings.s.game == Settings.Game.R2Revolution) {
				Pointer.Goto(ref writer, Pointer.Current(writer) + 4);
				writer.Write((byte)arrayTypeNumber);
				writer.Write(arrayLength);
				writer.Write((byte)0);
				writer.Write((byte)0);
			} else {
				writer.Write(arrayTypeNumber);
				writer.Write(arrayLength);
				writer.Write((byte)0);
				writer.Write((byte)0);
				writer.Write((byte)0);
			}
			arrayType = Settings.s.aiTypes.GetDsgVarType(arrayTypeNumber);
			if (DsgVarInfoEntry.GetDsgVarTypeFromArrayType(type) != arrayType) {
				Debug.LogWarning(currentbuf + " - " + arrayTypeNumber + " - " + arrayType + " - " + arrayLength + " - " + Pointer.Current(writer));
			}
			if (valueArray != null && arrayLength == valueArray.Length) {
				for (uint i = 0; i < arrayLength; i++) {
					valueArray[i].Write(writer);
				}
			}
		}

		public void ReadFromBuffer(Reader reader, DsgVarInfoEntry infoEntry, Pointer buffer) {
            ReadFromBuffer(reader, infoEntry.offsetInBuffer, buffer);
        }

        public void ReadFromBuffer(Reader reader, uint offsetInBuffer, Pointer buffer) {
			Pointer.DoAt(ref reader, buffer + offsetInBuffer, () => {
				Read(reader);
			});
        }

		public string currentbuf = "";
        public void ReadFromDsgMemBuffer(Reader reader, DsgVarInfoEntry infoEntry, DsgMem dsgMem) {
            currentbuf = "dsgmem";
            ReadFromBuffer(reader, infoEntry, dsgMem.memBuffer);
        }

        public void ReadFromDsgMemBufferInitial(Reader reader, DsgVarInfoEntry infoEntry, DsgMem dsgMem) {
            currentbuf = "dsgmem_initial";
            ReadFromBuffer(reader, infoEntry, dsgMem.memBufferInitial);
        }

        public void ReadFromDsgVarBuffer(Reader reader, DsgVarInfoEntry infoEntry, DsgVar dsgVar) {
            currentbuf = "dsgvar";
            ReadFromBuffer(reader, infoEntry, dsgVar.off_dsgMemBuffer);
        }

		public bool IsSameValue(DsgVarValue other) {
			if (other == null) return false;
			if(Equals(other)) return true;
			if (type != other.type) return false;
			switch (type) {
				case DsgVarInfoEntry.DsgVarType.Boolean:
					return valueBool == other.valueBool;
				case DsgVarInfoEntry.DsgVarType.Byte:
					return valueByte == other.valueByte;
				case DsgVarInfoEntry.DsgVarType.UByte:
					return valueUByte == other.valueUByte;
				case DsgVarInfoEntry.DsgVarType.Short:
					return valueShort == other.valueShort;
				case DsgVarInfoEntry.DsgVarType.UShort:
					return valueUShort == other.valueUShort;
				case DsgVarInfoEntry.DsgVarType.Int:
					return valueInt == other.valueInt;
				case DsgVarInfoEntry.DsgVarType.UInt:
					return valueUInt == other.valueUInt;
				case DsgVarInfoEntry.DsgVarType.Float:
					return valueFloat == other.valueFloat;
				case DsgVarInfoEntry.DsgVarType.Vector:
					return valueVector == other.valueVector;
				case DsgVarInfoEntry.DsgVarType.Text:
					return valueText == other.valueText;
				case DsgVarInfoEntry.DsgVarType.Graph:
					return valuePointer == other.valuePointer;
				case DsgVarInfoEntry.DsgVarType.WayPoint:
					return valuePointer == other.valuePointer;
				case DsgVarInfoEntry.DsgVarType.GameMaterial:
					return valuePointer == other.valuePointer;
				case DsgVarInfoEntry.DsgVarType.VisualMaterial:
					return valuePointer == other.valuePointer;
				case DsgVarInfoEntry.DsgVarType.ObjectList:
					return valuePointer == other.valuePointer;
				case DsgVarInfoEntry.DsgVarType.List:
					if (valueList.curLength != other.valueList.curLength
						|| valueList.maxLength != other.valueList.maxLength) {
						return false;
					}
					for (int i = 0; i < valueList.maxLength; i++) {
						if (valueList.list[i].value != other.valueList.list[i].value) {
							return false;
						}
					}
					return true;
				case DsgVarInfoEntry.DsgVarType.Light:
					return valuePointer == other.valuePointer;
				case DsgVarInfoEntry.DsgVarType.Comport:
					return valuePointer == other.valuePointer;
				case DsgVarInfoEntry.DsgVarType.Input:
					return valuePointer == other.valuePointer;
				case DsgVarInfoEntry.DsgVarType.Perso:
					return valuePointer == other.valuePointer;
				case DsgVarInfoEntry.DsgVarType.Action:
					return valuePointer == other.valuePointer;
				case DsgVarInfoEntry.DsgVarType.SuperObject:
					return valuePointer == other.valuePointer;
				case DsgVarInfoEntry.DsgVarType.Caps:
					return valueCaps == other.valueCaps;
				case DsgVarInfoEntry.DsgVarType.SOLinks:
					return valueSOLinks == other.valueSOLinks;
				case DsgVarInfoEntry.DsgVarType.SoundEvent:
					return valueSoundEvent == other.valueSoundEvent;
				case DsgVarInfoEntry.DsgVarType.Way:
					return valueWay == other.valueWay;

				// Arrays
				case DsgVarInfoEntry.DsgVarType.ActionArray:
				case DsgVarInfoEntry.DsgVarType.FloatArray:
				case DsgVarInfoEntry.DsgVarType.IntegerArray:
				case DsgVarInfoEntry.DsgVarType.PersoArray:
				case DsgVarInfoEntry.DsgVarType.SoundEventArray:
				case DsgVarInfoEntry.DsgVarType.SuperObjectArray:
				case DsgVarInfoEntry.DsgVarType.TextArray:
				case DsgVarInfoEntry.DsgVarType.TextRefArray:
				case DsgVarInfoEntry.DsgVarType.VectorArray:
				case DsgVarInfoEntry.DsgVarType.WayPointArray:
				case DsgVarInfoEntry.DsgVarType.GraphArray:
				case DsgVarInfoEntry.DsgVarType.Array11:
				case DsgVarInfoEntry.DsgVarType.Array9:
					if (arrayType != other.arrayType || arrayLength != other.arrayLength) {
						return false;
					}
					for (int i = 0; i < valueArray.Length; i++) {
						if (!valueArray[i].IsSameValue(other.valueArray[i])) return false;
					}
					return true;
			}
			return true;
		}

        public void RegisterReferences(DsgMem dsgMem)
        {
            switch(type) {
                case DsgVarInfoEntry.DsgVarType.SuperObject:    valueSuperObject?.References.referencedByDsgMems.Add(dsgMem); break;
                case DsgVarInfoEntry.DsgVarType.Perso:          valuePerso?.References.referencedByDsgMems.Add(dsgMem); break;
                case DsgVarInfoEntry.DsgVarType.WayPoint:       valueWayPoint?.References.referencedByDsgMems.Add(dsgMem); break;
                case DsgVarInfoEntry.DsgVarType.Graph:          valueGraph?.References.referencedByDsgMems.Add(dsgMem); break;
                // Arrays
                case DsgVarInfoEntry.DsgVarType.SuperObjectArray:
                case DsgVarInfoEntry.DsgVarType.PersoArray:
                case DsgVarInfoEntry.DsgVarType.WayPointArray:
                case DsgVarInfoEntry.DsgVarType.GraphArray:
                    valueArray.ToList().ForEach(v => v.RegisterReferences(dsgMem)); break;
                default: return;
            }
        }

        public class List {
			public byte curLength;
			public byte maxLength;
			public Entry[] list;

			public struct Entry {
				public uint value;
				public Pointer ptr;
			}

			public void Read(Reader reader) {
				curLength = reader.ReadByte();
				maxLength = reader.ReadByte();
				reader.ReadBytes(2); // padding
				list = new Entry[maxLength];
				for (int i = 0; i < maxLength; i++) {
					list[i] = new Entry();
					list[i].ptr = Pointer.GetPointerAtOffset(Pointer.Current(reader));
					list[i].value = reader.ReadUInt32();
				}
			}
			public void Write(Writer writer) {
				writer.Write(curLength);
				writer.Write(maxLength);
				writer.Write((byte)0);
				writer.Write((byte)0);
				for (int i = 0; i < maxLength; i++) {
					writer.Write(list[i].value);
				}
			}
		}
    }
}
