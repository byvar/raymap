using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEditor;

namespace OpenSpace
{
    public class MechanicsIDCardFlags
    {
        public int rawFlags;
        public string flagPreview;

        public bool Animation           { get { return GetFlag(00); } set { SetFlag(00, value); } }
        public bool Collision           { get { return GetFlag(01); } set { SetFlag(01, value); } }
        public bool Gravity             { get { return GetFlag(02); } set { SetFlag(02, value); } }
        public bool Tilt                { get { return GetFlag(03); } set { SetFlag(03, value); } }
        public bool Gymnastics          { get { return GetFlag(04); } set { SetFlag(04, value); } }
        public bool OnGround            { get { return GetFlag(05); } set { SetFlag(05, value); } }
        public bool Climbing            { get { return GetFlag(06); } set { SetFlag(06, value); } }
        public bool Spider              { get { return GetFlag(07); } set { SetFlag(07, value); } }
        public bool Shoot               { get { return GetFlag(08); } set { SetFlag(08, value); } }
        public bool CollisionControl    { get { return GetFlag(09); } set { SetFlag(09, value); } }
        public bool KeepZVelocity       { get { return GetFlag(10); } set { SetFlag(10, value); } }
        public bool SpeedLimit          { get { return GetFlag(11); } set { SetFlag(11, value); } }
        public bool Inertia             { get { return GetFlag(12); } set { SetFlag(12, value); } }
        public bool Stream              { get { return GetFlag(13); } set { SetFlag(13, value); } }
        public bool StickOnPlatform     { get { return GetFlag(14); } set { SetFlag(14, value); } }
        public bool Scale               { get { return GetFlag(15); } set { SetFlag(15, value); } }
        public bool Flag16              { get { return GetFlag(16); } set { SetFlag(16, value); } }
        public bool Swim                { get { return GetFlag(17); } set { SetFlag(17, value); } }
        public bool Flag18              { get { return GetFlag(18); } set { SetFlag(18, value); } }
        public bool Flag19              { get { return GetFlag(19); } set { SetFlag(19, value); } }
        public bool Flag20              { get { return GetFlag(20); } set { SetFlag(20, value); } }
        public bool Flag21              { get { return GetFlag(21); } set { SetFlag(21, value); } }
        public bool Flag22              { get { return GetFlag(22); } set { SetFlag(22, value); } }
        public bool Flag23              { get { return GetFlag(23); } set { SetFlag(23, value); } }
        public bool Flag24              { get { return GetFlag(24); } set { SetFlag(24, value); } }
        public bool Flag25              { get { return GetFlag(25); } set { SetFlag(25, value); } }
        public bool Flag26              { get { return GetFlag(26); } set { SetFlag(26, value); } }
        public bool Flag27              { get { return GetFlag(27); } set { SetFlag(27, value); } }
        public bool Flag28              { get { return GetFlag(28); } set { SetFlag(28, value); } }
        public bool Flag29              { get { return GetFlag(29); } set { SetFlag(29, value); } }
        public bool Flag30              { get { return GetFlag(30); } set { SetFlag(30, value); } }
        public bool Flag31              { get { return GetFlag(31); } set { SetFlag(31, value); } }

        public void SetFlag(int index, bool value)
        {
            BitArray bitArray = new BitArray(BitConverter.GetBytes(this.rawFlags));
            bitArray.Set(index, value);
            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            this.rawFlags = array[0];
        }

        public bool GetFlag(int index)
        {
            BitArray bitArray = new BitArray(BitConverter.GetBytes(this.rawFlags));
            return bitArray.Get(index);
        }

        public void SetRawFlags(int rawFlags)
        {
            this.rawFlags = rawFlags;
            this.flagPreview = "";
            this.flagPreview += Animation ? "Animation," : "";
            this.flagPreview += Collision ? "Collision," : "";
            this.flagPreview += Gravity ? "Gravity," : "";
            this.flagPreview += Tilt ? "Tilt," : "";
            this.flagPreview += Gymnastics ? "Gymnastics," : "";
            this.flagPreview += OnGround ? "OnGround," : "";
            this.flagPreview += Climbing ? "Climbing," : "";
            this.flagPreview += Spider ? "Spider," : "";
            this.flagPreview += Shoot ? "Shoot," : "";
            this.flagPreview += CollisionControl ? "CollisionControl," : "";
            this.flagPreview += KeepZVelocity ? "KeepZVelocity," : "";
            this.flagPreview += SpeedLimit ? "SpeedLimit," : "";
            this.flagPreview += Inertia ? "Inertia," : "";
            this.flagPreview += Stream ? "Stream," : "";
            this.flagPreview += StickOnPlatform ? "StickOnPlatform," : "";
            this.flagPreview += Scale ? "Scale," : "";
            this.flagPreview += Flag16 ? "Flag16," : "";
            this.flagPreview += Swim ? "Swim," : "";
            this.flagPreview += Flag18 ? "Flag18," : "";
            this.flagPreview += Flag19 ? "Flag19," : "";
            this.flagPreview += Flag20 ? "Flag20," : "";
            this.flagPreview += Flag21 ? "Flag21," : "";
            this.flagPreview += Flag22 ? "Flag22," : "";
            this.flagPreview += Flag23 ? "Flag23," : "";
            this.flagPreview += Flag24 ? "Flag24," : "";
            this.flagPreview += Flag25 ? "Flag25," : "";
            this.flagPreview += Flag26 ? "Flag26," : "";
            this.flagPreview += Flag27 ? "Flag27," : "";
            this.flagPreview += Flag28 ? "Flag28," : "";
            this.flagPreview += Flag29 ? "Flag29," : "";
            this.flagPreview += Flag30 ? "Flag30," : "";
            this.flagPreview += Flag31 ? "Flag31," : "";
            if (this.flagPreview.Length > 0) {
                this.flagPreview = this.flagPreview.Substring(0, this.flagPreview.Length - 1);
            }
        }
    }
}
