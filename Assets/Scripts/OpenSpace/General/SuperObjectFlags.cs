using System;
using System.Collections;
using UnityEngine;

public class SuperObjectFlags : MonoBehaviour
{

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

    public int rawFlags;
    public string flagPreview;
    
    public bool NoCollision                 { get { return GetFlag(00); } set { SetFlag(00, value); } }
    public bool Invisible                   { get { return GetFlag(01); } set { SetFlag(01, value); } }
    public bool NoTransformMatrix           { get { return GetFlag(02); } set { SetFlag(02, value); } } // No scale, no rotation
    public bool ZoomInsteadOfScale          { get { return GetFlag(03); } set { SetFlag(03, value); } } // Same scale coëfficient over all 3 axes
    public bool BoundingBoxInsteadOfSphere  { get { return GetFlag(04); } set { SetFlag(04, value); } }
    public bool DisplayOnTop                { get { return GetFlag(05); } set { SetFlag(05, value); } } // displayed over all C - 0 ; non collisionnable
    public bool NoRayTracing                { get { return GetFlag(06); } set { SetFlag(06, value); } }
    public bool NoShadow                    { get { return GetFlag(07); } set { SetFlag(07, value); } }
    public bool SemiLookat                  { get { return GetFlag(08); } set { SetFlag(08, value); } }
    public bool SpecialBoundingVolumes      { get { return GetFlag(09); } set { SetFlag(09, value); } } // Super object has one or more children whose bounding volumes are not included in that of the father
    public bool Flag10                      { get { return GetFlag(10); } set { SetFlag(10, value); } }
    public bool Flag11                      { get { return GetFlag(11); } set { SetFlag(11, value); } }
    public bool Flag12                      { get { return GetFlag(12); } set { SetFlag(12, value); } }
    public bool Flag13                      { get { return GetFlag(13); } set { SetFlag(13, value); } }
    public bool Flag14                      { get { return GetFlag(14); } set { SetFlag(14, value); } }
    public bool InfluencedByMagnet          { get { return GetFlag(15); } set { SetFlag(15, value); } }
    public bool Transparent                 { get { return GetFlag(16); } set { SetFlag(16, value); } }
    public bool NoLighting                  { get { return GetFlag(17); } set { SetFlag(17, value); } }
    public bool Flag18                      { get { return GetFlag(18); } set { SetFlag(18, value); } }
    public bool Flag19                      { get { return GetFlag(19); } set { SetFlag(19, value); } }
    public bool Flag20                      { get { return GetFlag(20); } set { SetFlag(20, value); } }
    public bool Flag21                      { get { return GetFlag(21); } set { SetFlag(21, value); } }
    public bool Flag22                      { get { return GetFlag(22); } set { SetFlag(22, value); } }
    public bool Flag23                      { get { return GetFlag(23); } set { SetFlag(23, value); } }
    public bool Flag24                      { get { return GetFlag(24); } set { SetFlag(24, value); } }
    public bool Flag25                      { get { return GetFlag(25); } set { SetFlag(25, value); } }
    public bool Flag26                      { get { return GetFlag(26); } set { SetFlag(26, value); } }
    public bool Flag27                      { get { return GetFlag(27); } set { SetFlag(27, value); } }
    public bool Flag28                      { get { return GetFlag(28); } set { SetFlag(28, value); } }
    public bool Flag29                      { get { return GetFlag(29); } set { SetFlag(29, value); } }
    public bool Flag30                      { get { return GetFlag(30); } set { SetFlag(30, value); } }
    public bool Flag31                      { get { return GetFlag(31); } set { SetFlag(31, value); } }

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
        this.flagPreview += NoCollision                ? "NoCollision," : "";
        this.flagPreview += Invisible                  ? "Invisible," : "";
        this.flagPreview += NoTransformMatrix          ? "NoTransformMatrix," : "";
        this.flagPreview += ZoomInsteadOfScale         ? "ZoomInsteadOfScale," : "";
        this.flagPreview += BoundingBoxInsteadOfSphere ? "BoundingSphereInsteadOfBox," : "";
        this.flagPreview += DisplayOnTop               ? "DisplayOnTop," : "";
        this.flagPreview += NoRayTracing               ? "NoRayTracing," : "";
        this.flagPreview += NoShadow                   ? "NoShadow," : "";
        this.flagPreview += SemiLookat                 ? "SemiLookat," : "";
        this.flagPreview += SpecialBoundingVolumes     ? "SpecialBoundingVolumes," : "";
        this.flagPreview += Flag10                     ? "Flag10," : "";
        this.flagPreview += Flag11                     ? "Flag11," : "";
        this.flagPreview += Flag12                     ? "Flag12," : "";
        this.flagPreview += Flag13                     ? "Flag13," : "";
        this.flagPreview += Flag14                     ? "Flag14," : "";
        this.flagPreview += InfluencedByMagnet         ? "InfluencedByMagnet," : "";
        this.flagPreview += Transparent                ? "Transparent," : "";
        this.flagPreview += NoLighting                 ? "NoLighting," : "";
        this.flagPreview += Flag18                     ? "Flag18," : "";
        this.flagPreview += Flag19                     ? "Flag19," : "";
        this.flagPreview += Flag20                     ? "Flag20," : "";
        this.flagPreview += Flag21                     ? "Flag21," : "";
        this.flagPreview += Flag22                     ? "Flag22," : "";
        this.flagPreview += Flag23                     ? "Flag23," : "";
        this.flagPreview += Flag24                     ? "Flag24," : "";
        this.flagPreview += Flag25                     ? "Flag25," : "";
        this.flagPreview += Flag26                     ? "Flag26," : "";
        this.flagPreview += Flag27                     ? "Flag27," : "";
        this.flagPreview += Flag28                     ? "Flag28," : "";
        this.flagPreview += Flag29                     ? "Flag29," : "";
        this.flagPreview += Flag30                     ? "Flag30," : "";
        this.flagPreview += Flag31                     ? "Flag31," : "";
        if (this.flagPreview.Length>0) {
            this.flagPreview = this.flagPreview.Substring(0, this.flagPreview.Length - 1);
        }
    }
}
 