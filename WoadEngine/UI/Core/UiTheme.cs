namespace WoadEngine.UI;

public sealed class UiTheme
{
    public UiStyle Default { get; } = new();
    public UiStyle Panel { get; } = new();
    public UiStyle Label { get; } = new();
    public UiStyle Button { get; } = new();
    public UiStyle ProgressBar { get; } = new();
}