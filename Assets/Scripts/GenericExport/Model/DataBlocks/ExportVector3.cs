using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GenericExport.Model.DataBlocks
{
    public class ExportVector3
    {
        public float x;
        public float y;
        public float z;

        public ExportVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static ExportVector3 FromVector3(Vector3 vec)
        {
            return new ExportVector3(
                x: vec.x,
                y: vec.y,
                z: vec.z
            );
        }

        public static ExportVector3 operator -(ExportVector3 v1, ExportVector3 v2)
        {
            return new ExportVector3(
                x: v1.x - v2.x,
                y: v1.y - v2.y,
                z: v1.z - v2.z
                );
        }
    }
}
