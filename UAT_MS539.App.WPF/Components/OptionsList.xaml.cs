using System;
using System.Collections.Generic;
using System.Windows.Controls;
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

                ButtonWithIcon newButton = new ButtonWithIcon();
                newButton.Setup(food.Definition.ArtId, locDatabase.Localize(food.Definition.NameId), () => callback(food));

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