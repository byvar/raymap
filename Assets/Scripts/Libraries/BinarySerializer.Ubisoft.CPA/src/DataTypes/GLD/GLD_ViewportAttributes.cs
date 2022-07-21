namespace BinarySerializer.Ubisoft.CPA {
	public class GLD_ViewportAttributes : BinarySerializable {
		public Dimensions InitialDimensions { get; set; }
		public Dimensions CurrentDimensions { get; set; }
		public int BytesPerPixel { get; set; }
		public Rect RectInPixels { get; set; }
		public Rect RectInPixelsForClip { get; set; }
		public Dimensions DimensionsInPercent { get; set; }
		public Rect RectInPercent { get; set; }
		public Rect ClipRectInPixels { get; set; }
		public Rect ClipRectInPermille { get; set; }
		public int OffsetPosX { get; set; } // Offset of display window relative to upper left corner
		public int OffsetPosY { get; set; }
		public Pointer VirtualScreen { get; set; } // Pointer to back memory associated with viewport
		public int Pitch { get; set; } // Distance to start of next line
		public Pointer Reserved { get; set; }
		public Pointer SpecificTo2Dor3D { get; set; }
		public Pointer Reserved2 { get; set; }
		public short Device { get; set; }
		public short Viewport { get; set; }

		public float ScreenRatioX { get; set; }
		public float ScreenRatioY { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			InitialDimensions = s.SerializeObject<Dimensions>(InitialDimensions, name: nameof(InitialDimensions));
			CurrentDimensions = s.SerializeObject<Dimensions>(CurrentDimensions, name: nameof(CurrentDimensions));
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				BytesPerPixel = s.Serialize<int>(BytesPerPixel, name: nameof(BytesPerPixel));
			}
			RectInPixels = s.SerializeObject<Rect>(RectInPixels, name: nameof(RectInPixels));
			RectInPixelsForClip = s.SerializeObject<Rect>(RectInPixelsForClip, name: nameof(RectInPixelsForClip));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				DimensionsInPercent = s.SerializeObject<Dimensions>(DimensionsInPercent, name: nameof(DimensionsInPercent));
				ClipRectInPixels = s.SerializeObject<Rect>(ClipRectInPixels, name: nameof(ClipRectInPixels));
				ClipRectInPermille = s.SerializeObject<Rect>(ClipRectInPermille, name: nameof(ClipRectInPermille));
				OffsetPosX = s.Serialize<int>(OffsetPosX, name: nameof(OffsetPosX));
				OffsetPosY = s.Serialize<int>(OffsetPosY, name: nameof(OffsetPosY));
			} else {
				RectInPercent = s.SerializeObject<Rect>(RectInPercent, name: nameof(RectInPercent));
			}
			VirtualScreen = s.SerializePointer(VirtualScreen, name: nameof(VirtualScreen));
			Pitch = s.Serialize<int>(Pitch, name: nameof(Pitch));
			if(!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2))
				Reserved = s.SerializePointer(Reserved, name: nameof(Reserved));
			SpecificTo2Dor3D = s.SerializePointer(SpecificTo2Dor3D, allowInvalid: true, name: nameof(SpecificTo2Dor3D));
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2))
				Reserved2 = s.SerializePointer(Reserved2, name: nameof(Reserved2));
			Device = s.Serialize<short>(Device, name: nameof(Device));
			Viewport = s.Serialize<short>(Viewport, name: nameof(Viewport));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_Montreal)) {
				// TODO: also TT?
				ScreenRatioX = s.Serialize<float>(ScreenRatioX, name: nameof(ScreenRatioX));
				ScreenRatioY = s.Serialize<float>(ScreenRatioY, name: nameof(ScreenRatioY));
			}
		}

		public class Rect : BinarySerializable {
			public uint Top { get; set; }
			public uint Bottom { get; set; }
			public uint Left { get; set; }
			public uint Right { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Top = s.Serialize<uint>(Top, name: nameof(Top));
				Bottom = s.Serialize<uint>(Bottom, name: nameof(Bottom));
				Left = s.Serialize<uint>(Left, name: nameof(Left));
				Right = s.Serialize<uint>(Right, name: nameof(Right));
			}
		}
		public class Dimensions : BinarySerializable {
			public uint Height { get; set; }
			public uint Width { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Height = s.Serialize<uint>(Height, name: nameof(Height));
				Width = s.Serialize<uint>(Width, name: nameof(Width));
			}
		}
	}
}
