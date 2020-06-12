using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using UAT_MS539.Code;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.StateMachine;
using UAT_MS539.Core.Code.StateMachine.Interactions;
using UAT_MS539.Core.Code.StateMachine.States;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Pages
{
    public partial class MainPage : UserControl, IStateChangeHandler, IInteractionHandler
    {
        private Context _sharedContext = null;
        private LocDatabase _locDatabase = null;

        private readonly IReadOnlyDictionary<Type, string> _backgroundPathByStateType = new Dictionary<Type, string>()
        {
            {typeof(SplashState), "/Assets/Backgrounds/bg_corral.jpg"},
            {typeof(TutorialCorral), "/Assets/Backgrounds/bg_corral.jpg"},
            {typeof(TutorialNursery), "/Assets/Backgrounds/bg_town.jpg"}, // TODO Nursery BG?
            {typeof(CorralDayState), "/Assets/Backgrounds/bg_corral.jpg"},
            {typeof(CorralMorningState), "/Assets/Backgrounds/bg_corral.jpg"},
            {typeof(CorralNightState), "/Assets/Backgrounds/bg_corral.jpg"},
            {typeof(TownMainState), "/Assets/Backgrounds/bg_town.jpg"},
            {typeof(TownLabState), "/Assets/Backgrounds/bg_town.jpg"}, // TODO Laboratory BG?
            {typeof(TownMarketState), "/Assets/Backgrounds/bg_town.jpg"}, // TODO Market BG?
            {typeof(TownNurseryState), "/Assets/Backgrounds/bg_town.jpg"}, // TODO Nursery BG?
            {typeof(ColiseumMainState), "/Assets/Backgrounds/bg_colosseum.jpg"},
            {typeof(ColiseumBattleState), "/Assets/Backgrounds/bg_colosseum.jpg"},
            {typeof(ColiseumResultsState), "/Assets/Backgrounds/bg_colosseum.jpg"}
        };

        public MainPage()
        {
            InitializeComponent();
        }

        public void OnStateChanged(IState state, Context sharedContext)
        {
            _sharedContext = sharedContext;
            _locDatabase = sharedContext.Get<LocDatabase>();

            PlayerData playerData = sharedContext.Get<PlayerData>();

            _timeDayLocationDock.Visibility = System.Windows.Visibility.Visible;
            _timeLabel.Content = _locDatabase.Localize(state.TimeLocId);
            _dayLabel.Content = _locDatabase.Localize($"Day/{playerData.Day % 7}");
            _locationLabel.Content = _locDatabase.Localize(state.LocationLocId);
            _currencyLabel.Content = playerData.Coins.ToString();

            _cryptidDisplay.SetCryptid(playerData.ActiveCryptid, _locDatabase);

            if (_backgroundPathByStateType.TryGetValue(state.GetType(), out string bgImagePath))
            {
                Uri bgImageUri = new Uri(bgImagePath, UriKind.Relative);
                if (!(_backgroundImage.Source is BitmapImage bitmapImage) ||
                    !string.Equals(bitmapImage.BaseUri?.OriginalString, bgImageUri.OriginalString))
                {
                    _backgroundImage.Source = new BitmapImage(bgImageUri);
                }
            }
        }

        public void HandleInteraction(IReadOnlyCollection<IInteraction> interactions)
        {
            void EndInteraction()
            {
                _optionsList.ClearOptions();
                _dialogBox.Visibility = System.Windows.Visibility.Hidden;
            }

            List<string> dialogLines = new List<string>(5);
            foreach (IInteraction interaction in interactions)
            {
                switch (interaction)
                {
                    case Dialog dialog:
                    {
                        dialogLines.Add(_locDatabase.Localize(dialog.LocId, dialog.LocParams));
                        break;
                    }
                    case DisplayTrainingResults displayTrainingResults:
                    {
                        for (int i = 0; i < (int) EPrimaryStat._Count; i++)
                        {
                            if (displayTrainingResults.TrainingPoints[i] > 0)
                            {
                                dialogLines.Add(_locDatabase.Localize($"{nameof(EPrimaryStat)}/{(EPrimaryStat) i}") + $" +{displayTrainingResults.TrainingPoints[i]} exp");
                            }
                        }
                        break;
                    }
                    case DisplayCryptidAdvancement displayCryptidAdvancement:
                    {
                        for (int i = 0; i < (int) ESecondaryStat._Count; i++)
                        {
                            if (displayCryptidAdvancement.SecondaryStatChanges[i] > 0)
                            {
                                dialogLines.Add(_locDatabase.Localize($"{nameof(ESecondaryStat)}/{(ESecondaryStat) i}") + $" +{displayCryptidAdvancement.SecondaryStatChanges[i]}");
                            }
                        }
                        for (int i = 0; i < (int) EPrimaryStat._Count; i++)
                        {
                            if (displayCryptidAdvancement.PrimaryStatChanges[i] > 0)
                            {
                                dialogLines.Add(_locDatabase.Localize($"{nameof(EPrimaryStat)}/{(EPrimaryStat) i}") + $" +{displayCryptidAdvancement.PrimaryStatChanges[i]}");
                            }
                        }
                        break;
                    }
                    case DisplayCoins displayCoins:
                    {
                        _currencyLabel.Content = displayCoins.Coins.ToString();
                        break;
                    }
                    case DisplayCryptid displayCryptid:
                    {
                        _cryptidDisplay.SetCryptid(displayCryptid.Cryptid, _locDatabase);
                        break;
                    }
                    case Option option:
                    {
                        _optionsList.AddButton(
                            _locDatabase.Localize(option.LocId),
                            () =>
                            {
                                EndInteraction();
                                option.ActionHandler?.Invoke();
                            });
                        break;
                    }
                    case RunePatternSelection runePatternSelection:
                    {
                        _optionsList.AddRuneSelection(
                            runePatternSelection.Options,
                            _sharedContext.Get<RuneDatabase>(),
                            (selectedOption) =>
                            {
                                EndInteraction();
                                runePatternSelection.OptionSelectedHandler?.Invoke(selectedOption);
                            });
                        break;
                    }
                    case FoodSelection foodSelection:
                    {
                        _optionsList.AddFoodSelection(
                            foodSelection.Options,
                            _locDatabase,
                            (selectedOption) =>
                            {
                                EndInteraction();
                                foodSelection.OptionSelectedHandler?.Invoke(selectedOption);
                            });
                        break;
                    }
                    case BuySellSelection buySellSelection:
                    {
                        _optionsList.AddBuySellSelection(
                            buySellSelection.Options,
                            _locDatabase,
                            _sharedContext.Get<PlayerData>().Coins,
                            (selectedOption) =>
                            {
                                EndInteraction();
                                buySellSelection.OptionSelectedHandler?.Invoke(selectedOption);
                            });
                        break;
                    }
                    case CryptidSelection cryptidSelection:
                    {
                        _optionsList.AddCryptidSelection(
                            cryptidSelection.Options,
                            _locDatabase,
                            (selectedOption) =>
                            {
                                EndInteraction();
                                cryptidSelection.OptionSelectedHandler?.Invoke(selectedOption);
                            });
                        break;
                    }
                    case DnaSampleSelection dnaSampleSelection:
                    {
                        _optionsList.AddDnaSampleSelection(
                            dnaSampleSelection.Options,
                            _locDatabase,
                            (selectedOption) =>
                            {
                                EndInteraction();
                                dnaSampleSelection.OptionSelectedHandler?.Invoke(selectedOption);
                            });
                        break;
                    }
                    case TrainingSelection trainingSelection:
                    {
                        _optionsList.AddTrainingSelection(
                            trainingSelection.Options,
                            _locDatabase,
                            (selectedOption) =>
                            {
                                EndInteraction();
                                trainingSelection.OptionSelectedHandler?.Invoke(selectedOption);
                            });
                        break;
                    }
                }
            }

            if (dialogLines.Count > 0)
            {
                _dialogBox.SetLabel(string.Join(Environment.NewLine, dialogLines));
                _dialogBox.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }
}