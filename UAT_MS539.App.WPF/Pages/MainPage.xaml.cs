using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using UAT_MS539.Code;
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

            _cryptidDisplay.SetCryptid(playerData.ActiveCryptid);

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

            LocDatabase locDatabase = _sharedContext.Get<LocDatabase>();

            foreach (IInteraction interaction in interactions)
            {
                switch (interaction)
                {
                    case Dialog dialog:
                    {
                        _dialogBox.SetLabel(locDatabase.Localize(dialog.LocId, dialog.LocParams));
                        _dialogBox.Visibility = System.Windows.Visibility.Visible;
                        break;
                    }
                    case DisplayCoins displayCoins:
                    {
                        _currencyLabel.Content = displayCoins.Coins.ToString();
                        break;
                    }
                    case DisplayCryptid displayCryptid:
                    {
                        _cryptidDisplay.SetCryptid(displayCryptid.Cryptid);
                        break;
                    }
                    case Option option:
                    {
                        _optionsList.AddButton(
                            locDatabase.Localize(option.LocId),
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
                            _sharedContext.Get<LocDatabase>(),
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
                            _sharedContext.Get<LocDatabase>(),
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
                            _sharedContext.Get<LocDatabase>(),
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
                            _sharedContext.Get<LocDatabase>(),
                            (selectedOption) =>
                            {
                                EndInteraction();
                                dnaSampleSelection.OptionSelectedHandler?.Invoke(selectedOption);
                            });
                        break;
                    }
                }
            }
        }
    }
}