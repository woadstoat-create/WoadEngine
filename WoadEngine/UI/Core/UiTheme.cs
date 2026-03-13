namespace WoadEngine.UI;

public sealed class UiTheme
{
    public UiStyle Default { get; set; } = new();
    public UiStyle Panel { get; set; } = new();
    public UiStyle Label { get; set; } = new();
    public UiStyle Button { get; set; } = new();
    public UiStyle ProgressBar { get; set; } = new();
    public UiStyle Slider { get; set; } = new();
}