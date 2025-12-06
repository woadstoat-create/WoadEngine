using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace WoadEngine.ECS
{
    public sealed class MusicSystem : IUpdateSystem
    {
        private Song _currentSong;
        private float _targetVolume = 1f;
        private float _fadeSpeed = 0f;
        private bool _isFadingOut = false;

        private readonly List<Song> _queue = new();
        private int _currentIndex = -1;
        private bool _loopQueue = true;
        private bool _autoAdvance = true;

        public bool IsPlaying => MediaPlayer.State == MediaState.Playing;
        public float Volume
        {
            get => MediaPlayer.Volume;
            set => MediaPlayer.Volume = MathHelper.Clamp(value, 0f, 1f);
        }

        public void Play(Song song, bool loop = true, float volume = 1f)
        {
            if (song == null)
                return;

            _queue.Clear();
            _currentIndex = -1;

            _currentSong = song;
            MediaPlayer.IsRepeating = loop;
            MediaPlayer.Volume = MathHelper.Clamp(volume, 0f, 1f);
            MediaPlayer.Play(song);

            _fadeSpeed = 0f;
            _isFadingOut = false;
        }

        public void Stop()
        {
            MediaPlayer.Stop();
            _currentSong = null; 
            _fadeSpeed = 0f;
            _isFadingOut = false;
        }

        public void Pause() => MediaPlayer.Pause();
        public void Resume() => MediaPlayer.Resume();

        public void FadeOut(float durationSeconds)
        {
            if (durationSeconds <= 0f) { Stop(); return; }
            _isFadingOut = true;
            _fadeSpeed = Volume / durationSeconds;
        }

        /// <summary>
        /// Replace the current playlist with these songs and start from the first.
        /// </summary>
        public void SetQueue(IEnumerable<Song> songs, bool loopQueue = true, float volume = 1f)
        {
            _queue.Clear();
            if (songs != null)
                _queue.AddRange(songs);

            _loopQueue = loopQueue;
            _autoAdvance = true;

            if (_queue.Count == 0)
            {
                Stop();
                return;
            }

            _currentIndex = 0;
            PlayCurrentFromQueue(volume);
        }

        /// <summary>
        /// Add a song to the end of the current queue (does not restart playback).
        /// </summary>
        public void Enqueue(Song song)
        {
            if (song == null)
                return;

            _queue.Add(song);

            // If nothing is playing and we want auto-advance, start this one
            if (_currentSong == null && _autoAdvance && _queue.Count == 1)
            {
                _currentIndex = 0;
                PlayCurrentFromQueue(Volume <= 0f ? 1f : Volume);
            }
        }

        /// <summary>Skip to next song in the queue.</summary>
        public void Next()
        {
            if (_queue.Count == 0)
                return;

            if (_currentIndex < 0)
                _currentIndex = 0;
            else
                _currentIndex++;

            if (_currentIndex >= _queue.Count)
            {
                if (_loopQueue)
                    _currentIndex = 0;
                else
                {
                    Stop();
                    return;
                }
            }

            PlayCurrentFromQueue(Volume <= 0f ? 1f : Volume);
        }

        /// <summary>Skip to previous song in the queue.</summary>
        public void Previous()
        {
            if (_queue.Count == 0)
                return;

            if (_currentIndex <= 0)
            {
                if (_loopQueue)
                    _currentIndex = _queue.Count - 1;
                else
                    _currentIndex = 0;
            }
            else
            {
                _currentIndex--;
            }

            PlayCurrentFromQueue(Volume <= 0f ? 1f : Volume);
        }

        private void PlayCurrentFromQueue(float volume)
        {
            if (_currentIndex < 0 || _currentIndex >= _queue.Count)
                return;

            var song = _queue[_currentIndex];
            _currentSong = song;

            MediaPlayer.IsRepeating = false; // queue handles repetition
            Volume = volume;
            MediaPlayer.Play(song);

            _targetVolume = Volume;
            _fadeSpeed = 0f;
            _isFadingOut = false;
        }

        // === UPDATE ===========================================================

        public void Update(GameTime gameTime, System.Collections.Generic.IReadOnlyList<Entity> entities)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Handle fade-out
            if (_isFadingOut && _currentSong != null)
            {
                Volume -= _fadeSpeed * dt;
                if (Volume <= 0.01f)
                {
                    Stop();
                }
            }

            // Auto-advance the queue when a song finishes normally
            if (_autoAdvance &&
                !_isFadingOut &&
                _queue.Count > 0 &&
                MediaPlayer.State == MediaState.Stopped)
            {
                // current song finished naturally
                Next();
            }
        }
    }
}