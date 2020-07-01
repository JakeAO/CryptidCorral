using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AppWPF.Code;
using AppWPF.Pages;
using Core.Code.StateMachine;
using Core.Code.StateMachine.Interactions;
using Core.Code.StateMachine.Signals;
using Core.Code.StateMachine.States;

namespace AppWPF
{
    public partial class MainWindow : Window
    {
        private readonly Context _sharedContext = null;
        private readonly StateMachine _stateMachine = null;
        private readonly Logger _logger = null;

        private IState _pendingStateChange = null;
        private readonly ConcurrentQueue<IReadOnlyCollection<IInteraction>> _interactionEventBuffer = new ConcurrentQueue<IReadOnlyCollection<IInteraction>>();

        private readonly IReadOnlyDictionary<Type, Type> _stateTypeToUserControlType = new Dictionary<Type, Type>()
        {
            {typeof(InitializationState), typeof(SplashPage)},
            {typeof(SplashState), typeof(SplashPage)},
            {typeof(TutorialCorral), typeof(MainPage)},
            {typeof(TutorialNursery), typeof(MainPage)},
            {typeof(CorralDayState), typeof(MainPage)},
            {typeof(CorralMorningState), typeof(MainPage)},
            {typeof(CorralNightState), typeof(MainPage)},
            {typeof(CorralCryptidEndState), typeof(MainPage)},
            {typeof(TownMainState), typeof(MainPage)},
            {typeof(TownLabState), typeof(MainPage)},
            {typeof(TownMarketState), typeof(MainPage)},
            {typeof(TownNurseryState), typeof(MainPage)},
            {typeof(ColiseumMainState), typeof(MainPage)},
            {typeof(ColiseumBattleState), typeof(CombatPage)},
            {typeof(ColiseumResultsState), typeof(MainPage)}
        };

        private UserControl _currentControl = null;

        public MainWindow()
        {
            InitializeComponent();

            Application.Current.MainWindow = this;
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            CompositionTarget.Rendering += OnRenderTick;

            _logger = new Logger();

            var stateChangedSignal = new StateChanged();
            stateChangedSignal.Listen(OnStateChanged);

            var interactionEventRaisedSignal = new InteractionEventRaised();
            interactionEventRaisedSignal.Listen(OnInteractionEventRaised);

            Uri GenerateAbsoluteUri(string relativePath)
            {
                return new Uri(
                    new Uri(AppDomain.CurrentDomain.BaseDirectory),
                    relativePath.Remove(0, 1));
            }

            var audioManager = new AudioManager(_logger,
                (AudioManager.AudioEvent.Click, new[]
                {
                    GenerateAbsoluteUri("/Assets/Audio/Click.wav"),
                }),
                (AudioManager.AudioEvent.Hit, new[]
                {
                    GenerateAbsoluteUri("/Assets/Audio/Hit1.wav"),
                    GenerateAbsoluteUri("/Assets/Audio/Hit2.wav"),
                    GenerateAbsoluteUri("/Assets/Audio/Hit3.wav"),
                    GenerateAbsoluteUri("/Assets/Audio/Hit4.wav"),
                    GenerateAbsoluteUri("/Assets/Audio/Hit5.wav"),
                }),
                (AudioManager.AudioEvent.CriticalHit, new[]
                {
                    GenerateAbsoluteUri("/Assets/Audio/Crit1.wav"),
                    GenerateAbsoluteUri("/Assets/Audio/Crit2.wav"),
                    GenerateAbsoluteUri("/Assets/Audio/Crit3.wav"),
                }),
                (AudioManager.AudioEvent.Miss, new[]
                {
                    GenerateAbsoluteUri("/Assets/Audio/Miss1.wav"),
                    GenerateAbsoluteUri("/Assets/Audio/Miss2.wav"),
                }),
                (AudioManager.AudioEvent.Defeat, new[]
                {
                    GenerateAbsoluteUri("/Assets/Audio/Defeat.wav"),
                }));

            _sharedContext = new Context(stateChangedSignal, interactionEventRaisedSignal, audioManager, _logger);
            _stateMachine = new StateMachine(_sharedContext);
        }

        ~MainWindow()
        {
            CompositionTarget.Rendering -= OnRenderTick;
        }

        private void OnRenderTick(object sender, EventArgs e)
        {
            // Handle State Change
            if (_pendingStateChange != null)
            {
                Type stateType = _pendingStateChange.GetType();
                if (_stateTypeToUserControlType.TryGetValue(stateType, out Type userControlType) &&
                    userControlType != null &&
                    _currentControl?.GetType() != userControlType)
                {
                    try
                    {
                        _currentControl = (UserControl) userControlType.GetConstructor(Type.EmptyTypes)?.Invoke(new object[0]);
                    }
                    catch (Exception ex)
                    {
                        _logger?.Log(Logger.LogLevel.Exception, $"{ex.GetType().Name}: {ex.Message} ({userControlType.Name})");
                    }
                }

                _contentControl.Content = _currentControl;
                if (_currentControl is IStateChangeHandler stateInjected)
                {
                    _logger?.Log(Logger.LogLevel.Debug, $"--> StateChange sent: {stateType.Name} / {stateInjected.GetType().Name}");
                    stateInjected.OnStateChanged(_pendingStateChange, _sharedContext);
                }

                _pendingStateChange = null;
            }

            // Handle Interactions
            if (_currentControl is IInteractionHandler interactionHandler)
            {
                if (_interactionEventBuffer.TryDequeue(out var interactions))
                {
                    _logger?.Log(Logger.LogLevel.Debug, $"--> InteractionEvents sent: {interactionHandler.GetType().Name} / {string.Join(", ", interactions.Select(x => x.GetType().Name))}");
                    interactionHandler.HandleInteraction(interactions);
                }
            }
        }

        private void OnStateChanged(IState newState)
        {
            _logger?.Log(Logger.LogLevel.Debug, $"<-- StateChange received: {newState.GetType().Name}");
            _pendingStateChange = newState;
        }

        private void OnInteractionEventRaised(IReadOnlyCollection<IInteraction> interactions)
        {
            _logger?.Log(Logger.LogLevel.Debug, $"<-- InteractionEvents received: {string.Join(", ", interactions.Select(x => x.GetType().Name))}");
            _interactionEventBuffer.Enqueue(interactions);
        }

        private void OnCloseClicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnBorderMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}