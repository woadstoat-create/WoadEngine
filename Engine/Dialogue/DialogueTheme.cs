using Microsoft.Xna.Framework;

namespace WoadEngine.Dialogue
{
    public class DialogueTheme
    {
        public Dictionary<string, SpeakerStyle> Speakers { get; } = new();
        public SpeakerStyle GetStyle(string speakerId)
        {
            if (Speakers.TryGetValue(speakerId, out var style))
                return style;

            return new SpeakerStyle
            {
                SpeakerId = speakerId,
                DisplayName = speakerId,
                NameColor = Color.White,
                Portrait = null
            };
        }
    }
}