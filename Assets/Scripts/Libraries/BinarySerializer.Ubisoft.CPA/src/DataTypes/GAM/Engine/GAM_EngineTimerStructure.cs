namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_EngineTimerStructure : BinarySerializable {
		public uint FrameNumber { get; set; }
		public short TimerHandle { get; set; }
		public uint CurrentTimerCount { get; set; } // Time spent in current frame in ms
		public uint DeltaTimerCount { get; set; } // Between two frames in ms
		public uint[] Counters { get; set; }
		public uint UsefulDeltaTime { get; set; } // Computed time in ms
		public uint PauseTime { get; set; }

		public float FrameLength { get; set; } // In seconds
		public TimerCount RealTimeCount { get; set; }
		public TimerCount PauseTimeCount { get; set; }
		public uint TickPerMS { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			FrameNumber = s.Serialize<uint>(FrameNumber, name: nameof(FrameNumber));
			TimerHandle = s.Serialize<short>(TimerHandle, name: nameof(TimerHandle));
			s.Align(4, Offset);
			CurrentTimerCount = s.Serialize<uint>(CurrentTimerCount, name: nameof(CurrentTimerCount));
			DeltaTimerCount = s.Serialize<uint>(DeltaTimerCount, name: nameof(DeltaTimerCount));
			Counters = s.SerializeArray<uint>(Counters, 16, name: nameof(Counters));
			UsefulDeltaTime = s.Serialize<uint>(UsefulDeltaTime, name: nameof(UsefulDeltaTime));
			PauseTime = s.Serialize<uint>(PauseTime, name: nameof(PauseTime));

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				FrameLength = s.Serialize<float>(FrameLength, name: nameof(FrameLength));
				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
					s.Align(8, Offset); // Because the following struct has 8-byte values
				}
				RealTimeCount = s.SerializeObject<TimerCount>(RealTimeCount, name: nameof(RealTimeCount));
				PauseTimeCount = s.SerializeObject<TimerCount>(PauseTimeCount, name: nameof(PauseTimeCount));
				TickPerMS = s.Serialize<uint>(TickPerMS, name: nameof(TickPerMS));

				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
					s.Align(8, Offset);
				}
			}
		}

		public class TimerCount : BinarySerializable {
			public ulong LowPart { get; set; }
			public ulong HighPart { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
					LowPart = s.Serialize<ulong>(LowPart, name: nameof(LowPart));
					HighPart = s.Serialize<ulong>(HighPart, name: nameof(HighPart));
				} else {
					LowPart = s.Serialize<uint>((uint)LowPart, name: nameof(LowPart));
					HighPart = s.Serialize<uint>((uint)HighPart, name: nameof(HighPart));
				}
			}
		}
	}
}
