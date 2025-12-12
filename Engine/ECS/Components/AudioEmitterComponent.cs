using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace WoadEngine.ECS
{
    public sealed class AudioEmitterComponent : IComponent
    {
        public Dictionary<string, SoundEffect> Sounds { get; } = new();

        /// <summary>Default volume for sounds from this emitter (0–1).</summary>
        public float Volume = 1f;

        /// <summary>Default pitch offset (-1 .. 1).</summary>
        public float Pitch = 0f;

        /// <summary>Default stereo pan (-1 .. 1, left..right).</summary>
        public float Pan = 0f;

        /// <summary>
        /// Try to get a sound by key.
        /// </summary>
        public bool TryGetSound(string key, out SoundEffect? sound)
        {
            return Sounds.TryGetValue(key, out sound);
        }

        /// <summary>
        /// Add or replace a sound with the given key.
        /// </summary>
        public void SetSound(string key, SoundEffect sound)
        {
            if (sound == null)
                return;

            Sounds[key] = sound;
        }
    }
}