using System.Collections.Generic;
using UAT_MS539.Core.Code.Cryptid;

namespace UAT_MS539.Core.Code.Utility
{
    public class PlayerData
    {
        public uint Day = 0;
        public Cryptid.Cryptid ActiveCryptid = null;
        
        public readonly HashSet<string> ConsumedRunePatterns = new HashSet<string>();
        public readonly List<Cryptid.Cryptid> FrozenCryptedInventory = new List<Cryptid.Cryptid>();
        public readonly List<CryptidDnaSample> DnaSampleInventory = new List<CryptidDnaSample>();

        public readonly List<Food.Food> FoodInventory = new List<Food.Food>();
    }
}