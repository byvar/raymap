using Newtonsoft.Json;
using OpenSpace.AI;
using OpenSpace.Object;
using OpenSpace.Visual;
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
            public Dictionary<string, EDsgVar> Variables;
        }

        public struct EDsgVar {
            public DsgVarInfoEntry.DsgVarType type;
            public object value;
        }

        public List<EPerso> Persos;

        public ExportableScene(MapLoader loader)
        {
            Persos = new List<EPerso>();

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

                if (perso.brain?.mind != null && perso.brain.mind.dsgMem!=null && perso.brain.mind.dsgMem.dsgVar != null) {

                    Mind mind = perso.brain.mind;
                    DsgMem dsgMem = mind.dsgMem;

                    DsgVarInfoEntry[] dsgVars = mind.AI_model.dsgVar.dsgVarInfos;
                    Dictionary<string, EDsgVar> variables= new Dictionary<string, EDsgVar>();

                    foreach(DsgVarInfoEntry infoEntry in dsgVars) {

                         variables.Add("dsgVar_"+infoEntry.number, GetExportableDsgVar(infoEntry));
                    }

                    ePerso.Variables = variables;
                }
                
                if (perso.Gao!=null && perso.Gao.transform.parent!=null) {
                    GameObject parent = perso.Gao.transform.parent.gameObject;
                    PersoBehaviour pb = parent.GetComponent<PersoBehaviour>();
                    if (pb != null && pb.perso != null) {
                        ePerso.ParentPerso = pb.perso.namePerso;
                    }
                }

                Persos.Add(ePerso);
            }
        }

        public EDsgVar GetExportableDsgVar(DsgVarInfoEntry infoEntry)
        {
            EDsgVar d = new EDsgVar();
            d.type = infoEntry.type;

            if (infoEntry.value==null) {
                return d;
            }

            switch (infoEntry.type) {
                default:
                    d.value = infoEntry.value;
                    break;
                case DsgVarInfoEntry.DsgVarType.None:
                    break;
                case DsgVarInfoEntry.DsgVarType.List: // TODO: figure out lists
                    break;
                case DsgVarInfoEntry.DsgVarType.Comport: // TODO: comport
                    break;
                case DsgVarInfoEntry.DsgVarType.Action: // TODO: action
                    break;
                case DsgVarInfoEntry.DsgVarType.Input: // TODO: check if this works
                    //d.value = infoEntry.value
                    break;
                case DsgVarInfoEntry.DsgVarType.SoundEvent: // TODO: check
                    break;
                case DsgVarInfoEntry.DsgVarType.Light: // TODO: check
                    break;
                case DsgVarInfoEntry.DsgVarType.GameMaterial:
                    d.value = HashUtils.MD5Hash(GameMaterial.FromOffset((Pointer)(infoEntry.value)).ToJSON());
                    break;
                case DsgVarInfoEntry.DsgVarType.VisualMaterial:
                    d.value = HashUtils.MD5Hash(VisualMaterial.FromOffset((Pointer)(infoEntry.value)).ToJSON());
                    break;
                case DsgVarInfoEntry.DsgVarType.Perso:
                    d.value = Perso.FromOffset((Pointer)(infoEntry.value))?.namePerso;
                    break;
                case DsgVarInfoEntry.DsgVarType.Waypoint: // TODO
                    break;
                case DsgVarInfoEntry.DsgVarType.Graph: // TODO
                    break;
                case DsgVarInfoEntry.DsgVarType.Text: // TODO: check
                    goto default;
                case DsgVarInfoEntry.DsgVarType.SuperObject: // TODO: check
                    break;
                case DsgVarInfoEntry.DsgVarType.SOLinks: // TODO
                    break;
                case DsgVarInfoEntry.DsgVarType.PersoArray:

                    List<string> persoNames = new List<string>();
                    foreach(object persoPointer in (object[])infoEntry.value) {
                        if (persoPointer==null) {
                            continue;
                        }

                        if (!(persoPointer is Pointer)) {
                            persoNames.Add("Not a valid pointer: " + (persoPointer).ToString()); // TODO: fix
                            continue;
                        }

                        Perso perso = Perso.FromOffset((Pointer)persoPointer);
                        if (perso != null) {
                            persoNames.Add(perso.namePerso);
                        } else {
                            persoNames.Add("NullPointer");
                        }
                    }

                    break;
                case DsgVarInfoEntry.DsgVarType.WayPointArray: // TODO
                    break;
                case DsgVarInfoEntry.DsgVarType.TextArray: // TODO: check
                    goto default;
                case DsgVarInfoEntry.DsgVarType.TextRefArray: // TODO: check
                    goto default;
                case DsgVarInfoEntry.DsgVarType.Array6:
                    break;
                case DsgVarInfoEntry.DsgVarType.Array9:
                    break;
                case DsgVarInfoEntry.DsgVarType.SoundEventArray: // TODO: check
                    goto default;
                case DsgVarInfoEntry.DsgVarType.Array11:
                    break;
                case DsgVarInfoEntry.DsgVarType.Way:
                    break;
                case DsgVarInfoEntry.DsgVarType.ActionArray: // TODO                    
                    break;
                case DsgVarInfoEntry.DsgVarType.SuperObjectArray: // TODO
                    break;
            }

            return d;
        }

        public string ToJSON()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;
            settings.Converters.Add(new VisualMaterial.VisualMaterialReferenceJsonConverter());
            settings.Converters.Add(new GameMaterial.GameMaterialReferenceJsonConverter());
            settings.ContractResolver = JsonIgnorePointersResolver.Instance;
            return JsonConvert.SerializeObject(this, settings);
        }
    }
}
