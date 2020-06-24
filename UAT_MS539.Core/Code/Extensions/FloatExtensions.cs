namespace UAT_MS539.Core.Code.Extensions
{
    public static class FloatExtensions
    {
        public static float Remap(this float val, float fromA, float toA, float fromB, float toB)
        {
            return (val - fromA) / (toA - fromA) * (toB - fromB) + fromB;
        }

        public static float Remap(this double val, float fromA, float toA, float fromB, float toB)
        {
            return (float) ((val - fromA) / (toA - fromA) * (toB - fromB) + fromB);
        }
    }
}