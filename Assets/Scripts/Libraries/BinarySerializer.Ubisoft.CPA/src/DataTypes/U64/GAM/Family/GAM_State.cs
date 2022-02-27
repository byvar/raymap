using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
    public class GAM_State : U64_Struct {
        public U64_Reference<GAM_AnimInfo> AnimInfo { get; set; }
        public U64_GenericReference MechanicsIdCard { get; set; }
        public U64_ArrayReference<GAM_StateTransition> TargetStateList { get; set; }
        public U64_Reference<GAM_State> NextState { get; set; }
        public U64_Reference<GAM_State> ProhibitedTargetState { get; set; }
        public ushort TargetStatesCount { get; set; }
        // public ushort MechanicsIdCardType { get; set; }
        public ushort Repeat { get; set; }
        public ushort Speed { get; set; }
        public byte TransitionStatusFlag { get; set; }
        public byte CustomBits { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            AnimInfo = s.SerializeObject<U64_Reference<GAM_AnimInfo>>(AnimInfo, name: nameof(AnimInfo))?.Resolve(s);
            MechanicsIdCard = s.SerializeObject<U64_GenericReference>(MechanicsIdCard,
                onPreSerialize: gr => gr.ImmediateSerializeType = U64_GenericReference.ImmediateSerialize.Index,
                name: nameof(MechanicsIdCard));
            TargetStateList = s.SerializeObject<U64_ArrayReference<GAM_StateTransition>>(TargetStateList, name: nameof(TargetStateList));
            NextState = s.SerializeObject<U64_Reference<GAM_State>>(NextState, name: nameof(NextState))?.Resolve(s);
            ProhibitedTargetState = s.SerializeObject<U64_Reference<GAM_State>>(ProhibitedTargetState, name: nameof(ProhibitedTargetState))?.Resolve(s);

            TargetStatesCount = s.Serialize<ushort>(TargetStatesCount, name: nameof(TargetStatesCount));
            MechanicsIdCard.SerializeType(s);
            Repeat = s.Serialize<ushort>(Repeat, name: nameof(Repeat));
            Speed = s.Serialize<ushort>(Speed, name: nameof(Speed));
            TransitionStatusFlag = s.Serialize<byte>(TransitionStatusFlag, name: nameof(TransitionStatusFlag));
            CustomBits = s.Serialize<byte>(CustomBits, name: nameof(CustomBits));

            TargetStateList?.Resolve(s, TargetStatesCount);
            MechanicsIdCard?.Resolve(s);
        }
    }
}
