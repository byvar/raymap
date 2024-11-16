using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GenericExport.Model.DataBlocks
{
    public class ExportColor
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public ExportColor(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static ExportColor FromColor(Color color)
        {
            return new ExportColor(
                color.r, color.g, color.b, color.a
            );
        }
    }

    public class ExportTexture
    {
        public int width;
        public int height;
        public List<ExportColor> pixels = new List<ExportColor>();
    }
}
