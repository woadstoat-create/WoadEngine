using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace WoadEngine.ECS
{
    public sealed class MusicSystem : IUpdateSystem
    {
        private Song _currentSong;
        private float _targetVolume = 1f;
        private float _fadeSpeed = 0f;
        private bool _isFadingOut = false;

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
            _fadeSpeed = volume / durationSeconds;
        }

        public void Update(GameTime gameTime, IReadOnlyList<Entity> entities)
        {
            if (!_isFadingOut || _currentSong == null)
                continue;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Volume -= _fadeSpeed * dt;

            if (Volume <= 0.01f)
            {
                Stop();
            }
        }
    }
}