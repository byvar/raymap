using OpenSpace.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.ROM {
	public class Localization {
		public NumLanguages numLanguages;
		public LanguageTable[] languageTables = null;

		public async Task Read(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			l.loadingState = "Loading language tables";
			await MapLoader.WaitIfNecessary();
			NumLanguages numLanguages = l.GetOrRead<NumLanguages>(reader, 0);
			l.print("Number of languages: " + numLanguages.num_languages);
			languageTables = new LanguageTable[numLanguages.num_languages];
			for (ushort i = 0; i < numLanguages.num_languages; i++) {
				l.loadingState = "Loading language table " + (i + 1) + "/" + numLanguages.num_languages;
				await MapLoader.WaitIfNecessary();
				languageTables[i] = l.GetOrRead<LanguageTable>(reader, i);
				if (languageTables[i] != null) {
					l.print(languageTables[i].name);
				}
			}
		}

		public string Lookup(int index) {
			if (languageTables == null || languageTables.Length == 0) return null;
			if (index < 0) return null;
			LanguageTable table = languageTables[1];
			if (index >= table.num_txtTable + table.num_binaryTable) {
				index = index - (table.num_txtTable + table.num_binaryTable);
				table = languageTables[0]; // Common table
			}
			if (index < table.num_txtTable) {
				return table.textTable.Value.strings[index].Value.str.Value.str;
			} else if (index < table.num_txtTable + table.num_binaryTable) {
				return table.binaryTable.Value.strings[index - table.num_txtTable].Value.ToString();
			}
			return null;
		}
	}
}
