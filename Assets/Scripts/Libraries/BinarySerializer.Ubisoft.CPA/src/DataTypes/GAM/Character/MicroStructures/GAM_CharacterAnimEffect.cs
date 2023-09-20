namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_CharacterAnimEffect : BinarySerializable {
		public MTH3D_Vector ShiftPhase { get; set; } // these describe the sinus phase of a module displacement along the anim's axis
		public MTH3D_Vector ShiftMax { get; set; }   // relative to the position given by the current frame.
		public MTH3D_Vector ShiftPlus { get; set; }  // there are time shifts that apply between two consecutive modules, so that they dont always do the same thing together

		public override void SerializeImpl(SerializerObject s) {
			ShiftPhase = s.SerializeObject<MTH3D_Vector>(ShiftPhase, name: nameof(ShiftPhase));
			ShiftMax = s.SerializeObject<MTH3D_Vector>(ShiftMax, name: nameof(ShiftMax));
			ShiftPlus = s.SerializeObject<MTH3D_Vector>(ShiftPlus, name: nameof(ShiftPlus));
		}
	}
}
