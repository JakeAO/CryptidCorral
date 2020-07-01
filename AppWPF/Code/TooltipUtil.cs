using System;
using System.Collections.Generic;
using System.Linq;
using Core.Code.Cryptid;
using Core.Code.Food;
using Core.Code.Training;
using Core.Code.Utility;

namespace AppWPF.Code
{
    public static class TooltipUtil
    {
        public static List<string> GetTooltipContent(Food food, LocDatabase locDatabase)
        {
            List<string> tooltipContent = new List<string>(5);
            if (food.MoraleBoost > 0)
            {
                tooltipContent.Add($"{locDatabase.Localize(EHiddenStat.Morale)} +{food.MoraleBoost}");
            }

            if (food.BoostsByStat.Values.Any(x => x > 0))
            {
                tooltipContent.Add(locDatabase.Localize("Tooltip/TrainingBonus"));
                foreach (var boostKvp in food.BoostsByStat)
                {
                    if (boostKvp.Value > 0)
                    {
                        tooltipContent.Add($"  {locDatabase.Localize(boostKvp.Key)} xp +{boostKvp.Value}");
                    }
                }
            }

            if (food.MultipliersByStat.Values.Any(x => Math.Abs(x - 1f) > 0.01f))
            {
                tooltipContent.Add(locDatabase.Localize("Tooltip/TrainingMultiplier"));
                foreach (var multKvp in food.MultipliersByStat)
                {
                    if (Math.Abs(multKvp.Value - 1f) > 0.01f)
                    {
                        tooltipContent.Add($"  {locDatabase.Localize(multKvp.Key)} tp x{multKvp.Value}");
                    }
                }
            }

            if (tooltipContent.Count == 0)
            {
                tooltipContent.Add(locDatabase.Localize("Tooltip/TrainingNoBonus"));
            }

            return tooltipContent;
        }

        public static List<string> GetTooltipContent(CryptidDnaSample dnaSample, LocDatabase locDatabase)
        {
            List<string> tooltipContent = new List<string>(15);
            tooltipContent.Add($"{locDatabase.Localize(dnaSample.Cryptid.Species.NameId)} ({locDatabase.Localize(dnaSample.Cryptid.Color.NameId)})");
            tooltipContent.Add($"{locDatabase.Localize("Renown")}: {dnaSample.Cryptid.CurrentRenown}");
            for (int i = 0; i < (int) EPrimaryStat._Count; i++)
                tooltipContent.Add($"{locDatabase.Localize((EPrimaryStat) i)}: {dnaSample.Cryptid.PrimaryStats[i]}");
            return tooltipContent;
        }

        public static List<string> GetTooltipContent(Cryptid cryptid, LocDatabase locDatabase)
        {
            List<string> tooltipContent = new List<string>(15);
            tooltipContent.Add($"{locDatabase.Localize(cryptid.Species.NameId)} ({locDatabase.Localize(cryptid.Color.NameId)})");
            tooltipContent.Add($"{locDatabase.Localize("Health")}: {cryptid.MaxHealth}");
            tooltipContent.Add($"{locDatabase.Localize("Stamina")}: {cryptid.MaxStamina}");
            tooltipContent.Add($"{locDatabase.Localize("Renown")}: {cryptid.CurrentRenown}");
            tooltipContent.Add($"{locDatabase.Localize("Stat/Age")}: {cryptid.AgeInDays}");
            for (int i = 0; i < (int) EPrimaryStat._Count; i++)
                tooltipContent.Add($"{locDatabase.Localize((EPrimaryStat) i)}: {cryptid.PrimaryStats[i]}");
            return tooltipContent;
        }

        public static List<string> GetTooltipContent(TrainingRegimen trainingRegimen, HashSet<EPrimaryStat> activeFoodModifiers, LocDatabase locDatabase)
        {
            List<string> tooltipContent = new List<string>(6);
            tooltipContent.Add($"{locDatabase.Localize("Stamina")} -{trainingRegimen.StaminaCost}");
            for (int i = 0; i < (int) EPrimaryStat._Count; i++)
            {
                EPrimaryStat enumVal = (EPrimaryStat) i;
                uint minGain = 0, maxGain = 0;

                if (trainingRegimen.GuaranteedStatIncrease.TryGetValue(enumVal, out uint guaranteedValue))
                {
                    minGain = maxGain = guaranteedValue;
                }

                if (trainingRegimen.RandomStatIncreases.TryGetValue(enumVal, out var randomDropCalculation))
                {
                    minGain += randomDropCalculation.Points.Min(x => x.Value);
                    maxGain += randomDropCalculation.Points.Max(x => x.Value);
                }

                if (minGain == maxGain && minGain == 0)
                    continue;

                string tooltipEntry = $"{locDatabase.Localize(enumVal)} ";
                if (minGain != maxGain)
                {
                    tooltipEntry += $"tp +{minGain}-{maxGain}";
                }
                else
                {
                    tooltipEntry += $"tp +{minGain}";
                }
                if (activeFoodModifiers?.Contains(enumVal) == true)
                {
                    tooltipEntry += " (Bonus)";
                }
                tooltipContent.Add(tooltipEntry);
            }

            return tooltipContent;
        }
    }
}
