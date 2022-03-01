using System.Collections.Generic;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class U64_DataFile : BinarySerializable {
		public U64_BinInfo BinInfo { get; set; }
		public U64_MainTable[] MainTables { get; set; }

		public Dictionary<U64_StructType, U64_MainTable> MainTablesDictionary { get; private set; }

		public override void SerializeImpl(SerializerObject s) {
			BinInfo = s.SerializeObject<U64_BinInfo>(BinInfo, name: nameof(BinInfo));
			s.DoWithDefaults(new SerializerDefaults() {
				PointerAnchor = Offset
			}, () => {
				MainTables = s.SerializeObjectArray<U64_MainTable>(MainTables, BinInfo.StructsCount, name: nameof(MainTables));
			});

			// Initialize MainTables dictionary
			CreateMainTablesDictionary(s.Context);

			if (s.GetCPASettings().Platform != Platform._3DS) {
				// For whatever reason these are stored using this struct table system instead of the main one
				MainTablesDictionary[U64_StructType.BitmapCI4].SerializeStructTable(s);
				MainTablesDictionary[U64_StructType.BitmapCI8].SerializeStructTable(s);
				MainTablesDictionary[U64_StructType.BitmapRGBA16].SerializeStructTable(s);
				if (s.GetCPASettings().Platform != Platform.N64) {
					MainTablesDictionary[U64_StructType.PaletteRGBA16].SerializeStructTable(s);
				}
			}
		}

		void CreateMainTablesDictionary(Context context) {
			MainTablesDictionary = new Dictionary<U64_StructType, U64_MainTable>();
			var dict = U64_StructType_Defines.GetTypeDictionary(context);
			foreach (var entry in dict) {
				if (entry.Key < MainTables.Length)
					MainTablesDictionary[entry.Value] = MainTables[entry.Key];
			}
		}
	}
}