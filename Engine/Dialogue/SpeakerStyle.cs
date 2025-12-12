using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WoadEngine.Dialogue
{
    public class SpeakerStyle
    {
        public required string SpeakerId;
        public required string DisplayName;
        public Color NameColor = Color.White;
        public Texture2D? Portrait;
    }
}