using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.EngineObject {
    /// <summary>
    /// SuperObjects with type 1 are "worlds", which have no data but are used for hierarchy purposes.
    /// ex:
    /// - "actual world" (=everything)
    /// - "dynamic world" (=only dynamic IPOs and Persos)
    /// - "inactive dynamic world" (only ingame, visible in Unity as empty gameobject)
    /// - a world containing the sectors
    /// - a world containing the objects that remain loaded during transit, if transit exists
    /// </summary>
    public class World : IEngineObject {
        public string name = "World";
        private GameObject gao;
        public GameObject Gao {
            get {
                if (gao == null) {
                    gao = new GameObject(name);
                }
                return gao;
            }
        }

        private SuperObject superObject;
        public SuperObject SuperObject {
            get {
                return superObject;
            }
        }

        public World(SuperObject so) {
            this.superObject = so;
        }

        public static World New(SuperObject so) {
            return new World(so);
        }
    }
}
