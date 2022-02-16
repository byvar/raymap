﻿using BinarySerializer.Unity;
using OpenSpace.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class Perso : OpenSpaceStruct { // Animation/state related
		public LegacyPointer off_p3dData;
		public LegacyPointer off_superObjectPointer;
		public LegacyPointer off_08; // dynamics
		public LegacyPointer off_0C;
		public LegacyPointer off_collSet;
		public LegacyPointer off_sectorSuperObjectPointer;

		// Parsed
		public Perso3dData p3dData;
		public CollSet collSet;
		public string name;

		protected override void ReadInternal(Reader reader) {
			//Load.print("Perso @ " + Offset);
			off_p3dData = LegacyPointer.Read(reader);
			off_superObjectPointer = LegacyPointer.Read(reader);
			off_08 = LegacyPointer.Read(reader); // Probably Dynamics!
			off_0C = LegacyPointer.Read(reader); // points to struct of 0x18 size
			off_collSet = LegacyPointer.Read(reader);
			off_sectorSuperObjectPointer = LegacyPointer.Read(reader); // points to 4 bytes: 00000000. Only filled in at runtime
			//Load.print(off_00 + " - " + off_04 + " - " + off_08 + " - " + off_0C + " - " + off_10 + " - " + off_14);

			p3dData = Load.FromOffsetOrRead<Perso3dData>(reader, off_p3dData);
			LegacyPointer.DoAt(ref reader, off_superObjectPointer, () => {
				LegacyPointer off_superobject = LegacyPointer.Read(reader);
				if (Legacy_Settings.s.game == Legacy_Settings.Game.RRush) {
					LegacyPointer off_name = LegacyPointer.Read(reader);
					LegacyPointer.DoAt(ref reader, off_name, () => {
						name = reader.ReadNullDelimitedString();
					});
				} else {
					name = reader.ReadNullDelimitedString();
				}
				Load.print("Perso:" + Offset + " - SO:" + off_superobject + " - " + name);
				//Load.print(Offset + " - " + off_08 + " - " + off_collSet + " - " + name);
			});
			collSet = Load.FromOffsetOrRead<CollSet>(reader, off_collSet);
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
			if (FileSystem.mode == FileSystem.Mode.Web) {
				gao.name = name;
			} else {
				gao.name = name + " | " + gao.name;
			}
			if (p3dData?.family?.name != null) {
				gao.name = $"[{p3dData?.family?.name}] {gao.name}";
			}
			PS1PersoBehaviour romPerso = gao.AddComponent<PS1PersoBehaviour>();
			romPerso.perso = this;
			romPerso.controller = MapLoader.Loader.controller;
			romPerso.controller.ps1Persos.Add(romPerso);
			return romPerso;
		}
	}
}
