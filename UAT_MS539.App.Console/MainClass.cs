using System;
using System.Collections.Generic;
using System.Text;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.StateMachine;
using UAT_MS539.Core.Code.StateMachine.Interactions;
using UAT_MS539.Core.Code.StateMachine.Signals;
using UAT_MS539.Core.Code.StateMachine.States;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.ConsoleApp
{
    public class MainClass
    {
        private readonly Context _sharedContext;
        private readonly StateMachine _stateMachine;

        public MainClass()
        {
            var stateChangedSignal = new StateChanged();
            stateChangedSignal.Listen(OnStateChanged);

            var interactionEventRaisedSignal = new InteractionEventRaised();
            interactionEventRaisedSignal.Listen(OnInteractionEventRaised);

            _sharedContext = new Context(stateChangedSignal, interactionEventRaisedSignal);
            _stateMachine = new StateMachine(_sharedContext);
        }

        private void OnStateChanged(IState state)
        {
            Console.WriteLine($"[State Changed] {state.GetType().Name}");
            if (state is CorralMorningState) Console.WriteLine($"========== Day {_sharedContext.Get<PlayerData>().Day} Start ==========");
        }

        private void OnInteractionEventRaised(IReadOnlyCollection<IInteraction> interactions)
        {
            var locDatabase = _sharedContext.Get<LocDatabase>();

            var userPrompts = new List<(string, Action)>(5);
            foreach (var interaction in interactions)
                switch (interaction)
                {
                    case Dialog dialog:
                    {
                        Console.WriteLine($"[Dialog] \"{locDatabase.Localize(dialog.LocId, dialog.LocParams)}\"");
                        break;
                    }
                    case Option option:
                    {
                        userPrompts.Add((locDatabase.Localize(option.LocId), option.ActionHandler));
                        break;
                    }
                    case DisplayCryptid displayCryptid:
                    {
                        var cryptid = displayCryptid.Cryptid;

                        var sb = new StringBuilder(100);
                        sb.AppendLine("[Cryptid] = = = = = = = =");
                        sb.AppendLine($"   Species: {locDatabase.Localize(cryptid.Species.NameId)}");
                        sb.AppendLine($"   Pattern: {locDatabase.Localize(cryptid.Pattern.NameId)}");
                        sb.AppendLine($"   Color: {locDatabase.Localize(cryptid.Color.NameId)}");
                        sb.AppendLine("   Stats:");
                        for (var i = 0; i < (int) EPrimaryStat._Count; i++) sb.AppendLine($"      {((EPrimaryStat) i).ToString()}: {cryptid.PrimaryStats[i]} ({cryptid.PrimaryStatExp[i]}/100)");
                        sb.AppendLine("   Secondary Stats:");
                        for (var i = 0; i < (int) ESecondaryStat._Count; i++) sb.AppendLine($"      {((ESecondaryStat) i).ToString()}: {cryptid.SecondaryStats[i]}");
                        sb.AppendLine("= = = = = = = = = = = = =");

                        Console.WriteLine(sb);
                        break;
                    }
                    case RunePatternSelection runePatternSelection:
                    {
                        foreach (var option in runePatternSelection.Options) userPrompts.Add((option, () => runePatternSelection.OptionSelectedHandler(option)));

                        break;
                    }
                    case FoodSelection foodSelection:
                    {
                        foreach (var food in foodSelection.Options)
                        {
                            var sb = new StringBuilder(50);
                            sb.Append(locDatabase.Localize(food.Definition.NameId));
                            for (var i = 0; i < (int) EPrimaryStat._Count; i++)
                            {
                                var hasMult = food.MultipliersByStat.TryGetValue((EPrimaryStat) i, out var statMultiplier) && statMultiplier != 0f;
                                var hasBoost = food.BoostsByStat.TryGetValue((EPrimaryStat) i, out var statBoost) && statBoost > 0;

                                if (hasMult || hasBoost) sb.Append($", {(EPrimaryStat) i}");

                                if (hasMult) sb.Append($" x{statMultiplier:P0}");

                                if (hasBoost) sb.Append($" +{statBoost}");
                            }

                            if (food.MoraleBoost != 0) sb.Append($", Morale +{food.MoraleBoost}");

                            userPrompts.Add((sb.ToString(), () => foodSelection.OptionSelectedHandler(food)));
                        }

                        break;
                    }
                    case DisplayCryptidAdvancement cryptidAdvancement:
                    {
                        var cryptid = cryptidAdvancement.Cryptid;

                        var sb = new StringBuilder(100);
                        sb.AppendLine("[Cryptid Growth] = = = = = = =");
                        for (var i = 0; i < (int) ESecondaryStat._Count; i++)
                            sb.AppendLine(
                                cryptidAdvancement.SecondaryStatChanges[i] > 0
                                    ? $"      {((ESecondaryStat) i).ToString()}: {cryptid.SecondaryStats[i] - cryptidAdvancement.SecondaryStatChanges[i]} +{cryptidAdvancement.SecondaryStatChanges[i]}"
                                    : $"      {((ESecondaryStat) i).ToString()}: {cryptid.SecondaryStats[i]}");
                        for (var i = 0; i < (int) EPrimaryStat._Count; i++)
                            sb.AppendLine(
                                cryptidAdvancement.PrimaryStatChanges[i] > 0
                                    ? $"      {((EPrimaryStat) i).ToString()}: {cryptid.PrimaryStats[i] - cryptidAdvancement.PrimaryStatChanges[i]} +{cryptidAdvancement.PrimaryStatChanges[i]}"
                                    : $"      {((EPrimaryStat) i).ToString()}: {cryptid.PrimaryStats[i]}");
                        sb.AppendLine("= = = = = = = = = = = = = = = =");

                        Console.WriteLine(sb.ToString());
                        break;
                    }
                }

            if (userPrompts.Count > 0)
            {
                Console.WriteLine("<< Select Option by Id >>");
                for (var i = 0; i < userPrompts.Count; i++) Console.WriteLine($"[{i}] {userPrompts[i].Item1}");

                var selectedOptionIndex = -1;
                do
                {
                    var readLine = Console.ReadLine();
                    if (!int.TryParse(readLine, out var potentialOptionIndex))
                        continue;
                    if (potentialOptionIndex < 0 || potentialOptionIndex >= userPrompts.Count)
                        continue;
                    selectedOptionIndex = potentialOptionIndex;
                } while (selectedOptionIndex < 0);

                Console.WriteLine();

                userPrompts[selectedOptionIndex].Item2.Invoke();
            }
        }
    }
}