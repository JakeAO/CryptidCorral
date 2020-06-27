using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using UAT_MS539.Code;
using UAT_MS539.Components;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.Food;
using UAT_MS539.Core.Code.StateMachine;
using UAT_MS539.Core.Code.StateMachine.CombatEngine;
using UAT_MS539.Core.Code.StateMachine.Interactions;
using UAT_MS539.Core.Code.StateMachine.States;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Pages
{
    public partial class MainPage : UserControl, IStateChangeHandler, IInteractionHandler
    {
        private Context _sharedContext = null;
        private Logger _logger = null;
        private LocDatabase _locDatabase = null;
        private AudioManager _audioManager = null;

        private IState _currentState = null;

        private readonly IReadOnlyDictionary<Type, string> _backgroundPathByStateType = new Dictionary<Type, string>()
        {
            {typeof(SplashState), "/Assets/Backgrounds/bg_corral.jpg"},
            {typeof(TutorialCorral), "/Assets/Backgrounds/bg_corral.jpg"},
            {typeof(TutorialNursery), "/Assets/Backgrounds/bg_town.jpg"}, // TODO Nursery BG?
            {typeof(CorralDayState), "/Assets/Backgrounds/bg_corral.jpg"},
            {typeof(CorralMorningState), "/Assets/Backgrounds/bg_corral.jpg"},
            {typeof(CorralNightState), "/Assets/Backgrounds/bg_corral.jpg"},
            {typeof(CorralCryptidEndState), "/Assets/Backgrounds/bg_corral.jpg"},
            {typeof(TownMainState), "/Assets/Backgrounds/bg_town.jpg"},
            {typeof(TownLabState), "/Assets/Backgrounds/bg_town.jpg"}, // TODO Laboratory BG?
            {typeof(TownMarketState), "/Assets/Backgrounds/bg_town.jpg"}, // TODO Market BG?
            {typeof(TownNurseryState), "/Assets/Backgrounds/bg_town.jpg"}, // TODO Nursery BG?
            {typeof(ColiseumMainState), "/Assets/Backgrounds/bg_colosseum.jpg"},
            {typeof(ColiseumBattleState), "/Assets/Backgrounds/bg_colosseum.jpg"},
            {typeof(ColiseumResultsState), "/Assets/Backgrounds/bg_colosseum.jpg"}
        };

        private readonly IReadOnlyDictionary<Type, bool> _showCryptidByStateType = new Dictionary<Type, bool>()
        {
            {typeof(InitializationState), false},
            {typeof(SplashState), false},
            {typeof(TutorialCorral), false},
            {typeof(TutorialNursery), false},
            {typeof(CorralDayState), true},
            {typeof(CorralMorningState), true},
            {typeof(CorralNightState), true},
            {typeof(CorralCryptidEndState), true},
            {typeof(TownMainState), true},
            {typeof(TownLabState), true},
            {typeof(TownMarketState), true},
            {typeof(TownNurseryState), true},
            {typeof(ColiseumMainState), true},
            {typeof(ColiseumBattleState), false},
            {typeof(ColiseumResultsState), true}
        };

        public MainPage()
        {
            InitializeComponent();
        }

        public void OnStateChanged(IState state, Context sharedContext)
        {
            _currentState = state;

            _sharedContext = sharedContext;
            _logger = sharedContext.Get<Logger>();
            _locDatabase = sharedContext.Get<LocDatabase>();
            _audioManager = sharedContext.Get<AudioManager>();

            PlayerData playerData = sharedContext.Get<PlayerData>();

            // Update Cryptid
            if (_showCryptidByStateType.TryGetValue(state.GetType(), out bool showCryptid) && showCryptid)
            {
                _cryptidDisplay.SetCryptid(playerData.ActiveCryptid, _locDatabase, _logger);
            }
            else
            {
                _cryptidDisplay.SetVisibility(false);
            }

            // Update Background
            if (_backgroundPathByStateType.TryGetValue(state.GetType(), out string bgImagePath))
            {
                Uri bgImageUri = new Uri(bgImagePath, UriKind.Relative);
                if (!(_backgroundImage.Source is BitmapImage bitmapImage) ||
                    !string.Equals(bitmapImage.BaseUri?.OriginalString, bgImageUri.OriginalString))
                {
                    try
                    {
                        _backgroundImage.Source = new BitmapImage(bgImageUri);
                    }
                    catch (Exception e)
                    {
                        _logger?.Log(Logger.LogLevel.Exception, $"{e.GetType().Name}: {e.Message} ({bgImageUri})");
                    }
                }
            }

            // Update Header Bar
            UpdateHeaderBar(playerData);
        }

        public void HandleInteraction(IReadOnlyCollection<IInteraction> interactions)
        {
            void EndInteraction()
            {
                _optionsList.ClearOptions();
                _audioManager.PlaySound(AudioManager.AudioEvent.Click);
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
                                dialogLines.Add(_locDatabase.Localize($"{nameof(EPrimaryStat)}/{(EPrimaryStat) i}") + $" +{displayTrainingResults.TrainingPoints[i]} tp");
                            }
                        }

                        break;
                    }
                    case DisplayCryptidAdvancement displayCryptidAdvancement:
                    {
                        if (displayCryptidAdvancement.HealthChange > 0)
                        {
                            dialogLines.Add(_locDatabase.Localize("Health") + $" +{displayCryptidAdvancement.HealthChange}");
                        }

                        if (displayCryptidAdvancement.StaminaChange > 0)
                        {
                            dialogLines.Add(_locDatabase.Localize("Stamina") + $" +{displayCryptidAdvancement.StaminaChange}");
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
                    case UpdatePlayerData updatePlayerData:
                    {
                        _cryptidDisplay.SetCryptid(updatePlayerData.PlayerData?.ActiveCryptid, _locDatabase, _logger);
                        UpdateHeaderBar(updatePlayerData.PlayerData);
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
                            buySellSelection.IsBuy,
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
                            _sharedContext.Get<DailyTrainingData>(),
                            _locDatabase,
                            (selectedOption) =>
                            {
                                EndInteraction();
                                trainingSelection.OptionSelectedHandler?.Invoke(selectedOption);
                            });
                        break;
                    }
                    case ColiseumOpponentSelection coliseumOpponentSelection:
                    {
                        _optionsList.AddOpponentSelection(
                            coliseumOpponentSelection.Options,
                            _locDatabase,
                            (selectedOption) =>
                            {
                                EndInteraction();
                                coliseumOpponentSelection.OptionSelectedHandler?.Invoke(selectedOption);
                            });
                        break;
                    }
                    case DisplayCombat displayCombat:
                    {
                        _optionsList.AddButton(
                            _locDatabase.Localize("Button/Skip"),
                            () =>
                            {
                                EndInteraction();
                                displayCombat.SkipCombatHandler?.Invoke();
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

        private void UpdateHeaderBar(PlayerData playerData)
        {
            if (_currentState is CorralMorningState)
                _headerMenu.Visibility = Visibility.Visible;

            _locationMenuItem.Header = _locDatabase.Localize(_currentState.LocationLocId);
            _dayMenuItem.Header = $"{_locDatabase.Localize($"Day/{playerData.Day % 7}")} {_locDatabase.Localize(_currentState.TimeLocId)}";
            _coinsMenuItem.Header = playerData.Coins.ToString();

            _foodInventoryMenuItem.Items.Clear();
            foreach (Food food in playerData.FoodInventory.OrderBy(x => x.TotalFoodQuality))
            {
                _foodInventoryMenuItem.Items.Add(new FoodInventoryMenuItem(food, _locDatabase));
            }

            _cryptidInventoryMenuItem.Items.Clear();
            foreach (Cryptid cryptid in playerData.FrozenCryptedInventory.OrderBy(x => WeightClassUtil.GetWeight(x)))
            {
                var tooltip = new CryptidTooltip();
                tooltip.SetCryptid(cryptid, _locDatabase);

                _cryptidInventoryMenuItem.Items.Add(new CryptidInventoryMenuItem(cryptid, _locDatabase, tooltip));
            }

            _dnaInventoryMenuItem.Items.Clear();
            foreach (CryptidDnaSample cryptidDnaSample in playerData.DnaSampleInventory.OrderBy(x => WeightClassUtil.GetWeight(x.Cryptid)))
            {
                var tooltip = new CryptidTooltip();
                tooltip.SetCryptid(cryptidDnaSample.Cryptid, _locDatabase);

                _dnaInventoryMenuItem.Items.Add(new CryptidInventoryMenuItem(cryptidDnaSample.Cryptid, _locDatabase, tooltip));
            }

            _trainingDataPointsMenuItem.Items.Clear();
            if (_sharedContext.TryGet(out DailyTrainingData trainingData))
            {
                if (trainingData.Food != null)
                {
                    _trainingDataFoodMenuItem.Header = _locDatabase.Localize(trainingData.Food.Definition.NameId);
                    _trainingDataFoodMenuItem.ToolTip = new Label()
                    {
                        Content = string.Join("\n", TooltipUtil.GetTooltipContent(trainingData.Food, _locDatabase))
                    };
                    _trainingDataFoodMenuItem.Icon = new Image()
                    {
                        Source = new BitmapImage(new Uri(trainingData.Food.Definition.ArtId, UriKind.Relative)),
                        Margin = new Thickness(-10)
                    };
                    _trainingDataFoodMenuItem.Visibility = Visibility.Visible;
                }
                else
                {
                    _trainingDataFoodMenuItem.Visibility = Visibility.Hidden;
                }

                for (EPrimaryStat stat = EPrimaryStat.Strength; stat < EPrimaryStat._Count; stat++)
                {
                    if (trainingData.Points[(int) stat] > 0)
                    {
                        _trainingDataPointsMenuItem.Items.Add(new MenuItem()
                        {
                            Header = $"{_locDatabase.Localize(stat)}: {trainingData.Points[(int) stat]} tp"
                        });
                    }
                }

                _trainingDataMenuItem.Visibility = Visibility.Visible;
            }
            else
            {
                _trainingDataMenuItem.Visibility = Visibility.Hidden;
            }
        }
    }
}