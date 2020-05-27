using System;
using System.IO;
using Newtonsoft.Json;
using UAT_MS539.Core.Code.Extensions;

namespace UAT_MS539.Core.Code.StateMachine.States
{
    public class SplashState : IState
    {
        private Context _sharedContext;

        public bool SaveFound { get; private set; } = false;

        public void PerformSetup(Context context, IState previousState)
        {
            _sharedContext = context;
        }

        public void PerformContent(Context context)
        {
            PlayerData saveData = null;
            try
            {
                string saveJson = File.ReadAllText(PlayerData.SavePath);
                saveData = JsonConvert.DeserializeObject<PlayerData>(saveJson, JsonExtensions.DefaultSettings);
                SaveFound = true;
            }
            catch (Exception e)
            {
                saveData = new PlayerData();
                SaveFound = false;
            }
            finally
            {
                context.Set(saveData);
                Continue();
            }
        }

        public void Continue()
        {
            if (SaveFound)
            {
                _sharedContext.Get<StateMachine>().ChangeState<ReturningPlayerState>();
            }
            else
            {
                _sharedContext.Get<StateMachine>().ChangeState<TutorialCorral>();
            }
        }
    }
}