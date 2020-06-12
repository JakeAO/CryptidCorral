using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.Food;
using UAT_MS539.Core.Code.Training;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Components
{
    public partial class OptionsList : UserControl
    {
        public OptionsList()
        {
            InitializeComponent();
        }

        private static List<string> GetTooltipContent(Food food, LocDatabase locDatabase)
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
                        tooltipContent.Add($"  {locDatabase.Localize(multKvp.Key)} xp x{multKvp.Value}");
                    }
                }
            }
            if (tooltipContent.Count == 0)
            {
                tooltipContent.Add(locDatabase.Localize("Tooltip/TrainingNoBonus"));
            }
            return tooltipContent;
        }

        private static List<string> GetTooltipContent(CryptidDnaSample dnaSample, LocDatabase locDatabase)
        {
            List<string> tooltipContent = new List<string>(15);
            tooltipContent.Add($"{locDatabase.Localize(dnaSample.Cryptid.Species.NameId)} ({locDatabase.Localize(dnaSample.Cryptid.Color.NameId)})");
            tooltipContent.Add($"{locDatabase.Localize(ESecondaryStat.Health)}: {dnaSample.Cryptid.SecondaryStats[(int)ESecondaryStat.Health]}");
            tooltipContent.Add($"{locDatabase.Localize(ESecondaryStat.Stamina)}: {dnaSample.Cryptid.SecondaryStats[(int)ESecondaryStat.Stamina]}");
            tooltipContent.Add($"{locDatabase.Localize(ESecondaryStat.Renown)}: {dnaSample.Cryptid.SecondaryStats[(int)ESecondaryStat.Renown]}");
            for (int i = 0; i < (int)EPrimaryStat._Count; i++)
                tooltipContent.Add($"{locDatabase.Localize((EPrimaryStat)i)}: {dnaSample.Cryptid.PrimaryStats[i]}");
            return tooltipContent;
        }

        private static List<string> GetTooltipContent(Cryptid cryptid, LocDatabase locDatabase)
        {
            List<string> tooltipContent = new List<string>(15);
            tooltipContent.Add($"{locDatabase.Localize(cryptid.Species.NameId)} ({locDatabase.Localize(cryptid.Color.NameId)})");
            tooltipContent.Add($"{locDatabase.Localize(ESecondaryStat.Health)}: {cryptid.SecondaryStats[(int) ESecondaryStat.Health]}");
            tooltipContent.Add($"{locDatabase.Localize(ESecondaryStat.Stamina)}: {cryptid.SecondaryStats[(int) ESecondaryStat.Stamina]}");
            tooltipContent.Add($"{locDatabase.Localize(ESecondaryStat.Renown)}: {cryptid.SecondaryStats[(int)ESecondaryStat.Renown]}");
            tooltipContent.Add($"{locDatabase.Localize("Stat/Age")}: {cryptid.AgeInDays}");
            for (int i = 0; i < (int) EPrimaryStat._Count; i++)
                tooltipContent.Add($"{locDatabase.Localize((EPrimaryStat) i)}: {cryptid.PrimaryStats[i]}");
            return tooltipContent;
        }

        private static List<string> GetTooltipContent(TrainingRegimen trainingRegimen, LocDatabase locDatabase)
        {
            List<string> tooltipContent = new List<string>(6);
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

                if (minGain != maxGain)
                {
                    tooltipContent.Add($"{locDatabase.Localize(enumVal)} xp +{minGain}-{maxGain}");
                }
                else if (minGain > 0)
                {
                    tooltipContent.Add($"{locDatabase.Localize(enumVal)} xp +{minGain}");
                }
            }
            return tooltipContent;
        }

        public void AddButton(string localizedText, Action callback)
        {
            DefaultButton newButton = new DefaultButton();
            newButton.Setup(localizedText, callback);

            _stackPanel.Children.Add(newButton);
        }

        public void AddRuneSelection(IReadOnlyList<RunePattern> options, RuneDatabase runeDatabase, Action<RunePattern> callback)
        {
            SelectionList newSelectionList = new SelectionList();
            for (int i = 0; i < options.Count; i++)
            {
                RunePattern pattern = options[i];

                RunePatternDisplay newPattern = new RunePatternDisplay();
                newPattern.SetPattern(pattern, runeDatabase, () => callback(pattern));

                newSelectionList.AddSelectable(newPattern);
            }
            _stackPanel.Children.Add(newSelectionList);
        }

        public void AddFoodSelection(IReadOnlyList<Food> options, LocDatabase locDatabase, Action<Food> callback)
        {
            SelectionList newSelectionList = new SelectionList();
            for (int i = 0; i < options.Count; i++)
            {
                Food food = options[i];

                ButtonWithIcon newButton = new ButtonWithIcon();
                newButton.Setup(
                    food.Definition.ArtId,
                    locDatabase.Localize(food.Definition.NameId),
                    () => callback(food),
                    string.Join("\n", GetTooltipContent(food, locDatabase)));

                newSelectionList.AddSelectable(newButton);
            }
            _stackPanel.Children.Add(newSelectionList);
        }

        public void AddBuySellSelection(IReadOnlyList<(Food, uint)> options, LocDatabase locDatabase, uint currentCoins, Action<(Food, uint)> callback)
        {
            SelectionList newSelectionList = new SelectionList();
            for (int i = 0; i < options.Count; i++)
            {
                Food food = options[i].Item1;
                uint cost = options[i].Item2;
                
                // TODO Special Tooltip
                ButtonWithIcon newButton = new ButtonWithIcon();
                newButton.Setup(
                    food.Definition.ArtId,
                    $"{locDatabase.Localize(food.Definition.NameId)} ({cost})",
                    () => callback((food, cost)),
                    string.Join("\n", GetTooltipContent(food, locDatabase)));

                newSelectionList.AddSelectable(newButton);
            }
            _stackPanel.Children.Add(newSelectionList);
        }

        public void AddCryptidSelection(IReadOnlyList<Cryptid> options, LocDatabase locDatabase, Action<Cryptid> callback)
        {
            SelectionList newSelectionList = new SelectionList();
            for (int i = 0; i < options.Count; i++)
            {
                Cryptid cryptid = options[i];

                // TODO Special Tooltip
                DefaultButton newButton = new DefaultButton();
                newButton.Setup(
                    $"{locDatabase.Localize(cryptid.Species.NameId)} (Age: {cryptid.AgeInDays})",
                    () => callback(cryptid),
                    string.Join("\n", GetTooltipContent(cryptid, locDatabase)));

                newSelectionList.AddSelectable(newButton);
            }
            _stackPanel.Children.Add(newSelectionList);
        }
        
        public void AddDnaSampleSelection(IReadOnlyList<CryptidDnaSample> options, LocDatabase locDatabase, Action<CryptidDnaSample> callback)
        {
            SelectionList newSelectionList = new SelectionList();
            for (int i = 0; i < options.Count; i++)
            {
                CryptidDnaSample dnaSample = options[i];

                // TODO Special Tooltip
                DefaultButton newButton = new DefaultButton();
                newButton.Setup(
                    $"{locDatabase.Localize(dnaSample.Cryptid.Species.NameId)} (Age: {dnaSample.Cryptid.AgeInDays})",
                    () => callback(dnaSample),
                    string.Join("\n", GetTooltipContent(dnaSample, locDatabase)));

                newSelectionList.AddSelectable(newButton);
            }
            _stackPanel.Children.Add(newSelectionList);
        }

        public void AddTrainingSelection(IReadOnlyList<TrainingRegimen> options, LocDatabase locDatabase, Action<TrainingRegimen> callback)
        {
            SelectionList newSelectionList = new SelectionList();
            for (int i = 0; i < options.Count; i++)
            {
                TrainingRegimen regimen = options[i];

                // TODO Special Tooltip
                DefaultButton newButton = new DefaultButton();
                newButton.Setup(
                    $"{locDatabase.Localize(regimen.NameId)} ({regimen.StaminaCost})",
                    () => callback(regimen),
                    string.Join("\n", GetTooltipContent(regimen, locDatabase)));

                newSelectionList.AddSelectable(newButton);
            }
            _stackPanel.Children.Add(newSelectionList);
        }

        public void ClearOptions()
        {
            _stackPanel.Children.Clear();
        }
    }
}