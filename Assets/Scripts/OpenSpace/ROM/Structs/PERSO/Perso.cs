using BinarySerializer.Unity;
using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class Perso : ROMStruct {
		// Size: 20
		public Reference<Perso3dData> p3dData;
		public Reference<Brain> brain;
		public Reference<CameraInfo> camera;
		public Reference<CollSet> collset;
		public ushort _8;
		public Reference<PersoMatrixAndVector> matrixAndVector;
		public Reference<StdGame> stdGame;
		public ushort _14;
		public ushort _16;
		public ushort _18;

		protected override void ReadInternal(Reader reader) {
            //Loader.print("Perso @ " + Offset);
            p3dData = new Reference<Perso3dData>(reader, true);
            brain = new Reference<Brain>(reader, true);
			camera = new Reference<CameraInfo>(reader, true);
			collset = new Reference<CollSet>(reader, true);
			_8 = reader.ReadUInt16();
			matrixAndVector = new Reference<PersoMatrixAndVector>(reader, true);
			stdGame = new Reference<StdGame>(reader, true);
			_14 = reader.ReadUInt16();
			_16 = reader.ReadUInt16();
			_18 = reader.ReadUInt16();
		}
		public ROMPersoBehaviour GetGameObject(GameObject gao) {
			/*string aiModelName = "Model_" + (brain.Value?.aiModel.Value?.IndexString ?? "null");
			gao.name = "[Family_" + stdGame.Value.family.Value.IndexString
				+ "] " + aiModelName +
				" | " + "Instance_" + IndexString + " - " + Offset;*/
			//gao.name += " - P3dData @ " + Offset;
			ROMPersoBehaviour romPerso = gao.AddComponent<ROMPersoBehaviour>();
			romPerso.perso = this;
			if (FileSystem.mode == FileSystem.Mode.Web) {
				gao.name = $"[{romPerso.NameFamily}] {romPerso.NameModel} | { romPerso.NameInstance}";
			} else {
				gao.name = $"[{romPerso.NameFamily}] {romPerso.NameModel} | { romPerso.NameInstance} - {Offset}";
			}
			romPerso.controller = MapLoader.Loader.controller;
			romPerso.controller.romPersos.Add(romPerso);
			//if (camera.Value != null) Loader.print(gao.name);

			return romPerso;
		}
	}
}
