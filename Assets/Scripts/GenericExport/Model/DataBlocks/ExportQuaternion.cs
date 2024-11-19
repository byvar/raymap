using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GenericExport.Model.DataBlocks
{
    public class ExportQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public ExportQuaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static ExportQuaternion FromQuaternion(Quaternion quat)
        {
            return new ExportQuaternion(
                x: quat.x,
                y: quat.y,
                z: quat.z,
                w: quat.w
                );
        }

        public static ExportQuaternion RotationDifference(ExportQuaternion rotation1, ExportQuaternion rotation2)
        {
            return rotation1;
        }
    }
}
