using System;
using System.Collections.Generic;
using System.Linq;

namespace BinarySerializer.Ubisoft.CPA.U64
{
    public class LDR_FatTable : BinarySerializable
    {
        public uint Pre_ObjectsCount { get; set; }

        public LDR_EntryRef[] Entries { get; set; }

        // Parsed
        public Dictionary<U64_StructType, Dictionary<ushort, LDR_EntryRef>> EntriesLookup { get; private set; }


        public override void SerializeImpl(SerializerObject s)
        {
			Entries = s.SerializeObjectArray<LDR_EntryRef>(Entries, Pre_ObjectsCount, name: nameof(Entries));
            Init();
		}

        public void Init() {
            EntriesLookup = new Dictionary<U64_StructType, Dictionary<ushort, LDR_EntryRef>>();
            foreach (var e in Entries) {
                var type = U64_StructType_Defines.GetType(Context, e.Type);
                if(!type.HasValue) continue;
                if(!EntriesLookup.ContainsKey(type.Value)) EntriesLookup[type.Value] = new Dictionary<ushort, LDR_EntryRef>();
                EntriesLookup[type.Value][e.Index] = e;
            }
        }

        public LDR_EntryRef GetEntry(U64_StructType type, ushort index) {
            if (!EntriesLookup.ContainsKey(type) || !EntriesLookup[type].ContainsKey(index)) return null;
            return EntriesLookup[type][index];
        }

        public LDR_EntryRef GetEntry(ushort type, ushort index) {
            return Entries.FirstOrDefault(e => e.Type == type && e.Index == index);
        }
    }
}