namespace BinarySerializer.Ubisoft.CPA.U64 {
    public class GAM_StateTransition : U64_Struct {
        public U64_Reference<GAM_State> TargetState { get; set; }
        public U64_Reference<GAM_State> StateToGo { get; set; }
        public ushort LinkingType { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            TargetState = s.SerializeObject<U64_Reference<GAM_State>>(TargetState, name: nameof(TargetState))?.Resolve(s);
            StateToGo = s.SerializeObject<U64_Reference<GAM_State>>(StateToGo, name: nameof(StateToGo))?.Resolve(s);
            LinkingType = s.Serialize<ushort>(LinkingType, name: nameof(LinkingType));
        }
    }
}
