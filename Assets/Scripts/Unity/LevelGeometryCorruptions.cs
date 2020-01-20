using OpenSpace;
using OpenSpace.Collide;
using OpenSpace.Object;
using OpenSpace.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LevelGeometryCorruptor {

    public Controller controller;
    public List<PhysicalObject> pos;
    public Reader reader;
    public MapLoader l;
    Writer writer = null;
    float lastScrambled = 0;

    public LevelGeometryCorruptor(Controller controller)
    {
        this.controller = controller;

        lastScrambled = Time.time;
        l = MapLoader.Loader;

        pos = new List<PhysicalObject>();
        foreach (Sector s in l.sectors) {
            List<SuperObject> ipoSuperObjects = s.SuperObject.children.Where(c => c.type == SuperObject.Type.IPO).ToList();

            foreach(SuperObject spo in ipoSuperObjects) {
                pos.Add((spo.data as IPO).data);
            }
        }

        reader = MapLoader.Loader.livePreviewReader;

        for (int i = 0; i < l.files_array.Length; i++) {
            if (l.files_array[i] != null) l.files_array[i].CreateWriter();
        }
        writer = null;
        for (int i = 0; i < l.files_array.Length; i++) {
            if (l.files_array[i] != null) writer = l.files_array[i].writer;
        }
    }
    

    public void DoCorruptions()
    {
        if (Time.time-lastScrambled > 2.0f) {
            DoOneScramble();
        }
    }

    public void DoOneScramble()
    {

        List<Pointer> vertexOffsets = new List<Pointer>();

        foreach (PhysicalObject po in pos) {


            if (po.visualSet[0].obj is MeshObject) {

                var vmo = po.visualSet[0].obj as MeshObject;

                if (vmo != null) {
                   
                            for (int i = 0; i < vmo.num_vertices; i++) {
                                Pointer offset = vmo.off_vertices + i * 12;
                                if (!vertexOffsets.Contains(offset))
                                    vertexOffsets.Add(offset);
                            }

                      
                }
            }

            var cmo = po.collideMesh;

            if (cmo != null) {

                for (int i = 0; i < cmo.num_vertices; i++) {
                    Pointer offset = cmo.off_vertices + i * 12;
                    if (!vertexOffsets.Contains(offset))
                        vertexOffsets.Add(offset);
                }

            }
        }

        Dictionary<Pointer, float> floatsToWrite = new Dictionary<Pointer, float>();

        foreach (Pointer o in vertexOffsets) {
            Pointer.Goto(ref reader, o);

            Vector3 v = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            float groupFactor = 5.0f;
            int gx = (int)(v.x / groupFactor);
            int gy = (int)(v.y / groupFactor);
            int gz = (int)(v.z / groupFactor);

            int vectorSeed = (gx + ";" + gy + ";" + gz).GetHashCode();
            System.Random r = new System.Random(vectorSeed);
            System.Random r2 = new System.Random(vectorSeed + (int)(Time.time * 1000));

            if (r2.NextDouble() > 0.25f) {
                continue;
            }

            float changeFactor = 0.1f;

            Vector3 changedVertex = v + (new Vector3((float)r.NextDouble() - 0.5f, (float)r.NextDouble() - 0.5f, (float)r.NextDouble() - 0.5f) * changeFactor);

            floatsToWrite.Add(o + 0, changedVertex.x);
            floatsToWrite.Add(o + 4, changedVertex.y);
            floatsToWrite.Add(o + 8, changedVertex.z);
        }

        foreach (Pointer o in floatsToWrite.Keys) {

            Pointer.Goto(ref writer, o);
            writer.Write(floatsToWrite[o]);
        }

        floatsToWrite.Clear();

        // recalculate normals
        foreach (PhysicalObject po in pos) {

            var cmo = po.collideMesh;

            if (cmo != null) {

                Pointer off_current = Pointer.Goto(ref reader, cmo.off_vertices);
                cmo.vertices = new Vector3[cmo.num_vertices];
                for (int i = 0; i < cmo.num_vertices; i++) {
                    float x = reader.ReadSingle();
                    float z = reader.ReadSingle();
                    float y = reader.ReadSingle();
                    cmo.vertices[i] = new Vector3(x, y, z);
                }

                foreach (var sb in cmo.subblocks) {
                    if (sb is CollideMeshElement) {
                        var me = sb as CollideMeshElement;

                        for (int j = 0; j < me.num_triangles; j++) {

                            int vertOffset_0 = me.triangles[j * 3 + 0];
                            int vertOffset_1 = me.triangles[j * 3 + 1];
                            int vertOffset_2 = me.triangles[j * 3 + 2];

                            Vector3 pointA = cmo.vertices[vertOffset_0];
                            Vector3 pointB = cmo.vertices[vertOffset_1];
                            Vector3 pointC = cmo.vertices[vertOffset_2];

                            Pointer.Goto(ref reader, me.off_normals + j * 12 + 0);

                            Vector3 oldNormal = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                            Vector3 autoNormal = Vector3.Cross(pointB - pointA, pointC - pointA).normalized;

                            if (!floatsToWrite.ContainsKey(me.off_normals + j * 12)) {
                                floatsToWrite.Add(me.off_normals + j * 12 + 0, -autoNormal.x);
                                floatsToWrite.Add(me.off_normals + j * 12 + 4, -autoNormal.z);
                                floatsToWrite.Add(me.off_normals + j * 12 + 8, -autoNormal.y);
                            }
                        }
                    }
                }
            }
        }

        foreach (Pointer o in floatsToWrite.Keys) {

            Pointer.Goto(ref writer, o);
            writer.Write(floatsToWrite[o]);
        }
    }
}