using System;
using System.Collections.Generic;
using System.Windows.Controls;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.Food;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Components
{
    public partial class OptionsList : UserControl
    {
        public OptionsList()
        {
            InitializeComponent();
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

                // TODO Special Tooltip
                ButtonWithIcon newButton = new ButtonWithIcon();
                newButton.Setup(food.Definition.ArtId, locDatabase.Localize(food.Definition.NameId), () => callback(food));

                newSelectionList.AddSelectable(newButton);
            }
            _stackPanel.Children.Add(newSelectionList);
        }

        public void AddBuySellSelection(IReadOnlyList<(Food, uint)> options, LocDatabase locDatabase, uint currentCoins, Action<Food> callback)
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
                    () => callback(food));

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
                    () => callback(cryptid));

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
                    () => callback(dnaSample));

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