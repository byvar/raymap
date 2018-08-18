using UnityEngine;
using UnityEditor;
namespace OpenSpace
{

    public class Dynamics // sizeof is 0x338, 0x1DC or 0x154 depending on type
    {
        public enum DynamicsType
        {
            Type0_SMALL, Type1_MEDIUM, Type2_BIG
        }

        // Raw data
        public Pointer offset;

        public uint field_0;
        public uint field_4;
        public uint field_8;
        public uint field_C;

        public Pointer off_speedVector;

        // Processed data
        public Matrix matrixA;
        public Matrix matrixB;

        public float rotX;
        public float rotY;
        public float rotZ;

        public Vector3 speedVector;

        public DynamicsType type;   

        public Dynamics(Pointer offset)
        {
            this.offset = offset;
        }

        public static Dynamics Read(Reader reader, Pointer offset)
        {
            Dynamics dynamics = new Dynamics(offset);
            
            // Read data

            dynamics.field_0 = reader.ReadUInt32();
            dynamics.field_4 = reader.ReadUInt32();
            dynamics.field_8 = reader.ReadUInt32();
            dynamics.field_C = reader.ReadUInt32();

            // Determine type
            int type;
            if ((dynamics.field_C & 4) != 0) {
                type = 2;
            } else if ((dynamics.field_C & 2) != 0) {
                type = 1;
            } else {
                type = 0;
            }

            dynamics.type = (DynamicsType)type;

            // Process data (common)

            Pointer.Goto(ref reader, offset + 0x54);
            dynamics.off_speedVector = Pointer.Read(reader);

            Pointer.Goto(ref reader, offset + 0x78);
            dynamics.matrixA = Matrix.Read(reader, offset + 0x78);

            Pointer.Goto(ref reader, offset + 0xD0);
            dynamics.matrixB = Matrix.Read(reader, offset + 0xD0);

            // Process data (type specific)

            if (dynamics.type == DynamicsType.Type0_SMALL) {

            } else if (dynamics.type == DynamicsType.Type1_MEDIUM) {

            } else if (dynamics.type == DynamicsType.Type2_BIG) {

                Pointer.Goto(ref reader, dynamics.off_speedVector);
                dynamics.speedVector = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            }

            return dynamics;
        }
    }

}