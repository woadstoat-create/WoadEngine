using System;
using System.Text.Json;
using WoadEngine.Diagnostics;

namespace WoadEngine.Persistence;

public sealed class JsonSaveSerializer : ISaveSerializer
{
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    public string Serialize<T>(T data)
    {
        return JsonSerializer.Serialize(data, _options);
    }

    public T Deserialize<T>(string text)
    {
        var result = JsonSerializer.Deserialize<T>(text, _options);
        if (result is null)
            Logger.Exception(new InvalidOperationException("Failed to deserialize save data."));

        return result;
    }
}