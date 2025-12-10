namespace WoadEngine.Dialogue
{
    public class ChatLog
    {
        private readonly Queue<DialogueLine> _messages = new();
        public int MaxMessages { get; set; } = 30;

        public void AddMessage(DialogueLine line)
        {
            _messages.Enqueue(line);
            while (_messages.Count > MaxMessages)
                _messages.Dequeue();
        }

        public IReadOnlyList<DialogueLine> GetMessages()
        {
            return _messages.ToList();
        }

        public void Clear() => _messages.Clear();
    }
}