namespace UAT_MS539.Core.Code.Extensions
{
    public static class FloatExtensions
    {
        public static float Remap(this float val, float fromA, float toA, float fromB, float toB)
        {
            return (val - fromA) / (toA - fromA) * (toB - fromB) + fromB;
        }
    }
}