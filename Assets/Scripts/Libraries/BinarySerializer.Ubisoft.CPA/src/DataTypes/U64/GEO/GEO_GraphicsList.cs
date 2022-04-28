using BinarySerializer.Nintendo;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GEO_GraphicsList : U64_Struct {
		public long Pre_Size { get; set; }

		public RSP_Command[] RSP_Commands { get; set; }
		public DS3D_CommandBlock[] DS3D_Commands { get; set; }
		public byte[] Bytes { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().Platform == Platform.N64) {
				// For N64: http://www.shootersforever.com/forums_message_boards/viewtopic.php?t=6920
				// Or RSP commands sheet
				// maybe this can help https://github.com/ricrpi/mupen64plus-video-gles2rice/blob/master/src/RSP_Parser.cpp
				// 8 bytes per command, 1st byte is RSP byte
				RSP_Commands = s.SerializeObjectArray<RSP_Command>(RSP_Commands, Pre_Size / RSP_Command.StructSize, name: nameof(RSP_Commands));
			} else {
				// For DS: https://github.com/scurest/apicula
				// http://problemkaputt.de/gbatek.htm#ds3dvideo check under Geometry Commands
				DS3D_Commands = s.SerializeObjectArrayUntil<DS3D_CommandBlock>(DS3D_Commands, _ => s.CurrentAbsoluteOffset >= Offset.AbsoluteOffset + Pre_Size, name: nameof(DS3D_Commands));
			}
		}
	}
}
