using System.Text.Json;

namespace WoadEngine.Dialogue
{
    public static class DialogueLoader
    {
        public static Dictionary<string, DialogueSequence> LoadFromJson(string path)
        {
            string json = File.ReadAllText(path);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var file = JsonSerializer.Deserialize<DialogueFile>(json, options);
            var dict = new Dictionary<string, DialogueSequence>();

            if (file != null)
            {
                foreach (var seq in file.Sequences)
                {
                    if (!string.IsNullOrEmpty(seq.Id))
                    {
                        dict[seq.Id] = seq;
                    }
                }
            }

            return dict;
        }
    }
}