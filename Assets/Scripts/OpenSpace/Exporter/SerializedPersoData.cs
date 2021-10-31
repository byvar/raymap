using OpenSpace.AI;
using OpenSpace.Object;
using OpenSpace.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.Exporter {
    public class SerializedPersoData {

        public Dictionary<string, EPerso> Persos;

        public SerializedPersoData(List<Perso> persos)
        {
            Persos = new Dictionary<string, EPerso>();

            foreach (Perso perso in persos) {

                EPerso ePerso = new EPerso()
                {
                    Position = perso.Gao.transform.localPosition,
                    Rotation = perso.Gao.transform.localRotation.eulerAngles,
                    Scale = perso.Gao.transform.localScale,
                    Family = perso.p3dData?.family?.name,
                    AIModel = perso.brain?.mind?.AI_model.name
                };

                if (perso.brain?.mind != null && perso.brain.mind.dsgMem != null && perso.brain.mind.dsgMem.dsgVar != null) {

                    Mind mind = perso.brain.mind;
                    DsgMem dsgMem = mind.dsgMem;

                    DsgVarInfoEntry[] dsgVars = dsgMem.dsgVar.dsgVarInfos;
                    Dictionary<string, EDsgVar> variables = new Dictionary<string, EDsgVar>();

                    for(int i = 0; i < dsgMem.dsgVar.dsgVarInfos.Length; i++) {

                        variables.Add("dsgVar_" + i, GetExportableDsgVar(dsgMem,dsgMem.dsgVar,i));
                    }

                    ePerso.Variables = variables;
                }

                if (perso.Gao != null && perso.Gao.transform.parent != null) {
                    GameObject parent = perso.Gao.transform.parent.gameObject;
                    PersoBehaviour pb = parent.GetComponent<PersoBehaviour>();
                    if (pb != null && pb.perso != null) {
                        ePerso.ParentPerso = pb.perso.namePerso;
                    }
                }

                ePerso.StandardGame = new EStandardGame()
                {
                    CustomBits = perso.stdGame.customBits,
                    IsAlwaysActive = perso.stdGame.IsAlwaysActive,
                    IsMainActor = perso.stdGame.IsMainActor,
                    IsAPlatform = perso.stdGame.platformType!=0 ? true:false,
                    UpdateCheckByte = perso.stdGame.miscFlags,
                    TransparencyZoneMin = perso.stdGame.transparencyZoneMin,
                    TransparencyZoneMax = perso.stdGame.transparencyZoneMax,
                    TooFarLimit = perso.stdGame.tooFarLimit
                };

                if (!Persos.ContainsKey(perso.namePerso)) {
                    Persos.Add(perso.namePerso, ePerso);
                } else {
                    Debug.LogWarning("Warning: duplicate perso name " + perso.namePerso + ", ignoring duplicates...");
                }
            }
        }

        public EDsgVar GetExportableDsgVar(DsgMem dsgMem, DsgVar dsgVar, int infoIndex)
        {
            DsgVarInfoEntry infoEntry = dsgVar.dsgVarInfos[infoIndex];
            EDsgVar d = new EDsgVar();
            d.type = infoEntry.type;

            if (dsgMem.values == null || dsgMem.values[infoIndex] == null) {
                return d;
            }
            DsgVarValue val = dsgMem.values[infoIndex];
            d.value = GetExportableDsgVarValueString(val);

            return d;
        }

        public string GetExportableDsgVarValueString(DsgVarValue val) {
            string value = "";
            switch (val.type) {
                default:
                    value = val.ToString();
                    break;
                case DsgVarType.GameMaterial:
                    value = HashUtils.MD5Hash(val.valueGameMaterial?.ToJSON());
                    break;
                case DsgVarType.VisualMaterial:
                    value = HashUtils.MD5Hash(val.valueVisualMaterial?.ToJSON());
                    break;
                case DsgVarType.Perso:
                    value = val.valuePerso?.namePerso;
                    break;
                case DsgVarType.PersoArray:
                    List<string> persoNames = new List<string>();
                    foreach (DsgVarValue child in val.valueArray) {
                        Perso perso = child.valuePerso;
                        if (perso != null) {
                            persoNames.Add(perso.namePerso);
                        } else {
                            persoNames.Add("NullPointer");
                        }
                    }
                    value = "{ " + string.Join(", ", persoNames) + " }";
                    break;
                case DsgVarType.WayPointArray: // TODO
                    break;
                case DsgVarType.TextArray: // TODO: check
                    goto default;
                //break;
                case DsgVarType.TextRefArray: // TODO: check
                    goto default;
                case DsgVarType.GraphArray:
                    break;
                case DsgVarType.SOLinksArray:
                    break;
                case DsgVarType.SoundEventArray: // TODO: check
                    goto default;
                case DsgVarType.VisualMatArray:
                    break;
                case DsgVarType.Way:
                    break;
                case DsgVarType.ActionArray: // TODO                    
                    break;
                case DsgVarType.SuperObjectArray: // TODO
                    break;
            }

            return value;
        }

        public struct EPerso {
            public string ParentPerso;
            public Vector3 Position;
            public Vector3 Rotation;
            public Vector3 Scale;
            public string Family;
            public string AIModel;
            public Dictionary<string, EDsgVar> Variables;
            public EStandardGame StandardGame;
            //public StandardGame StandardGame;
        }

        public struct EStandardGame {
            public uint CustomBits;
            public bool IsAlwaysActive;
            public bool IsMainActor;
            public bool IsAPlatform;
            public byte UpdateCheckByte;
            public byte TransparencyZoneMin;
            public byte TransparencyZoneMax;
            public float TooFarLimit;
        }

        public struct EDsgVar {
            public DsgVarType type;
            public object value;
        }
    }
}
