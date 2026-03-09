namespace WoadEngine.Persistence;

public interface ISavePathProvider
{
    string GetSaveDirectory(string gameId);
    string GetSavePath(string gameId, string slotName);
}