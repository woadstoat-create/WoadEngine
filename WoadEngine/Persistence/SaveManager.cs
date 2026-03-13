using System;
using System.IO;

namespace WoadEngine.Persistence;

public sealed class SaveManager
{
    private readonly ISaveSerializer _serializer;
    private readonly ISavePathProvider _pathProvider;

    public SaveManager(ISaveSerializer serializer, ISavePathProvider pathProvider)
    {
        _serializer = serializer;
        _pathProvider = pathProvider;
    }

    public void Save<T>(string gameId, string slotName, int version, T data)
    {
        var path = _pathProvider.GetSavePath(gameId, slotName);
        var directory = Path.GetDirectoryName(path);

        if (string.IsNullOrWhiteSpace(directory))
            throw new InvalidOperationException("Save path directory was invalid.");

        Directory.CreateDirectory(directory);

        SaveFile<T>? existing = null;
        if (File.Exists(path))
        {
            try
            {
                existing = _serializer.Deserialize<SaveFile<T>>(File.ReadAllText(path));
            }
            catch
            {
                // Ignore broken/old save when overwriting.
            }
        }

        var saveFile = new SaveFile<T>
        {
            GameId = gameId,
            Version = version,
            CreatedUtc = existing?.CreatedUtc ?? DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow,
            Data = data
        };

        var text = _serializer.Serialize(saveFile);
        File.WriteAllText(path, text);
    }

    public SaveFile<T>? Load<T>(string gameId, string slotName)
    {
        var path = _pathProvider.GetSavePath(gameId, slotName);
        if (!File.Exists(path))
            return null;

        var text = File.ReadAllText(path);
        return _serializer.Deserialize<SaveFile<T>>(text);
    }

    public bool Exists(string gameId, string slotName)
    {
        var path = _pathProvider.GetSavePath(gameId, slotName);
        return File.Exists(path);
    }

    public void Delete(string gameId, string slotName)
    {
        var path = _pathProvider.GetSavePath(gameId, slotName);
        if (File.Exists(path))
            File.Delete(path);
    }

    public string GetPath(string gameId, string slotName)
    {
        return _pathProvider.GetSavePath(gameId, slotName);
    }
}