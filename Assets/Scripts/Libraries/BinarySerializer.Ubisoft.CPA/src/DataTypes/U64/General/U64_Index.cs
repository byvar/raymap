using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
    public class U64_Index<T> : BinarySerializable {
        public ushort Index { get; set; }
        public T Value {
            get {
                if(IsNull) return default;
                return GetValue.Invoke(this);
            }
        }

        private Func<U64_Index<T>, T> GetValue { get; set; }

        public bool IsNull => IsNullable && (Index == 0xFFFF);
        public bool IsNullable { get; set; } = true;

        public override void SerializeImpl(SerializerObject s) {
            Index = s.Serialize<ushort>(Index, name: nameof(Index));
        }

        public U64_Index() {
            Index = 0xFFFF;
        }
        public U64_Index(Context c, ushort index) {
            Context = c;
            Index = index;
        }

        public U64_Index<T> SetAction(Func<U64_Index<T>, T> action) {
            GetValue = action;
            return this;
        }



        public override bool UseShortLog => true;
        public override string ShortLog => $"{Index:X4}";
    }
}
