namespace WoadEngine.ECS
{
    public sealed class AudioSystem : IUpdateSystem
    {
        private readonly Queue<SoundRequest> _queue = new();

        public void Play(
            SoundEffect effect, 
            float volume = 1f,
            float pitch = 0f,
            float pan = 0f)
        {
            if (effect == null)
                return;
            
            _queue.Enqueue(new SoundRequest
            {
                Effect = effect, 
                Volume = volume,
                Pitch = pitch,
                Pan = pan
            });
        }

        public void Update(GameTime gameTime, IReadOnlyList<Entity> entities)
        {
            while(_queue.Count > 0)
            {
                var req = _queue.Dequeue();
                req.Effect.Play(req.Volume, req.Pitch, req.Pan);
            }
        }

        private struct SoundRequest
        {
            public SoundEffect Effect;
            public float Volume;
            public float Pitch;
            public float Pan;
        }
        
    }
}