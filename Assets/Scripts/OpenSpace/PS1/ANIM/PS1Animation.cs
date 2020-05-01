using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class PS1Animation : OpenSpaceStruct {
		public uint index;
		/*
			9 times {
				0x4 offset {
					0x2 * 5
					0x2 - unk
				}
				0x2 count
				0x2 unknown
			}
			after this, unk amount of times {
				0x4 offset {
					0x2 * 1
					0x2 - unk
				}
				0x2 count
				0x2 unknown
			}
		 */

		protected override void ReadInternal(Reader reader) {
			index = reader.ReadUInt32();
		}
	}
}
