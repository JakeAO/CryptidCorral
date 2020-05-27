using System;
using System.Collections.Generic;
using System.IO;
using UAT_MS539.Core.Code.Cryptid;

namespace UAT_MS539.Core.Code
{
    public class PlayerData
    {
        public static readonly string SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "CryptidCorral", "save.dat");

        public uint Day = 0;

        public Cryptid.Cryptid ActiveCryptid = null;

        public readonly List<Food.Food> FoodInventory = new List<Food.Food>();
        public readonly List<CryptidDnaSample> DnaSampleInventory = new List<CryptidDnaSample>();
        public readonly List<Cryptid.Cryptid> FrozenCryptedInventory = new List<Cryptid.Cryptid>();
        public readonly HashSet<string> ConsumedRunePatterns = new HashSet<string>();
    }
}