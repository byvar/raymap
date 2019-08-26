using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.OpenSpace.Exporter {
    class AngleUtil {

        public enum RotSeq {
            zyx, zyz, zxy, zxz, yxz, yxy, yzx, yzy, xyz, xyx, xzy, xzx
        };

        static Vector3 twoaxisrot(float r11, float r12, float r21, float r31, float r32)
        {
            Vector3 ret = new Vector3();
            ret.x = Mathf.Atan2(r11, r12);
            ret.y = Mathf.Acos(r21);
            ret.z = Mathf.Atan2(r31, r32);
            return ret;
        }

        static Vector3 threeaxisrot(float r11, float r12, float r21, float r31, float r32)
        {
            Vector3 ret = new Vector3();
            ret.x = Mathf.Atan2(r31, r32);
            ret.y = Mathf.Asin(r21);
            ret.z = Mathf.Atan2(r11, r12);
            return ret;
        }

        public static Vector3 quaternion2Euler(Quaternion q, RotSeq rotSeq)
        {
            switch (rotSeq) {
                case RotSeq.zyx:
                    return threeaxisrot(2 * (q.x * q.y + q.w * q.z),
                        q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z,
                        -2 * (q.x * q.z - q.w * q.y),
                        2 * (q.y * q.z + q.w * q.x),
                        q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z);


                case RotSeq.zyz:
                    return twoaxisrot(2 * (q.y * q.z - q.w * q.x),
                        2 * (q.x * q.z + q.w * q.y),
                        q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                        2 * (q.y * q.z + q.w * q.x),
                        -2 * (q.x * q.z - q.w * q.y));


                case RotSeq.zxy:
                    return threeaxisrot(-2 * (q.x * q.y - q.w * q.z),
                        q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
                        2 * (q.y * q.z + q.w * q.x),
                        -2 * (q.x * q.z - q.w * q.y),
                        q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z);


                case RotSeq.zxz:
                    return twoaxisrot(2 * (q.x * q.z + q.w * q.y),
                        -2 * (q.y * q.z - q.w * q.x),
                        q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                        2 * (q.x * q.z - q.w * q.y),
                        2 * (q.y * q.z + q.w * q.x));


                case RotSeq.yxz:
                    return threeaxisrot(2 * (q.x * q.z + q.w * q.y),
                        q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                        -2 * (q.y * q.z - q.w * q.x),
                        2 * (q.x * q.y + q.w * q.z),
                        q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z);

                case RotSeq.yxy:
                    return twoaxisrot(2 * (q.x * q.y - q.w * q.z),
                        2 * (q.y * q.z + q.w * q.x),
                        q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
                        2 * (q.x * q.y + q.w * q.z),
                        -2 * (q.y * q.z - q.w * q.x));


                case RotSeq.yzx:
                    return threeaxisrot(-2 * (q.x * q.z - q.w * q.y),
                        q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z,
                        2 * (q.x * q.y + q.w * q.z),
                        -2 * (q.y * q.z - q.w * q.x),
                        q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z);


                case RotSeq.yzy:
                    return twoaxisrot(2 * (q.y * q.z + q.w * q.x),
                        -2 * (q.x * q.y - q.w * q.z),
                        q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
                        2 * (q.y * q.z - q.w * q.x),
                        2 * (q.x * q.y + q.w * q.z));


                case RotSeq.xyz:
                    return threeaxisrot(-2 * (q.y * q.z - q.w * q.x),
                        q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                        2 * (q.x * q.z + q.w * q.y),
                        -2 * (q.x * q.y - q.w * q.z),
                        q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z);


                case RotSeq.xyx:
                    return twoaxisrot(2 * (q.x * q.y + q.w * q.z),
                        -2 * (q.x * q.z - q.w * q.y),
                        q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z,
                        2 * (q.x * q.y - q.w * q.z),
                        2 * (q.x * q.z + q.w * q.y));


                case RotSeq.xzy:
                    return threeaxisrot(2 * (q.y * q.z + q.w * q.x),
                        q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
                        -2 * (q.x * q.y - q.w * q.z),
                        2 * (q.x * q.z + q.w * q.y),
                        q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z);


                case RotSeq.xzx:
                    return twoaxisrot(2 * (q.x * q.z - q.w * q.y),
                        2 * (q.x * q.y + q.w * q.z),
                        q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z,
                        2 * (q.x * q.z + q.w * q.y),
                        -2 * (q.x * q.y - q.w * q.z));

                default:
                    Debug.LogError("No good sequence");
                    return Vector3.zero;

            }

        }


    }
}
