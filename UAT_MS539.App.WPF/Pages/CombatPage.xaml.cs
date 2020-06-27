using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UAT_MS539.Code;
using UAT_MS539.Core.Code.Cryptid;
using UAT_MS539.Core.Code.Extensions;
using UAT_MS539.Core.Code.StateMachine;
using UAT_MS539.Core.Code.StateMachine.CombatEngine;
using UAT_MS539.Core.Code.StateMachine.Interactions;
using UAT_MS539.Core.Code.StateMachine.States;
using UAT_MS539.Core.Code.Utility;

namespace UAT_MS539.Pages
{
    public partial class CombatPage : UserControl, IStateChangeHandler, IInteractionHandler
    {
        private const float MIN_DISPLAY_TIME = 0.5f;
        private const float MAX_DISPLAY_TIME = 1.25f;

        private readonly Random _random = new Random();

        private Logger _logger = null;
        private AudioManager _audioManager = null;
        private LocDatabase _locDatabase = null;
        private PlayerData _playerData = null;

        private ColiseumBattleState _battleState = null;

        private DisplayCombat _displayCombat = null;
        private CombatData _combatData = null;
        private Queue<CombatEngine.CombatEvent> _combatEventsQueue = null;
        private Action _onSkipAction = null;

        private float _maxHealth1 = 100;
        private float _currentHealth1 = 100;
        private float _maxHealth2 = 100;
        private float _currentHealth2 = 100;

        private DateTime _lastDisplayTime = DateTime.UtcNow;
        private float _displayDelay = 1f;
        private SolidColorBrush _activeHitBrush = null;

        public CombatPage()
        {
            InitializeComponent();

            CompositionTarget.Rendering += OnRenderTick;
        }

        ~CombatPage()
        {
            CompositionTarget.Rendering -= OnRenderTick;
        }

        public void OnStateChanged(IState state, Context sharedContext)
        {
            Debug.Assert(state is ColiseumBattleState);

            _logger = sharedContext.Get<Logger>();
            _audioManager = sharedContext.Get<AudioManager>();
            _locDatabase = sharedContext.Get<LocDatabase>();
            _playerData = sharedContext.Get<PlayerData>();

            _battleState = (ColiseumBattleState) state;
        }

        public void HandleInteraction(IReadOnlyCollection<IInteraction> interactions)
        {
            foreach (IInteraction interaction in interactions)
            {
                if (interaction is DisplayCombat displayCombat)
                {
                    DisplayCombat(displayCombat);
                }
            }
        }

        private void DisplayCombat(DisplayCombat combat)
        {
            _displayCombat = combat;
            _combatData = combat.CombatData;
            _combatEventsQueue = new Queue<CombatEngine.CombatEvent>(combat.CombatEvents);
            _onSkipAction = combat.SkipCombatHandler;

            _optionsList.AddButton(_locDatabase.Localize("Button/Skip"), OnSkipPressed);

            Cryptid playerCombatant = _combatData.Combatants.First(x => x == _playerData.ActiveCryptid);
            _currentHealth1 = _combatData.StartingHealthTotals[playerCombatant];
            _maxHealth1 = playerCombatant.MaxHealth;
            _cryptidDisplay1.SetCryptid(playerCombatant, _locDatabase, _logger, true);
            _cryptidHealth1.Maximum = _maxHealth1;
            _cryptidHealth1.Value = _currentHealth1;
            _cryptidDamageIndicator1.Visibility = Visibility.Hidden;
            _cryptidDamageIndicatorLabel1.Text = string.Empty;
            _cryptidDamageIndicatorLabel1.Foreground = new SolidColorBrush(Colors.Gray);

            Cryptid opponentCombatant = _combatData.Combatants.Where(x => x != _playerData.ActiveCryptid).OrderBy(x => x.CurrentHealth).Reverse().First();
            _currentHealth2 = _combatData.StartingHealthTotals[opponentCombatant];
            _maxHealth2 = opponentCombatant.MaxHealth;
            _cryptidDisplay2.SetCryptid(opponentCombatant, _locDatabase, _logger, false);
            _cryptidHealth2.Maximum = _maxHealth2;
            _cryptidHealth2.Value = _currentHealth2;
            _cryptidDamageIndicator2.Visibility = Visibility.Hidden;
            _cryptidDamageIndicatorLabel2.Text = string.Empty;
            _cryptidDamageIndicatorLabel2.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void OnRenderTick(object sender, EventArgs e)
        {
            SolidColorBrush BrushFromDamageEvent(CombatEngine.CombatEvent dmgEvent)
            {
                return new SolidColorBrush(
                    dmgEvent.IsCrit
                        ? Colors.MediumVioletRed
                        : dmgEvent.Damage == 0
                            ? Colors.Gray
                            : Colors.DarkRed)
                {
                    Opacity = 1d
                };
            }

            void PlayAudioFromDamageEvent(CombatEngine.CombatEvent dmgEvent, AudioManager audioManager)
            {
                audioManager.PlaySound(
                    dmgEvent.IsCrit
                        ? AudioManager.AudioEvent.CriticalHit
                        : dmgEvent.Damage == 0
                            ? AudioManager.AudioEvent.Miss
                            : AudioManager.AudioEvent.Hit);
            }

            DateTime time = DateTime.UtcNow;
            double visibleTime = (time - _lastDisplayTime).TotalSeconds;
            if (visibleTime > _displayDelay)
            {
                _lastDisplayTime = time;
                _displayDelay = _random.NextDouble().Remap(0, 1, MIN_DISPLAY_TIME, MAX_DISPLAY_TIME);

                if (_combatEventsQueue.Count == 0)
                {
                    OnCombatEnded();
                    return;
                }

                CombatEngine.CombatEvent combatEvent = _combatEventsQueue.Dequeue();

                _activeHitBrush = BrushFromDamageEvent(combatEvent);
                PlayAudioFromDamageEvent(combatEvent, _audioManager);

                if (combatEvent.Target == _playerData.ActiveCryptid)
                {
                    _currentHealth1 -= combatEvent.Damage;

                    _cryptidHealth1.Value = _currentHealth1;
                    Canvas.SetLeft(_cryptidDamageIndicator1, _random.Next(130, 220));
                    Canvas.SetTop(_cryptidDamageIndicator1, _random.Next(230, 300));
                    _cryptidDamageIndicator1.Visibility = Visibility.Visible;
                    _cryptidDamageIndicatorLabel1.Text = combatEvent.Damage.ToString();
                    _cryptidDamageIndicatorLabel1.Foreground = _activeHitBrush;
                }
                else
                {
                    _currentHealth2 -= combatEvent.Damage;

                    _cryptidHealth2.Value = _currentHealth2;
                    Canvas.SetLeft(_cryptidDamageIndicator2, _random.Next(490, 570));
                    Canvas.SetTop(_cryptidDamageIndicator2, _random.Next(230, 300));
                    _cryptidDamageIndicator2.Visibility = Visibility.Visible;
                    _cryptidDamageIndicatorLabel2.Text = combatEvent.Damage.ToString();
                    _cryptidDamageIndicatorLabel2.Foreground = _activeHitBrush;
                }
            }
            else if (_activeHitBrush != null)
            {
                double opacity = 1d;
                if (visibleTime > _displayDelay * 0.5f)
                {
                    opacity = (_displayDelay - visibleTime) / _displayDelay * 0.5f;
                }

                _activeHitBrush.Opacity = opacity;
            }
        }

        private void OnSkipPressed()
        {
            CompositionTarget.Rendering -= OnRenderTick;
            _audioManager.PlaySound(AudioManager.AudioEvent.Click);
            _onSkipAction?.Invoke();
            _onSkipAction = null;
        }

        private void OnCombatEnded()
        {
            CompositionTarget.Rendering -= OnRenderTick;
            _audioManager.PlaySound(AudioManager.AudioEvent.Defeat);
            _onSkipAction?.Invoke();
            _onSkipAction = null;
        }
    }
}