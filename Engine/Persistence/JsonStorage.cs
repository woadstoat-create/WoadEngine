using System.Text.Json;
using System.Text.Json.Serialization;

namespace WoadEngine.Persistence
{
    public static class JsonStorage
    {
        private static readonly JsonSerializerOptions DefaultOptions = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public static T LoadOrDefualt<T>(string path, Func<T> factory, JsonSerializerOptions? options = null)
        {
            if (factory is null) throw new ArgumentNullException(nameof(factory));
            options ??= DefaultOptions;

            try
            {
                if (!File.Exists(path)) return factory();

                var json = File.ReadAllText(path);
                var obj = JsonSerializer.Deserialize<T>(json, options);
                return obj is null ? factory() : obj;
            }
            catch
            {
                return factory();
            }
        }

        public static bool TrySave<T>(string path, T value, JsonSerializerOptions options = null)
        {
            options ??= DefaultOptions;

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path) ?? ".");
                var json = JsonSerializer.Serialize(value, options);
                File.WriteAllText(path, json);
                return true;
            }   
            catch
            {
                return false;    
            }
        }
    }
}