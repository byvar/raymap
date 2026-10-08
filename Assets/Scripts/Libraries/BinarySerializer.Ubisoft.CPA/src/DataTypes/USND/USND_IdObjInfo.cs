namespace BinarySerializer.Ubisoft.CPA {
	public class USND_IdObjInfo : BinarySerializable {
		public uint Pre_Version { get; set; }
		public Pointer Pre_BaseOffset { get; set; }

		public string ClassName { get; set; }
		public USND_UUID Id { get; set; }
		public Pointer ObjPointer { get; set; }
		public uint ObjSize { get; set; }
		public bool Unknown { get; set; }
		public USND_IdObjInfoRefs References { get; set; }

		public string IdObjClassName { get; set; }
		public USND_IdObj IdObj { get; set; }
		public string IdObjClassName2 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ClassName = s.SerializeLengthPrefixedString<uint>(ClassName, name: nameof(ClassName));
			Id = s.SerializeObject<USND_UUID>(Id, name: nameof(Id));
			ObjPointer = s.SerializePointer(ObjPointer, anchor: Pre_BaseOffset, name: nameof(ObjPointer));
			ObjSize = s.Serialize<uint>(ObjSize, name: nameof(ObjSize));
			Unknown = s.SerializeBoolean<uint>(Unknown, name: nameof(Unknown));

			if (Pre_Version >= 2)
				References = s.SerializeObject<USND_IdObjInfoRefs>(References, x => x.Pre_Version = Pre_Version, name: nameof(References));

			s.DoAt(ObjPointer, () => {
				IdObjClassName = s.SerializeLengthPrefixedString<uint>(IdObjClassName, name: nameof(IdObjClassName));

				// TODO: Support remaining types
				switch (IdObjClassName) {
					case USND_EventResData.ClassName: SerializeObject<USND_EventResData>(s); break;
					case USND_RandomResData.ClassName: SerializeObject<USND_RandomResData>(s); break;
					case USND_PCWaveFileIdObj.ClassName: SerializeObject<USND_PCWaveFileIdObj>(s); break;
					case USND_PS2WaveFileIdObj.ClassName: SerializeObject<USND_PS2WaveFileIdObj>(s); break;
					case USND_PCWavResData.ClassName: SerializeObject<USND_PCWavResData>(s); break;
					case USND_PS2WavResData.ClassName: SerializeObject<USND_PS2WavResData>(s); break;
					default:
						s.SystemLogger?.LogWarning("Unsupported class type {0}", IdObjClassName);
						IdObj = s.SerializeObject<USND_PlaceholderIdObj>((USND_PlaceholderIdObj)IdObj, x => x.Pre_Length = ObjSize - (4 + IdObjClassName.Length + 8), name: nameof(IdObj));
						break;
				}
			});

			s.Log($"Object with ID {Id} and class {ClassName}");
		}

		private void SerializeObject<T>(SerializerObject s) where T : USND_IdObj, new() {
			IdObj = s.SerializeObject<T>((T)IdObj, name: nameof(IdObj));

			// Verify object size
			var serializedSize = s.CurrentPointer - ObjPointer;

			if (serializedSize != ObjSize) {
				s.SystemLogger?.LogWarning("{0}: Incorrect object length ({1} != {2})", ObjPointer, serializedSize, ObjSize);

				if (serializedSize < ObjSize)
					s.SerializeArray<byte>(null, ObjSize - serializedSize, name: "RemainingData");
			}
		}
	}
}