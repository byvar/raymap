using Newtonsoft.Json;
using OpenSpace.Animation.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.ComponentMontreal {
    public class AnimChannelMontreal {
        public Pointer offset;
        public Pointer off_matrix;
        public uint isIdentity = 0;
        public byte objectIndex;
        public byte unk1;
        public short unk2;
        public short unk3;
        public byte unkByte1; // object index?
        public byte unkByte2;
        public uint unkUint;
        public Matrix matrix = null;

        public AnimChannelMontreal(Pointer offset) {
            this.offset = offset;
        }

        public static AnimChannelMontreal Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            AnimChannelMontreal ch = new AnimChannelMontreal(offset);
            ch.off_matrix = Pointer.GetPointerAtOffset(offset);
            ch.isIdentity = reader.ReadUInt32(); // if this is 1, don't check the pointer
            ch.objectIndex = reader.ReadByte();
            ch.unk1 = reader.ReadByte();
            ch.unk2 = reader.ReadInt16();
            ch.unk3 = reader.ReadInt16();
            ch.unkByte1 = reader.ReadByte();
            ch.unkByte2 = reader.ReadByte();
            ch.unkUint = reader.ReadUInt32();

            // Read compressed matrix
            if (ch.isIdentity != 1 && ch.isIdentity != 0) {
                Pointer.DoAt(ref reader, ch.off_matrix, () => {
                    ch.matrix = Matrix.ReadCompressed(reader, ch.off_matrix);
                });
            } /*else if (ch.isIdentity == 1) {
                ch.matrix = Matrix.Identity;
            }*/
            return ch;
        }

    }
}
