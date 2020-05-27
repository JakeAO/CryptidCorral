using System;
using System.Linq;
using UAT_MS539.Core.Code.Extensions;

namespace UAT_MS539.Core.Code.Utility
{
    public class DropCalculation<T>
    {
        public class Point
        {
            public T Value;
            public uint Weight;

            public Point()
            {
                Value = default;
                Weight = 1;
            }
            public Point(T value, uint weight)
            {
                Value = value;
                Weight = weight;
            }
        }

        public Point[] Points;

        private uint TotalWeight => (uint) Points.Sum(x => x.Weight);

        public DropCalculation()
        {
            Points = new Point[0];
        }
        public DropCalculation(params Point[] points)
        {
            Points = points;
        }

        public T Evaluate(float percentage)
        {
            if (Points.Length == 0)
                return default;
            if (Points.Length == 1)
                return Points[0].Value;

            float target = percentage.Remap(0, 1, 0, TotalWeight);

            foreach (var point in Points)
            {
                if (target <= point.Weight)
                    return point.Value;
                target -= point.Weight;
            }

            return Points.Last().Value;
        }
    }
}