using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    /// <summary>
    /// SuperObjects with type 1 are "worlds", which have no data but are used for hierarchy purposes.
    /// ex:
    /// - "actual world" (=everything)
    /// - "dynamic world" (=only dynamic IPOs and Persos)
    /// - "inactive dynamic world" (only ingame, visible in Unity as empty gameobject)
    /// - a world containing the sectors
    /// - a world containing the objects that remain loaded during transit, if transit exists
    /// </summary>
    public class R3World : IR3Data {
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

        private R3SuperObject superObject;
        public R3SuperObject SuperObject {
            get {
                return superObject;
            }
        }

        public R3World(R3SuperObject so) {
            this.superObject = so;
        }

        public static R3World New(R3SuperObject so) {
            return new R3World(so);
        }
    }
}
