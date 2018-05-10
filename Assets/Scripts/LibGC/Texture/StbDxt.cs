// StbDxt.cs - Port of stb_dxt.h to C# - public domain
// stb_dxt.h - v1.04 - DXT1/DXT5 compressor - public domain
// original by fabian "ryg" giesen - ported to C by stb
// use '#define STB_DXT_IMPLEMENTATION' before including to create the implementation
//
// USAGE:
//   call stb_compress_dxt_block() for every block (you must pad)
//     source should be a 4x4 block of RGBA data in row-major order;
//     A is ignored if you specify alpha=0; you can turn on dithering
//     and "high quality" using mode.
//
// version history:
//   v1.04  - (ryg) default to no rounding bias for lerped colors (as per S3TC/DX10 spec);
//            single color match fix (allow for inexact color interpolation);
//            optimal DXT5 index finder; "high quality" mode that runs multiple refinement steps.
//   v1.03  - (stb) endianness support
//   v1.02  - (stb) fix alpha encoding bug
//   v1.01  - (stb) fix bug converting to RGB that messed up quality, thanks ryg & cbloom
//   v1.00  - (stb) first release
using System;

// configuration options for DXT encoder. set them in the project/makefile or just define
// them at the top.

// STB_DXT_USE_ROUNDING_BIAS
//     use a rounding bias during color interpolation. this is closer to what "ideal"
//     interpolation would do but doesn't match the S3TC/DX10 spec. old versions (pre-1.03)
//     implicitly had this turned on. 
//
//     in case you're targeting a specific type of hardware (e.g. console programmers):
//     NVidia and Intel GPUs (as of 2010) as well as DX9 ref use DXT decoders that are closer
//     to STB_DXT_USE_ROUNDING_BIAS. AMD/ATI, S3 and DX10 ref are closer to rounding with no bias.
//     you also see "(a*5 + b*3) / 8" on some old GPU designs.
// #define STB_DXT_USE_ROUNDING_BIAS

namespace LibGC.Texture
{
    /// <summary>DXT compression class.</summary>
    public static class StbDxt
    {
        /// <summary>compression mode (bitflags)</summary>
        [Flags]
        public enum CompressionMode
        {
            Normal = 0,
            /// <summary>use dithering. dubious win. never use for normal maps and the like!</summary>
            Dither = 1,
            /// <summary>high quality mode, does two refinement steps instead of 1. ~30-40% slower.</summary>
            HighQual = 2
        }

        static readonly byte[] Expand5 = new byte[32];
        static readonly byte[] Expand6 = new byte[64];
        static readonly byte[,] OMatch5 = new byte[256, 2];
        static readonly byte[,] OMatch6 = new byte[256, 2];
        static readonly byte[] QuantRBTab = new byte[256 + 16];
        static readonly byte[] QuantGTab = new byte[256 + 16];

        static int Mul8Bit(int a, int b)
        {
            int t = a*b + 128;
            return (t + (t >> 8)) >> 8;
        }

        static void From16Bit(byte[] outp, int outpIdx, ushort v)
        {
            int rv = (v & 0xf800) >> 11;
            int gv = (v & 0x07e0) >>  5;
            int bv = (v & 0x001f) >>  0;

            outp[outpIdx+0] = Expand5[rv];
            outp[outpIdx+1] = Expand6[gv];
            outp[outpIdx+2] = Expand5[bv];
            outp[outpIdx+3] = 0;
        }

        static ushort As16Bit(int r, int g, int b)
        {
            return (ushort)((Mul8Bit(r,31) << 11) + (Mul8Bit(g,63) << 5) + Mul8Bit(b,31));
        }

        /// <summary>linear interpolation at 1/3 point between a and b, using desired rounding type</summary>
        static int Lerp13(int a, int b)
        {
        #if STB_DXT_USE_ROUNDING_BIAS
           // with rounding bias
           return a + Mul8Bit(b-a, 0x55);
        #else
           // without rounding bias
           // replace "/ 3" by "* 0xaaab) >> 17" if your compiler sucks or you really need every ounce of speed.
           return (2*a + b) / 3;
        #endif
        }

        /// <summary>lerp RGB color</summary>
        static void Lerp13RGB(byte[] outp, int outpIdx, byte[] p1, int p1Idx, byte[] p2, int p2Idx)
        {
            outp[outpIdx+0] = (byte)Lerp13(p1[p1Idx+0], p2[p2Idx+0]);
            outp[outpIdx+1] = (byte)Lerp13(p1[p1Idx+1], p2[p2Idx+1]);
            outp[outpIdx+2] = (byte)Lerp13(p1[p1Idx+2], p2[p2Idx+2]);
        }

        /****************************************************************************/

        /// <summary>compute table to reproduce constant colors as accurately as possible</summary>
        static void PrepareOptTable(byte[,] Table, byte[] expand,int size)
        {
            int i,mn,mx;
            for (i=0;i<256;i++) {
                int bestErr = 256;
                for (mn=0;mn<size;mn++) {
                    for (mx=0;mx<size;mx++) {
                        int mine = expand[mn];
                        int maxe = expand[mx];
                        int err = Math.Abs(Lerp13(maxe, mine) - i);

                        // DX10 spec says that interpolation must be within 3% of "correct" result,
                        // add this as error term. (normally we'd expect a random distribution of
                        // +-1.5% error, but nowhere in the spec does it say that the error has to be
                        // unbiased - better safe than sorry).
                        err += Math.Abs(maxe - mine) * 3 / 100;

                        if(err < bestErr)
                        { 
                            Table[i,0] = (byte)mx;
                            Table[i,1] = (byte)mn;
                            bestErr = err;
                        }
                    }
                }
            }
        }

        static void EvalColors(byte[] color, int colorIdx, ushort c0, ushort c1)
        {
            From16Bit(color, colorIdx + 0, c0);
            From16Bit(color, colorIdx + 4, c1);
            Lerp13RGB(color, colorIdx + 8, color, colorIdx+0, color, colorIdx+4);
            Lerp13RGB(color, colorIdx +12, color, colorIdx+4, color, colorIdx+0);
        }

        /// <summary>
        /// Block dithering function. Simply dithers a block to 565 RGB.
        /// (Floyd-Steinberg)
        /// </summary>
        static void DitherBlock(byte[] dest, int destIdx, byte[] block, int blockIdx)
        {
            int[] err = new int[8];
            int ep1 = 0,ep2 = 4, et;
            int ch,y;

            // process channels seperately
            for (ch=0; ch<3; ++ch) {
                int bp = blockIdx+ch, dp = destIdx+ch;
                byte[] quant = (ch == 1) ? QuantGTab : QuantRBTab;
                Array.Clear(err, 0, err.Length);
                for(y=0; y<4; ++y) {
                    dest[dp+ 0] = quant[8+block[bp+ 0] + ((3*err[ep2+1] + 5*err[ep2+0]) >> 4)];
                    err[ep1+0] = block[bp+ 0] - dest[dp+ 0];
                    dest[dp+ 4] = quant[8+block[bp+ 4] + ((7*err[ep1+0] + 3*err[ep2+2] + 5*err[ep2+1] + err[ep2+0]) >> 4)];
                    err[ep1+1] = block[bp+ 4] - dest[dp+ 4];
                    dest[dp+ 8] = quant[8+block[bp+ 8] + ((7*err[ep1+1] + 3*err[ep2+3] + 5*err[ep2+2] + err[ep2+1]) >> 4)];
                    err[ep1+2] = block[bp+ 8] - dest[dp+ 8];
                    dest[dp+12] = quant[8+block[bp+12] + ((7*err[ep1+2] + 5*err[ep2+3] + err[ep2+2]) >> 4)];
                    err[ep1+3] = block[bp+12] - dest[dp+12];
                    bp += 16;
                    dp += 16;
                    et = ep1; ep1 = ep2; ep2 = et; // swap
                }
            }
        }

        /// <summary>The color matching function</summary>
        static uint MatchColorsBlock(byte[] block, int blockIdx, byte[] color, int colorIdx, bool dither)
        {
            uint mask = 0;
            int dirr = color[colorIdx+0*4+0] - color[colorIdx+1*4+0];
            int dirg = color[colorIdx+0*4+1] - color[colorIdx+1*4+1];
            int dirb = color[colorIdx+0*4+2] - color[colorIdx+1*4+2];
            int[] dots = new int[16];
            int[] stops = new int[4];
            int i;
            int c0Point, halfPoint, c3Point;

            for(i=0;i<16;i++)
                dots[i] = block[blockIdx+i*4+0]*dirr + block[blockIdx+i*4+1]*dirg + block[blockIdx+i*4+2]*dirb;

            for(i=0;i<4;i++)
                stops[i] = color[colorIdx+i*4+0]*dirr + color[colorIdx+i*4+1]*dirg + color[colorIdx+i*4+2]*dirb;

            // think of the colors as arranged on a line; project point onto that line, then choose
            // next color out of available ones. we compute the crossover points for "best color in top
            // half"/"best in bottom half" and then the same inside that subinterval.
            //
            // relying on this 1d approximation isn't always optimal in terms of euclidean distance,
            // but it's very close and a lot faster.
            // http://cbloomrants.blogspot.com/2008/12/12-08-08-dxtc-summary.html

            c0Point   = (stops[1] + stops[3]) >> 1;
            halfPoint = (stops[3] + stops[2]) >> 1;
            c3Point   = (stops[2] + stops[0]) >> 1;

            if(!dither) {
                // the version without dithering is straightforward
                for (i=15;i>=0;i--) {
                    int dot = dots[i];
                    mask <<= 2;

                    if(dot < halfPoint)
                        mask |= (uint)((dot < c0Point) ? 1 : 3);
                    else
                        mask |= (uint)((dot < c3Point) ? 2 : 0);
                }
            } else {
                // with floyd-steinberg dithering
                int[] err = new int[8];
                int ep1 = 0, ep2 = 4;
                int dp = 0, y;

                c0Point   <<= 4;
                halfPoint <<= 4;
                c3Point   <<= 4;
                for(i=0;i<8;i++)
                    err[i] = 0;

                for(y=0;y<4;y++)
                {
                    int dot,lmask,step;

                    dot = (dots[dp+0] << 4) + (3*err[ep2+1] + 5*err[ep2+0]);
                    if(dot < halfPoint)
                        step = (dot < c0Point) ? 1 : 3;
                    else
                        step = (dot < c3Point) ? 2 : 0;
                    err[ep1+0] = dots[dp+0] - stops[step];
                    lmask = step;

                    dot = (dots[dp+1] << 4) + (7*err[ep1+0] + 3*err[ep2+2] + 5*err[ep2+1] + err[ep2+0]);
                    if(dot < halfPoint)
                        step = (dot < c0Point) ? 1 : 3;
                    else
                        step = (dot < c3Point) ? 2 : 0;
                    err[ep1+1] = dots[dp+1] - stops[step];
                    lmask |= step<<2;

                    dot = (dots[dp+2] << 4) + (7*err[ep1+1] + 3*err[ep2+3] + 5*err[ep2+2] + err[ep2+1]);
                    if(dot < halfPoint)
                        step = (dot < c0Point) ? 1 : 3;
                    else
                        step = (dot < c3Point) ? 2 : 0;
                    err[ep1+2] = dots[dp+2] - stops[step];
                    lmask |= step<<4;

                    dot = (dots[dp+3] << 4) + (7*err[ep1+2] + 5*err[ep2+3] + err[ep2+2]);
                    if(dot < halfPoint)
                        step = (dot < c0Point) ? 1 : 3;
                    else
                        step = (dot < c3Point) ? 2 : 0;
                    err[ep1+3] = dots[dp+3] - stops[step];
                    lmask |= step<<6;

                    dp += 4;
                    mask |= (uint)(lmask << (y*8));
                    { int et = ep1; ep1 = ep2; ep2 = et; } // swap
                }
            }

            return mask;
        }

        // Constants for function OptimizeColorsBlock
        const int nIterPower = 4;

        /// <summary>The color optimization function. (Clever code, part 1)</summary>
        static void OptimizeColorsBlock(byte[] block, int blockIdx, out ushort pmax16, out ushort pmin16)
        {
            int mind = 0x7fffffff,maxd = -0x7fffffff;
            int minp = 0, maxp = 0; // Give them a value to avoid "unassigned variable warning"
            double magn;
            int v_r,v_g,v_b;
            float[] covf = new float[6];
            float vfr,vfg,vfb;

            // determine color distribution
            int[] cov = new int[6];
            int[] mu = new int[3],min = new int[3],max = new int[3];
            int ch,i,iter;

            for(ch=0;ch<3;ch++)
            {
                int bp = blockIdx+ch;
                int muv,minv,maxv;

                muv = minv = maxv = block[bp+0];
                for(i=4;i<64;i+=4)
                {
                    muv += block[bp+i];
                    if (block[bp+i] < minv) minv = block[bp+i];
                    else if (block[bp+i] > maxv) maxv = block[bp+i];
                }

                mu[ch] = (muv + 8) >> 4;
                min[ch] = minv;
                max[ch] = maxv;
            }

            // determine covariance matrix
            for (i=0;i<6;i++)
                cov[i] = 0;

            for (i=0;i<16;i++)
            {
                int r = block[blockIdx+i*4+0] - mu[0];
                int g = block[blockIdx+i*4+1] - mu[1];
                int b = block[blockIdx+i*4+2] - mu[2];

                cov[0] += r*r;
                cov[1] += r*g;
                cov[2] += r*b;
                cov[3] += g*g;
                cov[4] += g*b;
                cov[5] += b*b;
            }

            // convert covariance matrix to float, find principal axis via power iter
            for(i=0;i<6;i++)
                covf[i] = cov[i] / 255.0f;

            vfr = (float) (max[0] - min[0]);
            vfg = (float) (max[1] - min[1]);
            vfb = (float) (max[2] - min[2]);

            for(iter=0;iter<nIterPower;iter++)
            {
                float r = vfr*covf[0] + vfg*covf[1] + vfb*covf[2];
                float g = vfr*covf[1] + vfg*covf[3] + vfb*covf[4];
                float b = vfr*covf[2] + vfg*covf[4] + vfb*covf[5];

                vfr = r;
                vfg = g;
                vfb = b;
            }

            magn = Math.Abs(vfr);
            if (Math.Abs(vfg) > magn) magn = Math.Abs(vfg);
            if (Math.Abs(vfb) > magn) magn = Math.Abs(vfb);

            if(magn < 4.0f) { // too small, default to luminance
                v_r = 299; // JPEG YCbCr luma coefs, scaled by 1000.
                v_g = 587;
                v_b = 114;
            } else {
                magn = 512.0 / magn;
                v_r = (int) (vfr * magn);
                v_g = (int) (vfg * magn);
                v_b = (int) (vfb * magn);
            }

            // Pick colors at extreme points
            for(i=0;i<16;i++)
            {
                int dot = block[blockIdx+i*4+0]*v_r + block[blockIdx+i*4+1]*v_g + block[blockIdx+i*4+2]*v_b;

                if (dot < mind) {
                    mind = dot;
                    minp = blockIdx+i*4;
                }

                if (dot > maxd) {
                    maxd = dot;
                    maxp = blockIdx+i*4;
                }
            }

            pmax16 = As16Bit(block[maxp+0],block[maxp+1],block[maxp+2]);
            pmin16 = As16Bit(block[minp+0],block[minp+1],block[minp+2]);
        }

        static int sclamp(float y, int p0, int p1)
        {
            int x = (int) y;
            if (x < p0) return p0;
            if (x > p1) return p1;
            return x;
        }

        // Constants for function RefineBlock
        static readonly int[] w1Tab = new int[4] { 3,0,2,1 };
        static readonly int[] prods = new int[4] { 0x090000,0x000900,0x040102,0x010402 };
        // ^some magic to save a lot of multiplies in the accumulating loop...
        // (precomputed products of weights for least squares system, accumulated inside one 32-bit register)

        /// <summary>
        /// The refinement function. (Clever code, part 2)
        /// Tries to optimize colors to suit block contents better.
        /// (By solving a least squares system via normal equations+Cramer's rule)
        /// </summary>
        static bool RefineBlock(byte[] block, int blockIdx, ref ushort pmax16, ref ushort pmin16, uint mask)
        {
            float frb,fg;
            ushort oldMin, oldMax, min16, max16;
            int i, akku = 0, xx,xy,yy;
            int At1_r,At1_g,At1_b;
            int At2_r,At2_g,At2_b;
            uint cm = mask;

            oldMin = pmin16;
            oldMax = pmax16;

            if((mask ^ (mask<<2)) < 4) // all pixels have the same index?
            {
                // yes, linear system would be singular; solve using optimal
                // single-color match on average color
                int r = 8, g = 8, b = 8;
                for (i=0;i<16;++i) {
                    r += block[blockIdx+i*4+0];
                    g += block[blockIdx+i*4+1];
                    b += block[blockIdx+i*4+2];
                }

                r >>= 4; g >>= 4; b >>= 4;

                max16 = (ushort)((OMatch5[r,0]<<11) | (OMatch6[g,0]<<5) | OMatch5[b,0]);
                min16 = (ushort)((OMatch5[r,1]<<11) | (OMatch6[g,1]<<5) | OMatch5[b,1]);
            } else {
                At1_r = At1_g = At1_b = 0;
                At2_r = At2_g = At2_b = 0;
                for (i=0;i<16;++i,cm>>=2) {
                    int step = (int)(cm&3);
                    int w1 = w1Tab[step];
                    int r = block[blockIdx+i*4+0];
                    int g = block[blockIdx+i*4+1];
                    int b = block[blockIdx+i*4+2];

                    akku    += prods[step];
                    At1_r   += w1*r;
                    At1_g   += w1*g;
                    At1_b   += w1*b;
                    At2_r   += r;
                    At2_g   += g;
                    At2_b   += b;
                }

                At2_r = 3*At2_r - At1_r;
                At2_g = 3*At2_g - At1_g;
                At2_b = 3*At2_b - At1_b;

                // extract solutions and decide solvability
                xx = akku >> 16;
                yy = (akku >> 8) & 0xff;
                xy = (akku >> 0) & 0xff;

                frb = 3.0f * 31.0f / 255.0f / (xx*yy - xy*xy);
                fg = frb * 63.0f / 31.0f;

                // solve.
                max16 =   (ushort)(sclamp((At1_r*yy - At2_r*xy)*frb+0.5f,0,31) << 11);
                max16 |=  (ushort)(sclamp((At1_g*yy - At2_g*xy)*fg +0.5f,0,63) << 5);
                max16 |=  (ushort)(sclamp((At1_b*yy - At2_b*xy)*frb+0.5f,0,31) << 0);

                min16 =   (ushort)(sclamp((At2_r*xx - At1_r*xy)*frb+0.5f,0,31) << 11);
                min16 |=  (ushort)(sclamp((At2_g*xx - At1_g*xy)*fg +0.5f,0,63) << 5);
                min16 |=  (ushort)(sclamp((At2_b*xx - At1_b*xy)*frb+0.5f,0,31) << 0);
            }

            pmin16 = min16;
            pmax16 = max16;
            return oldMin != min16 || oldMax != max16;
        }

        // Color block compression
        static void CompressColorBlock(byte[] dest, int destIdx, byte[] block, int blockIdx, CompressionMode mode)
        {
            uint mask;
            int i;
            bool dither;
            int refinecount;
            ushort max16, min16;
            byte[] dblock = new byte[16*4],color = new byte[4*4];

            dither = (mode & CompressionMode.Dither) != 0;
            refinecount = ((mode & CompressionMode.HighQual) != 0) ? 2 : 1;

            // check if block is constant
            for (i=1;i<16;i++)
                if (block[blockIdx+i*4+0] != block[blockIdx+0] || block[blockIdx+i*4+1] != block[blockIdx+1] ||
                    block[blockIdx+i*4+2] != block[blockIdx+2] || block[blockIdx+i*4+3] != block[blockIdx+3])
                    break;

            if(i == 16) { // constant color
                int r = block[blockIdx+0], g = block[blockIdx+1], b = block[blockIdx+2];
                mask  = 0xaaaaaaaa;
                max16 = (ushort)((OMatch5[r,0]<<11) | (OMatch6[g,0]<<5) | OMatch5[b,0]);
                min16 = (ushort)((OMatch5[r,1]<<11) | (OMatch6[g,1]<<5) | OMatch5[b,1]);
            } else {
                // first step: compute dithered version for PCA if desired
                if(dither)
                    DitherBlock(dblock, 0, block, blockIdx);

                // second step: pca+map along principal axis
                OptimizeColorsBlock(dither ? dblock : block, dither ? 0 : blockIdx, out max16, out min16);
                if (max16 != min16) {
                    EvalColors(color, 0, max16,min16);
                    mask = MatchColorsBlock(block, blockIdx, color, 0, dither);
                } else
                    mask = 0;

                // third step: refine (multiple times if requested)
                for (i=0;i<refinecount;i++) {
                    uint lastmask = mask;

                    if (RefineBlock(dither ? dblock : block, dither ? 0 : blockIdx, ref max16, ref min16,mask)) {
                        if (max16 != min16) {
                            EvalColors(color, 0, max16,min16);
                            mask = MatchColorsBlock(block, blockIdx, color, 0, dither);
                        } else {
                            mask = 0;
                            break;
                        }
                    }

                    if(mask == lastmask)
                        break;
                }
            }

            // write the color block
            if(max16 < min16)
            {
                ushort t = min16;
                min16 = max16;
                max16 = t;
                mask ^= 0x55555555;
            }

            dest[destIdx+0] = (byte) (max16);
            dest[destIdx+1] = (byte) (max16 >> 8);
            dest[destIdx+2] = (byte) (min16);
            dest[destIdx+3] = (byte) (min16 >> 8);
            dest[destIdx+4] = (byte) (mask);
            dest[destIdx+5] = (byte) (mask >> 8);
            dest[destIdx+6] = (byte) (mask >> 16);
            dest[destIdx+7] = (byte) (mask >> 24);
        }

        /// <summary>
        /// Alpha block compression (this is easy for a change)
        /// </summary>
        static void CompressAlphaBlock(byte[] dest, int destIdx, byte[] src, int srcIdx, CompressionMode mode)
        {    
            int i,dist,bias,dist4,dist2,bits,mask;

            // find min/max color
            int mn,mx;
            mn = mx = src[srcIdx+3];

            for (i=1;i<16;i++)
            {
                if (src[srcIdx+i*4+3] < mn) mn = src[srcIdx+i*4+3];
                else if (src[srcIdx+i*4+3] > mx) mx = src[srcIdx+i*4+3];
            }

            // encode them
            dest[destIdx+0] = (byte)mx;
            dest[destIdx+1] = (byte)mn;
            destIdx += 2;

            // determine bias and emit color indices
            // given the choice of mx/mn, these indices are optimal:
            // http://fgiesen.wordpress.com/2009/12/15/dxt5-alpha-block-index-determination/
            dist = mx-mn;
            dist4 = dist*4;
            dist2 = dist*2;
            bias = (dist < 8) ? (dist - 1) : (dist/2 + 2);
            bias -= mn * 7;
            bits = 0; mask=0;

            for (i=0;i<16;i++) {
                int a = src[srcIdx+i*4+3]*7 + bias;
                int ind,t;

                // select index. this is a "linear scale" lerp factor between 0 (val=min) and 7 (val=max).
                t = (a >= dist4) ? -1 : 0; ind =  t & 4; a -= dist4 & t;
                t = (a >= dist2) ? -1 : 0; ind += t & 2; a -= dist2 & t;
                ind += (a >= dist) ? 1 : 0;

                // turn linear scale into DXT index (0/1 are extremal pts)
                ind = -ind & 7;
                ind ^= (2 > ind) ? 1 : 0;

                // write index
                mask |= ind << bits;
                if((bits += 3) >= 8) {
                    dest[destIdx++] = (byte)mask;
                    mask >>= 8;
                    bits -= 8;
                }
            }
        }

        static void InitDXT()
        {
            int i;
            for(i=0;i<32;i++)
                Expand5[i] = (byte)((i<<3)|(i>>2));

            for(i=0;i<64;i++)
                Expand6[i] = (byte)((i<<2)|(i>>4));

            for(i=0;i<256+16;i++)
            {
                int v = i-8 < 0 ? 0 : i-8 > 255 ? 255 : i-8;
                QuantRBTab[i] = Expand5[Mul8Bit(v,31)];
                QuantGTab[i] = Expand6[Mul8Bit(v,63)];
            }

            PrepareOptTable(OMatch5,Expand5,32);
            PrepareOptTable(OMatch6,Expand6,64);
        }

        static StbDxt()
        {
            InitDXT();
        }

        public static void CompressDxtBlock(byte[] dest, int destIdx, byte[] src, int srcIdx, bool alpha, CompressionMode mode)
        {
            if (alpha) {
                CompressAlphaBlock(dest, destIdx, src, srcIdx, mode);
                destIdx += 8;
            }

            CompressColorBlock(dest,destIdx, src, srcIdx, mode);
        }
    }
}

