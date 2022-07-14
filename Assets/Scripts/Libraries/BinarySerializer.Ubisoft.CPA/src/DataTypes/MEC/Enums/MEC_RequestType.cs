namespace BinarySerializer.Ubisoft.CPA {
	/// <summary>
	/// From LRM_eKindOfRequest_, from Hype The Time Quest PS2
	/// </summary>
    public enum MEC_RequestType : uint {
		NoAction               = 0,
		TurnUp                 = 1,
		TurnDown               = 2,
		TurnLeft               = 3,
		TurnRight              = 4,
		TurnAbsolute           = 5,
		Jump                   = 6,
		JumpAbsolute           = 7,
		JumpWithoutAddingSpeed = 8,
		Accelerate             = 9,
		Pulse                  = 10,
		Brake                  = 11,
		GoRelative             = 12,
		GoAbsolute             = 13,
		Start                  = 14,
		Continue               = 15,
		TrackingDirection      = 16,
		ActionOnCamera         = 17,
		TurnAbsoluteInProgress = 18,
		GoTarget               = 19,
		GoInDirection          = 20,
		Fire                   = 21,
		ReachTarget            = 22,
		TurnAround             = 23,
		BePushed               = 24,
		MoveLateralLeft        = 25,
		MoveLateralRight       = 26,
		NotValid               = 0xffffffff,
	}
}