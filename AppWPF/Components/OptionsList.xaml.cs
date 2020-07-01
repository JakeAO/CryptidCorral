using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using AppWPF.Code;
using Core.Code.Cryptid;
using Core.Code.Food;
using Core.Code.StateMachine.CombatEngine;
using Core.Code.Training;
using Core.Code.Utility;

namespace AppWPF.Components
{
    public partial class OptionsList : UserControl
    {
        public OptionsList()
        {
            InitializeComponent();
        }

        public void AddButton(string localizedText, Action callback, string localizedTooltip = null)
        {
            _stackPanel.Children.Add(
                new DefaultButton(
                    callback,
                    localizedText,
                    localizedTooltip));
        }

        public void AddRuneSelection(IReadOnlyList<RunePattern> options, RuneDatabase runeDatabase, Action<RunePattern> callback)
        {
            SelectionList newSelectionList = new SelectionList();
            foreach (RunePattern pattern in options
                .OrderBy(x => x.FilePath)
                .Reverse())
            {
                RunePatternDisplay newPattern = new RunePatternDisplay();
                newPattern.SetPattern(pattern, runeDatabase, () => callback(pattern));

                newSelectionList.AddSelectable(newPattern);
            }
            _stackPanel.Children.Add(newSelectionList);
        }

        public void AddFoodSelection(IReadOnlyList<Food> options, LocDatabase locDatabase, Action<Food> callback)
        {
            SelectionList newSelectionList = new SelectionList();
            foreach (Food food in options
                .OrderBy(x => x.TotalFoodQuality)
                .Reverse())
            {
                newSelectionList.AddSelectable(
                    new DefaultButton(
                        () => callback(food),
                        food.Definition.ArtId,
                        locDatabase.Localize(food.Definition.NameId),
                        string.Join("\n", TooltipUtil.GetTooltipContent(food, locDatabase))));
            }
            _stackPanel.Children.Add(newSelectionList);
        }

        public void AddBuySellSelection(bool isBuy, IReadOnlyList<(Food, uint)> options, LocDatabase locDatabase, uint currentCoins, Action<(Food, uint)> callback)
        {
            SelectionList newSelectionList = new SelectionList();
            foreach ((Food food, uint cost) option in options
                .OrderBy(x => x.Item1.TotalFoodQuality)
                .Reverse())
            {
                newSelectionList.AddSelectable(
                    new DefaultButton(
                        () => callback((option.food, option.cost)),
                        option.food.Definition.ArtId,
                        $"{locDatabase.Localize(option.food.Definition.NameId)} ({option.cost})",
                        string.Join("\n", TooltipUtil.GetTooltipContent(option.food, locDatabase)),
                        isBuy && option.cost > currentCoins ? DefaultButton.HighlightType.Bad : DefaultButton.HighlightType.Default));
            }
            _stackPanel.Children.Add(newSelectionList);
        }

        public void AddCryptidSelection(IReadOnlyList<Cryptid> options, LocDatabase locDatabase, Action<Cryptid> callback)
        {
            SelectionList newSelectionList = new SelectionList();
            foreach (Cryptid cryptid in options
                .OrderBy(x => WeightClassUtil.GetWeight(x))
                .Reverse())
            {
                newSelectionList.AddSelectable(
                    new DefaultButton(
                        () => callback(cryptid),
                        $"{locDatabase.Localize(cryptid.Species.NameId)} (Age: {cryptid.AgeInDays})",
                        string.Join("\n", TooltipUtil.GetTooltipContent(cryptid, locDatabase))));
            }
            _stackPanel.Children.Add(newSelectionList);
        }
        
        public void AddDnaSampleSelection(IReadOnlyList<CryptidDnaSample> options, LocDatabase locDatabase, Action<CryptidDnaSample> callback)
        {
            SelectionList newSelectionList = new SelectionList();
            foreach (CryptidDnaSample dnaSample in options
                .OrderBy(x => WeightClassUtil.GetWeight(x.Cryptid))
                .Reverse())
            {
                newSelectionList.AddSelectable(
                    new DefaultButton(
                        () => callback(dnaSample),
                        $"{locDatabase.Localize(dnaSample.Cryptid.Species.NameId)} (Age: {dnaSample.Cryptid.AgeInDays})",
                        string.Join("\n", TooltipUtil.GetTooltipContent(dnaSample, locDatabase))));
            }
            _stackPanel.Children.Add(newSelectionList);
        }

        public void AddTrainingSelection(IReadOnlyList<TrainingRegimen> options, DailyTrainingData trainingData, LocDatabase locDatabase, Action<TrainingRegimen> callback)
        {
            HashSet<EPrimaryStat> foodModifierStats = trainingData?.Food?.MultipliersByStat?
                .Where(x => x.Value != 1f)
                .Select(x => x.Key)
                .ToHashSet();
            
            SelectionList newSelectionList = new SelectionList();
            foreach (TrainingRegimen regimen in options
                .OrderBy(x => x.StaminaCost)
                .ThenBy(x => x.SpawnRate)
                .Reverse())
            {
                DefaultButton.HighlightType highlight = (foodModifierStats?.Overlaps(regimen.RandomStatIncreases.Keys) ?? false)
                    ? DefaultButton.HighlightType.Good
                    : DefaultButton.HighlightType.Default;

                newSelectionList.AddSelectable(
                    new DefaultButton(
                        () => callback(regimen),
                        $"{locDatabase.Localize(regimen.NameId)} ({regimen.StaminaCost})",
                        string.Join("\n", TooltipUtil.GetTooltipContent(regimen, foodModifierStats, locDatabase)),
                        highlight));
            }
            _stackPanel.Children.Add(newSelectionList);
        }

        public void AddOpponentSelection(IReadOnlyList<(Cryptid, EWeightClass)> options, LocDatabase locDatabase, Action<(Cryptid, EWeightClass)> callback)
        {
            SelectionList newSelectionList = new SelectionList();
            foreach ((Cryptid opponent, EWeightClass weight) option in options
                .OrderBy(x => x.Item2)
                .ThenBy(x => WeightClassUtil.GetWeight(x.Item1))
                .Reverse())
            {
                newSelectionList.AddSelectable(
                    new DefaultButton(
                        () => callback(option),
                        option.opponent.Species.ArtId,
                        $"[{option.weight}] {locDatabase.Localize(option.opponent.Species.NameId)}",
                        new CryptidTooltip(option.opponent, locDatabase)));
            }
            _stackPanel.Children.Add(newSelectionList);
        }

        public void ClearOptions()
        {
            _stackPanel.Children.Clear();
        }
    }
}