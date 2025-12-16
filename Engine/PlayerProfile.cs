using System;
using System.Collections.Generic;

namespace WoadEngine.Profiles
{
    public sealed class PlayerProfile<TAchievementId, TTutorialId, TStatId>
        where TAchievementId : struct, Enum
        where TTutorialId : struct, Enum
        where TStatId : struct, Enum
    {
        public string PlayerName { get; set; } = "Player";

        public int HighScore { get; set; } = 0;

        public int HighestLevelUnlocked { get; set; } = 0;

        public int TotalGamesPlayed { get; set; } = 0;

        public Dictionary<TStatId, int> Stats { get; set; } = new();

        public HashSet<TAchievementId> Achievements { get; set; } = new();

        public HashSet<TTutorialId> CompletedTutorials { get; set; } = new();

        public bool HasSeenTutorial(TTutorialId id) => CompletedTutorials.Contains(id);

        public bool MarkTutorialSeen(TTutorialId id) => CompletedTutorials.Add(id);

        public bool HasAchievement(TAchievementId id) => Achievements.Contains(id);

        public bool UnlockAchievement(TAchievementId id) => Achievements.Add(id);

        public int GetStat(TStatId id) => Stats.TryGetValue(id, out var v) ? v : 0;

        public void SetStat(TStatId id, int value) => Stats[id] = value;

        public int IncrementStat(TStatId id, int delta = 1)
        {
            var next = checked(GetStat(id) + delta);
            Stats[id] = next;
            return next;
        }
    }
}