using Newtonsoft.Json;
using OpenSpace.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OpenSpace.Exporter {

    public class ExportableScene {

        public struct EPerso {
            public string Name;
            public string ParentPerso;
            public Vector3 Position;
            public Vector3 Rotation;
            public Vector3 Scale;
            public string Family;
            public string AIModel;
        }

        public List<EPerso> persos;

        public ExportableScene(MapLoader loader)
        {
            persos = new List<EPerso>();

            foreach (Perso perso in loader.persos) {

                EPerso ePerso = new EPerso()
                {
                    Position = perso.Gao.transform.localPosition,
                    Rotation = perso.Gao.transform.localRotation.eulerAngles,
                    Scale = perso.Gao.transform.localScale,
                    Family = perso.p3dData?.family?.name,
                    AIModel = perso.brain?.mind?.AI_model.name,
                    Name = perso.namePerso
                };
                
                if (perso.Gao!=null && perso.Gao.transform.parent!=null) {
                    GameObject parent = perso.Gao.transform.parent.gameObject;
                    PersoBehaviour pb = parent.GetComponent<PersoBehaviour>();
                    if (pb != null && pb.perso != null) {
                        ePerso.ParentPerso = pb.perso.namePerso;
                    }
                }

                persos.Add(ePerso);
            }
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
