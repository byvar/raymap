namespace BinarySerializer.Ubisoft.CPA {
    public enum SND_EventType : int {
		Invalid = 0,
		Play = 1,
		Stop = 2,
		StopAll = 3,
		StopAndGo = 4,
		StopAndGoCrossfade = 5,
		Pitch = 6,
		Volume = 7,
		Pan = 8,
		Effect = 9,
		Extra = 10,
		ChangeVolume = 11,
	}
}