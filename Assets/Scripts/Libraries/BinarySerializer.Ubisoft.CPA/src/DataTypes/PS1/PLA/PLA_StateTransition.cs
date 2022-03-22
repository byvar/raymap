namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class PLA_StateTransition : BinarySerializable
	{
		public Pointer TargetStatePointer { get; set; }
		public Pointer StateToGoPointer { get; set; }

		// Serialized from pointers
		public PLA_State TargetState { get; set; }
		public PLA_State StateToGo { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			TargetStatePointer = s.SerializePointer(TargetStatePointer, name: nameof(TargetStatePointer));
			StateToGoPointer = s.SerializePointer(StateToGoPointer, name: nameof(StateToGoPointer));

			// Serialize data from pointers
			s.DoAt(TargetStatePointer, () => TargetState = s.SerializeObject<PLA_State>(TargetState, name: nameof(TargetState)));
			s.DoAt(StateToGoPointer, () => StateToGo = s.SerializeObject<PLA_State>(StateToGo, name: nameof(StateToGo)));
		}
	}
}