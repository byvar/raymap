using System;

namespace BinarySerializer.Ubisoft.CPA.U64
{
    public class LDR_Fat : BinarySerializable
    {
        public Pointer<LDR_FatTable> Fat { get; set; }
        public uint ObjectsCount { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Fat = s.SerializePointer<LDR_FatTable>(Fat, name: nameof(Fat));
            ObjectsCount = s.Serialize<uint>(ObjectsCount, name: nameof(ObjectsCount));
        }

        public void SerializeFat(SerializerObject s) {
            Fat?.Resolve(s, onPreSerialize: f => f.Pre_ObjectsCount = ObjectsCount);
        }
    }
}