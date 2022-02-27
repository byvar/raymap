using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
    public class GAM_StateRef : U64_Struct {
        public U64_ArrayReference<LST_Ref<GAM_State>> StateRefList { get; set; }
        public ushort StateRefListCount { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            StateRefList = s.SerializeObject<U64_ArrayReference<LST_Ref<GAM_State>>>(StateRefList, name: nameof(StateRefList));
            StateRefListCount = s.Serialize<ushort>(StateRefListCount, name: nameof(StateRefListCount));

            StateRefList?.Resolve(s, StateRefListCount);
        }
    }
}
