using System;
using System.Collections.Generic;
using System.Text;

namespace UAT_MS539.Code.Extensions
{
    public static class FloatExtensions
    {
        public static float Remap(this float val, float fromA, float toA, float fromB, float toB)
        {
            return (val - fromA) / (toA - fromA) * (toB - fromB) + fromB;
        }
    }
}
