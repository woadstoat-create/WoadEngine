namespace WoadEngine.Services
{
    public enum NotificationKind
    {
        Info,
        Warning,
        Success,
        Achievement
    }

    public struct NotificationRequest
    {
        public string Title;
        public string Message;
        public float DurationSeconds;
        public NotificationKind Kind;
    }

    public static class NotificationService
    {
        private static readonly Queue<NotificationRequest> _queue = new();

        public static void Enqueue(
            string title,
            string message,
            float durationSeconds = 3f,
            NotificationKind kind = NotificationKind.Info)
        {
            _queue.Enqueue(new NotificationRequest
            {
                Title = title,
                Message = message,
                DurationSeconds = durationSeconds,
                Kind = kind
            });
        }
        
        public static bool TryDequeue(out NotificationRequest req)
        {
            if (_queue.Count > 0)
            {
                req = _queue.Dequeue();
                return true;
            }

            req = default;
            return false;
        }

        public static bool HasPending => _queue.Count > 0;
    }
}