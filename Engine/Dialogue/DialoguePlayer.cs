namespace WoadEngine.Dialogue
{
    public enum DialoguePlayerState
    {
        Idle,
        Playing,
        Finished    
    }

    public class DialoguePlayer
    {
        private readonly IDictionary<string, DialogueSequence> _sequences;
        private readonly ChatLog _chatLog;

        public DialoguePlayerState State { get; private set;} = DialoguePlayerState.Idle;
        public string CurrentSequenceId { get; private set; }
        public int CurrentIndex { get; private set; }

        public bool AutoPushToChatLog { get; set; } = true;

        public DialoguePlayer(
            IDictionary<string, DialogueSequence> sequences,
            ChatLog chatLog)
        {
            _sequences = sequences;
            _chatLog = chatLog;
        }

        public void Start(string sequenceId)
        {
            if (!_sequences.ContainsKey(sequenceId))
            {
                throw new ArgumentException($"Dialogue sequence '{sequenceId}' not found");
            }

            CurrentSequenceId = sequenceId;
            CurrentIndex = 0;
            State = DialoguePlayerState.Playing;

            PushCurrentLine();
        }

        public void Advance()
        {
            if (State != DialoguePlayerState.Playing)
                return;

            var seq = _sequences[CurrentSequenceId];
            CurrentIndex++;

            if (CurrentIndex >= seq.Lines.Count)
            {
                State = DialoguePlayerState.Finished;
                return;
            }

            PushCurrentLine();
        }

        public DialogueLine GetCurrentLine()
        {
            if (State != DialoguePlayerState.Playing) return null;
            return _sequences[CurrentSequenceId].Lines[CurrentIndex];
        }

        private void PushCurrentLine()
        {
            if (!AutoPushToChatLog) return;
            var line = GetCurrentLine();
            if (line != null)
                _chatLog.AddMessage(line);
        }

        public void Reset()
        {
            State = DialoguePlayerState.Idle;
            CurrentSequenceId = null;
            CurrentIndex = 0;
        }
    } 
}