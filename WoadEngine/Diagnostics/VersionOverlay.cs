using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WoadEngine.Diagnostics;

// REMINDER: MAJOR.MINOR.PATCH.MINOR_PATCH

public sealed class VersionOverlay
{
    private SpriteFont? _font;
    public bool Enabled { get; set; } = false;
    public Vector2 Position { get; set; } = new(10, 10);
    public Color Color { get; set; } = Color.White;
    public string EngineVersion { get; }
    public string GameVersion { get; private set; } = "";

    public VersionOverlay()
    {
        EngineVersion = GetAssemblyInformationalVersion(typeof(VersionOverlay).Assembly)
                        ?? typeof(VersionOverlay).Assembly.GetName().Version?.ToString()
                        ?? "unknown";
    }

    public void LoadContent(SpriteFont font)
    {
        _font = font;
    }

    public void SetGameVersion(string? version)
    {
        GameVersion = version ?? "";
    }

    public void Draw()
    {
        if (!Enabled) return;
        if (_font is null) return;

        string text = string.IsNullOrWhiteSpace(GameVersion)
            ? $"WoadEngine {EngineVersion}"
            : $"V.{GameVersion}";

        Position = new Vector2(5, Core.Graphics.PreferredBackBufferHeight - 10);
        var sb = Core.SpriteBatch;
        sb.Begin(samplerState: SamplerState.PointClamp);
        sb.DrawString(_font, text, Position, Color);
        sb.End();
    }

    private static string? GetAssemblyInformationalVersion(Assembly asm)
    {
        return asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
    }
}