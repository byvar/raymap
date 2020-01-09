using OpenSpace;
using System;
using System.Collections;
using UnityEngine;

namespace OpenSpace.Object.Properties {
    public struct SuperObjectFlags {

        /*
         * 	NC	= 0	; no collision
        INV	= 1	; not visible
        NT	= 2	; no transform matrix (no scale and no rotation)
        ZOOM	= 3	; zoom instead of scale (same scale coefficient on the 3 axis)
        BVS	= 4	; bounding volume is a sphere instead of a box
        OVER	= 5	; displayed over all C - 0 ; non collisionnable
        NORAY	= 6	; cannot be hit by ray-tracing
        NOSHW	= 7	; cannot have a shadow projected on it
        SLOOK	= 8	; semi-lookat 
        ; 9  ; Super objet ayant un ou plusieurs fils dont les bounding volumes 
        ;      ne sont pas inclus dans celui du pere
        ; 15 ; objet sous l'influence d'un aimant (uniquement execution)
        ; 16 ; Activation de la transparence d'un module (uniquement execution)
        ; 17 ; Pas de calcul de lumière (uniquement execution)
        */

        public Flags flags;
        [Flags]
        public enum Flags {
            NoCollision = 1 << 0,
            Invisible = 1 << 1,
            NoTransformMatrix = 1 << 2, // No scale, no rotation
            ZoomInsteadOfScale = 1 << 3, // Same scale coëfficient over all 3 axes
            BoundingBoxInsteadOfSphere = 1 << 4,
            DisplayOnTop = 1 << 5, // displayed over all C - 0 ; non collisionnable. Superimposed
            NoRayTracing = 1 << 6,
            NoShadow = 1 << 7,
            SemiLookat = 1 << 8,
            SpecialBoundingVolumes = 1 << 9, // Super object has one or more children whose bounding volumes are not included in that of the father
            Flag10 = 1 << 10,
            Flag11 = 1 << 11,
            Flag12 = 1 << 12,
            Flag13 = 1 << 13,
            Flag14 = 1 << 14,
            InfluencedByMagnet = 1 << 15,
            Transparent = 1 << 16,
            NoLighting = 1 << 17,
            SuperimposedClipping = 1 << 18,
            OutlineMode = 1 << 19,
            Flag20 = 1 << 20,
            Flag21 = 1 << 21,
            Flag22 = 1 << 22,
            Flag23 = 1 << 23,
            Flag24 = 1 << 24,
            Flag25 = 1 << 25,
            Flag26 = 1 << 26,
            Flag27 = 1 << 27,
            Flag28 = 1 << 28,
            Flag29 = 1 << 29,
            Flag30 = 1 << 30,
            Flag31 = 1 << 31
        }

        public static SuperObjectFlags Read(Reader reader) {
            SuperObjectFlags soFlags = new SuperObjectFlags();
            soFlags.flags = (Flags)reader.ReadUInt32();
            return soFlags;
        }

        public void Write(Writer writer) {
            writer.Write((uint)flags);
        }

        public bool HasFlag(Flags flag) {
            return (flags & flag) == flag;
        }
    }
}