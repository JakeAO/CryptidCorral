using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace AppWPF.Code
{
    public class AudioManager
    {
        public enum AudioEvent
        {
            Click = 0,
            Hit = 1,
            CriticalHit = 2,
            Miss = 3,
            Defeat = 4
        }

        private readonly Logger _logger = null;
        private readonly Random _random = new Random();
        private readonly Queue<MediaPlayer> _mediaPlayerPool = new Queue<MediaPlayer>(10);
        private readonly List<MediaPlayer> _activeMediaPlayers = new List<MediaPlayer>(10);
        private readonly IReadOnlyDictionary<AudioEvent, IReadOnlyList<Uri>> _knownAudioUris = null;

        private float _volume = 0.5f;

        public float Volume
        {
            get => _volume;
            set => _volume = Math.Min(Math.Max(value, 0), 1);
        }

        public AudioManager(Logger logger, params (AudioEvent, IReadOnlyList<Uri>)[] knownAudioUris)
        {
            _logger = logger;
            _knownAudioUris = knownAudioUris.ToDictionary(x => x.Item1, x => x.Item2);
        }

        public void PlaySound(AudioEvent audioEvent)
        {
            if (_knownAudioUris.TryGetValue(audioEvent, out IReadOnlyList<Uri> uriList))
            {
                int index = _random.Next(uriList.Count);
                PlaySound(uriList[index]);
            }
        }

        public void PlaySound(Uri uri)
        {
            MediaPlayer mediaPlayer = _mediaPlayerPool.Count > 0
                ? _mediaPlayerPool.Dequeue()
                : new MediaPlayer();

            mediaPlayer.MediaEnded += OnMediaEnded;
            mediaPlayer.MediaFailed += OnMediaFailed;

            _activeMediaPlayers.Add(mediaPlayer);

            mediaPlayer.Position = TimeSpan.Zero;
            mediaPlayer.Volume = Volume;
            mediaPlayer.Open(uri);
            mediaPlayer.Play();
        }

        private void OnMediaEnded(object sender, EventArgs e)
        {
            if (sender is MediaPlayer mediaPlayer)
            {
                PoolMediaPlayer(mediaPlayer);
            }
        }

        private void OnMediaFailed(object sender, ExceptionEventArgs e)
        {
            if (sender is MediaPlayer mediaPlayer)
            {
                _logger?.Log(Logger.LogLevel.Exception, $"{e.ErrorException.GetType().Name}: {e.ErrorException.Message} ({mediaPlayer.Source})");
                PoolMediaPlayer(mediaPlayer);
            }
        }

        private void PoolMediaPlayer(MediaPlayer mediaPlayer)
        {
            mediaPlayer.MediaEnded -= OnMediaEnded;
            mediaPlayer.MediaFailed -= OnMediaFailed;
            mediaPlayer.Stop();
            mediaPlayer.Close();
            mediaPlayer.Position = TimeSpan.Zero;
            _activeMediaPlayers.Remove(mediaPlayer);
            _mediaPlayerPool.Enqueue(mediaPlayer);
        }
    }
}