namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description_TT_Text : BinarySerializable {
		public uint MaxStringCount { get; set; }
		public File[] Files { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			MaxStringCount = s.Serialize<uint>(MaxStringCount, name: nameof(MaxStringCount));
			Files = s.SerializeObjectArrayUntil<File>(Files, f => f.IsLastFile, name: nameof(Files));
		}

		public class File : BinarySerializable {
			public SNA_Description_String FileName { get; set; }
			public Entry[] Entries { get; set; }

			public const string EndFile = "ENDTXT";
			public bool IsLastFile => FileName.Value == EndFile;
			
			public override void SerializeImpl(SerializerObject s) {
				FileName = s.SerializeObject<SNA_Description_String>(FileName, name: nameof(FileName));
				if (!IsLastFile) {
					Entries = s.SerializeObjectArrayUntil<Entry>(Entries, f => f.IsLastEntry, name: nameof(Files));
				}
			}
		}

		public class Entry : BinarySerializable {
			public SNA_Description_String Command { get; set; }

			public SNA_Description_String Key { get; set; }
			public SNA_Description_String StringValue { get; set; }
			public SNA_Description_String Type { get; set; }
			public uint StringLengthValue { get; set; }

			public const string EndCommand = "END";
			public const string NewStringLength = "NewStringLenght";
			public const string NewString = "NewString";
			public const string NewUpperString = "NewUpperString";
			public bool IsLastEntry => Command.Value == EndCommand;

			public override void SerializeImpl(SerializerObject s) {
				Command = s.SerializeObject<SNA_Description_String>(Command, name: nameof(Command));
				switch (Command.Value) {
					case NewString:
					case NewUpperString:
						Key = s.SerializeObject<SNA_Description_String>(Key, name: nameof(Key));
						StringValue = s.SerializeObject<SNA_Description_String>(StringValue, name: nameof(StringValue));
						Type = s.SerializeObject<SNA_Description_String>(Type, name: nameof(Type));
						break;
					case NewStringLength:
						Key = s.SerializeObject<SNA_Description_String>(Key, name: nameof(Key));
						StringLengthValue = s.Serialize<uint>(StringLengthValue, name: nameof(StringLengthValue));
						Type = s.SerializeObject<SNA_Description_String>(Type, name: nameof(Type));
						break;
					case EndCommand:
						break;
					default:
						throw new BinarySerializableException(this, $"Unimplemented command {Command.Value}");
				}
			}
		}
	}
}
