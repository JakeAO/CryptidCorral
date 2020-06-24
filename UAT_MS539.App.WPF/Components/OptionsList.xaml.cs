using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using UAT_MS539.Code;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.Food;
using UAT_MS539.Core.Code.StateMachine.CombatEngine;
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

        public void AddButton(string localizedText, Action callback, string localizedTooltip = null)
        {
            DefaultButton newButton = new DefaultButton();
            newButton.Setup(localizedText, callback, localizedTooltip);

            _stackPanel.Children.Add(newButton);
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
                ButtonWithIcon newButton = new ButtonWithIcon();
                newButton.Setup(
                    food.Definition.ArtId,
                    locDatabase.Localize(food.Definition.NameId),
                    () => callback(food),
                    string.Join("\n", TooltipUtil.GetTooltipContent(food, locDatabase)));

                newSelectionList.AddSelectable(newButton);
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
                Color? highlight = null;
                if (isBuy && option.cost > currentCoins)
                    highlight = Colors.IndianRed;
                
                ButtonWithIcon newButton = new ButtonWithIcon();
                newButton.Setup(
                    option.food.Definition.ArtId,
                    $"{locDatabase.Localize(option.food.Definition.NameId)} ({option.cost})",
                    () => callback((option.food, option.cost)),
                    string.Join("\n", TooltipUtil.GetTooltipContent(option.food, locDatabase)),
                    highlight);

                newSelectionList.AddSelectable(newButton);
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
                DefaultButton newButton = new DefaultButton();
                newButton.Setup(
                    $"{locDatabase.Localize(cryptid.Species.NameId)} (Age: {cryptid.AgeInDays})",
                    () => callback(cryptid),
                    string.Join("\n", TooltipUtil.GetTooltipContent(cryptid, locDatabase)));

                newSelectionList.AddSelectable(newButton);
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
                DefaultButton newButton = new DefaultButton();
                newButton.Setup(
                    $"{locDatabase.Localize(dnaSample.Cryptid.Species.NameId)} (Age: {dnaSample.Cryptid.AgeInDays})",
                    () => callback(dnaSample),
                    string.Join("\n", TooltipUtil.GetTooltipContent(dnaSample, locDatabase)));

                newSelectionList.AddSelectable(newButton);
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
                Color? highlight = null;
                if (foodModifierStats?.Overlaps(regimen.RandomStatIncreases.Keys) ?? false)
                    highlight = Colors.PaleGreen;

                DefaultButton newButton = new DefaultButton();
                newButton.Setup(
                    $"{locDatabase.Localize(regimen.NameId)} ({regimen.StaminaCost})",
                    () => callback(regimen),
                    string.Join("\n", TooltipUtil.GetTooltipContent(regimen, locDatabase)),
                    highlight);

                newSelectionList.AddSelectable(newButton);
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
                CryptidTooltip tooltip = new CryptidTooltip();
                tooltip.SetCryptid(option.opponent, locDatabase);

                ButtonWithIcon newButton = new ButtonWithIcon();
                newButton.Setup(
                    option.opponent.Species.ArtId,
                    $"[{option.weight}] {locDatabase.Localize(option.opponent.Species.NameId)}",
                    () => callback(option),
                    tooltip);

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