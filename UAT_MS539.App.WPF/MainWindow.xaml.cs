using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UAT_MS539.Code;
using UAT_MS539.Core.Code.StateMachine;
using UAT_MS539.Core.Code.StateMachine.Interactions;
using UAT_MS539.Core.Code.StateMachine.Signals;
using UAT_MS539.Core.Code.StateMachine.States;
using UAT_MS539.Pages;

namespace UAT_MS539
{
    public partial class MainWindow : Window
    {
        private readonly Context _sharedContext = null;
        private readonly StateMachine _stateMachine = null;

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

            CompositionTarget.Rendering += OnRenderTick;

            var stateChangedSignal = new StateChanged();
            stateChangedSignal.Listen(OnStateChanged);

            var interactionEventRaisedSignal = new InteractionEventRaised();
            interactionEventRaisedSignal.Listen(OnInteractionEventRaised);

            Uri GenerateAbsoluteUri(string relativePath)
            {
                return new Uri(
                    Path.Combine(
                        new Uri(AppDomain.CurrentDomain.BaseDirectory).PathAndQuery,
                        relativePath.Remove(0, 1)),
                    UriKind.Absolute);
            }

            var audioManager = new AudioManager(
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

            _sharedContext = new Context(stateChangedSignal, interactionEventRaisedSignal, audioManager);
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
                if (_stateTypeToUserControlType.TryGetValue(stateType, out Type userControlType))
                {
                    if (_currentControl == null || _currentControl.GetType() != userControlType)
                    {
                        _currentControl = (UserControl)userControlType.GetConstructor(Type.EmptyTypes)?.Invoke(new object[0]);
                    }
                }

                contentControl.Content = _currentControl;
                if (_currentControl is IStateChangeHandler stateInjected)
                {
                    stateInjected.OnStateChanged(_pendingStateChange, _sharedContext);
                }

                _pendingStateChange = null;
            }

            // Handle Interactions
            if (_currentControl is IInteractionHandler interactionHandler)
            {
                if (_interactionEventBuffer.TryDequeue(out var interactions))
                {
                    interactionHandler.HandleInteraction(interactions);
                }
            }
        }

        private void OnStateChanged(IState newState)
        {
            _pendingStateChange = newState;
        }

        private void OnInteractionEventRaised(IReadOnlyCollection<IInteraction> interactions)
        {
            _interactionEventBuffer.Enqueue(interactions);
        }
    }
}