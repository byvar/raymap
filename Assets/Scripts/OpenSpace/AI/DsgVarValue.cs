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
        public DsgVarType type;
		public LegacyPointer offset;
        public DsgMem dsgMem;

		// Array data
		public uint arrayTypeNumber;
		public DsgVarType arrayType = DsgVarType.None;
        public byte arrayLength;
		public DsgVarValue[] valueArray;

		// Possible values
		public LegacyPointer valuePointer;

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
        public Macro valueComport;
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

		public DsgVarValue(DsgVarType type, DsgMem dsgMem) {
			this.type = type;
            this.dsgMem = dsgMem;
		}

        public void Read(Reader reader) {
			offset = LegacyPointer.Current(reader);
			switch (type) {
				case DsgVarType.Boolean:
					valueBool = reader.ReadBoolean(); break;
				case DsgVarType.Byte:
					valueByte = reader.ReadSByte(); break;
				case DsgVarType.UByte:
					valueUByte = reader.ReadByte(); break;
				case DsgVarType.Float:
					valueFloat = reader.ReadSingle(); break;
				case DsgVarType.Int:
					valueInt = reader.ReadInt32(); break;
				case DsgVarType.UInt:
					valueUInt = reader.ReadUInt32(); break;
				case DsgVarType.Short:
					valueShort = reader.ReadInt16(); break;
				case DsgVarType.UShort:
					valueUShort = reader.ReadUInt16(); break;
				case DsgVarType.Vector:
					float x = reader.ReadSingle();
					float y = reader.ReadSingle();
					float z = reader.ReadSingle();
					valueVector = new Vector3(x, y, z);
					break;
				case DsgVarType.Text:
					valueText = reader.ReadInt32(); break;
				case DsgVarType.Graph:
					valuePointer = LegacyPointer.Read(reader);
					valueGraph = Graph.FromOffsetOrRead(valuePointer, reader);
					break;
				case DsgVarType.WayPoint:
					valuePointer = LegacyPointer.Read(reader);
					valueWayPoint = WayPoint.FromOffsetOrRead(valuePointer, reader);
					break;
				case DsgVarType.GameMaterial:
					valuePointer = LegacyPointer.Read(reader);
					valueGameMaterial = GameMaterial.FromOffsetOrRead(valuePointer, reader);
					break;
				case DsgVarType.VisualMaterial:
					valuePointer = LegacyPointer.Read(reader);
					valueVisualMaterial = VisualMaterial.FromOffsetOrRead(valuePointer, reader);
					break;
				case DsgVarType.ObjectList:
					valuePointer = LegacyPointer.Read(reader);
					valueObjectList = ObjectList.FromOffsetOrRead(valuePointer, reader);
					break;
				case DsgVarType.List:
					valueList = new List();
					valueList.Read(reader);
					break;
				case DsgVarType.Light:
					valuePointer = LegacyPointer.Read(reader);
					valueLight = MapLoader.Loader.FromOffsetOrRead<LightInfo>(reader, valuePointer);
					break;
				case DsgVarType.Comport:
					valuePointer = LegacyPointer.Read(reader);
                    // 2021-05-25 Changed type from Behavior to Macro since it always seems to be a Macro (from R3, seemingly unused in R2) - RTS
					valueComport = MapLoader.Loader.FromOffsetOrRead<Macro>(reader, valuePointer);
					break;
				case DsgVarType.Input:
					valuePointer = LegacyPointer.Read(reader);
					valueInput = EntryAction.FromOffsetOrRead(valuePointer, reader);
					break;

				// Fill these in after loading
				case DsgVarType.Perso:
					valuePointer = LegacyPointer.Read(reader);
					// Don't fill in perso yet
					MapLoader.Loader.onPostLoad.Add(InitPostLoad);
					break;
				case DsgVarType.Action:
					valuePointer = LegacyPointer.Read(reader);
					// Don't fill in state yet
					MapLoader.Loader.onPostLoad.Add(InitPostLoad);
					break;
				case DsgVarType.SuperObject:
					valuePointer = LegacyPointer.Read(reader);
					// Don't fill in SO yet
					MapLoader.Loader.onPostLoad.Add(InitPostLoad);
					break;

				// TODO: Figure these out
				case DsgVarType.Caps:
					valueCaps = reader.ReadUInt32(); break;
				case DsgVarType.SOLinks:
					valueSOLinks = reader.ReadUInt32(); break;
				case DsgVarType.SoundEvent:
					valueSoundEvent = reader.ReadUInt32(); break;
				case DsgVarType.Way:
					valueWay = reader.ReadUInt32(); break;

				// Arrays
				case DsgVarType.ActionArray:
				case DsgVarType.FloatArray:
				case DsgVarType.IntegerArray:
				case DsgVarType.PersoArray:
				case DsgVarType.SoundEventArray:
				case DsgVarType.SuperObjectArray:
				case DsgVarType.TextArray:
				case DsgVarType.TextRefArray:
				case DsgVarType.VectorArray:
				case DsgVarType.WayPointArray:
				case DsgVarType.GraphArray:
				case DsgVarType.VisualMatArray:
				case DsgVarType.SOLinksArray:
					ReadArray(reader); break;
			}
		}

		public void InitPostLoad() {
			switch (type) {
				// Fill these in after loading
				case DsgVarType.Perso:
					SuperObject so = SuperObject.FromOffset(valuePointer);
					if (so != null) {
						valuePerso = so.data as Perso;
					}
					break;
				case DsgVarType.Action:
					valueAction = State.FromOffset(valuePointer);
					break;
				case DsgVarType.SuperObject:
					valueSuperObject = SuperObject.FromOffset(valuePointer);
					break;
			}

            if (dsgMem != null) {
                RegisterReferences(dsgMem);
            }
        }

		public override string ToString() {
			switch (type) {
				case DsgVarType.Boolean:
					return valueBool.ToString();
				case DsgVarType.Byte:
					return valueByte.ToString();
				case DsgVarType.UByte:
					return valueUByte.ToString();
				case DsgVarType.Short:
					return valueShort.ToString();
				case DsgVarType.UShort:
					return valueUShort.ToString();
				case DsgVarType.Int:
					return valueInt.ToString();
				case DsgVarType.UInt:
					return valueUInt.ToString();
				case DsgVarType.Float:
					return valueFloat.ToString();
				case DsgVarType.Vector:
					return "new Vector3(" + valueVector.x + ", " + valueVector.y + ", " + valueVector.z + ")";
				case DsgVarType.Text:
					return "TextRef(" + valueText + ")";
				case DsgVarType.Graph:
					return valueGraph?.ToString();
				case DsgVarType.WayPoint:
					return valueWayPoint?.ToString();
				case DsgVarType.GameMaterial:
					return valueGameMaterial?.ToString();
				case DsgVarType.VisualMaterial:
					return valueVisualMaterial?.ToString();
				case DsgVarType.ObjectList:
					return valueObjectList?.ToString();
				case DsgVarType.List:
					return valueList?.ToString();
				case DsgVarType.Light:
					return valueLight?.ToString();
				case DsgVarType.Comport:
					return valueComport?.ShortName;
				case DsgVarType.Input:
					return valueInput?.ToString();
				case DsgVarType.Perso:
					return valuePerso != null ? ("Perso.GetByName(" + valuePerso.namePerso + ")") : "null";
				case DsgVarType.Action:
					return valueAction?.ShortName;
				case DsgVarType.SuperObject:
					return valueSuperObject != null ? ("GameObject.GetByName(" + valueSuperObject.Gao.name + ")") : "null";
				case DsgVarType.Caps:
					return valueCaps.ToString();
				case DsgVarType.SOLinks:
					return valueSOLinks.ToString();
				case DsgVarType.SoundEvent:
					return valueSoundEvent.ToString();
				case DsgVarType.Way:
					return valueWay.ToString();

				// Arrays
				case DsgVarType.ActionArray:
				case DsgVarType.FloatArray:
				case DsgVarType.IntegerArray:
				case DsgVarType.PersoArray:
				case DsgVarType.SoundEventArray:
				case DsgVarType.SuperObjectArray:
				case DsgVarType.TextArray:
				case DsgVarType.TextRefArray:
				case DsgVarType.VectorArray:
				case DsgVarType.WayPointArray:
				case DsgVarType.GraphArray:
				case DsgVarType.VisualMatArray:
				case DsgVarType.SOLinksArray:
					return "new " + type + "[" + arrayLength + "]";
			}
			return null;
		}

		public void Write(Writer writer) {
			LegacyPointer.Goto(ref writer, offset);
			switch (type) {
				case DsgVarType.Boolean:
					writer.Write(valueBool); break;
				case DsgVarType.Byte:
					writer.Write(valueByte); break;
				case DsgVarType.UByte:
					writer.Write(valueUByte); break;
				case DsgVarType.Float:
					writer.Write(valueFloat); break;
				case DsgVarType.Int:
					writer.Write(valueInt); break;
				case DsgVarType.UInt:
					writer.Write(valueUInt); break;
				case DsgVarType.Short:
					writer.Write(valueShort); break;
				case DsgVarType.UShort:
					writer.Write(valueUShort); break;
				case DsgVarType.Vector:
					writer.Write(valueVector.x);
					writer.Write(valueVector.y);
					writer.Write(valueVector.z);
					break;
				case DsgVarType.Text:
					writer.Write(valueText); break;
				case DsgVarType.Graph:
					LegacyPointer.Write(writer, valuePointer); break;
				case DsgVarType.WayPoint:
					LegacyPointer.Write(writer, valuePointer); break;
				case DsgVarType.GameMaterial:
					LegacyPointer.Write(writer, valuePointer); break;
				case DsgVarType.VisualMaterial:
					LegacyPointer.Write(writer, valuePointer); break;
				case DsgVarType.ObjectList:
					LegacyPointer.Write(writer, valuePointer); break;
				case DsgVarType.List:
					valueList?.Write(writer); break;
				case DsgVarType.Light:
					LegacyPointer.Write(writer, valuePointer); break;
				case DsgVarType.Comport:
					LegacyPointer.Write(writer, valuePointer); break;
				case DsgVarType.Input:
					LegacyPointer.Write(writer, valuePointer); break;
				case DsgVarType.Perso:
					LegacyPointer.Write(writer, valuePointer); break;
				case DsgVarType.Action:
					LegacyPointer.Write(writer, valuePointer); break;
				case DsgVarType.SuperObject:
					LegacyPointer.Write(writer, valuePointer); break;

				// TODO: Figure these out
				case DsgVarType.Caps:
					writer.Write(valueCaps); break;
				case DsgVarType.SOLinks:
					writer.Write(valueSOLinks); break;
				case DsgVarType.SoundEvent:
					writer.Write(valueSoundEvent); break;
				case DsgVarType.Way:
					writer.Write(valueWay); break;

				// Arrays
				case DsgVarType.ActionArray:
				case DsgVarType.FloatArray:
				case DsgVarType.IntegerArray:
				case DsgVarType.PersoArray:
				case DsgVarType.SoundEventArray:
				case DsgVarType.SuperObjectArray:
				case DsgVarType.TextArray:
				case DsgVarType.TextRefArray:
				case DsgVarType.VectorArray:
				case DsgVarType.WayPointArray:
				case DsgVarType.GraphArray:
				case DsgVarType.VisualMatArray:
				case DsgVarType.SOLinksArray:
					WriteArray(writer); break;
			}
		}
		public void ReadArray(Reader reader) {
			if (Legacy_Settings.s.game == Legacy_Settings.Game.R2Revolution) {
				reader.ReadUInt32();
				arrayTypeNumber = reader.ReadByte();
				arrayLength = reader.ReadByte();
				reader.ReadBytes(2); // padding
			} else {
				arrayTypeNumber = reader.ReadUInt32();
				arrayLength = reader.ReadByte();
				reader.ReadBytes(3); // padding
			}
			arrayType = Legacy_Settings.s.aiTypes.GetDsgVarType(arrayTypeNumber);
			if (DsgVarInfoEntry.GetDsgVarTypeFromArrayType(type) != arrayType) {
				Debug.LogWarning(currentbuf + " - " + type + " - " + arrayTypeNumber + " - " + arrayType + " - " + arrayLength + " - " + LegacyPointer.Current(reader));
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
                case DsgVarType.Text:
                    results.Add(new SearchableString(MapLoader.Loader.localization.GetTextForHandleAndLanguageID(valueText, 0), perso.Gao, locationString));
                    break;
                case DsgVarType.TextArray:
                case DsgVarType.TextRefArray:

                    foreach (var item in valueArray) {
                        results.AddRange(item.GetSearchableString(perso, dsgVarNum));
                    }
                    break;
                default:break;
            }

            return results;

        }

        public void WriteArray(Writer writer) {
			if (Legacy_Settings.s.game == Legacy_Settings.Game.R2Revolution) {
				LegacyPointer.Goto(ref writer, LegacyPointer.Current(writer) + 4);
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
			arrayType = Legacy_Settings.s.aiTypes.GetDsgVarType(arrayTypeNumber);
			if (DsgVarInfoEntry.GetDsgVarTypeFromArrayType(type) != arrayType) {
				Debug.LogWarning(currentbuf + " - " + arrayTypeNumber + " - " + arrayType + " - " + arrayLength + " - " + LegacyPointer.Current(writer));
			}
			if (valueArray != null && arrayLength == valueArray.Length) {
				for (uint i = 0; i < arrayLength; i++) {
					valueArray[i].Write(writer);
				}
			}
		}

		public void ReadFromBuffer(Reader reader, DsgVarInfoEntry infoEntry, LegacyPointer buffer) {
            ReadFromBuffer(reader, infoEntry.offsetInBuffer, buffer);
        }

        public void ReadFromBuffer(Reader reader, uint offsetInBuffer, LegacyPointer buffer) {
			LegacyPointer.DoAt(ref reader, buffer + offsetInBuffer, () => {
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
				case DsgVarType.Boolean:
					return valueBool == other.valueBool;
				case DsgVarType.Byte:
					return valueByte == other.valueByte;
				case DsgVarType.UByte:
					return valueUByte == other.valueUByte;
				case DsgVarType.Short:
					return valueShort == other.valueShort;
				case DsgVarType.UShort:
					return valueUShort == other.valueUShort;
				case DsgVarType.Int:
					return valueInt == other.valueInt;
				case DsgVarType.UInt:
					return valueUInt == other.valueUInt;
				case DsgVarType.Float:
					return valueFloat == other.valueFloat;
				case DsgVarType.Vector:
					return valueVector == other.valueVector;
				case DsgVarType.Text:
					return valueText == other.valueText;
				case DsgVarType.Graph:
					return valuePointer == other.valuePointer;
				case DsgVarType.WayPoint:
					return valuePointer == other.valuePointer;
				case DsgVarType.GameMaterial:
					return valuePointer == other.valuePointer;
				case DsgVarType.VisualMaterial:
					return valuePointer == other.valuePointer;
				case DsgVarType.ObjectList:
					return valuePointer == other.valuePointer;
				case DsgVarType.List:
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
				case DsgVarType.Light:
					return valuePointer == other.valuePointer;
				case DsgVarType.Comport:
					return valuePointer == other.valuePointer;
				case DsgVarType.Input:
					return valuePointer == other.valuePointer;
				case DsgVarType.Perso:
					return valuePointer == other.valuePointer;
				case DsgVarType.Action:
					return valuePointer == other.valuePointer;
				case DsgVarType.SuperObject:
					return valuePointer == other.valuePointer;
				case DsgVarType.Caps:
					return valueCaps == other.valueCaps;
				case DsgVarType.SOLinks:
					return valueSOLinks == other.valueSOLinks;
				case DsgVarType.SoundEvent:
					return valueSoundEvent == other.valueSoundEvent;
				case DsgVarType.Way:
					return valueWay == other.valueWay;

				// Arrays
				case DsgVarType.ActionArray:
				case DsgVarType.FloatArray:
				case DsgVarType.IntegerArray:
				case DsgVarType.PersoArray:
				case DsgVarType.SoundEventArray:
				case DsgVarType.SuperObjectArray:
				case DsgVarType.TextArray:
				case DsgVarType.TextRefArray:
				case DsgVarType.VectorArray:
				case DsgVarType.WayPointArray:
				case DsgVarType.GraphArray:
				case DsgVarType.VisualMatArray:
				case DsgVarType.SOLinksArray:
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
                case DsgVarType.SuperObject:    valueSuperObject?.References.referencedByDsgMems.Add(dsgMem); break;
                case DsgVarType.Perso:          valuePerso?.References.referencedByDsgMems.Add(dsgMem); break;
                case DsgVarType.WayPoint:       valueWayPoint?.References.referencedByDsgMems.Add(dsgMem); break;
                case DsgVarType.Graph:          valueGraph?.References.referencedByDsgMems.Add(dsgMem); break;
                // Arrays
                case DsgVarType.SuperObjectArray:
                case DsgVarType.PersoArray:
                case DsgVarType.WayPointArray:
                case DsgVarType.GraphArray:
                    valueArray.ToList().ForEach(v => v.RegisterReferences(dsgMem)); break;
                default: return;
            }
        }

        public object GetUntypedValue()
        {
            switch (type) {
                case DsgVarType.Boolean: return valueBool;
                case DsgVarType.Byte: return valueByte;
                case DsgVarType.UByte: return valueUByte;
                case DsgVarType.Float: return valueFloat;
                case DsgVarType.Int: return valueInt;
                case DsgVarType.UInt: return valueUInt;
                case DsgVarType.Short: return valueShort;
                case DsgVarType.UShort: return valueUShort;
                case DsgVarType.Vector: return valueVector;
                case DsgVarType.Text: return valueText;

                case DsgVarType.Graph:
                case DsgVarType.WayPoint:
                case DsgVarType.GameMaterial:
                case DsgVarType.VisualMaterial:
                case DsgVarType.ObjectList:
                case DsgVarType.Light:
                case DsgVarType.Comport:
                case DsgVarType.Input:

                case DsgVarType.Perso:
				case DsgVarType.Action:
                case DsgVarType.SuperObject:
					return valuePointer;

                case DsgVarType.List:
                    return valueList.list;

                // TODO: Figure these out
                case DsgVarType.Caps: return valueCaps;
                case DsgVarType.SOLinks: return valueSOLinks;
                case DsgVarType.SoundEvent: return valueSoundEvent;
                case DsgVarType.Way: return valueWay;


                case DsgVarType.ActionArray:
                case DsgVarType.FloatArray:
                case DsgVarType.IntegerArray:
                case DsgVarType.PersoArray:
                case DsgVarType.SoundEventArray:
                case DsgVarType.SuperObjectArray:
                case DsgVarType.TextArray:
                case DsgVarType.TextRefArray:
                case DsgVarType.VectorArray:
                case DsgVarType.WayPointArray:
                case DsgVarType.GraphArray:
                case DsgVarType.VisualMatArray:
                case DsgVarType.SOLinksArray:
                    List<object> untypedArray = new List<object>();
                    foreach (var item in this.valueArray) {
						untypedArray.Add(item.GetUntypedValue());
                    }

                    return untypedArray.ToArray();
			}

            return null;
        }

        public class List {
			public byte curLength;
			public byte maxLength;
			public Entry[] list;

			public struct Entry {
				public uint value;
				public LegacyPointer ptr;
			}

			public void Read(Reader reader) {
				curLength = reader.ReadByte();
				maxLength = reader.ReadByte();
				reader.ReadBytes(2); // padding
				list = new Entry[maxLength];
				for (int i = 0; i < maxLength; i++) {
					list[i] = new Entry();
					list[i].ptr = LegacyPointer.GetPointerAtOffset(LegacyPointer.Current(reader));
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
