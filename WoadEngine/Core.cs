// --------------------------------------------------------------------------------------------
// WoadEngine
// Copyright (c) 2026 Woad Stoat Studios, All Rights Reserved.
// This source code may not be copied, modified, disclosed or distributed without permission.
//---------------------------------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WoadEngine.Scenes;
using WoadEngine.Input;
using WoadEngine.Audio;
using System.Security.Cryptography;
using WoadEngine.Diagnostics;

namespace WoadEngine;

public class Core : Game
{
    internal static Core s_instance;

    public static Core Instance => s_instance;

    private static Scene s_activeScene;
    private static Scene s_nextScene;
    public static GraphicsDeviceManager Graphics { get; private set; }
    public static new GraphicsDevice GraphicsDevice { get; private set; }
    public static SpriteBatch SpriteBatch { get; private set; }
    public static new ContentManager Content { get; private set; }
    public static InputManager Input { get; private set; }
    public static bool ExitOnEscape { get; set; }
    public static AudioController Audio { get; private set; }
    public static VersionOverlay VersionOverlay { get; private set; }

    private enum TransitionPhase
    {
        None, 
        FadingOut,
        Switching,
        FadingIn
    }

    private static TransitionPhase s_transitionPhase = TransitionPhase.None;
    private static float s_fadeOutDuration = 0.25f;
    private static float s_fadeInDuration = 0.25f;

    private static float s_transitionTimer = 0f;
    private static float s_fadeAlpha = 0f;

    private static Texture2D s_fadePixel;

    public Core(string title, int width, int height, bool fullScreen)
    {
        if (s_instance != null)
        {
            throw new InvalidOperationException($"Only a single Core instance can be created");
        }

        s_instance = this;

        Graphics = new GraphicsDeviceManager(this);

        Graphics.PreferredBackBufferWidth = width;
        Graphics.PreferredBackBufferHeight = height;
        Graphics.IsFullScreen = fullScreen;

        Graphics.ApplyChanges();

        Window.Title = title;

        Content = base.Content;

        Content.RootDirectory = "Content";

        IsMouseVisible = true;

        ExitOnEscape = true;

        SetFadeDurations(0.35f, 0.35f);
    }

    protected override void Initialize()
    {
        base.Initialize();

        GraphicsDevice = base.GraphicsDevice;
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        s_fadePixel = new Texture2D(GraphicsDevice, 1, 1);
        s_fadePixel.SetData(new[] { Color.White });

        Input = new InputManager();

        Audio = new AudioController();

        Logger.Init();
        Logger.Info("Game initializing...");
    }

    protected override void LoadContent()
    {
        base.LoadContent();
        VersionOverlay = new VersionOverlay();

    }

    protected override void UnloadContent()
    {
        Audio.Dispose();
        s_fadePixel?.Dispose();
        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        Input.Update(gameTime);
        Audio.Update();

        if (ExitOnEscape && Input.Keyboard.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        UpdateSceneTransition(gameTime);

        // if (s_nextScene != null)
        // {
        //     TransitionScene();
        // }

        if (s_activeScene != null && s_transitionPhase != TransitionPhase.Switching)
        {
            s_activeScene.Update(gameTime);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        if (s_activeScene != null)
        {
            s_activeScene.Draw(gameTime);
        }

        DrawFadeOverlay();

        base.Draw(gameTime);
        VersionOverlay?.Draw();
    }

    protected override void OnExiting(object sender, ExitingEventArgs args)
    {
        Logger.Info("Exiting...");
        Logger.Shutdown();
        base.OnExiting(sender, args);
    }

    public static void ChangeScene(Scene next)
    {
        if (s_activeScene == next)
            return;

        s_nextScene = next;

        if (s_activeScene == null)
        {
            TransitionScene();
            s_transitionPhase = TransitionPhase.FadingIn;
            s_transitionTimer = 0f;
            s_fadeAlpha = 1f;
            return;
        }

        if (s_transitionPhase == TransitionPhase.None)
        {
            s_transitionPhase = TransitionPhase.FadingOut;
            s_transitionTimer = 0f;
        }
        else if (s_transitionPhase == TransitionPhase.FadingIn)
        {
            s_transitionPhase = TransitionPhase.FadingOut;
            s_transitionTimer = 0f;
        }
    }

    private static void SetFadeDurations(float fadeOutSeconds, float fadeInSeconds)
    {
        s_fadeOutDuration = MathF.Max(0.001f, fadeOutSeconds);
        s_fadeInDuration = MathF.Max(0.001f, fadeInSeconds);
    }

    private static void UpdateSceneTransition(GameTime gameTime)
    {
        if (s_transitionPhase == TransitionPhase.None)
        {
            if (s_nextScene != null && s_activeScene != null)
            {
                s_transitionPhase = TransitionPhase.FadingOut;
                s_transitionTimer = 0f;
            }
            return;
        }

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        switch (s_transitionPhase)
        {
            case TransitionPhase.FadingOut:
            {
                s_transitionTimer += dt;
                float t = MathHelper.Clamp(s_transitionTimer / s_fadeOutDuration, 0f, 1f);
                s_fadeAlpha = t;

                if (t >= 1f)
                {
                    s_transitionPhase = TransitionPhase.Switching;
                    s_transitionTimer = 0f;

                    // Swap scenes when fully black
                    TransitionScene();

                    // Begin fading in
                    s_transitionPhase = TransitionPhase.FadingIn;
                    s_transitionTimer = 0f;
                }
                break;
            }

            case TransitionPhase.FadingIn:
            {
                s_transitionTimer += dt;
                float t = MathHelper.Clamp(s_transitionTimer / s_fadeInDuration, 0f, 1f);
                s_fadeAlpha = 1f - t;

                if (t >= 1f)
                {
                    s_fadeAlpha = 0f;
                    s_transitionPhase = TransitionPhase.None;
                }
                break;
            }

            case TransitionPhase.Switching:
            default:
                break;
        }
    }

    private static void DrawFadeOverlay()
    {
        if (s_fadePixel == null)
            return;

        if (s_fadeAlpha <= 0f)
            return;

        var vp = GraphicsDevice.Viewport;
        var rect = new Rectangle(0, 0, vp.Width, vp.Height);

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        SpriteBatch.Draw(s_fadePixel, rect, Color.Black * s_fadeAlpha);
        SpriteBatch.End();
    }

    private static void TransitionScene()
    {
        if (s_nextScene == null)
            return;

        if (s_activeScene != null)
            s_activeScene.Dispose();

        GC.Collect();

        s_activeScene = s_nextScene;
        s_nextScene = null;

        if (s_activeScene != null)
            s_activeScene.Initialize();
    }
}