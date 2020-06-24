using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.Food;
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
            if (state is CorralMorningState)
                Console.WriteLine($"========== Day {_sharedContext.Get<PlayerData>().Day} Start ==========");
        }

        private void OnInteractionEventRaised(IReadOnlyCollection<IInteraction> interactions)
        {
            var locDatabase = _sharedContext.Get<LocDatabase>();

            var userPrompts = new List<(string, Action)>(5);
            foreach (var interaction in interactions)
            {
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
                    case DisplayCryptidAdvancement cryptidAdvancement:
                    {
                        var cryptid = cryptidAdvancement.Cryptid;

                        var sb = new StringBuilder(100);
                        sb.AppendLine("[Cryptid Growth] = = = = = = =");
                        if (cryptidAdvancement.HealthChange > 0)
                            sb.AppendLine($"      Health: {cryptid.MaxHealth - cryptidAdvancement.HealthChange} +{cryptidAdvancement.HealthChange}");
                        if (cryptidAdvancement.StaminaChange > 0)
                            sb.AppendLine($"      Stamina: {cryptid.MaxStamina - cryptidAdvancement.StaminaChange} +{cryptidAdvancement.StaminaChange}");
                        for (var i = 0; i < (int) EPrimaryStat._Count; i++)
                            sb.AppendLine(
                                cryptidAdvancement.PrimaryStatChanges[i] > 0
                                    ? $"      {((EPrimaryStat) i).ToString()}: {cryptid.PrimaryStats[i] - cryptidAdvancement.PrimaryStatChanges[i]} +{cryptidAdvancement.PrimaryStatChanges[i]}"
                                    : $"      {((EPrimaryStat) i).ToString()}: {cryptid.PrimaryStats[i]}");
                        sb.AppendLine("= = = = = = = = = = = = = = = =");

                        Console.WriteLine(sb.ToString());
                        break;
                    }
                    case DisplayTrainingResults trainingResults:
                    {
                        var sb = new StringBuilder(100);
                        sb.AppendLine("[Training Results] = = = = = = =");
                        for (var i = 0; i < (int) EPrimaryStat._Count; i++)
                        {
                            if (trainingResults.TrainingPoints[i] > 0)
                            {
                                sb.AppendLine($"   {(EPrimaryStat) i}: +{trainingResults.TrainingPoints[i]} tp");
                            }
                        }

                        sb.AppendLine("= = = = = = = = = = = = = = = =");

                        Console.WriteLine(sb.ToString());
                        break;
                    }
                    case RunePatternSelection runePatternSelection:
                    {
                        foreach (var option in runePatternSelection.Options)
                        {
                            userPrompts.Add((option.RunicHash, () => runePatternSelection.OptionSelectedHandler(option)));
                        }

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
                    case BuySellSelection buySellSelection:
                    {
                        foreach (var foodCostPair in buySellSelection.Options)
                        {
                            Food food = foodCostPair.Item1;
                            uint cost = foodCostPair.Item2;

                            var sb = new StringBuilder(50);
                            sb.Append($"{cost} coins -> ");
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

                            userPrompts.Add((sb.ToString(), () => buySellSelection.OptionSelectedHandler(foodCostPair)));
                        }

                        break;
                    }
                    case CryptidSelection cryptidSelection:
                    {
                        foreach (var cryptid in cryptidSelection.Options)
                        {
                            var sb = new StringBuilder(50);
                            sb.Append(locDatabase.Localize(cryptid.Species.NameId));
                            sb.Append($" HP: {cryptid.MaxHealth}, STA: {cryptid.MaxStamina}");
                            sb.Append($" P[{string.Join(", ", cryptid.PrimaryStats)}]");
                            sb.Append($" Age: {cryptid.AgeInDays}");

                            userPrompts.Add((sb.ToString(), () => cryptidSelection.OptionSelectedHandler(cryptid)));
                        }

                        break;
                    }
                    case DnaSampleSelection dnaSampleSelection:
                    {
                        foreach (var dnaSample in dnaSampleSelection.Options)
                        {
                            var sb = new StringBuilder(50);
                            sb.Append(locDatabase.Localize(dnaSample.Cryptid.Species.NameId));
                            sb.Append($" HP: {dnaSample.Cryptid.MaxHealth}, STA: {dnaSample.Cryptid.MaxStamina}");
                            sb.Append($" P[{string.Join(", ", dnaSample.Cryptid.PrimaryStats)}]");
                            sb.Append($" Hash: {dnaSample.Cryptid.RunicHash}");

                            userPrompts.Add((sb.ToString(), () => dnaSampleSelection.OptionSelectedHandler(dnaSample)));
                        }

                        break;
                    }
                    case TrainingSelection trainingSelection:
                    {
                        foreach (var trainingRegimen in trainingSelection.Options)
                        {
                            var sb = new StringBuilder(50);
                            sb.Append($"{trainingRegimen.StaminaCost} stamina -> ");
                            sb.Append(locDatabase.Localize(trainingRegimen.NameId));
                            for (var i = 0; i < (int) EPrimaryStat._Count; i++)
                            {
                                uint minPoints = 0, maxPoints = 0;

                                if (trainingRegimen.RandomStatIncreases.TryGetValue((EPrimaryStat) i, out var statBoost))
                                {
                                    minPoints = statBoost.Points.Min(x => x.Value);
                                    maxPoints = statBoost.Points.Max(x => x.Value);
                                }

                                if (trainingRegimen.GuaranteedStatIncrease.TryGetValue((EPrimaryStat) i, out var guaranteedPoints))
                                {
                                    minPoints += guaranteedPoints;
                                    maxPoints += guaranteedPoints;
                                }

                                if (minPoints != maxPoints)
                                {
                                    sb.Append($", {(EPrimaryStat) i} +{minPoints}-{maxPoints} exp");
                                }
                                else if (minPoints != 0)
                                {
                                    sb.Append($", {(EPrimaryStat) i} +{minPoints} exp");
                                }
                            }

                            userPrompts.Add((sb.ToString(), () => trainingSelection.OptionSelectedHandler(trainingRegimen)));
                        }

                        break;
                    }
                    default:
                    {
                        Console.WriteLine($"Unhandled IInteraction type: {interaction.GetType()}");
                        break;
                    }
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