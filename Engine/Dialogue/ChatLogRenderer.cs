using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WoadEngine.Dialogue
{
    public class ChatLogRenderer
    {
        private readonly ChatLog _chatLog;
        private readonly DialogueTheme _theme;
        private readonly SpriteFont _font;
        private readonly Texture2D _whitePixel;

        public int LineSpacing = 4;

        public ChatLogRenderer(
            ChatLog chatLog,
            DialogueTheme theme,
            SpriteFont font,
            Texture2D whitePixel)
        {
            _chatLog = chatLog;
            _theme = theme;
            _font = font;
            _whitePixel = whitePixel;
        }

        public void Draw(SpriteBatch sb, Rectangle panelRect)
        {
            sb.Draw(_whitePixel, panelRect, Color.Black * 0.7f);

            var messages = _chatLog.GetMessages();
            if (messages.Count == 0) return;

            float y = panelRect.Y + 10;
            float x = panelRect.X = 10;
            float maxWidth = panelRect.Width - 20;

            foreach (var msg in messages)
            {
                var style = _theme.GetStyle(msg.SpeakerId);

                var nameText = style.DisplayName ?? msg.SpeakerId;
                sb.DrawString(_font, nameText, new Vector2(x, y), style.NameColor);
                y += _font.LineSpacing;

                y = DrawWrappedText(sb, msg.Text, new Vector2(x, y), maxWidth, Color.White);

                y += LineSpacing;
                
                if (y>panelRect.Bottom) break;
            }
        }

        private float DrawWrappedText(
            SpriteBatch sb,
            string text,
            Vector2 pos, 
            float maxWidth, 
            Color color)
        {
             // Very simple word-wrap (can be improved later)
            var words = text.Split(' ');
            string line = string.Empty;
            float y = pos.Y;

            foreach (var word in words)
            {
                string testLine = string.IsNullOrEmpty(line) ? word : line + " " + word;
                var size = _font.MeasureString(testLine);
                if (size.X > maxWidth)
                {
                    sb.DrawString(_font, line, new Vector2(pos.X, y), color);
                    y += _font.LineSpacing;
                    line = word;
                }
                else
                {
                    line = testLine;
                }
            }

            if (!string.IsNullOrEmpty(line))
            {
                sb.DrawString(_font, line, new Vector2(pos.X, y), color);
                y += _font.LineSpacing;
            }

            return y;
        }
    }
}