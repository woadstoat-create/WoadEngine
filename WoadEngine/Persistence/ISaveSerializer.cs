namespace WoadEngine.Persistence;

public interface ISaveSerializer
{
    string Serialize<T>(T data);
    T Deserialize<T>(string text);
}