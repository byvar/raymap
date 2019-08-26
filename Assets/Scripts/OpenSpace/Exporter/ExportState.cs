using Newtonsoft.Json;
using OpenSpace;
using OpenSpace.Animation.Component;
using OpenSpace.Exporter;
using OpenSpace.Object;
using OpenSpace.Object.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.OpenSpace.Exporter {
    public class ExportState {

        public int animationLength;
        public int autoNextState = -1;

        public class ExportObjectInstance {
            public int familyObjectIndex;
            public int channelId;
            public bool[] visibilities;
        }

        public class ExportChannel {
            public Vector3[] positions;
            public Quaternion[] rotations;
            public Vector3[] scales;
        }

        public List<ExportObjectInstance> instances;
        public Dictionary<int, ExportChannel> channels;

        private ExportState() { }

        public static ExportState CreateFromState(State s)
        {
            Family fam = s.family;
            ExportState es = new ExportState();

            Perso examplePerso = MapLoader.Loader.persos.Find(p => p.p3dData.family == s.family);

            if (examplePerso == null) {
                return es;
            }

            examplePerso.Gao.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            examplePerso.Gao.transform.localScale = Vector3.one;
            PersoBehaviour pb = examplePerso.Gao.GetComponent<PersoBehaviour>();
            pb.SetState(s);
            if (pb.a3d == null) {
                return es;
            }
            es.animationLength = pb.a3d.num_onlyFrames;

            es.instances = new List<ExportObjectInstance>();
            es.channels = new Dictionary<int, ExportChannel>();

            for (int c = 0; c < pb.a3d.num_channels; c++) {
                es.channels.Add(c, new ExportChannel()
                {
                    positions = new Vector3[es.animationLength],
                    rotations = new Quaternion[es.animationLength],
                    scales = new Vector3[es.animationLength],
                });

                AnimChannel ch = pb.a3d.channels[pb.a3d.start_channels + c];
                List<ushort> listOfNTTOforChannel = new List<ushort>();
                for (int j = 0; j < pb.a3d.num_onlyFrames; j++) {
                    AnimOnlyFrame of = pb.a3d.onlyFrames[pb.a3d.start_onlyFrames + j];
                    //print(ch.numOfNTTO + " - " + of.numOfNTTO + " - " + a3d.numOfNTTO.Length);
                    AnimNumOfNTTO numOfNTTO = pb.a3d.numOfNTTO[ch.numOfNTTO + of.numOfNTTO];
                    if (!listOfNTTOforChannel.Contains(numOfNTTO.numOfNTTO)) {
                        listOfNTTOforChannel.Add(numOfNTTO.numOfNTTO);
                    }
                }

                for (int k = 0; k < listOfNTTOforChannel.Count; k++) {
                    int j = listOfNTTOforChannel[k] - pb.a3d.start_NTTO;
                    AnimNTTO ntto = pb.a3d.ntto[pb.a3d.start_NTTO + j];
                    if (ntto.IsInvisibleNTTO) {

                    } else {
                        es.instances.Add(new ExportObjectInstance() {
                            channelId = c,
                            familyObjectIndex = ntto.object_index,
                            visibilities = new bool[es.animationLength]
                        });
                    }
                }
            }

            for (uint i = 0; i < es.animationLength; i++) {
                pb.currentFrame = i;

                pb.UpdateAnimation();

                for (int c = 0; c < pb.a3d.num_channels; c++) {

                    if (pb.channelObjects[c].transform.childCount==0) {
                        Debug.LogWarning("ChannelObject " + pb.channelObjects[c] + " has no children :(");
                        continue;
                    }

                    Vector3 position = pb.channelObjects[c].transform.GetChild(0).position;
                    Quaternion rotation = pb.channelObjects[c].transform.GetChild(0).rotation;
                    Vector3 scale = pb.channelObjects[c].transform.GetChild(0).lossyScale;
                    bool visible = pb.channelObjects[c].transform.GetChild(0).gameObject.activeSelf;

                    //Vector3 exportRotation = AngleUtil.quaternion2Euler(rotation, AngleUtil.RotSeq.)

                    //Quaternion swappedRotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z);

                    es.channels[c].positions[i] = position;
                    es.channels[c].rotations[i] = rotation;
                    es.channels[c].scales[i] = scale;
                    //es.channels[c].visibilities[i][objectIndex] = visible ? true : false;

                    foreach (var instance in es.instances.Where(instance=> instance.channelId == c)) {

                        AnimChannel ch = pb.a3d.channels[pb.a3d.start_channels + c];
                        AnimOnlyFrame of = pb.a3d.onlyFrames[pb.a3d.start_onlyFrames + i];
                        AnimNumOfNTTO numOfNTTO = pb.a3d.numOfNTTO[ch.numOfNTTO + of.numOfNTTO];
                        AnimNTTO ntto = pb.a3d.ntto[numOfNTTO.numOfNTTO];

                        if (ntto.object_index != instance.familyObjectIndex) {
                            continue;
                        }
                        
                        //if (ntto.IsBoneNTTO) continue;
                        int poNum = numOfNTTO.numOfNTTO - pb.a3d.start_NTTO;
                        PhysicalObject physicalObject = pb.subObjects[c][poNum];

                        instance.visibilities[i] = physicalObject == null ? false : true;

                        //instance.familyObjectIndex;
                    }
                }


            }

            return es;
        }

        public string ToJSON()
        {
            JsonSerializerSettings settings = MapExporter.JsonExportSettings;

            settings.Converters.Add(new UnityConverter());
            settings.Converters.Add(new ObjectList.ObjectListReferenceJsonConverter());

            return JsonConvert.SerializeObject(this, settings);
        }
    }
}
