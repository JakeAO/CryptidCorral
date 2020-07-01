using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Code.Cryptid;
using Core.Code.Extensions;
using Core.Code.Utility;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class SpeciesGenerator
    {
        private enum ColorCategory
        {
            Nil,
            Red,
            Blue,
            Green,
            Purple,
            Orange,
            Grey
        }

        private readonly Dictionary<ColorCategory, string[]> _colorDefinitions = new Dictionary<ColorCategory, string[]>()
        {
            {ColorCategory.Nil, new[] {"nil"}},
            {ColorCategory.Red, new[] {"red", "lightRed", "darkRed"}},
            {ColorCategory.Blue, new[] {"blue", "lightBlue", "darkBlue"}},
            {ColorCategory.Green, new[] {"green", "lightGreen", "darkGreen"}},
            {ColorCategory.Purple, new[] {"purple", "lightPurple", "darkPurple"}},
            {ColorCategory.Orange, new[] {"orange", "lightOrange", "darkOrange"}},
            {ColorCategory.Grey, new[] {"white", "grey", "black"}},
        };

        private enum StatSpread
        {
            LowStable,
            LowNormal,
            LowWide,
            MediumStable,
            MediumNormal,
            MediumWide,
            HighStable,
            HighNormal,
            HighWide,
        }

        private class StatRange
        {
            public uint Min;
            public uint Max;
            public uint Spread;
        }

        private readonly Dictionary<StatSpread, StatRange> _statRanges = new Dictionary<StatSpread, StatRange>()
        {
            {StatSpread.LowStable, new StatRange(){ Min = 8, Max = 16, Spread = 5}},
            {StatSpread.LowNormal, new StatRange(){ Min = 6, Max = 18, Spread = 9}},
            {StatSpread.LowWide, new StatRange(){ Min = 4, Max = 20, Spread = 13}},
            {StatSpread.MediumStable, new StatRange(){ Min = 16, Max = 32, Spread = 5}},
            {StatSpread.MediumNormal, new StatRange(){ Min = 12, Max = 36, Spread = 9}},
            {StatSpread.MediumWide, new StatRange(){ Min = 8, Max = 40, Spread = 13}},
            {StatSpread.HighStable, new StatRange(){ Min = 36, Max = 60, Spread = 5}},
            {StatSpread.HighNormal, new StatRange(){ Min = 30, Max = 66, Spread = 9}},
            {StatSpread.HighWide, new StatRange(){ Min = 24, Max = 72, Spread = 13}},
        };
        
        private class Config
        {
            public StatSpread[] Stats = new[] {StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal};
            public (ColorCategory, uint)[] Colors = new[] {(ColorCategory.Nil, 1u)};
            public int SpawnRateModifier = 0;
        }

        private readonly Config[] _speciesConfigs = new Config[]
        {
            new Config() // 00
            {
                Stats = new[] {StatSpread.LowWide, StatSpread.LowStable, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Red, 3u)}
            },
            new Config() // 01
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.MediumNormal, StatSpread.LowWide, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 3u), (ColorCategory.Grey, 3u)}
            },
            new Config() // 02
            {
                Stats = new[] {StatSpread.LowWide, StatSpread.MediumNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.MediumNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Green, 3u), (ColorCategory.Blue, 3u), (ColorCategory.Red, 3u)}
            },
            new Config() // 03
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.MediumStable, StatSpread.HighStable, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Green, 6u), (ColorCategory.Purple, 3u)}
            },
            new Config() // 04
            {
                Stats = new[] {StatSpread.HighWide, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Grey, 9u), (ColorCategory.Orange, 3u)}
            },
            new Config() // 05
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.MediumWide, StatSpread.LowNormal, StatSpread.HighNormal, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 6u), (ColorCategory.Purple, 3u)}
            },
            new Config() // 06
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowStable, StatSpread.HighWide, StatSpread.LowStable, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 9u), (ColorCategory.Red, 3u)}
            },
            new Config() // 07
            {
                Stats = new[] {StatSpread.MediumWide, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.MediumWide, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Grey, 9u), (ColorCategory.Purple, 3u)}
            },
            new Config() // 08
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.MediumWide, StatSpread.LowNormal, StatSpread.HighWide, StatSpread.LowNormal, StatSpread.LowWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Green, 9u), (ColorCategory.Purple, 3u)}
            },
            new Config() // 09
            {
                Stats = new[] {StatSpread.MediumWide, StatSpread.LowNormal, StatSpread.MediumStable, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 9u), (ColorCategory.Grey, 3u)}
            },
            new Config() // 10
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.MediumWide, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighStable, StatSpread.MediumWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Grey, 12u), (ColorCategory.Orange, 3u), (ColorCategory.Purple, 3u)}
            },
            new Config() // 11
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighWide, StatSpread.LowNormal, StatSpread.HighWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 3u), (ColorCategory.Purple, 3u)}
            },
            new Config() // 12
            {
                Stats = new[] {StatSpread.LowStable, StatSpread.HighWide, StatSpread.LowStable, StatSpread.LowStable, StatSpread.HighWide, StatSpread.LowStable},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Blue, 12u), (ColorCategory.Green, 6u)}
            },
            new Config() // 13
            {
                Stats = new[] {StatSpread.HighWide, StatSpread.LowStable, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighWide, StatSpread.LowStable},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Grey, 12u), (ColorCategory.Orange, 6u)}
            },
            new Config() // 14
            {
                Stats = new[] {StatSpread.MediumWide, StatSpread.LowNormal, StatSpread.LowWide, StatSpread.LowNormal, StatSpread.MediumWide, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Grey, 6u), (ColorCategory.Red, 12u)}
            },
            new Config() // 15
            {
                Stats = new[] {StatSpread.MediumWide, StatSpread.LowNormal, StatSpread.MediumWide, StatSpread.LowStable, StatSpread.LowStable, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Green, 6u), (ColorCategory.Red, 3u), (ColorCategory.Blue, 3u)}
            },
            new Config() // 16
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighWide, StatSpread.LowNormal, StatSpread.HighWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Grey, 12u), (ColorCategory.Red, 3u), (ColorCategory.Purple, 3u)}
            },
            new Config() // 17
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.MediumWide, StatSpread.LowNormal, StatSpread.MediumWide, StatSpread.MediumWide, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Grey, 12u)}
            },
            new Config() // 18
            {
                Stats = new[] {StatSpread.HighWide, StatSpread.HighWide, StatSpread.MediumWide, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 6u), (ColorCategory.Grey, 6u)}
            },
            new Config() // 19
            {
                Stats = new[] {StatSpread.LowWide, StatSpread.LowStable, StatSpread.HighWide, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.MediumStable},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 12u), (ColorCategory.Green, 6u), (ColorCategory.Red, 3u)}
            },
            new Config() // 20
            {
                Stats = new[] {StatSpread.MediumWide, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighWide, StatSpread.HighWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Blue, 6u), (ColorCategory.Red, 6u), (ColorCategory.Green, 6u)}
            },
            new Config() // 21
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.MediumWide, StatSpread.LowNormal, StatSpread.MediumWide, StatSpread.LowNormal, StatSpread.MediumWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Purple, 12u), (ColorCategory.Blue, 6u), (ColorCategory.Red, 3u)}
            },
            new Config() // 22
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.HighWide, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Grey, 12u), (ColorCategory.Purple, 6u)}
            },
            new Config() // 23
            {
                Stats = new[] {StatSpread.MediumWide, StatSpread.LowWide, StatSpread.MediumStable, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Green, 3u), (ColorCategory.Red, 3u), (ColorCategory.Blue, 3u)}
            },
            new Config() // 24
            {
                Stats = new[] {StatSpread.HighStable, StatSpread.LowNormal, StatSpread.MediumStable, StatSpread.LowNormal, StatSpread.MediumStable, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Purple, 3u), (ColorCategory.Red, 3u)}
            },
            new Config() // 25
            {
                Stats = new[] {StatSpread.MediumStable, StatSpread.MediumStable, StatSpread.MediumStable, StatSpread.LowStable, StatSpread.LowStable, StatSpread.LowStable},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Grey, 3u), (ColorCategory.Orange, 3u), (ColorCategory.Red, 3u)}
            },
            new Config() // 26
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighWide},
                Colors = new[] {(ColorCategory.Nil, 1u)},
                SpawnRateModifier = 5
            },
            new Config() // 27
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowStable, StatSpread.MediumStable, StatSpread.LowStable, StatSpread.MediumStable, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Blue, 3u), (ColorCategory.Green, 3u), (ColorCategory.Orange, 3u)}
            },
            new Config() // 28
            {
                Stats = new[] {StatSpread.HighStable, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighStable, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Red, 3u), (ColorCategory.Purple, 3u)}
            },
            new Config() // 29
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighWide, StatSpread.HighWide, StatSpread.HighWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Purple, 12u), (ColorCategory.Grey, 6u), (ColorCategory.Blue, 3u)}
            },
            new Config() // 30
            {
                Stats = new[] {StatSpread.MediumWide, StatSpread.LowNormal, StatSpread.MediumStable, StatSpread.LowStable, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 6u), (ColorCategory.Red, 6u), (ColorCategory.Purple, 6u)}
            },
            new Config() // 31
            {
                Stats = new[] {StatSpread.MediumStable, StatSpread.LowWide, StatSpread.HighWide, StatSpread.LowStable, StatSpread.LowStable, StatSpread.LowStable},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 12u), (ColorCategory.Green, 3u)}
            },
            new Config() // 32
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.HighWide, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighWide, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Purple, 6u), (ColorCategory.Orange, 6u), (ColorCategory.Red, 3u)}
            },
            new Config() // 33
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighWide, StatSpread.HighWide, StatSpread.HighWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Grey, 12u), (ColorCategory.Green, 6u)}
            },
            new Config() // 34
            {
                Stats = new[] {StatSpread.MediumWide, StatSpread.MediumStable, StatSpread.MediumWide, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Purple, 6u), (ColorCategory.Blue, 3u), (ColorCategory.Red, 3u)}
            },
            new Config() // 35
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.MediumWide, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.MediumStable, StatSpread.MediumStable},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Grey, 12u), (ColorCategory.Red, 3u), (ColorCategory.Blue, 3u), (ColorCategory.Orange, 3u)}
            },
            new Config() // 36
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowStable, StatSpread.MediumStable, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.MediumWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 6u), (ColorCategory.Green, 6u)}
            },
            new Config() // 37
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.MediumStable, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.MediumWide, StatSpread.MediumWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Purple, 12u), (ColorCategory.Orange, 6u)}
            },
            new Config() // 38
            {
                Stats = new[] {StatSpread.HighStable, StatSpread.LowNormal, StatSpread.HighStable, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Red, 6u), (ColorCategory.Blue, 6u)}
            },
            new Config() // 39
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.HighStable, StatSpread.LowNormal, StatSpread.HighStable, StatSpread.LowNormal, StatSpread.HighStable},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Red, 6u), (ColorCategory.Blue, 6u), (ColorCategory.Green, 6u)}
            },
            new Config() // 40
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.HighWide, StatSpread.LowNormal, StatSpread.LowStable, StatSpread.LowWide, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 12u), (ColorCategory.Grey, 3u)}
            },
            new Config() // 41
            {
                Stats = new[] {StatSpread.LowWide, StatSpread.MediumWide, StatSpread.MediumWide, StatSpread.LowStable, StatSpread.LowNormal, StatSpread.MediumStable},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Purple, 12u), (ColorCategory.Orange, 12u)}
            },
            new Config() // 42
            {
                Stats = new[] {StatSpread.LowStable, StatSpread.LowStable, StatSpread.MediumStable, StatSpread.LowStable, StatSpread.LowStable, StatSpread.HighStable},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Green, 3u), (ColorCategory.Blue, 3u), (ColorCategory.Red, 3u), (ColorCategory.Purple, 3u)},
                SpawnRateModifier = 5
            },
            new Config() // 43
            {
                Stats = new[] {StatSpread.MediumStable, StatSpread.MediumWide, StatSpread.MediumStable, StatSpread.LowNormal, StatSpread.MediumStable, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Red, 6u), (ColorCategory.Blue, 6u), (ColorCategory.Purple, 6u)}
            },
            new Config() // 44
            {
                Stats = new[] {StatSpread.MediumStable, StatSpread.LowNormal, StatSpread.HighWide, StatSpread.LowNormal, StatSpread.HighWide, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Purple, 6u), (ColorCategory.Blue, 6u), (ColorCategory.Orange, 6u)}
            },
            new Config() // 45
            {
                Stats = new[] {StatSpread.MediumWide, StatSpread.LowStable, StatSpread.HighWide, StatSpread.LowStable, StatSpread.LowStable, StatSpread.LowStable},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Grey, 6u), (ColorCategory.Red, 6u), (ColorCategory.Green, 6u)}
            },
            new Config() // 46
            {
                Stats = new[] {StatSpread.HighWide, StatSpread.MediumStable, StatSpread.LowNormal, StatSpread.LowStable, StatSpread.MediumStable, StatSpread.LowStable},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 6u), (ColorCategory.Red, 6u), (ColorCategory.Green, 3u)}
            },
            new Config() // 47
            {
                Stats = new[] {StatSpread.LowWide, StatSpread.MediumWide, StatSpread.LowWide, StatSpread.MediumWide, StatSpread.MediumWide, StatSpread.LowWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Blue, 3u), (ColorCategory.Red, 3u), (ColorCategory.Green, 3u)}
            },
            new Config() // 48
            {
                Stats = new[] {StatSpread.MediumStable, StatSpread.LowStable, StatSpread.MediumStable, StatSpread.HighWide, StatSpread.MediumStable, StatSpread.MediumStable},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Green, 12u), (ColorCategory.Purple, 6u), (ColorCategory.Blue, 3u)}
            },
            new Config() // 49
            {
                Stats = new[] {StatSpread.HighWide, StatSpread.MediumStable, StatSpread.MediumWide, StatSpread.LowStable, StatSpread.LowStable, StatSpread.LowWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 6u), (ColorCategory.Red, 6u)}
            },
            new Config() // 50
            {
                Stats = new[] {StatSpread.LowWide, StatSpread.MediumWide, StatSpread.LowWide, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Green, 3u), (ColorCategory.Blue, 6u), (ColorCategory.Red, 3u)}
            },
            new Config() // 51
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowStable, StatSpread.MediumWide, StatSpread.MediumStable, StatSpread.MediumStable, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Red, 6u), (ColorCategory.Purple, 3u), (ColorCategory.Orange, 3u)}
            },
            new Config() // 52
            {
                Stats = new[] {StatSpread.MediumWide, StatSpread.MediumWide, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.MediumWide, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Purple, 3u), (ColorCategory.Green, 3u), (ColorCategory.Red, 3u)}
            },
            new Config() // 53
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighWide, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Red, 6u), (ColorCategory.Orange, 6u), (ColorCategory.Blue, 6u)}
            },
            new Config() // 54
            {
                Stats = new[] {StatSpread.LowStable, StatSpread.LowWide, StatSpread.MediumWide, StatSpread.LowWide, StatSpread.LowWide, StatSpread.LowWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 12u), (ColorCategory.Red, 6u)}
            },
            new Config() // 55
            {
                Stats = new[] {StatSpread.MediumStable, StatSpread.MediumStable, StatSpread.MediumStable, StatSpread.MediumStable, StatSpread.MediumStable, StatSpread.MediumStable},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Green, 6u), (ColorCategory.Blue, 6u), (ColorCategory.Red, 6u)}
            },
            new Config() // 56
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.MediumWide, StatSpread.LowNormal, StatSpread.MediumWide, StatSpread.MediumWide, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Purple, 6u), (ColorCategory.Orange, 6u)}
            },
            new Config() // 57
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Green, 12u), (ColorCategory.Blue, 3u), (ColorCategory.Purple, 3u)}
            },
            new Config() // 58
            {
                Stats = new[] {StatSpread.MediumWide, StatSpread.MediumWide, StatSpread.MediumWide, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.MediumWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Purple, 12u), (ColorCategory.Red, 6u), (ColorCategory.Blue, 6u)}
            },
            new Config() // 59
            {
                Stats = new[] {StatSpread.LowStable, StatSpread.MediumWide, StatSpread.MediumWide, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Purple, 12u), (ColorCategory.Red, 6u), (ColorCategory.Blue, 6u)}
            },
            new Config() // 60
            {
                Stats = new[] {StatSpread.HighWide, StatSpread.HighWide, StatSpread.LowStable, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Red, 12u), (ColorCategory.Purple, 6u), (ColorCategory.Orange, 6u)}
            },
            new Config() // 61
            {
                Stats = new[] {StatSpread.HighWide, StatSpread.LowStable, StatSpread.HighWide, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 12u), (ColorCategory.Blue, 6u), (ColorCategory.Green, 6u)}
            },
            new Config() // 62
            {
                Stats = new[] {StatSpread.LowStable, StatSpread.LowStable, StatSpread.LowStable, StatSpread.LowStable, StatSpread.LowStable, StatSpread.HighWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Green, 12u), (ColorCategory.Orange, 6u)}
            },
            new Config() // 63
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.HighStable, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighStable, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Grey, 6u), (ColorCategory.Purple, 6u), (ColorCategory.Orange, 6u)}
            },
            new Config() // 64
            {
                Stats = new[] {StatSpread.HighWide, StatSpread.HighWide, StatSpread.HighWide, StatSpread.HighWide, StatSpread.HighWide, StatSpread.HighWide},
                Colors = new[] {(ColorCategory.Purple, 1u)}
            },
            new Config() // 65
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.MediumNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.MediumNormal, StatSpread.MediumNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Green, 3u), (ColorCategory.Blue, 3u), (ColorCategory.Red, 3u)}
            },
            new Config() // 66
            {
                Stats = new[] {StatSpread.HighWide, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighWide, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Purple, 3u), (ColorCategory.Red, 3u), (ColorCategory.Orange, 3u)}
            },
            new Config() // 67
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowWide, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 3u), (ColorCategory.Purple, 3u)}
            },
            new Config() // 68
            {
                Stats = new[] {StatSpread.LowStable, StatSpread.LowStable, StatSpread.LowStable, StatSpread.LowStable, StatSpread.LowStable, StatSpread.HighWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Purple, 3u), (ColorCategory.Orange, 3u), (ColorCategory.Green, 6u)}
            },
            new Config() // 69
            {
                Stats = new[] {StatSpread.MediumNormal, StatSpread.LowNormal, StatSpread.MediumNormal, StatSpread.LowNormal, StatSpread.MediumNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Red, 3u), (ColorCategory.Blue, 3u), (ColorCategory.Green, 3u)}
            },
            new Config() // 70
            {
                Stats = new[] {StatSpread.MediumStable, StatSpread.MediumStable, StatSpread.MediumStable, StatSpread.HighWide, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Purple, 3u), (ColorCategory.Red, 3u), (ColorCategory.Grey, 6u)}
            },
            new Config() // 71
            {
                Stats = new[] {StatSpread.MediumWide, StatSpread.LowStable, StatSpread.MediumWide, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Red, 6u), (ColorCategory.Purple, 6u)}
            },
            new Config() // 72
            {
                Stats = new[] {StatSpread.MediumNormal, StatSpread.LowNormal, StatSpread.LowStable, StatSpread.MediumNormal, StatSpread.MediumNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Grey, 12u), (ColorCategory.Blue, 3u), (ColorCategory.Red, 3u)}
            },
            new Config() // 73
            {
                Stats = new[] {StatSpread.LowStable, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighWide, StatSpread.HighWide, StatSpread.HighWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Purple, 6u), (ColorCategory.Orange, 6u), (ColorCategory.Blue, 6u), (ColorCategory.Red, 6u)}
            },
            new Config() // 74
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.MediumNormal, StatSpread.MediumWide, StatSpread.HighWide, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Red, 6u), (ColorCategory.Blue, 12u)}
            },
            new Config() // 75
            {
                Stats = new[] {StatSpread.LowStable, StatSpread.LowNormal, StatSpread.LowStable, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 12u), (ColorCategory.Grey, 6u)}
            },
            new Config() // 76
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Green, 6u), (ColorCategory.Purple, 3u), (ColorCategory.Red, 3u)}
            },
            new Config() // 77
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.HighWide, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 12u), (ColorCategory.Grey, 6u), (ColorCategory.Purple, 3u)}
            },
            new Config() // 78
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.MediumWide, StatSpread.MediumWide, StatSpread.MediumWide, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 6u), (ColorCategory.Grey, 6u)}
            },
            new Config() // 79
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.MediumWide, StatSpread.LowNormal, StatSpread.MediumWide, StatSpread.MediumWide, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Grey, 12u), (ColorCategory.Blue, 6u), (ColorCategory.Red, 6u)}
            },
            new Config() // 80
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.MediumNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.MediumNormal, StatSpread.MediumNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Purple, 6u), (ColorCategory.Blue, 6u), (ColorCategory.Red, 6u)}
            },
            new Config() // 81
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Purple, 6u), (ColorCategory.Orange, 6u)}
            },
            new Config() // 82
            {
                Stats = new[] {StatSpread.MediumNormal, StatSpread.LowWide, StatSpread.MediumNormal, StatSpread.LowStable, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Grey, 12u), (ColorCategory.Red, 6u), (ColorCategory.Green, 6u)}
            },
            new Config() // 83
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowStable, StatSpread.LowNormal, StatSpread.LowStable, StatSpread.LowNormal, StatSpread.LowStable},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Blue, 3u), (ColorCategory.Green, 3u), (ColorCategory.Red, 3u)}
            },
            new Config() // 84
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.MediumWide, StatSpread.LowNormal, StatSpread.MediumNormal, StatSpread.MediumNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 12u), (ColorCategory.Grey, 6u)}
            },
            new Config() // 85
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.MediumNormal, StatSpread.MediumNormal, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Red, 12u), (ColorCategory.Blue, 3u)}
            },
            new Config() // 86
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.MediumNormal, StatSpread.LowNormal, StatSpread.MediumNormal, StatSpread.HighStable},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 12u), (ColorCategory.Red, 6u)}
            },
            new Config() // 87
            {
                Stats = new[] {StatSpread.HighStable, StatSpread.LowWide, StatSpread.HighStable, StatSpread.LowWide, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Purple, 6u), (ColorCategory.Orange, 6u), (ColorCategory.Blue, 3u)}
            },
            new Config() // 88
            {
                Stats = new[] {StatSpread.LowStable, StatSpread.LowWide, StatSpread.LowStable, StatSpread.LowWide, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Purple, 6u), (ColorCategory.Orange, 6u), (ColorCategory.Blue, 3u)}
            },
            new Config() // 89
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighWide, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Green, 12u), (ColorCategory.Orange, 6u)}
            },
            new Config() // 90
            {
                Stats = new[] {StatSpread.MediumNormal, StatSpread.MediumNormal, StatSpread.MediumNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Blue, 3u), (ColorCategory.Red, 3u), (ColorCategory.Blue, 3u)}
            },
            new Config() // 91
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.HighStable, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighStable, StatSpread.HighStable},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 6u), (ColorCategory.Grey, 6u)}
            },
            new Config() // 92
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.MediumNormal, StatSpread.MediumNormal, StatSpread.MediumNormal, StatSpread.MediumNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Purple, 3u), (ColorCategory.Blue, 3u), (ColorCategory.Red, 3u), (ColorCategory.Green, 3u), (ColorCategory.Orange, 3u)}
            },
            new Config() // 93
            {
                Stats = new[] {StatSpread.HighStable, StatSpread.HighStable, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Blue, 6u), (ColorCategory.Red, 6u), (ColorCategory.Purple, 3u)}
            },
            new Config() // 94
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.MediumNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.MediumNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Red, 3u), (ColorCategory.Orange, 3u), (ColorCategory.Grey, 6u)}
            },
            new Config() // 95
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.MediumNormal, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.MediumNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 6u), (ColorCategory.Blue, 3u), (ColorCategory.Green, 3u)}
            },
            new Config() // 96
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.MediumNormal, StatSpread.MediumNormal, StatSpread.MediumNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 3u), (ColorCategory.Red, 3u), (ColorCategory.Green, 3u)}
            },
            new Config() // 97
            {
                Stats = new[] {StatSpread.HighWide, StatSpread.HighWide, StatSpread.LowWide, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.LowNormal},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Red, 6u), (ColorCategory.Grey, 6u)}
            },
            new Config() // 98
            {
                Stats = new[] {StatSpread.MediumNormal, StatSpread.LowNormal, StatSpread.MediumNormal, StatSpread.LowStable, StatSpread.LowStable, StatSpread.MediumWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Blue, 3u), (ColorCategory.Green, 3u)}
            },
            new Config() // 99
            {
                Stats = new[] {StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighWide, StatSpread.LowNormal, StatSpread.LowNormal, StatSpread.HighWide},
                Colors = new[] {(ColorCategory.Nil, 1u), (ColorCategory.Orange, 6u), (ColorCategory.Grey, 6u)},
                SpawnRateModifier = 5
            },
        };

        [TestCase(0u, 99u, "GenerateAllResults.json", TestName = "Generate All")]
        public void GenerateAll(uint min, uint max, string outputLocation)
        {
            Dictionary<string, SpeciesDefinition> results = new Dictionary<string, SpeciesDefinition>((int) (max - min));
            for (uint i = min; i <= max; i++)
            {
                if(i < _speciesConfigs.Length)
                {
                    Config config = _speciesConfigs[i];
                    string paddedId = i.ToString().PadLeft(2, '0');
                    string speciesId = $"Cryptid{paddedId}";
                    string nameId = $"Cryptid/{paddedId}";
                    string artId = $"/Assets/Cryptids/cryptid_{paddedId}.png";
                    string maskId = $"/Assets/Cryptids/cryptid_{paddedId}_mask.png";
                    uint spawnRate = (uint) Math.Max(1, Math.Round(50f - config.Stats.Sum(x => (int) x) + config.SpawnRateModifier).Remap(0, 50, 1, 20));
                    Dictionary<EPrimaryStat, DropCalculation<uint>> statTable = new Dictionary<EPrimaryStat, DropCalculation<uint>>();
                    DropCalculation<string> colorTable;

                    for (EPrimaryStat stat = EPrimaryStat.Strength; stat < EPrimaryStat._Count; stat++)
                    {
                        StatSpread statSpread = config.Stats[(int) stat];
                        StatRange statRange = _statRanges[statSpread];
                        uint minStat = statRange.Min;
                        uint maxStat = statRange.Max;
                        uint spread = statRange.Spread;
                        uint halfSpread = spread / 2;
                        float statStep = (maxStat - minStat) / (float) spread;

                        List<DropCalculation<uint>.Point> statPoints = new List<DropCalculation<uint>.Point>((int) spread);
                        for (int pointIdx = 0; pointIdx < spread; pointIdx++)
                        {
                            uint value = (uint) Math.Ceiling(minStat + statStep * pointIdx);
                            uint weight = (uint) ((halfSpread + 1) - Math.Abs(halfSpread - pointIdx));
                            statPoints.Add(new DropCalculation<uint>.Point(value, weight));
                        }
                        statTable[stat] = new DropCalculation<uint>(statPoints.ToArray());
                    }

                    List<DropCalculation<string>.Point> colorPoints = new List<DropCalculation<string>.Point>();
                    foreach (var colorTuple in config.Colors)
                    {
                        if (_colorDefinitions.TryGetValue(colorTuple.Item1, out var colorIds))
                        {
                            uint perVariantWeight = (uint) Math.Ceiling(colorTuple.Item2 / (float) colorIds.Length);
                            foreach (string colorId in colorIds)
                            {
                                colorPoints.Add(new DropCalculation<string>.Point(colorId, perVariantWeight));
                            }
                        }
                    }
                    colorTable = new DropCalculation<string>(colorPoints.ToArray());

                    SpeciesDefinition speciesDefinition = new SpeciesDefinition(
                        speciesId,
                        nameId,
                        artId,
                        maskId,
                        spawnRate,
                        statTable,
                        colorTable);
                    results[speciesId] = speciesDefinition;
                }
            }

            outputLocation = Path.Combine(AppContext.BaseDirectory, outputLocation);
            FileInfo fi = new FileInfo(outputLocation);
            if (!Directory.Exists(fi.DirectoryName))
                Directory.CreateDirectory(fi.DirectoryName);

            File.WriteAllText(fi.FullName, JsonConvert.SerializeObject(results, Formatting.Indented));
        }
    }
}