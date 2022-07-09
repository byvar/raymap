using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPAScriptSerializer {
	/*
	#define SCR_CC_c_Cfg_NameSeparator          '^'
	#define SCR_CC_sz_Cfg_NameSeparator         "^"
	#define SCR_CC_c_Cfg_SectionBegMark         '{'
	#define SCR_CC_c_Cfg_SectionEndMark         '}'
	#define SCR_CC_c_Cfg_SectionIdMark          ':'
	#define SCR_CC_sz_Cfg_SectionIdMark         ":"
	#define SCR_CC_c_Cfg_DirectiveMark          '$'
	#define SCR_CC_c_Cfg_CommentMark            ';'
	#define SCR_CC_c_Cfg_CommentMark1           '#'
	#define SCR_CC_c_Cfg_ParamSeparator         ','
	#define SCR_CC_c_Cfg_FormatBegMark          '['
	#define SCR_CC_c_Cfg_PostFormatBegMark      ';'
	#define SCR_CC_c_Cfg_PostFormatEndMark      ';'
	#define SCR_CC_c_Cfg_FormatEndMark          ']'
	#define SCR_CC_c_Cfg_ParamBegMark           '('
	#define SCR_CC_c_Cfg_ParamEndMark           ')'
	#define SCR_CC_c_Cfg_StringMark             '"'
	#define SCR_CC_c_Cfg_VarMark                '@'
	#define SCR_CC_C_Cfg_NoChar                 '\0'
	#define SCR_CC_C_Cfg_EOL                    '\n'
	#define SCR_CC_C_Cfg_EmptyParameter         0x01

	 *  Format specifiers/
	 *  'F' for Format.
	#define SCR_CF_c_Cfg_ScanfSeparator         ','
	#define SCR_CF_c_Cfg_FormatArray            'a'
	#define SCR_CF_c_Cfg_FormatArrayByte        'c'
	#define SCR_CF_c_Cfg_FormatArrayShort       'w'
	#define SCR_CF_c_Cfg_FormatArrayLong        'l'
	#define SCR_CF_c_Cfg_FormatArrayInt         'i'
	#define SCR_CF_c_Cfg_FormatArrayFloat       'f'
	#define SCR_CF_c_Cfg_FormatArrayDouble      'd'
	#define SCR_CF_c_Cfg_FormatArrayBoolean     'b'
	#define SCR_CF_c_Cfg_FormatArrayDisEna      'e'
	#define SCR_CF_c_Cfg_FormatArrayReferences  'r'
	#define SCR_CF_c_Cfg_FormatScanf            '%'
	*/

	/// <summary>
	/// The SCR module in CPA allows for parsing and writing of scripts using a system of callbacks
	/// Each module can register callbacks
	/// Each line is read in SCR_Pars.c, fn_i_Pars_Line
	/// </summary>

	public abstract class CPAScript
	{

		public const char MarkSectionBegin = '{';
		public const char MarkSectionEnd = '}';
		public const char MarkSectionId = ':';
		public const char MarkDirective = '$';
		public const char MarkComment = ';';

		public List<CPAScriptItem> Items;

		public abstract CPAScriptSection GenerateSection(string sectionType, string sectionId);

		public void Read(Stream s)
		{
			using StreamReader reader = new StreamReader(s);
			while (!reader.EndOfStream) {
				ParseLine(reader.ReadLine(), reader);
			}
		}

		public void Write(Stream s)
		{
			using StreamWriter writer = new StreamWriter(s);

			foreach (var item in Items) {
				item.Write(writer);
			}

			writer.Flush();
			writer.Close();
		}

		private void ParseLine(string line, StreamReader reader)
		{
			if (string.IsNullOrWhiteSpace(line)) {
				return;
			}
			switch (line[0]) {
				case MarkSectionBegin:

					// A section has a type and an ID, e.g. {InputAction:ReinitTheMap
					int sectionIdChar = line.IndexOf(MarkSectionId);

					string sectionType = (sectionIdChar > 0) ? line[1 .. sectionIdChar] : line[1..];
					string sectionId = (sectionIdChar > 0) ? line[(sectionIdChar+1) ..] : string.Empty;

					// Read all the lines for this section in advance
					List<string> sectionLines = new List<string>();

					string sectionLine = reader.ReadLine();
					while (sectionLine != null && sectionLine[0] != MarkSectionEnd) {
						sectionLines.Add(sectionLine);
					}

					var section = GenerateSection(sectionType, sectionId);
					section.Parse(sectionLines.ToArray());

					break;

				case MarkDirective:
					var directive = new CPAScriptDirective();
					directive.Read(line);
					Items.Add(directive);

					break;
				case MarkComment:

					var comment = new CPAScriptComment();
					comment.Read(line);
					Items.Add(comment);

					break;
				default: break;
			}
		}
	}
}
