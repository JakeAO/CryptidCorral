﻿using System.Collections.Generic;
using Core.Code.Cryptid;

namespace Core.Code.Utility
{
    public class PlayerData
    {
        public uint Day = 0;
        public uint Coins = 100;

        public Cryptid.Cryptid ActiveCryptid = null;
        
        public readonly HashSet<string> ConsumedRunePatterns = new HashSet<string>();
        public readonly List<Cryptid.Cryptid> FrozenCryptedInventory = new List<Cryptid.Cryptid>();
        public readonly List<CryptidDnaSample> DnaSampleInventory = new List<CryptidDnaSample>();

        public readonly List<Food.Food> FoodInventory = new List<Food.Food>();
    }
}