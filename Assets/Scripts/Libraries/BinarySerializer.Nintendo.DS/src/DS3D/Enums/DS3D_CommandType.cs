using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Nintendo.DS {
    public enum DS3D_CommandType : byte {
		None = 0,
		MTX_MODE = 0x10,
		COLOR = 0x20,
		NORMAL = 0x21,
		TEXCOORD = 0x22,
		VTX_16 = 0x23,
		VTX_10 = 0x24,
		VTX_XY = 0x25,
		VTX_XZ = 0x26,
		VTX_YZ = 0x27,
		VTX_DIFF = 0x28,
		POLYGON_ATTR = 0x29,
		TEXIMAGE_PARAM = 0x2A,
		PLTT_BASE = 0x2B,
		DIF_AMB = 0x30,
		SPE_EMI = 0x31,
		LIGHT_VECTOR = 0x32,
		LIGHT_COLOR = 0x33,
		SHININESS = 0x34,
		BEGIN_VTXS = 0x40,
		END_VTXS = 0x41,
	}
}
