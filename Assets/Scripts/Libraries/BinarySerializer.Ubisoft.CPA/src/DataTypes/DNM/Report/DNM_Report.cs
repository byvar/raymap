namespace BinarySerializer.Ubisoft.CPA {
	public class DNM_Report : BinarySerializable {
		// Surface report
		public DNM_SurfaceState PreviousSurfaceState { get; set; }
		public DNM_SurfaceState CurrentSurfaceState { get; set; }

		// Obstacle report
		public DNM_Obstacle Obstacle { get; set; }
		public DNM_Obstacle Ground { get; set; }
		public DNM_Obstacle Wall { get; set; }
		public DNM_Obstacle Character { get; set; }
		public DNM_Obstacle Water { get; set; }
		public DNM_Obstacle Ceiling { get; set; }

		// Kinetic report (only for camera)
		public DNM_Move AbsolutePreviousSpeed { get; set; }
		public DNM_Move AbsoluteCurrentSpeed { get; set; }
		public DNM_Move AbsolutePreviousPosition { get; set; }
		public DNM_Move AbsoluteCurrentPosition { get; set; }

		public DNM_ReportFlags Flags { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			PreviousSurfaceState = s.Serialize<DNM_SurfaceState>(PreviousSurfaceState, name: nameof(PreviousSurfaceState));
			CurrentSurfaceState = s.Serialize<DNM_SurfaceState>(CurrentSurfaceState, name: nameof(CurrentSurfaceState));

			Obstacle = s.SerializeObject<DNM_Obstacle>(Obstacle, name: nameof(Obstacle));
			Ground = s.SerializeObject<DNM_Obstacle>(Ground, name: nameof(Ground));
			Wall = s.SerializeObject<DNM_Obstacle>(Wall, name: nameof(Wall));
			Character = s.SerializeObject<DNM_Obstacle>(Character, name: nameof(Character));
			Water = s.SerializeObject<DNM_Obstacle>(Water, name: nameof(Water));
			Ceiling = s.SerializeObject<DNM_Obstacle>(Ceiling, name: nameof(Ceiling));

			AbsolutePreviousSpeed = s.SerializeObject<DNM_Move>(AbsolutePreviousSpeed, name: nameof(AbsolutePreviousSpeed));
			AbsoluteCurrentSpeed = s.SerializeObject<DNM_Move>(AbsoluteCurrentSpeed, name: nameof(AbsoluteCurrentSpeed));
			AbsolutePreviousPosition = s.SerializeObject<DNM_Move>(AbsolutePreviousPosition, name: nameof(AbsolutePreviousPosition));
			AbsoluteCurrentPosition = s.SerializeObject<DNM_Move>(AbsoluteCurrentPosition, name: nameof(AbsoluteCurrentPosition));

			Flags = s.Serialize<DNM_ReportFlags>(Flags, name: nameof(Flags));
		}
	}
}
