using System;
using System.IO;

namespace WoadEngine.Persistence;

public sealed class DefaultSavePathProvider : ISavePathProvider
{
    public string GetSaveDirectory(string gameId)
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "WoadStoat",
            gameId,
            "Saves");
    }

    public string GetSavePath(string gameId, string slotName)
    {
        return Path.Combine(GetSaveDirectory(gameId), $"{slotName}.json");
    }
}