using System;
using System.Collections.Generic;
using System.Text;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.StateMachine;
using UAT_MS539.Core.Code.StateMachine.Interactions;
using UAT_MS539.Core.Code.StateMachine.Signals;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.ConsoleApp
{
    public class MainClass
    {
        private readonly Context _sharedContext;
        private readonly StateMachine _stateMachine;

        public MainClass()
        {
            StateChanged stateChangedSignal = new StateChanged();
            stateChangedSignal.Listen(OnStateChanged);

            InteractionEventRaised interactionEventRaisedSignal = new InteractionEventRaised();
            interactionEventRaisedSignal.Listen(OnInteractionEventRaised);

            _sharedContext = new Context(stateChangedSignal, interactionEventRaisedSignal);
            _stateMachine = new StateMachine(_sharedContext);
        }

        private void OnStateChanged(IState state)
        {
            Console.WriteLine($"[State Changed] {state.GetType()}");
            Console.WriteLine();
        }

        private void OnInteractionEventRaised(IReadOnlyCollection<IInteraction> interactions)
        {
            LocDatabase locDatabase = _sharedContext.Get<LocDatabase>();

            List<(string, Action)> userPrompts = new List<(string, Action)>(5);
            foreach (IInteraction interaction in interactions)
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
                    case DisplayCryptid displayCryptid:
                    {
                        Cryptid cryptid = displayCryptid.Cryptid;

                        StringBuilder sb = new StringBuilder(100);
                        sb.AppendLine("[Cryptid] = = = = = = = =");
                        sb.AppendLine($"   Species: {locDatabase.Localize(cryptid.Species.NameId)}");
                        sb.AppendLine($"   Pattern: {locDatabase.Localize(cryptid.Pattern.NameId)}");
                        sb.AppendLine($"   Color: {locDatabase.Localize(cryptid.Color.NameId)}");
                        sb.AppendLine("   Stats:");
                        for (int i = 0; i < (int) EPrimaryStat._Count; i++)
                        {
                            sb.AppendLine($"      {((EPrimaryStat) i).ToString()}: {cryptid.PrimaryStats[i]} ({cryptid.PrimaryStatExp[i]}/100)");
                        }

                        sb.AppendLine("   Secondary Stats:");
                        for (int i = 0; i < (int) ESecondaryStat._Count; i++)
                        {
                            sb.AppendLine($"      {((ESecondaryStat) i).ToString()}: {cryptid.SecondaryStats[i]}");
                        }

                        sb.AppendLine("= = = = = = = = =");

                        Console.WriteLine(sb);
                        break;
                    }
                    case ListOption listOption:
                    {
                        foreach (string option in listOption.Options)
                        {
                            userPrompts.Add((option, () => listOption.OptionSelectedHandler(option)));
                        }

                        break;
                    }
                }
            }

            Console.WriteLine();

            if (userPrompts.Count > 0)
            {
                Console.WriteLine("<< Select Option by Id >>");
                for (int i = 0; i < userPrompts.Count; i++)
                {
                    Console.WriteLine($"[{i}] {userPrompts[i].Item1}");
                }

                int selectedOptionIndex = -1;
                do
                {
                    string readLine = Console.ReadLine();
                    if (!int.TryParse(readLine, out int potentialOptionIndex))
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