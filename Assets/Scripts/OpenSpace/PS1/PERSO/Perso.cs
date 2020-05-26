using OpenSpace.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class Perso : OpenSpaceStruct { // Animation/state related
		public Pointer off_p3dData;
		public Pointer off_superObjectPointer;
		public Pointer off_08;
		public Pointer off_0C;
		public Pointer off_10;
		public Pointer off_14;

		// Parsed
		public Perso3dData p3dData;
		public string name;

		protected override void ReadInternal(Reader reader) {
			Load.print("Perso @ " + Offset);
			off_p3dData = Pointer.Read(reader);
			off_superObjectPointer = Pointer.Read(reader);
			off_08 = Pointer.Read(reader);
			off_0C = Pointer.Read(reader);
			off_10 = Pointer.Read(reader);
			off_14 = Pointer.Read(reader);
			//Load.print(off_00 + " - " + off_04 + " - " + off_08 + " - " + off_0C + " - " + off_10 + " - " + off_14);

			p3dData = Load.FromOffsetOrRead<Perso3dData>(reader, off_p3dData);
			Pointer.DoAt(ref reader, off_superObjectPointer, () => {
				Pointer off_superobject = Pointer.Read(reader);
				if (Settings.s.game == Settings.Game.RRush) {
					Pointer off_name = Pointer.Read(reader);
					Pointer.DoAt(ref reader, off_name, () => {
						name = reader.ReadNullDelimitedString();
					});
				} else {
					name = reader.ReadNullDelimitedString();
				}
				Load.print(Offset + " - " + off_superObjectPointer + " - " + off_superobject + " - " + name);
			});
			/*Pointer.DoAt(ref reader, off_00, () => {
				reader.ReadBytes(0x5c);
				ushort num_unk = reader.ReadUInt16();
				reader.ReadBytes(0xa);
				Pointer off_family = Pointer.Read(reader);
				Pointer.DoAt(ref reader, off_family, () => {
					for (int i = 0; i < num_unk; i++) {
						reader.ReadUInt16();
					}
					reader.ReadByte();
					byte unk = reader.ReadByte();
					if (unk != 0xFF) {
						string familyName = reader.ReadString(0x20);
						Load.print(familyName);
					}
				});
			});*/
		}

		public PS1PersoBehaviour GetGameObject(GameObject gao) {
			LevelHeader h = (Load as R2PS1Loader).levelHeader;
			gao.name = name + " | " + gao.name;
			if (p3dData?.family?.name != null) {
				gao.name = $"[{p3dData?.family?.name}] {gao.name}";
			}
			gao.name = gao.name;
			PS1PersoBehaviour romPerso = gao.AddComponent<PS1PersoBehaviour>();
			romPerso.perso = this;
			romPerso.controller = MapLoader.Loader.controller;
			romPerso.controller.ps1Persos.Add(romPerso);
			return romPerso;
		}
	}
}
