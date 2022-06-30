using System.Linq;

namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description : BinarySerializable, SNA_IDescription {
		public SNA_Description_Item[] Items { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Items = s.SerializeObjectArrayUntil<SNA_Description_Item>(Items,
				i =>
				(s.CurrentAbsoluteOffset >= s.CurrentLength - 3
				|| i.Type == SNA_DescriptionType.EndOfDescSection)
				, name: nameof(Items));
		}

		public string GetDirectory(SNA_DescriptionType type) {
			var directoryDesc = Items
				.FirstOrDefault(i => i.Type == SNA_DescriptionType.DirectoryDescTitle)?
				.Data as SNA_Description_Section;
			if(directoryDesc == null) return null;

			var str = directoryDesc.Items.FirstOrDefault(i => i.Type == type)?.Data as SNA_Description_String;
			if(str == null) return null;

			return str.Value;
		}

		public int? GetMapIndex(string mapName) {
			var levelNameDesc = Items
				.FirstOrDefault(i => i.Type == SNA_DescriptionType.LevelNameTitle)?
				.Data as SNA_Description_Section;
			if (levelNameDesc == null) return null;

			int mapIndex = 0;
			var lowerMapName = mapName.ToLowerInvariant();
			foreach (var item in levelNameDesc.Items) {
				if (item.Type != SNA_DescriptionType.LevelName) continue;
				var name = (SNA_Description_String)item.Data;
				if (name.Value.ToLowerInvariant() == lowerMapName) 
					return mapIndex;

				mapIndex++;
			}
			return null;
		}
	}
}
