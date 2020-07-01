using System.Collections.Generic;
using Core.Code.Cryptid;
using Core.Code.StateMachine.Interactions;
using Core.Code.StateMachine.Signals;
using Core.Code.Utility;

namespace Core.Code.StateMachine.States
{
    public class TownLabState : IState
    {
        public string LocationLocId => "Location/Town/Laboratory";
        public string TimeLocId => "Time/Day";

        private Context _sharedContext;
        private PlayerData _playerData;

        public void PerformSetup(Context context, IState previousState)
        {
            _sharedContext = context;
            _playerData = context.Get<PlayerData>();
        }

        public void PerformContent(Context context)
        {
            MainPrompt();
        }

        public void PerformTeardown(Context context, IState nextState)
        {
        }

        private void MainPrompt()
        {
            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new Dialog("Town/Lab/Welcome"),
                new Option("Button/Freeze", OnFreezeSelected),
                new Option("Button/Thaw", OnThawSelected),
                new Option("Button/Retire", OnRetireSelected),
                new Option("Button/Fuse", OnFuseSelected),
                new Option("Button/Exit", OnExitSelected)
            });
        }

        private void OnFreezeSelected()
        {
            if (_playerData.ActiveCryptid != null)
            {
                Cryptid.Cryptid frozenCryptid = _playerData.ActiveCryptid;
                _playerData.FrozenCryptedInventory.Add(frozenCryptid);
                _playerData.ActiveCryptid = null;

                _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
                {
                    new UpdatePlayerData(_playerData), 
                    new Dialog("Town/Lab/FreezeResult", new KeyValuePair<string, string>("{species}", _sharedContext.Get<LocDatabase>().Localize(frozenCryptid.Species.NameId))),
                    new Option("Button/Okay", MainPrompt),
                });
            }
            else
            {
                _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
                {
                    new Dialog("Town/Lab/FreezeFail"),
                    new Option("Button/Okay", MainPrompt),
                });
            }
        }

        private void OnThawSelected()
        {
            if (_playerData.ActiveCryptid == null)
            {
                _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
                {
                    new Dialog("Town/Lab/ThawPrompt"),
                    new CryptidSelection(_playerData.FrozenCryptedInventory, OnThawSelected_CryptidSelected),
                    new Option("Button/Cancel", MainPrompt),
                });
            }
            else
            {
                _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
                {
                    new Dialog("Town/Lab/ThawFail"),
                    new Option("Button/Okay", MainPrompt),
                });
            }
        }

        private void OnThawSelected_CryptidSelected(Cryptid.Cryptid cryptid)
        {
            _playerData.ActiveCryptid = cryptid;
            _playerData.FrozenCryptedInventory.Remove(cryptid);

            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new UpdatePlayerData(_playerData), 
                new Dialog("Town/Lab/ThawResult", new KeyValuePair<string, string>("{species}", _sharedContext.Get<LocDatabase>().Localize(cryptid.Species.NameId))),
                new Option("Button/Okay", MainPrompt),
            });
        }

        private void OnRetireSelected()
        {
            if (_playerData.ActiveCryptid != null)
            {
                _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
                {
                    new Dialog("Town/Lab/RetirePrompt"),
                    new Option("Button/Confirm", OnRetireSelected_ConfirmSelected),
                    new Option("Button/Cancel", MainPrompt),
                });
            }
            else
            {
                _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
                {
                    new Dialog("Town/Lab/RetireFail"),
                    new Option("Button/Okay", MainPrompt),
                });
            }
        }

        private void OnRetireSelected_ConfirmSelected()
        {
            Cryptid.Cryptid cryptid = _playerData.ActiveCryptid;
            _playerData.ActiveCryptid = null;
            _playerData.DnaSampleInventory.Add(new CryptidDnaSample(cryptid));

            _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
            {
                new UpdatePlayerData(_playerData),
                new Dialog("Town/Lab/RetireResult"),
                new Option("Button/Okay", MainPrompt),
            });
        }

        private void OnFuseSelected()
        {
            if (_playerData.DnaSampleInventory.Count < 2)
            {
                _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
                {
                    new Dialog("Town/Lab/FuseFailTooFewSamples"),
                    new Option("Button/Okay", MainPrompt),
                });
            }
            else if (_playerData.ActiveCryptid != null)
            {
                _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
                {
                    new Dialog("Town/Lab/FuseFailCorralFull"),
                    new Option("Button/Okay", MainPrompt),
                });
            }
            else
            {
                void OnFirstSampleSelected(CryptidDnaSample sample1)
                {
                    var modifiedInventory = new List<CryptidDnaSample>(_playerData.DnaSampleInventory);
                    modifiedInventory.Remove(sample1);
                    _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
                    {
                        new Dialog("Town/Lab/FusePrompt2"),
                        new DnaSampleSelection(modifiedInventory, sample2 => OnSecondSampleSelected(sample1, sample2)),
                        new Option("Button/Cancel", MainPrompt),
                    });
                }

                void OnSecondSampleSelected(CryptidDnaSample sample1, CryptidDnaSample sample2)
                {
                    _playerData.DnaSampleInventory.Remove(sample1);
                    _playerData.DnaSampleInventory.Remove(sample2);

                    Cryptid.Cryptid newCryptid = CryptidUtilities.CreateCryptid(
                        sample1, sample2,
                        _sharedContext.Get<SpeciesDatabase>(),
                        _sharedContext.Get<ColorDatabase>());

                    _playerData.ActiveCryptid = newCryptid;

                    _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
                    {
                        new UpdatePlayerData(_playerData), 
                        new Dialog("Town/Lab/FuseResult", new KeyValuePair<string, string>("{species}", _sharedContext.Get<LocDatabase>().Localize(newCryptid.Species.NameId))),
                        new Option("Button/Okay", MainPrompt),
                    });
                }

                _sharedContext.Get<InteractionEventRaised>().Fire(new IInteraction[]
                {
                    new Dialog("Town/Lab/FusePrompt1"),
                    new DnaSampleSelection(_playerData.DnaSampleInventory, OnFirstSampleSelected),
                    new Option("Button/Cancel", MainPrompt),
                });
            }
        }

        private void OnExitSelected()
        {
            _sharedContext.Get<StateMachine>().ChangeState<TownMainState>();
        }
    }
}