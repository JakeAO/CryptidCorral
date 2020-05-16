using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using UAT_MS539.Code.Extensions;

namespace UAT_MS539.Code.DataTypes
{
    public enum EPrimaryStat
    {
        Strength = 0,
        Speed = 1,
        Vitality = 2,
        Smarts = 3,
        Skill = 4,
        Luck = 5,

        _Count
    }

    public enum ESecondaryStat
    {
        Health = 0,
        Renown = 1,
        Morale = 2,
        Lifespan = 3,

        _Count
    }

    public class StatCalcDefinition
    {
        public readonly float Min;
        public readonly float Max;

        public StatCalcDefinition(float min, float max)
        {
            Min = Math.Min(min, max);
            Max = Math.Max(min, max);
        }

        public float Evaluate(float percentage)
        {
            return percentage.Remap(0, 1, Min, Max);
        }
    }

    public class CryptidDefinition
    {
        public readonly string Name;
        public readonly string ArtPath;
        public readonly StatCalcDefinition[] StartingStatFormulas = new StatCalcDefinition[(int)EPrimaryStat._Count];
        public readonly (PatternDefinition, float)[] PatternLikelihood = new (PatternDefinition, float)[0];
    }

    public class PatternDefinition
    {
        public readonly string Name;
        public readonly string ArtPath;
    }

    public class ColorDefinition
    {
        public readonly Vector3 RGB;
    }

    public class Cryptid
    {
        public readonly CryptidDefinition Definition;
        public readonly PatternDefinition Pattern;

        public Cryptid(CryptidDefinition definition, PatternDefinition pattern)
        {
            Definition = definition;
            Pattern = pattern;
        }
    }
}
