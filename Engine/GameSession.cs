namespace WoadEngine.Profiles
{
    public sealed class GameSession<TProfile>
        where TProfile : class
    {
        public GameSession(TProfile profile)
        {
            Profile = profile ?? throw new ArgumentNullException(nameof(profile));
            SessionId = Guid.NewGuid();
            StartedUtc = DateTimeOffset.UtcNow;
        }

        public Guid SessionId { get; }
        public DateTimeOffset StartedUtc { get; }

        public TProfile Profile { get; private set; }

        public string? SaveSlot { get; set; }
        public string? Difficulty { get; set; }

        public void SwapProfile(TProfile newProfile)
            => Profile = newProfile ?? throw new ArgumentNullException(nameof(newProfile));
    }
}