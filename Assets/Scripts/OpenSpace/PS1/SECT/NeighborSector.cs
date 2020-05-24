using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class NeighborSector : OpenSpaceStruct {
		public Pointer off_sectorSO;
		public short word04;
		public short word06;

		// Parsed
		public SuperObject sectorSO;
		public Sector Sector {
			get {
				OpenSpaceStruct data = sectorSO?.Data;
				if (data != null && data is Sector) {
					return data as Sector;
				}
				return null;
			}
		}

		protected override void ReadInternal(Reader reader) {
			off_sectorSO = Pointer.Read(reader);
			if (Settings.s.game != Settings.Game.DD) {
				word04 = reader.ReadInt16();
				word06 = reader.ReadInt16();
			}

			sectorSO = Load.FromOffsetOrRead<SuperObject>(reader, off_sectorSO);
		}
	}
}
